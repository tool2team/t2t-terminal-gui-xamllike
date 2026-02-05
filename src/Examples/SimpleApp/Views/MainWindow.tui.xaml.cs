using Terminal.Gui.Views;

namespace SimpleApp.Views;

/// <summary>
/// Simple main view with logic directly in the view (no ViewModel)
/// </summary>
public partial class MainWindow : Window
{
    private int _counter = 0;
    private string _userName = string.Empty;

    public MainWindow()
    {
        InitializeComponent();
        
        // Set initial values
        UpdateDisplay();
        TxtUserName.SetFocus();
    }

    /// <summary>
    /// Handle button click to increment counter
    /// </summary>
    private void OnIncrementClicked(object? sender, EventArgs e)
    {
        _counter++;
        UpdateDisplay();
    }

    /// <summary>
    /// Handle button click to reset counter
    /// </summary>
    private void OnResetClicked(object? sender, EventArgs e)
    {
        _counter = 0;
        _userName = string.Empty;
        
        TxtUserName.Text = string.Empty;
            
        UpdateDisplay();
    }

    /// <summary>
    /// Handle text change in username field
    /// </summary>
    private void OnUserNameChanged(object? sender, EventArgs e)
    {
        if (sender is TextField textField)
        {
            _userName = textField.Text.ToString() ?? string.Empty;
            UpdateDisplay();
        }
    }

    /// <summary>
    /// Handle exit button click
    /// </summary>
    private void OnExitClicked(object? sender, EventArgs e)
    {
        App?.RequestStop();
    }

    /// <summary>
    /// Update the display with current values
    /// </summary>
    private void UpdateDisplay()
    {
        var greeting = string.IsNullOrEmpty(_userName) ? "Anonymous" : _userName;
        
        LblWelcome.Text = $"Welcome, {greeting}!";
            
        LblCounter.Text = $"Button clicked {_counter} time(s)";
    }
}