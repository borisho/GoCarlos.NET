using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.UI.Interfaces;
using GoCarlos.NET.UI.Messages;
using GoCarlos.NET.UI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.ObjectModel;

namespace GoCarlos.NET.UI.ViewModels;

public partial class PlayerControlViewModel : ObservableRecipient, IRecipient<EgdMessage>
{
    private readonly IStringLocalizer localizer;
    private readonly IWindowService windowService;
    private readonly IDialogService dialogService;
    private readonly IEgdService egdService;

    [ObservableProperty]
    private string pin, lastName, firstName, gor, grade, countryCode, club;

    public PlayerControlViewModel(ITournamentService tournamentService)
    {
        WeakReferenceMessenger.Default.Register(this);

        IServiceProvider serviceProvider = App.AppHost!.Services;
       
        localizer = serviceProvider.GetRequiredService<IStringLocalizerFactory>().Create("PlayerControl", "GoCarlos.NET.UI");
        windowService = serviceProvider.GetRequiredService<IWindowService>();
        dialogService = serviceProvider.GetRequiredService<IDialogService>();
        egdService = serviceProvider.GetRequiredService<IEgdService>();

        Pin = string.Empty;
        LastName = string.Empty;
        FirstName = string.Empty;
        Gor = string.Empty;
        Grade = string.Empty;
        CountryCode = string.Empty;
        Club = string.Empty;

        for (int i = 0; i < tournamentService.Tournament.Rounds; i++)
        {
            CheckBoxes.Add(new(true, localizer["Round"], i));
        }
    }

    protected IStringLocalizer Localizer { get => localizer; }
    protected IWindowService WindowService { get => windowService; }
    protected IDialogService DialogService { get => dialogService; }
    protected IEgdService EgdService { get => egdService; }

    public ObservableCollection<CheckBoxViewModel> CheckBoxes { get; } = new();

    [RelayCommand]
    public void SearchbyPin()
    {
        if (string.IsNullOrWhiteSpace(Pin) || !int.TryParse(Pin.Trim(), out _))
        {
            dialogService.Show(localizer["PinContainsOnlyNumbers"], localizer["Error"], Enums.MessageType.ERROR);
            return;
        }

        EgdData? data = egdService.SearchByPin(Pin);

        if (data?.Retcode == "Ok")
        {
            SetData(data);
        }
        else
        {
            dialogService.Show(localizer["PlayerNotFound"], localizer["Error"], Enums.MessageType.ERROR);
        }
    }

    [RelayCommand]
    public void SearchByData()
    {
        EgdDataList? list = egdService.SearchByData(LastName, FirstName);

        if (list?.Retcode == "Ok" && list.Players.Length > 0)
        {
            if (list.Players.Length == 1)
            {
                SetData(list.Players[0]);
            }

            else
            {
                windowService.ShowEgdSelectionWindow(list.Players);
            }
        }
        else
        {
            dialogService.Show(localizer["PlayerNotFound"], localizer["Error"], Enums.MessageType.ERROR);
        }
    }

    private void SetData(EgdData data)
    {
        Pin = data.Pin_Player;
        LastName = data.Last_Name;
        FirstName = data.Name;
        Gor = data.Gor;
        Grade = data.Grade;
        CountryCode = data.Country_Code;
        Club = data.Club;
    }

    public void Receive(EgdMessage message)
    {
        SetData(message.Value);
    }
}
