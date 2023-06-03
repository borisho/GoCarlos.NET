using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace GoCarlos.NET.ViewModels;

public partial class MenuItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string? header;

    public MenuItemViewModel(string? header)
    {
        Header = header;
        Items = new ObservableCollection<MenuItemViewModel>();
    }

    public ObservableCollection<MenuItemViewModel> Items { get; set; }
}
