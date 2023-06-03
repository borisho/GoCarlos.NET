using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Interfaces;
using System.Collections.ObjectModel;

namespace GoCarlos.NET.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private IMenuItemsService menuItemsService;

    [ObservableProperty]
    private MenuItemViewModel menuItemViewModel;

    public MainViewModel(IMenuItemsService menuItemsService) {
        this.menuItemsService = menuItemsService;
        menuItemViewModel = new MenuItemViewModel("Root");

        GenerateMenuItems(5);
    }

    private void GenerateMenuItems(int rounds)
    {
        var rootItems = MenuItemViewModel.Items;

        rootItems.Add(new MenuItemViewModel(menuItemsService["Settings"])
        {
            Items = {
                new MenuItemViewModel(menuItemsService["NewTournament"]),
                new MenuItemViewModel(menuItemsService["LoadTournament"]),
                new MenuItemViewModel(menuItemsService["SaveTournament"]),
                new MenuItemViewModel(null),
                new MenuItemViewModel(menuItemsService["Settings"]),
                new MenuItemViewModel(null),
                new MenuItemViewModel(menuItemsService["Exit"]),
            }
        });

        rootItems.Add(new MenuItemViewModel(menuItemsService["AddPlayer"]));
        rootItems.Add(new MenuItemViewModel(menuItemsService["Rounds"])
        {
            Items = {
                new MenuItemViewModel(menuItemsService["IncrementRound"]),
                new MenuItemViewModel(menuItemsService["DecrementRound"]),
                new MenuItemViewModel(null),
                new MenuItemViewModel(menuItemsService["GoToRound"])
                {
                    Items = GenerateRounds(rounds),
                },
                new MenuItemViewModel(null),
                new MenuItemViewModel(menuItemsService["CountCurrentRound"]),
            }
        });;

        rootItems.Add(new MenuItemViewModel(menuItemsService["Pairings"])
        {
            Items = {
                new MenuItemViewModel(menuItemsService["MakePairings"]),
                new MenuItemViewModel(menuItemsService["DropPairings"]),
            }
        });

        rootItems.Add(new MenuItemViewModel(menuItemsService["Exports"])
        {
            Items = {
                new MenuItemViewModel(menuItemsService["ExportWallist"]),
                new MenuItemViewModel(menuItemsService["ExportPairings"]),
                new MenuItemViewModel(menuItemsService["ExportEGD"]),
            }
        });

        rootItems.Add(new MenuItemViewModel(menuItemsService["Debug"])
        {
            Items = {
                new MenuItemViewModel(menuItemsService["Generate10"]),
                new MenuItemViewModel(menuItemsService["Generate100"]),
            }
        });
    }

    private ObservableCollection<MenuItemViewModel> GenerateRounds(int rounds)
    {
        ObservableCollection<MenuItemViewModel> items = new();

        for (int i = 1; i <= rounds; i++)
        {
            items.Add(new MenuItemViewModel(i.ToString()));
        }

        return items;
    }
}
