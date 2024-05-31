using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Messages;
using GoCarlos.NET.Models;
using System.Collections.Generic;

namespace GoCarlos.NET.ViewModels;

public partial class AddPlayerViewModel : PlayerControlViewModel
{
    [ObservableProperty]
    private bool addOneMore;

    public AddPlayerViewModel() : base()
    {
        addOneMore = false;
    }

    [RelayCommand]
    public void Save(ICloseable closeable)
    {
        if (FirstName.Trim() == "" || LastName.Trim() == "")
        {
            DialogService.Show(Localizer["NameErrorMessage"], Localizer["Error"], Enums.MessageType.ERROR);
        }

        else
        {
            if (int.TryParse(Gor.Trim(), out int parsedGor))
            {
                List<bool> roundsPlaying = [];

                foreach (CheckBoxViewModel checkBox in CheckBoxes)
                {
                    roundsPlaying.Add(checkBox.Checked);
                }

                Player player = new()
                {
                    Pin = Pin,
                    LastName = LastName,
                    FirstName = FirstName,
                    Gor = parsedGor,
                    Grade = Grade,
                    GradeR = GradeR,
                    GradeN = GradeN,
                    GradeNR = GradeNR,
                    CountryCode = CountryCode,
                    Club = Club,
                    RoundsPlaying = roundsPlaying
                };

                WeakReferenceMessenger.Default.Send(new AddPlayerMessage(player));

                if (AddOneMore)
                {
                    _windowService.ShowAddPlayerWindowWithParam(AddOneMore);
                }

                closeable.Close();
            }

            else
            {
                DialogService.Show(Localizer["RatingErrorMessage"], Localizer["Error"], Enums.MessageType.ERROR);
            }
        }
    }
}
