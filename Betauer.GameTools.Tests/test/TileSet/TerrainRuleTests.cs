using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.TestRunner;
using Betauer.TileSet.Image;
using Betauer.TileSet.Terrain;
using Betauer.TileSet.TileMap;
using Betauer.TileSet.TileMap.Handlers;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.TileSet;


internal static class TerrainRuleExtension {
    public static bool Check(this TerrainRule terrainRule, int value) {

        var other = TerrainRule.Parse(terrainRule.TileId, terrainRule.Export());
        Assert.True(terrainRule.Equals(other));
        
        var tileMap = new BasicTileMap(1, 1, 1);
        tileMap.SetType(0, 0, (BasicTileType)value);
        return terrainRule.Check(tileMap, 0, 0);
    }
}

[TestRunner.Test]
[Only]
public class TerrainRuleTests : BaseBlobTests {
    
    [TestRunner.Test]
    public void ParseRulesCheckTest() {
        // !2 means anything but 2, even empty is ok
        Assert.True(TerrainRule.Parse(0, "!2").Check(-1));
        Assert.True(TerrainRule.Parse(0, "!2").Check(0));
        Assert.True(TerrainRule.Parse(0, "!2").Check(1));
        Assert.False(TerrainRule.Parse(0, "!2").Check(2));

        // 2 means only 2, empty is not ok
        Assert.False(TerrainRule.Parse(0, "2").Check(-1));
        Assert.False(TerrainRule.Parse(0, "2").Check(0));
        Assert.False(TerrainRule.Parse(0, "2").Check(1));
        Assert.True(TerrainRule.Parse(0, "2").Check(2));

        // X means not empty
        Assert.False(TerrainRule.Parse(0, "X").Check(-1));
        Assert.True(TerrainRule.Parse(0, "X").Check(0));
        Assert.True(TerrainRule.Parse(0, "X").Check(1));
        Assert.True(TerrainRule.Parse(0, "X").Check(2));

        // . means empty
        Assert.True(TerrainRule.Parse(0, ".").Check(-1));
        Assert.False(TerrainRule.Parse(0, ".").Check(0));
        Assert.False(TerrainRule.Parse(0, ".").Check(1));
        Assert.False(TerrainRule.Parse(0, ".").Check(2));

        // ? means anything, even empty
        Assert.True(TerrainRule.Parse(0, "?").Check(-1));
        Assert.True(TerrainRule.Parse(0, "?").Check(0));
        Assert.True(TerrainRule.Parse(0, "?").Check(1));
        Assert.True(TerrainRule.Parse(0, "?").Check(2));

        // # means currentTerrain (0 in this case)
        Assert.False(TerrainRule.Parse(0, "#").ApplyTerrain(0).Check(-1));
        Assert.True(TerrainRule.Parse(0, "#").ApplyTerrain(0).Check(0));
        Assert.False(TerrainRule.Parse(0, "#").ApplyTerrain(0).Check(1));
        Assert.False(TerrainRule.Parse(0, "#").ApplyTerrain(11).Check(-1));
        Assert.True(TerrainRule.Parse(0, "#").ApplyTerrain(11).Check(11));
        Assert.False(TerrainRule.Parse(0, "#").ApplyTerrain(11).Check(12));
        Assert.Throws<Exception>(() => TerrainRule.Parse(0, "#").Check(-1));

        
        // ! means not currentTerrain (0 in this case)
        Assert.True(TerrainRule.Parse(0, "!").ApplyTerrain(0).Check(-1));
        Assert.False(TerrainRule.Parse(0, "!").ApplyTerrain(0).Check(0));
        Assert.True(TerrainRule.Parse(0, "!").ApplyTerrain(0).Check(1));
        Assert.True(TerrainRule.Parse(0, "!").ApplyTerrain(11).Check(-1));
        Assert.False(TerrainRule.Parse(0, "!").ApplyTerrain(11).Check(11));
        Assert.True(TerrainRule.Parse(0, "!").ApplyTerrain(11).Check(12));
        Assert.Throws<Exception>(() => TerrainRule.Parse(0, "!").Check(0));
        
    }
    
    [Betauer.TestRunner.Test]
    public void ExportParseTest() {
        foreach (var rule in TerrainRuleSets.Blob47Rules) {
            var other = TerrainRule.Parse(rule.TileId, rule.Export());
            Assert.True(rule.Equals(other));
        }
    }
    
    [Betauer.TestRunner.Test]
    public void Blob47RuleTest() {
        var tileMap = BasicTileMap.Parse(@$"
112
1 0
", 2);
        AreEqual(tileMap.ExportTerrainIdGrid(), new[,] {
            { 1 , 1, 2 },
            { 1, -1, 0 },
        });

        tileMap.Apply(new TerrainTileHandler(0, TerrainRuleSets.Blob47Rules.ApplyTerrain(1), 8, TileSetLayouts.Blob47Godot));

        // layer 0, terrain 1
        AreEqual(tileMap.ExportTileIdGrid(0), new[,] {
            { 20 , 64, -1 },
            {  1,  -1, -1 },
        });
        
        Assert.That(tileMap.GetCellInfoRef(0, 0, 0).TileId, Is.EqualTo(20));
        Assert.That(tileMap.GetCellInfoRef(0, 0, 0).AtlasCoords.Value, Is.EqualTo(TileSetLayouts.Blob47Godot.GetTilePositionById(20)));
        
        tileMap.Apply(new SetTileIdFromTerrainHandler(1, TerrainRuleSets.Blob47Rules.ApplyTerrain(2)),
            new SetAtlasCoordsFromTileSetLayoutHandler(1, 8, TileSetLayouts.Blob47Godot)
        );

        // layer 1, terrain 2
        AreEqual(tileMap.ExportTileIdGrid(1), new[,] {
            { -1, -1,  0 },
            { -1, -1, -1 },
        });
    }
}
