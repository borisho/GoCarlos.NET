using GoCarlos.NET.Interfaces;
using GoCarlos.NET.ViewModels;
using GoCarlos.NET.Views;
using Microsoft.Extensions.Localization;
using System.Windows;

namespace GoCarlos.NET.Services;

public sealed class WindowService : IWindowService
{
    private readonly IStringLocalizer<WindowService> _localizer = null!;

    public WindowService(IStringLocalizer<WindowService> localizer)
    {
        _localizer = localizer;
    }

    public void Show<T>(T viewModel)
    {
        switch (viewModel)
        {
            case AddPlayerViewModel apvm:
                AddPlayerWindow(apvm);
                break;
            case null:
                ErrorMessageBox("Null");
                break;
            default:
                ErrorMessageBox(viewModel.GetType().Name);
                break;
        }
    }

    private static void AddPlayerWindow(AddPlayerViewModel viewModel)
    {
        var window = new AddPlayerWindow()
        {
            DataContext = viewModel,
        };

        window.Show();
    }

    private void ErrorMessageBox(string name)
    {
        MessageBox.Show(_localizer["Message"] + name, _localizer["Error"], MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
