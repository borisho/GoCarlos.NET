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
