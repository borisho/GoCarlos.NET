using GoCarlos.NET.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoCarlos.NET;

public partial class MainWindow : Window, ICloseable
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
