using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;
using GoCarlos.NET.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GoCarlos.NET.Services;

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
        AddPlayerWindow window = serviceProvider.GetRequiredService<AddPlayerWindow>();
        window.GenerateCheckBoxes(5);
        window.Show();
    }

    public void ShowAddPlayerWindowWithParam(bool param)
    {
        AddPlayerWindow window = serviceProvider.GetRequiredService<AddPlayerWindow>();
        window.GenerateCheckBoxes(5);
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
