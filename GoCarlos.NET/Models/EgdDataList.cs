using GoCarlos.NET.Interfaces;

namespace GoCarlos.NET.Models;

public class EgdDataList : IEgdDataList
{
    public string Retcode { get; set; } = "";
    public IEgdData[] Players { get; set; } = System.Array.Empty<EgdData>();
}
