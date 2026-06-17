using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

using static GoCarlos.NET.Models.Utils;

namespace GoCarlos.NET.Models;

public class Round(int roundNumber) : IEquatable<Round?>
{
    [JsonProperty]
    private readonly int roundNumber = roundNumber;
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

        black.Opponents.Add(roundNumber, white);
        white.Opponents.Add(roundNumber, black);

        // Update balancers
        white.ColorBalancer[roundNumber] = true;
        AdjustPairingBalancer(black, white, roundNumber);

        Pairing pairing = new(black, white, handicap, comment, roundNumber);

        pairings.Add(pairing);
        black.Pairings.Add(roundNumber, pairing);
        white.Pairings.Add(roundNumber, pairing);

        unpairedPlayers.Remove(black);
        unpairedPlayers.Remove(white);

        return pairing;
    }

    private static void AdjustPairingBalancer(Player p1, Player p2, int roundNumber)
    {
        if (p1.Score > p2.Score)
        {
            p1.PairingBalancer[roundNumber] = -1;
            p2.PairingBalancer[roundNumber] = 1;
        }

        else if (p1.Score == p2.Score)
        {
            return;
        }

        else
        {
            p1.PairingBalancer[roundNumber] = 1;
            p2.PairingBalancer[roundNumber] = -1;
        }
    }

    public Pairing AddPairing(Player p1, Player p2, int handicapReduction, bool handicapBasedMm, bool handicapMaxNine)
    {
        if (p2.Group == Group.Bye)
        {
            return AddByePairing(p1, p2);
        }

        int handicap = CalculateHandicap(p1, p2, handicapReduction, handicapBasedMm, handicapMaxNine);

        if (handicap > 0)
        {
            if (GetHandicapGrade(p1, handicapBasedMm) > GetHandicapGrade(p2, handicapBasedMm))
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
            int p1cb = p1.ColorBalancer.Count;
            int p2cb = p2.ColorBalancer.Count;
            int cmp = p1cb.CompareTo(p2cb);

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

    private Pairing AddByePairing(Player black, Player bye)
    {
        Pairing pairing = new(black, bye, roundNumber);

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

            if (white.Group != Group.Bye)
            {
                // Reset balancers
                black.PairingBalancer[roundNumber] = 0;
                white.PairingBalancer[roundNumber] = 0;
                white.ColorBalancer[roundNumber] = false;

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
