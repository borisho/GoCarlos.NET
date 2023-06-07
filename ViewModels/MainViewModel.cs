using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace GoCarlos.NET.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    private MenuViewModel menuViewModel;

    private ObservableCollection<Player> players;

    public MainViewModel(MenuViewModel menuViewModel) {
        this.menuViewModel = menuViewModel;

        players = new();
        Players = CollectionViewSource.GetDefaultView(players);
    }

    public ICollectionView Players { get; }
}
