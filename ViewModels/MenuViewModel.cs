﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoCarlos.NET.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GoCarlos.NET.ViewModels;

public partial class MenuViewModel : ObservableObject
{
    private readonly IMenuItemsService menuItemsService;
    private readonly MenuItemViewModel goToRoundRoot;

    public MenuViewModel(IMenuItemsService menuItemsService)
    {
        this.menuItemsService = menuItemsService;
        Items = new ObservableCollection<MenuItemViewModel>();

        goToRoundRoot = new MenuItemViewModel(menuItemsService["GoToRound"]);

        GenerateMenuItems();
        GenerateGoToRound(5);
    }

    public ObservableCollection<MenuItemViewModel> Items { get; set; }

    [RelayCommand]
    public void NewTournament()
    {
        Debug.WriteLine("NewTournament Command");
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
        Debug.WriteLine("Settings Command");
    }

    [RelayCommand]
    public void Exit()
    {
        Debug.WriteLine("Exit Command");
    }

    [RelayCommand]
    public void AddPlayer()
    {
        Debug.WriteLine("AddPlayer Command");
    }

    [RelayCommand]
    public void IncrementRound()
    {
        Debug.WriteLine("IncrementRound Command");
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
        Items.Add(new MenuItemViewModel(menuItemsService["Settings"])
        {
            Items = {
                new MenuItemViewModel(menuItemsService["NewTournament"], NewTournamentCommand),
                new MenuItemViewModel(menuItemsService["LoadTournament"], LoadTournamentCommand),
                new MenuItemViewModel(menuItemsService["SaveTournament"], SaveTournamentCommand),
                new MenuItemViewModel(null),
                new MenuItemViewModel(menuItemsService["Settings"], SettingsCommand),
                new MenuItemViewModel(null),
                new MenuItemViewModel(menuItemsService["Exit"], ExitCommand),
            }
        });

        Items.Add(new MenuItemViewModel(menuItemsService["AddPlayer"], AddPlayerCommand));
        Items.Add(new MenuItemViewModel(menuItemsService["Rounds"])
        {
            Items = {
                new MenuItemViewModel(menuItemsService["IncrementRound"], IncrementRoundCommand),
                new MenuItemViewModel(menuItemsService["DecrementRound"], DecrementRoundCommand),
                new MenuItemViewModel(null),
                goToRoundRoot,
                new MenuItemViewModel(null),
                new MenuItemViewModel(menuItemsService["CountCurrentRound"], CountCurrentRoundCommand),
            }
        }); ;

        Items.Add(new MenuItemViewModel(menuItemsService["Pairings"])
        {
            Items = {
                new MenuItemViewModel(menuItemsService["MakePairings"], MakePairingsCommand),
                new MenuItemViewModel(menuItemsService["DropPairings"], DropPairingsCommand),
            }
        });

        Items.Add(new MenuItemViewModel(menuItemsService["Exports"])
        {
            Items = {
                new MenuItemViewModel(menuItemsService["ExportWallist"], ExportWallistCommand),
                new MenuItemViewModel(menuItemsService["ExportPairings"], ExportPairingsCommand),
                new MenuItemViewModel(menuItemsService["ExportEGD"], ExportEGDCommand),
            }
        });

        Items.Add(new MenuItemViewModel(menuItemsService["Debug"])
        {
            Items = {
                new MenuItemViewModel(menuItemsService["Generate10"], Generate10Command),
                new MenuItemViewModel(menuItemsService["Generate100"], Generate100Command),
            }
        });
    }
}