using System.Collections.Generic;
using Betauer.Core.Data;
using Betauer.TestRunner;

namespace Betauer.Core.Tests;

using NUnit.Framework;

[Betauer.TestRunner.Test]
public class DataGridTests {
    [Betauer.TestRunner.Test]
    public void ParseTest() {
        var grid = DataGrid<int>.Parse("""
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
        var grid = new DataGrid<int>(2, 2).Load((x, y) => x + y);
        Assert.AreEqual(0, grid.GetValue(0, 0));
        Assert.AreEqual(1, grid.GetValue(1, 0));
        Assert.AreEqual(1, grid.GetValue(0, 1));
        Assert.AreEqual(2, grid.GetValue(1, 1));
    }

    private DataGrid<int> _xyDataGrid;

    [Betauer.TestRunner.SetUp]
    public void SetUp() {
        _xyDataGrid = new DataGrid<int>(5, 5);
        var value = 0;
        for (var y = 0; y < _xyDataGrid.Height; y++) {
            for (var x = 0; x < _xyDataGrid.Width; x++) {
                _xyDataGrid.SetValue(x, y, value);
                value++;
            }
        }
    }

    [Betauer.TestRunner.Test]
    public void TestWidthAndHeight() {
        Assert.AreEqual(5, _xyDataGrid.Width);
        Assert.AreEqual(5, _xyDataGrid.Height);
    }

    [Betauer.TestRunner.Test]
    public void TestFill() {
        _xyDataGrid.Fill(1);
        for (var y = 0; y < _xyDataGrid.Height; y++) {
            for (var x = 0; x < _xyDataGrid.Width; x++) {
                Assert.AreEqual(1, _xyDataGrid.GetValue(x, y));
            }
        }
    }

    [Betauer.TestRunner.Test]
    public void TestSetValue() {
        _xyDataGrid.SetValue(2, 2, 1);
        Assert.AreEqual(1, _xyDataGrid.GetValue(2, 2));
    }

    [Betauer.TestRunner.Test]
    public void TestGetValueSafe() {
        Assert.AreEqual(default(int), _xyDataGrid.GetValueSafe(6, 6));
    }

    [Betauer.TestRunner.Test]
    public void TestSetAll() {
        var newData = new int[5, 5];
        _xyDataGrid.SetAll(newData);
        Assert.AreEqual(newData, _xyDataGrid.Data);
    }

    [Betauer.TestRunner.Test]
    public void TestLoad() {
        _xyDataGrid.Load((x, y) => x + y);
        for (var y = 0; y < _xyDataGrid.Height; y++) {
            for (var x = 0; x < _xyDataGrid.Width; x++) {
                Assert.AreEqual(x + y, _xyDataGrid.GetValue(x, y));
            }
        }
    }

    [Betauer.TestRunner.Test]
    public void TestLoop() {
        var total = 0;
        _xyDataGrid.Loop((value, x, y) => total += value);
        Assert.AreEqual(total, 300);
    }

    [Betauer.TestRunner.Test]
    public void TestTransform() {
        var backup = new DataGrid<int>(_xyDataGrid.Data);
        _xyDataGrid.Transform(value => value + 1);
        _xyDataGrid.Loop((value, x, y) => Assert.AreEqual(backup[x,y] + 1, value));
    }

    [Betauer.TestRunner.Test]
    public void NormalizeTest() {
        var grid = new DataGrid<float>(2, 2).Load((x, y) => x + y);
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