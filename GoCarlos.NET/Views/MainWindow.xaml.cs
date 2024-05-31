using GoCarlos.NET.Interfaces;
using GoCarlos.NET.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace GoCarlos.NET.Views;

public partial class MainWindow : Window, ICloseable
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            if (sender is DataGrid dataGrid)
            {
                if ((dataGrid.Name == "Wallist" && viewModel.SelectedPlayer == null) ||
                   (dataGrid.Name == "Pairings" && viewModel.SelectedPairing == null))
                {
                    e.Handled = true;
                }
            }
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
