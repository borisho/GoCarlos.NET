using Microsoft.Extensions.Localization;

namespace GoCarlos.NET.Interfaces;

public interface IMenuItemsService
{
    LocalizedString this[string name] { get; }
}
