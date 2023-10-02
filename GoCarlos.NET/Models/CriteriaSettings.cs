using CommunityToolkit.Mvvm.ComponentModel;

namespace GoCarlos.NET.Models;

public partial class CriteriaSettings : ObservableObject
{
    const int ALL_PARAMETERS_SIZE = 8;
    const int SELECTABLE_PARAMETER_SIZE = 4;

    private static readonly Criteria[] allCriterias = new Criteria[ALL_PARAMETERS_SIZE]
    {
        new Criteria("NUL"),
        new Criteria("NBW"),
        new Criteria("MMS"),
        new Criteria("RAT"),
        new Criteria("GRD"),
        new Criteria("SOS"),
        new Criteria("SDS"),
        new Criteria("SSS"),
    };

    [ObservableProperty]
    private Criteria[] criterias;

    public CriteriaSettings()
    {
        Criterias = new Criteria[SELECTABLE_PARAMETER_SIZE]
        {
            allCriterias[0],
            allCriterias[0],
            allCriterias[0],
            allCriterias[0],
        };
    }

    public Criteria[] AllCriteria { get => allCriterias; }
}
