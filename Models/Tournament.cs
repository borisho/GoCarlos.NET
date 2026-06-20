using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoCarlos.NET.Models;

[method: JsonConstructor]
public class Tournament()
{
    public static Player ByePlayer { get; } = new() { Group = Group.Bye };

    public Tournament(int numberOfRounds) : this()
    {
        for (int i = 0; i < numberOfRounds; i++)
        {
            Rounds!.Add(new(i));
        }
    }

    public string Name { get; set; } = "Turnaj";

    public List<Player> Players { get; set; } = [];
    public List<Round> Rounds { get; set; } = [];
    public CriteriaSettings CriteriaSettings { get; set; } = new();

    public PairingMethod TopGroupPairingMethod { get; set; } = PairingMethod.Cross;
    public PairingMethod PairingMethod { get; set; } = PairingMethod.Random;
    public PairingMethod AdditionMethod { get; set; } = PairingMethod.Weakest;

    public bool AvoidSameCityPairing { get; set; } = true;
    public bool HandicapBasedMm { get; set; } = true;
    public bool HandicapMaxNine { get; set; } = true;
    public int HandicapReduction { get; set; } = 2;

    public bool CountCurrentRound { get; set; } = false;
    public int CurrentRound { get; set; } = 0;
    public int NumberOfRounds { get; set; } = 5;

    public bool AutomaticTopGroupBar { get; set; } = true;
    public decimal SuperGroupGap { get; set; } = 2M;
    public decimal TopGroupBar { get; set; } = 29M;
    public decimal BottomGroupBar { get; set; } = 0M;

    public void UpdatePlayerPlayingRounds(Player player)
    {
        for (int i = 0; i < Rounds.Count; i++)
        {
            if (player.RoundsPlaying.Contains(i))
            {
                Rounds[i].AddPlayer(player);
            }
            else
            {
                Rounds[i].RemovePlayer(player);
            }
        }
    }

    public void UpdatePairingHandicaps()
    {
        foreach (Round round in Rounds)
        {
            foreach (Pairing pairing in round.Pairings)
            {
                if (pairing.Result.Equals(Result.NONE))
                {
                    int p1 = HandicapBasedMm == true ? (int)Math.Round(pairing.Black.Score) : Utils.GetValue(pairing.Black.Grade);
                    int p2 = HandicapBasedMm == true ? (int)Math.Round(pairing.White.Score) : Utils.GetValue(pairing.White.Grade);

                    pairing.Handicap = HandicapReduction >= 9 ? 0 : Math.Max(0, Math.Abs(p1 - p2) - HandicapReduction);
                }
            }
        }
    }

    public void AddOrRemoveRounds()
    {
        if (Rounds.Count > NumberOfRounds)
        {
            for (int i = Rounds.Count - 1; i > NumberOfRounds - 1; i--)
            {
                Round round = Rounds[i];

                foreach (Player player in Players)
                {
                    round.RemovePlayer(player);
                }

                Rounds.Remove(round);
            }
        }
        else
        {
            for (int i = Rounds.Count; i < NumberOfRounds; i++)
            {
                Rounds.Add(new(i));

                foreach (Player player in Players)
                {
                    Rounds[i].AddPlayer(player);
                }
            }
        }
    }

    public void CalculateCriteria(int roundNumber)
    {
        if (roundNumber < 1)
        {
            foreach (Player player in Players)
            {
                player.Points = 0;
                player.Score = StartScore(player);
                player.ScoreX = StartScore(player);
                player.SOS = 0;
                player.SOSOS = 0;
                player.SODOS = 0;
            }
        }

        else
        {
            foreach (Player player in Players)
            {
                player.Points = GetPoints(player, roundNumber);
                player.Score = StartScore(player) + GetResults(player, roundNumber);
                player.ScoreX = StartScore(player) + player.Points;
                player.SOS = 0;
                player.SOSOS = 0;
                player.SODOS = 0;
            }

            foreach (Player player in Players)
            {
                for (int i = 0; i < roundNumber; i++)
                {
                    if (player.Opponents.TryGetValue(i, out Player? opponent))
                    {
                        Pairing pairing = player.Pairings[i];
                        decimal pairingResult = pairing.GetPairingResult(player);

                        player.SOS += opponent.Score;
                        player.SODOS += opponent.Score * pairingResult;
                    }
                    else
                    {
                        player.SOS += StartScore(player);
                    }
                }
            }

            foreach (Player player in Players)
            {
                for (int i = 0; i < roundNumber; i++)
                {
                    if (player.Opponents.TryGetValue(i, out Player? opponent))
                    {
                        player.SOSOS += opponent.SOS;
                    }
                    else
                    {
                        player.SOSOS += player.SOS;
                    }
                }
            }
        }

        GetPlayerPlace();
    }

    private decimal StartScore(Player player)
    {
        TopGroupBar = AutomaticTopGroupBar ? CalculateTopBar() : TopGroupBar;

        decimal getScore(int score)
        {
            if (score > TopGroupBar)
            {
                return TopGroupBar;
            }
            else if (score < BottomGroupBar)
            {
                return BottomGroupBar;
            }
            else
            {
                return score;
            }
        }

        return player.Group switch
        {
            Group.SuperGroup => TopGroupBar + 1 + SuperGroupGap,
            Group.TopGroup => TopGroupBar + 1,
            Group.Default => getScore(player.StartScore),
            _ => 0,
        };
    }

    private int CalculateTopBar()
    {
        return Players.Where(p => p.Group == Group.Default)
                    .Select(p => p.StartScore)
                    .DefaultIfEmpty(0)
                    .Max();
    }

    private static decimal GetPoints(Player player, int roundNumber)
    {
        decimal points = 0M;

        for (int i = 0; i < roundNumber; i++)
        {
            if (player.Pairings.TryGetValue(i, out Pairing? pairing))
            {
                points += pairing.GetPairingResult(player);
            }
        }

        return points;
    }

    private static decimal GetResults(Player player, int roundNumber)
    {
        decimal points = 0M;

        for (int i = 0; i < roundNumber; i++)
        {
            if (player.Pairings.TryGetValue(i, out Pairing? pairing))
            {
                points += pairing.GetPairingResult(player);
            }

            else
            {
                points += 0.5M;
            }
        }

        return points;
    }

    public void ResetBoardNumbers()
    {
        int boardNumber = 1;
        List<Pairing> orderedPairings = [.. Rounds[CurrentRound].Pairings.OrderBy(p => p.White.Group == Group.Bye)
            .ThenBy(p => Math.Min(p.Black.Place, p.White.Place))
            .ThenByDescending(p => Math.Max(p.Black.Rating, p.White.Rating))];

        foreach (Pairing pairing in orderedPairings)
        {
            pairing.BoardNumber = boardNumber;
            boardNumber++;
        }
    }

    private void GetPlayerPlace()
    {
        List<Player> orderedList = [.. Utils.GetOrderedPlayerList(CriteriaSettings, Players, CountCurrentRound)];

        Player? temp = null;

        for (int i = 0; i < orderedList.Count; i++)
        {
            Player p = orderedList[i];
            p.Place = i + 1;

            if (CountCurrentRound)
            {
                p.SharedPlace = temp is not null && Utils.ComparePlayerPlace(CriteriaSettings, temp, p, CountCurrentRound);
                temp = p;
            }

            else
            {
                p.SharedPlace = false;
            }
        }
    }
}
