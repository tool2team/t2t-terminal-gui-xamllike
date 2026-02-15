using Terminal.Gui.Views;

namespace ViewShowcaseApp.Views;

public partial class DialogDemo
{
    public DialogDemo()
    {
        InitializeComponent();
    }

    private void OnShow(object? sender, EventArgs e)
    {
        DialogDemoDlg dlg = new DialogDemoDlg();
        var res = App?.Run(dlg);
        
        ResultsLabel.Text = $"You selected: {res} with {dlg.EnteredName}<{dlg.EnteredEmail}>";
    }
}
