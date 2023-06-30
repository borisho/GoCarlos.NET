using GoCarlos.NET.Models;
using System.Collections.ObjectModel;

namespace GoCarlos.NET.Interfaces;

/// <summary>
/// To be added
/// </summary>
public interface ITournament
{
    public int Rounds { get; set; }
    public ObservableCollection<Player> Players { get; set; }
    public void Reset();
}
