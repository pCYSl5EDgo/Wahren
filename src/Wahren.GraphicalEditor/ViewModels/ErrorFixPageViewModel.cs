namespace Wahren.GraphicalEditor.ViewModels;
using AbstractSyntaxTree.Project;

public sealed class ErrorFixPageViewModel : ViewModelBase, IDisposable
{
    private readonly Project project;

    public ReactiveCollection<ErrorFixPageViewModel_File> Files { get; }

    public ErrorFixPageViewModel(Project project)
    {
        this.project = project;
        Files = new();
        var files = project.Files.AsSpan();
        for (int i = 0; i < files.Length; i++)
        {
            Files.Add(new(project, (uint)i));
        }
    }

    public void Dispose()
    {
        while (Files.Count != 0)
        {
            var lastIndex = Files.Count - 1;
            Files[lastIndex].Dispose();
            Files.RemoveAt(lastIndex);
        }

        Files.Dispose();
    }
}
