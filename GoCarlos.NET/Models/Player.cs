// Copyright (C) 2023 Boris Dovčík
// This file is part of GoCarlos.NET.
//
// GoCarlos.NET is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 3, or (at your option) any
// later version.
//
// GoCarlos.NET is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with GoCarlos.NET. If not, see <https://www.gnu.org/licenses/>.

using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GoCarlos.NET.Models;

public class Player()
{
    public Player(EGD_Data data, int numberOfRounds) : this(data)
    {
        for (int i = 0; i < numberOfRounds; i++)
            RoundsPlaying.Add(i);
    }

    public Player(EGD_Data data, HashSet<int> roundsPlaying) : this(data)
    {
        RoundsPlaying = roundsPlaying;
    }

    private Player(EGD_Data data) : this()
    {
        Data = data;
        Grade = Data.Grade;

        if (data.Gor == "")
        {
            data.Gor = Utils.GetRating(Grade).ToString();
            Rating = Utils.GetRating(Grade);
        }
        else
        {
            Rating = int.TryParse(data.Gor, out int parsedRating) ? parsedRating : Utils.GetRating(Grade);
        }
        StartScore = Utils.GetValue(Grade);

        Score = StartScore;
    }

    [JsonProperty] public Guid Guid { get; private set; } = Guid.NewGuid();
    [JsonProperty] public HashSet<int> RoundsPlaying { get; } = [];
    [JsonProperty] public Dictionary<int, Player> Opponents { get; } = [];
    [JsonProperty] public Dictionary<int, Pairing> Pairings { get; } = [];
    [JsonProperty] public Dictionary<int, sbyte> ColorBalancer { get; } = [];
    [JsonProperty] public Dictionary<int, int> PairingBalancer { get; } = [];
    
    public EGD_Data Data { get; set; } = new();
    [JsonIgnore] public string FullName { get => $"{Data.Last_Name} {Data.Name}"; }

    public Group Group { get; set; } = Group.Default;
    public string Grade
    {
        get;
        set
        {
            field = value;
            Data.Grade = value;
            Data.Grade_n = Utils.GetValue(value).ToString();
            StartScore = Utils.GetValue(value);
            if (Data.Gor == "")
            {
                Data.Gor = Utils.GetRating(value).ToString();
                Rating = Utils.GetRating(value);
            }
        }
    } = "30k";
    public int Rating { get; set; } = -900;

    public int ByeBalancer { get; set; } = 0;

    public bool SharedPlace { get; set; } = false;
    public int Place { get; set; } = 0;

    public int StartScore { get; set; } = 0;
    public decimal Points { get; set; } = 0M;
    public decimal Score { get; set; } = 0M;
    public decimal ScoreX { get; set; } = 0M;
    public decimal SOS { get; set; } = 0M;
    public decimal SOSOS { get; set; } = 0M;
    public decimal SODOS { get; set; } = 0M;

    private sbyte PlayedWhite(int roundNumber) => ColorBalancer.TryGetValue(roundNumber, out sbyte w) ? w : (sbyte)0;

    public sbyte CalculateColorBalancer(int roundNumber)
    {
        sbyte cb = 0;
        for (int i = 0; i <= roundNumber; i++)
        {
            if (PlayedWhite(i) == 1)
            {
                cb++;
            }
        }
        return cb;
    }

    private int GetPairingBalancer(int roundNumber) => PairingBalancer.GetValueOrDefault(roundNumber);

    public int CalculatePairingBalancer(int roundNumber)
    {
        int pb = 0;
        for (int i = 0; i <= roundNumber; i++)
        {
            pb += GetPairingBalancer(i);
        }
        return pb;
    }

    public override bool Equals(object? obj)
    {
        return obj is Player player && Guid.Equals(player.Guid);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Guid);
    }
    public override string? ToString()
    {
        return string.Format("Player {0}: {1} {2} {3}, Rating: {4}", Guid, Data.Name, Data.Last_Name, Grade, Rating);
    }
}