using System.IO;
using MahApps.Metro.Controls;

namespace Wahren.GraphicalEditor;
using Views;
using ViewModels;
using Models;
using Wahren.AbstractSyntaxTree.Project;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : IInitialSettingsContainer, IPageNavigatorToErrorFixPage, IPageNavigatorToEditPage
{
    private InitialSettings settings;

    public ref InitialSettings GetInitialSettings() => ref settings;

    public void Save()
    {
        try
        {
            using var handle = File.OpenHandle("./InitialSettings.bytes", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            var bytes = MessagePack.MessagePackSerializer.Serialize(settings);
            RandomAccess.Write(handle, bytes.AsSpan(), 0);
        }
        catch
        {
        }
    }

    public void NavigateToErrorFixPage(Project project)
    {
        frame.Navigate(new ErrorFixPage(project, this));
    }

    public void NavigateToEditPage(Project project)
    {
        frame.Navigate(new EditPage(project));
    }

    public MainWindow()
    {
        if (File.Exists("./InitialSettings.bytes"))
        {
            try
            {
                var bytes = File.ReadAllBytes("./InitialSettings.bytes");
                settings = MessagePack.MessagePackSerializer.Deserialize<InitialSettings>(bytes);
            }
            catch
            {
                settings = new(true, null);
            }
        }
        else
        {
            settings = new(true, null);
        }

        InitializeComponent();
        frame.Navigate(new InitialPage(this, this, this));
    }
}
