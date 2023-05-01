using System;
using GoCarlos.NET.Models;

namespace GoCarlos.NET.Events;

public class PlayerEventArgs : EventArgs
{
    public PlayerEventArgs(Player player)
    {
        Player = player;
    }

    public Player Player { get; set; }
    public bool? Bool { get; set; }
}
