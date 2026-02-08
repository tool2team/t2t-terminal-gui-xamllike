using Microsoft.Extensions.DependencyInjection;
using CommunityMvvmApp.Views;
using Terminal.Gui.App;

namespace CommunityMvvmApp;

/// <summary>
/// MVVM application using CommunityToolkit.Mvvm with Dependency Injection
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting CommunityMvvm Demo with Dependency Injection...");

        try
        {
            using IApplication app = Application.Create().Init();

            // Configure dependency injection with the application instance
            using var serviceProvider = ServiceConfiguration.ConfigureServices(app);

            // Resolve MainWindow from DI container (ViewModel injected automatically)
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();

            app.Run(mainWindow);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
