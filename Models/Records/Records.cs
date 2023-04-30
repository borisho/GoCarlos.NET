using GoCarlos.NET.Models.Enums;
using System.Collections.Generic;

namespace GoCarlos.NET.Models.Records;

public record class PairingGeneratorParameters(Round Round,
    bool AvoidSameCityPairing,
    int HandicapReduction,
    TournamentType TournamentType,
    PairingMethod PairingMethod,
    PairingMethod AdditionMethod,
    List<Player> OrderedPlayers);

public record class StackGroupParameters(Round Round,
    int HandicapReduction,
    PairingMethod AdditionMethod,
    Stack<List<Player>> GroupStack);

public record class GroupParamters(Round Round,
    int HandicapReduction,
    List<Player> Group);

public record class PairingParameters(Round Round,
    int HandicapReduction,
    Player P1,
    Player P2);
