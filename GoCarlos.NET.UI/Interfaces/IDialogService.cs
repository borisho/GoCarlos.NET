using GoCarlos.NET.UI.Enums;

namespace GoCarlos.NET.UI.Interfaces;

/// <summary>
/// To be added
/// </summary>
public interface IDialogService
{
    void Show(string text, string caption, MessageType messageType);
}
