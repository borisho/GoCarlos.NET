using GoCarlos.NET.Interfaces;
using System.Text.RegularExpressions;
using System.Windows;

namespace GoCarlos.NET.Views;

/// <summary>
/// Interaction logic for SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : Window, ISettingsWindow, ICloseable
{
    [GeneratedRegex("^[0-9]*$")]
    private static partial Regex IsNumeral();
    [GeneratedRegex("[0-9]*")]
    private static partial Regex ContainsNumeral();

    public SettingsWindow()
    {
        InitializeComponent();
    }

    private void TextBox_PreviewTextInput_OnlyInteger(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        e.Handled = ContainsNumeral().IsMatch(e.Text);
    }

    private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            if (e.DataObject.GetData(typeof(string)) is string text && !IsNumeral().IsMatch(text))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }
}
