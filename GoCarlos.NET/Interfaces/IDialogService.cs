using GoCarlos.NET.Enums;
using System.Windows;

namespace GoCarlos.NET.Interfaces;

/// <summary>
/// To be added
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// To be added
    /// </summary>
    MessageBoxResult Show(string text, string caption, MessageType messageType);
}
