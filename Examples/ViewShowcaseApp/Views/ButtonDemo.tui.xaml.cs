using Terminal.Gui.ViewBase;

namespace ViewShowcaseApp.Views;

public partial class ButtonDemo : View
{
    private int _clickCount = 0;

    public ButtonDemo()
    {
        InitializeComponent();
    }

    private void OnButtonClicked(object? sender, EventArgs e)
    {
        _clickCount++;
        DemoButton.Text = $"Clicked {_clickCount} times!";
    }
}
