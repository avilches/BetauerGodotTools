using System;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Data;
using Betauer.TestRunner;
using Betauer.TileSet.Image;
using Betauer.TileSet.Terrain;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.TileSet;

internal static class TerrainRuleExtension {
    public static bool Matches(this TilePattern tilePattern, int value, int? templateTerrain = null) {
        AssertExportParse(tilePattern);
        var grid = new[,] { { value } }; 
        var matches = tilePattern.Matches(grid, templateTerrain);
        if (templateTerrain.HasValue) {
            var otherMatches = tilePattern.WithTerrain(templateTerrain.Value).Matches(grid);
            Assert.That(matches, Is.EqualTo(otherMatches));
        }
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
            Assert.That(rule.NeighborRule.ConditionType, Is.EqualTo(other.NeighborRule.ConditionType));
            Assert.That(rule.NeighborRule.ExpectedTerrain, Is.EqualTo(other.NeighborRule.ExpectedTerrain));
        });
    }
}

[TestRunner.Test]
public class TerrainRuleTests : BaseBlobTests {
    
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
        Assert.False(TilePattern.Parse("X").Matches(-1));
        Assert.True(TilePattern.Parse("X").Matches(0));
        Assert.True(TilePattern.Parse("X").Matches(1));
        Assert.True(TilePattern.Parse("X").Matches(2));

        // . means empty
        Assert.True(TilePattern.Parse(".").Matches(-1));
        Assert.False(TilePattern.Parse(".").Matches(0));
        Assert.False(TilePattern.Parse(".").Matches(1));
        Assert.False(TilePattern.Parse(".").Matches(2));

        // ? means anything, even empty
        Assert.True(TilePattern.Parse("?").Matches(-1));
        Assert.True(TilePattern.Parse("?").Matches(0));
        Assert.True(TilePattern.Parse("?").Matches(1));
        Assert.True(TilePattern.Parse("?").Matches(2));

        // # means currentTerrain (0 in this case)
        Assert.False(TilePattern.Parse("#").Matches(0, -1));
        Assert.True(TilePattern.Parse("#").Matches(0, 0));
        Assert.False(TilePattern.Parse("#").Matches(0, 1));
        Assert.False(TilePattern.Parse("#").Matches(11, -1));
        Assert.True(TilePattern.Parse("#").Matches(11, 11));
        Assert.False(TilePattern.Parse("#").Matches(11, 12));
        Assert.Throws<Exception>(() => TilePattern.Parse("#").Matches(-1));

        
        // ! means not currentTerrain (0 in this case)
        Assert.True(TilePattern.Parse("!").Matches(0,-1));
        Assert.False(TilePattern.Parse("!").Matches(0,0));
        Assert.True(TilePattern.Parse("!").Matches(0,1));
        Assert.True(TilePattern.Parse("!").Matches(11,-1));
        Assert.False(TilePattern.Parse("!").Matches(11,11));
        Assert.True(TilePattern.Parse("!").Matches(11,12));
        Assert.Throws<Exception>(() => TilePattern.Parse("!").Matches(0));
        
    }
    
    [Betauer.TestRunner.Test]
    public void ExportParseTest() {
        foreach (var rule in TilePatternRuleSets.Blob47Rules.Rules.Select(p => p.Item2)) {
            TerrainRuleExtension.AssertExportParse(rule);
        }
    }

    [Betauer.TestRunner.Test]
    public void Blob47Test() {
        var source = DataGrid<int>.Parse(@"
..0
000
", new System.Collections.Generic.Dictionary<char, int> {
            {'0', 0},
            {'.', -1}
        });

        var tileIds = new int[3, 2];
        var blob47 = TilePatternRuleSets.Blob47Rules.WithTerrain(0);
        
        var buffer = new int[3, 3];
        source.Loop((value, x, y) => {
            source.CopyCenterRectTo(x, y, -1, buffer);
            var tileId = blob47.FindRuleId(buffer, -1);
            tileIds[x,y] = tileId;
        });
        
        // put here the tileIds
        ArrayEquals(tileIds, new[,] {
            { -1,  -1, 16 },
            {  4,  68, 65 },
        }.FlipDiagonal());
    }

}
