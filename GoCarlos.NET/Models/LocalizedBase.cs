using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;

namespace GoCarlos.NET.Models;

public class LocalizedBase
{
    protected readonly IStringLocalizer _localizer;

    public LocalizedBase(string id, string baseName, string location)
    {
        Id = id;
        IServiceProvider serviceProvider = App.AppHost!.Services;
        _localizer = serviceProvider.GetRequiredService<IStringLocalizerFactory>().Create(baseName, location);
    }

    public string Id { get; set; }
}
