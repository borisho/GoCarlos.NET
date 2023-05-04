using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoCarlos.NET.Models;

public class Tournament
{
    private List<Player> players;
    private List<Round> rounds;

    private string name;

    private int handicapReduction;
    private TournamentType tournamentType;
    private PairingMethod pairingMethod;
    private PairingMethod additionMethod;

    private bool avoidSameCityPairing;

    private bool countCurrentRound;
    private int currentRound;
    private int numberOfRounds;

    private float topGroupBar;
    private float bottomGroupBar;

    [JsonConstructor]
    public Tournament()
    {
        players = new();
        rounds = new();

        name = "Turnaj";

        handicapReduction = 2;
        tournamentType = TournamentType.McMahon;
        pairingMethod = PairingMethod.Random;
        additionMethod = PairingMethod.Weakest;

        avoidSameCityPairing = true;

        countCurrentRound = false;
        currentRound = 0;
        numberOfRounds = 5;

        topGroupBar = 29f;
        bottomGroupBar = 10f;
    }

    public Tournament(int numberOfRounds) : this()
    {
        for(int i = 0; i < numberOfRounds; i++)
        {
            rounds!.Add(new(i, tournamentType));
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
    public TournamentType TournamentType
    {
        get => tournamentType;
        set => tournamentType = value;
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

    public void UpdatePairingHandicaps(int handicapReduction)
    {
        HandicapReduction = handicapReduction;

        foreach (Round round in rounds)
        {
            foreach (Pairing pairing in round.Pairings)
            {
                if (pairing.Result.Equals(Result.NONE))
                {
                    int p1GradeN = Utils.GetValue(pairing.Black.Grade);
                    int p2GradeN = Utils.GetValue(pairing.White.Grade);

                    pairing.Handicap = handicapReduction >= 9 ? 0 : Math.Max(0, Math.Abs(p1GradeN - p2GradeN) - handicapReduction);
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
                Round round = new(i + 1, TournamentType);

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
                player.Score = GetMacMahon(player);
                player.ScoreX = GetMacMahon(player);
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
                player.Score = GetMacMahon(player) + GetResults(player, roundNumber);
                player.ScoreX = GetMacMahon(player) + player.Points;
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
                        player.SOS += opponent.Score;

                        Pairing pairing = player.Pairings[i];

                        if (pairing.GetPairingResult(player) == 1f)
                        {
                            player.SODOS += opponent.Score;
                        }
                        else if (pairing.GetPairingResult(player) == 0.5f)
                        {
                            player.SODOS += opponent.Score / 2;
                        }
                    }
                    else
                    {
                        float startscore;

                        if (player.StartScore < bottomGroupBar)
                        {
                            startscore = bottomGroupBar;
                        }
                        else if (player.StartScore > topGroupBar)
                        {
                            startscore = topGroupBar;
                        }
                        else
                        {
                            startscore = player.StartScore;
                        }

                        player.SOS += player.IsSuperGroup ? startscore + 2 : startscore;
                        player.SODOS += startscore / 2f;
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

    private float GetMacMahon(Player player)
    {
        if (player.IsSuperGroup)
        {
            return TopGroupBar + 2;
        }
        else if (player.IsTopGroup)
        {
            return TopGroupBar;
        }
        else
        {
            if (player.StartScore > TopGroupBar)
            {
                return TopGroupBar;
            }
            else if (player.StartScore < BottomGroupBar)
            {
                return BottomGroupBar;
            }
            else
            {
                return player.StartScore;
            }
        }
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
        List<Pairing> orderedPairings = rounds[currentRound].Pairings;

        if (TournamentType.Equals(TournamentType.Swiss))
        {
            orderedPairings = orderedPairings.OrderBy(p => p.White.IsBye)
                .ThenByDescending(p => Math.Floor(Math.Max(p.Black.Points, p.White.Points)))
                .ThenByDescending(p => Math.Max(p.Black.Rating, p.White.Rating))
                .ToList();
        }
        else
        {
            orderedPairings = orderedPairings.OrderBy(p => p.White.IsBye)
                .ThenByDescending(p => Math.Floor(Math.Max(p.Black.Score, p.White.Score)))
                .ThenByDescending(p => Math.Max(p.Black.Rating, p.White.Rating))
                .ToList();
        }

        foreach (Pairing pairing in orderedPairings)
        {
            pairing.BoardNumber = boardNumber;
            boardNumber++;
        }
    }

    private void GetPlayerPlace()
    {
        List<Player> orderedList = Utils.GetOrderedPlayerList(players, TournamentType, currentRound).ToList();

        foreach (Player p in orderedList)
        {
            p.Place = orderedList.IndexOf(p) + 1;
        }
    }
}
