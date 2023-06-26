using GoCarlos.NET.UI.Interfaces;

namespace GoCarlos.NET.UI.ViewModels;

public class EditPlayerViewModel : PlayerControlViewModel
{
    public EditPlayerViewModel(ITournamentService tournamentService) : base(tournamentService) { }
}
