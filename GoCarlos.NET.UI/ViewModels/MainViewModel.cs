using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.UI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace GoCarlos.NET.UI.ViewModels;

public partial class MainViewModel : ObservableRecipient, IRecipient<Player>
{
    [ObservableProperty]
    private MenuViewModel menuViewModel;

    private readonly ObservableCollection<Player> players;

    public MainViewModel(MenuViewModel menuViewModel) {
        this.menuViewModel = menuViewModel;

        players = new();
        Players = CollectionViewSource.GetDefaultView(players);

        WeakReferenceMessenger.Default.Register(this);
    }

    public ICollectionView Players { get; }

    public void Receive(Player player)
    {
        players.Add(player);
    }
}
