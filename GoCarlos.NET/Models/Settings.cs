namespace GoCarlos.NET.Models;

public class Settings
{
    public GeneralSettings GeneralSettings { get; set; }
    public HandicapSettings HandicapSettings { get; set; }

    public Settings()
    {
        GeneralSettings = new();
        HandicapSettings = new();
    }

    public Settings(GeneralSettings generalSettings, HandicapSettings handicapSettings)
    {
        GeneralSettings = generalSettings;
        HandicapSettings = handicapSettings;
    }
}
