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

using GoCarlos.NET.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace GoCarlos.NET.Pairings;

internal static class PairingDebugUtils
{
    [Conditional("DEBUG")]
    internal static void PrintPlayer(Player player)
    {
        Debug.WriteLine(player.Data.Last_Name + ", " 
            + player.Data.Name + ": " 
            + player.Score + ", " 
            + player.SODOS + ", " 
            + player.SOS + ", " 
            + player.SOSOS);
    }

    [Conditional("DEBUG")]
    internal static void PrintPlayers(IEnumerable<PlayerWrapper> players)
    {
        foreach (var wrapper in players)
        {
            Debug.Write("Paired: " + wrapper.IsPaired + ", ");
            PrintPlayer(wrapper.Player);
        }
    }   

    [Conditional("DEBUG")]
    internal static void PrintGroupsOfPlayers(IEnumerable<IEnumerable<PlayerWrapper>> playerGroups)
    {
        foreach (var group in playerGroups)
        {
            Debug.WriteLine("\nGroup:");
            PrintPlayers(group);
        }
    }

    [Conditional("DEBUG")]
    internal static void PrintGroupsOfPlayers(IEnumerable<MsrGenerator.GroupState> playerGroups)
    {
        foreach (var group in playerGroups)
        {
            Debug.WriteLine("\nGroup:");
            PrintPlayers(group.Players);
        }
    }

    [Conditional("DEBUG")]
    internal static void PrintPairings(IEnumerable<(PlayerWrapper P1, PlayerWrapper P2)> pairings)
    {
        Debug.WriteLine("\nPairings:");
        foreach (var (p1, p2) in pairings)
        {
            Debug.WriteLine($"  {p1.Player.FullName} vs {p2.Player.FullName}");
        }
    }
}
