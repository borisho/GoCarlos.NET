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

using GoCarlos.NET.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace GoCarlos.NET.Models.Comparers;

public class MutualGameComparer : IComparer<Player>
{
    public int Compare(Player? x, Player? y)
    {
        if (x is null || y is null || ReferenceEquals(x, y)) return 0;

        var pairing = x.Pairings.Values.FirstOrDefault(p => 
            p.Black.Equals(y) || p.White.Equals(y));

        if (pairing is null) return 0;

        bool isBlack = pairing.Black.Equals(y);

        return pairing.Result switch
        {
            Result.BLACK_WON => isBlack ? -1 : 1,
            Result.WHITE_WON => isBlack ? 1 : -1,
            _ => 0,
        };
    }
}
