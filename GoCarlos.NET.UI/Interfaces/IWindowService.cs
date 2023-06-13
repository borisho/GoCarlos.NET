using GoCarlos.NET.UI.Enums;
using GoCarlos.NET.UI.Models;

namespace GoCarlos.NET.UI.Interfaces;

/// <summary>
/// To be added
/// </summary>
public interface IWindowService
{
    /// <summary>
    /// To be added
    /// </summary>
    void Show(Windows type);

    /// <summary>
    /// To be added
    /// </summary>
    void ShowEgdSelectionWindow(EgdData[] egdDatas);
}
