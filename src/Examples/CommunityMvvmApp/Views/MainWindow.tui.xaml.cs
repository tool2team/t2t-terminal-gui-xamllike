using CommunityMvvmApp.ViewModels;
using Terminal.Gui.Input;
using Terminal.Gui.Views;

namespace CommunityMvvmApp.Views;

/// <summary>
/// Main view using CommunityToolkit.Mvvm ViewModels
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

    #region Event Handlers

    private void OnIncrementClicked(object? sender, CommandEventArgs e)
    {
        ViewModel.IncrementCommand.Execute(null);
    }

    private void OnResetClicked(object? sender, EventArgs e)
    {
        ViewModel.ResetCommand.Execute(null);
        
        // Reset UI controls
        TxtUserName?.Text = string.Empty;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        await ViewModel.SaveProfileCommand.ExecuteAsync(null);
    }

    private void OnToggleClicked(object? sender, EventArgs e)
    {
        ViewModel.ToggleEnabledCommand.Execute(null);
    }

    private async void OnLoadDataClicked(object? sender, EventArgs e)
    {
        await ViewModel.LoadDataCommand.ExecuteAsync(null);
    }

    private void OnExitClicked(object? sender, EventArgs e)
    {
        App?.RequestStop();
    }

    #endregion
}