using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Messages;
using GoCarlos.NET.Models;
using System.Collections;
using System.ComponentModel;
using System.Windows.Data;

namespace GoCarlos.NET.ViewModels;

public partial class MainViewModel : ObservableObject, IRecipient<BoolMessage>, IReferenceCleanup
{
    private readonly IWindowService windowService;
    private readonly ITournament tournament;

    [ObservableProperty]
    private MenuViewModel menuViewModel;

    [ObservableProperty]
    private Player? selectedPlayer;

    [ObservableProperty]
    private Pairing? selectedPairing;

    public MainViewModel(IWindowService windowService, ITournament tournament, MenuViewModel menuViewModel)
    {
        this.windowService = windowService;
        this.tournament = tournament;
        this.menuViewModel = menuViewModel;

        Players = CollectionViewSource.GetDefaultView(tournament.Players);
        Pairings = CollectionViewSource.GetDefaultView(tournament.Pairings);
        UnpairedPlayers = CollectionViewSource.GetDefaultView(tournament.UnpairedPlayers);

        WeakReferenceMessenger.Default.Register(this, ITournament.TOKEN_REFRESH_MAIN_VIEW_MODEL);
    }

    public ICollectionView Players { get; }

    public ICollectionView Pairings { get; }

    public ICollectionView UnpairedPlayers { get; }

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
            //Delete Player
            SelectedPlayer = null;
        }
    }

    [RelayCommand]
    public void EditPairing()
    {
        if (SelectedPairing != null)
        {
            //windowService.ShowPairingWindow(SelectedPairing);
        }
    }

    [RelayCommand]
    public void DeletePairing()
    {
        if (SelectedPairing != null)
        {
            //Delete pairing
            SelectedPairing = null;
        }
    }

    [RelayCommand]
    public void MakePairing(IList selectedPlayers)
    {
        if (selectedPlayers is not null)
        {
            // TODO
        }
    }

    public void Receive(BoolMessage message)
    {
        if (message.Value)
        {
            Players.Refresh();
            Pairings.Refresh();
            UnpairedPlayers.Refresh();
        }
    }

    public void Unregister()
    {
        tournament.Unregister();
        WeakReferenceMessenger.Default.Unregister<BoolMessage, int>(this, ITournament.TOKEN_REFRESH_MAIN_VIEW_MODEL);
    }
}
