using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;
using GoCarlos.NET.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace GoCarlos.NET.Services;

/// <inheritdoc cref="IWindowService"/>
public sealed class WindowService(ITournament tournament, IServiceProvider serviceProvider) : IWindowService
{
    public void ShowAddPlayerWindow()
    {
        AddPlayerWindow window = serviceProvider.GetRequiredService<AddPlayerWindow>();
        window.GenerateCheckBoxes(tournament.Settings.GeneralSettings.NumberOfRounds);
        window.Show();
    }

    public void ShowAddPlayerWindowWithParam(bool param)
    {
        AddPlayerWindow window = serviceProvider.GetRequiredService<AddPlayerWindow>();
        window.GenerateCheckBoxes(tournament.Settings.GeneralSettings.NumberOfRounds);
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

    public void ShowSettingsWindow()
    {
        SettingsWindow window = serviceProvider.GetRequiredService<SettingsWindow>();
        window.ShowDialog();
    }

    public void Shutdown()
    {
        Application.Current.Shutdown();
    }
}
