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
using System.Linq;

namespace GoCarlos.NET.Pairings;

public sealed class PlayerWrapper(Player player)
{
    public bool IsPaired { get; set; } = false;
    public Player Player { get; set; } = player;
    public HashSet<Player> TemporaryForbiddenPairing { get; set; } = [];
    public HashSet<Player> ForbiddenPairing { get; set; } = [.. player.Opponents.Values];

    public int PairingBalancer { get; init; }

    public bool PlayedDown { get; } = player.PairingBalancer.Values.Any(p => p < 0);
    public bool PlayedUp { get; } = player.PairingBalancer.Values.Any(p => p > 0);

    public override bool Equals(object? obj)
    {
        return obj is PlayerWrapper wrapper && Player.Equals(wrapper.Player);
    }

    public override int GetHashCode() => Player.GetHashCode();

    public override string? ToString() => Player.ToString();
}
