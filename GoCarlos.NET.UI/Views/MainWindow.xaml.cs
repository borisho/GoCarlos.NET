using GoCarlos.NET.UI.Interfaces;
using System.Windows;

namespace GoCarlos.NET.UI.Views;

public partial class MainWindow : Window, ICloseable
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
