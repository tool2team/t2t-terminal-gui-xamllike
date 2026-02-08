using CommunityMvvmApp.ViewModels;
using Terminal.Gui.Input;
using Terminal.Gui.Views;

namespace CommunityMvvmApp.Views;

/// <summary>
/// Main view using CommunityToolkit.Mvvm ViewModels
/// </summary>
public partial class MainWindow : Window
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
