using CommunityToolkit.Mvvm.ComponentModel;

namespace GoCarlos.NET.Models;

public partial class PairingSettings : ObservableObject
{
    const int SIZE = 3;

    private static readonly PairingMethod[] allMethods = new PairingMethod[SIZE]
{
        new PairingMethod("M1"),
        new PairingMethod("M2"),
        new PairingMethod("M3"),
};

    [ObservableProperty]
    private bool sameCityPairing = true;

    public PairingMethod[] AllMethods { get => allMethods; }
}
