using GoCarlos.NET.UI.Interfaces;
using GoCarlos.NET.UI.Models;
using GoCarlos.NET.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GoCarlos.NET.UI.Services;

/// <inheritdoc cref="IWindowService"/>
public sealed class WindowService : IWindowService
{
    private readonly IServiceProvider serviceProvider;

    public WindowService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public void ShowAddPlayerWindow()
    {
        serviceProvider.GetRequiredService<AddPlayerWindow>().Show();
    }

    public void ShowAddPlayerWindowWithParam(bool param)
    {
        AddPlayerWindow window = serviceProvider.GetRequiredService<AddPlayerWindow>();
        window.AddOneMore(param);
        window.Show();
    }

    public void ShowEgdSelectionWindow(EgdData[] egdDatas)
    {
        EgdSelectionWindow window = serviceProvider.GetRequiredService<EgdSelectionWindow>();
        window.AddPlayers(egdDatas);
        window.Show();
    }
}
