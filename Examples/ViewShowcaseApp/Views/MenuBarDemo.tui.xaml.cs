using Terminal.Gui.Views;

namespace ViewShowcaseApp.Views;

public partial class MenuBarDemo
{
    public MenuBarDemo()
    {
        InitializeComponent();
    }

    private void OnNew(object? sender, EventArgs e)
    {
        StatusLabel.Text = "File -> New selected";
        ContentArea.Text = "";
    }

    private void OnOpen(object? sender, EventArgs e)
    {
        StatusLabel.Text = "File -> Open selected";
        var dialog = new OpenDialog();
        App?.Run(dialog);
    }

    private void OnSave(object? sender, EventArgs e)
    {
        StatusLabel.Text = "File -> Save selected";
    }

    private void OnSaveAs(object? sender, EventArgs e)
    {
        StatusLabel.Text = "File -> Save As selected";
        var dialog = new SaveDialog();
        App?.Run(dialog);
    }

    private void OnExit(object? sender, EventArgs e)
    {
        App?.RequestStop();
    }

    private void OnUndo(object? sender, EventArgs e)
    {
        StatusLabel.Text = "Edit -> Undo selected";
    }

    private void OnRedo(object? sender, EventArgs e)
    {
        StatusLabel.Text = "Edit -> Redo selected";
    }

    private void OnCut(object? sender, EventArgs e)
    {
        StatusLabel.Text = "Edit -> Cut selected";
    }

    private void OnCopy(object? sender, EventArgs e)
    {
        StatusLabel.Text = "Edit -> Copy selected";
    }

    private void OnPaste(object? sender, EventArgs e)
    {
        StatusLabel.Text = "Edit -> Paste selected";
    }

    private void OnDocumentation(object? sender, EventArgs e)
    {
        if (App is not null) MessageBox.Query(App, "Documentation", "Opening documentation...", "OK");
        StatusLabel.Text = "Help -> Documentation selected";
    }

    private void OnAbout(object? sender, EventArgs e)
    {
        if (App is not null) MessageBox.Query(App, "About", "Terminal.Gui XamlLike Demo Application\nVersion 1.0.0", "OK");
        StatusLabel.Text = "Help -> About selected";
    }
}
