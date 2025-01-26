using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
    private bool handicapBasedMm;

    private bool countCurrentRound;
    private int currentRound;
    private int numberOfRounds;

    private bool automaticTopBar;
    private float topGroupBar;
    private float bottomGroupBar;

    [JsonConstructor]
    public Tournament()
    {
        players = [];
        rounds = [];

        name = "Turnaj";

        handicapReduction = 2;
        tournamentType = TournamentType.McMahon;
        pairingMethod = PairingMethod.Random;
        additionMethod = PairingMethod.Weakest;

        avoidSameCityPairing = true;
        handicapBasedMm = false;

        countCurrentRound = false;
        currentRound = 0;
        numberOfRounds = 5;

        automaticTopBar = true;
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

                        if (tournamentType == TournamentType.Swiss)
                        {
                            player.SOS += opponent.Points;
                            player.SODOS += opponent.Points * pairingResult;
                        }
                        else
                        {
                            player.SOS += opponent.Score;
                            player.SODOS += opponent.Score * pairingResult;
                        }
                    }
                    else
                    {
                        float startscore = tournamentType == TournamentType.Swiss ? 0 : StartScore(player);

                        player.SOS += startscore;
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

    private float StartScore(Player player)
    {
        TopGroupBar = automaticTopBar ? players.Where(p => p.Group == Group.Default)
            .Max(p => p.StartScore) : TopGroupBar;

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
        List<Player> orderedList = [.. Utils.GetOrderedPlayerList(players, TournamentType, currentRound, numberOfRounds)];

        foreach (Player p in orderedList)
        {
            p.Place = orderedList.IndexOf(p) + 1;
        }
    }
}
