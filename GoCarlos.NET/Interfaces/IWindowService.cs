using GoCarlos.NET.Models;

namespace GoCarlos.NET.Interfaces;

/// <summary>
/// Service for creating WPF windows
/// </summary>
public interface IWindowService
{
    /// <summary>
    /// Show <seealso cref="IPlayerWindow" /> with option to add player to tournament
    /// </summary>
    void ShowAddPlayerWindow();

    /// <summary>
    /// Show <seealso cref="IPlayerWindow" /> with option to add player to tournament.
    /// <para>
    /// Pass <paramref name="param"/> value to AddOneMorePlayer CheckBox.
    /// </para>
    /// </summary>
    /// <param name="param">Value of AddOneMorePlayer CheckBox</param>
    void ShowAddPlayerWindowWithParam(bool param);

    /// <summary>
    /// Show <seealso cref="IPlayerWindow" /> and populate with data of <paramref name="player"/>
    /// </summary>
    /// <param name="player">Selected Player</param>
    void ShowPlayerWindow(Player player);

    /// <summary>
    /// Show <seealso cref="IEgdSelectionWindow" /> and populate with <paramref name="egdDatas"/>
    /// </summary>
    /// <param name="egdDatas">Players retrieved from European Go Database</param>
    void ShowEgdSelectionWindow(EgdData[] egdDatas);

    /// <summary>
    /// Show as dialog <seealso cref="ISettingsWindow" />
    /// </summary>
    void ShowSettingsWindow();

    /// <summary>
    /// Shutdown application
    /// </summary>
    void Shutdown();
}
