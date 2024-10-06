using GoCarlos.NET.Interfaces;
using Microsoft.Extensions.Localization;

namespace GoCarlos.NET.Services;

/// <inheritdoc cref="ILocalizerService"/>
public sealed class MainViewService(IStringLocalizer<MainViewService> localizer) : ILocalizerService
{
    public LocalizedString this[string name] => localizer[name];
}
