using GoCarlos.NET.Enums;
using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Services;
using System.Collections.Generic;

namespace GoCarlos.NET.Factories;

public class LocalizerFactory : ILocalizerFactory
{
    private Dictionary<LocalizerType, ILocalizerService> _services;

    public LocalizerFactory(
        MainViewService mainViewService,
        MenuItemsService menuItemsService
    ) {
        _services = new()
        {
            { LocalizerType.MainViewService, mainViewService },
            { LocalizerType.MenuItemsService, menuItemsService }
        };
    }

    public ILocalizerService GetByQualifier(LocalizerType type) => _services[type];
}
