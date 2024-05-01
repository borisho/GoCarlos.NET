namespace GoCarlos.NET.Models;

public record class EgdDataList
{
    public string Retcode { get; init; } = string.Empty;
    public EgdData[] Players { get; init; } = [];
}
