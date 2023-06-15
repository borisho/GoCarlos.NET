using GoCarlos.NET.UI.Enums;
using GoCarlos.NET.UI.Interfaces;
using System.Windows;

namespace GoCarlos.NET.UI.Services;

/// <inheritdoc cref="IDialogService"/>
public sealed class DialogService : IDialogService
{
    public void Show(string text, string caption, MessageType messageType)
    {
        switch (messageType)
        {
            case MessageType.INFO:
                ShowInfo(text, caption);
                break;
            case MessageType.WARNING:
                ShowWarning(text, caption);
                break;
            case MessageType.ERROR:
                ShowError(text, caption);
                break;
        }
    }

    private static void ShowInfo(string text, string caption)
    {
        MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private static void ShowWarning(string text, string caption)
    {
        MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private static void ShowError(string text, string caption)
    {
        MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
