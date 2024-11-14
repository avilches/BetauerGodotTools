using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath.Array2D;
using Betauer.Core.DataMath.Data;
using Betauer.TestRunner;
using Godot;

namespace Betauer.Core.Tests;

using NUnit.Framework;

[Betauer.TestRunner.Test]
public class Array2DTests {
    private Array2D<int> _array2D;

    [Betauer.TestRunner.SetUp]
    public void SetUp() {
        _array2D = new Array2D<int>(5, 4);
        var value = 0;
        foreach (var cell in _array2D) {
            _array2D.SetValue(cell.Position, value);
            value++;
        }
    }

    [Betauer.TestRunner.Test]
    public void ParseTest() {
        var grid = Array2D<int>.Parse("""
                                       ...#
                                       @###
                                       ..#@
                                       """, new Dictionary<char, int> { { '#', 2 }, { '.', 0 }, { '@', 3 } });
        
        Assert.AreEqual(4, grid.Width);
        Assert.AreEqual(3, grid.Height);

        ArrayEquals(grid.Data, new[,] {
            { 0, 0, 0, 2 },
            { 3, 2, 2, 2 },
            { 0, 0, 2, 3 }
        });

        // Direct access to the array is y, x
        Assert.AreEqual(grid.Data[0, 0], 0);
        Assert.AreEqual(grid.Data[0, 1], 0);
        Assert.AreEqual(grid.Data[0, 2], 0);
        Assert.AreEqual(grid.Data[0, 3], 2);
        Assert.AreEqual(grid.Data[1, 0], 3);
        Assert.AreEqual(grid.Data[2, 3], 3);
        
        // Access through the GetValue method is x, y
        
        Assert.AreEqual(grid.GetValue(new Vector2I(0, 0)), 0);
        Assert.AreEqual(grid.GetValue(new Vector2I(1, 0)), 0);
        Assert.AreEqual(grid.GetValue(new Vector2I(2, 0)), 0);
        Assert.AreEqual(grid.GetValue(new Vector2I(3, 0)), 2);
        Assert.AreEqual(grid.GetValue(new Vector2I(0, 1)), 3);
        Assert.AreEqual(grid.GetValue(new Vector2I(3, 2)), 3);
        
        Assert.AreEqual(grid[0, 0], 0);
        Assert.AreEqual(grid[1, 0], 0);
        Assert.AreEqual(grid[2, 0], 0);
        Assert.AreEqual(grid[3, 0], 2);
        Assert.AreEqual(grid[0, 1], 3);
        Assert.AreEqual(grid[3, 2], 3);
        
        Assert.AreEqual(grid[new Vector2I(0, 0)], 0);
        Assert.AreEqual(grid[new Vector2I(1, 0)], 0);
        Assert.AreEqual(grid[new Vector2I(2, 0)], 0);
        Assert.AreEqual(grid[new Vector2I(3, 0)], 2);
        Assert.AreEqual(grid[new Vector2I(0, 1)], 3);
        Assert.AreEqual(grid[new Vector2I(3, 2)], 3);
    }

    [Betauer.TestRunner.Test]
    public void TestWidthAndHeight() {
        Assert.AreEqual(5, _array2D.Width);
        Assert.AreEqual(4, _array2D.Height);
    }

    [Betauer.TestRunner.Test]
    public void TestFill() {
        _array2D.Fill(1);
        for (var y = 0; y < _array2D.Height; y++) {
            for (var x = 0; x < _array2D.Width; x++) {
                Assert.AreEqual(1, _array2D[x, y]);
            }
        }
        
        var original = new Array2D<int>(new[,] {
            {  0,  1,  2,  3,  4 },
            { 10, 11, 12, 13, 14 },
            { 20, 21, 22, 23, 24 },
            { 30, 31, 32, 33, 34 },
        });
        original.Fill(2, 1, 3, 2, 8);
        ArrayEquals(original.Data, new[,] {
            {  0,  1,  2,  3,  4 },
            { 10, 11,  8,  8,  8 },
            { 20, 21,  8,  8,  8 },
            { 30, 31, 32, 33, 34 },
        });
    }

    [Betauer.TestRunner.Test]
    public void TestSetValue() {
        _array2D.SetValue(new Vector2I(1, 2), 123);
        Assert.AreEqual(123, _array2D.GetValue(new Vector2I(1, 2)));
        Assert.AreEqual(123, _array2D[new Vector2I(1, 2)]);
        Assert.AreEqual(123, _array2D[1, 2]);
        
        _array2D[new Vector2I(1, 2)] = 1234;
        Assert.AreEqual(1234, _array2D.GetValue(new Vector2I(1, 2)));
        Assert.AreEqual(1234, _array2D[new Vector2I(1, 2)]);
        Assert.AreEqual(1234, _array2D[1, 2]);
        
        _array2D[1, 2] = 12345;
        Assert.AreEqual(12345, _array2D.GetValue(new Vector2I(1, 2)));
        Assert.AreEqual(12345, _array2D[new Vector2I(1, 2)]);
        Assert.AreEqual(12345, _array2D[1, 2]);
    }

    [Betauer.TestRunner.Test]
    public void TestGetValueSafe() {
        Assert.AreEqual(default(int), _array2D.GetValueSafe(16, 16));
    }

    [Betauer.TestRunner.Test]
    public void TestLoad() {
        _array2D.Load((x, y) => x + y);
        for (var y = 0; y < _array2D.Height; y++) {
            for (var x = 0; x < _array2D.Width; x++) {
                Assert.AreEqual(x + y, _array2D[x, y]);
            }
        }
        var original = new Array2D<int>(new[,] {
            {  0,  1,  2,  3,  4 },
            { 10, 11, 12, 13, 14 },
            { 20, 21, 22, 23, 24 },
            { 30, 31, 32, 33, 34 },
        });
        original.Load(2, 1, 3, 2, (x,y) => 8);
        ArrayEquals(original.Data, new[,] {
            {  0,  1,  2,  3,  4 },
            { 10, 11,  8,  8,  8 },
            { 20, 21,  8,  8,  8 },
            { 30, 31, 32, 33, 34 },
        });

    }

    [Betauer.TestRunner.Test]
    public void TestExport() {
        var exported = _array2D.Export(value => ""+value);
        for (var y = 0; y < _array2D.Height; y++) {
            for (var x = 0; x < _array2D.Width; x++) {
                Assert.AreEqual(""+_array2D[x, y], exported[x, y]);
            }
        }
        var original = new Array2D<int>(new[,] {
            {  0,  1,  2,  3,  4 },
            { 10, 11, 12, 13, 14 },
            { 20, 21, 22, 23, 24 },
            { 30, 31, 32, 33, 34 },
        });
        var exported2 = original.Export(2, 1, 3, 2, value => ""+value);
        ArrayEquals(exported2.Data, new[,] {
            {   "12",  "13",  "14" },
            {   "22",  "23",  "24" },
        });

    }

    [Betauer.TestRunner.Test]
    public void TestLoop() {
        var total = 0;
        foreach (var cell in _array2D) {
            total += cell.Value;
        }
        Assert.AreEqual(total, 190);

        var total2 = _array2D.Sum(cell => cell.Value);
        Assert.AreEqual(total2, 190);
    }

    [Betauer.TestRunner.Test]
    public void TestTransform() {
        var original = new Array2D<int>(new[,] {
            {  0,  1,  2,  3,  4 },
            { 10, 11, 12, 13, 14 },
            { 20, 21, 22, 23, 24 },
            { 30, 31, 32, 33, 34 },
        });
        original.Transform(value => value + 1);
        ArrayEquals(original.Data, new[,] {
            {  1,  2,  3,  4,  5 },
            { 11, 12, 13, 14, 15 },
            { 21, 22, 23, 24, 25 },
            { 31, 32, 33, 34, 35 },
        });

        original = new Array2D<int>(new[,] {
            {  0,  1,  2,  3,  4 },
            { 10, 11, 12, 13, 14 },
            { 20, 21, 22, 23, 24 },
            { 30, 31, 32, 33, 34 },
        });
        original.Transform(2, 1, 3, 2, value => value + 1);
        ArrayEquals(original.Data, new[,] {
            {  0,  1,  2,  3,  4 },
            { 10, 11, 13, 14, 15 },
            { 20, 21, 23, 24, 25 },
            { 30, 31, 32, 33, 34 },
        });

    }

    [Betauer.TestRunner.Test]
    public void NormalizeTest() {
        var grid = new Array2D<float>(2, 2).Load((x, y) => x + y);
        Assert.AreEqual(2, grid.Width);
        Assert.AreEqual(2, grid.Height);

        Assert.AreEqual(0f, grid[0, 0]);
        Assert.AreEqual(1f, grid[1, 0]);
        Assert.AreEqual(1f, grid[0, 1]);
        Assert.AreEqual(2f, grid[1, 1]);

        grid.Normalize();

        Assert.AreEqual(0f, grid[0, 0]);
        Assert.AreEqual(0.5f, grid[1, 0]);
        Assert.AreEqual(0.5f, grid[0, 1]);
        Assert.AreEqual(1f, grid[1, 1]);

        grid.Normalize(0f, 1f);

        Assert.AreEqual(0f, grid[0, 0]);
        Assert.AreEqual(0.5f, grid[1, 0]);
        Assert.AreEqual(0.5f, grid[0, 1]);
        Assert.AreEqual(1f, grid[1, 1]);

        grid.Normalize(-10f, 30f);
        Assert.AreEqual(-10f, grid[0, 0]);
        Assert.AreEqual(10f, grid[1, 0]);
        Assert.AreEqual(10f, grid[0, 1]);
        Assert.AreEqual(30f, grid[1, 1]);

        grid.Normalize(0f, 1f);
        Assert.AreEqual(0f, grid[0, 0]);
        Assert.AreEqual(0.5f, grid[1, 0]);
        Assert.AreEqual(0.5f, grid[0, 1]);
        Assert.AreEqual(1f, grid[1, 1]);
    }
    
    [TestRunner.Test]
    public void CopyGridTests() {
        var original = new Array2D<int>(new[,] {
            {  0,  1,  2,  3,  4 },
            { 10, 11, 12, 13, 14 },
            { 20, 21, 22, 23, 24 },
            { 30, 31, 32, 33, 34 },
        });
            
        var dest = new int[2, 2];
        original.CopyTo(0, 0, dest);
        ArrayEquals(dest, new[,] {
            {  0,  1, },
            { 10, 11, },
        });

        original.CopyTo(1, 2, dest);
        ArrayEquals(dest, new[,] {
            { 21, 22, },
            { 31, 32, },
        });

        
        var buffer = new int[3, 3];
        original.CopyNeighbors(0, 0, buffer, -1);
        ArrayEquals(buffer, new[,] {
            { -1, -1, -1 },
            { -1,  0,  1 },
            { -1, 10, 11 },
        });

        original.CopyNeighbors(1, 2, buffer, -1);
        ArrayEquals(buffer, new[,] {
            { 10, 11, 12 },
            { 20, 21, 22 },
            { 30, 31, 32 },
        });
    }

    public static bool ArrayEquals<T>(T[,] array1, T[,] array2) {
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