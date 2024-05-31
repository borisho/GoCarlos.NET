using GoCarlos.NET.Models;
using System.Collections.Generic;

namespace GoCarlos.NET.Interfaces;

/// <summary>
/// To be added
/// </summary>
public interface ITournament : IReferenceCleanup
{
    const int TOKEN_REFRESH_MAIN_VIEW_MODEL = 1;
    public int CurrentRound { get; set; }
    public Settings Settings { get; }
    public IEnumerable<Player> Players { get; }
    public IEnumerable<Pairing> Pairings { get; }
    public IEnumerable<Player> UnpairedPlayers { get; }
    public void AddPlayer(Player player);
    public void RemovePlayer(Player player);
    public void AddPairing(Pairing pairing);
    public void RemovePairing(Pairing pairing);
    public void Reset();
}
