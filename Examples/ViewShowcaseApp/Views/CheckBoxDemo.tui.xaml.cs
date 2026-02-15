using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace ViewShowcaseApp.Views;

public partial class CheckBoxDemo : View
{
    public CheckBoxDemo()
    {
        InitializeComponent();
    }

    private void OnCheckBoxToggled(object? sender, ValueChangedEventArgs<CheckState> e)
    {
        var count = 0;
        if (Option1.Value == CheckState.Checked) count++;
        if (Option2.Value == CheckState.Checked) count++;
        if (Option3.Value == CheckState.Checked) count++;

        StatusLabel.Text = count == 0 ? "None selected" : $"{count} option(s) selected";
    }
}
