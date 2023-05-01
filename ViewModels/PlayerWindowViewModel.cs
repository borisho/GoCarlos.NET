using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.Events;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Messages;
using GoCarlos.NET.Models;
using GoCarlos.NET.Services;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Windows;

namespace GoCarlos.NET.ViewModels;

public partial class PlayerWindowViewModel : ObservableRecipient, IRecipient<EGDSelectionMessage>
{
    private readonly PlayerViewModel? pvm;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Pin_Player), nameof(Name), nameof(LastName), nameof(Gor), nameof(Grade), nameof(Country_Code), nameof(Club))]
    private EGD_Data data;

    [ObservableProperty]
    private int numberOfRounds;

    [ObservableProperty]
    private bool isSuperGroup, isTopGroup, addOneMore;

    [ObservableProperty]
    private bool ch1, ch2, ch3, ch4, ch5, ch6, ch7, ch8, ch9, ch10;

    public PlayerWindowViewModel(PlayerViewModel? pvm, int numberOfRounds, bool addOneMore)
    {
        this.addOneMore = addOneMore;

        this.pvm = pvm;
        data = new();
        this.numberOfRounds = numberOfRounds;

        isSuperGroup = false;
        isTopGroup = false;
        ch1 = true;
        ch2 = true;
        ch3 = true;
        ch4 = true;
        ch5 = true;
        ch6 = true;
        ch7 = true;
        ch8 = true;
        ch9 = true;
        ch10 = true;

        if (pvm is not null)
        {
            data = new(pvm.Player.Data);
            isSuperGroup = pvm.Player.IsSuperGroup;
            isTopGroup = pvm.Player.IsTopGroup;

            if (!pvm.Player.RoundsPlaying.Contains(1))
                ch1 = false;
            if (!pvm.Player.RoundsPlaying.Contains(2))
                ch2 = false;
            if (!pvm.Player.RoundsPlaying.Contains(3))
                ch3 = false;
            if (!pvm.Player.RoundsPlaying.Contains(4))
                ch4 = false;
            if (!pvm.Player.RoundsPlaying.Contains(5))
                ch5 = false;
            if (!pvm.Player.RoundsPlaying.Contains(6))
                ch6 = false;
            if (!pvm.Player.RoundsPlaying.Contains(7))
                ch7 = false;
            if (!pvm.Player.RoundsPlaying.Contains(8))
                ch8 = false;
            if (!pvm.Player.RoundsPlaying.Contains(9))
                ch9 = false;
            if (!pvm.Player.RoundsPlaying.Contains(10))
                ch10 = false;
        }
    }

    public event EventHandler<PlayerEventArgs>? AddPlayerEvent;
    public event EventHandler<PlayerEventArgs>? EditPlayerEvent;

    #region Visibility properties

    public bool CHK2_Visibility { get => NumberOfRounds > 1; }
    public bool CHK3_Visibility { get => NumberOfRounds > 2; }
    public bool CHK4_Visibility { get => NumberOfRounds > 3; }
    public bool CHK5_Visibility { get => NumberOfRounds > 4; }
    public bool CHK6_Visibility { get => NumberOfRounds > 5; }
    public bool CHK7_Visibility { get => NumberOfRounds > 6; }
    public bool CHK8_Visibility { get => NumberOfRounds > 7; }
    public bool CHK9_Visibility { get => NumberOfRounds > 8; }
    public bool CHK10_Visibility { get => NumberOfRounds > 9; }
    public bool AddPlayerVisibility { get => pvm is null; }
    public bool EditPlayerVisibility { get => pvm is not null; }

    #endregion

    #region Player properties and linking for UI
    public string Pin_Player
    {
        get => Data.Pin_Player;
        set
        {
            Data.Pin_Player = value;
            OnPropertyChanged(nameof(Pin_Player));
        }
    }
    public string Name
    {
        get => Data.Name;
        set
        {
            Data.Name = value;
            OnPropertyChanged(nameof(Name));
        }
    }
    public string LastName
    {
        get => Data.Last_Name;
        set
        {
            Data.Last_Name = value;
            OnPropertyChanged(nameof(LastName));
        }
    }
    public string Gor
    {
        get => Data.Gor;
        set
        {
            Data.Gor = value;
            OnPropertyChanged(nameof(Gor));
        }
    }
    public string Grade
    {
        get => Data.Grade;
        set
        {
            Data.Grade = value;
            OnPropertyChanged(nameof(Grade));
        }
    }
    public string Country_Code
    {
        get => Data.Country_Code;
        set
        {
            Data.Country_Code = value;
            OnPropertyChanged(nameof(Country_Code));
        }
    }
    public string Club
    {
        get => Data.Club;
        set
        {
            Data.Club = value;
            OnPropertyChanged(nameof(Club));
        }
    }

    #endregion

    [RelayCommand]
    private void SearchByPin()
    {
        try
        {
            string json = EGDService.SearchByPin(Data.Pin_Player.Trim());
            EGD_Data? data = JsonConvert.DeserializeObject<EGD_Data>(json);

            if (data is not null && data.Retcode == "Ok")
                Data = data;

            else if (data is null)
                MessageBox.Show("Chyba odpovedi z EGD", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);

            else
                MessageBox.Show("EGD retcode: " + data?.Retcode, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        catch (AggregateException e)
        {
            MessageBox.Show(e.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void SearchByData()
    {
        try
        {
            string json = EGDService.SearchByData(Data.Last_Name.Trim(), Data.Name.Trim());
            EGD_DataList? data_list = JsonConvert.DeserializeObject<EGD_DataList>(json);

            if (data_list is null)
            {
                MessageBox.Show("Chyba odpovedi z EGD", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (data_list.Retcode == "Ok")
            {
                if (data_list.Players is null || data_list.Players.Length == 0)
                {
                    MessageBox.Show("Hráč nebol nájdený", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                else if (data_list.Players.Length > 1)
                {
                    IsActive = true;

                    EGDSelectionWindow listViewWindow = new()
                    {
                        DataContext = new EGDSelectionViewModel(data_list.Players)
                    };
                    listViewWindow.ShowDialog();

                    IsActive = false;
                }

                else
                {
                    Data = data_list.Players.First();
                }
            }

            else
            {
                MessageBox.Show("EGD retcode: " + data_list.Retcode, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        catch (AggregateException e)
        {
            MessageBox.Show(e.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void EditPlayer(ICloseable window)
    {
        if (pvm is not null)
        {
            Player player = pvm.Player;

            player.Data = Data;
            player.Rating = int.Parse(Data.Gor);
            player.Grade = Data.Grade;

            if (player.IsSuperGroup != IsSuperGroup || player.IsTopGroup != IsTopGroup)
            {
                player.IsSuperGroup = IsSuperGroup;
                player.IsTopGroup = IsTopGroup;
            }

            SetPlayingRounds(player);
            EditPlayerEvent?.Invoke(this, new PlayerEventArgs(player));
        }

        window?.Close();
    }

    [RelayCommand]
    private void AddPlayer(ICloseable window)
    {
        if (!string.IsNullOrWhiteSpace(Data.Name) && !string.IsNullOrWhiteSpace(Data.Last_Name))
        {
            Player player = new(Data)
            {
                IsSuperGroup = IsSuperGroup,
                IsTopGroup = IsTopGroup
            };

            SetPlayingRounds(player);

            AddPlayerEvent?.Invoke(this, new PlayerEventArgs(player)
            {
                Bool = AddOneMore,
            });

            window?.Close();
        }

        else
        {
            MessageBox.Show("Chýbajú údaje o hráčovi! \n (Meno alebo priezvisko)", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void SetPlayingRounds(Player player)
    {
        _ = Ch1 && NumberOfRounds > 0 ? player.RoundsPlaying.Add(1) : player.RoundsPlaying.Remove(1);
        _ = Ch2 && NumberOfRounds > 1 ? player.RoundsPlaying.Add(2) : player.RoundsPlaying.Remove(2);
        _ = Ch3 && NumberOfRounds > 2 ? player.RoundsPlaying.Add(3) : player.RoundsPlaying.Remove(3);
        _ = Ch4 && NumberOfRounds > 3 ? player.RoundsPlaying.Add(4) : player.RoundsPlaying.Remove(4);
        _ = Ch5 && NumberOfRounds > 4 ? player.RoundsPlaying.Add(5) : player.RoundsPlaying.Remove(5);
        _ = Ch6 && NumberOfRounds > 5 ? player.RoundsPlaying.Add(6) : player.RoundsPlaying.Remove(6);
        _ = Ch7 && NumberOfRounds > 6 ? player.RoundsPlaying.Add(7) : player.RoundsPlaying.Remove(7);
        _ = Ch8 && NumberOfRounds > 7 ? player.RoundsPlaying.Add(8) : player.RoundsPlaying.Remove(8);
        _ = Ch9 && NumberOfRounds > 8 ? player.RoundsPlaying.Add(9) : player.RoundsPlaying.Remove(9);
        _ = Ch10 && NumberOfRounds > 9 ? player.RoundsPlaying.Add(10) : player.RoundsPlaying.Remove(10);
    }

    public void Receive(EGDSelectionMessage message)
    {
        Data = message.Value;
    }

    protected override void OnActivated()
    {
        Messenger.Register(this);
    }

    protected override void OnDeactivated()
    {
        Messenger.Unregister<EGDSelectionMessage>(this);
    }
}
