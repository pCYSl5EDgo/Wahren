global using System;
global using System.Buffers;
global using System.IO;
global using System.Runtime.InteropServices;
global using System.Threading;
global using Wahren.AbstractSyntaxTree.Parser;
global using Wahren.FileLoader;
global using System.Diagnostics;
global using System.Text;

namespace Wahren.Compiler;

public partial class Program
{
    [Command(new string[] {
        "analyze",
    })]
    public async ValueTask<int> Analyze(
        [Option(0, "input folder")] string rootFolder = ".",
        [Option("switch")] bool @switch = false,
        [Option("s")] PseudoDiagnosticSeverity severity = PseudoDiagnosticSeverity.Error,
        [Option("t")] bool time = false
    )
    {
        var stopwatch = time ? Stopwatch.StartNew() : null;
        CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromMinutes(1));

        var debugPaper = await GetDebugPaper(rootFolder, cancellationTokenSource.Token).ConfigureAwait(false);
        var contentsFolder = debugPaper.Folder ?? DetectContentsFolder(rootFolder);
        if (contentsFolder is null)
        {
            Console.Error.WriteLine("Contents folder is not found.\n\nContents folder contains 'script'/'image'/'stage' folders.");
            return 1;
        }

        var scriptFolderPath = Path.GetRelativePath(Environment.CurrentDirectory, Path.GetFullPath(Path.Combine(contentsFolder, "script")));
        var (isUnicode, isEnglish) = await IsEnglish(scriptFolderPath).ConfigureAwait(false);
        var files = Directory.GetFiles(scriptFolderPath, "*.dat", SearchOption.AllDirectories);
        var solution = new Solution();
        try
        {
            solution.Files.PrepareAddRange(files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                solution.Files.Add(new(solution, (uint)i));
            }

            await Parallel.ForEachAsync(System.Linq.Enumerable.Range(0, files.Length), cancellationTokenSource.Token, async (int index, CancellationToken token) =>
            {
                byte[] rental;
                int actual;
                solution.Files[index].FilePath = Path.GetRelativePath(Environment.CurrentDirectory, files[index]);
                var handle = File.OpenHandle(files[index], FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.Asynchronous, 4096);
                try
                {
                    var length = RandomAccess.GetLength(handle);
                    if (length == 0)
                    {
                        return;
                    }

                    rental = ArrayPool<byte>.Shared.Rent((int)length);
                    try
                    {
                        actual = await RandomAccess.ReadAsync(handle, rental.AsMemory(0, (int)length), 0, token).ConfigureAwait(false);
                    }
                    catch
                    {
                        ArrayPool<byte>.Shared.Return(rental);
                        throw;
                    }
                }
                finally
                {
                    handle.Dispose();
                }

                try
                {
                    LoadAndParse((DiagnosticSeverity)(uint)severity, @switch, isUnicode, isEnglish, ref solution.Files[index], rental.AsSpan(0, actual));
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(rental);
                }
            }).ConfigureAwait(false);

            var diagnosticsSeverity = (DiagnosticSeverity)(uint)severity;
            if (!CompileResultSync(solution, diagnosticsSeverity))
            {
                return 0;
            }

            if (!CheckSync(solution))
            {
                return 0;
            }
        }
        finally
        {
            solution.Dispose();
            stopwatch?.Stop();
            if (stopwatch is not null)
            {
                var milliseconds = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"{milliseconds} milli seconds passed.");
            }
        }

        return 0;
    }

    private bool CheckSync(Solution solution)
    {
        var success = solution.CheckExistance();
        if (!success)
        {
            foreach (var error in solution.SolutionErrorList)
            {
                Console.WriteLine(error.Text);
            }
        }

        return success;
    }

    private static bool CompileResultSync(Solution solution, DiagnosticSeverity severity)
    {
        var stringBuilder = new StringBuilder(1 << 14);
        bool isSuccess = true;
        foreach (ref var result in solution.Files)
        {
            if (result.ErrorList.IsEmpty)
            {
                continue;
            }

            for (uint j = 0; j < result.ErrorList.Count; j++)
            {
                var error = result.ErrorList[j];
                if (error.Severity == DiagnosticSeverity.Error)
                {
                    isSuccess = false;
                }

                stringBuilder.AppendLine(error.Text);
                stringBuilder.Append(result.FilePath);
                stringBuilder.Append("(");
                stringBuilder.Append(error.Range.StartInclusive.Line + 1);
                stringBuilder.Append(", ");
                stringBuilder.Append(error.Range.StartInclusive.Offset + 1);
                stringBuilder.Append(")");
                if (severity != 0U)
                {
                    stringBuilder.Append(", Severity: ");
                    stringBuilder.Append(error.Severity);
                }
                stringBuilder.AppendLine();
            }
        }

        Console.WriteLine(stringBuilder.ToString());
        return isSuccess;
    }

    private static void LoadAndParse(DiagnosticSeverity severity, bool treatSlashPlusAsSingleLineComment, bool isUnicode, bool isEnglish, ref Result result, Span<byte> input)
    {
        if (isUnicode)
        {
            UnicodeHandler.Load(input, out result.Source);
        }
        else
        {
            Cp932Handler.Load(input, out result.Source);
        }

        Context context = new(treatSlashPlusAsSingleLineComment, isEnglish, severity);
        result.Success = Parser.Parse(ref context, ref result);
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
