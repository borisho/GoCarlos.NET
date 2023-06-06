using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.Messages;
using GoCarlos.NET.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace GoCarlos.NET.ViewModels;

public partial class MainViewModel : ObservableRecipient, IRecipient<AddPlayerMessage>
{
    [ObservableProperty]
    private MenuViewModel menuViewModel;

    private ObservableCollection<Player> players;

    public MainViewModel(MenuViewModel menuViewModel) {
        this.menuViewModel = menuViewModel;

        players = new();
        Players = CollectionViewSource.GetDefaultView(players);

        WeakReferenceMessenger.Default.Register<AddPlayerMessage>(this);
    }

    public ICollectionView Players { get; }

    public void Receive(AddPlayerMessage message)
    {
        players.Add(message.Value);
        Players.Refresh();
    }
}
