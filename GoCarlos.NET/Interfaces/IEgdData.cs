namespace GoCarlos.NET.Interfaces;

/// <summary>
/// Player data from European Go Database
/// </summary>
public interface IEgdData
{
    public string Retcode { get; set; }
    public string Pin_Player { get; set; }
    public string AGAID { get; set; }
    public string Last_Name { get; set; }
    public string Name { get; set; }
    public string Country_Code { get; set; }
    public string Club { get; set; }
    public string Grade { get; set; }
    public string Grade_n { get; set; }
    public string EGF_Placement { get; set; }
    public string Gor { get; set; }
    public string DGor { get; set; }
    public string Proposed_Grade { get; set; }
    public string Tot_Tournaments { get; set; }
    public string Last_Appearance { get; set; }
    public string Elab_Date { get; set; }
    public string Hidden_History { get; set; }
    public string Real_Last_Name { get; set; }
    public string Real_Name { get; set; }
}
