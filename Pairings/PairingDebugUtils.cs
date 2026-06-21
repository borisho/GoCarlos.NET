using GoCarlos.NET.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace GoCarlos.NET.Pairings;

public static class PairingDebugUtils
{
    [Conditional("DEBUG")]
    public static void PrintPlayer(Player player)
    {
        Debug.WriteLine(player.Data.Last_Name + ", " 
            + player.Data.Name + ": " 
            + player.Score + ", " 
            + player.SODOS + ", " 
            + player.SOS + ", " 
            + player.SOSOS);
    }

    [Conditional("DEBUG")]
    public static void PrintPlayers(IEnumerable<PlayerWrapper> players)
    {
        foreach (var wrapper in players)
        {
            ;
            Debug.Write("Paired: " + wrapper.IsPaired + ", ");
            PrintPlayer(wrapper.Player);
        }
    }
}
