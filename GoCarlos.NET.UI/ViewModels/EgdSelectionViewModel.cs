using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.UI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace GoCarlos.NET.UI.ViewModels;

public partial class EgdSelectionViewModel : ObservableObject
{
    private readonly ObservableCollection<EgdData> players;

    public EgdSelectionViewModel(EgdData[] egdDatas)
    {
        players = new ObservableCollection<EgdData>();

        foreach (EgdData data in egdDatas)
        {
            players.Add(data);
        }

        Players = CollectionViewSource.GetDefaultView(players);
        Players.SortDescriptions.Add(new SortDescription(nameof(EgdData.Last_Name), ListSortDirection.Ascending));
        Players.SortDescriptions.Add(new SortDescription(nameof(EgdData.Name), ListSortDirection.Ascending));
    }

    public ICollectionView Players { get; }
}
