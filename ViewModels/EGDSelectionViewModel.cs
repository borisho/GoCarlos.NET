using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace GoCarlos.NET.ViewModels;

public partial class EGDSelectionViewModel : ObservableObject
{
    private readonly PlayerWindowViewModel playerWindowViewModel;
    private readonly ObservableCollection<EGD_Data> players;

    [ObservableProperty]
    private EGD_Data? selectedItem;

    public EGDSelectionViewModel(PlayerWindowViewModel playerWindowViewModel, EGD_Data[] eGD_Datas)
    {

        this.playerWindowViewModel = playerWindowViewModel;
        players = new ObservableCollection<EGD_Data>();

        foreach (EGD_Data data in eGD_Datas)
        {
            players.Add(data);
        }

        Players = CollectionViewSource.GetDefaultView(players);
        Players.SortDescriptions.Add(new SortDescription(nameof(EGD_Data.Last_Name), ListSortDirection.Ascending));
        Players.SortDescriptions.Add(new SortDescription(nameof(EGD_Data.Name), ListSortDirection.Ascending));
    }

    public ICollectionView Players { get; }

    public void SendSelectedPlayer(ICloseable window, EGD_Data player)
    {
        playerWindowViewModel.Data = player;

        window?.Close();
    }
}
