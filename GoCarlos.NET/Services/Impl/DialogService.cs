using GoCarlos.NET.Enums;
using GoCarlos.NET.Services.Api;
using System.Windows;

namespace GoCarlos.NET.Services.Impl;

/// <inheritdoc cref="IDialogService"/>
public sealed class DialogService : IDialogService
{
    public MessageBoxResult Show(string text, string caption, MessageType messageType) => messageType switch
    {
        MessageType.INFO => ShowInfo(text, caption),
        MessageType.INFO_YES_NO => ShowInfoYesNo(text, caption),
        MessageType.INFO_YES_NO_CANCEL => ShowInfoYesNoCancel(text, caption),
        MessageType.WARNING => ShowWarning(text, caption),
        MessageType.WARNING_YES_NO => ShowWarningYesNo(text, caption),
        MessageType.WARNING_YES_NO_CANCEL => ShowWarningYesNoCancel(text, caption),
        MessageType.ERROR => ShowError(text, caption),
        MessageType.ERROR_YES_NO => ShowErrorYesNo(text, caption),
        MessageType.ERROR_YES_NO_CANCEL => ShowErrorYesNoCancel(text, caption),
        MessageType.QUESTION => ShowQuestion(text, caption),
        MessageType.QUESTION_YES_NO => ShowQuestionYesNo(text, caption),
        MessageType.QUESTION_YES_NO_CANCEL => ShowQuestionYesNoCancel(text, caption),
        MessageType.NONE => ShowNone(text, caption),
        MessageType.NONE_YES_NO => ShowNoneYesNo(text, caption),
        MessageType.NONE_YES_NO_CANCEL => ShowNoneYesNoCancel(text, caption),
        _ => ShowNone(text, caption),
    };

    private static MessageBoxResult ShowInfo(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private static MessageBoxResult ShowInfoYesNo(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Information);
    }

    private static MessageBoxResult ShowInfoYesNoCancel(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
    }

    private static MessageBoxResult ShowWarning(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private static MessageBoxResult ShowWarningYesNo(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);
    }

    private static MessageBoxResult ShowWarningYesNoCancel(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
    }

    private static MessageBoxResult ShowError(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private static MessageBoxResult ShowErrorYesNo(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Error);
    }

    private static MessageBoxResult ShowErrorYesNoCancel(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Error);
    }

    private static MessageBoxResult ShowQuestion(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Question);
    }

    private static MessageBoxResult ShowQuestionYesNo(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
    }

    private static MessageBoxResult ShowQuestionYesNoCancel(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
    }

    private static MessageBoxResult ShowNone(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.None);
    }

    private static MessageBoxResult ShowNoneYesNo(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.None);
    }

    private static MessageBoxResult ShowNoneYesNoCancel(string text, string caption)
    {
        return MessageBox.Show(text, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.None);
    }
}
