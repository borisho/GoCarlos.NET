using GoCarlos.NET.Interfaces;
using GoCarlos.NET.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace GoCarlos.NET.Views;

/// <inheritdoc cref="IPlayerWindow"/>
public partial class AddPlayerWindow : Window, IPlayerWindow, ICloseable
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

    protected override void OnClosing(CancelEventArgs e)
    {
        if (DataContext is IReferenceCleanup viewModel)
        {
            viewModel.Unregister();
        }

        base.OnClosing(e);
    }
}
