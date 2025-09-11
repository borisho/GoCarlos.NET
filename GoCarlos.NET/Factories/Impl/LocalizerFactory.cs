using GoCarlos.NET.Enums;
using GoCarlos.NET.Factories.Api;
using GoCarlos.NET.Services.Api;
using GoCarlos.NET.Services.Impl;
using System.Collections.Generic;

namespace GoCarlos.NET.Factories.Impl;

public class LocalizerFactory : ILocalizerFactory
{
    private readonly Dictionary<LocalizerType, ILocalizerService> _services;

    public LocalizerFactory(
        MainViewLocalizerService mainViewLocalizerService,
        MenuItemLocalizerService menuItemLocalizerService
    ) {
        _services = new()
        {
            { LocalizerType.MainViewService, mainViewLocalizerService },
            { LocalizerType.MenuItemsService, menuItemLocalizerService }
        };
    }

    public ILocalizerService GetByQualifier(LocalizerType type) => _services[type];
}
