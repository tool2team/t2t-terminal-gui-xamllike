using CommunityMvvmApp.Views;
using Terminal.Gui.App;

namespace CommunityMvvmApp;

/// <summary>
/// MVVM application using CommunityToolkit.Mvvm
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting CommunityMvvm Demo...");

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