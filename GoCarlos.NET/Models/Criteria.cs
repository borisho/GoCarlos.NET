using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;

namespace GoCarlos.NET.Models;

public class Criteria
{
    private IStringLocalizer _localizer;

    public Criteria()
    {
        Abbreviation = string.Empty;

        IServiceProvider serviceProvider = App.AppHost!.Services;

        _localizer = serviceProvider.GetRequiredService<IStringLocalizerFactory>().Create("SettingsWindow", "GoCarlos.NET");
    }

    public Criteria(string abbreviation) : this()
    {
        Abbreviation = abbreviation;
    }


    public string Abbreviation { get; set; }

    public string Name { get => _localizer[Abbreviation]; }
    public string Description { get => _localizer[Abbreviation + "D"]; }
}
