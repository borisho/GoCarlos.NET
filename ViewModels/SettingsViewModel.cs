using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models.Enums;
using System.Collections.ObjectModel;
using System.Windows;

namespace GoCarlos.NET.ViewModels;

public partial class SettingsViewModel(MainViewModel mvm) : ObservableObject
{
    private const string MacMahon = "MacMahon";
    private const string Championship = "Majstrovstvá";
    private const string Swiss = "Swiss";

    private const string Cross = "Cross";
    private const string Adjacent = "Najsilnejší súper";
    private const string Weakest = "Najslabší súper";
    private const string Random = "Náhodný súper";

    public readonly MainViewModel mvm = mvm;

    [ObservableProperty]
    private string selectedTournamentType = mvm.Tournament.TournamentType switch
    {
        TournamentType.Championship => Championship,
        TournamentType.Swiss => Swiss,
        _ => MacMahon,
    };

    [ObservableProperty]
    private string selectedPairingMethod = mvm.Tournament.PairingMethod switch
    {
        PairingMethod.Cross => Cross,
        PairingMethod.Strongest => Adjacent,
        PairingMethod.Weakest => Weakest,
        _ => Random,
    };

    [ObservableProperty]
    private string selectedGroupAdditionMethod = mvm.Tournament.AdditionMethod switch
    {
        PairingMethod.Strongest => Adjacent,
        PairingMethod.Random => Random,
        _ => Weakest,
    };

    [ObservableProperty]
    private string name = mvm.Tournament.Name;

    [ObservableProperty]
    private string numberOfRounds = mvm.Tournament.NumberOfRounds.ToString();

    [ObservableProperty]
    private string handicapReduction = mvm.Tournament.HandicapReduction.ToString();

    [ObservableProperty]
    private string topGroupBar = mvm.Tournament.TopGroupBar.ToString();

    [ObservableProperty]
    private string bottomGroupBar = mvm.Tournament.BottomGroupBar.ToString();

    [ObservableProperty]
    private bool avoidSameCityPairing = mvm.Tournament.AvoidSameCityPairing;

    [ObservableProperty]
    private bool handicapBasedMm = mvm.Tournament.HandicapBasedMm;

    public ObservableCollection<string> TournamentTypeCollection { get; private set; } =
        [
            MacMahon,
            Championship,
            Swiss,
        ];
    public ObservableCollection<string> PairingMethodCollection { get; private set; } =
        [
            Cross,
            Adjacent,
            Weakest,
            Random,
        ];
    public ObservableCollection<string> GroupAdditionCollection { get; private set; } =
        [
            Adjacent,
            Weakest,
            Random,
        ];

    [RelayCommand]
    private void SaveAndExit(ICloseable? window)
    {
        if (window is not null)
        {
            mvm.Tournament.Name = Name;
            mvm.Tournament.AvoidSameCityPairing = AvoidSameCityPairing;
            mvm.Tournament.HandicapBasedMm = HandicapBasedMm;

            mvm.Tournament.TournamentType = SelectedTournamentType switch
            {
                Championship => TournamentType.Championship,
                Swiss => TournamentType.Swiss,
                _ => TournamentType.McMahon,
            };

            mvm.Tournament.PairingMethod = SelectedPairingMethod switch
            {
                Cross => PairingMethod.Cross,
                Adjacent => PairingMethod.Strongest,
                Weakest => PairingMethod.Weakest,
                _ => PairingMethod.Random,
            };

            mvm.Tournament.AdditionMethod = SelectedGroupAdditionMethod switch
            {
                Adjacent => PairingMethod.Strongest,
                Random => PairingMethod.Random,
                _ => PairingMethod.Weakest,
            };

            if (int.TryParse(NumberOfRounds, out int nr))
            {
                if (nr > 10)
                {
                    nr = 10;
                    MessageBox.Show("Maximálny počet kôl je 10, nastavujem 10!", "Upozornenie", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                mvm.Tournament.NumberOfRounds = nr;

                if (mvm.Tournament.CurrentRound > nr - 1)
                {
                    mvm.GoToAndRefreshRound(nr - 1);
                }

                mvm.Tournament.AddOrRemoveRounds();
            }
            else
            {
                MessageBox.Show("Nepodarilo sa nastaviť počet kôl!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (int.TryParse(HandicapReduction, out int hr))
            {
                mvm.Tournament.HandicapReduction = hr;
            }
            else
            {
                MessageBox.Show("Nepodarilo sa nastaviť redukciu hendikepu!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            mvm.Tournament.UpdatePairingHandicaps();

            if (float.TryParse(TopGroupBar, out float tgb))
            {
                mvm.Tournament.TopGroupBar = tgb;
            }
            else
            {
                MessageBox.Show("Nepodarilo sa nastaviť hranicu TopGroup!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (float.TryParse(BottomGroupBar, out float bgb))
            {
                mvm.Tournament.BottomGroupBar = bgb;
            }
            else
            {
                MessageBox.Show("Nepodarilo sa nastaviť hranicu BottomGroup!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            window?.Close();
        }
    }
}
