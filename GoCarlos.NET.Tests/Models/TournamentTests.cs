// Copyright (C) 2023 Boris Dovčík
// This file is part of GoCarlos.NET.
//
// GoCarlos.NET is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 3, or (at your option) any
// later version.
//
// GoCarlos.NET is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with GoCarlos.NET. If not, see <https://www.gnu.org/licenses/>.

using GoCarlos.NET.Models;
using GoCarlos.NET.Models.Enums;

namespace GoCarlos.NET.Tests.Models;

public class TournamentTests
{
    readonly Player player;
    readonly Tournament tournament;

    public TournamentTests()
    {
        player = new()
        {
            Data = new()
            {
                Name = "Test",
                Last_Name = "Player",
                Grade = "1d"
            },
            Score = 30m,
            Grade = "1d"
        };
        player.RoundsPlaying.Add(0);
        player.RoundsPlaying.Add(2);

        tournament = new Tournament(3);
        tournament.Players.Add(player);
    }

    [Fact]
    public void UpdatePlayerPlayingRounds_ShouldAddPlayerToCorrectRounds()
    {
        tournament.UpdatePlayerPlayingRounds(player);
        var rounds = tournament.Rounds;

        Assert.Single(rounds[0].Players);
        Assert.Empty(rounds[1].Players);
        Assert.Single(rounds[2].Players);
    }

    [Theory]
    [InlineData(0, 0, 30)]
    [InlineData(1, 0, 29)]
    [InlineData(10, 0, 20)]
    [InlineData(30, 0, 0)]
    [InlineData(22, 8, 0)]
    [InlineData(4, 1, 25)]
    [InlineData(3, 2, 25)]
    [InlineData(2, 3, 25)]
    [InlineData(1, 4, 25)]
    public void UpdatePairingHandicaps_BasedOnMM_NoHandicapCap_EqualsExcpectedValue(
        int expectedHandicap,
        int handicapReduction,
        decimal score
    )
    {
        Player opponent = new()
        {
            Data = new EGD_Data()
            {
                Name = "Opponent",
                Last_Name = "Player"
            },
            Score = 0m
        };

        Pairing? pairing = tournament.Rounds[0]
            .AddPairing(player, opponent, 0, true, false);

        Assert.NotNull(pairing);
        Assert.Equal(30, pairing.Handicap);

        opponent.Score = score;
        tournament.HandicapReduction = handicapReduction;
        tournament.HandicapBasedMm = true;
        tournament.HandicapMaxNine = false;
        tournament.UpdatePairingHandicaps();

        Assert.Equal(expectedHandicap, pairing.Handicap);
    }

    [Theory]
    [InlineData(0, 0, 30)]
    [InlineData(1, 0, 29)]
    [InlineData(9, 0, 20)]
    [InlineData(9, 0, 0)]
    [InlineData(9, 8, 0)]
    [InlineData(4, 1, 25)]
    [InlineData(3, 2, 25)]
    [InlineData(2, 3, 25)]
    [InlineData(1, 4, 25)]
    public void UpdatePairingHandicaps_BasedOnMM_HandicapCap_EqualsExcpectedValue(
        int expectedHandicap,
        int handicapReduction,
        decimal score
    )
    {
        Player opponent = new()
        {
            Data = new EGD_Data()
            {
                Name = "Opponent",
                Last_Name = "Player"
            },
            Score = 0m
        };

        Pairing? pairing = tournament.Rounds[0]
            .AddPairing(player, opponent, 0, true, true);

        Assert.NotNull(pairing);
        Assert.Equal(9, pairing.Handicap);

        opponent.Score = score;
        tournament.HandicapReduction = handicapReduction;
        tournament.HandicapBasedMm = true;
        tournament.HandicapMaxNine = true;
        tournament.UpdatePairingHandicaps();

        Assert.Equal(expectedHandicap, pairing.Handicap);
    }

    [Theory]
    [InlineData(0, 0, "1d")]
    [InlineData(1, 0, "1k")]
    [InlineData(10, 0, "10k")]
    [InlineData(30, 0, "30k")]
    [InlineData(22, 8, "30k")]
    [InlineData(4, 1, "5k")]
    [InlineData(3, 2, "5k")]
    [InlineData(2, 3, "5k")]
    [InlineData(1, 4, "5k")]
    public void UpdatePairingHandicaps_BasedOnGrade_NoHandicapCap_EqualsExcpectedValue(
        int expectedHandicap,
        int handicapReduction,
        string grade
    )
    {
        Player opponent = new()
        {
            Data = new EGD_Data()
            {
                Name = "Opponent",
                Last_Name = "Player"
            },
            Grade = "30k"
        };

        Pairing? pairing = tournament.Rounds[0]
            .AddPairing(player, opponent, 0, false, false);

        Assert.NotNull(pairing);
        Assert.Equal(30, pairing.Handicap);

        opponent.Grade = grade;
        tournament.HandicapReduction = handicapReduction;
        tournament.HandicapBasedMm = false;
        tournament.HandicapMaxNine = false;
        tournament.UpdatePairingHandicaps();

        Assert.Equal(expectedHandicap, pairing.Handicap);
    }

    [Theory]
    [InlineData(0, 0, "1d")]
    [InlineData(1, 0, "1k")]
    [InlineData(9, 0, "10k")]
    [InlineData(9, 0, "30k")]
    [InlineData(9, 8, "30k")]
    [InlineData(4, 1, "5k")]
    [InlineData(3, 2, "5k")]
    [InlineData(2, 3, "5k")]
    [InlineData(1, 4, "5k")]
    public void UpdatePairingHandicaps_BasedOnGrade_HandicapCap_EqualsExcpectedValue(
        int expectedHandicap,
        int handicapReduction,
        string grade
    )
    {
        Player opponent = new()
        {
            Data = new EGD_Data()
            {
                Name = "Opponent",
                Last_Name = "Player"
            },
            Grade = "30k"
        };

        Pairing? pairing = tournament.Rounds[0]
            .AddPairing(player, opponent, 0, false, true);

        Assert.NotNull(pairing);
        Assert.Equal(9, pairing.Handicap);

        opponent.Grade = grade;
        tournament.HandicapReduction = handicapReduction;
        tournament.HandicapBasedMm = false;
        tournament.HandicapMaxNine = true;
        tournament.UpdatePairingHandicaps();

        Assert.Equal(expectedHandicap, pairing.Handicap);
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(-10)]
    [InlineData(0)]
    public void AddOrRemoveRounds_ZeroAndLess_ThrowsError(int expectedRounds)
    {
        void act() => tournament.NumberOfRounds = expectedRounds;
        ArgumentException exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("Number of rounds must be at least 1!", exception.Message);
    }

    [Theory]
    [InlineData(11)]
    [InlineData(55)]
    [InlineData(100)]
    public void AddOrRemoveRounds_ElevenAndMore_ThrowsError(int expectedRounds)
    {
        void act() => tournament.NumberOfRounds = expectedRounds;
        ArgumentException exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("Number of rounds must be at most 10!", exception.Message);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void AddOrRemoveRounds_ExpectedNumberOfRoundsIsPresent(int expectedRounds)
    {
        tournament.NumberOfRounds = expectedRounds;
        Assert.Equal(expectedRounds, tournament.Rounds.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CalculateCriteria_RoundLessThanOne_ResetsPlayerCriteria(int roundNumber)
    {
        player.Points = 12m;
        player.Score = 13m;
        player.ScoreX = 14m;
        player.SOS = 15m;
        player.SOSOS = 16m;
        player.SODOS = 17m;

        tournament.CalculateCriteria(roundNumber);

        Assert.Equal(0m, player.Points);
        Assert.Equal(30m, player.Score);
        Assert.Equal(30m, player.ScoreX);
        Assert.Equal(0m, player.SOS);
        Assert.Equal(0m, player.SOSOS);
        Assert.Equal(0m, player.SODOS);
    }

    [Fact]
    public void CalculateCriteria_UnpairedRound_AwardsHalfPointAndUsesStartScoreForTieBreakers()
    {
        tournament.CalculateCriteria(1);

        Assert.Equal(0m, player.Points);
        Assert.Equal(30.5m, player.Score);
        Assert.Equal(30m, player.ScoreX);
        Assert.Equal(30m, player.SOS);
        Assert.Equal(0m, player.SODOS);
        Assert.Equal(30m, player.SOSOS);
    }

    [Fact]
    public void CalculateCriteria_PairedRound_CalculatesScoresAndTieBreakersFromOpponent()
    {
        var opponent = new Player
        {
            Data = new EGD_Data { Name = "Opponent", Last_Name = "Player" },
            Grade = "1d"
        };
        tournament.Players.Add(opponent);

        var pairing = tournament.Rounds[0]
            .AddPairing(player, opponent, 0, false, false);
        Assert.NotNull(pairing);
        pairing!.Result = Result.DRAW;

        tournament.CalculateCriteria(1);

        Assert.Equal(0.5m, player.Points);
        Assert.Equal(30.5m, player.Score);
        Assert.Equal(30.5m, player.ScoreX);
        Assert.Equal(30.5m, player.SOS);
        Assert.Equal(15.25m, player.SODOS);
        Assert.Equal(30.5m, player.SOSOS);
    }

    [Fact]
    public void ResetBoardNumbers_SortsByPlace()
    {
        var firstByPlace = new Pairing(
            new Player { Place = 1 },
            new Player { Place = 8 },
            0, "", 0);

        var secondByRating = new Pairing(
            new Player { Place = 7 },
            new Player { Place = 2 },
            0, "", 0);

        var thirdByRating = new Pairing(
            new Player { Place = 3 },
            new Player { Place = 6 },
            0, "", 0);

        var fourthByPlace = new Pairing(
            new Player { Place = 4 },
            new Player { Place = 5 },
            0, "", 0);

        var expectedOrder = new[]
        {
            firstByPlace,
            secondByRating,
            thirdByRating,
            fourthByPlace
        };

        List<Pairing> pairings = tournament.Rounds[0].Pairings;

        foreach (var pairing in expectedOrder)
        {
            pairing.BoardNumber = 99;
            pairings.Add(pairing);
        }

        tournament.ResetBoardNumbers();

        Assert.Equal(expectedOrder, pairings.OrderBy(p => p.BoardNumber));
        Assert.Equal([1, 2, 3, 4], expectedOrder.Select(p => p.BoardNumber));
    }
}
