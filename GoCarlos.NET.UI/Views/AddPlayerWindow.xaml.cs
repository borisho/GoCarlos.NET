using GoCarlos.NET.UI.Interfaces;
using GoCarlos.NET.UI.ViewModels;
using System.Windows;

namespace GoCarlos.NET.UI.Views;
public partial class AddPlayerWindow : Window, ICloseable
{
    public AddPlayerWindow()
    {
        InitializeComponent();
    }

    public void AddOneMore(bool param)
    {
        if (DataContext is AddPlayerViewModel viewModel)
        {
            viewModel.AddOneMore = param;
        }
    }
}
