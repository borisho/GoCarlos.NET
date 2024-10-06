﻿using GoCarlos.NET.Interfaces;
using Microsoft.Extensions.Localization;

namespace GoCarlos.NET.Services;

/// <inheritdoc cref="ILocalizerService"/>
public sealed class MenuItemsService(IStringLocalizer<MenuItemsService> localizer) : ILocalizerService
{
    public LocalizedString this[string name] => localizer[name];
}
