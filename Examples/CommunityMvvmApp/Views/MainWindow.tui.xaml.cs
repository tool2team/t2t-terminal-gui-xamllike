using CommunityMvvmApp.ViewModels;

namespace CommunityMvvmApp.Views;

/// <summary>
/// Main view using CommunityToolkit.Mvvm ViewModels
/// </summary>
public partial class MainWindow
{
    internal MainViewModel ViewModel { get; }

    public MainWindow(MainViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        TxtUserName.SetFocus();
    }

    // Note: All button actions are now handled via Commands in the ViewModel
    // Pure MVVM approach with CommunityToolkit.Mvvm [RelayCommand]
}
