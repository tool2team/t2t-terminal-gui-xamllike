using MvvmApp.Views;
using Terminal.Gui.App;

namespace MvvmApp;

/// <summary>
/// MVVM application with custom ViewModels and INotifyPropertyChanged
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting MVVM Demo...");
        
        try
        {
            using IApplication app = Application.Create().Init();
            var mainWindow = new MainWindow(new ViewModels.MainViewModel());
            app.Run(mainWindow);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}