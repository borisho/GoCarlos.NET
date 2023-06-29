using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Messages;
using System.Collections.ObjectModel;

namespace GoCarlos.NET.Models;

public partial class Tournament : ObservableObject, ITournament, IRecipient<AddPlayerMessage>
{
    const int DEFAULT_NUMBER_OF_ROUNDS = 5;

    [ObservableProperty]
    private int rounds;

    [ObservableProperty]
    private ObservableCollection<Player> players;

    public Tournament()
    {
        rounds = DEFAULT_NUMBER_OF_ROUNDS;
        players = new();

        WeakReferenceMessenger.Default.Register(this);
    }

    public void Receive(AddPlayerMessage message)
    {
        Players.Add(message.Value);
    }
}
