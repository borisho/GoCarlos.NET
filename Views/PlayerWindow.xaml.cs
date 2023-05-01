using GoCarlos.NET.Interfaces;
using System.Windows;

namespace GoCarlos.NET;

public partial class PlayerWindow : Window, ICloseable
{
    public PlayerWindow()
    {
        InitializeComponent();
    }
}
