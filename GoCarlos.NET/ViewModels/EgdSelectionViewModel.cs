using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace GoCarlos.NET.ViewModels;

public partial class EgdSelectionViewModel : ObservableObject
{
    private readonly ObservableCollection<EgdData> players;

    public EgdSelectionViewModel()
    {
        players = new ObservableCollection<EgdData>();

        Players = CollectionViewSource.GetDefaultView(players);
        Players.SortDescriptions.Add(new SortDescription(nameof(EgdData.Last_Name), ListSortDirection.Ascending));
        Players.SortDescriptions.Add(new SortDescription(nameof(EgdData.Name), ListSortDirection.Ascending));
    }

    public ICollectionView Players { get; }

    public void AddPlayers(EgdData[] egdDatas)
    {
        foreach (EgdData data in egdDatas)
        {
            players.Add(data);
        }
    }
}
