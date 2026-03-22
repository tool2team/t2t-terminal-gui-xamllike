using Terminal.Gui.ViewBase;

namespace ViewShowcaseApp.Views;

public partial class LinkDemo : View
{
    public LinkDemo()
    {
        InitializeComponent();

        DemoLink.MouseEnter += (s, e) => StatusLabel.Text = DemoLink.Url;
        DemoLink.MouseLeave += (s, e) => StatusLabel.Text = "";
    }

    protected void OnQuit(object sender, EventArgs e)
    {
        App?.RequestStop();
    }
}
