using Terminal.Gui.Views;

namespace ViewShowcaseApp.Views;

public partial class DialogDemoDlg
{
    public string? EnteredName { get; private set; }
    public string? EnteredEmail { get; private set; }

    public DialogDemoDlg()
    {
        InitializeComponent();
    }

    private void OnOk(object? sender, EventArgs e)
    {
        EnteredName = NameTextField.Text?.ToString();
        EnteredEmail = EmailTextField.Text?.ToString();
        
        if (string.IsNullOrWhiteSpace(EnteredName))
        {
            if (App is not null) MessageBox.ErrorQuery(App, "Validation Error", "Please enter your name.", "OK");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(EnteredEmail))
        {
            if (App is not null) MessageBox.ErrorQuery(App, "Validation Error", "Please enter your email.", "OK");
            return;
        }
        
        Result = 0; // OK button index
    }

    private void OnCancel(object? sender, EventArgs e)
    {
        Result = 1; // Cancel button index
    }
}
