using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;

namespace GoCarlos.NET.Tests;

[TestClass]
public class TournamentTest
{

    private ITournament tournament;

    public TournamentTest()
    {
        tournament = new Tournament();
    }

    [TestMethod]
    public void Reset_CreateNewTournament_ClearPlayers()
    {
        tournament.Players.Add(new());

        tournament.Reset();

        Assert.AreEqual(tournament.Players.Count, 0);
    }

    [TestMethod]
    public void Reset_CreateNewTournament_ResetNumberOfRounds()
    {
        tournament.Rounds = 10;

        tournament.Reset();

        Assert.AreEqual(tournament.Rounds, 5);
    }
}
