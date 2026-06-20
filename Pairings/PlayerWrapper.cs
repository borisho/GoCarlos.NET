using GoCarlos.NET.Models;
using System;
using System.Collections.Generic;

namespace GoCarlos.NET.Pairings;

public sealed class PlayerWrapper(Player player)
{
    public static PlayerWrapper ByeWrapper { get; } = new(Tournament.ByePlayer);

    public bool IsPaired { get; set; } = false;
    public Player Player { get; set; } = player;
    public HashSet<Player> TemporaryForbiddenPairing { get; set; } = [];
    public HashSet<Player> ForbiddenPairing { get; set; } = [.. player.Opponents.Values];

    public override bool Equals(object? obj)
    {
        return obj is PlayerWrapper wrapper && Player.Equals(wrapper.Player);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Player, TemporaryForbiddenPairing, ForbiddenPairing);
    }
}
