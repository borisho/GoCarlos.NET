using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;

namespace GoCarlos.NET.ViewModels;

public partial class CheckBoxViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _checked;

    [ObservableProperty]
    private string content;

    public CheckBoxViewModel()
    {
        Checked = false;
        Content = string.Empty;
    }

    public CheckBoxViewModel(bool _checked) : this()
    {
        Checked = _checked;
    }

    public CheckBoxViewModel(bool _checked, string text, int round) : this(_checked)
    {
        StringBuilder builder = new(text);
        builder.Append(' ');
        builder.Append(round);
        builder.Append(':');

        Content = builder.ToString();
        Round = round;
    }

    public int Round { get; } = 0;
}
