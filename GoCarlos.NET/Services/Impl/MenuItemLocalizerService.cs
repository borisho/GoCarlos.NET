using GoCarlos.NET.Services.Api;
using Microsoft.Extensions.Localization;

namespace GoCarlos.NET.Services.Impl;

/// <inheritdoc cref="ILocalizerService"/>
public sealed class MenuItemLocalizerService(IStringLocalizer<MenuItemLocalizerService> localizer) : ILocalizerService
{
    public LocalizedString this[string name] => localizer[name];
}
