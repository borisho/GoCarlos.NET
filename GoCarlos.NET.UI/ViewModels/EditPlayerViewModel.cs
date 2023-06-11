using GoCarlos.NET.Core.Interfaces;

namespace GoCarlos.NET.UI.ViewModels;

public class EditPlayerViewModel : PlayerControlViewModel
{
    public EditPlayerViewModel(ITournament tournament) : base(tournament) { }
}
