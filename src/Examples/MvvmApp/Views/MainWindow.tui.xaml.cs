using MvvmApp.ViewModels;
using Terminal.Gui.Views;

namespace MvvmApp.Views;

/// <summary>
/// Main view with MVVM pattern using custom ViewModels
/// </summary>
public partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }

    public MainWindow(MainViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        
        TxtUserName.SetFocus();
    }

    /// <summary>
    /// Handle increment button click
    /// </summary>
    private void OnIncrementClicked(object? sender, EventArgs e)
    {
        ViewModel.IncrementCommand.Execute(null);
    }

    /// <summary>
    /// Handle reset button click
    /// </summary>
    private void OnResetClicked(object? sender, EventArgs e)
    {
        ViewModel.ResetCommand.Execute(null);
    }

    /// <summary>
    /// Handle save button click
    /// </summary>
    private void OnSaveClicked(object? sender, EventArgs e)
    {
        if (ViewModel.SaveCommand.CanExecute(null))
            ViewModel.SaveCommand.Execute(null);
    }

    /// <summary>
    /// Handle toggle button click
    /// </summary>
    private void OnToggleClicked(object? sender, EventArgs e)
    {
        ViewModel.ToggleEnabled();
    }

    /// <summary>
    /// Handle exit button click
    /// </summary>
    private void OnExitClicked(object? sender, EventArgs e)
    {
        App?.RequestStop();
    }
}