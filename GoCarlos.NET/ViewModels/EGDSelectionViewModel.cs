// Copyright (C) 2023 Boris Dovčík
// This file is part of GoCarlos.NET.
//
// GoCarlos.NET is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 3, or (at your option) any
// later version.
//
// GoCarlos.NET is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with GoCarlos.NET. If not, see <https://www.gnu.org/licenses/>.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Messages;
using GoCarlos.NET.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace GoCarlos.NET.ViewModels;

public partial class EGDSelectionViewModel : ObservableObject
{
    private readonly ObservableCollection<EGD_Data> players;

    [ObservableProperty]
    public partial EGD_Data? SelectedItem { get; set; }

    public EGDSelectionViewModel(EGD_Data[] eGD_Datas)
    {
        players = [.. eGD_Datas];

        Players = CollectionViewSource.GetDefaultView(players);
        Players.SortDescriptions.Add(new SortDescription(nameof(EGD_Data.Last_Name), ListSortDirection.Ascending));
        Players.SortDescriptions.Add(new SortDescription(nameof(EGD_Data.Name), ListSortDirection.Ascending));
    }

    public ICollectionView Players { get; }

    public static void SendSelectedPlayer(ICloseable window, EGD_Data player)
    {
        WeakReferenceMessenger.Default.Send(new EGDSelectionMessage(player));
        window?.Close();
    }
}
