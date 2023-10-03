namespace GoCarlos.NET.Models;

public class PairingMethod : LocalizedBase
{
    public PairingMethod(string id) : base(id, "SettingsWindow", "GoCarlos.NET") {}

    public string Method { get => _localizer[Id]; }
}
