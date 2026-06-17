using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Models;
using GoCarlos.NET.Models.Enums;
using System;
using System.Collections.Generic;

namespace GoCarlos.NET.ViewModels;

public partial class PlayerViewModel(Tournament tournament, Player player) : ObservableObject, IEquatable<PlayerViewModel?>
{
    private readonly Tournament tournament = tournament;
    private readonly Player player = player;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Place), nameof(FullName), nameof(State), nameof(Club), nameof(Grade), nameof(Gor), nameof(GroupColor),
        nameof(R1), nameof(R2), nameof(R3), nameof(R4), nameof(R5), nameof(R6), nameof(R7), nameof(R8), nameof(R9), nameof(R10), nameof(Points),
        nameof(Score), nameof(ScoreX), nameof(SOS), nameof(SOSOS), nameof(SODOS), nameof(EGDPoints), nameof(EGDScore), nameof(EGDSOS),
        nameof(EGDSOSOS), nameof(EGDSODOS))]
    public partial int CurrentRound { get; set; } = tournament.CurrentRound;

    public Player Player { get => player; }

    public int Place { get => player.Place; }
    public string FullName { get => player.FullName; }
    public string State { get => player.Data.Country_Code; }
    public string Club { get => player.Data.Club; }
    public string Grade { get => player.Grade; }
    public int Gor { get => player.Rating; }
    public Group GroupColor
    {
        get => player.Group;
        set
        {
            player.Group = value;
            OnPropertyChanged(nameof(GroupColor));
        }
    }
    public bool SharedPlace { get => player.SharedPlace; }
    public string R1 => GetRoundResult(0);
    public string R2 => GetRoundResult(1);
    public string R3 => GetRoundResult(2);
    public string R4 => GetRoundResult(3);
    public string R5 => GetRoundResult(4);
    public string R6 => GetRoundResult(5);
    public string R7 => GetRoundResult(6);
    public string R8 => GetRoundResult(7);
    public string R9 => GetRoundResult(8);
    public string R10 => GetRoundResult(9);
    public int NrW
    {
        get
        {
            int roundNumber = CurrentRound;

            if (!tournament.CountCurrentRound) roundNumber--;

            int nrw = 0;

            for (int i = 0; i <= roundNumber; i++)
            {
                if (player.PlayedWhite(i))
                {
                    nrw++;
                }
            }

            return nrw;
        }
    }
    public int PairingBalancer
    {
        get
        {
            int roundNumber = CurrentRound;

            if (!tournament.CountCurrentRound) roundNumber--;

            int pb = 0;

            for (int i = 0; i <= roundNumber; i++)
            {
                pb += player.GetPairingPalancer(i);
            }

            return pb;
        }
    }

    public decimal Points { get => player.Points; }
    public decimal Score { get => player.Score; }
    public decimal ScoreX { get => player.ScoreX; }
    public decimal SOS { get => player.SOS; }
    public decimal SOSOS { get => player.SOSOS; }
    public decimal SODOS { get => player.SODOS; }

    public string EGDPoints { get => ToEGD(player.Points); }
    public string EGDScore { get => ToEGD(player.Score); }
    public string EGDScoreX { get => ToEGD(player.ScoreX); }
    public string EGDSOS { get => ToEGD(player.SOS); }
    public string EGDSOSOS { get => ToEGD(player.SOSOS); }
    public string EGDSODOS { get => ToEGD(player.SODOS); }

    private static string ToEGD(decimal value) => value % 1 > 0
        ? Math.Floor(value).ToString() + Utils.EQUALS
        : Math.Floor(value).ToString();

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
        var isBlack = player == pairing.Black;
        var opponent = isBlack ? pairing.White : pairing.Black;
        var color = isBlack ? "b" : "w";

        var result = pairing.Result switch
        {
            Result.NONE => Utils.QUESTION_MARK,
            Result.BLACK_WON => isBlack ? Utils.PLUS : Utils.DASH,
            Result.WHITE_WON => isBlack ? Utils.DASH : Utils.PLUS,
            Result.DRAW => Utils.EQUALS,
            Result.BOTH_WON => Utils.PLUS,
            Result.BOTH_LOST => Utils.DASH,
            _ => Utils.QUESTION_MARK
        };

        return pairing.Handicap == 0
            ? $"{opponent.Place}{result}/{color}"
            : $"{opponent.Place}{result}/{color}{pairing.Handicap}";
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