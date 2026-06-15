using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;
using GoCarlos.NET.Models.Enums;
using System.Collections.ObjectModel;
using System.Windows;

namespace GoCarlos.NET.ViewModels;

public partial class SettingsViewModel(MainViewModel mvm) : ObservableObject
{
    private const string Cross = "Hrable";
    private const string Adjacent = "Najsilnejší súper";
    private const string Weakest = "Najslabší súper";
    private const string Random = "Náhodný súper";

    public readonly MainViewModel mvm = mvm;

    [ObservableProperty]
    public partial Criteria[] AllCriteria { get; set; } = CriteriaSettings.AllCriteria;

    [ObservableProperty]
    public partial Criteria C1 { get; set; } = mvm.Tournament.CriteriaSettings.Criterias[0];

    [ObservableProperty]
    public partial Criteria C2 { get; set; } = mvm.Tournament.CriteriaSettings.Criterias[1];

    [ObservableProperty]
    public partial Criteria C3 { get; set; } = mvm.Tournament.CriteriaSettings.Criterias[2];

    [ObservableProperty]
    public partial Criteria C4 { get; set; } = mvm.Tournament.CriteriaSettings.Criterias[3];

    [ObservableProperty]
    public partial Criteria C5 { get; set; } = mvm.Tournament.CriteriaSettings.Criterias[4];

    [ObservableProperty]
    public partial string SelectedTopGroupPairingMethod { get; set; } = mvm.Tournament.TopGroupPairingMethod switch
    {
        PairingMethod.Cross => Cross,
        PairingMethod.Strongest => Adjacent,
        PairingMethod.Weakest => Weakest,
        _ => Random,
    };
    [ObservableProperty]
    public partial string SelectedPairingMethod { get; set; } = mvm.Tournament.PairingMethod switch
    {
        PairingMethod.Strongest => Adjacent,
        PairingMethod.Weakest => Weakest,
        _ => Random,
    };

    [ObservableProperty]
    public partial string SelectedGroupAdditionMethod { get; set; } = mvm.Tournament.AdditionMethod switch
    {
        PairingMethod.Strongest => Adjacent,
        PairingMethod.Random => Random,
        _ => Weakest,
    };

    [ObservableProperty]
    public partial string Name { get; set; } = mvm.Tournament.Name;

    [ObservableProperty]
    public partial string NumberOfRounds { get; set; } = mvm.Tournament.NumberOfRounds.ToString();

    [ObservableProperty]
    public partial string HandicapReduction { get; set; } = mvm.Tournament.HandicapReduction.ToString();

    [ObservableProperty]
    public partial bool HandicapMaxNine { get; set; } = mvm.Tournament.HandicapMaxNine;

    [ObservableProperty]
    public partial bool AutomaticTopGroupBar { get; set; } = mvm.Tournament.AutomaticTopGroupBar;

    [ObservableProperty]
    public partial string SuperGroupGap { get; set; } = mvm.Tournament.SuperGroupGap.ToString();

    [ObservableProperty]
    public partial string TopGroupBar { get; set; } = mvm.Tournament.TopGroupBar.ToString();

    [ObservableProperty]
    public partial string BottomGroupBar { get; set; } = mvm.Tournament.BottomGroupBar.ToString();

    [ObservableProperty]
    public partial bool AvoidSameCityPairing { get; set; } = mvm.Tournament.AvoidSameCityPairing;

    [ObservableProperty]
    public partial bool HandicapBasedMm { get; set; } = mvm.Tournament.HandicapBasedMm;

    public ObservableCollection<string> TopGroupPairingMethodCollection { get; private set; } =
        [
            Cross,
            Adjacent,
            Weakest,
            Random,
        ];
    public ObservableCollection<string> PairingMethodCollection { get; private set; } =
        [
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
            mvm.Tournament.HandicapMaxNine = HandicapMaxNine;
            mvm.Tournament.AutomaticTopGroupBar = AutomaticTopGroupBar;

            mvm.Tournament.TopGroupPairingMethod = SelectedTopGroupPairingMethod switch
            {
                Cross => PairingMethod.Cross,
                Adjacent => PairingMethod.Strongest,
                Weakest => PairingMethod.Weakest,
                _ => PairingMethod.Random,
            };

            mvm.Tournament.PairingMethod = SelectedPairingMethod switch
            {
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

            if (decimal.TryParse(SuperGroupGap, out decimal sgg))
            {
                mvm.Tournament.SuperGroupGap = sgg;
            }
            else
            {
                MessageBox.Show("Nepodarilo sa nastaviť odskok SuperGroup!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (decimal.TryParse(TopGroupBar, out decimal tgb))
            {
                mvm.Tournament.TopGroupBar = tgb;
            }
            else
            {
                MessageBox.Show("Nepodarilo sa nastaviť hranicu TopGroup!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (decimal.TryParse(BottomGroupBar, out decimal bgb))
            {
                mvm.Tournament.BottomGroupBar = bgb;
            }
            else
            {
                MessageBox.Show("Nepodarilo sa nastaviť hranicu BottomGroup!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            mvm.Tournament.CriteriaSettings.Criterias[0] = C1;
            mvm.Tournament.CriteriaSettings.Criterias[1] = C2;
            mvm.Tournament.CriteriaSettings.Criterias[2] = C3;
            mvm.Tournament.CriteriaSettings.Criterias[3] = C4;
            mvm.Tournament.CriteriaSettings.Criterias[4] = C5;

            window?.Close();
        }
    }
}
