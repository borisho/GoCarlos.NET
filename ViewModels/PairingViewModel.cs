using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;
using GoCarlos.NET.Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GoCarlos.NET.ViewModels;

public partial class PairingViewModel : ObservableObject, IEquatable<PairingViewModel?>
{
    private const string BLACK_WON = "1-0";
    private const string WHITE_WON = "0-1";
    private const string BOTH_WON = "1-1";
    private const string BOTH_LOST = "0-0";
    private const string DRAW = "½-½";
    private const string NONE = "?-?";
    private const string MARK = "(!)";

    private readonly Pairing pairing;
    private string selectedResult;

    [ObservableProperty]
    private bool isRefereeEnabled;
    public PairingViewModel(Pairing pairing)
    {
        this.pairing = pairing;

        ResultList =
        [
            BLACK_WON,
            WHITE_WON,
            BOTH_WON,
            BOTH_LOST,
            DRAW,
            NONE,
        ];

        switch (pairing.Result)
        {
            case Result.BLACK_WON:
                isRefereeEnabled = true;
                selectedResult = BLACK_WON;
                break;
            case Result.WHITE_WON:
                isRefereeEnabled = true;
                selectedResult = WHITE_WON;
                break;
            case Result.BOTH_WON:
                isRefereeEnabled = false;
                selectedResult = BOTH_WON;
                break;
            case Result.BOTH_LOST:
                isRefereeEnabled = false;
                selectedResult = BOTH_LOST;
                break;
            case Result.DRAW:
                isRefereeEnabled = true;
                selectedResult = DRAW;
                break;
            default:
                isRefereeEnabled = true;
                selectedResult = NONE;
                break;
        }
    }
    public event EventHandler? DeleteSelectedPlayerEvent;
    public ObservableCollection<string> ResultList { get; private set; }
    public string SelectedResult
    {
        get => selectedResult;
        set
        {
            selectedResult = value;
            switch (value)
            {
                case BLACK_WON:
                    pairing.Result = Result.BLACK_WON;
                    IsRefereeEnabled = true;
                    break;
                case WHITE_WON:
                    pairing.Result = Result.WHITE_WON;
                    IsRefereeEnabled = true;
                    break;
                case BOTH_WON:
                    pairing.Result = Result.BOTH_WON;
                    IsRefereeEnabled = false;
                    RefereeDecision = true;
                    break;
                case BOTH_LOST:
                    pairing.Result = Result.BOTH_LOST;
                    IsRefereeEnabled = false;
                    RefereeDecision = true;
                    break;
                case DRAW:
                    pairing.Result = Result.DRAW;
                    IsRefereeEnabled = true;
                    break;
                case NONE:
                    pairing.Result = Result.NONE;
                    IsRefereeEnabled = true;
                    break;
                default: break;
            }

            OnPropertyChanged(nameof(SelectedResult));
            OnPropertyChanged(nameof(IsRefereeEnabled));
            OnPropertyChanged(nameof(Results));
        }
    }
    public void SetResult(Result result)
    {
        IsRefereeEnabled = true;
        RefereeDecision = false;
        switch (result)
        {
            case Result.BLACK_WON:
                pairing.Result = Result.BLACK_WON;
                SelectedResult = BLACK_WON;
                break;
            case Result.WHITE_WON:
                pairing.Result = Result.WHITE_WON;
                SelectedResult = WHITE_WON;
                break;
            case Result.BOTH_WON:
                pairing.Result = Result.BOTH_WON;
                SelectedResult = BOTH_WON;
                break;
            case Result.BOTH_LOST:
                pairing.Result = Result.BOTH_LOST;
                SelectedResult = BOTH_LOST;
                break;
            case Result.DRAW:
                pairing.Result = Result.DRAW;
                SelectedResult = DRAW;
                break;
            case Result.NONE:
                pairing.Result = Result.NONE;
                SelectedResult = NONE;
                break;
            default: break;
        }
    }
    public Pairing Pairing { get => pairing; }
    public int Board { get => pairing.BoardNumber; }
    public string Black
    {
        get
        {
            if (pairing.Black.IsBye)
            {
                return "BYE";
            }

            return pairing.Black.Data.Last_Name + ", " + pairing.Black.Data.Name + " (" + pairing.Black.Data.Grade + ")";
        }
    }
    public string White
    {
        get
        {
            if (pairing.White.IsBye)
            {
                return "BYE";
            }

            return pairing.White.Data.Last_Name + ", " + pairing.White.Data.Name + " (" + pairing.White.Data.Grade + ")";
        }
    }
    public string Results
    {
        get
        {
            return pairing.Result switch
            {
                Result.NONE => NONE,
                Result.DRAW => pairing.ResultByReferee ? DRAW + MARK : DRAW,
                Result.BOTH_LOST => BOTH_LOST + MARK,
                Result.BOTH_WON => BOTH_WON + MARK,
                Result.BLACK_WON => pairing.ResultByReferee ? BLACK_WON + MARK : BLACK_WON,
                Result.WHITE_WON => pairing.ResultByReferee ? WHITE_WON + MARK : WHITE_WON,
                _ => NONE,
            };
        }
    }
    public int Handicap
    {
        get => pairing.Handicap;
        set
        {
            pairing.Handicap = value;
            OnPropertyChanged(nameof(Handicap));
        }
    }
    public bool RefereeDecision
    {
        get => pairing.ResultByReferee;
        set
        {
            pairing.ResultByReferee = value;
            OnPropertyChanged(nameof(RefereeDecision));
            OnPropertyChanged(nameof(Results));
        }
    }

    [RelayCommand]
    private void SwapColors()
    {
        Player black = pairing.Black;
        Player white = pairing.White;

        white.ColorBalancer--;
        black.ColorBalancer++;

        pairing.Black = white;
        pairing.White = black;

        if (pairing.Result.Equals(Result.BLACK_WON))
        {
            pairing.Result = Result.WHITE_WON;
        }
        else if (pairing.Result.Equals(Result.WHITE_WON))
        {
            pairing.Result = Result.BLACK_WON;
        }

        OnPropertyChanged(nameof(Board));
        OnPropertyChanged(nameof(Black));
        OnPropertyChanged(nameof(White));
        OnPropertyChanged(nameof(Results));
        OnPropertyChanged(nameof(Handicap));
    }

    [RelayCommand]
    private void DeletePairing(ICloseable? window)
    {
        if (window is not null)
        {
            DeleteSelectedPlayerEvent?.Invoke(this, EventArgs.Empty);
            window?.Close();
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is PairingViewModel pvm &&
            EqualityComparer<Pairing>.Default.Equals(pairing, pvm.pairing);
    }
    public bool Equals(PairingViewModel? other)
    {
        return other is not null &&
            EqualityComparer<Pairing>.Default.Equals(pairing, other.pairing);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public static bool operator ==(PairingViewModel? left, PairingViewModel? right)
    {
        return EqualityComparer<PairingViewModel>.Default.Equals(left, right);
    }
    public static bool operator !=(PairingViewModel? left, PairingViewModel? right)
    {
        return !(left == right);
    }
}
