using Betauer.TestRunner;

namespace Betauer.Core.Tests;

using NUnit.Framework;

[Betauer.TestRunner.Test]
public class NormalizedGridTests {
    [Betauer.TestRunner.Test]
    public void Resize_GivenWidthAndHeight_SetsWidthAndHeight() {
        // Arrange
        var grid = new NormalizedGrid(5, 3, (x, y) => 0f);

        // Act
        grid.Resize(10, 7);

        // Assert
        Assert.AreEqual(10, grid.Width);
        Assert.AreEqual(7, grid.Height);
    }

    [Betauer.TestRunner.Test]
    public void NormalizedRange_GivenMinAndMax_SetsRangeMinAndRangeMax() {
        // Arrange
        var grid = new NormalizedGrid(5, 3, (x, y) => 0f);

        // Act
        grid.NormalizedRange(-10f, 10f);

        // Assert
        Assert.AreEqual(-10f, grid.RangeMin);
        Assert.AreEqual(10f, grid.RangeMax);
    }

    [Betauer.TestRunner.Test]
    public void Load_GivenValueFunc_PopulatesValuesDefaultNormalizeRange() {
        var grid = new NormalizedGrid(2, 2, (x, y) => x + y);
        grid.Load();

        Assert.AreEqual(0f , grid.GetValue(0, 0));
        Assert.AreEqual(0.5f, grid.GetValue(1, 0));
        Assert.AreEqual(0.5f, grid.GetValue(0, 1));
        Assert.AreEqual(1f , grid.GetValue(1, 1));
    }

    [Betauer.TestRunner.Test]
    public void Load_GivenValueFunc_PopulatesValuesAndNormalizesRange() {
        var grid = new NormalizedGrid(2, 2, (x, y) => x + y);
        grid.NormalizedRange(-1f, 1f);
        grid.Load();

        // Assert
        Assert.AreEqual(-1f,grid.GetValue(0, 0));
        Assert.AreEqual(0f, grid.GetValue(1, 0));
        Assert.AreEqual(0f, grid.GetValue(0, 1));
        Assert.AreEqual(1f,grid.GetValue(1, 1));
    }
}