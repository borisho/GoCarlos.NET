using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Models;
using GoCarlos.NET.Models.Enums;
using System;
using System.Collections.Generic;

namespace GoCarlos.NET.ViewModels;

public partial class PlayerViewModel : ObservableObject, IEquatable<PlayerViewModel?>
{
    private readonly Player player;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Place), nameof(FullName), nameof(State), nameof(Club), nameof(Grade), nameof(Gor), nameof(GroupColor),
        nameof(R1), nameof(R2), nameof(R3), nameof(R4), nameof(R5), nameof(R6), nameof(R7), nameof(R8), nameof(R9), nameof(R10), nameof(Points),
        nameof(Score), nameof(ScoreX), nameof(SOS), nameof(SOSOS), nameof(SODOS), nameof(EGDPoints), nameof(EGDScore), nameof(EGDSOS),
        nameof(EGDSOSOS), nameof(EGDSODOS))]
    private int currentRound;

    public PlayerViewModel(Player player, int currentRound)
    {
        this.player = player;
        this.currentRound = currentRound;
    }

    public Player Player { get => player; }

    public int Place { get => player.Place; }
    public string FullName { get => player.Data.Last_Name + " " + player.Data.Name; }
    public string State { get => player.Data.Country_Code; }
    public string Club { get => player.Data.Club; }
    public string Grade { get => player.Grade; }
    public int Gor { get => player.Rating; }
    public string GroupColor { get => player.IsSuperGroup ? "2" : player.IsTopGroup ? "1" : ""; }
    public string R1 { get => GetRoundResult(0); }
    public string R2 { get => GetRoundResult(1); }
    public string R3 { get => GetRoundResult(2); }
    public string R4 { get => GetRoundResult(3); }
    public string R5 { get => GetRoundResult(4); }
    public string R6 { get => GetRoundResult(5); }
    public string R7 { get => GetRoundResult(6); }
    public string R8 { get => GetRoundResult(7); }
    public string R9 { get => GetRoundResult(8); }
    public string R10 { get => GetRoundResult(9); }
    public float Points { get => player.Points; }
    public float Score { get => player.Score; }
    public float ScoreX { get => player.ScoreX; }
    public float SOS { get => player.SOS; }
    public float SOSOS { get => player.SOSOS; }
    public float SODOS { get => player.SODOS; }

    public string EGDPoints { get => player.Points % 1 > 0 ? Math.Floor(player.Points).ToString() + Utils.EQUALS : Math.Floor(player.Points).ToString(); }
    public string EGDScore { get => player.Score % 1 > 0 ? Math.Floor(player.Score).ToString() + Utils.EQUALS : Math.Floor(player.Score).ToString(); }
    public string EGDScoreX { get => player.ScoreX % 1 > 0 ? Math.Floor(player.ScoreX).ToString() + Utils.EQUALS : Math.Floor(player.ScoreX).ToString(); }
    public string EGDSOS { get => player.SOS % 1 > 0 ? Math.Floor(player.SOS).ToString() + Utils.EQUALS : Math.Floor(player.SOS).ToString(); }
    public string EGDSOSOS { get => player.SOSOS % 1 > 0 ? Math.Floor(player.SOSOS).ToString() + Utils.EQUALS : Math.Floor(player.SOSOS).ToString(); }
    public string EGDSODOS { get => player.SODOS % 1 > 0 ? Math.Floor(player.SODOS).ToString() + Utils.EQUALS : Math.Floor(player.SODOS).ToString(); }

    private string GetRoundResult(int roundNumber)
    {
        if (player.RoundsPlaying.Contains(roundNumber))
        {
            if (player.Pairings.TryGetValue(roundNumber, out Pairing? pairing))
            {
                return GetPairingResult(pairing);
            }

            else
            {
                return Utils.QUESTION_MARK;
            }
        }

        else
        {
            return Utils.DASH;
        }
    }

    private string GetPairingResult(Pairing pairing)
    {
        switch (pairing.Result)
        {
            case Result.NONE:
                if (player == pairing.Black)
                {
                    return pairing.White.Place.ToString() + Utils.QUESTION_MARK + "/b" + pairing.Handicap;
                }
                else
                {
                    return pairing.Black.Place.ToString() + Utils.QUESTION_MARK + "/w" + pairing.Handicap;
                }
            case Result.BLACK_WON:
                if (player == pairing.Black)
                {
                    return pairing.White.Place.ToString() + Utils.PLUS + "/b" + pairing.Handicap;
                }
                else
                {
                    return pairing.Black.Place.ToString() + Utils.DASH + "/w" + pairing.Handicap;
                }
            case Result.WHITE_WON:
                if (player == pairing.Black)
                {
                    return pairing.White.Place.ToString() + Utils.DASH + "/b" + pairing.Handicap;
                }
                else
                {
                    return pairing.Black.Place.ToString() + Utils.PLUS + "/w" + pairing.Handicap;
                }
            case Result.DRAW:
                if (player == pairing.Black)
                {
                    return pairing.White.Place.ToString() + Utils.EQUALS + "/b" + pairing.Handicap;
                }
                else
                {
                    return pairing.Black.Place.ToString() + Utils.EQUALS + "/w" + pairing.Handicap;
                }
            case Result.BOTH_WON:
                if (player == pairing.Black)
                {
                    return pairing.White.Place.ToString() + Utils.PLUS + "/b" + pairing.Handicap;
                }
                else
                {
                    return pairing.Black.Place.ToString() + Utils.PLUS + "/w" + pairing.Handicap;
                }
            case Result.BOTH_LOST:
                if (player == pairing.Black)
                {
                    return pairing.White.Place.ToString() + Utils.DASH + "/b" + pairing.Handicap;
                }
                else
                {
                    return pairing.Black.Place.ToString() + Utils.DASH + "/w" + pairing.Handicap;
                }
            default:
                return Utils.QUESTION_MARK;
        };
    }

    public override bool Equals(object? obj)
    {
        return obj is PlayerViewModel pvm &&
            EqualityComparer<Player>.Default.Equals(player, pvm.player) &&
               CurrentRound == pvm.CurrentRound;
    }
    public bool Equals(PlayerViewModel? other)
    {
        return other is not null &&
               EqualityComparer<Player>.Default.Equals(player, other.player) &&
               CurrentRound == other.CurrentRound;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(player, CurrentRound);
    }
    public static bool operator ==(PlayerViewModel? left, PlayerViewModel? right)
    {
        return EqualityComparer<PlayerViewModel>.Default.Equals(left, right);
    }
    public static bool operator !=(PlayerViewModel? left, PlayerViewModel? right)
    {
        return !(left == right);
    }
}