using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace GoCarlos.NET.Models;

public partial class GeneralSettings : ObservableObject
{
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
        numberOfRounds = 5,
        topGroupBar = 29,
        bottomGroupBar  = 10;
}
