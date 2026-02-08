using MvvmApp.ViewModels;
using Terminal.Gui.Views;

namespace MvvmApp.Views;

/// <summary>
/// Main view with MVVM pattern using custom ViewModels
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
    // No event handlers needed in code-behind for pure MVVM approach
}