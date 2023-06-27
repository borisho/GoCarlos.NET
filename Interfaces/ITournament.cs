using GoCarlos.NET.Models;
using System.Collections.ObjectModel;

namespace GoCarlos.NET.Interfaces;

public interface ITournament
{
    public ObservableCollection<Player> Players { get; set; }
}
