namespace GoCarlos.NET.Models;

public class Settings
{
    public GeneralSettings GeneralSettings { get; set; }
    public HandicapSettings HandicapSettings { get; set; }
    public CriteriaSettings CriteriaSettings { get; set; }
    public PairingSettings PairingSettings { get; set; }

    public Settings()
    {
        GeneralSettings = new();
        HandicapSettings = new();
        CriteriaSettings = new();
        PairingSettings = new();
    }

    public Settings(GeneralSettings generalSettings, HandicapSettings handicapSettings, CriteriaSettings criteriaSettings, PairingSettings pairingSettings)
    {
        GeneralSettings = generalSettings;
        HandicapSettings = handicapSettings;
        CriteriaSettings = criteriaSettings;
        PairingSettings = pairingSettings;
    }
}
