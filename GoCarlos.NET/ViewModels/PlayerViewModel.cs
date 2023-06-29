using GoCarlos.NET.Interfaces;

namespace GoCarlos.NET.ViewModels;

public class PlayerViewModel : PlayerControlViewModel
{
    public PlayerViewModel() : base() { }

    public void SetSelectedPlayer(IPlayer player)
    {
        GenerateCheckBoxes(player.RoundsPlaying);

        Pin = player.Pin;
        LastName = player.LastName;
        FirstName = player.FirstName;
        Gor = player.Gor.ToString();
        Grade = player.Grade;
        CountryCode = player.CountryCode;
        Club = player.Club;
    }
}
