namespace Wahren.GraphicalEditor.Views;
using ViewModels;
using AbstractSyntaxTree.Project;

public partial class ErrorFixPage
{
    private readonly Project project;
    private readonly IPageNavigatorToEditPage navigatorToEditPage;

    private readonly ErrorFixPageViewModel viewModel;

    public ErrorFixPage(Project project, IPageNavigatorToEditPage navigatorToEditPage)
    {
        this.project = project;
        this.navigatorToEditPage = navigatorToEditPage;
        viewModel = new ErrorFixPageViewModel(project);
        InitializeComponent();
    }
}
