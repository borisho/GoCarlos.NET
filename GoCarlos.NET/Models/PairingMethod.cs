namespace GoCarlos.NET.Models;

public class PairingMethod(string id) : LocalizedBase(id, "SettingsWindow", "GoCarlos.NET")
{
    public string Method { get => _localizer[Id]; }
}
