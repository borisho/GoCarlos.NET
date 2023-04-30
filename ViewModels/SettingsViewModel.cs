using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models.Enums;
using System.Collections.ObjectModel;
using System.Windows;

namespace GoCarlos.NET.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private const string MacMahon = "MacMahon";
    private const string Championship = "Majstrovstvá";
    private const string Swiss = "Swiss";

    private const string Cross = "Cross";
    private const string Adjacent = "Najsilnejší súper";
    private const string Weakest = "Najslabší súper";
    private const string Random = "Náhodný súper";

    private readonly MainViewModel mvm;

    [ObservableProperty]
    private string selectedTournamentType;

    [ObservableProperty]
    private string selectedPairingMethod;

    [ObservableProperty]
    private string selectedGroupAdditionMethod;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string numberOfRounds;

    [ObservableProperty]
    private string handicapReduction;

    [ObservableProperty]
    private string topGroupBar;

    [ObservableProperty]
    private string bottomGroupBar;

    [ObservableProperty]
    private bool avoidSameCityPairing;

    public SettingsViewModel(MainViewModel mvm)
    {
        this.mvm = mvm;

        name = mvm.Tournament.Name;
        numberOfRounds = mvm.Tournament.NumberOfRounds.ToString();
        handicapReduction = mvm.Tournament.HandicapReduction.ToString();
        topGroupBar = mvm.Tournament.TopGroupBar.ToString();
        bottomGroupBar = mvm.Tournament.BottomGroupBar.ToString();

        avoidSameCityPairing = mvm.Tournament.AvoidSameCityPairing;

        TournamentTypeCollection = new ObservableCollection<string>
        {
            MacMahon,
            Championship,
            Swiss,
        };

        PairingMethodCollection = new ObservableCollection<string>
        {
            Cross,
            Adjacent,
            Weakest,
            Random,
        };

        GroupAdditionCollection = new ObservableCollection<string>
        {
            Adjacent,
            Weakest,
            Random,
        };

        selectedTournamentType = mvm.Tournament.TournamentType switch
        {
            TournamentType.Championship => Championship,
            TournamentType.Swiss => Swiss,
            _ => MacMahon,
        };

        selectedPairingMethod = mvm.Tournament.PairingMethod switch
        {
            PairingMethod.Cross => Cross,
            PairingMethod.Strongest => Adjacent,
            PairingMethod.Weakest => Weakest,
            _ => Random,
        };

        selectedGroupAdditionMethod = mvm.Tournament.AdditionMethod switch
        {
            PairingMethod.Strongest => Adjacent,
            PairingMethod.Random => Random,
            _ => Weakest,
        };
    }
    public ObservableCollection<string> TournamentTypeCollection { get; private set; }
    public ObservableCollection<string> PairingMethodCollection { get; private set; }
    public ObservableCollection<string> GroupAdditionCollection { get; private set; }

    [RelayCommand]
    private void SaveAndExit(ICloseable? window)
    {
        if (window is not null)
        {
            mvm.Tournament.Name = Name;

            mvm.Tournament.AvoidSameCityPairing = AvoidSameCityPairing;

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
                mvm.Tournament.NumberOfRounds = nr;

                if (mvm.Tournament.CurrentRound > nr)
                {
                    mvm.GoToAndRefreshRound(nr);
                }

                mvm.Tournament.AddOrRemoveRounds();
            }
            else
            {
                MessageBox.Show("Nepodarilo sa nastaviť počet kôl!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (int.TryParse(HandicapReduction, out int hr))
            {
                mvm.Tournament.UpdatePairingHandicaps(hr);
            }
            else
            {
                MessageBox.Show("Nepodarilo sa nastaviť redukciu hendikepu!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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

            mvm.GoToAndRefreshRound(mvm.CurrentRoundNumber);

            window?.Close();
        }
    }
}
