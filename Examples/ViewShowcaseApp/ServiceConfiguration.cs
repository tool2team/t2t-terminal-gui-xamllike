using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui.App;
using ViewShowcaseApp.ViewModels;
using ViewShowcaseApp.Views;

namespace ViewShowcaseApp;

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