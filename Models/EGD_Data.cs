namespace GoCarlos.NET.Models;

public class EGD_Data
{
    private string? retcode;
    private string? pin_Player;
    private string? aGAID;
    private string? last_Name;
    private string? name;
    private string? country_Code;
    private string? club;
    private string? grade;
    private string? grade_n;
    private string? eGF_Placement;
    private string? gor;
    private string? dGor;
    private string? proposed_Grade;
    private string? tot_Tournaments;
    private string? last_Appearance;
    private string? elab_Date;
    private string? hidden_History;
    private string? real_Last_Name;
    private string? real_Name;

    public EGD_Data() { }
    public EGD_Data(
        string retcode,
        string pin_Player,
        string aGAID,
        string last_Name,
        string name,
        string country_Code,
        string club,
        string grade,
        string grade_n,
        string eGF_Placement,
        string gor,
        string dGor,
        string proposed_Grade,
        string tot_Tournaments,
        string last_Appearance,
        string elab_Date,
        string hidden_History,
        string real_Last_Name,
        string real_Name)
    {
        Retcode = retcode;
        Pin_Player = pin_Player;
        AGAID = aGAID;
        Last_Name = last_Name;
        Name = name;
        Country_Code = country_Code;
        Club = club;
        Grade = grade;
        Grade_n = grade_n;
        EGF_Placement = eGF_Placement;
        Gor = gor;
        DGor = dGor;
        Proposed_Grade = proposed_Grade;
        Tot_Tournaments = tot_Tournaments;
        Last_Appearance = last_Appearance;
        Elab_Date = elab_Date;
        Hidden_History = hidden_History;
        Real_Last_Name = real_Last_Name;
        Real_Name = real_Name;
    }

    public EGD_Data(EGD_Data data)
    {
        Retcode = data?.retcode!;
        Pin_Player = data?.pin_Player!;
        AGAID = data?.aGAID!;
        Last_Name = data?.last_Name!;
        Name = data?.name!;
        Country_Code = data?.country_Code!;
        Club = data?.club!;
        Grade = data?.grade!;
        Grade_n = data?.grade_n!;
        EGF_Placement = data?.eGF_Placement!;
        Gor = data?.gor!;
        DGor = data?.dGor!;
        Proposed_Grade = data?.proposed_Grade!;
        Tot_Tournaments = data?.tot_Tournaments!;
        Last_Appearance = data?.last_Appearance!;
        Elab_Date = data?.elab_Date!;
        Hidden_History = data?.hidden_History!;
        Real_Last_Name = data?.real_Last_Name!;
        Real_Name = data?.real_Name!;
    }

    public string Retcode
    {
        get => retcode ?? "";
        set => retcode = value;
    }
    public string Pin_Player
    {
        get => pin_Player ?? "";
        set => pin_Player = value;
    }
    public string AGAID
    {
        get => aGAID ?? "";
        set => aGAID = value;
    }
    public string Last_Name
    {
        get => last_Name ?? "";
        set => last_Name = value;
    }
    public string Name
    {
        get => name ?? "";
        set => name = value;
    }
    public string Country_Code
    {
        get => country_Code ?? "";
        set => country_Code = value;
    }
    public string Club
    {
        get => club ?? "";
        set => club = value;
    }
    public string Grade
    {
        get => grade ?? "";
        set => grade = value;
    }
    public string Grade_n
    {
        get => grade_n ?? "";
        set => grade_n = value;
    }
    public string EGF_Placement
    {
        get => eGF_Placement ?? "";
        set => eGF_Placement = value;
    }
    public string Gor
    {
        get => gor ?? "";
        set => gor = value;
    }
    public string DGor
    {
        get => dGor ?? "";
        set => dGor = value;
    }
    public string Proposed_Grade
    {
        get => proposed_Grade ?? "";
        set => proposed_Grade = value;
    }
    public string Tot_Tournaments
    {
        get => tot_Tournaments ?? "";
        set => tot_Tournaments = value;
    }
    public string Last_Appearance
    {
        get => last_Appearance ?? "";
        set => last_Appearance = value;
    }
    public string Elab_Date
    {
        get => elab_Date ?? "";
        set => elab_Date = value;
    }
    public string Hidden_History
    {
        get => hidden_History ?? "";
        set => hidden_History = value;
    }
    public string Real_Last_Name
    {
        get => real_Last_Name ?? "";
        set => real_Last_Name = value;
    }
    public string Real_Name
    {
        get => real_Name ?? "";
        set => real_Name = value;
    }
}
