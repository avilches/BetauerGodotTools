using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Image;
using Godot;

namespace Betauer.TileSet.Image;

public class TileSetImage {

    public CellImage CellImage { get; }
    
    public int Width => CellImage.Width;
    public int Height => CellImage.Height;
    public int ImageWidth => CellImage.ImageWidth;
    public int ImageHeight => CellImage.ImageHeight;
    public int CellSize => CellImage.CellSize;
    public void SavePng(string filename) => CellImage.SavePng(filename);
    public byte[] GetPng() => CellImage.GetPng();

    public TileSetLayout Layout { get; }
    
    public IReadOnlyCollection<TileSetLayout.TilePosition> GetTiles() => Layout.GetTiles();
    public IReadOnlyCollection<int> GetTileIds() => Layout.GetTileIds();
    public bool HasTile(int tileId) => Layout.HasTile(tileId);
    public (int, int) GetTilePositionById(int tileId) => Layout.GetTilePositionById(tileId);
    public int GetTileIdByPosition(int x, int y) => Layout.GetTileIdByPosition(x, y);

    public TileSetImage(TileSetLayout layout, int cellSize) {
        CellImage = new CellImage(layout.Width, layout.Height, cellSize);
        Layout = layout;
    }

    public TileSetImage(string resourcePath, TileSetLayout layout, global::Godot.Image.Format? format = null) {
        CellImage = new CellImage(resourcePath, layout.Width, layout.Height, format);
        Layout = layout;
    }

    public TileSetImage(FastImage fastImage, TileSetLayout layout) {
        CellImage = new CellImage(fastImage, layout.Width, layout.Height);
        Layout = layout;
    }

    public Color[,] Copy(int tileId) {
        var (cellX, cellY) = GetTilePositionById(tileId);
        return CellImage.Copy(cellX, cellY);
    }

    public Color[,] CopyPart(int tileId, int x, int y, int width, int height) {
        var (cellX, cellY) = GetTilePositionById(tileId);
        return CellImage.Copy(cellX, cellY, x, y, width, height);
    }

    public void Paste(Color[,] cell, int tileId, bool blend = true) {
        var (cellX, cellY) = GetTilePositionById(tileId);
        CellImage.Paste(cell, cellX, cellY, blend);
    }

    public void PastePart(Color[,] cell, int tileId, int x, int y, bool blend = true) {
        var (cellX, cellY) = GetTilePositionById(tileId);
        CellImage.PastePart(cell, cellX, cellY, x, y, blend);
    }

    public TileSetImage ExportAs(TileSetLayout layout) {
        var destination = new TileSetImage(layout, CellSize);
        layout.GetTileIds().ForEach(tileId => {
            if (HasTile(tileId)) {
                Copy(tileId).PasteTL(destination, tileId);
            }
        });
        return destination;
    }    

    public TileSetImage ExportAs(TileSetLayout desiredLayout, List<BaseRule> originalRules) {
        var layout = new TileSetLayoutBuilder(desiredLayout);
        var destination = new TileSetImage(layout, CellSize);
        var tilesGenerated = new HashSet<int>();
        // First copy all tiles that are already in the source
        var tilesPending = new List<int>(destination.GetTileIds());
        tilesPending.RemoveAll(tileId => {
            if (HasTile(tileId)) {
                Copy(tileId).PasteTL(destination, tileId);
                tilesGenerated.Add(tileId);
                return true;
            }
            return false;
        });

        var rules = new List<BaseRule>(originalRules); // clone the rules so it can be removed when they are applied
        var canApplyMoreRules = true;
        while (canApplyMoreRules) {
            canApplyMoreRules = false;
            // Console.WriteLine("Checking rules...");
            rules.RemoveAll(rule => {
                if (tilesGenerated.Contains(rule.TileId)) {
                    // Console.WriteLine($"Removing rule. Tile {rule.TileId} already processed");
                    // Delete the rules that are already applied
                    return true;
                }
                if (!destination.HasTile(rule.TileId)) {
                    // TODO: the rule could be applied, but there is no place to put the tile generated. It can be placed in some of temporal
                    // place and make it available for the next rules so they can use to generate other tiles that are present or needed in the
                    // Console.WriteLine($"Removing rule. Tile {rule.TileId} is not present in destination");
                    // Delete the rules that are already applied
                    return true;
                }
                if (rule.Dependencies?.All(tilesGenerated.Contains) ?? true) {
                    // Console.WriteLine($"Generating tile {rule.TileId}...");
                    rule.Apply(destination);
                    tilesGenerated.Add(rule.TileId);
                    tilesPending.Remove(rule.TileId);
                    canApplyMoreRules = true; // if a rule is applied, maybe this rule helps to another one, so we need to apply again
                    return true;
                } else {
                    Console.WriteLine($"Ignoring rule for {rule.TileId}, it needs tiles {string.Join(",", rule.Dependencies??Array.Empty<int>())}");
                }
                return false;
            });
        }
        // Remove from the layout the empty tiles (not copied, not generated by rules from the source)
        tilesPending.ForEach(tileId => layout.RemoveTile(tileId));
        return destination;
    }

    private static List<BaseRule>? _blob47Rules;
    public static List<BaseRule> Blob47Rules => _blob47Rules ??= new List<BaseRule> {
        // *
        // |
        // *
        new StepRule(16).LeftRight(28,112),
        new StepRule(17).LeftRight(31,241),
        new StepRule(1).LeftRight(7, 193),
        
        // *-*
        new StepRule(4).UpDown(28, 7),
        new StepRule(68).UpDown(124, 199),
        new StepRule(64).UpDown(112, 193),

        // *
        new StepRule(0).Do(28, StepRule.Step.UpLeftQuarter).Do(112, StepRule.Step.UpRightQuarter)
            .Do(7, StepRule.Step.DownLeftQuarter).Do(193, StepRule.Step.DownRightQuarter),
        
        new StepRule(20).Quarters(28, 28, 28, 247),
        new StepRule(84).Quarters(124, 124, 223, 247),
        new StepRule(80).Quarters(112, 112, 223, 112),
        new StepRule(21).Quarters(31, 253,31, 247),
        new StepRule(85).Quarters(127,253, 223, 247),
        new StepRule(81).Quarters(127, 241, 223, 241),
        new StepRule( 5).Quarters(7, 253, 7, 7),
        new StepRule(69).Quarters(127, 253, 199, 199),
        new StepRule(65).Quarters(127, 193, 193, 193),

        new StepRule(213).Quarters(193, 253, 223, 247),
        new StepRule(92).Quarters(124, 124, 223, 28),
        new StepRule(116).Quarters(124, 124, 112, 247),
        new StepRule(87).Quarters(127, 7, 223, 247),
        new StepRule(29).Quarters(31, 253, 31, 28),
        // 127
        // 253
        new StepRule(113).Quarters(127, 241, 112, 241),
        new StepRule(23).Quarters(31, 7, 31, 247),
        // 223
        // 247
        new StepRule(209).Quarters(193, 241, 223, 241),
        new StepRule(117).Quarters(127, 253, 112, 247),
        new StepRule(71).Quarters(127, 7, 199, 199),
        new StepRule(197).Quarters(193, 253, 199, 199),
        new StepRule(93).Quarters(127, 253, 223, 28),
        
        
        new StepRule(125).Quarters(127, 253, 124, 124),
        new StepRule(119).Quarters(127, 7, 112, 247),
        new StepRule(245).Quarters(241, 253, 241, 247),
        new StepRule(95).Quarters(127, 31, 223, 31),
        new StepRule(221).Quarters(193, 253, 223, 28),
        new StepRule(215).Quarters(199,199, 223, 247),
    };

}