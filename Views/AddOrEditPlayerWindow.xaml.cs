using GoCarlos.NET.Interfaces;
using System.Windows;

namespace GoCarlos.NET;

public partial class AddOrEditPlayerWindow : Window, ICloseable
{
    public AddOrEditPlayerWindow()
    {
        InitializeComponent();
    }
}
