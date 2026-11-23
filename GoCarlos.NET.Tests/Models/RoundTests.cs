using GoCarlos.NET.Models;
using GoCarlos.NET.Models.Enums;

namespace GoCarlos.NET.Tests.Models;

public class RoundTests
{
    readonly Player player, opponent;

    public RoundTests()
    {
        player = new()
        {
            Data = new()
            {
                Name = "Test",
                Last_Name = "Player",
                Grade = "1d"
            }
        };

        opponent = new()
        {
            Data = new()
            {
                Name = "Opponent",
                Last_Name = "Player",
                Grade = "1d"
            }
        };
    }

    [Fact]
    public void AddPlayer_AddsPlayerToPlayersAndUnpairedPlayers()
    {
        Round round = new(1);
        round.AddPlayer(player);

        Assert.Contains(round.RoundNumber, player.RoundsPlaying);
        Assert.Contains(player, round.Players);
        Assert.Contains(player, round.UnpairedPlayers);
    }

    [Fact]
    public void RemovePlayer_PlayerNotPaired_RemovesPlayerFromPlayersAndUnpairedPlayers()
    {
        Round round = new(1)
        {
            Players = [player],
            UnpairedPlayers = [player]
        };
        round.RemovePlayer(player);

        Assert.DoesNotContain(round.RoundNumber, player.RoundsPlaying);
        Assert.DoesNotContain(player, round.Players);
        Assert.DoesNotContain(player, round.UnpairedPlayers);
    }

    [Fact]
    public void RemovePlayer_PlayerPaired_RemovePairingAndPlayerFromPlayersAndUnpairedPlayers()
    {
        Round round = new(1)
        {
            Players = [player, opponent],
            UnpairedPlayers = [player, opponent]
        };

        round.AddPairing(player, opponent, 9, true, false);

        Assert.Single(round.Pairings);

        round.RemovePlayer(player);

        Assert.Empty(round.Pairings);
        Assert.Empty(player.Pairings);
        Assert.Empty(opponent.Pairings);
        Assert.DoesNotContain(round.RoundNumber, player.RoundsPlaying);
        Assert.DoesNotContain(player, round.Players);
        Assert.DoesNotContain(player, round.UnpairedPlayers);
    }

    [Fact]
    public void AddPairing_AddsByePairing()
    {
        Player bye = new() { Group = Group.Bye };

        Round round = new(1)
        {
            Players = [player, bye],
            UnpairedPlayers = [player, bye]
        };

        Pairing? pairing = round.AddPairing(player, bye, 0, true, false);

        Assert.NotNull(pairing);
        Assert.Contains(pairing, round.Pairings);
        Assert.Equal(Result.BLACK_WON, pairing.Result);

        Assert.DoesNotContain(player, round.UnpairedPlayers);
        Assert.DoesNotContain(bye, round.UnpairedPlayers);

        Assert.Equal(bye, player.Opponents[round.RoundNumber]);
        Assert.Equal(player, bye.Opponents[round.RoundNumber]);

        Assert.Equal(-1, player.ColorBalancer[1]);
        Assert.Equal(1, player.ByeBalancer);
    }

    [Fact]
    public void AddPairing_ReturnNull_AlreadyByePairedInRound()
    {
        Player opponent = new();
        Player bye = new() { Group = Group.Bye };
        bye.Opponents[1] = opponent;

        Round round = new(1)
        {
            Players = [player, bye, opponent],
            UnpairedPlayers = [player]
        };

        Pairing? pairing = round.AddPairing(player, bye, 0, true, false);

        Assert.Null(pairing);
    }


    [Theory]
    [InlineData(true, 30, 20, "1d", "5k", 8, false)]
    [InlineData(true, 20, 30, "1d", "5k", 8, true)]
    [InlineData(false, 0, 0, "1d", "5k", 3, false)]
    [InlineData(false, 0, 0, "5k", "1d", 3, true)]
    public void AddPairing_AddsPairingAndUpdatesPlayers(
        bool handicapBasedMm,
        decimal playerScore,
        decimal opponentScore,
        string playerGrade,
        string opponentGrade,
        int expectedHandicap,
        bool expectPlayerBlack
    )
    {
        player.Score = playerScore;
        player.Grade = playerGrade;
        opponent.Score = opponentScore;
        opponent.Grade = opponentGrade;

        Round round = new(1)
        {
            Players = [player, opponent],
            UnpairedPlayers = [player, opponent]
        };

        Pairing? pairing = round.AddPairing(
            player, opponent, 2, handicapBasedMm, false);

        Assert.NotNull(pairing);
        Assert.Equal(expectedHandicap, pairing.Handicap);
        Assert.Same(expectPlayerBlack ? player : opponent, pairing.Black);
        Assert.Same(expectPlayerBlack ? opponent : player, pairing.White);

        Assert.Contains(pairing, round.Pairings);
        Assert.DoesNotContain(player, round.UnpairedPlayers);
        Assert.DoesNotContain(opponent, round.UnpairedPlayers);
        Assert.Same(opponent, player.Opponents[round.RoundNumber]);
        Assert.Same(player, opponent.Opponents[round.RoundNumber]);
    }

    [Fact]
    public void RemovePairing_RemovesPairingAndUpdatesPlayers()
    {
        Round round = new(1)
        {
            Players = [player, opponent],
            UnpairedPlayers = [player, opponent]
        };

        Pairing? pairing = round.AddPairing(player, opponent, 9, true, false);
        Assert.NotNull(pairing);
        pairing!.BoardNumber = 7;

        var result = round.RemovePairing(pairing);

        Assert.Equal((true, 7), result);
        Assert.Empty(round.Pairings);
        Assert.Contains(pairing.Black, round.UnpairedPlayers);
        Assert.Contains(pairing.White, round.UnpairedPlayers);

        Assert.False(pairing.Black.Pairings.ContainsKey(round.RoundNumber));
        Assert.False(pairing.White.Pairings.ContainsKey(round.RoundNumber));
        Assert.False(pairing.Black.Opponents.ContainsKey(round.RoundNumber));
        Assert.False(pairing.White.Opponents.ContainsKey(round.RoundNumber));

        Assert.Equal(0, pairing.Black.PairingBalancer[round.RoundNumber]);
        Assert.Equal(0, pairing.White.PairingBalancer[round.RoundNumber]);
        Assert.Equal(0, pairing.Black.ColorBalancer[round.RoundNumber]);
        Assert.Equal(0, pairing.White.ColorBalancer[round.RoundNumber]);
    }

    [Fact]
    public void RemovePairing_ByePairing_DecreasesByeBalancerWithoutReaddingBye()
    {
        Player bye = new() { Group = Group.Bye };
        Round round = new(1)
        {
            Players = [player, bye],
            UnpairedPlayers = [player, bye]
        };

        Pairing? pairing = round.AddPairing(player, bye, 0, true, false);
        Assert.NotNull(pairing);

        var result = round.RemovePairing(pairing!);

        Assert.Equal((true, 0), result);
        Assert.Empty(round.Pairings);
        Assert.Contains(pairing.Black, round.UnpairedPlayers);
        Assert.DoesNotContain(pairing.White, round.UnpairedPlayers);

        Assert.False(pairing.Black.Pairings.ContainsKey(round.RoundNumber));
        Assert.False(pairing.White.Pairings.ContainsKey(round.RoundNumber));
        Assert.False(pairing.Black.Opponents.ContainsKey(round.RoundNumber));
        Assert.False(pairing.White.Opponents.ContainsKey(round.RoundNumber));

        Assert.Equal(0, pairing.Black.PairingBalancer[round.RoundNumber]);
        Assert.Equal(0, pairing.White.PairingBalancer[round.RoundNumber]);
        Assert.Equal(0, pairing.Black.ColorBalancer[round.RoundNumber]);
        Assert.Equal(0, pairing.White.ColorBalancer[round.RoundNumber]);
    }

    [Fact]
    public void RemovePairing_PairingNotInRound_ReturnsFalseWithoutChangingState()
    {
        Round round = new(1)
        {
            Players = [player, opponent],
            UnpairedPlayers = [player, opponent]
        };
        Pairing pairing = new() { BoardNumber = 7 };

        var result = round.RemovePairing(pairing);

        Assert.Equal((false, -1), result);
        Assert.Empty(round.Pairings);
        Assert.Contains(player, round.UnpairedPlayers);
        Assert.Contains(opponent, round.UnpairedPlayers);
    }
}
