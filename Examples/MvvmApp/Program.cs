using Microsoft.Extensions.DependencyInjection;
using MvvmApp.Views;
using Terminal.Gui.App;

namespace MvvmApp;

/// <summary>
/// MVVM application with custom ViewModels and Dependency Injection
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting MVVM Demo with Dependency Injection...");

        try
        {
            using IApplication app = Application.Create().Init();

            // Configure dependency injection with the application instance
            using ServiceProvider serviceProvider = ServiceConfiguration.ConfigureServices(app);

            // Resolve MainWindow from DI container (ViewModel injected automatically)
            MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();

            app.Run(mainWindow);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
