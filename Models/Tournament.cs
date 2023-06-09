using GoCarlos.NET.Interfaces;

namespace GoCarlos.NET.Models;

public class Tournament : ITournament
{
    public string Title { get; set; } = "";
    public int Rounds { get; set; } = 5;
}
