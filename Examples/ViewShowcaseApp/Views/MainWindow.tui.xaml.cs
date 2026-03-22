using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using ViewShowcaseApp.ViewModels;

namespace ViewShowcaseApp.Views;

public partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }
    private View? _currentDemoView;

    public MainWindow(MainViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        UpdatePlaceholderVisibility();

        ViewModel.ViewSelectionChanged += OnViewSelectionChanged;
    }
 
    private void OnViewSelectionChanged(object? sender, View demoView)
    {
        // Remove current demo view if exists
        if (_currentDemoView != null)
        {
            MainFrame.Remove(_currentDemoView);
            _currentDemoView.Dispose();
            _currentDemoView = null;
        }

        // Add new demo view
        if (ViewModel.CurrentView != null)
        {
            _currentDemoView = demoView;
            _currentDemoView.CanFocus = true; // ⚠️ IMPORTANT

            MainFrame.Add(_currentDemoView);
        }

        UpdatePlaceholderVisibility();
    }

    private void UpdatePlaceholderVisibility()
    {
        PlaceholderLabel.Visible = _currentDemoView == null;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _currentDemoView?.Dispose();
            ViewModel.ViewSelectionChanged -= OnViewSelectionChanged;
        }
        base.Dispose(disposing);
    }
}
