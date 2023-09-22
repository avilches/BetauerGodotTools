using System.Collections.Generic;
using Betauer.TestRunner;
using Betauer.TileSet.Terrain;

namespace Betauer.GameTools.Tests.TileSet;

[Test]
[Only]
public class TerrainTests : BaseBlobTests {
    [Betauer.TestRunner.Test]
    public void BasicTest() {
        var tileMap = new SingleTerrain(3, 3);
        AreEqual(tileMap.Grid, new[,] {
            { -1, -1, -1 },
            { -1, -1, -1 },
            { -1, -1, -1 },
        });
    }

    [Betauer.TestRunner.Test]
    public void ParseDictionaryTest() {
        var tileMap = SingleTerrain.Parse(@$"
 ***

 P P|
@- @@
", new Dictionary<char, int>() {
            { ' ', (int)SingleTerrain.TileType.None },
            { '@', 0 },
            { 'P', 124 },
            { '*', (int)SingleTerrain.TileType.Auto },
        });
        AreEqual(tileMap.Grid, new[,] {
            { -1, -2, -2, -2, -1 },
            { -1, -1, -1, -1, -1 },
            { -1, 124, -1, 124, 17 },
            { 0, 68, -1, 0, 0 },
        });
    }

    [Betauer.TestRunner.Test]
    public void ParseWithBordersTest() {
        var tileMap = SingleTerrain.Parse(@$"

:   :
 : * :
  :|| :


");
        AreEqual(tileMap.Grid, new[,] {
            { -1, -1, -1 },
            { -1, -2, -1 },
            { 17, 17, -1 },
        });
    }
}