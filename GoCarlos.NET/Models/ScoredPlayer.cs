namespace GoCarlos.NET.Models;

public class ScoredPlayer(Player player, GeneralSettings generalSettings) : Player(player)
{
    public float[] Points { get; init; } = new float[generalSettings.NumberOfRounds];
    public float[] Score { get; init; } = new float[generalSettings.NumberOfRounds];
    public float[] ScoreX { get; init; } = new float[generalSettings.NumberOfRounds];
    public float[] Sos { get; init; } = new float[generalSettings.NumberOfRounds];
    public float[] Sosos { get; init; } = new float[generalSettings.NumberOfRounds];
    public float[] Sodos { get; init; } = new float[generalSettings.NumberOfRounds];

    public float GetValue(Criteria criteria, int roundNumber)
    {
        return criteria.Id switch
        {
            "NUL" => 0f,
            "NBW" => Points[roundNumber],
            "MMS" => Score[roundNumber],
            "RAT" => Gor,
            "GRD" => GradeN,
            "SOS" => Sos[roundNumber],
            "SDS" => Sodos[roundNumber],
            "SSS" => Sosos[roundNumber],
            // "MTG" => Compare game,
            _ => 0f
        };
    }
}
