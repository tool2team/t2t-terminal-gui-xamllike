using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace ViewShowcaseApp.Views;

public partial class SpinnerViewDemo : View
{
    public SpinnerViewDemo()
    {
        InitializeComponent();
        
        // Configure spinner
        DotsSpinner.Style = new SpinnerStyle.Dots();
        DotsSpinner.AutoSpin = false;
    }

    private void OnToggleSpinner(object? sender, EventArgs e)
    {
        DotsSpinner.AutoSpin = !DotsSpinner.AutoSpin;
        
        if (DotsSpinner.AutoSpin)
        {
            ToggleButton.Text = "Stop Spinner";
            StatusLabel.Text = "Spinner is running...";
        }
        else
        {
            ToggleButton.Text = "Start Spinner";
            StatusLabel.Text = "Spinner is stopped";
        }
    }
}
