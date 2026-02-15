using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using ViewShowcaseApp.Models;

namespace ViewShowcaseApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    readonly IApplication? _application;

    public MainViewModel(IApplication application)
    {
        _application = application;
    }

    public static ObservableCollection<ViewInfo> ViewInfos { get; } = new(ViewInfoCollection.GetAllViews());
    public ListWrapper<ViewInfo> ViewInfosWrapper { get; } = new ListWrapper<ViewInfo>(ViewInfos);
    public ViewInfo SelectedViewInfo => SelectedViewIndex >= 0 && SelectedViewIndex < ViewInfos.Count ? ViewInfos[SelectedViewIndex.Value] : ViewInfoCollection.DefaultView;

    public string ViewName => SelectedViewInfo.Name;
    public string ViewDescription => SelectedViewInfo.Description;

    private View? _currentView;
    public View? CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }

    public event EventHandler<View>? ViewSelectionChanged;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ViewName))]
    [NotifyPropertyChangedFor(nameof(ViewDescription))]
    private int? selectedViewIndex;

    partial void OnSelectedViewIndexChanged(int? value)
    {
        CurrentView = SelectedViewInfo.CreateView();
        ViewSelectionChanged?.Invoke(this, CurrentView);
        StatusText = $"Selected: {SelectedViewInfo.Name}";
    }

    [ObservableProperty]
    private string statusText = "Ready - Select a view from the left panel";

    [RelayCommand]
    private void ShowAbout()
    {
        if(_application is not null)
        _ = MessageBox.Query(_application, "About View Showcase", 
            "Terminal.Gui View Showcase\n\nThis application demonstrates all available Terminal.Gui views.\n\nUse the left panel to browse and select views to test.", 
            "OK");
    }

    [RelayCommand] 
    private void RefreshView()
    {
        CurrentView = SelectedViewInfo.CreateView();
        ViewSelectionChanged?.Invoke(this, CurrentView);
        StatusText = $"Refreshed: {SelectedViewInfo.Name}";
    }

    [RelayCommand]
    private void ShowViewDetails()
    {
        var details = $"View: {SelectedViewInfo.Name}\nCategory: {SelectedViewInfo.Category}\n\nDescription:\n{SelectedViewInfo.Description}";
        if (_application is not null) _ = MessageBox.Query(_application, $"Details: {SelectedViewInfo.Name}", details, "OK");
    }

    [RelayCommand]
    private void Quit()
    {
        _application?.RequestStop();
    }
}
