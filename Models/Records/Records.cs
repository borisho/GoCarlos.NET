using GoCarlos.NET.Models.Enums;
using System.Collections.Generic;

namespace GoCarlos.NET.Models.Records;

public record class PairingGeneratorParameters(Round Round,
    bool AvoidSameCityPairing,
    int HandicapReduction,
    bool HandicapBasedMm,
    PairingMethod TopGroupPairingMethod,
    PairingMethod PairingMethod,
    PairingMethod AdditionMethod,
    List<Player> OrderedPlayers,
    int NumberOfRounds);

public record class GroupParamters(Round Round,
    int HandicapReduction,
    bool HandicapBasedMm,
    List<Player> Group);

public record class PairingParameters(Round Round,
    int HandicapReduction,
    bool HandicapBasedMm,
    Player P1,
    Player P2);
