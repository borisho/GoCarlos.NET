using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoCarlos.NET.Events;
using GoCarlos.NET.Models;
using GoCarlos.NET.Models.Enums;
using GoCarlos.NET.Models.Records;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace GoCarlos.NET.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ObservableCollection<PlayerViewModel> playerViewModel;
    private readonly ObservableCollection<PairingViewModel> pairingViewModel;
    private readonly ObservableCollection<PlayerViewModel> unpairedPlayers;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedPairingIsNull), nameof(SelectedPairingIsBye))]
    private PairingViewModel? selectedPairing;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSuitableToDelete))]
    private PlayerViewModel? selectedPlayer;

    private Tournament tournament;

    public MainViewModel()
    {
        tournament = new(5);

        playerViewModel = [];
        PlayerData = CollectionViewSource.GetDefaultView(playerViewModel);
        PlayerData.SortDescriptions.Add(new SortDescription(nameof(PlayerViewModel.Place), ListSortDirection.Ascending));

        pairingViewModel = [];
        PairingData = CollectionViewSource.GetDefaultView(pairingViewModel);
        PairingData.SortDescriptions.Add(new SortDescription(nameof(PairingViewModel.Board), ListSortDirection.Ascending));

        unpairedPlayers = [];
        UnpairedPlayers = CollectionViewSource.GetDefaultView(unpairedPlayers);
        UnpairedPlayers.SortDescriptions.Add(new SortDescription(nameof(PlayerViewModel.Score), ListSortDirection.Descending));
        UnpairedPlayers.SortDescriptions.Add(new SortDescription(nameof(PlayerViewModel.Gor), ListSortDirection.Descending));
    }

    public ICollectionView PlayerData { get; }
    public ICollectionView PairingData { get; }
    public ICollectionView UnpairedPlayers { get; }

    public bool SelectedPairingIsNull { get => SelectedPairing is not null; }
    public Visibility SelectedPairingIsBye
    {
        get
        {
            if (SelectedPairing is not null)
            {
                if (SelectedPairing.Pairing.White.Group == Group.Bye)
                {
                    return Visibility.Collapsed;
                }

                else
                {
                    return Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }
    }
    public Visibility IsSuitableToDelete
    {
        get
        {
            if (SelectedPlayer is not null)
            {
                foreach (Round r in Tournament.Rounds)
                {
                    foreach (Pairing p in r.Pairings)
                    {
                        if (p.Black.Equals(SelectedPlayer.Player) || p.White.Equals(SelectedPlayer.Player))
                        {
                            return Visibility.Collapsed;
                        }
                    }
                }
            }

            return Visibility.Visible;
        }
    }

    public Tournament Tournament { get => tournament; }

    public bool IsCurrentRoundCounting
    {
        get => tournament.CountCurrentRound;
        set
        {
            tournament.CountCurrentRound = value;
            GoToAndRefreshRound(CurrentRoundNumber);
        }
    }
    public bool CanGoNextRound { get => tournament.CurrentRound < tournament.NumberOfRounds - 1; }
    public bool CanGoPreviousRound { get => tournament.CurrentRound > 0; }
    public int CurrentRoundNumber
    {
        get => tournament.CurrentRound;
        set
        {
            tournament.CurrentRound = value;
            CallPropertyChangedEvent();
        }
    }
    public string GetTitle { get => "GoCarlos " + Utils.VERSION + " - Kolo " + (tournament.CurrentRound + 1).ToString(); }

    #region GoToRound visibility

    public Visibility GTR1V { get => tournament.NumberOfRounds > 0 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility GTR2V { get => tournament.NumberOfRounds > 1 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility GTR3V { get => tournament.NumberOfRounds > 2 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility GTR4V { get => tournament.NumberOfRounds > 3 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility GTR5V { get => tournament.NumberOfRounds > 4 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility GTR6V { get => tournament.NumberOfRounds > 5 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility GTR7V { get => tournament.NumberOfRounds > 6 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility GTR8V { get => tournament.NumberOfRounds > 7 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility GTR9V { get => tournament.NumberOfRounds > 8 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility GTR10V { get => tournament.NumberOfRounds > 9 ? Visibility.Visible : Visibility.Collapsed; }

    #endregion

    #region Wallist round visibility

    public Visibility R2V { get => tournament.CurrentRound > 0 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility R3V { get => tournament.CurrentRound > 1 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility R4V { get => tournament.CurrentRound > 2 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility R5V { get => tournament.CurrentRound > 3 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility R6V { get => tournament.CurrentRound > 4 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility R7V { get => tournament.CurrentRound > 5 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility R8V { get => tournament.CurrentRound > 6 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility R9V { get => tournament.CurrentRound > 7 ? Visibility.Visible : Visibility.Collapsed; }
    public Visibility R10V { get => tournament.CurrentRound > 8 ? Visibility.Visible : Visibility.Collapsed; }

    #endregion


    [RelayCommand]
    private void CreateNewTournament()
    {
        if (MessageBox.Show("Vytvoriť nový turnaj? Neuložené zmeny budú stratené!",
            "Vytvoriť nový turnaj",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
            tournament = new(5);

            GoToAndRefreshRound(0);
            ClearObservables();
        }
    }

    [RelayCommand]
    private void LoadTournament()
    {
        OpenFileDialog ofd = new()
        {
            Title = "Načítať turnaj z",
            DefaultExt = ".json",
            Filter = "JsonFiles (*.json)|*.json"
        };

        if (ofd.ShowDialog() == true)
        {
            var fileStream = ofd.OpenFile();

            using StreamReader reader = new(fileStream);
            Tournament? t = JsonConvert.DeserializeObject<Tournament>(reader.ReadToEnd(), Utils.JsonSerializerSettings);

            if (t is not null)
            {
                tournament = t;

                playerViewModel.Clear();
                pairingViewModel.Clear();
                unpairedPlayers.Clear();

                foreach (Player p in tournament.Players)
                {
                    playerViewModel.Add(new(p, tournament.CurrentRound));
                }

                GoToAndRefreshRound(t.CurrentRound);
            }
            else
            {
                MessageBox.Show("Nepodarilo sa načítať turnaj zo súboru!",
                    "Chyba!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private void SaveTournament()
    {
        SaveFileDialog sfd = new()
        {
            Title = "Uložiť turnaj do",
            DefaultExt = ".json",
            Filter = "JsonFiles (*.json)|*.json"
        };

        if (sfd.ShowDialog() == true)
        {
            using Stream myStream = sfd.OpenFile();
            StreamWriter wText = new(myStream);

            string json = JsonConvert.SerializeObject(tournament, Utils.JsonSerializerSettings);
            wText.Write(json);
            wText.Close();
        }
    }

    [RelayCommand]
    private void AddPlayerWindow(bool? param)
    {
        bool addOneMore = param ?? false;

        PlayerWindowViewModel pwvm = new(null, tournament.NumberOfRounds, addOneMore);
        pwvm.AddPlayerEvent += OnAddPlayer;

        PlayerWindow playerWindow = new()
        {
            Title = "Pridať hráča",
            DataContext = pwvm,
        };

        playerWindow.Show();
    }
    private void OnAddPlayer(object? sender, PlayerEventArgs args)
    {
        AddPlayer(args.Player);
        GoToAndRefreshRound(CurrentRoundNumber);

        bool param = args.Bool ?? false;

        if (param)
        {
            AddPlayerWindowCommand.Execute(param);
        }
    }

    private void AddPlayer(Player player)
    {
        foreach (int i in player.RoundsPlaying)
        {
            tournament.Rounds[i].AddPlayer(player);

            if (CurrentRoundNumber == i)
            {
                unpairedPlayers.Add(new(player, CurrentRoundNumber));
            }
        }

        tournament.Players.Add(player);
        playerViewModel.Add(new(player, CurrentRoundNumber));
    }

    private void AddPlayerBatch(List<Player> players)
    {
        foreach (Player player in players)
        {
            AddPlayer(player);
        }
    }

    [RelayCommand]
    private void GoToSettigns()
    {
        SettingsWindow settingsWindow = new()
        {
            DataContext = new SettingsViewModel(this)
        };
        settingsWindow.ShowDialog();
    }

    [RelayCommand]
    private void MakePairings()
    {
        MakePairings(unpairedPlayers.Select(pvm => pvm.Player).ToList());
    }

    [RelayCommand]
    private void DropPairings()
    {
        List<Pairing> pairings = [.. tournament.Rounds[tournament.CurrentRound].Pairings];
        foreach (Pairing p in pairings)
        {
            DeletePairing(p);
        }

        GoToAndRefreshRound(CurrentRoundNumber);
    }

    [RelayCommand]
    private void MakePairingSelection(object? obj)
    {
        if (obj is not null)
        {
            System.Collections.IList list = (System.Collections.IList)obj;
            MakePairings(list.Cast<PlayerViewModel>().Select(pvm => pvm.Player).ToList());
        }
    }

    private void MakePairings(List<Player> playersToPair)
    {
        Round currentRound = tournament.Rounds[tournament.CurrentRound];

        PairingGeneratorParameters parameters = new(currentRound,
            tournament.AvoidSameCityPairing,
            tournament.HandicapReduction,
            tournament.HandicapBasedMm,
            tournament.TournamentType,
            tournament.TopGroupPairingMethod,
            tournament.PairingMethod,
            tournament.AdditionMethod,
            [.. Utils.GetOrderedPlayerList(playersToPair,
                tournament.TournamentType,
                currentRound.RoundNumber,
                tournament.NumberOfRounds)],
            tournament.NumberOfRounds
        );

        PairingGenerator.PerformPairings(parameters);

        tournament.ResetBoardNumbers();

        GoToAndRefreshRound(CurrentRoundNumber);
    }

    [RelayCommand]
    private void EditSelectedPlayer()
    {
        if (SelectedPlayer is not null)
        {
            PlayerWindowViewModel pwvm = new(SelectedPlayer, tournament.NumberOfRounds, false);
            pwvm.EditPlayerEvent += OnEditPlayer;

            PlayerWindow playerWindow = new()
            {
                Title = SelectedPlayer.FullName,
                DataContext = pwvm,
            };
            playerWindow.ShowDialog();

            pwvm.EditPlayerEvent -= OnEditPlayer;
        }
    }

    [RelayCommand]
    private void DeleteSelectedPlayer()
    {
        if (SelectedPlayer is not null)
        {
            PlayerViewModel pvm = SelectedPlayer;

            foreach (Round r in tournament.Rounds)
            {
                r.RemovePlayer(SelectedPlayer.Player);
            }

            unpairedPlayers.Remove(pvm);
            playerViewModel.Remove(pvm);
            tournament.Players.Remove(pvm.Player);

            GoToAndRefreshRound(CurrentRoundNumber);
        }
    }

    private void OnEditPlayer(object? sender, PlayerEventArgs args)
    {
        tournament.UpdatePlayerPlayingRounds(args.Player);
        GoToAndRefreshRound(CurrentRoundNumber);
    }
    public void SelectedPairingClose(object? sender, EventArgs args)
    {
        GoToAndRefreshRound(CurrentRoundNumber);
    }
    private void OnSelectedPairingDelete(object? sender, EventArgs args)
    {
        DeleteSelectedPairing();
    }

    [RelayCommand]
    private void EditSelectedPairing()
    {
        if (SelectedPairing is not null)
        {
            PairingWindow pairingWindow = new()
            {
                DataContext = SelectedPairing
            };
            pairingWindow.Closed += SelectedPairingClose;
            pairingWindow.ShowDialog();
        }
    }

    [RelayCommand]
    private void SwapColors()
    {
        SelectedPairing?.SwapColorsCommand.Execute(null);
    }

    [RelayCommand]
    private void EditPairingResult()
    {
        if (SelectedPairing is not null && SelectedPairing.Pairing.White.Group != Group.Bye)
        {
            Group whitePlayerGroup = SelectedPairing.Pairing.White.Group;
            if (whitePlayerGroup != Group.Bye)
            {
                SelectedPairing.SetResult(Utils.Next(SelectedPairing.Pairing.Result));
                GoToAndRefreshRound(CurrentRoundNumber);
            }
        }
    }

    [RelayCommand]
    private void EditPlayerGroup()
    {
        if (SelectedPlayer is not null && SelectedPlayer.Player.Group != Group.Bye)
        {
            Group newGroup = Utils.Next(SelectedPlayer.GroupColor);
            SelectedPlayer.GroupColor = newGroup == Group.Bye ? Utils.Next(newGroup) : newGroup;
            GoToAndRefreshRound(CurrentRoundNumber);
        }
    }

    [RelayCommand]
    private void DeleteSelectedPairing()
    {
        if (SelectedPairing is not null && DeletePairing(SelectedPairing.Pairing))
        {
            pairingViewModel.Remove(SelectedPairing);
            GoToAndRefreshRound(CurrentRoundNumber);
        }
    }
    private bool DeletePairing(Pairing pairing)
    {

        if (tournament.Rounds[tournament.CurrentRound].RemovePairing(pairing))
        {
            unpairedPlayers.Add(new(pairing.Black, CurrentRoundNumber));

            if (pairing.White.Group != Group.Bye)
            {
                unpairedPlayers.Add(new(pairing.White, CurrentRoundNumber));
            }

            tournament.ResetBoardNumbers();

            GoToAndRefreshRound(CurrentRoundNumber);

            return true;
        }

        return false;
    }

    [RelayCommand]
    private void ExportWallist()
    {
        SaveFileDialog sfd = new()
        {
            Title = "Exportovať do",
            DefaultExt = ".txt",
            Filter = "Text documents (.txt)|*.txt"
        };

        if (sfd.ShowDialog() == true)
        {
            using Stream myStream = sfd.OpenFile();
            StreamWriter wText = new(myStream);

            int nameLength = Math.Max(4, playerViewModel.Max(p => p.FullName.Length));
            int clubLenght = Math.Max(4, playerViewModel.Max(p => p.Club.Length));
            int r1Length = Math.Max(2, playerViewModel.Max(p => p.R1.Length));
            int r2Length = Math.Max(2, playerViewModel.Max(p => p.R2.Length));
            int r3Length = Math.Max(2, playerViewModel.Max(p => p.R3.Length));
            int r4Length = Math.Max(2, playerViewModel.Max(p => p.R4.Length));
            int r5Length = Math.Max(2, playerViewModel.Max(p => p.R5.Length));
            int r6Length = Math.Max(2, playerViewModel.Max(p => p.R6.Length));
            int r7Length = Math.Max(2, playerViewModel.Max(p => p.R7.Length));
            int r8Length = Math.Max(2, playerViewModel.Max(p => p.R8.Length));
            int r9Length = Math.Max(2, playerViewModel.Max(p => p.R9.Length));
            int r10Length = Math.Max(2, playerViewModel.Max(p => p.R10.Length));
            int scoreLength = Math.Max(2, playerViewModel.Max(p => p.Score.ToString().Length));
            int scoreXLength = Math.Max(3, playerViewModel.Max(p => p.ScoreX.ToString().Length));
            int sosLength = Math.Max(3, playerViewModel.Max(p => p.SOS.ToString().Length));
            int sososLength = Math.Max(5, playerViewModel.Max(p => p.SOSOS.ToString().Length));
            int sodosLength = Math.Max(5, playerViewModel.Max(p => p.SODOS.ToString().Length));

            if (tournament.CountCurrentRound)
            {
                wText.WriteLine("Wallist - Po " + (tournament.CurrentRound + 1) + ". kole - " + tournament.Name);
                wText.Write("\n");
            }

            else
            {
                wText.WriteLine("Wallist - Pred " + (tournament.CurrentRound + 1) + ". kolom - " + tournament.Name);
                wText.Write("\n");
            }

            wText.Write("{0, -2} {1, -" + nameLength + "} {2, -" + clubLenght + "} {3, -3} {4, -4} ", "Po", "Meno", "Klub", "Tr", "Rt");

            for (int i = 0; i <= tournament.CurrentRound; i++)
            {
                int rLength = 2;
                string rString = "";
                switch (i)
                {
                    case 0:
                        rLength = r1Length;
                        rString = "1.";
                        break;
                    case 1:
                        rLength = r2Length;
                        rString = "2.";
                        break;
                    case 2:
                        rLength = r3Length;
                        rString = "3.";
                        break;
                    case 3:
                        rLength = r4Length;
                        rString = "4.";
                        break;
                    case 4:
                        rLength = r5Length;
                        rString = "5.";
                        break;
                    case 5:
                        rLength = r6Length;
                        rString = "6.";
                        break;
                    case 6:
                        rLength = r7Length;
                        rString = "7.";
                        break;
                    case 7:
                        rLength = r8Length;
                        rString = "8.";
                        break;
                    case 8:
                        rLength = r9Length;
                        rString = "9.";
                        break;
                    case 9:
                        rLength = r10Length;
                        rString = "10.";
                        break;
                    default:
                        break;
                }
                wText.Write("{0, -" + rLength + "} ", rString);
            }
            wText.Write("{0, -3} ", "NrW");
            wText.Write("{0, -4} ", "Body");
            wText.Write("{0, -" + scoreXLength + "} ", "MMX");
            wText.Write("{0, -" + scoreLength + "} ", "MM");
            wText.Write("{0, -" + sodosLength + "} ", "SODOS");
            wText.Write("{0, -" + sosLength + "}", "SOS");
            wText.Write("\n");

            foreach (PlayerViewModel p in PlayerData)
            {
                wText.Write("{0, -2} {1, -" + nameLength + "} {2, -" + clubLenght + "} {3, -3} {4, -4} ", p.Place, p.FullName, p.Club, p.Grade, p.Gor);
                for (int i = 0; i <= tournament.CurrentRound; i++)
                {
                    switch (i)
                    {
                        case 0:
                            wText.Write("{0, -" + r1Length + "} ", p.R1);
                            break;
                        case 1:
                            wText.Write("{0, -" + r2Length + "} ", p.R2);
                            break;
                        case 2:
                            wText.Write("{0, -" + r3Length + "} ", p.R3);
                            break;
                        case 3:
                            wText.Write("{0, -" + r4Length + "} ", p.R4);
                            break;
                        case 4:
                            wText.Write("{0, -" + r5Length + "} ", p.R5);
                            break;
                        case 5:
                            wText.Write("{0, -" + r6Length + "} ", p.R6);
                            break;
                        case 6:
                            wText.Write("{0, -" + r7Length + "} ", p.R7);
                            break;
                        case 7:
                            wText.Write("{0, -" + r8Length + "} ", p.R8);
                            break;
                        case 8:
                            wText.Write("{0, -" + r9Length + "} ", p.R9);
                            break;
                        case 9:
                            wText.Write("{0, -" + r10Length + "} ", p.R10);
                            break;
                        default:
                            break;
                    }
                }

                wText.Write("{0, -3} ", p.NrW);
                wText.Write("{0, -4} ", p.Points);
                wText.Write("{0, -" + scoreXLength + "} ", p.ScoreX);
                wText.Write("{0, -" + scoreLength + "} ", p.Score);
                wText.Write("{0, -" + sodosLength + "} ", p.SODOS);
                wText.Write("{0, -" + sosLength + "}", p.SOS);
                wText.Write("\n");
            }

            wText.WriteLine("\nDátum a čas výpisu: {0:F}", DateTime.Now.ToString());
            wText.Close();
        }
    }

    [RelayCommand]
    private void ExportPairing()
    {
        SaveFileDialog sfd = new()
        {
            Title = "Exportovať do",
            DefaultExt = ".txt",
            Filter = "Text documents (.txt)|*.txt"
        };

        if (sfd.ShowDialog() == true)
        {
            using Stream myStream = sfd.OpenFile();
            StreamWriter wText = new(myStream);

            int blackLength = Math.Max(6, pairingViewModel.Max(p => p.Black.Length));
            int whiteLenght = Math.Max(5, pairingViewModel.Max(p => p.White.Length));

            wText.WriteLine("Párovanie - " + (tournament.CurrentRound + 1) + ". kolo - " + tournament.Name);
            wText.WriteLine("\n{0, -2} {1, -" + blackLength + "} {2, -" + whiteLenght + "} {3, -8} {4, -2} ", "Po", "Čierny", "Biely", "Výsledok", "He");

            foreach (PairingViewModel p in PairingData)
            {
                wText.WriteLine();
                wText.WriteLine("{0, -2} {1, -" + blackLength + "} {2, -" + whiteLenght + "} {3, -8} {4, -2} ", p.Board, p.Black, p.White, p.Results, p.Handicap);
            }

            wText.WriteLine("\nDátum a čas výpisu: {0:F}", DateTime.Now.ToString());
            wText.Close();
        }
    }

    [RelayCommand]
    private void ExportEgd()
    {
        SaveFileDialog sfd = new()
        {
            Title = "Exportovať do",
            DefaultExt = ".txt",
            Filter = "Text documents (.txt)|*.txt"
        };

        if (sfd.ShowDialog() == true)
        {
            using Stream myStream = sfd.OpenFile();
            StreamWriter wText = new(myStream);

            int nameLength = Math.Max(4, playerViewModel.Max(p => p.FullName.Length));
            int clubLenght = Math.Max(4, playerViewModel.Max(p => p.Club.Length));
            int r1Length = playerViewModel.Max(p => p.R1.Length);
            int r2Length = playerViewModel.Max(p => p.R2.Length);
            int r3Length = playerViewModel.Max(p => p.R3.Length);
            int r4Length = playerViewModel.Max(p => p.R4.Length);
            int r5Length = playerViewModel.Max(p => p.R5.Length);
            int r6Length = playerViewModel.Max(p => p.R6.Length);
            int r7Length = playerViewModel.Max(p => p.R7.Length);
            int r8Length = playerViewModel.Max(p => p.R8.Length);
            int r9Length = playerViewModel.Max(p => p.R9.Length);
            int r10Length = playerViewModel.Max(p => p.R10.Length);
            int pointsLength = playerViewModel.Max(p => p.EGDPoints.Length);
            int scoreLength = playerViewModel.Max(p => p.EGDScore.Length);
            int scoreXLength = playerViewModel.Max(p => p.EGDScoreX.Length);
            int sosLength = playerViewModel.Max(p => p.EGDSOS.Length);
            int sososLength = playerViewModel.Max(p => p.EGDSOSOS.Length);
            int sodosLength = playerViewModel.Max(p => p.EGDSODOS.Length);

            wText.WriteLine("; EV[" + tournament.Name + "]");

            foreach (PlayerViewModel p in PlayerData)
            {
                wText.Write("{0, -2} {1, -" + nameLength + "} {2, -3} {3, -2} {4, -" + clubLenght + "} ", p.Place, p.FullName, p.Grade, p.State, p.Club);
                wText.Write("{0, -" + pointsLength + "} ", p.EGDPoints);
                wText.Write("{0, -" + scoreXLength + "} ", p.EGDScoreX);
                wText.Write("{0, -" + scoreLength + "} ", p.EGDScore);
                wText.Write("{0, -" + sodosLength + "} ", p.EGDSODOS);
                wText.Write("{0, -" + sosLength + "} ", p.EGDSOS);

                for (int i = 1; i <= tournament.NumberOfRounds; i++)
                {
                    switch (i)
                    {
                        case 1:
                            wText.Write("{0, -" + r1Length + "} ", p.R1);
                            break;
                        case 2:
                            wText.Write("{0, -" + r2Length + "} ", p.R2);
                            break;
                        case 3:
                            wText.Write("{0, -" + r3Length + "} ", p.R3);
                            break;
                        case 4:
                            wText.Write("{0, -" + r4Length + "} ", p.R4);
                            break;
                        case 5:
                            wText.Write("{0, -" + r5Length + "} ", p.R5);
                            break;
                        case 6:
                            wText.Write("{0, -" + r6Length + "} ", p.R6);
                            break;
                        case 7:
                            wText.Write("{0, -" + r7Length + "} ", p.R7);
                            break;
                        case 8:
                            wText.Write("{0, -" + r8Length + "} ", p.R8);
                            break;
                        case 9:
                            wText.Write("{0, -" + r9Length + "} ", p.R9);
                            break;
                        case 10:
                            wText.Write("{0, -" + r10Length + "} ", p.R10);
                            break;
                        default:
                            break;
                    }
                }
                wText.Write("\n");
            }

            wText.Close();
        }
    }

    [RelayCommand]
    private void GeneratePlayers(int number)
    {
        Random random = new(DateAndTime.Now.Millisecond);
        List<string> clubs =
        [
            "Kosi",
            "Brat",
            "Vlky",
            "Brno",
            "Prah",

            "Pres",
            "Rozn",
            "SoIm",
            "ProG",
            "kVag",
        ];
        List<string> countryCode =
        [
            "SK",
            "CZ",
            "DE",
            "HU",
            "PL",

            "RS",
            "UA",
            "RU",
            "RO",
            "BG",
        ];

        List<Player> players = [];

        for (int i = 0; i < number; i++)
        {
            EGD_Data data = new()
            {
                Club = clubs[random.Next(10)],
                Country_Code = countryCode[random.Next(10)],
                Name = random.NextInt64(100000, 1000000).ToString(),
                Last_Name = random.NextInt64(100000, 1000000).ToString(),
                Grade = Utils.GetGrade(random.Next(38))
            };

            players.Add(new(data));
        }

        AddPlayerBatch(players);
        GoToAndRefreshRound(CurrentRoundNumber);
    }

    [RelayCommand]
    private void IncreaseRound() { GoToAndRefreshRound(CurrentRoundNumber + 1); }

    [RelayCommand]
    private void DecreaseRound() { GoToAndRefreshRound(CurrentRoundNumber - 1); }

    [RelayCommand]
    public void GoToAndRefreshRound(int round)
    {
        CurrentRoundNumber = round;
        CalculateCriteria();

        foreach (PlayerViewModel pvm in playerViewModel)
        {
            pvm.CurrentRound = CurrentRoundNumber;
        }

        ClearAndFillPairings(tournament.Rounds[CurrentRoundNumber]);

        PlayerData.Refresh();
        PairingData.Refresh();
        UnpairedPlayers.Refresh();

        CallPropertyChangedEvent();
    }

    [RelayCommand]
    private void CountCurrentRound()
    {
        IsCurrentRoundCounting = !IsCurrentRoundCounting;
    }

    private void ClearObservables()
    {
        playerViewModel.Clear();
        pairingViewModel.Clear();
        unpairedPlayers.Clear();
    }

    private void ClearAndFillPairings(Round round)
    {
        pairingViewModel.Clear();
        unpairedPlayers.Clear();

        foreach (Pairing p in round.Pairings)
        {
            PairingViewModel pvm = new(p);

            pvm.DeleteSelectedPlayerEvent += OnSelectedPairingDelete;

            pairingViewModel.Add(pvm);
        }

        foreach (Player p in round.UnpairedPlayers)
        {
            unpairedPlayers.Add(new(p, tournament.CurrentRound));
        }
    }

    private void CalculateCriteria()
    {
        if (tournament.CountCurrentRound)
        {
            tournament.CalculateCriteria(CurrentRoundNumber + 1);
        }
        else
        {
            tournament.CalculateCriteria(CurrentRoundNumber);
        }
    }

    private void CallPropertyChangedEvent()
    {
        OnPropertyChanged(nameof(CurrentRoundNumber));
        OnPropertyChanged(nameof(IsCurrentRoundCounting));
        OnPropertyChanged(nameof(CanGoNextRound));
        OnPropertyChanged(nameof(CanGoPreviousRound));
        OnPropertyChanged(nameof(GetTitle));
        OnPropertyChanged(nameof(GTR1V));
        OnPropertyChanged(nameof(GTR2V));
        OnPropertyChanged(nameof(GTR3V));
        OnPropertyChanged(nameof(GTR4V));
        OnPropertyChanged(nameof(GTR5V));
        OnPropertyChanged(nameof(GTR6V));
        OnPropertyChanged(nameof(GTR7V));
        OnPropertyChanged(nameof(GTR8V));
        OnPropertyChanged(nameof(GTR9V));
        OnPropertyChanged(nameof(GTR10V));
        OnPropertyChanged(nameof(R2V));
        OnPropertyChanged(nameof(R3V));
        OnPropertyChanged(nameof(R4V));
        OnPropertyChanged(nameof(R5V));
        OnPropertyChanged(nameof(R6V));
        OnPropertyChanged(nameof(R7V));
        OnPropertyChanged(nameof(R8V));
        OnPropertyChanged(nameof(R9V));
        OnPropertyChanged(nameof(R10V));
        OnPropertyChanged(nameof(IsSuitableToDelete));
    }

    [RelayCommand]
    private static void Exit()
    {
        if (MessageBox.Show("Ukončiť program? Neuložené zmeny budú navždy stratené!",
            "Ukončiť program",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
            Application.Current.Shutdown();
        }
    }
}
