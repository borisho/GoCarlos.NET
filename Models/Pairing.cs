using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GoCarlos.NET.Models;

public class Pairing() : IEquatable<Pairing?>
{
    [JsonProperty]
    private readonly Guid uuid = Guid.NewGuid();

    [JsonProperty]
    private readonly int roundNumber;

    private DateTime timeStamp = DateTime.Now;

    private Player black = new();
    private Player white = new();

    private int boardNumber = 0;
    private int handicap = 0;

    private Result result = Result.NONE;
    private bool resultByReferee = false;
    private string comment = "";

    public Pairing(int roundNumber) : this()
    {
        this.roundNumber = roundNumber;
    }

    public Pairing(Player black, Player bye, int roundNumber) : this(roundNumber)
    {
        result = Result.BLACK_WON;

        this.black = black;
        white = bye;
        comment = "free";
    }

    public Pairing(Player black, Player white, int handicap, string comment, int roundNumber) : this(roundNumber)
    {
        this.black = black;
        this.white = white;
        this.handicap = handicap;
        this.comment = comment;
    }

    public Guid Guid { get => uuid; }
    public DateTime TimeStamp { get => timeStamp; set => timeStamp = value; }
    public int BoardNumber { get => boardNumber; set => boardNumber = value; }
    public Player Black { get => black; set => black = value; }
    public Player White { get => white; set => white = value; }
    public int Handicap { get => handicap; set => handicap = value; }
    public Result Result { get => result; set => result = value; }
    public bool ResultByReferee { get => resultByReferee; set => resultByReferee = value; }
    public string Comment { get => comment; set => comment = value; }
    public int RoundNumber { get => roundNumber; }

    public decimal GetPairingResult(Player player)
    {
        if (Result.Equals(Result.BOTH_WON) ||
            (Black.Equals(player) && Result.Equals(Result.BLACK_WON)) ||
            (White.Equals(player) && Result.Equals(Result.WHITE_WON)))
        {
            return 1M;
        }
        else if (Result.Equals(Result.DRAW))
        {
            return 0.5M;
        }
        else
        {
            return 0M;
        }
    }

    public bool IsPlayerPaired(Player player)
    {
        return player == black || player == white;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Pairing);
    }
    public bool Equals(Pairing? other)
    {
        return other is not null &&
               uuid.Equals(other.uuid);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(uuid);
    }
    public static bool operator ==(Pairing? left, Pairing? right)
    {
        return EqualityComparer<Pairing>.Default.Equals(left, right);
    }
    public static bool operator !=(Pairing? left, Pairing? right)
    {
        return !(left == right);
    }
    public override string? ToString()
    {
        return string.Format("Pairing {0}: {1} {2} vs {3} {4}", uuid, black.Data.Name, black.Data.Last_Name, white.Data.Name, white.Data.Last_Name);
    }
}
