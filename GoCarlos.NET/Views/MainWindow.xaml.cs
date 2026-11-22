using GoCarlos.NET.Interfaces;
using System.Windows;

namespace GoCarlos.NET;

public partial class MainWindow : Window, ICloseable
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
