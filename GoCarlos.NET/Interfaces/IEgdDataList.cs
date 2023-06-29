namespace GoCarlos.NET.Interfaces;

/// <summary>
/// Array of players data from European Go Database
/// </summary>
public interface IEgdDataList
{
    public string Retcode { get; set; }
    public IEgdData[] Players { get; set; }
}
