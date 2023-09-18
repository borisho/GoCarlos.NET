using GoCarlos.NET.Interfaces;
using System.Windows;

namespace GoCarlos.NET.Views;

/// <summary>
/// Interaction logic for SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : Window, ISettingsWindow, ICloseable
{
    public SettingsWindow()
    {
        InitializeComponent();
    }
}
