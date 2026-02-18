using Terminal.Gui.Input;
using Terminal.Gui.Views;

namespace ViewShowcaseApp.Views;

public partial class DialogDemoDlg
{
    public string? EnteredName => NameTextField.Text;
    public string? EnteredEmail => EmailTextField.Text;

    public DialogDemoDlg()
    {
        InitializeComponent();
    }

    private void OnReset(object? sender, CommandEventArgs e)
    {
        NameTextField.Text = "";
        EmailTextField.Text = "";
        e.Handled = true;
    }

    private void OnOk(object? sender, CommandEventArgs e)
    {        
        if (string.IsNullOrWhiteSpace(EnteredName))
        {
            if (App is not null) MessageBox.ErrorQuery(App, "Validation Error", "Please enter your name.", "OK");
            e.Handled = true; // Keep the dialog open
            return;
        }
        
        if (string.IsNullOrWhiteSpace(EnteredEmail))
        {
            if (App is not null) MessageBox.ErrorQuery(App, "Validation Error", "Please enter your email.", "OK");
            e.Handled = true; // Keep the dialog open
            return;
        }
        
        Result = 0; // OK button index
    }

    private void OnCancel(object? sender, CommandEventArgs e)
    {
        Result = 1; // Cancel button index
    }
}
