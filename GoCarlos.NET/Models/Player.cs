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
    private string pin, grade, countryCode, club;

    [ObservableProperty]
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
        CountryCode = string.Empty;
        Club = string.Empty;

        RoundsPlaying = new List<bool>();
    }

    public string FullName
    {
        get => LastName + " " + FirstName;
    }
}
