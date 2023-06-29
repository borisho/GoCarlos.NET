using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace GoCarlos.NET.ViewModels;

public partial class EgdSelectionViewModel : ObservableObject
{
    private readonly ObservableCollection<IEgdData> players;

    public EgdSelectionViewModel()
    {
        players = new ObservableCollection<IEgdData>();

        Players = CollectionViewSource.GetDefaultView(players);
        Players.SortDescriptions.Add(new SortDescription(nameof(IEgdData.Last_Name), ListSortDirection.Ascending));
        Players.SortDescriptions.Add(new SortDescription(nameof(IEgdData.Name), ListSortDirection.Ascending));
    }

    public ICollectionView Players { get; }

    public void AddPlayers(IEgdData[] egdDatas)
    {
        foreach (IEgdData data in egdDatas)
        {
            players.Add(data);
        }
    }
}
