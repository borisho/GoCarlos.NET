using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;

namespace GoCarlos.NET.Tests;

[TestClass]
public class TournamentTest
{

    private readonly ITournament tournament;

    public TournamentTest()
    {
        tournament = new Tournament();
    }

    [TestMethod]
    public void AddPlayer_AddPlayer_PlayerAddedToList()
    {
        tournament.AddPlayer(new());

        Assert.AreEqual(tournament.Players.Count(), 1);
    }

    [TestMethod]
    public void RemovePlayer_RemovePlayer_PlayerRemovedFromList()
    {
        Player player = new();
        tournament.AddPlayer(player);

        tournament.RemovePlayer(player);

        Assert.AreEqual(tournament.Players.Count(), 0);
    }

    [TestMethod]
    public void Reset_CreateNewTournament_ClearPlayers()
    {
        tournament.AddPlayer(new());

        tournament.Reset();

        Assert.AreEqual(tournament.Players.Count(), 0);
    }

    [TestMethod]
    public void Reset_CreateNewTournament_ClearPairings()
    {
        tournament.AddPairing(new(tournament.CurrentRound));

        tournament.Reset();

        Assert.AreEqual(tournament.Pairings.Count(), 0);
    }

    [TestMethod]
    public void Reset_CreateNewTournament_ResetNumberOfRounds()
    {
        tournament.SetNumberOfRounds(10);

        tournament.Reset();

        Assert.AreEqual(tournament.NumberOfRounds, 5);
    }
}
