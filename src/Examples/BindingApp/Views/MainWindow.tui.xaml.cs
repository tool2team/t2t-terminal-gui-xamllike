using System.ComponentModel;
using Terminal.Gui.Views;

namespace BindingApp.Views;

/// <summary>
/// Main view demonstrating self-binding (binding to view properties without ViewModel)
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private int _counter = 0;
    private string _userName = string.Empty;

    public MainWindow()
    {
        InitializeComponent();
        TxtUserName.SetFocus();
    }

    // Bindable properties
    public int Counter
    {
        get => _counter;
        set
        {
            if (_counter != value)
            {
                _counter = value;
                OnPropertyChanged(nameof(Counter));
                OnPropertyChanged(nameof(CounterMessage));
            }
        }
    }

    public string UserName
    {
        get => _userName;
        set
        {
            if (_userName != value)
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
                OnPropertyChanged(nameof(WelcomeMessage));
            }
        }
    }

    // Computed properties
    public string WelcomeMessage => 
        string.IsNullOrEmpty(UserName) ? "Welcome, Anonymous!" : $"Welcome, {UserName}!";

    public string CounterMessage => 
        $"Button clicked {Counter} time(s)";

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Event handlers
    private void OnIncrementClicked(object? sender, EventArgs e)
    {
        Counter++;
    }

    private void OnResetClicked(object? sender, EventArgs e)
    {
        Counter = 0;
        UserName = string.Empty;
    }

    private void OnExitClicked(object? sender, EventArgs e)
    {
        App?.RequestStop();
    }
}