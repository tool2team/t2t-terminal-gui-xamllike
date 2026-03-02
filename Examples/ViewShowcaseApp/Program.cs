using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui.App;
using ViewShowcaseApp.Views;

namespace ViewShowcaseApp;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting View Showcase...");

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