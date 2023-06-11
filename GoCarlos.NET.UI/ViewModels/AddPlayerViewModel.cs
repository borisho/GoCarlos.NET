using GoCarlos.NET.Core.Interfaces;

namespace GoCarlos.NET.UI.ViewModels;

public partial class AddPlayerViewModel : PlayerControlViewModel
{
    public AddPlayerViewModel(ITournament tournament) : base(tournament) { }
}
