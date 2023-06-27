using GoCarlos.NET.Models;

namespace GoCarlos.NET.Interfaces;

/// <summary>
/// To be added
/// </summary>
public interface IWindowService
{
    /// <summary>
    /// To be added
    /// </summary>
    void ShowAddPlayerWindow();

    /// <summary>
    /// To be added
    /// </summary>
    void ShowAddPlayerWindowWithParam(bool param);

    /// <summary>
    /// To be added
    /// </summary>
    void ShowEgdSelectionWindow(EgdData[] egdDatas);
}
