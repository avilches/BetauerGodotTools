using System.Collections.Generic;
using Betauer.TestRunner;
using Betauer.TileSet;

namespace Betauer.GameTools.Tests;

[Test]
[Only]
public class TerrainTests : BlobTests {
    [Betauer.TestRunner.Test]
    public void BasicTest() {
        var tileMap = new Terrain(3, 3);
        AreEqual(tileMap.Grid, new[,] {
            { -1, -1, -1 },
            { -1, -1, -1 },
            { -1, -1, -1 },
        });
    }

    [Betauer.TestRunner.Test]
    public void ParseDictionaryTest() {
        var tileMap = Terrain.Parse(@$"
 ###

 P P
@@ @@
", new Dictionary<char, int>() {
            { ' ', (int)Terrain.TileType.None },
            { '@', 0 },
            { 'P', 124 },
            { '#', (int)Terrain.TileType.Auto },
        });
        AreEqual(tileMap.Grid, new[,] {
            { -1, -2, -2, -2, -1 },
            { -1, -1, -1, -1, -1 },
            { -1, 124, -1, 124, -1 },
            { 0, 0, -1, 0, 0 },
        });
    }

    [Betauer.TestRunner.Test]
    public void ParseWithBordersTest() {
        var tileMap = Terrain.Parse(@$"

    |   |
    | # |
     |   |
");
        AreEqual(tileMap.Grid, new[,] {
            { -1, -1, -1 },
            { -1, -2, -1 },
            { -1, -1, -1 },
        });
    }
}