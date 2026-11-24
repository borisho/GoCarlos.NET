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
using GoCarlos.NET.Models.Converters;
using GoCarlos.NET.Models.Enums;

namespace GoCarlos.NET.Tests.Models;

public class UtilsTests
{
    private static readonly string[] Grades =
    [
        "9p", "8p", "7p", "6p", "5p", "4p", "3p", "2p", "1p",
        "9d", "8d", "7d", "6d", "5d", "4d", "3d", "2d", "1d",
        "1k", "2k", "3k", "4k", "5k", "6k", "7k", "8k", "9k",
        "10k", "11k", "12k", "13k", "14k", "15k", "16k", "17k",
        "18k", "19k", "20k", "21k", "22k", "23k", "24k", "25k",
        "26k", "27k", "28k", "29k", "30k"
    ];

    private static readonly int[] Ratings =
    [
        2940, 2910, 2880, 2850, 2820, 2790, 2760, 2730, 2700,
        2900, 2800, 2700, 2600, 2500, 2400, 2300, 2200, 2100,
        2000, 1900, 1800, 1700, 1600, 1500, 1400, 1300, 1200,
        1100, 1000, 900, 800, 700, 600, 500, 400, 300, 200,
        100, 0, -100, -200, -300, -400, -500, -600, -700, -800, -900
    ];

    [Fact]
    public void ConstantsAndJsonSettings_HaveExpectedValues()
    {
        Assert.Equal("0+", Utils.BYE);
        Assert.Equal("?", Utils.QUESTION_MARK);
        Assert.Equal("=", Utils.EQUALS);
        Assert.Equal("-", Utils.DASH);
        Assert.Equal("+", Utils.PLUS);
        Assert.Contains(Utils.JsonSerializerSettings.Converters, converter => converter is DecimalConverter);
        Assert.Equal(Newtonsoft.Json.PreserveReferencesHandling.Objects, Utils.JsonSerializerSettings.PreserveReferencesHandling);
        Assert.Equal(Newtonsoft.Json.ReferenceLoopHandling.Serialize, Utils.JsonSerializerSettings.ReferenceLoopHandling);
        Assert.Equal(Newtonsoft.Json.Formatting.None, Utils.JsonSerializerSettings.Formatting);
        Assert.Equal(256, Utils.JsonSerializerSettings.MaxDepth);
        Assert.NotNull(Utils.Random);
    }

    [Fact]
    public void DropLast_HandlesEmptySingleAndMultipleValues()
    {
        int[] singleElement = [1];
        int[] multipleElements = [1, 2, 3];

        Assert.Empty(Array.Empty<int>().DropLast());
        Assert.Empty(singleElement.DropLast());
        Assert.Equal([1, 2], multipleElements.DropLast());
    }

    [Fact]
    public void CalculateHandicap_ReturnsZeroWhenReductionExceedsGradeDifference()
    {
        Assert.Equal(0, Utils.CalculateHandicap(CreatePlayer("1d"), CreatePlayer("2d"), 2, false, false));
    }

    [Theory]
    [InlineData(false, 0, 30, 30, 0)]
    [InlineData(false, 0, 30, 29, 1)]
    [InlineData(false, 0, 30, 20, 10)]
    [InlineData(false, 0, 30, 0, 30)]
    [InlineData(false, 8, 30, 0, 22)]
    [InlineData(false, 1, 30, 25, 4)]
    [InlineData(false, 2, 30, 25, 3)]
    [InlineData(false, 3, 30, 25, 2)]
    [InlineData(false, 4, 30, 25, 1)]
    [InlineData(true, 0, 30, 30, 0)]
    [InlineData(true, 0, 30, 29, 1)]
    [InlineData(true, 0, 30, 20, 9)]
    [InlineData(true, 0, 30, 0, 9)]
    [InlineData(true, 8, 30, 0, 9)]
    [InlineData(true, 1, 30, 25, 4)]
    [InlineData(true, 2, 30, 25, 3)]
    [InlineData(true, 3, 30, 25, 2)]
    [InlineData(true, 4, 30, 25, 1)]
    public void CalculateHandicap_BasedOnMM_EqualsExcpectedValue(
        bool maxNine,
        int reduction,
        decimal strongerScore,
        decimal weakerScore,
        int expected)
    {
        var stronger = CreatePlayer(strongerScore);
        var weaker = CreatePlayer(weakerScore);

        Assert.Equal(expected, Utils.CalculateHandicap(stronger, weaker, reduction, true, maxNine));
    }

    [Theory]
    [InlineData(false, 0, "1d", "1d", 0)]
    [InlineData(false, 0, "1d", "1k", 1)]
    [InlineData(false, 0, "1d", "10k", 10)]
    [InlineData(false, 0, "1d", "30k", 30)]
    [InlineData(false, 8, "1d", "30k", 22)]
    [InlineData(false, 1, "1d", "5k", 4)]
    [InlineData(false, 2, "1d", "5k", 3)]
    [InlineData(false, 3, "1d", "5k", 2)]
    [InlineData(false, 4, "1d", "5k", 1)]
    [InlineData(true, 0, "1d", "1d", 0)]
    [InlineData(true, 0, "1d", "1k", 1)]
    [InlineData(true, 0, "1d", "10k", 9)]
    [InlineData(true, 0, "1d", "30k", 9)]
    [InlineData(true, 8, "1d", "30k", 9)]
    [InlineData(true, 1, "1d", "5k", 4)]
    [InlineData(true, 2, "1d", "5k", 3)]
    [InlineData(true, 3, "1d", "5k", 2)]
    [InlineData(true, 4, "1d", "5k", 1)]
    public void CalculateHandicap_BasedOnGrade_EqualsExcpectedValue(
        bool maxNine,
        int reduction,
        string strongerGrade,
        string weakerGrade,
        int expected)
    {
        var stronger = CreatePlayer(strongerGrade);
        var weaker = CreatePlayer(weakerGrade);

        Assert.Equal(expected, Utils.CalculateHandicap(stronger, weaker, reduction, false, maxNine));
    }

    [Fact]
    public void GetHandicapGrade_SelectsGradeOrRoundedScore()
    {
        var player = CreatePlayer("1d");
        player.Score = 4.5m;

        Assert.Equal(30, Utils.GetHandicapGrade(player, false));
        Assert.Equal(5, Utils.GetHandicapGrade(player, true));
    }

    [Fact]
    public void Next_AdvancesAndWrapsEnumsAndReturnsDefaultForNonEnumTypes()
    {
        Assert.Equal(Group.TopGroup, Utils.Next(Group.SuperGroup));
        Assert.Equal(Group.Bye, Utils.Next(Group.Default));
        Assert.Equal(Group.SuperGroup, Utils.Next(Group.Bye));
        Assert.Equal(Group.SuperGroup, Utils.Next((Group)99));
        Assert.Equal(Result.BLACK_WON, Utils.Next(Result.NONE));
        Assert.Equal(0, Utils.Next(42));
    }

    [Fact]
    public void GetValueAndGetGrade_ConvertAllRanksAndUseFallbackValues()
    {
        for (int i = 0; i < Grades.Length; i++)
        {
            int value = 47 - i;
            Assert.Equal(value, Utils.GetValue(Grades[i]));
            Assert.Equal(Grades[i], Utils.GetGrade(value));
        }

        Assert.Equal(0, Utils.GetValue("unknown"));
        Assert.Equal(0, Utils.GetValue("1D"));
        Assert.Equal("30k", Utils.GetGrade(-1));
        Assert.Equal("30k", Utils.GetGrade(48));
    }

    [Fact]
    public void GetRating_MapsEveryRankAndUsesFallback()
    {
        for (int i = 0; i < Grades.Length; i++)
            Assert.Equal(Ratings[i], Utils.GetRating(Grades[i]));
        Assert.Equal(-900, Utils.GetRating("unknown"));
    }

    [Fact]
    public void GetGradeFromRating_UsesEveryThresholdAndFallback()
    {
        (int Rating, string Grade)[] thresholds =
        [
            (2850, "9d"), (2750, "8d"), (2650, "7d"), (2550, "6d"),
            (2450, "5d"), (2350, "4d"), (2250, "3d"), (2150, "2d"),
            (2050, "1d"), (1950, "1k"), (1850, "2k"), (1750, "3k"),
            (1650, "4k"), (1550, "5k"), (1450, "6k"), (1350, "7k"),
            (1250, "8k"), (1150, "9k"), (1050, "10k"), (950, "11k"),
            (850, "12k"), (750, "13k"), (650, "14k"), (550, "15k"),
            (450, "16k"), (350, "17k"), (250, "18k"), (150, "19k"),
            (50, "20k"), (-50, "21k"), (-150, "22k"), (-250, "23k"),
            (-350, "24k"), (-450, "25k"), (-550, "26k"), (-650, "27k"),
            (-750, "28k"), (-850, "29k"), (-900, "30k")
        ];

        foreach ((int rating, string grade) in thresholds)
            Assert.Equal(grade, Utils.GetGradeFromRating(rating));

        Assert.Equal("9d", Utils.GetGradeFromRating(3000));
        Assert.Equal("", Utils.GetGradeFromRating(-901));
    }

    [Theory]
    [InlineData(CriteriaType.NUL)]
    [InlineData(CriteriaType.POV)]
    [InlineData(CriteriaType.MMS)]
    [InlineData(CriteriaType.RAT)]
    [InlineData(CriteriaType.TRD)]
    [InlineData(CriteriaType.SOS)]
    [InlineData(CriteriaType.SDS)]
    [InlineData(CriteriaType.SSS)]
    [InlineData(CriteriaType.VZP)]
    public void GetOrderedPlayerList_OrdersByPrimaryCriterion(CriteriaType type)
    {
        var intendedWinner = CreatePlayer("2d");
        var other = CreatePlayer("3d");
        SetHigherCriterionValue(intendedWinner, other, type);

        List<Player> ordered = [.. Utils.GetOrderedPlayerList(CreateSettings(type), [other, intendedWinner], false)];

        Assert.Equal(type == CriteriaType.NUL ? other : intendedWinner, ordered[0]);
    }

    [Theory]
    [InlineData(CriteriaType.POV)]
    [InlineData(CriteriaType.MMS)]
    [InlineData(CriteriaType.RAT)]
    [InlineData(CriteriaType.TRD)]
    [InlineData(CriteriaType.SOS)]
    [InlineData(CriteriaType.SDS)]
    [InlineData(CriteriaType.SSS)]
    [InlineData(CriteriaType.VZP)]
    public void GetOrderedPlayerList_OrdersBySecondaryCriterion(CriteriaType type)
    {
        var intendedWinner = CreatePlayer("2d");
        var other = CreatePlayer("3d");
        SetHigherCriterionValue(intendedWinner, other, type);
        CriteriaSettings settings = CreateSettings(CriteriaType.NUL, type);

        List<Player> ordered = [.. Utils.GetOrderedPlayerList(settings, [other, intendedWinner], false)];

        Assert.Equal(intendedWinner, ordered[0]);
    }

    [Fact]
    public void GetOrderedPlayerList_UsesCurrentRoundScoreForMcMahonCriterion()
    {
        var currentRoundLeader = CreatePlayer(2m);
        var pairingLeader = CreatePlayer(3m);
        currentRoundLeader.ScoreX = 4m;
        pairingLeader.ScoreX = 1m;
        CriteriaSettings settings = CreateSettings(CriteriaType.MMS);

        Assert.Equal(currentRoundLeader, Utils.GetOrderedPlayerList(settings, [pairingLeader, currentRoundLeader], true).First());
        Assert.Equal(pairingLeader, Utils.GetOrderedPlayerList(settings, [pairingLeader, currentRoundLeader], false).First());
    }

    [Theory]
    [InlineData(CriteriaType.POV, false)]
    [InlineData(CriteriaType.MMS, false)]
    [InlineData(CriteriaType.MMS, true)]
    [InlineData(CriteriaType.RAT, false)]
    [InlineData(CriteriaType.TRD, false)]
    [InlineData(CriteriaType.SOS, false)]
    [InlineData(CriteriaType.SDS, false)]
    [InlineData(CriteriaType.SSS, false)]
    [InlineData(CriteriaType.VZP, false)]
    public void IsPlayerPlaceShared_ReturnsTrueWhenCriterionMatch(CriteriaType type, bool countCurrentRound)
    {
        var first = CreatePlayer("1d");
        var second = CreatePlayer("1d");
        MakeCriterionValuesMatch(first, second, type, countCurrentRound);
        CriteriaSettings settings = CreateSettings(type);

        Assert.True(Utils.IsPlayerPlaceShared(settings, first, second, countCurrentRound));
    }

    [Theory]
    [InlineData(CriteriaType.POV, false)]
    [InlineData(CriteriaType.MMS, false)]
    [InlineData(CriteriaType.MMS, true)]
    [InlineData(CriteriaType.RAT, false)]
    [InlineData(CriteriaType.TRD, false)]
    [InlineData(CriteriaType.SOS, false)]
    [InlineData(CriteriaType.SDS, false)]
    [InlineData(CriteriaType.SSS, false)]
    [InlineData(CriteriaType.VZP, false)]
    public void IsPlayerPlaceShared_ReturnsFalseWhenCriterionDiffers(CriteriaType type, bool countCurrentRound)
    {
        var first = CreatePlayer("1d");
        var second = CreatePlayer("1d");
        MakeCriterionValuesDiffer(first, second, type, countCurrentRound);
        CriteriaSettings settings = CreateSettings(type);

        Assert.False(Utils.IsPlayerPlaceShared(settings, first, second, countCurrentRound));
    }

    [Theory]
    [InlineData(CriteriaType.NUL)]
    [InlineData((CriteriaType)99)]
    public void ComparePlayerPlace_TreatsUnsupportedCriteriaAsEqual(CriteriaType type)
    {
        CriteriaSettings settings = CreateSettings(type);

        Assert.True(Utils.IsPlayerPlaceShared(settings, CreatePlayer("1d"), CreatePlayer("30k"), false));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ComparePlayerPlace_UsesCurrentRoundMcMahonScore(bool countCurrentRound)
    {
        var first = CreatePlayer("1d");
        var second = CreatePlayer("1d");
        first.Score = second.Score = 5m;
        first.ScoreX = 3m;
        second.ScoreX = 4m;
        CriteriaSettings settings = CreateSettings(CriteriaType.MMS);

        Assert.Equal(!countCurrentRound, Utils.IsPlayerPlaceShared(settings, first, second, countCurrentRound));
    }

    [Fact]
    public void ComparePlayerPlace_ConsidersAllFiveCriteria()
    {
        var first = CreatePlayer("1d");
        var second = CreatePlayer("1d");
        CriteriaSettings settings = CreateSettings(
            CriteriaType.NUL,
            CriteriaType.NUL,
            CriteriaType.SOS,
            CriteriaType.NUL,
            CriteriaType.NUL);
        second.SOS = 1m;

        Assert.False(Utils.IsPlayerPlaceShared(settings, first, second, false));
    }

    [Fact]
    public void ComparePlayerPlace_TreatsMutualWinsAsDifferentAndDrawsAsEqual()
    {
        var black = CreatePlayer("1d");
        var white = CreatePlayer("1d");
        CriteriaSettings settings = CreateSettings(CriteriaType.VZP);
        var pairing = new Pairing(black, white, 0, "", 0);
        black.Pairings.Add(0, pairing);

        pairing.Result = Result.BLACK_WON;
        Assert.False(Utils.IsPlayerPlaceShared(settings, black, white, false));

        pairing.Result = Result.DRAW;
        Assert.True(Utils.IsPlayerPlaceShared(settings, black, white, false));

        pairing.Result = Result.BOTH_WON;
        Assert.True(Utils.IsPlayerPlaceShared(settings, black, white, false));
    }

    private static Player CreatePlayer(string grade) => new()
    {
        Grade = grade
    };

    private static Player CreatePlayer(decimal score) => new()
    {
        Score = score
    };

    private static CriteriaSettings CreateSettings(params CriteriaType[] types)
    {
        List<Criteria> criteria = [.. types.Select(type => CriteriaSettings.AllCriteriaDict.GetValueOrDefault(type) 
        ?? new Criteria(type, type.ToString(), ""))];

        while (criteria.Count < 5)
            criteria.Add(CriteriaSettings.AllCriteriaDict[CriteriaType.NUL]);

        return new() { Criterias = [.. criteria] };
    }

    private static void SetHigherCriterionValue(Player higher, Player lower, CriteriaType type)
    {
        switch (type)
        {
            case CriteriaType.POV:
                higher.Points = 2m;
                lower.Points = 1m;
                break;
            case CriteriaType.MMS:
                higher.Score = higher.ScoreX = 2m;
                lower.Score = lower.ScoreX = 1m;
                break;
            case CriteriaType.RAT:
                higher.Rating = 300;
                lower.Rating = 200;
                break;
            case CriteriaType.TRD:
                higher.Grade = "2d";
                lower.Grade = "1d";
                break;
            case CriteriaType.SOS:
                higher.SOS = 2m;
                lower.SOS = 1m;
                break;
            case CriteriaType.SDS:
                higher.SODOS = 2m;
                lower.SODOS = 1m;
                break;
            case CriteriaType.SSS:
                higher.SOSOS = 2m;
                lower.SOSOS = 1m;
                break;
            case CriteriaType.VZP:
                var pairing = new Pairing(higher, lower, 0, "", 0) { Result = Result.BLACK_WON };
                higher.Pairings.Add(0, pairing);
                lower.Pairings.Add(0, pairing);
                break;
        }
    }

    private static void MakeCriterionValuesMatch(Player first, Player second, CriteriaType type, bool countCurrentRound)
    {
        switch (type)
        {
            case CriteriaType.POV:
                first.Points = 1m;
                second.Points = 1m;
                break;
            case CriteriaType.MMS:
                if (countCurrentRound)
                {
                    first.ScoreX = 1m;
                    second.ScoreX = 1m;
                }
                else
                {
                    first.Score = 1m;
                    second.Score = 1m;
                }
                break;
            case CriteriaType.RAT:
                first.Rating = 100;
                second.Rating = 100;
                break;
            case CriteriaType.TRD:
                first.Grade = "1d";
                second.Grade = "1d";
                break;
            case CriteriaType.SOS:
                first.SOS = 1m;
                second.SOS = 1m;
                break;
            case CriteriaType.SDS:
                first.SODOS = 1m;
                second.SODOS = 1m;
                break;
            case CriteriaType.SSS:
                first.SOSOS = 1m;
                second.SOSOS = 1m;
                break;
            case CriteriaType.VZP:
                var pairing = new Pairing(first, second, 0, "", 0) { Result = Result.DRAW };
                first.Pairings.Add(0, pairing);
                second.Pairings.Add(0, pairing);
                break;
        }
    }

    private static void MakeCriterionValuesDiffer(Player first, Player second, CriteriaType type, bool countCurrentRound)
    {
        switch (type)
        {
            case CriteriaType.POV:
                second.Points = 1m;
                break;
            case CriteriaType.MMS:
                if (countCurrentRound)
                    second.ScoreX = 1m;
                else
                    second.Score = 1m;
                break;
            case CriteriaType.RAT:
                second.Rating++;
                break;
            case CriteriaType.TRD:
                second.Grade = "2d";
                break;
            case CriteriaType.SOS:
                second.SOS = 1m;
                break;
            case CriteriaType.SDS:
                second.SODOS = 1m;
                break;
            case CriteriaType.SSS:
                second.SOSOS = 1m;
                break;
            case CriteriaType.VZP:
                var pairing = new Pairing(first, second, 0, "", 0) { Result = Result.BLACK_WON };
                first.Pairings.Add(0, pairing);
                break;
        }
    }
}
