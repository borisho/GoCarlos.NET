using System.Collections.Generic;
using System;

namespace GoCarlos.NET.Interfaces;

/// <summary>
/// Player data
/// </summary>
public interface IPlayer
{
    Guid Guid { get; }
    string LastName { get; set; }
    string FirstName { get; set; }
    string Pin { get; set; }
    string Grade { get; set; }
    string CountryCode { get; set; }
    string Club { get; set; }
    int Gor { get; set; }
    List<bool> RoundsPlaying { get; set; }
}
