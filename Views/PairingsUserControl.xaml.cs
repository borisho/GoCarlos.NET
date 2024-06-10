using GoCarlos.NET.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoCarlos.NET.Views;

public partial class PairingsUserControl : UserControl
{
    public PairingsUserControl()
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
                viewModel.SelectedPairing = dataGridCell.DataContext as PairingViewModel;
                if (viewModel.EditPairingResultCommand.CanExecute(viewModel.SelectedPairing))
                {
                    viewModel.EditPairingResultCommand.Execute(viewModel.SelectedPairing);
                    e.Handled = true;
                }
            }
        }
    }
}
