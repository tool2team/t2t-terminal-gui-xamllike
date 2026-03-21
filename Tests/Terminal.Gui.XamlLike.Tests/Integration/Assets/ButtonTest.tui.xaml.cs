using System.ComponentModel;
using System.Windows.Input;
using Terminal.Gui.Input;
using Terminal.Gui.Views;

namespace Terminal.Gui.XamlLike.Tests.Integration.Xaml;

/// <summary>
/// Partial class for ButtonTest test view
/// </summary>
public partial class ButtonTest : Button, INotifyPropertyChanged
{
    public ButtonTest()
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnAccepting(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private ICommand AcceptedCommand => new Command(() => OnAccepting(this, CommandEventArgs.Empty));

}
