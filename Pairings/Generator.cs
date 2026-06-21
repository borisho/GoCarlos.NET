using GoCarlos.NET.Models;
using GoCarlos.NET.Models.Enums;
using GoCarlos.NET.Models.Records;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using static GoCarlos.NET.Pairings.PairingDebugUtils;

namespace GoCarlos.NET.Pairings;

///<remarks>Not thread safe</remarks>
public static class Generator
{
    private static Round? _round;

    private static bool _avoidSameCityPairing;
    private static bool _handicapBasedMm;
    private static bool _handicapMaxNine;
    private static int _handicapReduction;

    private static PairingMethod _topGroupPairingMethod;
    private static PairingMethod _pairingMethod;
    private static PairingMethod _additionMethod;

    public static List<(PlayerWrapper P1, PlayerWrapper P2)>? Pair(PairParameters pp, List<Player> orderedPlayers)
    {
        if (orderedPlayers.Count == 0)
        {
            Debug.WriteLine("\nEmpty list no pairing is made");
            return null;
        }

        // Set PairParameters to static fields
        // Not thread safe, use Generator class in a single thread context
        _round = pp.Round;
        _avoidSameCityPairing = pp.AvoidSameCityPairing;
        _handicapBasedMm = pp.HandicapBasedMm;
        _handicapMaxNine = pp.HandicapMaxNine;
        _handicapReduction = pp.HandicapReduction;
        _topGroupPairingMethod = pp.TopGroupPairingMethod;
        _pairingMethod = pp.PairingMethod;
        _additionMethod = pp.AdditionMethod;

        Debug.WriteLine("\nCurrentRound: " + _round!.RoundNumber);
        Debug.WriteLine("\nPairingMethod: " + _pairingMethod);
        Debug.WriteLine("\nAdditionMethod: " + _additionMethod);

        List<PlayerWrapper> players = [.. orderedPlayers.Select(p => new PlayerWrapper(p))];
        Debug.WriteLine("\nOrdered players: ");
        PrintPlayers(players);

        try
        {
            // Add BYE player if ODD number of Players
            if (players.Count % 2 == 1)
                players.Add(PlayerWrapper.ByeWrapper);

            return MakePairing(players);
        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message);
            Debug.Print(ex.StackTrace);
            MessageBox.Show("Error in pairing: " + ex.Message, "Pairing Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        return null;
    }

    private static List<(PlayerWrapper P1, PlayerWrapper P2)>? MakePairing(List<PlayerWrapper> players)
    {
        List<(PlayerWrapper P1, PlayerWrapper P2)> pairings = [];

        // TODO pridať man. možnosť opakovať párovanie
        while (true)
        {
            IEnumerable<PlayerWrapper> notPaired = players.Where(w => !w.IsPaired);
            
            if (TryOptimizeOddGroup(pairings, [.. notPaired])) continue;

            PlayerWrapper? wrapper = notPaired.FirstOrDefault();

            if (wrapper is null) break;

            Debug.WriteLine("\nOrdered players: ");
            PrintPlayers(players);

            Debug.WriteLine("\nMaking pairing for: ");
            PrintPlayer(wrapper.Player);

            List<PlayerWrapper> opponents = [.. notPaired
                .Where(w => !w.Equals(wrapper))
                .Where(w => !wrapper.TemporaryForbiddenPairing.Contains(w.Player))
                .Where(w => !wrapper.ForbiddenPairing.Contains(w.Player))];

            if (TryPairExactMatch(pairings,wrapper, opponents)) continue;
            if (TryPairCloseMatch(pairings, wrapper, opponents)) continue;
            if (TryPairNextAvailable(pairings, wrapper, opponents)) continue;
            if (TryRemoveLastPairing(pairings, wrapper)) continue;

            return null;
        }

        return pairings.Count == 0 ? null : pairings;
    }

    private static bool TryOptimizeOddGroup(
        List<(PlayerWrapper P1, PlayerWrapper P2)> pairings, 
        List<PlayerWrapper> pool)
    {
        List<PlayerWrapper>? group = TryGetStrongestGroup(pool);

        if (group is null || group.Count == 1 || group.Count % 2 == 0) return false;

        Debug.WriteLine("\nOptimizing odd group before regular pairing");

        var orderedByPairingBalancer = group
            .OrderByDescending(w => CalculatePairingBalancer(w.Player))
            .OrderByDescending(w => w.Player.Place);

        foreach (var wrapper in orderedByPairingBalancer)
        {
            List<PlayerWrapper> tmp = [];

            foreach (var w in orderedByPairingBalancer)
            {
                if (w.Equals(wrapper)) continue;
                tmp.Add(new(w.Player));
            }

            var tmpPairings = MakePairing(tmp);
            
            if (tmpPairings is not null)
            {
                List<PlayerWrapper> opponents = [.. pool
                .Where(w => !w.Equals(wrapper))
                .Where(w => w.Player.Score < wrapper.Player.Score)
                .Where(w => !wrapper.TemporaryForbiddenPairing.Contains(w.Player))
                .Where(w => !wrapper.ForbiddenPairing.Contains(w.Player))];

                if (TryPairCloseMatch(pairings, wrapper, opponents)) return true;
                if (TryPairNextAvailable(pairings, wrapper, opponents)) return true;
            }
        }

        return false;
    }

    private static bool TryPairExactMatch(
        List<(PlayerWrapper P1, PlayerWrapper P2)> pairings, 
        PlayerWrapper wrapper,
        List<PlayerWrapper> opponents)
    {
        List<PlayerWrapper> pool = [.. opponents.Where(w => w.Player.Score == wrapper.Player.Score)];

        if (pool.Count == 0) return false; 

        if (_avoidSameCityPairing && _round!.RoundNumber < 2)
            pool = TryAvoidSameClub(wrapper, pool);

        PlayerWrapper opponent = OpponentSelection(_pairingMethod, pool);

        return PairPlayers(pairings, wrapper, opponent);
    }

    private static bool TryPairCloseMatch(
        List<(PlayerWrapper P1, PlayerWrapper P2)> pairings, 
        PlayerWrapper wrapper,
        List<PlayerWrapper> opponents)
    {
        List<PlayerWrapper> pool = [.. opponents.Where(w =>
            w.Player.Score >= wrapper.Player.Score - 1 &&
            w.Player.Score <= wrapper.Player.Score + 1)];

        if (pool.Count == 0) return false;

        // Try to pair lowest pairing balancer first
        pool = TryGetMinMetric(pool, w => CalculatePairingBalancer(w.Player));

        if (_avoidSameCityPairing && _round!.RoundNumber < 2)
            pool = TryAvoidSameClub(wrapper, pool);

        PlayerWrapper opponent = OpponentSelection(_additionMethod, pool);

        return PairPlayers(pairings, wrapper, opponent);
    }

    private static bool TryPairNextAvailable(
        List<(PlayerWrapper P1, PlayerWrapper P2)> pairings, 
        PlayerWrapper wrapper,
        List<PlayerWrapper> opponents)
    {
        List<PlayerWrapper>? pool = TryGetStrongestGroup(opponents);

        if (pool is null) return false;

        // Try to avoid high handicap pairings
        pool = TryGetMinMetric(pool, w => Utils.CalculateHandicap(
                wrapper.Player,
                w.Player,
                _handicapReduction,
                _handicapBasedMm,
                _handicapMaxNine));

        if (_avoidSameCityPairing && _round!.RoundNumber < 2)
            pool = TryAvoidSameClub(wrapper, pool);

        PlayerWrapper opponent = OpponentSelection(_additionMethod, pool);

        return PairPlayers(pairings, wrapper, opponent);
    }

    private static bool TryRemoveLastPairing(
        List<(PlayerWrapper P1, PlayerWrapper P2)> pairings,
        PlayerWrapper wrapper)
    {
        try
        {
            // Not possible to make pairing, remove last made
            (PlayerWrapper P1, PlayerWrapper P2) = pairings[^1];
            UnpairPlayers(pairings, P1, P2);
            wrapper.TemporaryForbiddenPairing.Clear();
            P1.TemporaryForbiddenPairing.Add(P2.Player);
            P2.TemporaryForbiddenPairing.Add(P1.Player);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static List<PlayerWrapper> TryAvoidSameClub(PlayerWrapper wrapper, List<PlayerWrapper> pool)
    {
        string club = wrapper.Player.Data.Club;
        List<PlayerWrapper> filtered = [.. pool.Where(w => w.Player.Data.Club != club)];
        return filtered.Count == 0 ? pool : filtered;
    }

    private static PlayerWrapper OpponentSelection(
        PairingMethod method,
        List<PlayerWrapper> pool) => method switch
    {
        PairingMethod.Strongest => pool[0],
        PairingMethod.Weakest => pool[^1],
        _ => pool[Utils.Random.Next(pool.Count)],
    };

    private static bool PairPlayers(
        List<(PlayerWrapper P1, PlayerWrapper P2)> pairings,
        PlayerWrapper w1,
        PlayerWrapper w2)
    {
        pairings.Add((w1, w2));
        w1.IsPaired = true;
        w2.IsPaired = true;

        Debug.WriteLine("\nPlayers paired: ");
        PrintPlayer(w1.Player);
        PrintPlayer(w2.Player);

        return true;
    }

    private static void UnpairPlayers(
        List<(PlayerWrapper P1, PlayerWrapper P2)> pairings, 
        PlayerWrapper w1,
        PlayerWrapper w2)
    {
        pairings.Remove((w1, w2));
        w1.IsPaired = false;
        w2.IsPaired = false;

        Debug.WriteLine("\nPlayers unpaired: ");
        PrintPlayer(w1.Player);
        PrintPlayer(w2.Player);
    }

    private static int CalculatePairingBalancer(Player player)
    {
        int pb = 0;
        for (int i = 0; i < _round!.RoundNumber; i++)
        {
            pb += player.GetPairingBalancer(i);
        }
        return pb;
    }

    private static List<PlayerWrapper> TryGetMinMetric(
        List<PlayerWrapper> pool,
        Func<PlayerWrapper, int> metricCalculator)
    {
        List<PlayerWrapper> filtered = [];
        int min = int.MaxValue;

        foreach (var w in pool)
        {
            int tmpMin = metricCalculator(w);

            if (tmpMin < min)
            {
                min = tmpMin;
                filtered.Clear();
                filtered.Add(w);
            }
            else if (tmpMin == min)
            {
                filtered.Add(w);
            }
        }

        return filtered.Count == 0 ? pool : filtered;
    }

    private static List<PlayerWrapper>? TryGetStrongestGroup(List<PlayerWrapper> pool)
        => pool.GroupBy(w => w.Player.Score).FirstOrDefault()?.ToList();
}
