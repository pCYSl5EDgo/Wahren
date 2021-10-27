namespace Wahren.GraphicalEditor.Views;
using AbstractSyntaxTree.Project;

public partial class EditPage
{
    public ReactivePropertySlim<Project> Project;
    public EditPage(Project project)
    {
        Project = new(project);
        InitializeComponent();
    }
}
