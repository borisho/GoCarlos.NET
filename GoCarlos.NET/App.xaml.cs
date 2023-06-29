using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;
using GoCarlos.NET.Services;
using GoCarlos.NET.ViewModels;
using GoCarlos.NET.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace GoCarlos.NET;

public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(options =>
                {
                    options.AddConsole().SetMinimumLevel(LogLevel.Information);
                    options.AddDebug().SetMinimumLevel(LogLevel.Trace);
                });

                services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

                services.AddTransient<IWindowService, WindowService>();
                services.AddTransient<IDialogService, DialogService>();
                services.AddTransient<IMenuItemsService, MenuItemsService>();
                services.AddSingleton<IEgdService, EgdService>();
                services.AddSingleton<ITournament, Tournament>();


                services.AddTransient<MenuViewModel>();

                services.AddTransient<EgdSelectionViewModel>();
                services.AddTransient(provider => new EgdSelectionWindow
                {
                    DataContext = provider.GetRequiredService<EgdSelectionViewModel>(),
                });

                services.AddTransient<AddPlayerViewModel>();
                services.AddTransient(provider => new AddPlayerWindow
                {
                    DataContext = provider.GetRequiredService<AddPlayerViewModel>(),
                });

                services.AddTransient<PlayerViewModel>();
                services.AddTransient(provider => new PlayerWindow
                {
                    DataContext = provider.GetRequiredService<PlayerViewModel>(),
                });

                services.AddSingleton<MainViewModel>();
                services.AddSingleton(provider => new MainWindow
                {
                    DataContext = provider.GetRequiredService<MainViewModel>(),
                });
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        AppHost.Services.GetRequiredService<MainWindow>().Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();

        base.OnExit(e);
    }
}
