namespace GoCarlos.NET.Models;

public record class EgdData
{
    public string Retcode { get; init; } = string.Empty;
    public string Pin_Player { get; init; } = string.Empty;
    public string AGAID { get; init; } = string.Empty;
    public string Last_Name { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Country_Code { get; init; } = string.Empty;
    public string Club { get; init; } = string.Empty;
    public string Grade { get; init; } = string.Empty;
    public string Grade_n { get; init; } = string.Empty;
    public string EGF_Placement { get; init; } = string.Empty;
    public string Gor { get; init; } = string.Empty;
    public string DGor { get; init; } = string.Empty;
    public string Proposed_Grade { get; init; } = string.Empty;
    public string Tot_Tournaments { get; init; } = string.Empty;
    public string Last_Appearance { get; init; } = string.Empty;
    public string Elab_Date { get; init; } = string.Empty;
    public string Hidden_History { get; init; } = string.Empty;
    public string Real_Last_Name { get; init; } = string.Empty;
    public string Real_Name { get; init; } = string.Empty;
}
