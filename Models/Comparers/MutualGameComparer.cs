using GoCarlos.NET.Models.Enums;
using System.Collections.Generic;

namespace GoCarlos.NET.Models.Comparers;

public class MutualGameComparer : IComparer<Player>
{
    public int Compare(Player? x, Player? y)
    {
        if (x == null || y == null) return 0;

        foreach (KeyValuePair<int, Pairing> kvp in x.Pairings)
        {
            Pairing p = kvp.Value;

            if (p.Black.Equals(y))
            {
                return p.Result switch
                {
                    Result.BLACK_WON => -1,
                    Result.WHITE_WON => 1,
                    _ => 0,
                }; ;
            }

            else if (p.White.Equals(y))
            {
                return p.Result switch
                {
                    Result.BLACK_WON => 1,
                    Result.WHITE_WON => -1,
                    _ => 0,
                };
            }
        }

        return 0;
    }
}
