namespace GoCarlos.NET.Models;

public class Settings
{
    public GeneralSettings GeneralSettings { get; set; }

    public Settings()
    {
        GeneralSettings = new();
    }

    public Settings(GeneralSettings generalSettings)
    {
        GeneralSettings = generalSettings;
    }
}
