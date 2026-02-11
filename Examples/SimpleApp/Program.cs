using SimpleApp.Views;
using Terminal.Gui.App;

namespace SimpleApp;

/// <summary>
/// Simple application without ViewModels - all logic in views
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting Simple Demo...");

        try
        {
            using IApplication app = Application.Create().Init();
            app.Run<MainWindow>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}