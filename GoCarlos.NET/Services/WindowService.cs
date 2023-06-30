using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;
using GoCarlos.NET.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GoCarlos.NET.Services;

/// <inheritdoc cref="IWindowService"/>
public sealed class WindowService : IWindowService
{
    private readonly ITournament tournament;
    private readonly IServiceProvider serviceProvider;

    public WindowService(ITournament tournament, IServiceProvider serviceProvider)
    {
        this.tournament = tournament;
        this.serviceProvider = serviceProvider;
    }

    public void ShowAddPlayerWindow()
    {
        AddPlayerWindow window = serviceProvider.GetRequiredService<AddPlayerWindow>();
        window.GenerateCheckBoxes(tournament.Rounds);
        window.Show();
    }

    public void ShowAddPlayerWindowWithParam(bool param)
    {
        AddPlayerWindow window = serviceProvider.GetRequiredService<AddPlayerWindow>();
        window.GenerateCheckBoxes(tournament.Rounds);
        window.AddOneMore(param);
        window.Show();
    }

    public void ShowPlayerWindow(Player player)
    {
        PlayerWindow window = serviceProvider.GetRequiredService<PlayerWindow>();
        window.SetSelectedPlayer(player);
        window.Show();
    }

    public void ShowEgdSelectionWindow(EgdData[] egdDatas)
    {
        EgdSelectionWindow window = serviceProvider.GetRequiredService<EgdSelectionWindow>();
        window.AddPlayers(egdDatas);
        window.Show();
    }
}
