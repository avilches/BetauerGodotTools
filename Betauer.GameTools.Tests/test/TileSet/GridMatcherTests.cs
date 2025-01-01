using System;
using System.Collections.Generic;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.GridMatching;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.TileSet;

[TestFixture]
public class GridMatcherTests : BaseBlobTests {
    [Test]
    public void ParseTilePatternRuleTest() {
        var dictionary = new Dictionary<string, Func<int, bool>> {
            { "X", (v) => v == -1 },
        };
        Assert.True(GridMatcher.Parse("X", dictionary).Matches(-1));
        Assert.False(GridMatcher.Parse("X", dictionary).Matches(0));
        Assert.False(GridMatcher.Parse("X", dictionary).Matches(1));
        Assert.False(GridMatcher.Parse("X", dictionary).Matches(2));
    }

    [Test]
    public void ParseTilePatternIntTests() {
        Assert.True(GridMatcher.Parse("-9").Matches(-9));
        Assert.True(GridMatcher.Parse("-8").Matches(-8));
        Assert.True(GridMatcher.Parse("-7").Matches(-7));
        Assert.True(GridMatcher.Parse("-6").Matches(-6));
        Assert.True(GridMatcher.Parse("-5").Matches(-5));
        Assert.True(GridMatcher.Parse("-4").Matches(-4));
        Assert.True(GridMatcher.Parse("-3").Matches(-3));
        Assert.True(GridMatcher.Parse("-2").Matches(-2));
        Assert.True(GridMatcher.Parse("-1").Matches(-1));
        Assert.True(GridMatcher.Parse("-0").Matches(0));
        Assert.True(GridMatcher.Parse("0").Matches(0));
        Assert.True(GridMatcher.Parse("1").Matches(1));
        Assert.True(GridMatcher.Parse("2").Matches(2));
        Assert.True(GridMatcher.Parse("3").Matches(3));
        Assert.True(GridMatcher.Parse("4").Matches(4));
        Assert.True(GridMatcher.Parse("5").Matches(5));
        Assert.True(GridMatcher.Parse("6").Matches(6));
        Assert.True(GridMatcher.Parse("7").Matches(7));
        Assert.True(GridMatcher.Parse("8").Matches(8));
        Assert.True(GridMatcher.Parse("9").Matches(9));

        Assert.False(GridMatcher.Parse("-9").Matches(9 + 1));
        Assert.False(GridMatcher.Parse("-8").Matches(-8 + 1));
        Assert.False(GridMatcher.Parse("-7").Matches(-7 + 1));
        Assert.False(GridMatcher.Parse("-6").Matches(-6 + 1));
        Assert.False(GridMatcher.Parse("-5").Matches(-5 + 1));
        Assert.False(GridMatcher.Parse("-4").Matches(-4 + 1));
        Assert.False(GridMatcher.Parse("-3").Matches(-3 + 1));
        Assert.False(GridMatcher.Parse("-2").Matches(-2 + 1));
        Assert.False(GridMatcher.Parse("-1").Matches(-1 + 1));
        Assert.False(GridMatcher.Parse("-0").Matches(-0 + 1));
        Assert.False(GridMatcher.Parse("0").Matches(0 + 1));
        Assert.False(GridMatcher.Parse("1").Matches(1 + 1));
        Assert.False(GridMatcher.Parse("2").Matches(2 + 1));
        Assert.False(GridMatcher.Parse("3").Matches(3 + 1));
        Assert.False(GridMatcher.Parse("4").Matches(4 + 1));
        Assert.False(GridMatcher.Parse("5").Matches(5 + 1));
        Assert.False(GridMatcher.Parse("6").Matches(6 + 1));
        Assert.False(GridMatcher.Parse("7").Matches(7 + 1));
        Assert.False(GridMatcher.Parse("8").Matches(8 + 1));
        Assert.False(GridMatcher.Parse("9").Matches(9 + 1));
    }

    [Test]
    public void ParseTilePatternIntNegativeTests() {
        Assert.False(GridMatcher.Parse("!-9").Matches(-9));
        Assert.False(GridMatcher.Parse("!-8").Matches(-8));
        Assert.False(GridMatcher.Parse("!-7").Matches(-7));
        Assert.False(GridMatcher.Parse("!-6").Matches(-6));
        Assert.False(GridMatcher.Parse("!-5").Matches(-5));
        Assert.False(GridMatcher.Parse("!-4").Matches(-4));
        Assert.False(GridMatcher.Parse("!-3").Matches(-3));
        Assert.False(GridMatcher.Parse("!-2").Matches(-2));
        Assert.False(GridMatcher.Parse("!-1").Matches(-1));
        Assert.False(GridMatcher.Parse("!-0").Matches(0));
        Assert.False(GridMatcher.Parse("!0").Matches(0));
        Assert.False(GridMatcher.Parse("!1").Matches(1));
        Assert.False(GridMatcher.Parse("!2").Matches(2));
        Assert.False(GridMatcher.Parse("!3").Matches(3));
        Assert.False(GridMatcher.Parse("!4").Matches(4));
        Assert.False(GridMatcher.Parse("!5").Matches(5));
        Assert.False(GridMatcher.Parse("!6").Matches(6));
        Assert.False(GridMatcher.Parse("!7").Matches(7));
        Assert.False(GridMatcher.Parse("!8").Matches(8));
        Assert.False(GridMatcher.Parse("!9").Matches(9));

        Assert.False(GridMatcher.Parse("!-9").Matches(-9));
        Assert.False(GridMatcher.Parse("!-8").Matches(-8));
        Assert.False(GridMatcher.Parse("!-7").Matches(-7));
        Assert.False(GridMatcher.Parse("!-6").Matches(-6));
        Assert.False(GridMatcher.Parse("!-5").Matches(-5));
        Assert.False(GridMatcher.Parse("!-4").Matches(-4));
        Assert.False(GridMatcher.Parse("!-3").Matches(-3));
        Assert.False(GridMatcher.Parse("!-2").Matches(-2));
        Assert.False(GridMatcher.Parse("!-1").Matches(-1));
        Assert.False(GridMatcher.Parse("!-0").Matches(0));
        Assert.False(GridMatcher.Parse("!0").Matches(0));
        Assert.False(GridMatcher.Parse("!1").Matches(1));
        Assert.False(GridMatcher.Parse("!2").Matches(2));
        Assert.False(GridMatcher.Parse("!3").Matches(3));
        Assert.False(GridMatcher.Parse("!4").Matches(4));
        Assert.False(GridMatcher.Parse("!5").Matches(5));
        Assert.False(GridMatcher.Parse("!6").Matches(6));
        Assert.False(GridMatcher.Parse("!7").Matches(7));
        Assert.False(GridMatcher.Parse("!8").Matches(8));
        Assert.False(GridMatcher.Parse("!9").Matches(9));
    }

    [Test]
    public void Blob47Test() {
        var source = Array2D.Parse("""
                                   ..0
                                   000
                                   """, new Dictionary<char, int> {
            { '0', 0 },
            { '.', -1 }
        });

        var tileIds = new int[2, 3];
        var blob47 = Blob47.Rules;

        var buffer = new int[3, 3];
        foreach (var ((x, y), value) in source.GetIndexedValues()) {
            source.CopyNeighbors(x, y, buffer, -1);
            var tileId = blob47.FindTilePatternId(buffer, -1);
            tileIds[y, x] = tileId;
        }

        // put here the tileIds
        ArrayEquals(tileIds, new[,] {
            { -1, -1, 16 },
            { 4, 68, 65 },
        });
    }
}

internal static class TerrainRuleExtension {
    public static bool Matches(this GridMatcher<int> gridMatcher, int value) {
        var grid = new[,] { { value } };
        var matches = gridMatcher.Matches(grid);
        var cloned = GridMatcher.Parse(gridMatcher.Export(), gridMatcher.Rules);
        var matchesCloned = cloned.Matches(grid);
        Assert.That(matches, Is.EqualTo(matchesCloned));
        return matches;
    }
}