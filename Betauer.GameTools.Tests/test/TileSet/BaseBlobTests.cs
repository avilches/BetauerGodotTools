using Betauer.Core;
using Betauer.Core.Data;
using Betauer.TileSet.Terrain;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.TileSet;

public abstract class BaseBlobTests {
    protected void AssertBlob47(string str, int[,] grid) {
        var source = XyDataGrid<int>.Parse(str, new System.Collections.Generic.Dictionary<char, int> {
            {'#',  0},
            {'.', -1}
        });
        
        var tileIds = new int[3, 3];
        var buffer = new int[3, 3];
        source.Loop((value, x, y) => {
            source.Data.CopyXyCenterRect(x, y, -1, buffer);
            var tileId = TilePatternRuleSets.Blob47.FindXyTilePatternId(buffer, -1);
            tileIds[x,y] = tileId;
        });
        
        ArrayEquals(tileIds, grid.YxFlipDiagonal());
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