global using System;
global using System.Reactive.Threading.Tasks;
global using System.Buffers;
global using Reactive.Bindings;
global using System.Reactive.Linq;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wahren.AbstractSyntaxTree.Parser;
using Wahren.AbstractSyntaxTree.Project;

namespace Wahren.GraphicalEditor.ViewModels;

public sealed class ProjectViewModel : IDisposable
{
    private Project? _project = null;
    public ReactiveProperty<Project?> Project { get; }
    public ReadOnlyReactivePropertySlim<string> Load { get; }

    public ProjectViewModel()
    {
        Project = new();
        Load = Project.Select(a => a is null ? "Default" : "Success").ToReadOnlyReactivePropertySlim("Default");
    }

    public void Dispose()
    {
        _project?.Dispose();
        _project = null;
        Project.Dispose();
        Load.Dispose();
    }

    private async Task<Project> LoadProjectAsync(string contentFolderPath)
    {
        this._project?.Dispose();
        this._project = new() { RequiredSeverity = DiagnosticSeverity.Error };
        var scriptFolderPath = Path.Combine(contentFolderPath, "script");
        var files = Directory.GetFiles(scriptFolderPath, "*.dat", SearchOption.AllDirectories);
        var commandLine = Environment.CommandLine;
        var isUnicode = commandLine.Contains(" -u");
        var isEnglish = commandLine.Contains(" -e");
        isUnicode |= isEnglish;
        var isSwitch = commandLine.Contains(" -switch") || commandLine.Contains(" --switch");
        this._project.Files.PrepareAddRange(files.Length);
        for (int i = 0; i < files.Length; i++)
        {
            this._project.Files.Add(new((uint)i));
            this._project.Files.Last.FilePath = files[i];
            this._project.FileAnalysisList.Add(new());
        }

        await ParallelLoadAndParseAsync(this._project, isSwitch, isUnicode, isEnglish, default).ConfigureAwait(false);
        return this._project;
    }

    private static async Task ParallelLoadAndParseAsync(Project project, bool @switch, bool isUnicode, bool isEnglish, CancellationToken token)
    {
        await Parallel.ForEachAsync(Enumerable.Range(0, project.Files.Count), token, async (int index, CancellationToken token) =>
        {
            var path = project.Files[index].FilePath;
            var (rental, actual) = await ReadFileUtility.ReadAsync(path!, token).ConfigureAwait(false);
            try
            {
                if (actual == 0)
                {
                    return;
                }
                token.ThrowIfCancellationRequested();
                LoadAndParse(project.RequiredSeverity, @switch, isUnicode, isEnglish, ref project.Files[index], project.FileAnalysisList[index], rental.AsSpan(0, actual));
            }
            finally
            {
                if (rental.Length != 0)
                {
                    ArrayPool<byte>.Shared.Return(rental);
                }
            }
        }).ConfigureAwait(false);
    }

    private static void LoadAndParse(DiagnosticSeverity severity, bool treatSlashPlusAsSingleLineComment, bool isUnicode, bool isEnglish, ref Result result, AnalysisResult analysisResult, Span<byte> input)
    {
        if (isUnicode)
        {
            FileLoader.UnicodeHandler.Load(input, out result.Source);
        }
        else
        {
            FileLoader.Cp932Handler.Load(input, out result.Source);
        }

        Context context = new(treatSlashPlusAsSingleLineComment, isEnglish, false, severity);
        result.Success = Parser.Parse(ref context, ref result);
        PerResultValidator.AddReferenceAndValidate(ref context, ref result, analysisResult);
        if (result.Success)
        {
            ref var errorList = ref result.ErrorList;
            for (uint i = 0, end = (uint)errorList.Count; i != end; i++)
            {
                if (errorList[i].Severity == DiagnosticSeverity.Error)
                {
                    result.Success = false;
                    break;
                }
            }
        }
    }
}
