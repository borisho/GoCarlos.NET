using GoCarlos.NET.UI.Enums;
using GoCarlos.NET.UI.Interfaces;
using GoCarlos.NET.UI.Models;
using GoCarlos.NET.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GoCarlos.NET.UI.Services;

public sealed class WindowService : IWindowService
{
    private readonly IServiceProvider serviceProvider;

    public WindowService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public void Show(Windows type)
    {
        switch (type)
        {
            case Windows.AddPlayerWindow:
                AddPlayerWindow();
                break;
            default:
                break;
        }
    }

    public void ShowEgdSelectionWindow(EgdData[] egdDatas)
    {
        EgdSelectionWindow window = serviceProvider.GetRequiredService<EgdSelectionWindow>();
        window.AddPlayers(egdDatas);
        window.Show();
    }

    private void AddPlayerWindow()
    {
        serviceProvider.GetRequiredService<AddPlayerWindow>().Show();
    }
}
