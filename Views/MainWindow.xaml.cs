using GoCarlos.NET.Interfaces;
using System.Windows;

namespace GoCarlos.NET.Views;

public partial class MainWindow : Window, ICloseable
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
