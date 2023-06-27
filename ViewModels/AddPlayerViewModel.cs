using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Messages;

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
            DialogService.Show(Localizer["Temporary warning"], Localizer["warning"], Enums.MessageType.WARNING);
        }

        if (int.TryParse(Gor.Trim(), out int parsedGor))
        {
            WeakReferenceMessenger.Default.Send(new AddPlayerMessage()
            {
                Pin = Pin,
                LastName = LastName,
                FirstName = FirstName,
                Gor = parsedGor,
                Grade = Grade,
                CountryCode = CountryCode,
                Club = Club,
            });

            if (AddOneMore)
            {
                _windowService.ShowAddPlayerWindowWithParam(AddOneMore);
            }

            closeable.Close();
        }

        else
        {
            DialogService.Show(Localizer["Gor musi byt cislo"], Localizer["Varovanie"], Enums.MessageType.WARNING);
        }
    }
}
