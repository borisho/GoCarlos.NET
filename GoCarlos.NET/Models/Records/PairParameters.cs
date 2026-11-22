using GoCarlos.NET.Models.Enums;

namespace GoCarlos.NET.Models.Records;

public record class PairParameters(
    Round Round,
    bool AvoidSameCityPairing,
    int HandicapReduction,
    bool HandicapBasedMm,
    bool HandicapMaxNine,
    PairingMethod TopGroupPairingMethod,
    PairingMethod PairingMethod,
    PairingMethod AdditionMethod);