using GoCarlos.NET.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using static GoCarlos.NET.Pairings.PairingDebugUtils;

namespace GoCarlos.NET.Pairings;

/// <summary>Pairing algorithm according to the original GoCarlos specification.</summary>
/// <remarks>Not thread safe</remarks>
public static class MsrGenerator
{
    private enum CriteriaStrength
    {
        ALL,
        REDUCED,
        NONE
    }

    /// <summary>
    /// Wrapper for Players in same group with their in group pairings.
    /// </summary>
    internal class GroupState
    {
        public List<PlayerWrapper> Players { get; set; } = [];
        public List<(PlayerWrapper P1, PlayerWrapper P2)> Pairings { get; set; } = [];
    }

    /// <summary>
    /// Pair players by original GoCarlos pairing algorithm.
    /// </summary>
    /// <remarks>
    /// If pairings cannot be made without repeating opponents no pairings are returned.
    /// </remarks>
    /// <param name="orderedPlayers">Ordered list of players to be paired.</param>
    /// <returns>List of pairings</returns>
    public static List<(PlayerWrapper P1, PlayerWrapper P2)>? Pair(List<Player> orderedPlayers)
    {
        if (orderedPlayers.Count == 0)
        {
            Debug.WriteLine("\nEmpty list no pairing is made");
            return null;
        }

        ImmutableList<ImmutableList<PlayerWrapper>> startingGroups = [.. orderedPlayers
            .Select(p => new PlayerWrapper(p))
            .GroupBy(p => p.Player.Score)
            .Select(grp => grp.ToImmutableList())];

        Debug.WriteLine("\nOrdered players in immutable groups: ");
        PrintGroupsOfPlayers(startingGroups);

        /*
         * ALGORITMUS losovacieho programu GoCarlos
         * ------------------------------------------
         * Oznacenie:
         * - pre kazdeho hraca a zo skupiny A bude D(a) mnozina hracov, s ktorymi nebude moct hrat
         * - m je prvy hrac skupiny A
         * 
         * (START) -> (CH).
         * 
         * (CH)     Ak je pocet hracov, ktory sa ide nalosovat, neparny, doplnime
         *          na koniec pre zjednodusenie formulacie Algoritmu fiktivneho
         *          hraca, ktory ma vzdy 0 bodov. Hrat s fiktivnym hracom znamena
         *          pauzovat.
         *          
         *          Pokracujeme -> (1S).
         * 
         *          Pozn. Krokmi (1S) az (3S) rozdelime vsetkych hracov do skupin s
         *          parnymi poctami hracov, pricom plati, ze vsetci hraci z rovnakym
         *          poctom bodov sa nachadzaju v jednej skupine.
         *          
         *          Splené pred párovaním, BYE hráč je doplnený pred volaním Pair().
         *          
         * (1S)     Ak je kazdy hrac zaradeny do nejakej skupiny, pokracujeme -> (1).
         *          Ak existuje hrac, ktory nepatri do ziadnej skupiny, zalozime novu
         *          skupinu S a pokracujeme -> (2S)
         */

        List<GroupState> groups = [];

        for (int i = 0; i < startingGroups.Count; i++)
        {
            /*
             * (2S)     Zoberieme mnozinu hracov s najvyssim poctom bodov tak, ze nie je
             *          este podmnozinou ziadnej skupiny a zaradime ju do skupiny S.
             *          Pokracujeme -> (3S).
             *          
             * (3S)     Ak ma S neparny pocet clenov, pokracujeme -> (2S).
             *          Ak ma S parny pocet clenov, pokracujeme -> (1S).
             */
            List<PlayerWrapper> S = [.. startingGroups[i]];
            while (S.Count % 2 != 0)
            {
                i++;
                S.AddRange(startingGroups[i]);
            }

            groups.Add(new GroupState { Players = S });
        }

        Debug.WriteLine("\nEven groups after merging, ready for pairing:");
        PrintGroupsOfPlayers(groups);

        List<(PlayerWrapper P1, PlayerWrapper P2)> finalPairings = [];
        int currentGroupIndex = 0;

        Debug.WriteLine("\nStarting pairing of groups");

        /*
         * (1)      Ak vo vsetkych skupinach maju hraci superov, algoritmus je hotovy.
         *          -> (END).
         *          
         *          Ak existuje skupina, v ktorej hracom este neboli prideleni superi,
         *          zoberieme taku skupinu s najvyssim poctom bodov a oznacime ju A.
         *          
         *          Pokracujeme -> (2)
         */
        while (currentGroupIndex < groups.Count)
        {
            GroupState A = groups[currentGroupIndex];

            Debug.WriteLine("\nPairing group: " + currentGroupIndex);
            PrintPlayers(A.Players);

            if (A.Players.All(p => p.IsPaired))
            {
                Debug.WriteLine("\nAll players in group " + currentGroupIndex + " are paired.");
                currentGroupIndex++;
                continue;
            }

            /*
             * (2)      Utriedime hracov skupiny A podla dopredu urcenych kriterii.
             *          Vsetky mnoziny D(a) su prazdne.
             *          
             *          Pokracujeme-> (3)
             */
            Debug.WriteLine("\nClear temporary forbidden pairings for group " + currentGroupIndex);
            A.Players.ForEach(p => p.TemporaryForbiddenPairing.Clear());

            CriteriaStrength CS = CriteriaStrength.ALL;

            // (3)
            if (!TryPairGroup(A, ref CS))
            {
                Debug.WriteLine("\nCould not pair players in group: " + currentGroupIndex + " with criteria: " + CS);
                // (4)
                if (!HandleGroupMerge(groups, currentGroupIndex, finalPairings, out bool wasMergedWithPrevious))
                {
                    Debug.WriteLine("\nPairing is not possible - cannot merge groups");
                    return null;
                }

                // If merged with previous group, adjust index
                if (wasMergedWithPrevious)
                {
                    currentGroupIndex--;
                }

                continue;
            }

            // Add pairings from group A to final pairings
            foreach (var (p1, p2) in A.Pairings)
            {
                Debug.WriteLine("Pairing players " + p1.Player.FullName + " and " + p2.Player.FullName);
                finalPairings.Add((p1, p2));
            }

            currentGroupIndex++;
        }

        // (END)
        if (finalPairings.Count == 0)
        {
            Debug.WriteLine("\nIt is not possible to make pairing without repetition of opponent.");
            return null;
        }

        PrintPairings(finalPairings);

        return finalPairings;
    }

    private static bool TryPairGroup(
        GroupState A,
        ref CriteriaStrength CS)
    {
        /*
         * (3)      Nech a je prvy hrac zo skupiny A, ktory je zatial bez supera.
         *          Hracovi a hladame do dvojice najblizsieho nasledujuceho hraca z A,
         *          ktory nepatri do skupiny D(a) (oznacime ho b), takeho, ze pre
         *          dvojicu hracov (a,b) plati:
         *          
         *          a) Hraci a, b este spolu nehrali.
         *          b) Medzi hracmi a,b je rozdiel bodov max. jedna.
         *          c) Ak je rozdiel bodov 1, hrac a hra prvy krat s hracom s nizsim
         *             poctom bodov.
         *          d) Ak je rozdiel bodov 1, hrac b hra prvy krat s hracom s vyssim
         *             poctom bodov. ***
         *          
         *          Ak sme takeho hraca b nasli a v A existuje hrac bez supera,
         *          pokracujeme -> (3).
         *          (Pozn. V ramci skupiny A sme utvorili dalsiu dvojicu hracov.)
         *          
         *          Ak sme takeho hraca b nasli a v A maju vsetci hraci supera,
         *          pokracujeme -> (1).
         *          (Pozn. V skupine A sme kazdemu hracovi priradili supera.)
         */
        Debug.WriteLine("\nTrying to pair group with criteria: " + CS);

        int aIndex = 0;
        PlayerWrapper m = A.Players[0];


        while (aIndex < A.Players.Count)
        {
            PlayerWrapper a = A.Players[aIndex];
            Debug.WriteLine("\nTrying to find opponent for player: " + a.Player.FullName + " with criteria: " + CS);

            if (a.IsPaired)
            {
                Debug.WriteLine("Player " + a.Player.FullName + " is already paired, skipping.");
                aIndex++;
                continue;
            }

            PlayerWrapper? b = FindOpponent(A.Players, a, ref CS, aIndex);

            if (b is null)
            {
                /*
                 * Ak nejakemu hracovi a <> m nenajdeme hraca b s podmienkami (3a) - (3d),
                 * vyprazdnime mnozinu D(a), zoberieme poslednu zostavenu dvojicu (k,l),
                 * zrusime ju, hraca l zaradime naspat na povodne miesto v skupine A
                 * a zaroven hraca l doplnime do skupiny D(k) (nemoze hrat s hracom k).
                 * 
                 * Dalej pokracujeme -> (3).
                 * (Pozn. Rozhodili sme poslednu dvojicu.)
                 */
                if (a != m)
                {
                    // vyprazdnime mnozinu D(a)
                    Debug.WriteLine("Player " + a.Player.FullName + " is not first player, attempting backtrack");
                    a.TemporaryForbiddenPairing.Clear();

                    if (A.Pairings.Count > 0)
                    {
                        // zoberieme poslednu zostavenu dvojicu (k,l), zrusime ju
                        var (lastP1, lastP2) = A.Pairings[^1];
                        Debug.WriteLine($"Backtracking pair: {lastP1.Player.FullName} <-> {lastP2.Player.FullName}");

                        A.Pairings.RemoveAt(A.Pairings.Count - 1);
                        lastP1.IsPaired = false;
                        lastP2.IsPaired = false;

                        // hraca l doplnime do skupiny D(k) (nemoze hrat s hracom k)
                        lastP1.TemporaryForbiddenPairing.Add(lastP2.Player);
                        lastP2.TemporaryForbiddenPairing.Add(lastP1.Player);

                        // hraca l zaradime naspat na povodne miesto v skupine A - index
                        int lastP1Index = A.Players.IndexOf(lastP1);
                        aIndex = lastP1Index;

                        continue;
                    }
                    else
                    {
                        Console.Error.WriteLine("No pairings to backtrack for non-first player!");
                        return false;
                    }
                }
                else
                {
                    /*
                     * Ak takeho hraca b nenajdeme pre a = m, znamena to, ze v skupine A sa
                     * nedaju zostavit dvojice tak, aby boli dodrzane kriteria (3a) az (3d).
                     * Prestavame brat do uvahy kriteria (3c) a (3d) a pokracujeme -> (2).
                     * (Pozn. Oslabili sme kriteria na zostavenie dvojic na (3a) a (3b).)
                     */
                    if (CS == CriteriaStrength.ALL)
                    {
                        Debug.WriteLine("First player could not find opponent with ALL criteria, weakening to REDUCED");
                        CS = CriteriaStrength.REDUCED;
                        continue;
                    }
                    /*
                     * Ak sme ani takto nenasli supera pre a = m, znamena to, ze v skupine A
                     * sa nedaju zostavit dvojice tak, aby boli dodrzane kriteria (3a) a (3b).
                     * Prestavame brat do uvahy kriteria (3b) az (3d) a pokracujeme -> (2).
                     * (Pozn. Oslabili sme kriteria na zostavenie dvojic na (3a).)
                     */
                    else if (CS == CriteriaStrength.REDUCED)
                    {
                        Debug.WriteLine("First player could not find opponent with REDUCED criteria, weakening to NONE");
                        CS = CriteriaStrength.NONE;
                        continue;
                    }
                    /*
                     * Ak sme ani takto nenasli supera pre a = m,
                     * znamena to, ze v skupine A sa nedaju zostavit dvojice tak,
                     * aby bolo dodrzane kriterium (3a).
                     * 
                     * Pokracujeme -> (4).
                     */
                    else
                    {
                        Debug.WriteLine("First player could not find opponent with NONE criteria, merging groups");
                        return false;
                    }
                }
            }
            else
            {
                a.IsPaired = true;
                b.IsPaired = true;
                Debug.WriteLine($"Found opponent for {a.Player.FullName}: {b.Player.FullName}");
                A.Pairings.Add((a, b));
                aIndex++;
            }
        }

        return true;
    }

    private static PlayerWrapper? FindOpponent(
        List<PlayerWrapper> A,
        PlayerWrapper a,
        ref CriteriaStrength CS,
        int aIndex)
    {
        foreach (PlayerWrapper b in A.Skip(aIndex + 1))
        {
            // Skip if already paired
            if (b.IsPaired) continue;

            // Skip if in forbidden pairing list
            if (a.TemporaryForbiddenPairing.Contains(b.Player) ||
                a.ForbiddenPairing.Contains(b.Player)) continue;

            //  a) Hraci a, b este spolu nehrali.
            if (a.Player.Opponents.ContainsValue(b.Player)) continue;

            decimal scoreDiff = a.Player.Score - b.Player.Score;

            if (CS == CriteriaStrength.ALL)
            {
                if (Math.Abs(scoreDiff) > 1) continue;

                // b) Medzi hracmi a,b je rozdiel bodov max. jedna.
                if (Math.Abs(scoreDiff) == 1)
                {
                    // c) Ak je rozdiel bodov 1, hrac a hra prvy krat s hracom s nizsim poctom bodov.
                    if (a.Player.PairingBalancer.Values.Any(p => p < 0))
                        continue;

                    // d) Ak je rozdiel bodov 1, hrac b hra prvy krat s hracom s vyssim poctom bodov.
                    if (b.Player.PairingBalancer.Values.Any(p => p > 0))
                        continue;
                }
            }
            else if (CS == CriteriaStrength.REDUCED)
            {
                // b) Medzi hracmi a,b je rozdiel bodov max. jedna.
                if (Math.Abs(scoreDiff) > 1)
                    continue;

                return b;
            }
            else if (CS == CriteriaStrength.NONE)
            {
                //  a) Hraci a, b este spolu nehrali. - skontrolovane vyssie
                return b;
            }
        }

        return null;
    }

    private static bool HandleGroupMerge(
        List<GroupState> pairingGroups,
        int currentGroupIndex,
        List<(PlayerWrapper P1, PlayerWrapper P2)> finalPairings,
        out bool wasMergedWithPrevious)
    {
        wasMergedWithPrevious = false;
        GroupState A = pairingGroups[currentGroupIndex];


        /*
         * (4)      Ak existuje nasledujuca skupina B, spojime ju s A
         *          (A := A zjednotenie B) a pri pouzitch vsetkych kriterii (3a) - (3d)
         *          pokracujeme -> (2).
         */
        if (currentGroupIndex + 1 < pairingGroups.Count)
        {
            GroupState B = pairingGroups[currentGroupIndex + 1];

            // (A:= A zjednotenie B)
            Debug.WriteLine($"\nMerging group {currentGroupIndex} with next group {currentGroupIndex + 1}");
            A.Players.AddRange(B.Players);
            pairingGroups.RemoveAt(currentGroupIndex + 1);

            return true;
        }
        /*
         *          Ak A je posledna skupina, rozhodime vsetky zostavene dvojice v
         *          predchadzajucej skupine (ozn. B) a skupiny A a B spojime
         *          (t.j. A := A zjednotenie B). Pri pouziti vsetkych kriterii
         *          (3a) - (3d) pokracujeme -> (2).
         *          
         *          Pozn. Spojili sme A s nasledujucou resp. predchadzajucou skupinou.
         */
        else if (currentGroupIndex > 0)
        {
            GroupState B = pairingGroups[currentGroupIndex - 1];

            Debug.WriteLine($"\nA is last group. Undoing pairings in group {currentGroupIndex - 1} and merging with group {currentGroupIndex}");
            foreach (var (p1, p2) in B.Pairings)
            {
                p1.IsPaired = false;
                p2.IsPaired = false;
                finalPairings.Remove((p1, p2));
                Debug.WriteLine("Pairing removed for players: " + p1.Player.FullName + ", " + p2.Player.FullName);
            }
            B.Pairings.Clear();

            // (A := A zjednotenie B)
            B.Players.AddRange(A.Players);
            pairingGroups.RemoveAt(currentGroupIndex);

            wasMergedWithPrevious = true;
            return true;
        }

        return false;
    }
}