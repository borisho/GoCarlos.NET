using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoCarlos.NET.Events;
using GoCarlos.NET.Models;
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

        playerViewModel = new();
        PlayerData = CollectionViewSource.GetDefaultView(playerViewModel);
        PlayerData.SortDescriptions.Add(new SortDescription(nameof(PlayerViewModel.Place), ListSortDirection.Ascending));

        pairingViewModel = new();
        PairingData = CollectionViewSource.GetDefaultView(pairingViewModel);
        PairingData.SortDescriptions.Add(new SortDescription(nameof(PairingViewModel.Board), ListSortDirection.Ascending));

        unpairedPlayers = new();
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
                if (SelectedPairing.Pairing.White.IsBye)
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
            tournament = new();

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
            player.IsSuperGroup = false;
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
        List<Pairing> pairings = tournament.Rounds[tournament.CurrentRound].Pairings.ToList();
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
            tournament.TournamentType,
            tournament.PairingMethod,
            tournament.AdditionMethod,
            Utils.GetOrderedPlayerList(playersToPair,
                tournament.TournamentType,
                currentRound.RoundNumber)
            .ToList()
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
        if (SelectedPairing is not null &&
            !SelectedPairing.Pairing.White.IsBye &&
            !SelectedPairing.Pairing.ResultByReferee)
        {
            SelectedPairing.Pairing.Result = Utils.Next(SelectedPairing.Pairing.Result);
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

            if (!pairing.White.IsBye)
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

            int nameLength = 4;
            int clubLenght = 4;
            int r1Length = 2;
            int r2Length = 2;
            int r3Length = 2;
            int r4Length = 2;
            int r5Length = 2;
            int r6Length = 2;
            int r7Length = 2;
            int r8Length = 2;
            int r9Length = 2;
            int r10Length = 2;
            int scoreLength = 2;
            int scoreXLength = 3;
            int sosLength = 3;
            int sososLength = 5;
            int sodosLength = 5;

            foreach (PlayerViewModel p in PlayerData)
            {
                if (p.FullName.Length > nameLength)
                {
                    nameLength = p.FullName.Length;
                }

                if (p.Club.Length > clubLenght)
                {
                    clubLenght = p.Club.Length;
                }

                if (p.R1.Length > r1Length)
                {
                    r1Length = p.R1.Length;
                }

                if (p.R2.Length > r2Length)
                {
                    r2Length = p.R2.Length;
                }

                if (p.R3.Length > r3Length)
                {
                    r3Length = p.R3.Length;
                }

                if (p.R4.Length > r4Length)
                {
                    r4Length = p.R4.Length;
                }

                if (p.R5.Length > r5Length)
                {
                    r5Length = p.R5.Length;
                }

                if (p.R6.Length > r6Length)
                {
                    r6Length = p.R6.Length;
                }

                if (p.R7.Length > r7Length)
                {
                    r7Length = p.R7.Length;
                }

                if (p.R8.Length > r8Length)
                {
                    r8Length = p.R8.Length;
                }

                if (p.R9.Length > r9Length)
                {
                    r9Length = p.R9.Length;
                }

                if (p.R10.Length > r10Length)
                {
                    r10Length = p.R10.Length;
                }

                if (p.Score.ToString().Length > scoreLength)
                {
                    scoreLength = p.Score.ToString().Length;
                }

                if (p.ScoreX.ToString().Length > scoreXLength)
                {
                    scoreXLength = p.ScoreX.ToString().Length;
                }

                if (p.SOS.ToString().Length > sosLength)
                {
                    sosLength = p.SOS.ToString().Length;
                }

                if (p.SOSOS.ToString().Length > sososLength)
                {
                    sososLength = p.SOSOS.ToString().Length;
                }

                if (p.SODOS.ToString().Length > sodosLength)
                {
                    sodosLength = p.SODOS.ToString().Length;
                }
            }

            if (tournament.CountCurrentRound)
            {
                wText.WriteLine("Wallist - Po " + tournament.CurrentRound.ToString() + ". kole - " + tournament.Name);
                wText.Write("\n");
            }

            else
            {
                wText.WriteLine("Wallist - Pred " + tournament.CurrentRound.ToString() + ". kolom - " + tournament.Name);
                wText.Write("\n");
            }

            wText.Write("{0, -2} {1, -" + nameLength.ToString() + "} {2, -" + clubLenght.ToString() + "} {3, -3} {4, -4} ", "Po", "Meno", "Klub", "Tr", "Rt");

            for (int i = 1; i <= tournament.NumberOfRounds; i++)
            {
                int rLength = 2;
                string rString = "";
                switch (i)
                {
                    case 1:
                        rLength = r1Length;
                        rString = "1.";
                        break;
                    case 2:
                        rLength = r2Length;
                        rString = "2.";
                        break;
                    case 3:
                        rLength = r3Length;
                        rString = "3.";
                        break;
                    case 4:
                        rLength = r4Length;
                        rString = "4.";
                        break;
                    case 5:
                        rLength = r5Length;
                        rString = "5.";
                        break;
                    case 6:
                        rLength = r6Length;
                        rString = "6.";
                        break;
                    case 7:
                        rLength = r7Length;
                        rString = "7.";
                        break;
                    case 8:
                        rLength = r8Length;
                        rString = "8.";
                        break;
                    case 9:
                        rLength = r9Length;
                        rString = "9.";
                        break;
                    case 10:
                        rLength = r10Length;
                        rString = "10.";
                        break;
                    default:
                        break;
                }
                wText.Write("{0, -" + rLength.ToString() + "} ", rString);
            }
            wText.Write("{0, -3} ", "NrW");
            wText.Write("{0, -4} ", "Body");
            wText.Write("{0, -" + scoreXLength + "} ", "MMX");
            wText.Write("{0, -" + scoreLength + "} ", "MM");
            wText.Write("{0, -" + sodosLength + "} ", "SODOS");
            wText.Write("{0, -" + sosLength + "} ", "SOS");
            wText.Write("\n");

            foreach (PlayerViewModel p in PlayerData)
            {
                wText.Write("{0, -2} {1, -" + nameLength.ToString() + "} {2, -" + clubLenght.ToString() + "} {3, -3} {4, -4} ", p.Place, p.FullName, p.Club, p.Grade, p.Gor);
                for (int i = 1; i <= tournament.NumberOfRounds; i++)
                {
                    switch (i)
                    {
                        case 1:
                            wText.Write("{0, -" + r1Length.ToString() + "} ", p.R1);
                            break;
                        case 2:
                            wText.Write("{0, -" + r2Length.ToString() + "} ", p.R2);
                            break;
                        case 3:
                            wText.Write("{0, -" + r3Length.ToString() + "} ", p.R3);
                            break;
                        case 4:
                            wText.Write("{0, -" + r4Length.ToString() + "} ", p.R4);
                            break;
                        case 5:
                            wText.Write("{0, -" + r5Length.ToString() + "} ", p.R5);
                            break;
                        case 6:
                            wText.Write("{0, -" + r6Length.ToString() + "} ", p.R6);
                            break;
                        case 7:
                            wText.Write("{0, -" + r7Length.ToString() + "} ", p.R7);
                            break;
                        case 8:
                            wText.Write("{0, -" + r8Length.ToString() + "} ", p.R8);
                            break;
                        case 9:
                            wText.Write("{0, -" + r9Length.ToString() + "} ", p.R9);
                            break;
                        case 10:
                            wText.Write("{0, -" + r10Length.ToString() + "} ", p.R10);
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
                wText.Write("{0, -" + sosLength + "} ", p.SOS);
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

            int blackLength = 6;
            int whiteLenght = 5;

            foreach (PairingViewModel p in PairingData)
            {
                if (p.Black.Length > blackLength)
                {
                    blackLength = p.Black.Length;
                }

                if (p.White.Length > whiteLenght)
                {
                    whiteLenght = p.White.Length;
                }
            }

            wText.WriteLine("Párovanie - " + tournament.CurrentRound.ToString() + ". kolo - " + tournament.Name);
            wText.WriteLine("\n{0, -2} {1, -" + blackLength.ToString() + "} {2, -" + whiteLenght.ToString() + "} {3, -8} {4, -2} ", "Po", "Čierny", "Biely", "Výsledok", "He");

            foreach (PairingViewModel p in PairingData)
            {
                wText.WriteLine("{0, -2} {1, -" + blackLength.ToString() + "} {2, -" + whiteLenght.ToString() + "} {3, -8} {4, -2} ", p.Board, p.Black, p.White, p.Results, p.Handicap);
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

            int nameLength = 4;
            int clubLenght = 5;
            int r1Length = 2;
            int r2Length = 2;
            int r3Length = 2;
            int r4Length = 2;
            int r5Length = 2;
            int r6Length = 2;
            int r7Length = 2;
            int r8Length = 2;
            int r9Length = 2;
            int r10Length = 2;
            int pointsLength = 1;
            int scoreLength = 1;
            int scoreXLength = 1;
            int sosLength = 1;
            int sososLength = 1;
            int sodosLength = 1;

            foreach (PlayerViewModel p in PlayerData)
            {
                if (p.FullName.Length > nameLength)
                {
                    nameLength = p.FullName.Length;
                }

                if (p.Club.Length > clubLenght)
                {
                    clubLenght = p.Club.Length;
                }

                if (p.R1.Length > r1Length)
                {
                    r1Length = p.R1.Length;
                }

                if (p.R2.Length > r2Length)
                {
                    r2Length = p.R2.Length;
                }

                if (p.R3.Length > r3Length)
                {
                    r3Length = p.R3.Length;
                }

                if (p.R4.Length > r4Length)
                {
                    r4Length = p.R4.Length;
                }

                if (p.R5.Length > r5Length)
                {
                    r5Length = p.R5.Length;
                }

                if (p.R6.Length > r6Length)
                {
                    r6Length = p.R6.Length;
                }

                if (p.R7.Length > r7Length)
                {
                    r7Length = p.R7.Length;
                }

                if (p.R8.Length > r8Length)
                {
                    r8Length = p.R8.Length;
                }

                if (p.R9.Length > r9Length)
                {
                    r9Length = p.R9.Length;
                }

                if (p.R10.Length > r10Length)
                {
                    r10Length = p.R10.Length;
                }

                if (p.EGDPoints.Length > pointsLength)
                {
                    pointsLength = p.EGDPoints.Length;
                }

                if (p.EGDScore.Length > scoreLength)
                {
                    scoreLength = p.EGDScore.Length;
                }

                if (p.EGDScoreX.Length > scoreXLength)
                {
                    scoreXLength = p.EGDScoreX.Length;
                }

                if (p.EGDSOS.Length > sosLength)
                {
                    sosLength = p.EGDSOS.Length;
                }

                if (p.EGDSOSOS.Length > sososLength)
                {
                    sososLength = p.EGDSOSOS.Length;
                }

                if (p.EGDSODOS.Length > sodosLength)
                {
                    sodosLength = p.EGDSODOS.Length;
                }
            }

            wText.WriteLine("; EV[" + tournament.Name + "]");

            foreach (PlayerViewModel p in PlayerData)
            {
                wText.Write("{0, -2} {1, -" + nameLength.ToString() + "} {2, -3} {3, -2} {4, -" + clubLenght.ToString() + "}", p.Place, p.FullName, p.Grade, p.State, p.Club);
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
                            wText.Write("{0, -" + r1Length.ToString() + "} ", p.R1);
                            break;
                        case 2:
                            wText.Write("{0, -" + r2Length.ToString() + "} ", p.R2);
                            break;
                        case 3:
                            wText.Write("{0, -" + r3Length.ToString() + "} ", p.R3);
                            break;
                        case 4:
                            wText.Write("{0, -" + r4Length.ToString() + "} ", p.R4);
                            break;
                        case 5:
                            wText.Write("{0, -" + r5Length.ToString() + "} ", p.R5);
                            break;
                        case 6:
                            wText.Write("{0, -" + r6Length.ToString() + "} ", p.R6);
                            break;
                        case 7:
                            wText.Write("{0, -" + r7Length.ToString() + "} ", p.R7);
                            break;
                        case 8:
                            wText.Write("{0, -" + r8Length.ToString() + "} ", p.R8);
                            break;
                        case 9:
                            wText.Write("{0, -" + r9Length.ToString() + "} ", p.R9);
                            break;
                        case 10:
                            wText.Write("{0, -" + r10Length.ToString() + "} ", p.R10);
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
        List<string> clubs = new()
        {
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
        };
        List<string> countryCode = new()
        {
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
        };

        List<Player> players = new();

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
