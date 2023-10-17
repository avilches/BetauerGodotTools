using System.Collections.Generic;
using Betauer.Core;
using Betauer.TestRunner;
using Betauer.TileSet.Terrain;
using Betauer.TileSet.TileMap;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.TileSet;

[TestRunner.Test]
[Only]
public class TerrainTests : BaseBlobTests {
    [Betauer.TestRunner.Test]
    public void BasicTest() {
        var tileMap = new BasicTileMap(2, 3, 2);

        AreEqual(tileMap.TypeGrid, new[,] {
            { -1, -1, -1 },
            { -1, -1, -1 },
        });

        AreEqual(tileMap.ExportTileIdGrid(0), new[,] {
            { -1, -1, -1 },
            { -1, -1, -1 },
        });

        AreEqual(tileMap.ExportTileIdGrid(1), new[,] {
            { -1, -1, -1 },
            { -1, -1, -1 },
        });

        Assert.That(tileMap.GetType(0, 0), Is.EqualTo(BasicTileType.Empty));
        Assert.That(tileMap.GetCellInfoRef(0, 0, 0).TileId, Is.EqualTo(-1));
        Assert.That(tileMap.GetCellInfoRef(0, 0, 0).AtlasCoords.HasValue, Is.EqualTo(false));

        // Type
        tileMap.SetType(0, 0, BasicTileType.Type1);
        tileMap.SetTileId(0, 0, 0, 2);
        tileMap.SetAtlasCoords(0, 1,0, 0, Vector2I.Right);

        Assert.That(tileMap.GetType(0, 0), Is.EqualTo(BasicTileType.Type1));
        Assert.That(tileMap.GetCellInfoRef(0, 0, 0).TileId, Is.EqualTo(2));
        Assert.That(tileMap.GetCellInfoRef(0, 0, 0).SourceId, Is.EqualTo(1));
        Assert.That(tileMap.GetCellInfoRef(0, 0, 0).AtlasCoords.HasValue, Is.EqualTo(true));
        Assert.That(tileMap.GetCellInfoRef(0, 0, 0).AtlasCoords.Value, Is.EqualTo(Vector2I.Right));

        AreEqual(tileMap.ExportTileIdGrid(0), new[,] {
            { 2, -1, -1 },
            { -1, -1, -1 },
        });
        
        AreEqual(tileMap.TypeGrid, new[,] {
            {  1, -1, -1 },
            { -1, -1, -1 },
        });


    }

    public enum DemoTerrain {
        DemoTerrainEmpty = -1,
        DemoTerrain1 = 0,
        DemoTerrain2 = 124,
        DemoTerrain3 = 17,
        DemoTerrain4 = 68,
        DemoTerrain5 = 3,
    }
   
    [Betauer.TestRunner.Test]
    public void ParseDictionaryTest() {
        var tileMap = BasicTileMap.Parse(@$"
 ***

 P P|
@- @@
", new Dictionary<char, DemoTerrain>() {
            { ' ', DemoTerrain.DemoTerrainEmpty },
            { '@', DemoTerrain.DemoTerrain1 },
            { 'P', DemoTerrain.DemoTerrain2 },
            { '|', DemoTerrain.DemoTerrain3 },
            { '-', DemoTerrain.DemoTerrain4 },
            { '*', DemoTerrain.DemoTerrain5 },
        });
        
        AreEqual(tileMap.TypeGrid, new[,] {
            { -1,   3,  3,   3, -1 },
            { -1,  -1, -1,  -1, -1 },
            { -1, 124, -1, 124, 17 },
            {  0,  68, -1,   0, 0 },
        });
    }
}
