using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GoCarlos.NET.Models;

public class Player : IEquatable<Player?>
{
    [JsonProperty]
    private readonly Guid uuid;

    [JsonProperty]
    private readonly HashSet<int> roundsPlaying;

    [JsonProperty]
    private readonly HashSet<Player> temporaryForbidenPairings;

    [JsonProperty]
    private readonly Dictionary<int, Player> opponents;

    [JsonProperty]
    private readonly Dictionary<int, Pairing> pairings;

    private EGD_Data data;

    private string grade;
    private int rating;

    private Group group;

    private int byeBalancer;
    private int colorBalancer;
    private int pairingBalancer;
    private int startScore;

    private int place;
    private float points;
    private float score;
    private float scoreX;
    private float sos;
    private float sosos;
    private float sodos;

    public Player()
    {
        uuid = Guid.NewGuid();

        roundsPlaying = [];
        temporaryForbidenPairings = [];
        opponents = [];
        pairings = [];
        data = new()
        {
            Last_Name = "BYE",
        };

        grade = "30k";
        rating = -900;

        group = Group.Default;

        byeBalancer = 0;
        colorBalancer = 0;
        pairingBalancer = 0;
        startScore = 0;

        place = 0;
        points = 0F;
        score = 0F;
        scoreX = 0F;
        sos = 0F;
        sosos = 0F;
        sodos = 0F;
    }

    public Player(EGD_Data data) : this()
    {
        roundsPlaying = [0,1,2,3,4];

        this.data = data;
        grade = this.data.Grade;

        if (data.Gor == "")
        {
            data.Gor = Utils.GetRating(grade).ToString();
            rating = Utils.GetRating(grade);
        }
        else
        {
            rating = int.Parse(data.Gor);
        }
        startScore = Utils.GetValue(grade);

        score = startScore;
    }

    public Player(HashSet<int> roundsPlaying, EGD_Data data) : this(data)
    {
        this.roundsPlaying = roundsPlaying;
    }

    public Guid Guid { get => uuid; }
    public HashSet<int> RoundsPlaying { get => roundsPlaying; }
    public HashSet<Player> TemporaryForbiddenPairings { get => temporaryForbidenPairings; }
    public Dictionary<int, Player> Opponents { get => opponents; }
    public Dictionary<int, Pairing> Pairings { get => pairings; }
    public EGD_Data Data { get => data; set => data = value; }
    public string FullName { get => $"{data.Last_Name} {data.Name}"; }

    public string Grade
    {
        get => grade;
        set
        {
            grade = value;
            data.Grade = value;
            data.Grade_n = Utils.GetValue(value).ToString();
            if (data.Gor == "") rating = Utils.GetRating(value);
            startScore = Utils.GetValue(grade);
        }
    }
    public int Rating { get => rating; set => rating = value; }

    public Group Group { get => group; set => group = value; }

    public int ByeBalancer { get => byeBalancer; set => byeBalancer = value; }
    public int ColorBalancer { get => colorBalancer; set => colorBalancer = value; }
    public int PairingBalancer { get => pairingBalancer; set => pairingBalancer = value; }
    public int StartScore { get => startScore; set => startScore = value; }

    public int Place { get => place; set => place = value; }
    public float Points { get => points; set => points = value; }
    public float Score { get => score; set => score = value; }
    public float ScoreX { get => scoreX; set => scoreX = value; }
    public float SOS { get => sos; set => sos = value; }
    public float SOSOS { get => sosos; set => sosos = value; }
    public float SODOS { get => sodos; set => sodos = value; }

    public override bool Equals(object? obj)
    {
        return obj is Player player &&
            uuid.Equals(player.uuid);
    }
    public bool Equals(Player? other)
    {
        return other is not null &&
            uuid.Equals(other.uuid);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(uuid);
    }
    public static bool operator ==(Player? left, Player? right)
    {
        return EqualityComparer<Player>.Default.Equals(left, right);
    }
    public static bool operator !=(Player? left, Player? right)
    {
        return !(left == right);
    }
    public override string? ToString()
    {
        return string.Format("Player {0}: {1} {2} {3}, Rating: {4}", uuid, data.Name, data.Last_Name, Grade, Rating);
    }
}