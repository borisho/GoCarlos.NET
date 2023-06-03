using CommunityToolkit.Mvvm.ComponentModel;

namespace GoCarlos.NET.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private MenuViewModel menuViewModel;

    public MainViewModel() {
    }
}
