using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Models.Enums;
using System.Collections.Generic;

namespace GoCarlos.NET.Models;

public partial class CriteriaSettings : ObservableObject
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
    public partial Criteria[] Criterias { get; set; }

    public CriteriaSettings()
    {
        Criterias =
        [
            allCriterias[CriteriaType.MMS],
            allCriterias[CriteriaType.SDS],
            allCriterias[CriteriaType.SOS],
            allCriterias[CriteriaType.VZP],
            allCriterias[CriteriaType.NUL]
        ];
    }

    public static Dictionary<CriteriaType, Criteria> AllCriteriaDict => allCriterias;

    public static Criteria[] AllCriteria => [.. allCriterias.Values];
}