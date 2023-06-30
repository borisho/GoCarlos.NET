using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;
using System.ComponentModel;
using System.Windows.Data;

namespace GoCarlos.NET.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IWindowService windowService;
    private readonly ITournament tournament;

    [ObservableProperty]
    private MenuViewModel menuViewModel;

    [ObservableProperty]
    private Player? selectedPlayer;

    public MainViewModel(IWindowService windowService, ITournament tournament, MenuViewModel menuViewModel)
    {
        this.windowService = windowService;
        this.tournament = tournament;
        this.menuViewModel = menuViewModel;

        Players = CollectionViewSource.GetDefaultView(tournament.Players);
    }

    public ICollectionView Players { get; }

    [RelayCommand]
    public void EditPlayer()
    {
        if (SelectedPlayer != null)
        {
            windowService.ShowPlayerWindow(SelectedPlayer);
        }
    }

    [RelayCommand]
    public void DeletePlayer()
    {
        if (SelectedPlayer != null)
        {
            tournament.Players.Remove(SelectedPlayer);
            SelectedPlayer = null;
        }
    }
}
