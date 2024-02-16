using Betauer.Core.Data;
using Betauer.TestRunner;

namespace Betauer.Core.Tests;

using NUnit.Framework;

[Betauer.TestRunner.Test]
public class NormalizedGridTests {
    
    [Betauer.TestRunner.Test]
    public void NormalizedVirtualDataGridTests() {
        var grid = new NormalizedDataGrid(2, 2).Load((x, y) => x + y);
        Assert.AreEqual(2, grid.Width);
        Assert.AreEqual(2, grid.Height);
        
        Assert.AreEqual(0f , grid.GetValue(0, 0));
        Assert.AreEqual(1f, grid.GetValue(1, 0));
        Assert.AreEqual(1f, grid.GetValue(0, 1));
        Assert.AreEqual(2f , grid.GetValue(1, 1));

        grid.Normalize();

        Assert.AreEqual(0f , grid.GetValue(0, 0));
        Assert.AreEqual(0.5f, grid.GetValue(1, 0));
        Assert.AreEqual(0.5f, grid.GetValue(0, 1));
        Assert.AreEqual(1f , grid.GetValue(1, 1));
        
        grid.Normalize(0f, 1f);

        Assert.AreEqual(0f , grid.GetValue(0, 0));
        Assert.AreEqual(0.5f, grid.GetValue(1, 0));
        Assert.AreEqual(0.5f, grid.GetValue(0, 1));
        Assert.AreEqual(1f , grid.GetValue(1, 1));

        grid.Normalize(-10f, 30f);
        Assert.AreEqual(-10f , grid.GetValue(0, 0));
        Assert.AreEqual(10f, grid.GetValue(1, 0));
        Assert.AreEqual(10f, grid.GetValue(0, 1));
        Assert.AreEqual(30f , grid.GetValue(1, 1));

        grid.Normalize(0f, 1f);
        Assert.AreEqual(0f , grid.GetValue(0, 0));
        Assert.AreEqual(0.5f, grid.GetValue(1, 0));
        Assert.AreEqual(0.5f, grid.GetValue(0, 1));
        Assert.AreEqual(1f , grid.GetValue(1, 1));

    }
}