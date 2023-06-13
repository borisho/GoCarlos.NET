using Microsoft.Extensions.Localization;

namespace GoCarlos.NET.UI.Interfaces;

/// <summary>
/// To be added
/// </summary>
public interface IMenuItemsService
{
    /// <summary>
    /// To be added
    /// </summary>
    LocalizedString this[string name] { get; }
}
