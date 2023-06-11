using CommunityToolkit.Mvvm.ComponentModel;

namespace GoCarlos.NET.UI.ViewModels;

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

    public CheckBoxViewModel(bool _checked, string content) : this(_checked) 
    {
        Content = content;
    }
}
