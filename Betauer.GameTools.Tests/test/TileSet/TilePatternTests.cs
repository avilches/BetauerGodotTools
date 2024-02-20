using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Data;
using Betauer.TestRunner;
using Betauer.TileSet.Terrain;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.TileSet;

internal static class TerrainRuleExtension {
    public static bool Matches(this TilePattern tilePattern, int value) {
        AssertExportParse(tilePattern);
        var grid = new[,] { { value } }; 
        var matches = tilePattern.Matches(grid);
        return matches;
    }
    
    public static void AssertExportParse(TilePattern a) {
        var export = a.Export();
        var b = TilePattern.Parse(export);
        Assert.That(export, Is.EqualTo(b.Export()));
        Assert.That(a.GridSize, Is.EqualTo(b.GridSize));
        Assert.That(a.Rules.Length, Is.EqualTo(b.Rules.Length));
        a.Rules.ForEach((rule, i) => {
            var other = b.Rules[i];
            Assert.That(rule.X, Is.EqualTo(other.X));
            Assert.That(rule.Y, Is.EqualTo(other.Y));
            Assert.That(rule.EqualsTo, Is.EqualTo(other.EqualsTo));
            Assert.That(rule.Value, Is.EqualTo(other.Value));
        });
    }
}

[TestRunner.Test]
[Only]
public class TilePatternTests : BaseBlobTests {
    
    [TestRunner.Test]
    public void ParseRulesCheckTest() {
        // !2 means anything but 2, even empty is ok
        Assert.True(TilePattern.Parse("!2").Matches(-1));
        Assert.True(TilePattern.Parse("!2").Matches(0));
        Assert.True(TilePattern.Parse("!2").Matches(1));
        Assert.False(TilePattern.Parse("!2").Matches(2));

        // 2 means only 2, empty is not ok
        Assert.False(TilePattern.Parse("2").Matches(-1));
        Assert.False(TilePattern.Parse("2").Matches(0));
        Assert.False(TilePattern.Parse("2").Matches(1));
        Assert.True(TilePattern.Parse("2").Matches(2));

        // X means not empty
        var dictionary = new Dictionary<string, NeighborRule> {
            {"X", NeighborRule.CreateEqualsTo(-1)},
        };
        Assert.True(TilePattern.Parse("X", dictionary).Matches(-1));
        Assert.False(TilePattern.Parse("X", dictionary).Matches(0));
        Assert.False(TilePattern.Parse("X", dictionary).Matches(1));
        Assert.False(TilePattern.Parse("X", dictionary).Matches(2));

        // ? means anything, even empty
        Assert.True(TilePattern.Parse("?").Matches(-1));
        Assert.True(TilePattern.Parse("?").Matches(0));
        Assert.True(TilePattern.Parse("?").Matches(1));
        Assert.True(TilePattern.Parse("?").Matches(2));
    }
    
    [Betauer.TestRunner.Test]
    public void ExportParseTest() {
        foreach (var rule in TilePatternRuleSets.Blob47.Patterns.Select(p => p.Item2)) {
            TerrainRuleExtension.AssertExportParse(rule);
        }
    }

    [Betauer.TestRunner.Test]
    public void Blob47Test() {
        var source = DataGrid<int>.Parse(@"
..0
000
", new System.Collections.Generic.Dictionary<char, int> {
            {'0',  0},
            {'.', -1}
        });

        var tileIds = new int[3, 2];
        var blob47 = TilePatternRuleSets.Blob47;
        
        var buffer = new int[3, 3];
        source.Loop((value, x, y) => {
            source.CopyCenterRectTo(x, y, -1, buffer);
            var tileId = blob47.FindTilePatternId(buffer, -1);
            tileIds[x,y] = tileId;
        });
        
        // put here the tileIds
        ArrayEquals(tileIds, new[,] {
            { -1,  -1, 16 },
            {  4,  68, 65 },
        }.FlipDiagonal());
    }

}
