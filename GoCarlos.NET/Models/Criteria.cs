using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;

namespace GoCarlos.NET.Models;

public class Criteria
{
    private IStringLocalizer _localizer;

    public Criteria(string id)
    {
        Id = id;
        IServiceProvider serviceProvider = App.AppHost!.Services;
        _localizer = serviceProvider.GetRequiredService<IStringLocalizerFactory>().Create("SettingsWindow", "GoCarlos.NET");
    }


    public string Id { get; set; }
    public string Abbreviation { get => _localizer[Id]; }
    public string Name { get => _localizer[Id + "N"]; }
    public string Description { get => _localizer[Id + "D"]; }
}
