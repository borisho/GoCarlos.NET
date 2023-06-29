namespace GoCarlos.NET.Interfaces;

/// <summary>
/// To be added
/// </summary>
public interface IEgdService
{
    /// <summary>
    /// To be added
    /// </summary>
    IEgdData? SearchByPin(string pin);

    /// <summary>
    /// To be added
    /// </summary>
    IEgdDataList? SearchByData(string lastName, string name);
}
