using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;
using GoCarlos.NET.ViewModels;
using GoCarlos.NET.Views.Api;
using System.ComponentModel;
using System.Windows;

namespace GoCarlos.NET.Views;

/// <inheritdoc cref="IPlayerWindow"/>
public partial class PlayerWindow : Window, IPlayerWindow, ICloseable
{
    public PlayerWindow()
    {
        InitializeComponent();
    }

    public void SetSelectedPlayer(Player player)
    {
        if (DataContext is PlayerViewModel viewModel)
        {
            viewModel.SetSelectedPlayer(player);
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (DataContext is IReferenceCleanup viewModel)
        {
            viewModel.Unregister();
        }

        base.OnClosing(e);
    }
}
