using System;

namespace GoCarlos.NET.UI.Models;

public class Player
{
    public Player()
    {
        Guid = Guid.NewGuid();
    }

    public Guid Guid { get; }
    public string Pin { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public int Gor { get; set; } = -900;
    public string Grade { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string Club { get; set; } = string.Empty;
}
