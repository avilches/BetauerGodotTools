using System;
using System.IO;
using System.Linq;
using Betauer.Core.Image;
using Betauer.TestRunner;
using Betauer.TileSet.Image;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.TileSet;

[TestRunner.Test]
[Only]
public class TileSetLayoutTests : BaseBlobTests {
    [TestRunner.Test]
    public void BasicTest() {
        ITileSetLayout tileSet = new TileSetLayout(new int[,] { { 0, 1 }, { 2, 3 } });
        Assert.True(tileSet.HasTile(0));
        Assert.False(tileSet.HasTile(9));
        Assert.That(tileSet.GetTileIdByPosition(1, 1), Is.EqualTo(3));
        Assert.That(tileSet.GetTilePositionById(3), Is.EqualTo(new Vector2I(1, 1)));
        Assert.That(tileSet.Width, Is.EqualTo(2));
        Assert.That(tileSet.Height, Is.EqualTo(2));
        CollectionAssert.AreEquivalent(tileSet.GetTileIds(), new[] { 0, 1, 2, 3 });
        AreEqual(tileSet.Export(), new [,] { { 0, 1 }, { 2, 3 } });
        
    }

    [TestRunner.Test]
    public void BuilderTest() {

        var tileSet = new TileSetLayoutBuilder(2, 2);
        tileSet.AddTile(0, 0, 0);
        tileSet.AddTile(10, 1, 0);
        tileSet.AddTile(1, 0, 1);
        tileSet.AddTile(11, 1, 1);
        CollectionAssert.AreEquivalent(tileSet.GetTileIds(), new[] { 0, 10, 1, 11 });

        AreEqual(tileSet.Export(), new [,] { { 0, 10 }, { 1, 11 } });
    }
}