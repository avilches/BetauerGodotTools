using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Terrain;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.TileSet;

public abstract class BaseBlobTests {
    protected void AssertBlob47(string str, int[,] grid) {
        var source = Array2D.Parse(str, new System.Collections.Generic.Dictionary<char, int> {
            {'#',  0},
            {'.', -1}
        });
        
        var tileIds = new int[3, 3];
        var buffer = new int[3, 3];
        foreach (var cell in source) {
            source.CopyNeighbors(cell.Position.X, cell.Position.Y, buffer, -1);
            var tileId = TilePatternRuleSets.Blob47.FindTilePatternId(buffer, -1);
            tileIds[cell.Position.Y, cell.Position.X] = tileId;
        }
        
        ArrayEquals(tileIds, grid);
    }


    public static bool ArrayEquals<T>(T[,] array1, T[,] array2) {
        Assert.AreEqual(array1.GetLength(0), array2.GetLength(0),
            $"Array dimension [{array1.GetLength(0)},{array1.GetLength(1)}] wrong. Expected: [{array2.GetLength(0)},{array2.GetLength(0)}]");

        Assert.AreEqual(array1.GetLength(1), array2.GetLength(1),
            $"Array dimension [{array1.GetLength(0)},{array1.GetLength(1)}] wrong. Expected: [{array2.GetLength(0)},{array2.GetLength(0)}]");

        for (var x = 0; x < array1.GetLength(0); x++) {
            for (var y = 0; y < array1.GetLength(1); y++) {
                Assert.That(array1[x, y], Is.EqualTo(array2[x, y]), "Element at position: [" + x + "," + y + "]");
            }
        }
        return true;
    }
}