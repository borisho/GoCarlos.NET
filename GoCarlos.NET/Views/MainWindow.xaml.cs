using GoCarlos.NET.Interfaces;
using GoCarlos.NET.ViewModels;
using System.Windows;

namespace GoCarlos.NET.Views;

public partial class MainWindow : Window, ICloseable
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ContextMenu_ContextMenuOpening(object sender, System.Windows.Controls.ContextMenuEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            if(viewModel.SelectedPlayer == null)
            {
                e.Handled = true;
            }
        }
    }
}
