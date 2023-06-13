using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.Core.Interfaces;
using GoCarlos.NET.UI.Interfaces;
using GoCarlos.NET.UI.Models;

namespace GoCarlos.NET.UI.ViewModels;

public partial class AddPlayerViewModel : PlayerControlViewModel
{
    [ObservableProperty]
    private bool addOneMore;

    public AddPlayerViewModel(ITournament tournament) : base(tournament)
    {
        addOneMore = false;
    }

    [RelayCommand]
    public void Save(ICloseable closeable)
    {
        if(FirstName.Trim() == "" || LastName.Trim() == "")
        {
            DialogService.Show(Localizer["Temporary warning"], Localizer["warning"], Enums.MessageType.WARNING);
        }

        if (int.TryParse(Gor.Trim(), out int parsedGor))
        {
            Player player = new()
            {
                Pin = Pin,
                LastName = LastName,
                FirstName = FirstName,
                Gor = parsedGor,
                Grade = Grade,
                CountryCode = CountryCode,
                Club = Club,
            };

            WeakReferenceMessenger.Default.Send(player);
            closeable.Close();
        }
        
        else
        {
            DialogService.Show(Localizer["Gor musi byt cislo"], Localizer["Varovanie"], Enums.MessageType.WARNING);
        }
    }
}
