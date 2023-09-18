using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Image;
using Godot;

namespace Betauer.TileSet.Image;

public class TileSetImage {

    public CellImage CellImage { get; }
    
    public int Columns => CellImage.Columns;
    public int Rows => CellImage.Rows;
    public int ImageWidth => CellImage.ImageWidth;
    public int ImageHeight => CellImage.ImageHeight;
    public int CellSize => CellImage.CellSize;
    public void SavePng(string filename) => CellImage.SavePng(filename);
    public byte[] GetPng() => CellImage.GetPng();

    public TileSetLayout Layout { get; }
    
    public IReadOnlyCollection<TileSetLayoutBuilder.TilePosition> GetTiles() => Layout.GetTiles();
    public IReadOnlyCollection<int> GetTileIds() => Layout.GetTileIds();
    public bool HasTile(int tileId) => Layout.HasTile(tileId);
    public (int, int) GetTilePositionById(int tileId) => Layout.GetTilePositionById(tileId);
    public int GetTileIdByPosition(int x, int y) => Layout.GetTileIdByPosition(x, y);

    public TileSetImage(TileSetLayout layout, int cellSize) {
        CellImage = new CellImage(layout.Columns, layout.Rows, cellSize);
        Layout = layout;
    }

    public TileSetImage(FastImage fastImage, TileSetLayout layout) {
        CellImage = new CellImage(fastImage, layout.Rows, layout.Columns);
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
            Console.WriteLine("Checking rules...");
            rules.RemoveAll(rule => {
                if (tilesGenerated.Contains(rule.TileId)) {
                    Console.WriteLine($"Removing rule. Tile {rule.TileId} already processed");
                    // Delete the rules that are already applied
                    return true;
                }
                if (!destination.HasTile(rule.TileId)) {
                    // TODO: the rule could be applied, but there is no place to put the tile generated. It can be placed in some of temporal
                    // place and make it available for the next rules so they can use to generate other tiles that are present or needed in the
                    Console.WriteLine($"Removing rule. Tile {rule.TileId} is not present in destination");
                    // Delete the rules that are already applied
                    return true;
                }
                if (rule.Dependencies?.All(tilesGenerated.Contains) ?? true) {
                    Console.WriteLine($"Generating tile {rule.TileId}...");
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
        tilesPending.ForEach(tileId => layout.RemoveTile(tileId));
        return destination;
    }

    private static List<BaseRule>? _blob47Rules;
    public static List<BaseRule> Blob47Rules => _blob47Rules ??= new List<BaseRule> {
        // *
        // |
        // *
        new StepRule(16).Do(20, StepRule.Step.LH).Do(80, StepRule.Step.RH),
        new StepRule(17).Do(21, StepRule.Step.LH).Do(81, StepRule.Step.RH),
        new StepRule(1).Do(5, StepRule.Step.LH).Do(65, StepRule.Step.RH),
        
        // *-*
        new StepRule(4).Do(20, StepRule.Step.TH).Do(5, StepRule.Step.BH),
        new StepRule(68).Do(84, StepRule.Step.TH).Do(69, StepRule.Step.BH),
        new StepRule(64).Do(80, StepRule.Step.TH).Do(65, StepRule.Step.BH),

        // *
        new StepRule(0).Do(20, StepRule.Step.TLQ).Do(80, StepRule.Step.TRQ)
            .Do(5, StepRule.Step.BLQ).Do(64, StepRule.Step.BRQ),
        
    };

}