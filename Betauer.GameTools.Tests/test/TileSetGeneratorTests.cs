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
        source.ExportAs(TileSetLayouts.Minimal3X3Godot).SavePng(".tmp/wang-47-export.png");
        source.ExportAs(TileSetLayouts.Minimal3X3Godot, TileSetImage.Blob47Rules).SavePng(".tmp/wang-47-export-rules.png");
    }

    [TestRunner.Test]
    public void CreateWangSubset13Test() {
        var source = new TileSetImage(new FastImage("test-resources/tileset/godot-full-example.png"), TileSetLayouts.Minimal3X3Godot);
        // source.ExportAs(TileSetLayouts.WangSubset13).SavePng("test-resources/tileset/wang-subset-13.png");
        source.ExportAs(TileSetLayouts.WangSubset13).SavePng(".tmp/wang-13-from-full.png");
    }
}