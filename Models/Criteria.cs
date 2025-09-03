using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;

namespace GoCarlos.NET.Models;

public class Criteria(CriteriaType type, string name, string desc)
{
    private string abbreviation = type.ToString();

    public CriteriaType Type { get => type; set => type = value; }

    [JsonIgnore]
    public string Abbreviation { get => abbreviation; set => abbreviation = value; }

    [JsonIgnore]
    public string Name { get => name; set => name = value; }

    [JsonIgnore]
    public string Description { get => desc; set => desc = value; }
}