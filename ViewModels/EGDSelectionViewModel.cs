using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GoCarlos.NET.ViewModels;

public class EGDSelectionViewModel : ObservableObject
{
    private readonly AddOrEditPlayerViewModel addPlayerViewModel;
    private readonly ObservableCollection<EGD_Data> players;
    private EGD_Data? selectedItem;

    public EGDSelectionViewModel(AddOrEditPlayerViewModel addPlayerViewModel, EGD_Data[] eGD_Datas)
    {

        this.addPlayerViewModel = addPlayerViewModel;
        players = new ObservableCollection<EGD_Data>();

        foreach (EGD_Data data in eGD_Datas)
        {
            players.Add(data);
        }
    }

    public IEnumerable<EGD_Data> Players { get => players; }
    public EGD_Data SelectedItem
    {
        get => selectedItem!;
        set => selectedItem = value;
    }

    public void SendSelectedPlayer(ICloseable window, EGD_Data player)
    {
        addPlayerViewModel.Data = player;

        window?.Close();
    }
}
