using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Enums;

namespace GoCarlos.NET.Models;

public partial class Pairing : ObservableObject
{
    [ObservableProperty]
    private int roundNumber, boardNumber, handicap;

    [ObservableProperty]
    private Result result;

    [ObservableProperty]
    private Player black, white;

    public Pairing(int roundNumber)
    {
        RoundNumber = roundNumber;
        BoardNumber = 0;
        Handicap = 0;

        Result = Result.NONE;

        black = new Player();
        white = new Player();
    }

    public Pairing(int roundNumber, int boardNumber, int handicap, Result result, Player black, Player white)
    {
        RoundNumber = roundNumber;
        BoardNumber = boardNumber;
        Handicap = handicap;
        Result = result;
        Black = black;
        White = white;
    }

    public string Title => Black.FullName + " - " + White.FullName;

    public bool ContainsPlayer(Player player) => Black.Equals(player) || White.Equals(player);

    public override bool Equals(object? obj) => obj is Pairing pairing &&
        ((Black.Equals(pairing.Black) && White.Equals(pairing.White)) ||
        (Black.Equals(pairing.White) && White.Equals(pairing.Black)));

    public override int GetHashCode() => base.GetHashCode();
}
