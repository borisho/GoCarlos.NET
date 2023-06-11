using GoCarlos.NET.UI.Enums;
using GoCarlos.NET.UI.Models;

namespace GoCarlos.NET.UI.Interfaces;

/// <summary>
/// To be added
/// </summary>
public interface IWindowService
{
    void Show(Windows type);
    void ShowEgdSelectionWindow(EgdData[] egdDatas);
}
