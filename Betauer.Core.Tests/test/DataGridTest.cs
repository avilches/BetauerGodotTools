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
                                       ...
                                       .##
                                       ..@
                                       """, new Dictionary<char, int> { { '#', 2 }, { '.', 0 }, { '@', 3 } });
        
        Assert.AreEqual(3, grid.Width);
        Assert.AreEqual(3, grid.Height);

        ArrayEquals(grid.Data, new[,] {
            { 0, 0, 0 },
            { 0, 2, 2 },
            { 0, 0, 3 }
        }.FlipDiagonal());

    }

    [Betauer.TestRunner.Test]
    public void RegularDataGridTests() {
        var grid = new DataGrid<int>(2, 2).Load((x, y) => x + y);
        Assert.AreEqual(0, grid.GetValue(0, 0));
        Assert.AreEqual(1, grid.GetValue(1, 0));
        Assert.AreEqual(1, grid.GetValue(0, 1));
        Assert.AreEqual(2, grid.GetValue(1, 1));
    }

    [Betauer.TestRunner.Test]
    public void NormalizedDataGridTests() {
        var grid = new NormalizedDataGrid(2, 2).Load((x, y) => x + y);
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

    private DataGrid<int> _dataGrid;

    [Betauer.TestRunner.SetUp]
    public void SetUp() {
        _dataGrid = new DataGrid<int>(5, 5);
        var value = 0;
        for (var y = 0; y < _dataGrid.Height; y++) {
            for (var x = 0; x < _dataGrid.Width; x++) {
                _dataGrid.SetValue(x, y, value);
                value++;
            }
        }
    }

    [Betauer.TestRunner.Test]
    public void TestWidthAndHeight() {
        Assert.AreEqual(5, _dataGrid.Width);
        Assert.AreEqual(5, _dataGrid.Height);
    }

    [Betauer.TestRunner.Test]
    public void TestFill() {
        _dataGrid.Fill(1);
        for (var y = 0; y < _dataGrid.Height; y++) {
            for (var x = 0; x < _dataGrid.Width; x++) {
                Assert.AreEqual(1, _dataGrid.GetValue(x, y));
            }
        }
    }

    [Betauer.TestRunner.Test]
    public void TestSetValue() {
        _dataGrid.SetValue(2, 2, 1);
        Assert.AreEqual(1, _dataGrid.GetValue(2, 2));
    }

    [Betauer.TestRunner.Test]
    public void TestGetValueSafe() {
        Assert.AreEqual(default(int), _dataGrid.GetValueSafe(6, 6));
    }

    [Betauer.TestRunner.Test]
    public void TestSetAll() {
        var newData = new int[5, 5];
        _dataGrid.SetAll(newData);
        Assert.AreEqual(newData, _dataGrid.Data);
    }

    [Betauer.TestRunner.Test]
    public void TestLoad() {
        _dataGrid.Load((x, y) => x + y);
        for (var y = 0; y < _dataGrid.Height; y++) {
            for (var x = 0; x < _dataGrid.Width; x++) {
                Assert.AreEqual(x + y, _dataGrid.GetValue(x, y));
            }
        }
    }

    [Betauer.TestRunner.Test]
    public void TestLoop() {
        var total = 0;
        _dataGrid.Loop((value, x, y) => total += value);
        Assert.AreEqual(total, 300);
    }

    [Betauer.TestRunner.Test]
    public void TestTransform() {
        var backup = new DataGrid<int>(_dataGrid.Data);
        _dataGrid.Transform(value => value + 1);
        _dataGrid.Loop((value, x, y) => Assert.AreEqual(backup[x,y] + 1, value));
    }

    [Betauer.TestRunner.Test]
    public void TestCopyRectTo() {
        var destination = new int[3, 3];
        _dataGrid.CopyRectTo(1, 1, destination);
        for (var y = 0; y < destination.GetLength(1); y++) {
            for (var x = 0; x < destination.GetLength(0); x++) {
                Assert.AreEqual(_dataGrid.GetValue(x + 1, y + 1), destination[x, y]);
            }
        }
    }

    [Betauer.TestRunner.Test]
    public void TestCopyCenterRectTo() {
        var destination = new int[3, 3];
        _dataGrid.CopyCenterRectTo(2, 2, 0, destination);
        ArrayEquals(destination, new[,] {
            {  6,  7,  8 },
            { 11, 12, 13 },
            { 16, 17, 18 }
        }.FlipDiagonal());
    }
    
    [Betauer.TestRunner.Test]
    public void TestCopyCenterRectToOutside() {
        var destination = new int[3, 3];
        _dataGrid.CopyCenterRectTo(0, 0, -1, destination);
        ArrayEquals(destination, new[,] {
            {  -1, -1, -1 }, 
            {  -1,  0,  1 }, 
            {  -1,  5,  6 }
        }.FlipDiagonal());
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