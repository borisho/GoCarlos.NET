using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GoCarlos.NET.ViewModels;

public partial class MenuItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int? commandParameter;

    [ObservableProperty]
    private string? header;

    [ObservableProperty]
    private ICommand? command;

    public MenuItemViewModel()
    {
        commandParameter = null;

        Header = "";
        Command = null;
    }

    public MenuItemViewModel(string? header) : this()
    {
        Header = header;
    }

    public MenuItemViewModel(string? header, ICommand command) : this(header)
    {
        Command = command; 
    }

    public MenuItemViewModel(string? header, ICommand command, int commandParameter) : this(header, command)
    {
        CommandParameter = commandParameter;
    }

    public ObservableCollection<MenuItemViewModel> Items { get; set; } = new();
}
