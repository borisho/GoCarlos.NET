using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;

namespace GoCarlos.NET.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ITournament tournament;

    [ObservableProperty]
    private GeneralSettings generalSettings;

    [ObservableProperty]
    private HandicapSettings handicapSettings;

    public SettingsViewModel(ITournament tournament)
    {
        this.tournament = tournament;
        GeneralSettings = tournament.Settings.GeneralSettings;
        HandicapSettings = tournament.Settings.HandicapSettings;
    }
}
