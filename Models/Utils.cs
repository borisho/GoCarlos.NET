using GoCarlos.NET.Models.Comparers;
using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoCarlos.NET.Models;

internal static class Utils
{
    public const string VERSION = "0.0.10";
    public const string BYE = "0+";
    public const string QUESTION_MARK = "?";
    public const string EQUALS = "=";
    public const string DASH = "-";
    public const string PLUS = "+";

    private static readonly Random random = new();
    private static readonly MutualGameComparer mutualGameComparer = new();

    public static JsonSerializerSettings JsonSerializerSettings = new()
    {
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        Formatting = Formatting.None,
        MaxDepth = 256,
    };

    public static Random Random { get => random; }

    public static IEnumerable<T> DropLast<T>(this IEnumerable<T> source)
    {
        using var e = source.GetEnumerator();

        if (e.MoveNext())
        {
            for (var value = e.Current; e.MoveNext(); value = e.Current)
            {
                yield return value;
            }
        }
    }

    public static T Next<T>(this T source) where T : struct
    {
        if (!typeof(T).IsEnum) return default;

        T[] array = (T[])Enum.GetValues(source.GetType());
        int index = Array.IndexOf<T>(array, source) + 1;
        return (array.Length == index) ? array[0] : array[index];
    }

    public static int GetValue(string grade)
    {
        return grade switch
        {
            "9p" => 47,
            "8p" => 46,
            "7p" => 45,
            "6p" => 44,
            "5p" => 43,
            "4p" => 42,
            "3p" => 41,
            "2p" => 40,
            "1p" => 39,
            "9d" => 38,
            "8d" => 37,
            "7d" => 36,
            "6d" => 35,
            "5d" => 34,
            "4d" => 33,
            "3d" => 32,
            "2d" => 31,
            "1d" => 30,
            "1k" => 29,
            "2k" => 28,
            "3k" => 27,
            "4k" => 26,
            "5k" => 25,
            "6k" => 24,
            "7k" => 23,
            "8k" => 22,
            "9k" => 21,
            "10k" => 20,
            "11k" => 19,
            "12k" => 18,
            "13k" => 17,
            "14k" => 16,
            "15k" => 15,
            "16k" => 14,
            "17k" => 13,
            "18k" => 12,
            "19k" => 11,
            "20k" => 10,
            "21k" => 9,
            "22k" => 8,
            "23k" => 7,
            "24k" => 6,
            "25k" => 5,
            "26k" => 4,
            "27k" => 3,
            "28k" => 2,
            "29k" => 1,
            "30k" => 0,
            _ => 0,
        };
    }
    public static string GetGrade(int grade_n)
    {
        return grade_n switch
        {
            47 => "9p",
            46 => "8p",
            45 => "7p",
            44 => "6p",
            43 => "5p",
            42 => "4p",
            41 => "3p",
            40 => "2p",
            39 => "1p",
            38 => "9d",
            37 => "8d",
            36 => "7d",
            35 => "6d",
            34 => "5d",
            33 => "4d",
            32 => "3d",
            31 => "2d",
            30 => "1d",
            29 => "1k",
            28 => "2k",
            27 => "3k",
            26 => "4k",
            25 => "5k",
            24 => "6k",
            23 => "7k",
            22 => "8k",
            21 => "9k",
            20 => "10k",
            19 => "11k",
            18 => "12k",
            17 => "13k",
            16 => "14k",
            15 => "15k",
            14 => "16k",
            13 => "17k",
            12 => "18k",
            11 => "19k",
            10 => "20k",
            9 => "21k",
            8 => "22k",
            7 => "23k",
            6 => "24k",
            5 => "25k",
            4 => "26k",
            3 => "27k",
            2 => "28k",
            1 => "29k",
            0 => "30k",
            _ => "30k",
        };
    }
    public static int GetRating(string grade)
    {
        return grade switch
        {
            "9p" => 2940,
            "8p" => 2910,
            "7p" => 2880,
            "6p" => 2850,
            "5p" => 2820,
            "4p" => 2790,
            "3p" => 2760,
            "2p" => 2730,
            "1p" => 2700,
            "9d" => 2900,
            "8d" => 2800,
            "7d" => 2700,
            "6d" => 2600,
            "5d" => 2500,
            "4d" => 2400,
            "3d" => 2300,
            "2d" => 2200,
            "1d" => 2100,
            "1k" => 2000,
            "2k" => 1900,
            "3k" => 1800,
            "4k" => 1700,
            "5k" => 1600,
            "6k" => 1500,
            "7k" => 1400,
            "8k" => 1300,
            "9k" => 1200,
            "10k" => 1100,
            "11k" => 1000,
            "12k" => 900,
            "13k" => 800,
            "14k" => 700,
            "15k" => 600,
            "16k" => 500,
            "17k" => 400,
            "18k" => 300,
            "19k" => 200,
            "20k" => 100,
            "21k" => 0,
            "22k" => -100,
            "23k" => -200,
            "24k" => -300,
            "25k" => -400,
            "26k" => -500,
            "27k" => -600,
            "28k" => -700,
            "29k" => -800,
            "30k" => -900,
            _ => -900,
        };
    }

    public static IOrderedEnumerable<Player> GetOrderedPlayerList(List<Player> players, TournamentType tournamentType, int roundNumber, int numberOfRounds)
    {
        if (roundNumber == numberOfRounds - 1)
        {
            return tournamentType switch
            {
                TournamentType.Swiss => players
                        .OrderByDescending(p => p.Points)
                        .ThenByDescending(p => p.SODOS)
                        .ThenByDescending(p => p.SOS)
                        .ThenByDescending(p => p.Rating),
                TournamentType.McMahon => players
                        .OrderByDescending(p => p.ScoreX)
                        .ThenByDescending(p => p.SODOS)
                        .ThenByDescending(p => p.SOS)
                        .ThenByDescending(p => p, mutualGameComparer)
                        .ThenByDescending(p => p.Rating),
                _ => players
                        .OrderByDescending(p => p.ScoreX)
                        .ThenByDescending(p => p.SOS)
                        .ThenByDescending(p => p.SOSOS)
                        .ThenByDescending(p => p.SODOS)
                        .ThenByDescending(p => p.Rating),
            };
        }
        else if (roundNumber < 2)
        {
            return tournamentType switch
            {
                TournamentType.Swiss => players
                        .OrderByDescending(p => p.Points)
                        .ThenByDescending(p => p.Rating),
                TournamentType.McMahon => players
                        .OrderByDescending(p => p.Score)
                        .ThenByDescending(p => p.Rating),
                _ => players
                        .OrderByDescending(p => p.Score)
                        .ThenByDescending(p => p.SOS)
                        .ThenByDescending(p => p.SOSOS)
                        .ThenByDescending(p => p.SODOS)
                        .ThenByDescending(p => p.Rating),
            };
        }
        else
        {
            return tournamentType switch
            {
                TournamentType.Swiss => players
                        .OrderByDescending(p => p.Points)
                        .ThenByDescending(p => p.Rating),
                TournamentType.McMahon => players
                        .OrderByDescending(p => p.Score)
                        .ThenByDescending(p => p.SODOS)
                        .ThenByDescending(p => p.SOS)
                        .ThenByDescending(p => p, mutualGameComparer)
                        .ThenByDescending(p => p.Rating),
                _ => players
                        .OrderByDescending(p => p.Score)
                        .ThenByDescending(p => p.SOS)
                        .ThenByDescending(p => p.SOSOS)
                        .ThenByDescending(p => p.SODOS)
                        .ThenByDescending(p => p.Rating),
            };
        }
    }
}
