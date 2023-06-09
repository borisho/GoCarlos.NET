using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace GoCarlos.NET.ViewModels;

public partial class PlayerControlViewModel : ObservableObject
{
    [ObservableProperty]
    private string title, pin, lastName, firstName, gor, grade, countryCode, club;

    public PlayerControlViewModel(int roundNumber)
    {
        title = string.Empty;
        pin = string.Empty;
        lastName = string.Empty;
        firstName = string.Empty;
        gor = string.Empty;
        grade = string.Empty;
        countryCode = string.Empty;
        club = string.Empty;

        for (int i = 0; i < roundNumber; i++)
        {
            CheckBoxes.Add(new(true, "Round: " + i));
        }
    }

    public ObservableCollection<CheckBoxViewModel> CheckBoxes { get; } = new();

    [RelayCommand]
    public void SearchbyPin()
    {

    }

    [RelayCommand]
    public void SearchByData()
    {

    }
}
