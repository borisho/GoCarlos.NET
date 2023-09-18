using CommunityToolkit.Mvvm.Input;
using GoCarlos.NET.Enums;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;
using System.Collections.Generic;

namespace GoCarlos.NET.ViewModels;

public partial class PlayerViewModel : PlayerControlViewModel
{
    public PlayerViewModel() : base() { }

    public Player? Player { get; set; }

    public void SetSelectedPlayer(Player player)
    {
        Player = player;

        GenerateCheckBoxes(new List<bool>(player.RoundsPlaying));

        Pin = new string(player.Pin);
        LastName = new string(player.LastName);
        FirstName = new string(player.FirstName);
        Gor = player.Gor.ToString();
        Grade = new string(player.Grade);
        CountryCode = new string(player.CountryCode);
        Club = new string(player.Club);
    }

    [RelayCommand]
    public void Save(ICloseable window)
    {
        if (Player is not null)
        {
            if (FirstName.Trim() == "" || LastName.Trim() == "")
            {
                DialogService.Show(Localizer["NameErrorMessage"], Localizer["Error"], MessageType.ERROR);
            }

            else
            {
                if (int.TryParse(Gor, out int gor))
                {
                    Player.Pin = Pin;
                    Player.LastName = LastName;
                    Player.FirstName = FirstName;
                    Player.Gor = gor;
                    Player.Grade = Grade;
                    Player.CountryCode = CountryCode;
                    Player.Club = Club;

                    window.Close();
                }

                else
                {
                    DialogService.Show(Localizer["RatingErrorMessage"], Localizer["Error"], MessageType.ERROR);
                }
            }
        }

        else
        {
            window.Close();
        }
    }

    [RelayCommand]
    public void Cancel(ICloseable window)
    {
        window.Close();
    }
}
