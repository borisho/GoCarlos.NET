using CommunityToolkit.Mvvm.ComponentModel;

namespace GoCarlos.NET.Models;

public partial class CriteriaSettings : ObservableObject
{
    private static readonly Criteria[] allCriterias =
    [
        new Criteria("NUL"),
        new Criteria("NBW"),
        new Criteria("MMS"),
        new Criteria("RAT"),
        new Criteria("GRD"),
        new Criteria("SOS"),
        new Criteria("SDS"),
        new Criteria("SSS"),
        new Criteria("MTG"),
    ];

    [ObservableProperty]
    private Criteria[] criterias;

    public CriteriaSettings()
    {
        Criterias =
        [
            allCriterias[2],
            allCriterias[6],
            allCriterias[5],
            allCriterias[8],
        ];
    }

    public Criteria[] AllCriteria { get => allCriterias; }
}
