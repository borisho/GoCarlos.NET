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

using GoCarlos.NET.ViewModels;
using System.Windows;

namespace GoCarlos.NET;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new MainWindow()
        {
            DataContext = new MainViewModel()
        };

        MainWindow.Show();

        base.OnStartup(e);
    }
}
