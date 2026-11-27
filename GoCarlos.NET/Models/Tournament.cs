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

using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoCarlos.NET.Models;

[method: JsonConstructor]
public class Tournament()
{
    [JsonProperty]
    public Player ByePlayer { get; private set; } = new() { Group = Group.Bye };

    public Tournament(int numberOfRounds) : this()
    {
        NumberOfRounds = numberOfRounds;
    }

    public string Name { get; set; } = "Turnaj";

    public List<Player> Players { get; set; } = [];
    public List<Round> Rounds { get; set; } = [];
    public CriteriaSettings CriteriaSettings { get; set; } = new([
            CriteriaSettings.AllCriteriaDict[CriteriaType.MMS],
            CriteriaSettings.AllCriteriaDict[CriteriaType.SDS],
            CriteriaSettings.AllCriteriaDict[CriteriaType.SOS],
            CriteriaSettings.AllCriteriaDict[CriteriaType.VZP],
            CriteriaSettings.AllCriteriaDict[CriteriaType.NUL]
        ]);

    public PairingMethod TopGroupPairingMethod { get; set; } = PairingMethod.Cross;
    public PairingMethod PairingMethod { get; set; } = PairingMethod.Random;
    public PairingMethod AdditionMethod { get; set; } = PairingMethod.Weakest;

    public bool AvoidSameCityPairing { get; set; } = true;
    public bool HandicapBasedMm { get; set; } = true;
    public bool HandicapMaxNine { get; set; } = true;
    public int HandicapReduction { get; set; } = 2;

    public bool CountCurrentRound { get; set; } = false;
    public int CurrentRound { get; set; } = 0;
    public int NumberOfRounds
    {
        get;
        set
        {
            if (value < 1)
            {
                throw new ArgumentException("Number of rounds must be at least 1!");
            }
            else if (value > 10)
            {
                throw new ArgumentException("Number of rounds must be at most 10!");
            }

            field = value;
            AddOrRemoveRounds();
        }
    } = 5;

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
                    pairing.Handicap = Utils.CalculateHandicap(pairing.Black, pairing.White,
                        HandicapReduction, HandicapBasedMm, HandicapMaxNine);
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
        if (AutomaticTopGroupBar)
        {
            TopGroupBar = CalculateTopBar();
        }

        if (roundNumber < 1)
        {
            foreach (Player player in Players)
            {
                player.Points = 0m;
                player.Score = StartScore(player);
                player.ScoreX = StartScore(player);
                player.SOS = 0m;
                player.SOSOS = 0m;
                player.SODOS = 0m;
            }
        }

        else
        {
            foreach (Player player in Players)
            {
                decimal score = StartScore(player) + GetResults(player, roundNumber);

                player.Points = GetPoints(player, roundNumber);
                player.Score = score / 1.000m; // Remove trailing zeros
                player.ScoreX = StartScore(player) + player.Points;
                player.SOS = 0m;
                player.SOSOS = 0m;
                player.SODOS = 0m;
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

                player.SOS /= 1.000m;
                player.SODOS /= 1.000m;
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

                player.SOSOS /= 1.000m;
            }
        }

        GetPlayerPlace();
    }

    private decimal StartScore(Player player)
    {
        decimal getScore(int score)
        {
            if (score > TopGroupBar) return TopGroupBar;
            else if (score < BottomGroupBar) return BottomGroupBar;
            return score;
        }

        return player.Group switch
        {
            Group.SuperGroup => TopGroupBar + 1m + SuperGroupGap,
            Group.TopGroup => TopGroupBar + 1m,
            Group.Default => getScore(player.StartScore),
            _ => 0m,
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
            p.SharedPlace = temp is not null && Utils.IsPlayerPlaceShared(CriteriaSettings, temp, p, CountCurrentRound);
            temp = p;
        }
    }
}
