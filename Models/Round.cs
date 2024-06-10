using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoCarlos.NET.Models;

public class Round(int roundNumber, TournamentType tournamentType) : IEquatable<Round?>
{
    [JsonProperty]
    private readonly int roundNumber = roundNumber;
    private readonly TournamentType tournamentType = tournamentType;
    private HashSet<Player> players = [];
    private HashSet<Player> unpairedPlayers = [];
    private List<Pairing> pairings = [];

    public int RoundNumber { get => roundNumber; }
    public HashSet<Player> Players { get => players; set => players = value; }
    public HashSet<Player> UnpairedPlayers { get => unpairedPlayers; set => unpairedPlayers = value; }
    public List<Pairing> Pairings { get => pairings; set => pairings = value; }

    public void AddPlayer(Player player)
    {
        player.RoundsPlaying.Add(roundNumber);
        players.Add(player);
        if (!pairings.Where(p => p.IsPlayerPaired(player)).Any())
        {
            unpairedPlayers.Add(player);
        }
    }

    public void RemovePlayer(Player player)
    {
        player.RoundsPlaying.Remove(roundNumber);

        Pairing? pairing = pairings.Where(p => p.IsPlayerPaired(player)).FirstOrDefault();

        if (pairing is not null)
        {
            RemovePairing(pairing);
        }

        players.Remove(player);
        unpairedPlayers.Remove(player);
    }

    private Pairing AddPairing(Player black, Player white, int handicap, string comment)
    {
        white.ColorBalancer++;

        black.Opponents.Add(roundNumber, white);
        white.Opponents.Add(roundNumber, black);

        AdjustPairingBalancer(black, white, true);

        Pairing pairing = new(black, white, handicap, comment);

        pairings.Add(pairing);
        black.Pairings.Add(roundNumber, pairing);
        white.Pairings.Add(roundNumber, pairing);

        unpairedPlayers.Remove(black);
        unpairedPlayers.Remove(white);

        return pairing;
    }

    public Pairing AddPairing(Player p1, Player p2, int handicapReduction, bool handicapBasedMm)
    {
        if (p2.Group == Group.Bye)
        {
            return AddByePairing(p1);
        }

        int p1GradeN = handicapBasedMm == true ? (int)Math.Round(p1.Score) : Utils.GetValue(p1.Grade);
        int p2GradeN = handicapBasedMm == true ? (int)Math.Round(p2.Score) : Utils.GetValue(p2.Grade);

        int handicap = handicapReduction >= 9 ? 0 : Math.Max(0, Math.Abs(p1GradeN - p2GradeN) - handicapReduction);

        if (handicap > 0)
        {
            if (p1GradeN > p2GradeN)
            {
                return AddPairing(p2, p1, handicap, "");
            }

            else
            {
                return AddPairing(p1, p2, handicap, "");
            }
        }

        else
        {
            int cmp = p1.ColorBalancer.CompareTo(p2.ColorBalancer);

            if (cmp > 0)
            {
                return AddPairing(p1, p2, handicap, "");
            }

            else if (cmp == 0)
            {
                int val = Utils.Random.Next(2);
                if (val == 0) return AddPairing(p1, p2, handicap, "");
                else return AddPairing(p2, p1, handicap, "");
            }

            else
            {
                return AddPairing(p2, p1, handicap, "");
            }
        }
    }

    private Pairing AddByePairing(Player black)
    {
        Pairing pairing = new(black);

        pairings.Add(pairing);

        black.ByeBalancer++;
        black.Pairings.Add(roundNumber, pairing);

        unpairedPlayers.Remove(black);

        return pairing;
    }

    public bool RemovePairing(Pairing pairing)
    {
        if (pairings.Contains(pairing))
        {
            int tmpBoardNumber = pairing.BoardNumber;

            Player black = pairing.Black;
            Player white = pairing.White;

            black.Pairings.Remove(roundNumber);
            black.Opponents.Remove(roundNumber);
            unpairedPlayers.Add(black);

            AdjustPairingBalancer(black, white, false);

            if (white.Group != Group.Bye)
            {
                white.ColorBalancer--;
                white.Pairings.Remove(roundNumber);
                white.Opponents.Remove(roundNumber);
                unpairedPlayers.Add(white);
            }
            else
            {
                black.ByeBalancer--;
            }

            pairings.Remove(pairing);

            foreach (Pairing p in pairings.Where(p => p.BoardNumber > tmpBoardNumber).OrderBy(p => p.BoardNumber).ToList())
            {
                p.BoardNumber--;
            }

            return true;
        }

        return false;
    }

    private void AdjustPairingBalancer(Player black, Player white, bool mode)
    {
        if (mode)
        {
            if (tournamentType.Equals(TournamentType.Swiss))
            {
                if (black.Points > white.Points)
                {
                    black.PairingBalancer++;
                    white.PairingBalancer--;
                }

                if (black.Points < white.Points)
                {
                    black.PairingBalancer--;
                    white.PairingBalancer++;
                }
            }
            else
            {
                if (black.Score > white.Score)
                {
                    black.PairingBalancer++;
                    white.PairingBalancer--;
                }

                if (black.Score < white.Score)
                {
                    black.PairingBalancer--;
                    white.PairingBalancer++;
                }
            }
        }
        else
        {
            if (tournamentType.Equals(TournamentType.Swiss))
            {
                if (black.Points > white.Points)
                {
                    black.PairingBalancer--;
                    white.PairingBalancer++;
                }

                if (black.Points < white.Points)
                {
                    black.PairingBalancer++;
                    white.PairingBalancer--;
                }
            }
            else
            {
                if (black.Score > white.Score)
                {
                    black.PairingBalancer--;
                    white.PairingBalancer++;
                }

                if (black.Score < white.Score)
                {
                    black.PairingBalancer++;
                    white.PairingBalancer--;
                }
            }
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is Round round &&
               roundNumber == round.roundNumber;
    }
    public bool Equals(Round? other)
    {
        return other is not null &&
            other.roundNumber == roundNumber;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(roundNumber);
    }
    public static bool operator ==(Round? left, Round? right)
    {
        return EqualityComparer<Round>.Default.Equals(left, right);
    }
    public static bool operator !=(Round? left, Round? right)
    {
        return !(left == right);
    }
    public override string? ToString()
    {
        return string.Format("Round {0}", roundNumber);
    }
}
