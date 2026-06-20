using GoCarlos.NET.Models;
using GoCarlos.NET.Models.Enums;
using GoCarlos.NET.Models.Records;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using static GoCarlos.NET.Pairings.PairingDebugUtils;

namespace GoCarlos.NET.Pairings;

///<remarks>Not thread safe</remarks>
public static class Generator
{
    private static readonly List<PlayerWrapper> _players = [];
    private static readonly List<(PlayerWrapper P1, PlayerWrapper P2)> _pairings = [];

    private static Round? _round;

    private static bool _avoidSameCityPairing;
    private static bool _handicapBasedMm;
    private static bool _handicapMaxNine;

    private static int _handicapReduction;
    private static int _numberOfRounds;

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
        _players.AddRange(orderedPlayers.Select(p => new PlayerWrapper(p)));
        _round = pp.Round;
        _avoidSameCityPairing = pp.AvoidSameCityPairing;
        _handicapBasedMm = pp.HandicapBasedMm;
        _handicapMaxNine = pp.HandicapMaxNine;
        _handicapReduction = pp.HandicapReduction;
        _numberOfRounds = pp.NumberOfRounds;
        _topGroupPairingMethod = pp.TopGroupPairingMethod;
        _pairingMethod = pp.PairingMethod;
        _additionMethod = pp.AdditionMethod;

        Debug.WriteLine("\nCurrentRound: " + _round!.RoundNumber);
        Debug.WriteLine("\nPairingMethod: " + _pairingMethod);
        Debug.WriteLine("\nAdditionMethod: " + _additionMethod);
        Debug.WriteLine("\nOrdered players: ");
        PrintPlayers(_players);

        try
        {
            // Add BYE player if ODD number of Players
            if (_players.Count % 2 == 1)
                _players.Add(PlayerWrapper.ByeWrapper);

            return MakePairing();
        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message);
            Debug.Print(ex.StackTrace);
            MessageBox.Show("Error in pairing: " + ex.Message, "Pairing Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _players.Clear();
            _pairings.Clear();
        }

        return null;
    }

    private static List<(PlayerWrapper P1, PlayerWrapper P2)>? MakePairing()
    {
        // TODO pridať man. možnosť opakovať párovanie
        while (true)
        {
            PlayerWrapper? wrapper = _players
                .Where(w => !w.IsPaired)
                .FirstOrDefault();

            if (wrapper is null) break;

            Debug.WriteLine("\nOrdered players: ");
            PrintPlayers(_players);

            Debug.WriteLine("\nMaking pairing for: ");
            PrintPlayer(wrapper.Player);

            List<PlayerWrapper> opponents = [.. _players
                .Where(w => !w.Equals(wrapper))
                .Where(w => !w.IsPaired)
                .Where(w => !wrapper.TemporaryForbiddenPairing.Contains(w.Player))
                .Where(w => !wrapper.ForbiddenPairing.Contains(w.Player))];

            if (TryPairExactMatch(wrapper, opponents)) continue;
            if (TryPairCloseMatch(wrapper, opponents)) continue;
            if (TryPairNextAvailable(wrapper, opponents)) continue;
            if (TryRemoveLastPairing(wrapper)) continue;

            return null;
        }

        return _pairings.Count == 0 ? null : [.. _pairings];
    }

    private static bool TryPairExactMatch(PlayerWrapper wrapper, List<PlayerWrapper> opponents)
    {
        bool isOdd = _players.Where(w => !w.IsPaired)
            .Where(w => w.Player.Score == wrapper.Player.Score)
            .Count() % 2 == 1;
        
        List<PlayerWrapper> pool = [.. opponents.Where(w => w.Player.Score == wrapper.Player.Score)];

        if (pool.Count == 0) return false; 

        if (isOdd)
            pool = TryOptimizeOddGroup(wrapper, pool);

        if (_avoidSameCityPairing && _round!.RoundNumber < 2)
            pool = TryAvoidSameClub(wrapper, pool);

        PlayerWrapper opponent = OpponentSelection(_pairingMethod, pool);

        return PairPlayers(wrapper, opponent);
    }

    private static bool TryPairCloseMatch(PlayerWrapper wrapper, List<PlayerWrapper> opponents)
    {
        List<PlayerWrapper> pool = opponents.Where(w =>
            w.Player.Score >= wrapper.Player.Score - 1 &&
            w.Player.Score <= wrapper.Player.Score + 1).ToList();

        if (pool.Count == 0) return false;

        // Try to pair lowest pairing balancer first
        pool = TryGetMinMetric(pool, w => CalculatePairingBalancer(w.Player));

        if (_avoidSameCityPairing && _round!.RoundNumber < 2)
            pool = TryAvoidSameClub(wrapper, pool);

        PlayerWrapper opponent = OpponentSelection(_additionMethod, pool);

        return PairPlayers(wrapper, opponent);
    }

    private static bool TryPairNextAvailable(PlayerWrapper wrapper, List<PlayerWrapper> opponents)
    {
        var groups = opponents.GroupBy(w => w.Player.Score);
        
        if (!groups.Any()) return false;

        List<PlayerWrapper> pool = [.. groups.First()];

        if ( pool.Count == 0) return false;

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

        return PairPlayers(wrapper, opponent);
    }

    private static bool TryRemoveLastPairing(PlayerWrapper wrapper)
    {
        try
        {
            // Nie je možné vykonať párovanie, rozhoď poslednú dvojicu a skús znovu
            (PlayerWrapper P1, PlayerWrapper P2) p = _pairings[^1];
            UnpairPlayers(p.P1, p.P2);
            wrapper.TemporaryForbiddenPairing.Clear();
            p.P1.TemporaryForbiddenPairing.Add(p.P2.Player);
            p.P2.TemporaryForbiddenPairing.Add(p.P1.Player);

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

    private static List<PlayerWrapper> TryOptimizeOddGroup(PlayerWrapper wrapper, List<PlayerWrapper> pool)
    {
        int lowestPairingBalancer = pool.Min(w => CalculatePairingBalancer(w.Player));;

        List<PlayerWrapper> filtered = [];
        // TODO doplniť optimalizáciu 

        //List<PlayerWrapper> filtered = [.. pool.Where(p => )];
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

    private static bool PairPlayers(PlayerWrapper w1, PlayerWrapper w2)
    {
        _pairings.Add((w1, w2));
        w1.IsPaired = true;
        w2.IsPaired = true;

        Debug.WriteLine("\nPlayers paired: ");
        PrintPlayer(w1.Player);
        PrintPlayer(w2.Player);

        return true;
    }

    private static void UnpairPlayers(PlayerWrapper w1, PlayerWrapper w2)
    {
        _pairings.Remove((w1, w2));
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
}
