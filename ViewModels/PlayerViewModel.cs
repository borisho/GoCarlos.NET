using CommunityToolkit.Mvvm.ComponentModel;

namespace GoCarlos.NET.ViewModels;

public partial class PlayerViewModel : ObservableObject
{
    [ObservableProperty]
    private string pin;

    public PlayerViewModel()
    {
        pin = string.Empty;
    }
}
