﻿using GoCarlos.NET.Interfaces;
using Microsoft.Extensions.Localization;

namespace GoCarlos.NET.Services;

/// <inheritdoc cref="IMenuItemsService"/>
public sealed class MenuItemsService : IMenuItemsService
{
    private readonly IStringLocalizer<MenuItemsService> _localizer = null!;

    public MenuItemsService(IStringLocalizer<MenuItemsService> localizer) =>
        _localizer = localizer;

    public LocalizedString this[string name] => _localizer[name];
}