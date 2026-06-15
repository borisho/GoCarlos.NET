using GoCarlos.NET.Models;
using System;

namespace GoCarlos.NET.Events;

public class PlayerEventArgs(Player player) : EventArgs
{
    public Player Player { get; set; } = player;
    public bool? Bool { get; set; }
}
