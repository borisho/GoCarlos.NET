using GoCarlos.NET.Interfaces;
using GoCarlos.NET.ViewModels;
using System.Windows;

namespace GoCarlos.NET.Views;

/// <inheritdoc cref="IPlayerWindow"/>
public partial class PlayerWindow : Window, IPlayerWindow, ICloseable
{
    public PlayerWindow()
    {
        InitializeComponent();
    }

    public void SetSelectedPlayer(IPlayer player)
    {
        if (DataContext is PlayerViewModel viewModel)
        {
            viewModel.SetSelectedPlayer(player);
        }
    }
}
