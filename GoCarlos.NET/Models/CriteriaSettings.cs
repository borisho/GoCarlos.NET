using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Models.Converters;
using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GoCarlos.NET.Models;

[method: JsonConstructor]
public partial class CriteriaSettings() : ObservableObject
{
    private static readonly Dictionary<CriteriaType, Criteria> allCriterias = new()
    {
        { CriteriaType.NUL, new Criteria(CriteriaType.NUL, "Null", "Bez parametra") },
        { CriteriaType.POV, new Criteria(CriteriaType.POV, "Počet výhier", "") },
        { CriteriaType.MMS, new Criteria(CriteriaType.MMS, "McMahon skóre", "") },
        { CriteriaType.RAT, new Criteria(CriteriaType.RAT, "Rating", "Rating od -900 do ∞") },
        { CriteriaType.TRD, new Criteria(CriteriaType.TRD, "Trieda", "Trieda od 9d do 30k") },
        { CriteriaType.SOS, new Criteria(CriteriaType.SOS, "SOS", "Súčet skóre súperov") },
        { CriteriaType.SDS, new Criteria(CriteriaType.SDS, "SODOS", "Súčet skóre porazených súperov") },
        { CriteriaType.SSS, new Criteria(CriteriaType.SSS, "SOSOS", "Súčet SOS súperov") },
        { CriteriaType.VZP, new Criteria(CriteriaType.VZP, "Vzájomná partia", "Víťaz vzájomnej partie berie lepšiu priečku") }
    };

    [ObservableProperty]
    [JsonConverter(typeof(CriteriaConverter))]
    public partial ObservableCollection<Criteria> Criterias { get; set; } = [];

    public static Dictionary<CriteriaType, Criteria> AllCriteriaDict => allCriterias;

    public static Criteria[] AllCriteria => [.. allCriterias.Values];
}