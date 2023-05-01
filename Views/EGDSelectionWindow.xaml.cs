using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;
using GoCarlos.NET.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoCarlos.NET;

public partial class EGDSelectionWindow : Window, ICloseable
{
    public EGDSelectionWindow()
    {
        InitializeComponent();
    }

    private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListBoxItem listBoxItem)
        {
            if (DataContext is EGDSelectionViewModel viewModel)
            {
                EGD_Data? selectedItem = listBoxItem.DataContext as EGD_Data;
                if (viewModel.SelectedItem == selectedItem)
                {
                    EGDSelectionViewModel.SendSelectedPlayer(this, selectedItem);
                    e.Handled = false;
                }
            }
        }
    }
}
