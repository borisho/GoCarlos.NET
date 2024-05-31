﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.Enums;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Messages;
using GoCarlos.NET.Models;
using GoCarlos.NET.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GoCarlos.NET.ViewModels;

public partial class PlayerControlViewModel : ObservableRecipient, IRecipient<EgdMessage>, IReferenceCleanup
{
    protected readonly IStringLocalizer _localizer;
    protected readonly IWindowService _windowService;
    protected readonly IDialogService _dialogService;
    protected readonly IEgdService _egdService;

    [ObservableProperty]
    private string pin, lastName, firstName, gor, gradeR, countryCode, club;

    private string grade;

    public PlayerControlViewModel()
    {
        WeakReferenceMessenger.Default.Register(this);

        IServiceProvider serviceProvider = App.AppHost!.Services;

        _localizer = serviceProvider.GetRequiredService<IStringLocalizerFactory>().Create("PlayerControl", "GoCarlos.NET");
        _windowService = serviceProvider.GetRequiredService<IWindowService>();
        _dialogService = serviceProvider.GetRequiredService<IDialogService>();
        _egdService = serviceProvider.GetRequiredService<IEgdService>();

        pin = string.Empty;
        lastName = string.Empty;
        firstName = string.Empty;
        gor = string.Empty;
        grade = string.Empty;
        gradeR = string.Empty;
        countryCode = string.Empty;
        club = string.Empty;

        GradeN = 0;
        GradeNR = 0;
    }

    protected int GradeN, GradeNR;

    protected IStringLocalizer Localizer { get => _localizer; }
    protected IWindowService WindowService { get => _windowService; }
    protected IDialogService DialogService { get => _dialogService; }
    protected IEgdService EgdService { get => _egdService; }

    public ObservableCollection<CheckBoxViewModel> CheckBoxes { get; } = [];

    public string Grade
    {
        get => grade;
        set
        {
            grade = value != null ? value.ToLower() : string.Empty;

            OnPropertyChanged(nameof(Grade));
        }
    }

    [RelayCommand]
    public void SearchbyPin()
    {
        if (string.IsNullOrWhiteSpace(Pin) || !int.TryParse(Pin.Trim(), out _))
        {
            _dialogService.Show(_localizer["PinContainsOnlyNumbers"], _localizer["Error"], Enums.MessageType.ERROR);
            return;
        }

        EgdData? data = _egdService.SearchByPin(Pin);

        if (data?.Retcode == "Ok")
        {
            SetData(data);
        }
        else
        {
            _dialogService.Show(_localizer["PlayerNotFound"], _localizer["Error"], Enums.MessageType.ERROR);
        }
    }

    [RelayCommand]
    public void SearchByData()
    {
        EgdDataList? list = _egdService.SearchByData(LastName, FirstName);

        if (list?.Retcode == "Ok" && list.Players.Length > 0)
        {
            if (list.Players.Length == 1)
            {
                SetData(list.Players[0]);
            }

            else
            {
                _windowService.ShowEgdSelectionWindow(list.Players);
            }
        }
        else
        {
            _dialogService.Show(_localizer["PlayerNotFound"], _localizer["Error"], Enums.MessageType.ERROR);
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

        if (!string.IsNullOrEmpty(Gor))
        {
            if (int.TryParse(data.Gor, out int rating))
            {
                GradeR = GradeUtils.GetGradeFromRating(rating);
                GradeNR = GradeUtils.GetGradeNFromRating(rating);
            }
            else
            {
                DialogService.Show(_localizer["RatingParseErrorMessage"], _localizer["Error"], MessageType.WARNING);
            }
        }

        if (int.TryParse(data.Grade_n, out int grade_n))
        {
            GradeN = grade_n;
        }
        else
        {
            DialogService.Show(_localizer["GradeNParseErrorMessage"], _localizer["Error"], MessageType.WARNING);
        }
    }

    public void GenerateCheckBoxes(int rounds)
    {
        for (int i = 1; i <= rounds; i++)
        {
            CheckBoxes.Add(new(true, _localizer["Round"], i));
        }
    }

    public void GenerateCheckBoxes(List<bool> rounds)
    {
        int i = 1;

        foreach (bool b in rounds)
        {
            CheckBoxes.Add(new(b, _localizer["Round"], i++));
        }
    }

    public void Receive(EgdMessage message)
    {
        SetData(message.Value);
    }

    public void Unregister()
    {
        WeakReferenceMessenger.Default.Unregister<EgdMessage>(this);
    }
}
