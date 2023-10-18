using System;
using Betauer.Core;
using Betauer.TestRunner;
using Betauer.TileSet.Image;
using Betauer.TileSet.Terrain;
using Betauer.TileSet.TileMap;
using Betauer.TileSet.TileMap.Handlers;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.TileSet;

internal static class TerrainRuleExtension {
    public static bool Matches(this TilePattern tilePattern, int value) {
        var other = TilePattern.Parse(tilePattern.Export());
        Assert.True(tilePattern.Equals(other));
        
        var tileMap = new BasicTileMap(1, 1, 1);
        tileMap.SetTerrain(0, 0, value.ToEnum<BasicTileType>());
        return tilePattern.Matches(tileMap, 0, 0);
    }

    public static bool MatchesTemplate(this TilePattern tilePattern, int templateTerrain, int value) {
        var other = TilePattern.Parse(tilePattern.Export());
        Assert.True(tilePattern.Equals(other));
        
        var tileMap = new BasicTileMap(1, 1, 1);
        tileMap.SetTerrain(0, 0, value.ToEnum<BasicTileType>());
        return tilePattern.MatchesTemplate(tileMap, templateTerrain, 0, 0);
    }
}

[TestRunner.Test]
[Only]
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
        Assert.False(TilePattern.Parse("#").MatchesTemplate(0, -1));
        Assert.True(TilePattern.Parse("#").MatchesTemplate(0, 0));
        Assert.False(TilePattern.Parse("#").MatchesTemplate(0, 1));
        Assert.False(TilePattern.Parse("#").MatchesTemplate(11, -1));
        Assert.True(TilePattern.Parse("#").MatchesTemplate(11, 11));
        Assert.False(TilePattern.Parse("#").MatchesTemplate(11, 12));
        Assert.Throws<Exception>(() => TilePattern.Parse("#").Matches(-1));

        
        // ! means not currentTerrain (0 in this case)
        Assert.True(TilePattern.Parse("!").MatchesTemplate(0,-1));
        Assert.False(TilePattern.Parse("!").MatchesTemplate(0,0));
        Assert.True(TilePattern.Parse("!").MatchesTemplate(0,1));
        Assert.True(TilePattern.Parse("!").MatchesTemplate(11,-1));
        Assert.False(TilePattern.Parse("!").MatchesTemplate(11,11));
        Assert.True(TilePattern.Parse("!").MatchesTemplate(11,12));
        Assert.Throws<Exception>(() => TilePattern.Parse("!").Matches(0));
        
    }
    
    [Betauer.TestRunner.Test]
    public void ExportParseTest() {
        foreach (var rule in TerrainRuleSets.Blob47Rules) {
            var other = TilePattern.Parse(rule.Item2.Export());
            Assert.True(rule.Item2.Equals(other));
        }
    }
    
    [Betauer.TestRunner.Test]
    public void Blob47RuleTest() {
        var tileMap = BasicTileMap.Parse(@$"
112
1 0
", 2);
        AreEqual(tileMap.TerrainGrid, new[,] {
            { 1 , 1, 2 },
            { 1, -1, 0 },
        });

        tileMap.Apply(new TerrainTileHandler(0, 1, TerrainRuleSets.Blob47Rules, 8, TileSetLayouts.Blob47Godot));

        // layer 0, terrain 1
        AreEqual(tileMap.ExportTileIdGrid(0), new[,] {
            { 20 , 64, -1 },
            {  1,  -1, -1 },
        });
        
        Assert.That(tileMap.GetCellInfoRef(0, 0, 0).TileId, Is.EqualTo(20));
        Assert.That(tileMap.GetCellInfoRef(0, 0, 0).AtlasCoords.Value, Is.EqualTo(TileSetLayouts.Blob47Godot.GetTilePositionById(20)));
        
        tileMap.Apply(new SetTileIdFromTerrainHandler(1, TerrainRuleSets.Blob47Rules.WithTerrain(2)),
            new SetAtlasCoordsFromTileSetLayoutHandler(1, 8, TileSetLayouts.Blob47Godot)
        );

        // layer 1, terrain 2
        AreEqual(tileMap.ExportTileIdGrid(1), new[,] {
            { -1, -1,  0 },
            { -1, -1, -1 },
        });
    }
}
