using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoCarlos.NET.Models;

[method: JsonConstructor]
public class Tournament()
{
    private List<Player> players = [];
    private List<Round> rounds = [];

    private CriteriaSettings criteriaSettings = new();

    private string name = "Turnaj";

    private int handicapReduction = 2;
    private PairingMethod topGroupPairingMethod = PairingMethod.Cross;
    private PairingMethod pairingMethod = PairingMethod.Random;
    private PairingMethod additionMethod = PairingMethod.Weakest;

    private bool avoidSameCityPairing = true;
    private bool handicapBasedMm = false;

    private bool countCurrentRound = false;
    private int currentRound = 0;
    private int numberOfRounds = 5;

    private bool automaticTopBar = true;
    private float topGroupBar = 29f;
    private float bottomGroupBar = 10f;

    public Tournament(int numberOfRounds) : this()
    {
        for(int i = 0; i < numberOfRounds; i++)
        {
            rounds!.Add(new(i));
        }
    }

    public List<Player> Players
    {
        get => players;
        set => players = value;
    }
    public List<Round> Rounds
    {
        get => rounds;
        set => rounds = value;
    }

    public CriteriaSettings CriteriaSettings
    {
        get => criteriaSettings;
        set => criteriaSettings = value;
    }

    public string Name
    {
        get => name;
        set => name = value;
    }

    public int HandicapReduction
    {
        get => handicapReduction;
        set => handicapReduction = value;
    }

    public PairingMethod TopGroupPairingMethod
    {
        get => topGroupPairingMethod;
        set => topGroupPairingMethod = value;
    }
    public PairingMethod PairingMethod
    {
        get => pairingMethod;
        set => pairingMethod = value;
    }
    public PairingMethod AdditionMethod
    {
        get => additionMethod;
        set => additionMethod = value;
    }

    public bool AvoidSameCityPairing
    {
        get => avoidSameCityPairing;
        set => avoidSameCityPairing = value;
    }

    public bool HandicapBasedMm
    {
        get => handicapBasedMm;
        set => handicapBasedMm = value;
    }

    public bool CountCurrentRound
    {
        get => countCurrentRound;
        set => countCurrentRound = value;
    }
    public int CurrentRound
    {
        get => currentRound;
        set => currentRound = value;
    }
    public int NumberOfRounds
    {
        get => numberOfRounds;
        set => numberOfRounds = value;
    }

    public bool AutomaticTopGroupBar
    {
        get => automaticTopBar;
        set => automaticTopBar = value;
    }

    public float TopGroupBar
    {
        get => topGroupBar;
        set => topGroupBar = value;
    }
    public float BottomGroupBar
    {
        get => bottomGroupBar;
        set => bottomGroupBar = value;
    }

    public void UpdatePlayerPlayingRounds(Player player)
    {
        for (int i = 0; i < rounds.Count; i++)
        {
            if (player.RoundsPlaying.Contains(i))
            {
                rounds[i].AddPlayer(player);
            }
            else
            {
                rounds[i].RemovePlayer(player);
            }
        }
    }

    public void UpdatePairingHandicaps()
    {
        foreach (Round round in rounds)
        {
            foreach (Pairing pairing in round.Pairings)
            {
                if (pairing.Result.Equals(Result.NONE))
                {
                    int p1 = HandicapBasedMm == true ? (int)Math.Round(pairing.Black.Score) : Utils.GetValue(pairing.Black.Grade);
                    int p2 = HandicapBasedMm == true ? (int)Math.Round(pairing.White.Score) : Utils.GetValue(pairing.White.Grade);

                    pairing.Handicap = handicapReduction >= 9 ? 0 : Math.Max(0, Math.Abs(p1 - p2) - handicapReduction);
                }
            }
        }
    }

    public void AddOrRemoveRounds()
    {
        if (rounds.Count > numberOfRounds)
        {
            for (int i = rounds.Count - 1; i > numberOfRounds - 1; i--)
            {
                Round round = rounds[i];

                foreach (Player player in players)
                {
                    round.RemovePlayer(player);
                }

                rounds.Remove(round);
            }
        }
        else
        {
            for (int i = rounds.Count - 1; i < numberOfRounds; i++)
            {
                Round round = new(i + 1);

                rounds.Add(round);

                foreach (Player player in players)
                {
                    round.AddPlayer(player);
                }
            }
        }
    }

    public void CalculateCriteria(int roundNumber)
    {
        if (roundNumber < 1)
        {
            foreach (Player player in players)
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
            foreach (Player player in players)
            {
                player.Points = GetPoints(player, roundNumber);
                player.Score = StartScore(player) + GetResults(player, roundNumber);
                player.ScoreX = StartScore(player) + player.Points;
                player.SOS = 0;
                player.SOSOS = 0;
                player.SODOS = 0;
            }

            foreach (Player player in players)
            {
                for (int i = 0; i < roundNumber; i++)
                {
                    if (player.Opponents.TryGetValue(i, out Player? opponent))
                    {
                        Pairing pairing = player.Pairings[i];
                        float pairingResult = pairing.GetPairingResult(player);

                        player.SOS += opponent.Score;
                        player.SODOS += opponent.Score * pairingResult;
                    }
                    else
                    {
                        player.SOS += StartScore(player);
                    }
                }
            }

            foreach (Player player in players)
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

    private float StartScore(Player player)
    {
        TopGroupBar = automaticTopBar ? CalculateTopBar() : TopGroupBar;

        float getScore(int score)
        {
            if (score > topGroupBar)
            {
                return topGroupBar;
            }
            else if (score < bottomGroupBar)
            {
                return bottomGroupBar;
            }
            else
            {
                return score;
            }
        }

        return player.Group switch
        {
            Group.SuperGroup => topGroupBar + 3,
            Group.TopGroup => topGroupBar + 1,
            Group.Default => getScore(player.StartScore),
            _ => 0,
        };
    }

    private int CalculateTopBar()
    {
        return players.Where(p => p.Group == Group.Default)
                    .Select(p => p.StartScore)
                    .DefaultIfEmpty(0)
                    .Max();
    }

    private static float GetPoints(Player player, int roundNumber)
    {
        float points = 0;

        for (int i = 0; i < roundNumber; i++)
        {
            if (player.Pairings.TryGetValue(i, out Pairing? pairing))
            {
                points += pairing.GetPairingResult(player);
            }
        }

        return points;
    }

    private static float GetResults(Player player, int roundNumber)
    {
        float points = 0;

        for (int i = 0; i < roundNumber; i++)
        {
            if (player.Pairings.TryGetValue(i, out Pairing? pairing))
            {
                points += pairing.GetPairingResult(player);
            }

            else
            {
                points += 0.5f;
            }
        }

        return points;
    }

    public void ResetBoardNumbers()
    {
        int boardNumber = 1;
        List<Pairing> orderedPairings = [.. rounds[currentRound].Pairings.OrderBy(p => p.White.Group == Group.Bye)
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
        List<Player> orderedList = [.. Utils.GetOrderedPlayerList(criteriaSettings, players, CountCurrentRound)];

        Player? temp = null;

        foreach (Player p in orderedList)
        {
            p.Place = orderedList.IndexOf(p) + 1;
            
            if (countCurrentRound)
            {
                p.SharedPlace = temp is not null && Utils.ComparePlayerPlace(criteriaSettings, temp, p, countCurrentRound);
                temp = p;
            }

            else
            {
                p.SharedPlace = false;
            }
        }
    }
}
