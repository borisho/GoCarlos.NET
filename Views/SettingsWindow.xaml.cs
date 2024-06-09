using GoCarlos.NET.Interfaces;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace GoCarlos.NET;

public partial class SettingsWindow : Window, ICloseable
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        Regex regex = NonNumeric();
        e.Handled = regex.IsMatch(e.Text);
    }

    private void WhiteSpace_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
            e.Handled = true;
    }

    [GeneratedRegex("[^0-9]+")]
    private static partial Regex NonNumeric();
}
