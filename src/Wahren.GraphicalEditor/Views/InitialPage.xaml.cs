using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Wahren.GraphicalEditor.Views;
using Models;
using System.ComponentModel;

public partial class InitialPage : IDisposable, INotifyPropertyChanged
{
    private readonly IInitialSettingsContainer initialSettingsContainer;
    private readonly IPageNavigatorToErrorFixPage navigatorToErrorFixPage;
    private readonly IPageNavigatorToEditPage navigatorToEditPage;

    public ReactivePropertySlim<string?> WorkingDirectory { get; }
    public ReactivePropertySlim<bool> SaveWorkingDirectory { get; }
    public AsyncReactiveCommand StartEditCommand { get; }

    private readonly IDisposable toggleSaveWorkingDirectory;
    private readonly IDisposable changeWorkingDirectory;
    private readonly IDisposable startEditAsyncTaskDisposable;

    public event PropertyChangedEventHandler? PropertyChanged;

    public InitialPage(IInitialSettingsContainer initialSettingsContainer, IPageNavigatorToErrorFixPage navigatorToErrorFixPage, IPageNavigatorToEditPage navigatorToEditPage)
    {
        this.initialSettingsContainer = initialSettingsContainer;
        this.navigatorToErrorFixPage = navigatorToErrorFixPage;
        this.navigatorToEditPage = navigatorToEditPage;
        ref var initialSettings = ref initialSettingsContainer.GetInitialSettings();
        WorkingDirectory = new(initialSettings.SaveWorkingDirectory ? initialSettings.WorkingDirectory : null);
        SaveWorkingDirectory = new(initialSettings.SaveWorkingDirectory);
        toggleSaveWorkingDirectory = SaveWorkingDirectory.AsObservable().Subscribe(ChangeSaveWorkingDirectory);
        var workingDirectoryObservable = WorkingDirectory.AsObservable();
        changeWorkingDirectory = workingDirectoryObservable.Subscribe(ChangeWorkingDirectory);

        StartEditCommand = new(workingDirectoryObservable.Select(path =>
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return false;
            }

            if (!Directory.Exists(Path.Combine(path, "script")))
            {
                return false;
            }

            string imagePath = Path.Combine(path, "image");
            if (!Directory.Exists(imagePath))
            {
                return false;
            }

            if (!File.Exists(Path.Combine(imagePath, "image.dat")) || !File.Exists(Path.Combine(imagePath, "imagedata.dat")) || !File.Exists(Path.Combine(imagePath, "image2.dat")) || !File.Exists(Path.Combine(imagePath, "imagedata2.dat")))
            {
                return false;
            }

            if (!Directory.Exists(Path.Combine(path, "stage")))
            {
                return false;
            }

            return true;
        }));

        startEditAsyncTaskDisposable = StartEditCommand.Subscribe(MoveToNextPage);

        DataContext = this;
        InitializeComponent();
    }

    private void ChangeSaveWorkingDirectory(bool value)
    {
        ref var settings = ref initialSettingsContainer.GetInitialSettings();
        if (settings.SaveWorkingDirectory != value)
        {
            settings.SaveWorkingDirectory = value;
            initialSettingsContainer.Save();
        }
    }

    private void Button_OpenFolder_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        using System.Windows.Forms.FolderBrowserDialog dialog = new();
        ref var settings = ref initialSettingsContainer.GetInitialSettings();
        if (settings.SaveWorkingDirectory && settings.WorkingDirectory is not null)
        {
            dialog.InitialDirectory = settings.WorkingDirectory;
        }
        else
        {
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
        var dialogResult = dialog.ShowDialog();
        if (dialogResult != System.Windows.Forms.DialogResult.OK)
        {
            return;
        }

        WorkingDirectory.Value = dialog.SelectedPath;
    }

    private void ChangeWorkingDirectory(string? value)
    {
        ref var settings = ref initialSettingsContainer.GetInitialSettings();
        if (settings.WorkingDirectory != value)
        {
            settings.WorkingDirectory = value;
            initialSettingsContainer.Save();
        }
    }

    private async Task MoveToNextPage()
    {
        AbstractSyntaxTree.Project.Project project = new() { RequiredSeverity = AbstractSyntaxTree.Parser.DiagnosticSeverity.Error };
        var workingDirectory = WorkingDirectory.Value;
        if (workingDirectory is null)
        {
            return;
        }

        var scriptFolder = Path.Combine(workingDirectory, "script");
        var isEnglishTask = DetectIsEnglishAsync(scriptFolder);
        var files = Directory.GetFiles(scriptFolder, "*.dat", SearchOption.AllDirectories);
        project.Files.PrepareAddRange(files.Length);
        project.FileAnalysisList.PrepareAddRange(files.Length);
        for (uint i = 0; i < files.Length; i++)
        {
            project.Files.Add(new(i));
            project.Files[i].FilePath = files[i];
            project.FileAnalysisList.Add(new());
        }

        var (isUnicode, isEnglish) = await isEnglishTask.ConfigureAwait(false);
        await Parallel.ForEachAsync(System.Linq.Enumerable.Range(0, files.Length), async ValueTask (int index, CancellationToken token) =>
        {
            var filePath = project.Files[index].FilePath;
            if (filePath is null)
            {
                throw new InvalidProgramException();
            }

            var (buffer, size) = await ReadFileUtility.ReadAsync(filePath, token).ConfigureAwait(false);
            try
            {
                if (isUnicode)
                {
                    FileLoader.UnicodeHandler.Load(buffer.AsSpan(0, size), out project.Files[index].Source);
                }
                else
                {
                    FileLoader.Cp932Handler.Load(buffer.AsSpan(0, size), out project.Files[index].Source);
                }
            }
            finally
            {
                if (buffer.Length != 0)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }

            AbstractSyntaxTree.Parser.Context context = new(isEnglishMode: isEnglish, deleteDiscardedToken: true, requiredSeverity: project.RequiredSeverity);
            AbstractSyntaxTree.Parser.Parser.Parse(ref context, ref project.Files[index]);
            AbstractSyntaxTree.Parser.PerResultValidator.AddReferenceAndValidate(ref context, ref project.Files[index], project.FileAnalysisList[index]);
        });

        static bool IsFail(Span<AbstractSyntaxTree.Parser.Result> results)
        {
            foreach (ref var result in results)
            {
                if (!result.NoError())
                {
                    return true;
                }
            }

            return false;
        }

        if (IsFail(project.Files.AsSpan()))
        {
            navigatorToErrorFixPage.NavigateToErrorFixPage(project);
            return;
        }

        project.AddReferenceAndValidate();
        if (IsFail(project.Files.AsSpan()) || !project.DetectInfiniteLoop() || !project.CheckExistance())
        {
            navigatorToErrorFixPage.NavigateToErrorFixPage(project);
        }
        else
        {
            navigatorToEditPage.NavigateToEditPage(project);
        }
    }

    private async ValueTask<(bool isUnicode, bool isEnglish)> DetectIsEnglishAsync(string scriptFolder)
    {
        if (File.Exists(Path.Combine(scriptFolder, "english.txt")))
        {
            return (true, true);
        }

        var unicodeTxt = Path.Combine(scriptFolder, "unicode.txt");
        if (!File.Exists(unicodeTxt))
        {
            return (false, false);
        }

        static bool IsEnglish(ReadOnlySpan<byte> span)
        {
            if (span.Length <= 2)
            {
                return false;
            }

            if (span[0] == 0xff && span[1] == 0xfe)
            {
                span = span.Slice(2);
            }

            return MemoryMarshal.Cast<char, byte>("foreign").SequenceEqual(span);
        }

        var (buffer, size) = await ReadFileUtility.ReadAsync(unicodeTxt, CancellationToken.None).ConfigureAwait(false);
        try
        {
            return (true, IsEnglish(buffer.AsSpan(0, size)));
        }
        finally
        {
            if (buffer.Length != 0)
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }

    public void Dispose()
    {
        startEditAsyncTaskDisposable.Dispose();
        StartEditCommand.Dispose();
        changeWorkingDirectory.Dispose();
        toggleSaveWorkingDirectory.Dispose();
        WorkingDirectory.Dispose();
        SaveWorkingDirectory.Dispose();
    }
}
