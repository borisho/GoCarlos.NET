using GoCarlos.NET.Enums;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.ViewModels;
using GoCarlos.NET.Views;
using Microsoft.Extensions.Localization;
using System.Windows;

namespace GoCarlos.NET.Services;

public sealed class WindowService : IWindowService
{
    private readonly IStringLocalizer<WindowService> localizer;
    private readonly ITournament tournament;

    public WindowService(IStringLocalizer<WindowService> localizer, ITournament tournament)
    {
        this.localizer = localizer;
        this.tournament = tournament;
    }

    public LocalizedString this[string name] => localizer[name];

    public void Show(Windows type)
    {
        switch (type)
        {
            case Windows.AddPlayerWindow:
                AddPlayerWindow();
                break;
            default:
                ErrorMessageBox();
                break;
        }
    }

    private void AddPlayerWindow()
    {
        var window = new AddPlayerWindow()
        {
            DataContext = new AddPlayerViewModel(localizer["AddPlayerWindow"], tournament.Rounds),
        };

        window.Show();
    }

    private void ErrorMessageBox()
    {
        MessageBox.Show(localizer["Message"], localizer["Error"], MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
