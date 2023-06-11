using GoCarlos.NET.Core.Interfaces;

namespace GoCarlos.NET.Core;

public class Tournament : ITournament
{
    public string Title { get; set; } = "";
    public int Rounds { get; set; } = 5;
}
