using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Interfaces;
using System.ComponentModel;
using System.Windows.Data;

namespace GoCarlos.NET.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private ITournament tournament;

    [ObservableProperty]
    private MenuViewModel menuViewModel;

    public MainViewModel(ITournament tournament, MenuViewModel menuViewModel)
    {
        this.tournament = tournament;
        this.menuViewModel = menuViewModel;

        Players = CollectionViewSource.GetDefaultView(tournament.Players);
    }

    public ICollectionView Players { get; }
}
