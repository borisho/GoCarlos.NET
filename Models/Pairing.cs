using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GoCarlos.NET.Models;

public class Pairing : IEquatable<Pairing?>
{
    [JsonProperty]
    private readonly Guid uuid;

    private DateTime timeStamp;
    private int boardNumber;

    private Player black;
    private Player white;
    private int handicap;

    private Result result;
    private bool resultByReferee;
    private string comment;

    public Pairing()
    {
        uuid = Guid.NewGuid();

        timeStamp = DateTime.Now;
        black = new();
        white = new();
        comment = "";
    }

    public Pairing(Player black, Player bye) : this()
    {
        result = Result.BLACK_WON;

        this.black = black;
        white = bye;
        handicap = 0;
        boardNumber = 0;
        resultByReferee = false;
        comment = "free";
    }

    public Pairing(Player black, Player white, int handicap, string comment) : this()
    {
        result = Result.NONE;

        this.black = black;
        this.white = white;
        this.handicap = handicap;
        this.comment = comment;
        boardNumber = 0;
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

    public float GetPairingResult(Player player)
    {
        if (Result.Equals(Result.BOTH_WON) ||
            (Black.Equals(player) && Result.Equals(Result.BLACK_WON)) ||
            (White.Equals(player) && Result.Equals(Result.WHITE_WON)))
        {
            return 1f;
        }
        else if (Result.Equals(Result.DRAW))
        {
            return 0.5f;
        }
        else
        {
            return 0f;
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
