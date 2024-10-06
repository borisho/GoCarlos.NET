using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.Enums;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Messages;
using GoCarlos.NET.Models;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Data;

namespace GoCarlos.NET.ViewModels;

public partial class MainViewModel : ObservableObject, IRecipient<EmptyMessage>, IRecipient<BoolMessage>, IReferenceCleanup
{
    private readonly ITournament tournament;
    private readonly IWindowService windowService;
    private readonly ILocalizerService localizerService;

    [ObservableProperty]
    private MenuViewModel menuViewModel;

    [ObservableProperty]
    private DataTable dataTable;

    [ObservableProperty]
    private Player? selectedPlayer;

    [ObservableProperty]
    private Pairing? selectedPairing;

    public MainViewModel(ITournament tournament,
        IWindowService windowService,
        ILocalizerFactory localizerFactory,
        MenuViewModel menuViewModel)
    {
        this.windowService = windowService;
        this.tournament = tournament;
        this.menuViewModel = menuViewModel;

        // Get localizer service from factory
        localizerService = localizerFactory.GetByQualifier(LocalizerType.MainViewService);

        dataTable = CreateDataTable();

        Players = CollectionViewSource.GetDefaultView(tournament.Players);
        Pairings = CollectionViewSource.GetDefaultView(tournament.Pairings);
        UnpairedPlayers = CollectionViewSource.GetDefaultView(tournament.UnpairedPlayers);

        WeakReferenceMessenger.Default.Register<BoolMessage, int>(this, ITournament.TOKEN_REFRESH_MAIN_VIEW_MODEL);
        WeakReferenceMessenger.Default.Register<EmptyMessage, int>(this, ITournament.TOKEN_ROUND_CHANGE);
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
            //SelectedPlayer = null;
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
            //SelectedPairing = null;
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

    public void Receive(EmptyMessage message)
    {
        // TODO create new table based on current round number
    }

    public void Unregister()
    {
        tournament.Unregister();
        WeakReferenceMessenger.Default.Unregister<BoolMessage, int>(this, ITournament.TOKEN_REFRESH_MAIN_VIEW_MODEL);
        WeakReferenceMessenger.Default.Unregister<EmptyMessage, int>(this, ITournament.TOKEN_REFRESH_MAIN_VIEW_MODEL);
    }

    private DataTable CreateDataTable()
    {
        var table = new DataTable();

        // Create fix columns
        table.Columns.Add(localizerService["Place"]);
        table.Columns.Add(localizerService["Name"]);
        table.Columns.Add(localizerService["Club"]);
        table.Columns.Add(localizerService["Grade"]);
        table.Columns.Add(localizerService["Rating"]);

        // Create dynamic columns

        // Fill with ScoredPlayers

        return table;
    }
}
