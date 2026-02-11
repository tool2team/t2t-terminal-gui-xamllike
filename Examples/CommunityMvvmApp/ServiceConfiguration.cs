using Microsoft.Extensions.DependencyInjection;
using CommunityMvvmApp.ViewModels;
using CommunityMvvmApp.Views;
using Terminal.Gui.App;

namespace CommunityMvvmApp;

/// <summary>
/// Configures dependency injection for the application
/// </summary>
public static class ServiceConfiguration
{
    /// <summary>
    /// Configures services and builds the service provider
    /// </summary>
    public static ServiceProvider ConfigureServices(IApplication application)
    {
        var services = new ServiceCollection();

        // Register Terminal.Gui Application as Singleton
        services.AddSingleton(application);

        // Register ViewModels as Singletons
        services.AddSingleton<MainViewModel>();

        // Register Views as Transient (new instance each time, but with singleton VM)
        services.AddTransient<MainWindow>();

        return services.BuildServiceProvider();
    }
}
