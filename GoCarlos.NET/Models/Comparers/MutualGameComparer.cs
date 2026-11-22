using GoCarlos.NET.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace GoCarlos.NET.Models.Comparers;

public class MutualGameComparer : IComparer<Player>
{
    public int Compare(Player? x, Player? y)
    {
        if (x == null || y == null) return 0;

        var pairing = x.Pairings.Values.FirstOrDefault(p => 
            p.Black.Equals(y) || p.White.Equals(y));

        if (pairing == null) return 0;

        bool isBlack = pairing.Black.Equals(y);

        return pairing.Result switch
        {
            Result.BLACK_WON => isBlack ? -1 : 1,
            Result.WHITE_WON => isBlack ? 1 : -1,
            _ => 0,
        };
    }
}
