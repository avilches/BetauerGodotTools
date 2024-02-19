using System;
using System.Linq;
using Betauer.Core;
using Betauer.TestRunner;
using Betauer.TileSet.Image;
using Betauer.TileSet.Terrain;
using Betauer.TileSet.TileMap;
using Betauer.TileSet.TileMap.Handlers;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.TileSet;

internal static class TerrainRuleExtension {
    public static bool Matches(this TilePattern tilePattern, int value, int? templateTerrain = null) {
        Equals(tilePattern);
        var tileMap = new BasicTileMap(1, 1, 1);
        tileMap.SetTerrain(0, 0, value.ToEnum<BasicTileType>());
        var buffer = new int[1, 1]; 
        TilePatternRuleSet.CopyCenterRectTo(tileMap.TerrainGrid, 0, 0, -1, buffer);
        var matches = tilePattern.Matches(buffer, templateTerrain);
        var otherMatches = false;
        var tother = tilePattern;
        if (templateTerrain.HasValue) {
            tother = tilePattern.WithTerrain(templateTerrain.Value);
        }
        tileMap.NewAction().IfPattern(tother).Do((x, y) => otherMatches = true).Apply();
        Assert.That(matches, Is.EqualTo(otherMatches));
        return matches;
    }
    
    public static void Equals(TilePattern a) {
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
            TerrainRuleExtension.Equals(rule);
        }
    }
    
    [Betauer.TestRunner.Test]
    public void Blob47RuleTerrainTileHandlerTest() {
        var tileMap = BasicTileMap.Parse(@$"
112
100
", 3);

        // layer 0, terrain 1 without template
        tileMap.Loop(new TerrainTileHandler(0, TilePatternRuleSets.Blob47Rules.WithTerrain(1), tileMap.CreateSource(7, TileSetLayouts.Blob47Godot)));

        IsEqualToDiagonal(tileMap.TileId, new[,] {
            { 20 , 64, -1 },
            {  1,  -1, -1 },
        });
        
        Assert.That(tileMap.GetTileId(0, 0), Is.EqualTo(20));
        Assert.That(tileMap.GetCellInfoRef(0, 0, 0).SourceId, Is.EqualTo(7));
        Assert.That(tileMap.GetCellInfoRef(0, 0, 0).AtlasCoords.Value, Is.EqualTo(TileSetLayouts.Blob47Godot.GetAtlasCoordsByTileId(20)));
        

        // layer 1, terrain 0 with template
        tileMap.Loop(new TerrainTileHandler(1, TilePatternRuleSets.Blob47Rules.WithTerrain(0), tileMap.CreateSource(8, TileSetLayouts.Blob47Godot)));

        IsEqualToDiagonal(tileMap.TileId, new[,] {
            { 20, 64, -1 },
            {  1,  4, 64 },
        });
        
        Assert.That(tileMap.GetTileId(1, 1), Is.EqualTo(4));
        Assert.That(tileMap.GetCellInfoRef(1, 1, 1).SourceId, Is.EqualTo(8));
        Assert.That(tileMap.GetCellInfoRef(1, 1, 1).AtlasCoords.Value, Is.EqualTo(TileSetLayouts.Blob47Godot.GetAtlasCoordsByTileId(4)));
        
    }
    
    [Betauer.TestRunner.Test]
    public void Blob47RuleTest() {
        var tileMap = BasicTileMap.Parse(@$"
112
100
", 3);

        // layer 2, terrain 2
        tileMap.Loop(new SetTileIdFromTerrainHandler(TilePatternRuleSets.Blob47Rules.WithTerrain(2)),
            new SetAtlasCoordsFromTileSetLayoutHandler(2, tileMap.CreateSource(9, TileSetLayouts.Blob47Godot))
        );

        IsEqualToDiagonal(tileMap.TileId, new[,] {
            { -1, -1,  0 },
            { -1, -1, -1 },
        });
        Assert.That(tileMap.GetTileId(2, 0), Is.EqualTo(0));
        Assert.That(tileMap.GetCellInfoRef(2, 2, 0).SourceId, Is.EqualTo(9));
        Assert.That(tileMap.GetCellInfoRef(2, 2, 0).AtlasCoords.Value, Is.EqualTo(TileSetLayouts.Blob47Godot.GetAtlasCoordsByTileId(0)));
    }

    [Betauer.TestRunner.Test]
    public void PipelineTest() {
        var tileMap = BasicTileMap.Parse(@$"
112
100
", 3);

        // layer 2, terrain 2
        tileMap.NewAction().If((t, x, y) => true)
            .If((x, y) => true)
            .IfTerrain(2)
            .Do(new SetTileIdFromTerrainHandler(TilePatternRuleSets.Blob47Rules.WithTerrain(2)))
            .Do(new SetAtlasCoordsFromTileSetLayoutHandler(2, tileMap.CreateSource(9, TileSetLayouts.Blob47Godot)))
            .Apply();

        IsEqualToDiagonal(tileMap.TileId, new[,] {
            { -1, -1,  0 },
            { -1, -1, -1 },
        });
        Assert.That(tileMap.GetTileId(2, 0), Is.EqualTo(0));
        Assert.That(tileMap.GetCellInfoRef(2, 2, 0).SourceId, Is.EqualTo(9));
        Assert.That(tileMap.GetCellInfoRef(2, 2, 0).AtlasCoords.Value, Is.EqualTo(TileSetLayouts.Blob47Godot.GetAtlasCoordsByTileId(0)));
    }
    
    [Betauer.TestRunner.Test]
    public void PipelineNoFilterTest() {
        var tileMap = BasicTileMap.Parse(@$"
112
100
", 3);

        var called = false;

        ((TileMap)tileMap).NewAction().IfTerrain(3)
            .Do((x, y) => called = true)
            .Apply();
        
        tileMap
            .IfTerrain(BasicTileType.Type9)
            .Do((x, y) => called = true)
            .Apply();
        
        tileMap
            .NewAction().If((t, x, y) => false)
            .Do((x, y) => called = true)
            .Apply();
        
        Assert.False(called);

    }
}
