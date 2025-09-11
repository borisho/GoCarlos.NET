using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.Enums;
using GoCarlos.NET.Factories.Api;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Messages;
using GoCarlos.NET.Services.Api;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GoCarlos.NET.ViewModels;

public partial class MenuViewModel : ObservableObject
{
    private readonly ITournament tournament;
    private readonly IDialogService dialogService;
    private readonly IWindowService windowService;
    private readonly ILocalizerService localizerService;

    private readonly MenuItemViewModel goToRoundRoot;

    public MenuViewModel(ITournament tournament,
        IDialogService dialogService,
        IWindowService windowService,
        ILocalizerFactory localizerFactory)
    {
        this.tournament = tournament;
        this.dialogService = dialogService;
        this.windowService = windowService;

        // Get localizer service from factory
        localizerService = localizerFactory.GetByQualifier(LocalizerType.MenuItemsService);

        goToRoundRoot = new MenuItemViewModel(localizerService["GoToRound"]);

        GenerateMenuItems();
        GenerateGoToRound(tournament.Settings.GeneralSettings.NumberOfRounds);
    }

    public ObservableCollection<MenuItemViewModel> Items { get; } = [];

    [RelayCommand]
    public void NewTournament()
    {
        if (dialogService.Show(localizerService["CreateTournamentMessage"], localizerService["CreateTournamentTitle"], MessageType.WARNING_YES_NO)
            == System.Windows.MessageBoxResult.Yes)
        {
            tournament.Reset();
        }
    }

    [RelayCommand]
    public void LoadTournament()
    {
        Debug.WriteLine("LoadTournament Command");
    }

    [RelayCommand]
    public void SaveTournament()
    {
        Debug.WriteLine("SaveTournament Command");
    }

    [RelayCommand]
    public void Settings()
    {
        windowService.ShowSettingsWindow();
    }

    [RelayCommand]
    public void Exit()
    {
        if (dialogService.Show(localizerService["ExitTournamentMessage"], localizerService["ExitTournamentTitle"], MessageType.WARNING_YES_NO)
            == System.Windows.MessageBoxResult.Yes)
        {
            windowService.Shutdown();
        }
    }

    [RelayCommand]
    public void AddPlayer()
    {
        windowService.ShowAddPlayerWindow();
    }

    [RelayCommand]
    public void IncrementRound()
    {
        Debug.WriteLine("IncrementRound Command");
        WeakReferenceMessenger.Default.Send(EmptyMessage.INSTANCE, ITournament.TOKEN_ROUND_CHANGE);
    }

    [RelayCommand]
    public void DecrementRound()
    {
        Debug.WriteLine("DecrementRound Command");
    }

    [RelayCommand]
    public void GoToRound(int i)
    {
        Debug.WriteLine("Go to round: " + i);
    }

    [RelayCommand]
    public void CountCurrentRound()
    {
        Debug.WriteLine("CountCurrentRound Command");
    }

    [RelayCommand]
    public void MakePairings()
    {
        Debug.WriteLine("MakePairings Command");
    }

    [RelayCommand]
    public void DropPairings()
    {
        Debug.WriteLine("DropPairings Command");
    }

    [RelayCommand]
    public void ExportWallist()
    {
        Debug.WriteLine("ExportWallist Command");
    }

    [RelayCommand]
    public void ExportPairings()
    {
        Debug.WriteLine("ExportPairings Command");
    }

    [RelayCommand]
    public void ExportEGD()
    {
        Debug.WriteLine("ExportEGD Command");
    }

    [RelayCommand]
    public void Generate10()
    {
        Debug.WriteLine("Generate10 Command");
    }

    [RelayCommand]
    public void Generate100()
    {
        Debug.WriteLine("Generate100 Command");
    }

    public void GenerateGoToRound(int rounds)
    {
        goToRoundRoot.Items.Clear();

        for (int i = 1; i <= rounds; i++)
        {
            goToRoundRoot.Items.Add(new MenuItemViewModel(i.ToString(), GoToRoundCommand, i));
        }
    }

    private void GenerateMenuItems()
    {
        Items.Add(new MenuItemViewModel(localizerService["Settings"])
        {
            Items = {
                new MenuItemViewModel(localizerService["NewTournament"], NewTournamentCommand),
                new MenuItemViewModel(localizerService["LoadTournament"], LoadTournamentCommand),
                new MenuItemViewModel(localizerService["SaveTournament"], SaveTournamentCommand),
                new MenuItemViewModel(null),
                new MenuItemViewModel(localizerService["Settings"], SettingsCommand),
                new MenuItemViewModel(null),
                new MenuItemViewModel(localizerService["Exit"], ExitCommand),
            }
        });

        Items.Add(new MenuItemViewModel(localizerService["AddPlayer"], AddPlayerCommand));
        Items.Add(new MenuItemViewModel(localizerService["Rounds"])
        {
            Items = {
                new MenuItemViewModel(localizerService["IncrementRound"], IncrementRoundCommand),
                new MenuItemViewModel(localizerService["DecrementRound"], DecrementRoundCommand),
                new MenuItemViewModel(null),
                goToRoundRoot,
                new MenuItemViewModel(null),
                new MenuItemViewModel(localizerService["CountCurrentRound"], CountCurrentRoundCommand),
            }
        }); ;

        Items.Add(new MenuItemViewModel(localizerService["Pairings"])
        {
            Items = {
                new MenuItemViewModel(localizerService["MakePairings"], MakePairingsCommand),
                new MenuItemViewModel(localizerService["DropPairings"], DropPairingsCommand),
            }
        });

        Items.Add(new MenuItemViewModel(localizerService["Exports"])
        {
            Items = {
                new MenuItemViewModel(localizerService["ExportWallist"], ExportWallistCommand),
                new MenuItemViewModel(localizerService["ExportPairings"], ExportPairingsCommand),
                new MenuItemViewModel(localizerService["ExportEGD"], ExportEGDCommand),
            }
        });

        Items.Add(new MenuItemViewModel(localizerService["Debug"])
        {
            Items = {
                new MenuItemViewModel(localizerService["Generate10"], Generate10Command),
                new MenuItemViewModel(localizerService["Generate100"], Generate100Command),
            }
        });
    }
}
