using System;
using System.IO;
using System.Linq;
using Betauer.Core.Image;
using Betauer.TestRunner;
using Betauer.TileSet.Image;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.TileSet;

[TestRunner.Test]
public class TileSetImageTests {
    [SetUpClass]
    public void SetUp() {
        if (!Directory.Exists(".tmp")) {
            Directory.CreateDirectory(".tmp");
        }
    }

    [TestRunner.Test]
    public void CreateBlob47FromWangSubset13() {
        var source = new TileSetImage(new FastImage().LoadResource("test-resources/tileset/wang-subset-13.png"), TileSetLayouts.WangSubset13);
        source.ExportAs(TileSetLayouts.Blob47Godot).SavePng(".tmp/wang-47-export.png");
        source.ExportAs(TileSetLayouts.Blob47Godot, TileSetImage.Blob47Rules).SavePng(".tmp/wang-47-export-rules.png");
    }

    [TestRunner.Test]
    public void EnsureBlob47RulesOnlyNeedWangSubset13() {
        var dependencies = TileSetImage.Blob47Rules.Select(it => it.Dependencies ?? Array.Empty<int>()).SelectMany(i => i).Distinct().ToList();
        // The dependencies don't use th 255, which is a block solid tile
        CollectionAssert.AreEquivalent(dependencies.Append(255), TileSetLayouts.WangSubset13.GetTileIds());
    }

    [TestRunner.Test]
    public void CreateWangSubset13TestFromBlob47() {
        var source = new TileSetImage(new FastImage().LoadResource( "test-resources/tileset/godot-full-example.png"), TileSetLayouts.Blob47Godot);
        // source.ExportAs(TileSetLayouts.WangSubset13).SavePng("test-resources/tileset/wang-subset-13.png");
        source.ExportAs(TileSetLayouts.WangSubset13).SavePng(".tmp/wang-13-from-full.png");
    }
}