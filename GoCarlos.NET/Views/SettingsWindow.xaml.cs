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
using GoCarlos.NET.ViewModels;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace GoCarlos.NET;

public partial class SettingsWindow : Window, ICloseable
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        Regex regex = NonNumeric();
        e.Handled = regex.IsMatch(e.Text);
    }

    private void WhiteSpace_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
            e.Handled = true;
    }

    [GeneratedRegex("[^0-9]+")]
    private static partial Regex NonNumeric();

    protected override void OnClosing(CancelEventArgs e)
    {
        if (DataContext is SettingsViewModel svm)
        {
            MainViewModel mvm = svm.mvm;
            mvm.GoToAndRefreshRound(mvm.CurrentRoundNumber);
        }

        base.OnClosing(e);
    }
}
