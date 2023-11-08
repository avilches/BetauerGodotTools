using Betauer.Core.Data;
using Betauer.TestRunner;

namespace Betauer.Core.Tests;

using NUnit.Framework;

[Betauer.TestRunner.Test]
[Only]
public class NormalizedGridTests {
    [Betauer.TestRunner.Test]
    public void Resize_GivenWidthAndHeight_SetsWidthAndHeight() {
        var grid = new NormalizedDataGrid(5, 3, (x, y) => 0f);
        grid.Resize(10, 7);

        Assert.AreEqual(10, grid.Width);
        Assert.AreEqual(7, grid.Height);
    }

    [Betauer.TestRunner.Test]
    public void Load_GivenValueFunc_PopulatesValuesDefaultNormalizeRange() {
        var grid = new NormalizedDataGrid(2, 2, (x, y) => x + y);
        grid.Load();

        Assert.AreEqual(0f , grid.GetValue(0, 0));
        Assert.AreEqual(0.5f, grid.GetValue(1, 0));
        Assert.AreEqual(0.5f, grid.GetValue(0, 1));
        Assert.AreEqual(1f , grid.GetValue(1, 1));
    }

    [Betauer.TestRunner.Test]
    public void Load_GivenValueFunc_PopulatesValuesAndNormalizesRange() {
        var grid = new NormalizedVirtualDataGrid(2, 2, (x, y) => x + y);
        grid.Load();

        Assert.AreEqual(0f , grid.GetValue(0, 0));
        Assert.AreEqual(0.5f, grid.GetValue(1, 0));
        Assert.AreEqual(0.5f, grid.GetValue(0, 1));
        Assert.AreEqual(1f , grid.GetValue(1, 1));
    }
}