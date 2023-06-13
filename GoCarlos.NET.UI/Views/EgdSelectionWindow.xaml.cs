using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.UI.Interfaces;
using GoCarlos.NET.UI.Messages;
using GoCarlos.NET.UI.Models;
using GoCarlos.NET.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoCarlos.NET.UI.Views;

public partial class EgdSelectionWindow : Window, ICloseable
{
    public EgdSelectionWindow()
    {
        InitializeComponent();
    }

    public void AddPlayers(EgdData[] egdDatas)
    {
        if (DataContext is EgdSelectionViewModel viewModel)
        {
            viewModel.AddPlayers(egdDatas);
        }
    }

    private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListViewItem listViewItem)
        {
            if (DataContext is EgdSelectionViewModel viewModel)
            {
                EgdData? selectedItem = listViewItem.DataContext as EgdData;
                
                if (selectedItem is not null && listViewItem.IsSelected)
                {
                    WeakReferenceMessenger.Default.Send<EgdMessage>(new (selectedItem));

                    Close();
                }
            }
        }
    }
}
