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
    [NotifyPropertyChangedFor(nameof(WelcomeMessage))]
    [NotifyCanExecuteChangedFor(nameof(SaveProfileCommand))]
    private string _userName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CounterMessage))]
    [NotifyCanExecuteChangedFor(nameof(IncrementCommand))]
    private int _counter = 0;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSaveEnabled))]
    [NotifyPropertyChangedFor(nameof(EnabledMessage))]
    [NotifyPropertyChangedFor(nameof(IsReadOnly))]
    [NotifyCanExecuteChangedFor(nameof(IncrementCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveProfileCommand))]
    private bool _isEnabled = true;

    public bool IsSaveEnabled => IsEnabled && !string.IsNullOrWhiteSpace(UserName);

    public string EnabledMessage => IsEnabled ? "Controls enabled" : "Controls disabled";

    public bool IsReadOnly => !IsEnabled;

    [ObservableProperty]
    private UserRole _role = UserRole.Guest;

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
        StatusMessage = $"Counter incremented to {Counter}";

        if (Counter >= 99)
            StatusMessage += " (Maximum reached!)";
    }

    private bool CanIncrement() => IsEnabled && Counter < 99;

    [RelayCommand]
    private void Reset()
    {
        Counter = 0;
        UserName = string.Empty;
        Role = UserRole.Guest;
        StatusMessage = "All values reset";
    }

    [RelayCommand(CanExecute = nameof(CanSaveProfile))]
    private async Task SaveProfile()
    {
        StatusMessage = $"Saving profile for {UserName}...";

        // Simulate async operation
        await Task.Delay(1500);

        StatusMessage = $"Profile saved successfully! Counter: {Counter}, Role: {Role}";
    }

    private bool CanSaveProfile() => IsEnabled && !string.IsNullOrWhiteSpace(UserName);

    [RelayCommand]
    private void Toggle()
    {
        IsEnabled = !IsEnabled;
    }

    [RelayCommand]
    private async Task LoadData()
    {
        StatusMessage = "Loading data...";
        IsEnabled = false;

        try
        {
            // Simulate data loading
            await Task.Delay(2000);

            Counter = new Random().Next(1, 50);
            UserName = $"User{Counter}";
            StatusMessage = "Data loaded successfully!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading data: {ex.Message}";
        }
        finally
        {
            IsEnabled = true;
        }
    }

    [RelayCommand]
    private void Exit()
    {
        StatusMessage = "Exiting application...";
        _application.RequestStop();
    }
}
