using GoCarlos.NET.Interfaces;
using Microsoft.Extensions.Localization;

namespace GoCarlos.NET.Services;

/// <inheritdoc cref="IMenuItemsService"/>
public sealed class MenuItemsService(IStringLocalizer<MenuItemsService> localizer) : IMenuItemsService
{
    public LocalizedString this[string name] => localizer[name];
}
