using System;
using Betauer.TileSet.Terrain;
using Betauer.TileSet.TileMap;
using Betauer.TileSet.TileMap.Handlers;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.TileSet;

public class BaseBlobTests {
    protected void AssertBlob47(string str, int[,] grid) {
        var tileMap = BasicTileMap.Parse(str);
        tileMap.Execute(new SetTileIdFromTerrainHandler(TilePatternRuleSets.Blob47Rules.WithTerrain(0)));
        try {
            AreEqual(tileMap.TileId, grid);
        } catch (Exception e) {
            Console.WriteLine("Error parsing: ");
            for (var y = 0; y < tileMap.Height; y++) {
                Console.Write("|");
                for (var x = 0; x < tileMap.Width; x++) {
                    Console.Write(tileMap.GetTileId(x, y).ToString().PadLeft(3) + "|");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Expected: ");
            for (var y = 0; y < grid.GetLength(0); y++) {
                Console.Write("|");
                for (var x = 0; x < grid.GetLength(1); x++) {
                    Console.Write(grid[y, x].ToString().PadLeft(3) + "|");
                }
                Console.WriteLine();
            }
            throw;
        }
    }


    public static bool AreEqual<T>(T[,] array1, T[,] array2) {
        Assert.AreEqual(array1.GetLength(0), array2.GetLength(0),
            $"Array dimension [{array1.GetLength(0)},{array1.GetLength(1)}] wrong. Expected: [{array2.GetLength(0)},{array2.GetLength(0)}]");

        Assert.AreEqual(array1.GetLength(1), array2.GetLength(1),
            $"Array dimension [{array1.GetLength(0)},{array1.GetLength(1)}] wrong. Expected: [{array2.GetLength(0)},{array2.GetLength(0)}]");

        for (var i = 0; i < array1.GetLength(0); i++) {
            for (var j = 0; j < array1.GetLength(1); j++) {
                Assert.That(array1[i, j], Is.EqualTo(array2[i, j]), "Element position: [" + i + "," + j + "]");
            }
        }
        return true;
    }
}