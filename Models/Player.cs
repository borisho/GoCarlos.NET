using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GoCarlos.NET.Models;

public class Player : IEquatable<Player?>
{
    [JsonProperty] private readonly Guid uuid;
    [JsonProperty] private readonly HashSet<int> roundsPlaying;
    [JsonProperty] private readonly HashSet<Player> temporaryForbiddenPairings;
    [JsonProperty] private readonly Dictionary<int, Player> opponents;
    [JsonProperty] private readonly Dictionary<int, Pairing> pairings;
    [JsonProperty] private readonly Dictionary<int, bool> colorBalancer;
    [JsonProperty] private readonly Dictionary<int, int> pairingBalancer;

    private EGD_Data data;

    private string grade;
    private int rating;

    private Group group;
    private bool sharedPlace;

    private int byeBalancer;
    private int startScore;

    private int place;
    private decimal points;
    private decimal score;
    private decimal scoreX;
    private decimal sos;
    private decimal sosos;
    private decimal sodos;

    public Player()
    {
        uuid = Guid.NewGuid();

        roundsPlaying = [];
        temporaryForbiddenPairings = [];
        opponents = [];
        pairings = [];
        colorBalancer = [];
        pairingBalancer = [];

        data = new()
        {
            Last_Name = "BYE",
        };

        grade = "30k";
        rating = -900;

        group = Group.Default;
        sharedPlace = false;

        byeBalancer = 0;
        startScore = 0;

        place = 0;
        points = 0M;
        score = 0M;
        scoreX = 0M;
        sos = 0M;
        sosos = 0M;
        sodos = 0M;
    }

    public Player(EGD_Data data, int numberOfRounds) : this(data)
    {
        for (int i = 0; i < numberOfRounds; i++)
            roundsPlaying.Add(i);
    }

    public Player(EGD_Data data, HashSet<int> roundsPlaying) : this(data)
    {
        this.roundsPlaying = roundsPlaying;
    }

    private Player(EGD_Data data) : this()
    {
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

    public Guid Guid { get => uuid; }
    public HashSet<int> RoundsPlaying { get => roundsPlaying; }
    public HashSet<Player> TemporaryForbiddenPairings { get => temporaryForbiddenPairings; }
    public Dictionary<int, Player> Opponents { get => opponents; }
    public Dictionary<int, Pairing> Pairings { get => pairings; }
    public Dictionary<int, bool> ColorBalancer { get => colorBalancer; }
    public Dictionary<int, int> PairingBalancer { get => pairingBalancer; }
    public EGD_Data Data { get => data; set => data = value; }
    [JsonIgnore] public string FullName { get => $"{data.Last_Name} {data.Name}"; }

    public string Grade
    {
        get => grade;
        set
        {
            grade = value;
            data.Grade = value;
            data.Grade_n = Utils.GetValue(value).ToString();
            startScore = Utils.GetValue(value);
            if (data.Gor == "")
            {
                data.Gor = Utils.GetRating(value).ToString();
                rating = Utils.GetRating(value);
            }
        }
    }
    public int Rating { get => rating; set => rating = value; }
    public Group Group { get => group; set => group = value; }
    public bool SharedPlace { get => sharedPlace; set => sharedPlace = value; }
    public int ByeBalancer { get => byeBalancer; set => byeBalancer = value; }
    public int StartScore { get => startScore; set => startScore = value; }
    public int Place { get => place; set => place = value; }
    public decimal Points { get => points; set => points = value; }
    public decimal Score { get => score; set => score = value; }
    public decimal ScoreX { get => scoreX; set => scoreX = value; }
    public decimal SOS { get => sos; set => sos = value; }
    public decimal SOSOS { get => sosos; set => sosos = value; }
    public decimal SODOS { get => sodos; set => sodos = value; }

    public bool PlayedWhite(int roundNumber) => ColorBalancer.TryGetValue(roundNumber, out bool w) && w;
    public int GetPairingBalancer(int roundNumber) => PairingBalancer.GetValueOrDefault(roundNumber);

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