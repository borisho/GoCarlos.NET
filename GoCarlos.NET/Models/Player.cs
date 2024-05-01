using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace GoCarlos.NET.Models;

public partial class Player : ObservableObject
{
    const int MIN_RATING = -900;

    [ObservableProperty]
    private Guid guid;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullName))]
    private string lastName, firstName;

    [ObservableProperty]
    private string pin, grade, gradeR, countryCode, club;

    private int gor;

    [ObservableProperty]
    private List<bool> roundsPlaying;

    public Player()
    {
        Guid = Guid.NewGuid();
        Pin = string.Empty;
        LastName = string.Empty;
        FirstName = string.Empty;
        Gor = MIN_RATING;
        Grade = string.Empty;
        GradeR = string.Empty;
        CountryCode = string.Empty;
        Club = string.Empty;

        RoundsPlaying = [];
    }

    public int Gor
    {
        get => gor;
        set
        {
            if (value < MIN_RATING)
            {
                gor = MIN_RATING;
            }

            else
            {
                gor = value;
            }

            OnPropertyChanged(nameof(Gor));
        }
    }

    public string FullName
    {
        get => LastName + " " + FirstName;
    }

    public override bool Equals(object? obj) => obj is Player player && Guid.Equals(player.Guid);

    public override int GetHashCode() => HashCode.Combine(Guid);
}
