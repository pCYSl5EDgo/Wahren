using System.IO;
using System.ComponentModel;

namespace Wahren.GraphicalEditor.ViewModels;
using AbstractSyntaxTree.Project;

public sealed class ErrorFixPageViewModel_File : INotifyPropertyChanged, IDisposable
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Project project;
    private readonly uint fileId;

    public ReactivePropertySlim<string> FileName { get; }

    public ErrorFixPageViewModel_File(Project project, uint fileId)
    {
        this.project = project;
        this.fileId = fileId;

        ref var file = ref project.Files[fileId];
        FileName = new(Path.GetFileNameWithoutExtension(file.FilePath) ?? "________");
    }

    public void Dispose()
    {
        FileName.Dispose();
    }
}
