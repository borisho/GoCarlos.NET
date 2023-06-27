using Microsoft.Extensions.Localization;

namespace GoCarlos.NET.Interfaces;

/// <summary>
/// To be added
/// </summary>
public interface IMenuItemsService
{
    LocalizedString this[string name] { get; }
}
