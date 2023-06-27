using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Messages;
using System.Collections.ObjectModel;

namespace GoCarlos.NET.Models;

public class Tournament : ObservableObject, ITournament, IRecipient<AddPlayerMessage>
{
    private ObservableCollection<Player> players;

    public Tournament()
    {
        players = new();

        WeakReferenceMessenger.Default.Register(this);
    }

    public ObservableCollection<Player> Players
    {
        get => players;
        set
        {
            players = value;
            OnPropertyChanged(nameof(Players));
        }
    }

    public void Receive(AddPlayerMessage message)
    {
        Players.Add(new()
        {
            Pin = message.Pin,
            LastName = message.LastName,
            FirstName = message.FirstName,
            Gor = message.Gor,
            Grade = message.Grade,
            Club = message.Club,
            CountryCode = message.CountryCode,
        });
    }
}
