using System.Windows.Input;
using Terminal.Gui.App;

namespace MvvmApp.ViewModels;

/// <summary>
/// Main ViewModel for the MVVM demo
/// </summary>
public class MainViewModel : BaseViewModel
{
    private readonly IApplication _application;
    private string _userName = string.Empty;
    private int _counter = 0;
    private string _status = "Ready";
    private bool _isEnabled = true;

    public MainViewModel(IApplication application)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));

        // Initialize commands
        IncrementCommand = new RelayCommand(ExecuteIncrement, CanExecuteIncrement);
        ResetCommand = new RelayCommand(ExecuteReset);
        SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
        ToggleCommand = new RelayCommand(ExecuteToggle);
        ExitCommand = new RelayCommand(ExecuteExit);
    }

    #region Properties

    public string UserName
    {
        get => _userName;
        set
        {
            if (SetProperty(ref _userName, value))
            {
                OnPropertyChanged(nameof(WelcomeMessage));
                OnPropertyChanged(nameof(IsSaveEnabled));
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public int Counter
    {
        get => _counter;
        set
        {
            if (SetProperty(ref _counter, value))
            {
                OnPropertyChanged(nameof(CounterMessage));
                ((RelayCommand)IncrementCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (SetProperty(ref _isEnabled, value))
            {
                OnPropertyChanged(nameof(IsSaveEnabled));
                ((RelayCommand)IncrementCommand).RaiseCanExecuteChanged();
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsSaveEnabled => IsEnabled && !string.IsNullOrWhiteSpace(UserName);

    public string WelcomeMessage => 
        string.IsNullOrEmpty(UserName) ? "Hello, Anonymous!" : $"Hello, {UserName}!";

    public string CounterMessage => 
        $"Counter: {Counter}";

    #endregion

    #region Commands

    public ICommand IncrementCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ToggleCommand { get; }
    public ICommand ExitCommand { get; }

    private void ExecuteIncrement()
    {
        Counter++;
        Status = $"Counter incremented to {Counter}";
    }

    private bool CanExecuteIncrement()
    {
        return IsEnabled && Counter < 99;
    }

    private void ExecuteReset()
    {
        Counter = 0;
        UserName = string.Empty;
        Status = "Reset completed";
    }

    private void ExecuteSave()
    {
        Status = $"Saved profile for {UserName} with counter {Counter}";
        
        // Simulate async operation
        Task.Run(async () =>
        {
            await Task.Delay(1000);
            Status = "Save completed successfully!";
        });
    }

    private bool CanExecuteSave()
    {
        return IsEnabled && !string.IsNullOrWhiteSpace(UserName);
    }

    private void ExecuteToggle()
    {
        IsEnabled = !IsEnabled;
        Status = IsEnabled ? "Controls enabled" : "Controls disabled";
    }

    private void ExecuteExit()
    {
        Status = "Exiting application...";
        _application.RequestStop();
    }

    #endregion
}
