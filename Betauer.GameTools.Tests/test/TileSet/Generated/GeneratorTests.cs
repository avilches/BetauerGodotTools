using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Betauer.Core;
using Betauer.TestRunner;
using Betauer.TileSet.Godot;
using Betauer.TileSet.Image;
using Godot;
using NUnit.Framework;
using TileMap = Godot.TileMap;

namespace Betauer.GameTools.Tests.TileSet.Generated;

[Betauer.TestRunner.Test]
[Betauer.TestRunner.Ignore("Only use it to generate the test")]
public class GeneratorTests {

    [Betauer.TestRunner.Test(Description = "Ensure the Tilemap.tscn has all the possible values for the minimal 3x3 tileset blob 47")]
    public void VerifyTest() {
        var scene = ResourceLoader.Load<PackedScene>("res://test-resources/tileset/Tilemap.tscn");
        var godotTileMap = scene.Instantiate<Godot.TileMap>();
        var layer = 0;
        // var godotTileSet = ResourceLoader.Load<Godot.TileSet>("res://test-resources/tileset/Tileset.tres");
        var tiles = godotTileMap.GetTerrainMasksGrid(layer);
        var values = tiles.Cast<int>().Distinct().ToList();
        values.Remove(-1);
        Assert.That(values.Count, Is.EqualTo(47));
        CollectionAssert.AreEquivalent(values, TileSetLayouts.Blob47Godot.GetTileIds());
    }
    
    /*
     * How the Blob47Test.cs is generated
     *
     * 1) Run the Generate256Combinations() test only to generate the Tilemap-256-no-connected.tscn scene
     * 2) Open the Tilemap-256-no-connected.tscn in the Godot editor, connect all the tiles manually and save the file as Tilemap-256.tscn
     * 3) Run the GenerateBlob47Tests() test to generate the Blob47Tests.cs 
     * 4) Now you can run the Blob47Tests.cs test to verify that all the 256 combinations are correctly mapped to the blob 47
     */

    [Betauer.TestRunner.Test(Description = "Generate a no connected patterns in a Godot Tilemap with all the 256 combinations")]
    // [Betauer.TestRunner.Ignore(
    // "This test is only to generate a file that it will need to manually in Godot and save it as Tilemap-256.tscn with all the connections")]
    public void Generate256Combinations() {

        var terrain = new int[16 * 4, 16 * 4];
        var x = 0;
        var y = 0;
        for (var i = 0; i < 256; i++) {
            var neighbours = CreateNeighboursGrid(i, 0, -1);

            for (var xx = 0; xx < neighbours.GetLength(0); xx++) {
                for (var yy = 0; yy < neighbours.GetLength(1); yy++) {
                    terrain[x + xx, y + yy] = neighbours[xx, yy];
                }
            }
            x += 4;
            if (x >= terrain.GetLength(0)) {
                x = 0;
                y += 4;
            }
        }
        var layer = 0;
        var sourceId = 2;
        var atlasCoords = new Vector2I(0, 3);
        var scene = ResourceLoader.Load<PackedScene>("res://test-resources/tileset/Tilemap.tscn");
        var godotTileMap = scene.Instantiate<TileMap>();
        godotTileMap.Clear();
        for (var yy = 0; yy < terrain.GetLength(1); yy++) {
            for (var xx = 0; xx < terrain.GetLength(0); xx++) {
                var tileId = terrain[xx, yy];
                if (tileId >= 0) {
                    godotTileMap.SetCell(layer, new Vector2I(xx, yy), sourceId, atlasCoords);
                }
            }
        }
        var packedScene = new PackedScene();
        packedScene.Pack(godotTileMap);
        ResourceSaver.Save(packedScene, "res://test-resources/tileset/Tilemap-256-no-connected.tscn");
    }

    [Betauer.TestRunner.Test(Description = "Generate Blob47Tests.cs, which tests all the 256 possible combinations")]
    public void GenerateBlob47Tests() {
        var scene = ResourceLoader.Load<PackedScene>("res://test-resources/tileset/Tilemap-256.tscn");

        var godotTileMap = scene.Instantiate<TileMap>();
        var tiles = GetCentralTiles(godotTileMap);
        var values = tiles.Distinct().ToList();
        Assert.That(values.Count, Is.EqualTo(47));
        CollectionAssert.AreEquivalent(values, TileSetLayouts.Blob47Godot.GetTileIds());

        Dictionary<int, List<int>> shared = new();
        for (var i = 0; i < 256; i++) {
            var tile = tiles[i];
            if (!shared.ContainsKey(tile)) {
                shared[tile] = new List<int>() { i };
            } else {
                shared[tile].Add(i);
            }
        }
        
        var testClass = new StringWriter();
        foreach (var (mainTileId, sharedList) in shared) {
    
            var x = 0;  
            var terrainList = new int[3 * sharedList.Count, 3];
            foreach (var tileId in sharedList) {
                var neighbours = CreateNeighboursGrid(tileId, 0, -1);
                for (var xx = 0; xx < neighbours.GetLength(0); xx++) {
                    for (var yy = 0; yy < neighbours.GetLength(1); yy++) {
                        terrainList[x + xx, yy] = neighbours[xx, yy];
                    }
                }
                x += 3;
            }
            testClass.Write($"    // |");
            x = 0;
            foreach (var tileId in sharedList) {
                if (x == 1) testClass.Write("   |");
                testClass.Write(tileId.ToString().PadLeft(3, ' ') + "|");
                x++;
            }
            testClass.WriteLine();
            for (var yy = 0; yy < terrainList.GetLength(1); yy++) {
                testClass.Write($"    // |");
                for (var xx = 0; xx < terrainList.GetLength(0); xx++) {
                    if (xx == 3) testClass.Write("   |");
                    var tileId = terrainList[xx, yy];
                    testClass.Write(tileId == 0 ? "#" : " ");
                    if (xx % 3 == 2) {
                        testClass.Write("|");
                    }
                }
                testClass.WriteLine();
            }
            
            testClass.WriteLine($"    [Test(Description=\"{mainTileId} when {string.Join(",", sharedList)}\")]");
            testClass.WriteLine($"    public void TestTile{mainTileId}() {{");
    
            foreach (var tileId in sharedList) {
                var terrain = new int[3, 3];
                var neighbours = CreateNeighboursGrid(tileId, 0, -1);
                for (var xx = 0; xx < neighbours.GetLength(0); xx++) {
                    for (var yy = 0; yy < neighbours.GetLength(1); yy++) {
                        terrain[xx, yy] = neighbours[xx, yy];
                    }
                }
                testClass.WriteLine($"        ");
                testClass.WriteLine($"        // Pattern where central tile with {tileId} mask is transformed to {mainTileId}");
                testClass.WriteLine($"        AssertBlob47(\"\"\"");
                for (var yy = 0; yy < terrain.GetLength(1); yy++) {
                    testClass.Write($"                         ");
                    for (var xx = 0; xx < terrain.GetLength(0); xx++) {
                        testClass.Write(terrain[xx, yy] == 0 ? "#" : ".");
                    }
                    testClass.WriteLine();
                }
                testClass.WriteLine($"                         \"\"\", new[,] {{");
                var maskGrid = godotTileMap.GetTerrainMasksGrid(0, tileId % 16 * 4, tileId / 16 * 4, 3, 3);
                for (var yy = 0; yy < maskGrid.GetLength(0); yy++) {
                    testClass.Write($"                         {{");
                    for (var xx = 0; xx < maskGrid.GetLength(1); xx++) {
                        testClass.Write(maskGrid[yy, xx].ToString().PadLeft(4) + ", ");
                    }
                    testClass.WriteLine("}, ");
                }
                testClass.WriteLine($"                         }});");
            }
            testClass.WriteLine($"   }}");
        }
        // write testClass to a file
        File.WriteAllText("Betauer.GameTools.Tests/test/TileSet/Generated/Blob47Tests.cs",
            $$"""
              using Betauer.TestRunner;

              namespace Betauer.GameTools.Tests.TileSet.Generated;

              [Test]
              public class Blob47Tests : BaseBlobTests {
              {{testClass}}
              }
              """);
    }

    /// <summary>
    /// From mask 20, which is
    /// |   |
    /// | **|
    /// | * |
    /// it will return { { -1, -1, -1 }, { -1, 0, 0 }, { -1, 0,-1 } }
    /// </summary>
    /// <param name="mask"></param>
    /// <returns></returns>
    public static T[,] CreateNeighboursGrid<T>(int mask, T value, T empty) {
        var neighbours = new T[3, 3];
        const int x = 1;
        const int y = 1;
        neighbours[x    , y - 1] = BitTools.HasBit(mask, 1) ? value : empty; // TopSide
        neighbours[x + 1, y - 1] = BitTools.HasBit(mask, 2) ? value : empty; // TopRightCorner
        neighbours[x + 1, y    ] = BitTools.HasBit(mask, 3) ? value : empty; // RightSide
        neighbours[x + 1, y + 1] = BitTools.HasBit(mask, 4) ? value : empty; // BottomRightCorner
        neighbours[x    , y + 1] = BitTools.HasBit(mask, 5) ? value : empty; // BottomSide
        neighbours[x - 1, y + 1] = BitTools.HasBit(mask, 6) ? value : empty; // BottomLeftCorner
        neighbours[x - 1, y    ] = BitTools.HasBit(mask, 7) ? value : empty; // LeftSide
        neighbours[x - 1, y - 1] = BitTools.HasBit(mask, 8) ? value : empty; // TopLeftCorner
        neighbours[x    , y    ] = value; // Center
        return neighbours;
    }

    private static List<int> GetCentralTiles(TileMap godotTileMap) {
        var x = 0;
        var y = 0;
        var tiles = new List<int>();
        var tileLegend = new StringWriter();
        for (var i = 0; i < 256; i++) {
            var tileData = godotTileMap.GetCellTileData(0, new Vector2I(x + 1, y + 1));
            var bitmask = tileData?.GetTerrainMask() ?? -1;
            tileLegend.Write(i.ToString().PadLeft(3) + ":" + bitmask.ToString().PadRight(3) + " | ");
            tiles.Add(bitmask);
            x += 4;
            if (x >= 16 * 4) {
                x = 0;
                tileLegend.WriteLine();
                y += 4;
            }
        }

        Console.WriteLine(tileLegend);
        return tiles;
    }
}