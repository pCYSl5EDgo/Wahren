namespace Wahren.GraphicalEditor.Models;

public interface IInitialSettingsContainer
{
    ref InitialSettings GetInitialSettings();

    void Save();
}
