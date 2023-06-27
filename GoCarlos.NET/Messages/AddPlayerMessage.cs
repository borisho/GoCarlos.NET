namespace GoCarlos.NET.Messages;

public class AddPlayerMessage
{
    public string Pin { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public int Gor { get; init; } = -900;
    public string Grade { get; init; } = string.Empty;
    public string CountryCode { get; init; } = string.Empty;
    public string Club { get; init; } = string.Empty;
}
