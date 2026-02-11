using BindingApp.Views;
using Terminal.Gui.App;

namespace BindingApp;

/// <summary>
/// Application demonstrating self-binding (binding to view properties without ViewModel)
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting Self-Binding Demo...");

        try
        {
            using IApplication app = Application.Create().Init();
            var mainWindow = new MainWindow();
            app.Run(mainWindow);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}