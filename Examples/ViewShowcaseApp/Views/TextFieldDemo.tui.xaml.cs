using Terminal.Gui.ViewBase;

namespace ViewShowcaseApp.Views;

public partial class TextFieldDemo : View
{
    public TextFieldDemo()
    {
        InitializeComponent();
    }

    private void OnTextChanged(object? sender, EventArgs e)
    {
        CharCountLabel.Text = $"Characters: {DemoTextField.Text?.Length ?? 0}";
    }
}
