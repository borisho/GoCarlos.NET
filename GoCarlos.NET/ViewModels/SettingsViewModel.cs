using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;

namespace GoCarlos.NET.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ITournament tournament;

    [ObservableProperty]
    private GeneralSettings generalSettings;

    public SettingsViewModel(ITournament tournament)
    {
        this.tournament = tournament;
        generalSettings = tournament.Settings.GeneralSettings;
    }
}
