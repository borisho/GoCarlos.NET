// Copyright (C) 2023 Boris Dovčík
// This file is part of GoCarlos.NET.
//
// GoCarlos.NET is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 3, or (at your option) any
// later version.
//
// GoCarlos.NET is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with GoCarlos.NET. If not, see <https://www.gnu.org/licenses/>.

using GoCarlos.NET.Models;
using GoCarlos.NET.Models.Enums;

namespace GoCarlos.NET.Tests.Models;

public class PairingTests
{
    [Theory]
    [InlineData(Result.BOTH_WON, 1, 1)]
    [InlineData(Result.BLACK_WON, 1, 0)]
    [InlineData(Result.WHITE_WON, 0, 1)]
    [InlineData(Result.DRAW, 0.5, 0.5)]
    [InlineData(Result.BOTH_LOST, 0, 0)]
    [InlineData(Result.NONE, 0, 0)]
    public void GetPairingResult_ReturnsCorrectValuesForEachResult(
        Result result,
        decimal expectedBlackResult,
        decimal expectedWhiteResult
    ) {
        var black = new Player();
        var white = new Player();
        var pairing = new Pairing(black, white, 0, "", 0)
        {
            Result = result
        };

        Assert.Equal(expectedBlackResult, pairing.GetPairingResult(black));
        Assert.Equal(expectedWhiteResult, pairing.GetPairingResult(white));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void IsPlayerPaired_ReturnsTrueForEitherParticipant(bool checkWhite)
    {
        var black = new Player();
        var white = new Player();
        var pairing = new Pairing(black, white, 0, "", 0);

        var player = checkWhite ? white : black;

        Assert.True(pairing.IsPlayerPaired(player));
    }

    [Fact]
    public void IsPlayerPaired_ReturnsFalseForNotPairedPlayer()
    {
        var notPaired = new Player();
        var black = new Player();
        var white = new Player();
        var pairing = new Pairing(black, white, 0, "", 0);

        Assert.False(pairing.IsPlayerPaired(notPaired));
    }
}
