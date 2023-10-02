namespace GoCarlos.NET.Models;

public class Settings
{
    public GeneralSettings GeneralSettings { get; set; }
    public HandicapSettings HandicapSettings { get; set; }
    public CriteriaSettings CriteriaSettings { get; set; }

    public Settings()
    {
        GeneralSettings = new();
        HandicapSettings = new();
        CriteriaSettings = new();
    }

    public Settings(GeneralSettings generalSettings, HandicapSettings handicapSettings, CriteriaSettings criteriaSettings)
    {
        GeneralSettings = generalSettings;
        HandicapSettings = handicapSettings;
        CriteriaSettings = criteriaSettings;
    }
}
