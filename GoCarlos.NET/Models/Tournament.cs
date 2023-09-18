using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Messages;
using System.Collections.Generic;
using System.Linq;

namespace GoCarlos.NET.Models;

public partial class Tournament : ITournament, IRecipient<AddPlayerMessage>
{
    const int DEFAULT_NUMBER_OF_ROUNDS = 5;
    private int numberOfRounds, currentRound;

    private Settings settings;

    private List<Player> players;
    private List<Pairing> pairings;

    public Tournament()
    {
        numberOfRounds = DEFAULT_NUMBER_OF_ROUNDS;
        currentRound = 1;

        settings = new();

        players = new();
        pairings = new();

        WeakReferenceMessenger.Default.Register(this);
    }

    public int CurrentRound { get => currentRound; set => currentRound = value; }

    public int NumberOfRounds { get => numberOfRounds; set => numberOfRounds = value; }

    // SETTINGS
    public Settings Settings { get => settings; set => settings = value; }

    //PLAYERS AND PAIRINGS
    public IEnumerable<Player> Players => players;

    public IEnumerable<Pairing> Pairings => pairings;

    public IEnumerable<Player> UnpairedPlayers
    {
        get
        {
            foreach (Player player in players)
            {
                if (player.RoundsPlaying[CurrentRound - 1] &&
                    !pairings.Where(p => p.RoundNumber == CurrentRound)
                        .Where(p => p.ContainsPlayer(player))
                        .Any())
                {
                    yield return player;
                }
            }
        }
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);

        // Send notification for UI refresh
        WeakReferenceMessenger.Default.Send(new BoolMessage(true), ITournament.TOKEN_REFRESH_MAIN_VIEW_MODEL);
    }

    public void RemovePlayer(Player player)
    {
        players.Remove(player);

        // Send notification for UI refresh
        WeakReferenceMessenger.Default.Send(new BoolMessage(true), ITournament.TOKEN_REFRESH_MAIN_VIEW_MODEL);
    }

    public void AddPairing(Pairing pairing)
    {
        pairings.Add(pairing);

        // Send notification for UI refresh
        WeakReferenceMessenger.Default.Send(new BoolMessage(true), ITournament.TOKEN_REFRESH_MAIN_VIEW_MODEL);
    }

    public void RemovePairing(Pairing pairing)
    {
        pairings.Remove(pairing);

        // Send notification for UI refresh
        WeakReferenceMessenger.Default.Send(new BoolMessage(true), ITournament.TOKEN_REFRESH_MAIN_VIEW_MODEL);
    }

    public void Reset()
    {
        numberOfRounds = DEFAULT_NUMBER_OF_ROUNDS;

        players.Clear();
        pairings.Clear();
    }

    public void Receive(AddPlayerMessage message) => AddPlayer(message.Value);
}
