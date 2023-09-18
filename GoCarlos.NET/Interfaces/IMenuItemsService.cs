using Microsoft.Extensions.Localization;

namespace GoCarlos.NET.Interfaces;

/// <summary>
/// Provides localization for main window menu
/// </summary>
public interface IMenuItemsService
{
    LocalizedString this[string name] { get; }
}
