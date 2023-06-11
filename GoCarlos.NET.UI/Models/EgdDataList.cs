namespace GoCarlos.NET.UI.Models;

public class EgdDataList
{
    public string Retcode { get; set; } = "";
    public EgdData[] Players { get; set; } = System.Array.Empty<EgdData>();
}
