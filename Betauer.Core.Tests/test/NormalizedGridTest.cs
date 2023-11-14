using Betauer.Core.Data;
using Betauer.TestRunner;

namespace Betauer.Core.Tests;

using NUnit.Framework;

[Betauer.TestRunner.Test]
public class NormalizedGridTests {
    [Betauer.TestRunner.Test]
    public void NormalizedFloatDataGridTests() {
        var grid = new NormalizedDataGrid(2, 2);
        grid.Load((x, y) => x + y);
        grid.Normalize();

        Assert.AreEqual(0f , grid.GetValue(0, 0));
        Assert.AreEqual(0.5f, grid.GetValue(1, 0));
        Assert.AreEqual(0.5f, grid.GetValue(0, 1));
        Assert.AreEqual(1f , grid.GetValue(1, 1));
    }
    
    [Betauer.TestRunner.Test]
    public void NormalizedVirtualDataGridTests() {
        var grid = new NormalizedVirtualDataGrid(2, 2, (x, y) => x + y);
        Assert.AreEqual(2, grid.Width);
        Assert.AreEqual(2, grid.Height);
        grid.Normalize();

        Assert.AreEqual(0f , grid.GetValue(0, 0));
        Assert.AreEqual(0.5f, grid.GetValue(1, 0));
        Assert.AreEqual(0.5f, grid.GetValue(0, 1));
        Assert.AreEqual(1f , grid.GetValue(1, 1));
        
        grid.Resize(10, 7);
        grid.Normalize();

        Assert.AreEqual(10, grid.Width);
        Assert.AreEqual(7, grid.Height);

    }
}