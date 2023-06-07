using GoCarlos.MAUI.Services;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Services;
using GoCarlos.NET.ViewModels;
using GoCarlos.NET.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace GoCarlos.NET;

public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddLogging(options =>
        {
            options.AddConsole().SetMinimumLevel(LogLevel.Information);
            options.AddDebug().SetMinimumLevel(LogLevel.Trace);
        });

        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        
        services.AddTransient<IEgdService, EgdService>();
        services.AddTransient<IWindowService, WindowService>();
        services.AddTransient<IMenuItemsService, MenuItemsService>();
        services.AddTransient<MenuViewModel>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton(provider => new MainWindow
        {
            DataContext = provider.GetRequiredService<MainViewModel>(),
        });

        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _serviceProvider.GetRequiredService<MainWindow>().Show();
        base.OnStartup(e);
    }
}
