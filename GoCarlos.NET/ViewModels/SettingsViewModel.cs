using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Interfaces;

namespace GoCarlos.NET.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ITournament tournament;

    public SettingsViewModel(ITournament tournament)
    {
        this.tournament = tournament;
    }
}
