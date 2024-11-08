using System.Collections.Generic;
using Betauer.Core.DataMath.Data;
using Betauer.TestRunner;

namespace Betauer.Core.Tests;

using NUnit.Framework;

[Betauer.TestRunner.Test]
public class YxDataGridTests {
    private YxDataGrid<int> _yxDataGrid;

    [Betauer.TestRunner.SetUp]
    public void SetUp() {
        _yxDataGrid = new YxDataGrid<int>(5, 4);
        var value = 0;
        for (var y = 0; y < _yxDataGrid.Height; y++) {
            for (var x = 0; x < _yxDataGrid.Width; x++) {
                _yxDataGrid.SetValue(x, y, value);
                value++;
            }
        }
    }

    [Betauer.TestRunner.Test]
    public void ParseTest() {
        var grid = YxDataGrid<int>.Parse("""
                                       ...#
                                       .###
                                       ..@#
                                       """, new Dictionary<char, int> { { '#', 2 }, { '.', 0 }, { '@', 3 } });
        
        Assert.AreEqual(4, grid.Width);
        Assert.AreEqual(3, grid.Height);

        ArrayEquals(grid.Data, new[,] {
            { 0, 0, 0, 2 },
            { 0, 2, 2, 2 },
            { 0, 0, 3, 2 }
        });

    }

    [Betauer.TestRunner.Test]
    public void RegularDataGridTests() {
        var grid = new YxDataGrid<int>(2, 2).Load((x, y) => x + y);
        Assert.AreEqual(0, grid.GetValue(0, 0));
        Assert.AreEqual(1, grid.GetValue(1, 0));
        Assert.AreEqual(1, grid.GetValue(0, 1));
        Assert.AreEqual(2, grid.GetValue(1, 1));
    }

    [Betauer.TestRunner.Test]
    public void TestWidthAndHeight() {
        Assert.AreEqual(5, _yxDataGrid.Width);
        Assert.AreEqual(4, _yxDataGrid.Height);
    }

    [Betauer.TestRunner.Test]
    public void TestFill() {
        _yxDataGrid.Fill(1);
        for (var y = 0; y < _yxDataGrid.Height; y++) {
            for (var x = 0; x < _yxDataGrid.Width; x++) {
                Assert.AreEqual(1, _yxDataGrid.GetValue(x, y));
            }
        }
    }

    [Betauer.TestRunner.Test]
    public void TestSetValue() {
        _yxDataGrid.SetValue(2, 2, 1);
        Assert.AreEqual(1, _yxDataGrid.GetValue(2, 2));
    }

    [Betauer.TestRunner.Test]
    public void TestGetValueSafe() {
        Assert.AreEqual(default(int), _yxDataGrid.GetValueSafe(6, 6));
    }

    [Betauer.TestRunner.Test]
    public void TestLoad() {
        _yxDataGrid.Load((x, y) => x + y);
        for (var y = 0; y < _yxDataGrid.Height; y++) {
            for (var x = 0; x < _yxDataGrid.Width; x++) {
                Assert.AreEqual(x + y, _yxDataGrid.GetValue(x, y));
            }
        }
    }

    [Betauer.TestRunner.Test]
    public void TestLoop() {
        var total = 0;
        _yxDataGrid.Loop((value, x, y) => total += value);
        Assert.AreEqual(total, 190);
    }

    [Betauer.TestRunner.Test]
    public void TestTransform() {
        var backup = _yxDataGrid.Clone();
        _yxDataGrid.Transform(value => value + 1);
        _yxDataGrid.Loop((value, x, y) => Assert.AreEqual(backup[x,y] + 1, value));
    }

    [Betauer.TestRunner.Test]
    public void NormalizeTest() {
        var grid = new YxDataGrid<float>(2, 2).Load((x, y) => x + y);
        Assert.AreEqual(2, grid.Width);
        Assert.AreEqual(2, grid.Height);

        Assert.AreEqual(0f, grid.GetValue(0, 0));
        Assert.AreEqual(1f, grid.GetValue(1, 0));
        Assert.AreEqual(1f, grid.GetValue(0, 1));
        Assert.AreEqual(2f, grid.GetValue(1, 1));

        grid.Normalize();

        Assert.AreEqual(0f, grid.GetValue(0, 0));
        Assert.AreEqual(0.5f, grid.GetValue(1, 0));
        Assert.AreEqual(0.5f, grid.GetValue(0, 1));
        Assert.AreEqual(1f, grid.GetValue(1, 1));

        grid.Normalize(0f, 1f);

        Assert.AreEqual(0f, grid.GetValue(0, 0));
        Assert.AreEqual(0.5f, grid.GetValue(1, 0));
        Assert.AreEqual(0.5f, grid.GetValue(0, 1));
        Assert.AreEqual(1f, grid.GetValue(1, 1));

        grid.Normalize(-10f, 30f);
        Assert.AreEqual(-10f, grid.GetValue(0, 0));
        Assert.AreEqual(10f, grid.GetValue(1, 0));
        Assert.AreEqual(10f, grid.GetValue(0, 1));
        Assert.AreEqual(30f, grid.GetValue(1, 1));

        grid.Normalize(0f, 1f);
        Assert.AreEqual(0f, grid.GetValue(0, 0));
        Assert.AreEqual(0.5f, grid.GetValue(1, 0));
        Assert.AreEqual(0.5f, grid.GetValue(0, 1));
        Assert.AreEqual(1f, grid.GetValue(1, 1));
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