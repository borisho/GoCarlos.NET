using GoCarlos.NET.Models.Enums;
using GoCarlos.NET.Models.Records;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace GoCarlos.NET.Models;

public static class PairingGenerator
{

    private sealed class State
    {
        public readonly Stack<Pairing> Pairings = new();
        public readonly List<Player> Players = [];
    }

    public static void PerformPairings(PairingGeneratorParameters parameters)
    {
        State state = new();

        try
        {
            if (parameters.OrderedPlayers.Count == 0)
            {
                Debug.WriteLine("\nEmpty list no pairing is made");
                return;
            }

            Debug.WriteLine("\nCurrentRound: " + parameters.Round.RoundNumber);
            Debug.WriteLine("\nPairingMethod: " + parameters.PairingMethod);

            state.Players.AddRange(parameters.OrderedPlayers);

            CheckForBye(state, parameters);

            Debug.WriteLine("\nOrdered players: ");
            PrintPlayers(parameters.OrderedPlayers);

            MakePairing(state, parameters);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Chyba pri párovaní: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            foreach (Player player in parameters.OrderedPlayers)
            {
                player.TemporaryForbiddenPairings.Clear();
            }

            state.Players.Clear();
            state.Pairings.Clear();
        }
    }

    private static void MakePairing(State state, PairingGeneratorParameters parameters)
    {
        int roundNumber = parameters.Round.RoundNumber;

        // Použi losovaciu strátégiu pre top skupiny v prvom kole
        if (roundNumber == 0)
        {
            PairTopGroup(state, parameters, Group.SuperGroup);
            PairTopGroup(state, parameters, Group.TopGroup);
        }

        while(true)
        {
            var player = parameters.OrderedPlayers.FirstOrDefault(p => parameters.Round.UnpairedPlayers.Contains(p));

            // Ak nie je hráč, ktorý ešte nebol spárovaný, párovanie je hotové
            if (player is null)
            {
                break;
            }

            Debug.WriteLine("\nMaking pairing for: ");
            PrintPlayer(player);

            state.Players.Remove(player);

            List<Player> unpairedPlayers = [.. state.Players];

            Debug.WriteLine("\nUnpaired players: ");
            PrintPlayers(unpairedPlayers);

            // Odstránia sa hráči, s ktorými už hral alebo s ktorými nie je možné urobiť párovanie
            HashSet<Player> played = [.. player.Opponents.Values];
            List<Player> opponents = [.. unpairedPlayers.Where(p => !played.Contains(p) 
                && !player.TemporaryForbiddenPairings.Contains(p))];

            Debug.WriteLine("\nAvailable opponents: ");
            PrintPlayers(opponents);

            if (opponents.Count == 0)
            {
                TryToPairWithoutRepetition(state, parameters, player, unpairedPlayers);
                continue;
            }

            if (TryPairExactMatch(state, parameters, player, opponents))
            {
                continue;
            }

            if (TryPairCloseMatch(state, parameters, player, opponents))
            {
                continue;
            }

            PairStrongestGroup(state, parameters, player, opponents);
        }
    }

    private static void TryToPairWithoutRepetition(
        State state,
        PairingGeneratorParameters parameters,
        Player player,
        List<Player> unpairedPlayers)
    {
        if (state.Pairings.TryPop(out Pairing? pairing))
        {

            player.TemporaryForbiddenPairings.Clear();

            pairing.Black.TemporaryForbiddenPairings.Add(pairing.White);
            pairing.White.TemporaryForbiddenPairings.Add(pairing.Black);

            state.Players.Add(player);
            RemovePairing(state, parameters.Round, pairing);
        }
        else
        {
            if (unpairedPlayers.Count > 0)
            {
                List<Player> filtered = TryToAvoidSamePlayers(unpairedPlayers, player);
                filtered = TryToAvoidHighHandicap(filtered, player,
                    parameters.HandicapReduction,
                    parameters.HandicapBasedMm,
                    parameters.HandicapMaxNine);

                //Player opponent = filtered[Utils.Random.Next(filtered.Count)];
                Player opponent = OpponentSelection(parameters.PairingMethod, filtered);

                _ = PairPlayers(
                    state,
                    new PairingParameters(
                        parameters.Round,
                        parameters.HandicapReduction,
                        parameters.HandicapBasedMm,
                        parameters.HandicapMaxNine,
                        player,
                        opponent
                    )
                );
            }
        }
    }

    private static bool TryPairExactMatch(
        State state,
        PairingGeneratorParameters parameters,
        Player player,
        List<Player> opponents)
    {
        int roundNumber = parameters.Round.RoundNumber;

        // Získa zoznam oponentov s rovnakým počtom bodov/MM
        List<Player> exactMatch = [.. opponents.Where(p => p.Score == player.Score)];
        int exactMatchMax = exactMatch.Count > 0 ? exactMatch.Max(p => CalculatePairingBalancer(p, roundNumber)) : 0;

        // V prípade nepárneho počtu hráčov v skupine (párny počet opponentov)
        // vyber hráča s najvyšším PB, ktorý bude nalosovaný dole
        bool checkPb = exactMatch.Count % 2 == 0 && CalculatePairingBalancer(player, roundNumber) > exactMatchMax;

        if (exactMatch.Count != 0 && !checkPb)
        {
            if (parameters.AvoidSameCityPairing && roundNumber < 2)
            {
                // Pokiaľ je to možné vyber súpera, ktorý pochádza z iného mesta
                exactMatch = TryToAvoidSameCity(exactMatch, player);
            }

            Player opponent = OpponentSelection(parameters.PairingMethod, exactMatch);

            Pairing pairing = PairPlayers(
                state,
                new PairingParameters(
                    parameters.Round,
                    parameters.HandicapReduction,
                    parameters.HandicapBasedMm,
                    parameters.HandicapMaxNine,
                    player,
                    opponent
                )
            );

            state.Pairings.Push(pairing);
            return true;
        }

        return false;
    }

    private static bool TryPairCloseMatch(
        State state,
        PairingGeneratorParameters parameters,
        Player player,
        List<Player> opponents)
    {
        int roundNumber = parameters.Round.RoundNumber;

        // Získa zoznam oponentov s počtom bodov/MM o 1 nižším alebo vyšším
        List<Player> closeMatch = [.. opponents.Where(p => p.Score >= player.Score - 1 && p.Score <= player.Score + 1)];

        if (closeMatch.Count > 0)
        {
            List<(Player Pc, int PBc)> withPb = [.. closeMatch.Select(p => (p, CalculatePairingBalancer(p, roundNumber)))];
            int lowestPairingBalancer = withPb.Min(p => p.PBc);

            closeMatch = [.. withPb.Where(p => p.PBc == lowestPairingBalancer).Select(p => p.Pc)];

            if (parameters.AvoidSameCityPairing && roundNumber < 2)
            {
                // Pokiaľ je to možné vyber súpera, ktorý pochádza z iného mesta
                closeMatch = TryToAvoidSameCity(closeMatch, player);
            }

            Player  opponent = parameters.OrderedPlayers.Count(p => p.Score == player.Score) == 1
            ? OpponentSelection(parameters.PairingMethod, closeMatch)
            : OpponentSelection(parameters.AdditionMethod, closeMatch);

            Pairing pairing = PairPlayers(
                state,
                new PairingParameters(
                    parameters.Round,
                    parameters.HandicapReduction,
                    parameters.HandicapBasedMm,
                    parameters.HandicapMaxNine,
                    player,
                    opponent
                )
            );

            state.Pairings.Push(pairing);
            return true;
        }

        return false;
    }

    private static void PairStrongestGroup(
        State state,
        PairingGeneratorParameters parameters,
        Player player,
        List<Player> opponents)
    {
        // Párovanie ak nie je dostupný oponent s rovnakým alebo podobným skóre/bodmi,
        // získa sa najsilnejšia skupina a oponent sa z nej vyberie podľa vybraných kritérií
        List<Player> strongestGroup = [.. opponents.GroupBy(p => p.Score).First()];

        // pokiaľ je to možné vyhni sa hendikepu viac ako 9
        strongestGroup = TryToAvoidHighHandicap(strongestGroup, player,
            parameters.HandicapReduction,
            parameters.HandicapBasedMm,
            parameters.HandicapMaxNine);
        Player opponent = OpponentSelection(parameters.AdditionMethod, strongestGroup);

        Pairing pairing = PairPlayers(
            state,
            new PairingParameters(
                parameters.Round,
                parameters.HandicapReduction,
                parameters.HandicapBasedMm,
                parameters.HandicapMaxNine,
                player,
                opponent
            )
        );

        state.Pairings.Push(pairing);
    }

    private static int CalculatePairingBalancer(Player player, int roundNumber)
    {
        int pb = 0;
        for (int i = 0; i < roundNumber; i++)
        {
            pb += player.GetPairingBalancer(i);
        }
        return pb;
    }

    private static void PairTopGroup(State state, PairingGeneratorParameters parameters, Group group)
    {
        List<Player> players = parameters.TopGroupPairingMethod switch
        {
            PairingMethod.Cross => CrossPairing(state, parameters, group),
            PairingMethod.Weakest => WeakestPairing(state, parameters, group),
            PairingMethod.Strongest => StrongestPairing(state, parameters, group),
            PairingMethod.Random => RandomPairing(state, parameters, group),
            _ => throw new NotImplementedException(),
        };

        state.Players.Clear();
        state.Players.AddRange(players);
    }

    private static List<Player> CrossPairing(State state, PairingGeneratorParameters parameters, Group playerGroup)
    {
        List<Player> group = [.. state.Players.Where(p => p.Group == playerGroup)];
        group = CheckAndFillOddCount(state.Players, group);

        Debug.WriteLine("\nPerforming cross pairing:");
        PrintPlayers(group);

        int groupSize = group.Count;
        int groupHalf = groupSize / 2;

        if (groupSize != 0)
        {
            for (int i = 0; i < groupHalf; i++)
            {
                Pairing pairing = PairPlayers(
                    state,
                    new(
                        parameters.Round,
                        parameters.HandicapReduction,
                        parameters.HandicapBasedMm,
                        parameters.HandicapMaxNine,
                        group[i],
                        group[i + groupHalf]));
                state.Pairings.Push(pairing);
            }
        }
        else
        {
            Debug.WriteLine("\nNo players were provided...");
        }

        return [.. state.Players.Except(group)];
    }

    private static List<Player> WeakestPairing(State state, PairingGeneratorParameters parameters, Group playerGroup)
    {
        List<Player> group = [.. state.Players.Where(p => p.Group == playerGroup)];
        group = CheckAndFillOddCount(state.Players, group);

        Debug.WriteLine("\nPerforming slaughter pairing:");
        PrintPlayers(group);

        int groupSize = group.Count;
        int groupHalf = groupSize / 2;

        if (groupSize != 0)
        {
            for (int i = 0, j = groupSize - 1; i < groupHalf; i++, j--)
            {
                Pairing pairing = PairPlayers(
                    state,
                    new(
                        parameters.Round,
                        parameters.HandicapReduction,
                        parameters.HandicapBasedMm,
                        parameters.HandicapMaxNine,
                        group[i],
                        group[j]));
                state.Pairings.Push(pairing);
            }
        }
        else
        {
            Debug.WriteLine("\nNo players were provided...");
        }

        return [.. state.Players.Except(group)];
    }

    private static List<Player> StrongestPairing(State state, PairingGeneratorParameters parameters, Group playerGroup)
    {
        List<Player> group = [.. state.Players.Where(p => p.Group == playerGroup)];
        group = CheckAndFillOddCount(state.Players, group);

        Debug.WriteLine("\nPerforming king of the hill pairing:");
        PrintPlayers(group);

        int groupSize = group.Count;

        if (groupSize != 0)
        {
            for (int i = 0; i < groupSize - 1; i += 2)
            {
                Pairing pairing = PairPlayers(
                    state,
                    new(
                        parameters.Round,
                        parameters.HandicapReduction,
                        parameters.HandicapBasedMm,
                        parameters.HandicapMaxNine,
                        group[i],
                        group[i + 1]));
                state.Pairings.Push(pairing);
            }
        }
        else
        {
            Debug.WriteLine("\nNo players were provided...");
        }

        return [.. state.Players.Except(group)];
    }

    private static List<Player> RandomPairing(State state, PairingGeneratorParameters parameters, Group playerGroup)
    {
        List<Player> group = [.. state.Players.Where(p => p.Group == playerGroup)];
        group = CheckAndFillOddCount(state.Players, group);
        List<Player> unpairedPlayers = [.. group];

        Debug.WriteLine("\nPerforming random pairing:");
        PrintPlayers(group);

        int groupSize = group.Count;

        if (groupSize != 0)
        {
            foreach (Player p1 in group)
            {

                if (!parameters.Round.UnpairedPlayers.Contains(p1))
                {
                    Debug.WriteLine("Skipping: " + p1);
                    continue;
                }

                Player p2 = unpairedPlayers[Utils.Random.Next(groupSize - 1) + 1];
                Pairing pairing = PairPlayers(
                    state,
                    new(
                        parameters.Round,
                        parameters.HandicapReduction,
                        parameters.HandicapBasedMm,
                        parameters.HandicapMaxNine,
                        p1, p2));
                state.Pairings.Push(pairing);

                groupSize -= 2;

                unpairedPlayers.Remove(p1);
                unpairedPlayers.Remove(p2);
            }
        }
        else
        {
            Debug.WriteLine("\nNo players were provided...");
        }

        return [.. state.Players.Except(group)];
    }

    private static List<Player> CheckAndFillOddCount(List<Player> players, List<Player> group)
    {
        if (group.Count != 0)
        {
            if (group.Count % 2 == 1)
            {
                Player first = players.Except(group).First();
                group.Add(first);
            }
        }

        return group;
    }

    private static void CheckForBye(State state, PairingGeneratorParameters parameters)
    {
        if (state.Players.Count % 2 == 1)
        {
            Debug.WriteLine("\nOdd players, making bye:");

            int lowestBye = state.Players.Min(p => p.ByeBalancer);
            Player byePlayer = state.Players.Where(p => p.ByeBalancer == lowestBye).Last();

            PrintPlayer(byePlayer);

            Player bye = new()
            {
                Group = Group.Bye
            };

            Pairing pairing = PairPlayers(
                state,
                new PairingParameters(
                    parameters.Round,
                    parameters.HandicapReduction,
                    parameters.HandicapBasedMm,
                    parameters.HandicapMaxNine,
                    byePlayer,
                    bye
                )
            );

            state.Pairings.Push(pairing);
        }
    }

    private static Pairing PairPlayers(State state, PairingParameters parameters)
    {
        Player p1 = parameters.P1;
        Player p2 = parameters.P2;

        if (p1.Opponents.ContainsValue(p2))
        {
            MessageBox.Show($"Hráči {p1.FullName} a {p2.FullName} už proti sebe hrali!", "Kolízia pri párovaní!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        Debug.WriteLine("\nPairing players: ");
        Debug.WriteLine(p1.Data.Last_Name + ", " + p1.Data.Name + ": " + p1.Data.Gor);
        Debug.WriteLine(p2.Data.Last_Name + ", " + p2.Data.Name + ": " + p2.Data.Gor);
        Pairing pairing = parameters.Round.AddPairing(p1, p2, parameters.HandicapReduction, parameters.HandicapBasedMm, parameters.HandicapMaxNine);

        state.Players.Remove(p1);
        state.Players.Remove(p2);

        return pairing;
    }

    private static void RemovePairing(State state, Round round, Pairing pairing)
    {
        state.Players.Add(pairing.Black);
        state.Players.Add(pairing.White);

        round.RemovePairing(pairing);
    }

    private static Player OpponentSelection(PairingMethod method, List<Player> opponents)
    {
        return method switch
        {
            PairingMethod.Strongest => opponents[0],
            PairingMethod.Weakest => opponents[^1],
            _ => opponents[Utils.Random.Next(opponents.Count)],
        };
    }

    private static List<Player> TryToAvoidSameCity(List<Player> list, Player player)
    {
        List<Player> opponents = [.. list.Where(p => !p.Data.Club.Equals(player.Data.Club))];

        if (opponents.Count == 0)
        {
            Debug.WriteLine("No opponent from different city found");
            return list;
        }

        else
        {
            Debug.WriteLine("Opponent from different city found");
            return opponents;
        }
    }

    private static List<Player> TryToAvoidHighHandicap(
        List<Player> opponentList,
        Player player,
        int handicapReduction,
        bool handicapBasedMm,
        bool handicapMaxNine)
    {
        List<Player> filteredOpponentList = [.. opponentList.Where(p =>
            Utils.CalculateHandicap(player, p, handicapReduction, handicapBasedMm, handicapMaxNine) <= 9)];

        if (filteredOpponentList.Count == 0)
        {
            Debug.WriteLine("No opponent with handicap 9 or less available");
            return opponentList;
        }

        else
        {
            Debug.WriteLine("Opponent with minimal handicap group found");
            return filteredOpponentList;
        }
    }

    private static List<Player> TryToAvoidSamePlayers(List<Player> opponentList, Player player)
    {
        List<Player> filteredOpponentList = [.. opponentList.Except(player.Opponents.Values)];

        if (filteredOpponentList.Count == 0)
        {
            Debug.WriteLine("Only same opponents left");
            return opponentList;
        }

        else
        {
            Debug.WriteLine("Found new opponent");
            return filteredOpponentList;
        }
    }

    [Conditional("DEBUG")]
    private static void PrintPlayer(Player player)
    {
        Debug.WriteLine(player.Data.Last_Name + ", " + player.Data.Name + ": " + player.Score + ", " + player.SODOS + ", " + player.SOS + ", " + player.SOSOS);
    }

    [Conditional("DEBUG")]
    private static void PrintPlayers(IEnumerable<Player> players)
    {
        foreach (Player p in players)
        {
            PrintPlayer(p);
        }
    }
}
