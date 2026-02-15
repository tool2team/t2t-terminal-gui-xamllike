using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace ViewShowcaseApp.Views;

public partial class TabViewDemo : View
{
    public TabViewDemo()
    {
        InitializeComponent();
        
    }

    private void OnCheckUpdates(object? sender, EventArgs e)
    {
        if (App is not null) MessageBox.Query(App, "Update Check", "You are running the latest version!", "OK");
    }
}
