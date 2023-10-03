namespace GoCarlos.NET.Models;

public class Criteria : LocalizedBase
{
    public Criteria(string id) : base(id, "SettingsWindow", "GoCarlos.NET") {}

    public string Abbreviation { get => _localizer[Id]; }
    public string Name { get => _localizer[Id + "N"]; }
    public string Description { get => _localizer[Id + "D"]; }
}
