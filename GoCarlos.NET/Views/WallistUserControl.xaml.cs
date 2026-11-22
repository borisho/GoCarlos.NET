using GoCarlos.NET.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoCarlos.NET.Views;

public partial class WallistUserControl : UserControl
{
    public WallistUserControl()
    {
        InitializeComponent();
    }

    private void DataGrid_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is ScrollViewer)
        {
            ((DataGrid)sender).UnselectAll();
        }
    }

    private void DataGridCell_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            if (sender is DataGridCell dataGridCell)
            {
                var selectedPlayer = dataGridCell.DataContext as PlayerViewModel;
                if (viewModel.EditPlayerGroupCommand.CanExecute(selectedPlayer))
                {
                    viewModel.SelectedPlayer = selectedPlayer;
                    viewModel.EditPlayerGroupCommand.Execute(selectedPlayer);
                }
            }
        }
    }

    private void WallList_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DataGrid grid || DataContext is not MainViewModel viewModel)
        {
            return;
        }

        var row = ItemsControl.ContainerFromElement(grid, e.OriginalSource as DependencyObject) as DataGridRow;

        if (row?.Item is PlayerViewModel player)
        {
            grid.SelectedItem = player;
            viewModel.SelectedPlayer = player;
        }
    }
}
