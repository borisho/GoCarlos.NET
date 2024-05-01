using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace GoCarlos.NET.Models;

public partial class GeneralSettings : ObservableObject
{
    public const int DEFAULT_NUMBER_OF_ROUNDS = 5;
    const int DEFAULT_TOP_GROUP_BAR = 29;
    const int DEFAULT_BOTTOM_GROUP_BAR = 10;

    [ObservableProperty]
    private string
        shortName = "Turnaj",
        name = "Turnaj",
        location = "SK, ";

    [ObservableProperty]
    private DateTime
        startDate = DateTime.Now,
        endDate = DateTime.Now;

    [ObservableProperty]
    private int
        numberOfRounds = DEFAULT_NUMBER_OF_ROUNDS,
        topGroupBar = DEFAULT_TOP_GROUP_BAR,
        bottomGroupBar  = DEFAULT_BOTTOM_GROUP_BAR;
}
