using GoCarlos.NET.Models;

namespace GoCarlos.NET.Services.Api;

/// <summary>
/// To be added
/// </summary>
public interface IEgdService
{
    /// <summary>
    /// To be added
    /// </summary>
    EgdData? SearchByPin(string pin);

    /// <summary>
    /// To be added
    /// </summary>
    EgdDataList? SearchByData(string lastName, string name);
}
