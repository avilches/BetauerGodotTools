using System.IO;
using Betauer.Core.Image;
using Betauer.TestRunner;
using Betauer.TileSet.Image;

namespace Betauer.GameTools.Tests;

[TestRunner.Test]
[Only]
public class TileSetGeneratorTests {
    [SetUpClass]
    public void SetUp() {
        if (!Directory.Exists(".tmp")) {
            Directory.CreateDirectory(".tmp");
        }
    }

    [TestRunner.Test]
    public void BasicTest() {
        var source = new TileSetImage(new FastImage("test-resources/tileset/wang-subset-13.png"), TileSetLayouts.WangSubset13);
        source.ExportAs(TileSetLayouts.Minimal3X3Godot).SavePng(".tmp/wang-subset-13-export.png");
        source.ExportAs(TileSetLayouts.Minimal3X3Godot, TileSetImage.Blob47Rules).SavePng(".tmp/wang-subset-13-export-rules.png");
    }

}