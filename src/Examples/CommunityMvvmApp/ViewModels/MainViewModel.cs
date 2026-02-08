using CommunityMvvmApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Terminal.Gui.Input;
using Terminal.Gui.App;

namespace CommunityMvvmApp.ViewModels;

/// <summary>
/// Main ViewModel using CommunityToolkit.Mvvm with source generators
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IApplication _application;

    public MainViewModel(IApplication application)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));
    }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSaveEnabled))]
    private string _userName = string.Empty;

    [ObservableProperty]
    private int _counter = 0;

    [ObservableProperty]
    private string _status = "Ready";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSaveEnabled))]
    private bool _isEnabled = true;

    public bool IsSaveEnabled => IsEnabled && !string.IsNullOrWhiteSpace(UserName);

    [ObservableProperty]
    private UserRole _role = UserRole.Guest;

    // Computed properties using partial method generation
    partial void OnUserNameChanged(string value)
    {
        OnPropertyChanged(nameof(WelcomeMessage));
        SaveProfileCommand.NotifyCanExecuteChanged();
    }

    partial void OnCounterChanged(int value)
    {
        OnPropertyChanged(nameof(CounterMessage));
        IncrementCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsEnabledChanged(bool value)
    {
        IncrementCommand.NotifyCanExecuteChanged();
        SaveProfileCommand.NotifyCanExecuteChanged();
        Status = value ? "Controls enabled" : "Controls disabled";
    }

    // Computed properties
    public string WelcomeMessage => 
        string.IsNullOrEmpty(UserName) ? "Hello, Anonymous!" : $"Hello, {UserName}!";

    public string CounterMessage => 
        $"Counter: {Counter} (Max: 99)";

    // Commands using source generators
    [RelayCommand(CanExecute = nameof(CanIncrement))]
    private void Increment(CommandEventArgs args)
    {
        Counter++;
        Status = $"Counter incremented to {Counter}";
        
        if (Counter >= 99)
            Status += " (Maximum reached!)";
    }

    private bool CanIncrement() => IsEnabled && Counter < 99;

    [RelayCommand]
    private void Reset()
    {
        Counter = 0;
        UserName = string.Empty;
        Role = UserRole.Guest;
        Status = "All values reset";
    }

    [RelayCommand(CanExecute = nameof(CanSaveProfile))]
    private async Task SaveProfile()
    {
        Status = $"Saving profile for {UserName}...";

        // Simulate async operation
        await Task.Delay(1500);

        Status = $"Profile saved successfully! Counter: {Counter}, Role: {Role}";
    }

    private bool CanSaveProfile() => IsEnabled && !string.IsNullOrWhiteSpace(UserName);

    [RelayCommand]
    private void Toggle()
    {
        IsEnabled = !IsEnabled;
        Status = IsEnabled ? "Controls enabled" : "Controls disabled";
    }

    [RelayCommand]
    private async Task LoadData()
    {
        Status = "Loading data...";
        IsEnabled = false;

        try
        {
            // Simulate data loading
            await Task.Delay(2000);

            Counter = new Random().Next(1, 50);
            UserName = $"User{Counter}";
            Status = "Data loaded successfully!";
        }
        catch (Exception ex)
        {
            Status = $"Error loading data: {ex.Message}";
        }
        finally
        {
            IsEnabled = true;
        }
    }

    [RelayCommand]
    private void Exit()
    {
        Status = "Exiting application...";
        _application.RequestStop();
    }
}
