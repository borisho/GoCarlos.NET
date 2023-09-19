using CommunityToolkit.Mvvm.ComponentModel;

namespace GoCarlos.NET.Models;

public partial class HandicapSettings : ObservableObject
{
    [ObservableProperty]
    private bool basedOnMM = false;

    [ObservableProperty]
    private int
        ceiling = 9,
        reduction = 2;
}
