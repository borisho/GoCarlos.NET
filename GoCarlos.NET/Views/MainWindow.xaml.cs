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
using System.Windows;

namespace GoCarlos.NET;

public partial class MainWindow : Window, ICloseable
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void LicenseMenuItem_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "Copyright (C) 2023 Boris Dovčík\n\n" +
            "This program is free software: you can redistribute it and/or modify " +
            "it under the terms of the GNU General Public License as published by " +
            "the Free Software Foundation, either version 3 of the License, or " +
            "(at your option) any later version.\n\n" +
            "This program is distributed in the hope that it will be useful, " +
            "but WITHOUT ANY WARRANTY; without even the implied warranty of " +
            "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the " +
            "GNU General Public License for more details\n\n" +
            "The complete license is available in the LICENSE file. If not, see <https://www.gnu.org/licenses/>",
            "Licencia",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}
