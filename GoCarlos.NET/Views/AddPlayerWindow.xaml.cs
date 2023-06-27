using GoCarlos.NET.Interfaces;
using GoCarlos.NET.ViewModels;
using System.Windows;

namespace GoCarlos.NET.Views;
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

    public void GenerateCheckBoxes(int rounds)
    {
        if (DataContext is AddPlayerViewModel viewModel)
        {
            viewModel.GenerateCheckBoxes(rounds);
        }
    }
}
