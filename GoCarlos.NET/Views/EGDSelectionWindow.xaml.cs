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

using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;
using GoCarlos.NET.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoCarlos.NET;

public partial class EGDSelectionWindow : Window, ICloseable
{
    public EGDSelectionWindow()
    {
        InitializeComponent();
    }

    private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListBoxItem listBoxItem)
        {
            if (DataContext is EGDSelectionViewModel viewModel)
            {
                EGD_Data? selectedItem = listBoxItem.DataContext as EGD_Data;
                if (viewModel.SelectedItem == selectedItem && selectedItem is not null)
                {
                    EGDSelectionViewModel.SendSelectedPlayer(this, selectedItem);
                    e.Handled = false;
                }
            }
        }
    }
}
