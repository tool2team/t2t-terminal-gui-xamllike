using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.Views;

namespace ViewShowcaseApp.Views;

public partial class AdvancedDialogDemoDlg
{
    public AdvancedDialogDemoDlg()
    {
        InitializeComponent();
    }

    private void OnBrowse(object? sender, CommandEventArgs e)
    {
        // Open file dialog to select a file
        var fileDialog = new OpenDialog
        {
            Title = "Select File"
        };

        App?.Run(fileDialog);

        if (!fileDialog.Canceled && fileDialog.Path != null)
        {
            FilePathTextField.Text = fileDialog.Path.ToString();
        }
        e.Handled = true;
    }

    private void OnApply(object? sender, EventArgs e)
    {
        // Apply changes and close
        if (App != null)
        {
            MessageBox.Query(App, "Success", $"File path applied: {FilePathTextField.Text}", "OK");
        }
        RequestStop();
    }

    private void OnCancel(object? sender, EventArgs e)
    {
        // Cancel and close without applying
        RequestStop();
    }
}
