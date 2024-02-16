using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Betauer.Core;
using Betauer.TileSet.Image;
using Betauer.TileSet.Terrain;
using Betauer.TileSet.TileMap.Handlers;
using Godot;

namespace Betauer.TileSet.TileMap;

public class TileMap<TTerrain> : TileMap where TTerrain : Enum {
    public TileMap(int layers, int width, int height, TTerrain defaultTerrain = default) : base(layers, width, height, defaultTerrain.ToInt()) {
    }

    public TTerrain GetTerrainEnum(int x, int y) {
        return GetTerrain(x, y).ToEnum<TTerrain>();
    }

    public void SetTerrain(int x, int y, TTerrain terrain) {
        TerrainGrid[x, y] = terrain.ToInt();
    }

    public void SetTerrainGrid(int x, int y, TTerrain[,] grid) {
        for (var xx = 0; xx < grid.GetLength(0); xx++) {
            for (var yy = 0; yy < grid.GetLength(1); yy++) {
                TerrainGrid[x + xx, y + yy] = grid[xx, yy].ToInt();
            }
        }
    }

    public void SetTerrainGrid(int x, int y, int width, int height, TTerrain terrain) {
        SetTerrainGrid(x, y, width, height, terrain.ToInt());
    }

    public TileActionBuilder IfTerrain<T>(T terrain) where T : Enum {
        return NewAction().IfTerrain(terrain);
    }

    public static TileMap<T> Parse<T>(string value, Dictionary<char, T> charToEnum, int layers = 1) where T : Enum {
        var lines = TileMap.Parse(value);
        var maxLength = lines[0].Length; // all lines have the same length
        var tileMap = new TileMap<T>(layers, maxLength, lines.Count);
        var y = 0;
        foreach (var line in lines) {
            var x = 0;
            foreach (var terrainEnum in line.Select(c => charToEnum[c])) {
                tileMap.SetTerrain(x, y, terrainEnum);
                x++;
            }
            y++;
        }
        return tileMap;
    }
}

public class TileMap {
    public struct TileInfo {
        public int SourceId { get; set; }
        public Vector2I? AtlasCoords { get; set; }

        public TileInfo() {
            Clear();
        }

        public void Clear() {
            SourceId = 0;
            AtlasCoords = null;
        }
    }

    private readonly List<TileActionBuilder> _pendingPipelines = new();

    public int Layers { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }

    public int[,] TerrainGrid { get; }
    public int[,] TileId { get; }
    public TileInfo[][,] TileInfoGrid { get; }

    private readonly int _defaultTerrain;

    public TileMap(int layers, int width, int height, int defaultTerrain = -1) {
        Width = width;
        Height = height;
        Layers = layers;
        TileInfoGrid = new TileInfo[layers][,];
        TerrainGrid = new int[width, height];
        TileId = new int[width, height];
        _defaultTerrain = defaultTerrain;
        
        for (var xx = 0; xx < width; xx++) {
            for (var yy = 0; yy < height; yy++) {
                TerrainGrid[xx, yy] = _defaultTerrain;
                TileId[xx, yy] = -1;
            }
        }

        for (var layer = 0; layer < Layers; layer++) {
            TileInfoGrid[layer] = new TileInfo[width, height];
        }
    }

    public TileMapSource CreateSource(int sourceId, ITileSetLayout tileSetLayout) {
        return new TileMapSource(this, sourceId, tileSetLayout);
    }
    
    public ref TileInfo GetCellInfoRef(int layer, int x, int y) {
        return ref TileInfoGrid[layer][x, y];
    }

    public void Clear() {
        Clear(0, 0, Width, Height);
    }

    public void Clear(int x, int y, int width, int height) {
        for (var xx = 0; xx < width; xx++) {
            for (var yy = 0; yy < height; yy++) {
                TerrainGrid[xx, yy] = _defaultTerrain;
                TileId[xx, yy] = -1;
            }
        }
        for (var layer = 0; layer < Layers; layer++) {
            for (var xx = 0; xx < width; xx++) {
                for (var yy = 0; yy < height; yy++) {
                    ref var currentInfo = ref TileInfoGrid[layer][x + xx, y + yy];
                    currentInfo.Clear();
                }
            }
        }
    }

    public void ClearCell(int x, int y) {
        TerrainGrid[x, y] = _defaultTerrain;
        TileId[x, y] = -1;
        for (var layer = 0; layer < Layers; layer++) {
            ref var currentInfo = ref TileInfoGrid[layer][x, y];
            currentInfo.Clear();
        }
    }

    public int GetTerrain(int x, int y) {
        return x < 0 || x > Width || y < 0 || y > Height ? _defaultTerrain : TerrainGrid[x, y];
    }

    public bool SetTerrain(int x, int y, int terrain) {
        if (TerrainGrid[x, y] == terrain) return false;
        TerrainGrid[x, y] = terrain;
        return true;
    }

    public void SetTerrainGrid(int x, int y, int[,] grid) {
        for (var xx = 0; xx < grid.GetLength(0); xx++) {
            for (var yy = 0; yy < grid.GetLength(1); yy++) {
                TerrainGrid[x + xx, y + yy] = grid[xx, yy];
            }
        }
    }

    public void SetTerrainGrid(int x, int y, int width, int height, int terrain) {
        for (var xx = 0; xx < width; xx++) {
            for (var yy = 0; yy < height; yy++) {
                TerrainGrid[x + xx, y + yy] = terrain;
            }
        }
    }

    public bool UpdateAtlasCoords(int layer, TileMapSource source, int x, int y) {
        var atlasCoords = source.TileSetLayout.GetAtlasCoordsByTileId(GetTileId(x, y));
        return SetAtlasCoords(layer, source.SourceId, x, y, atlasCoords);
    }

    public bool SetAtlasCoords(int layer, TileMapSource source, int x, int y, int tileId) {
        var atlasCoords = source.TileSetLayout.GetAtlasCoordsByTileId(tileId);
        return SetAtlasCoords(layer, source.SourceId, x, y, atlasCoords);
    }

    public bool SetAtlasCoords(int layer, TileMapSource source, int x, int y, Vector2I? atlasCoords) {
        return SetAtlasCoords(layer, source.SourceId, x, y, atlasCoords);
    }

    public bool SetAtlasCoords(int layer, int sourceId, int x, int y, Vector2I? atlasCoords) {
        ref var currentInfo = ref TileInfoGrid[layer][x, y];
        if (currentInfo.AtlasCoords == atlasCoords && currentInfo.SourceId == sourceId) return false;
        currentInfo.AtlasCoords = atlasCoords;
        currentInfo.SourceId = sourceId;
        return true;
    }

    public int GetTileId(int x, int y) {
        return TileId[x, y];
    }

    public bool SetTileId(int x, int y, int tileId) {
        if (TileId[x, y] == tileId) return false;
        TileId[x, y] = tileId;
        return true;
    }

    public void SetTileIdGrid(int layer, int x, int y, int width, int height, int tileId) {
        for (var xx = 0; xx < width; xx++) {
            for (var yy = 0; yy < height; yy++) {
                TileId[xx + x, yy +y] = tileId;
            }
        }
    }

    public void SetTileIdGrid(int layer, int x, int y, int[,] tileIdGrid) {
        var width = tileIdGrid.GetLength(0);
        var height = tileIdGrid.GetLength(1);
        for (var xx = 0; xx < width; xx++) {
            for (var yy = 0; yy < height; yy++) {
                TileId[xx + x, yy + y] = tileIdGrid[xx, yy];
            }
        }
    }
    
    public TileActionBuilder NewAction() {
        var pipeline = new TileActionBuilder(this);
        _pendingPipelines.Add(pipeline);
        return pipeline;
    }
    
    public void Apply() {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var span = CollectionsMarshal.AsSpan(_pendingPipelines);
                for (var idx = 0; idx < span.Length; idx++) {
                    var pipeline = span[idx];
                    pipeline._Apply(x, y);
                }
            }
        }
        _pendingPipelines.Clear();
    }

    public void Loop(Action<int, int> action) {
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {
                action(x, y);
            }
        }
    }

    public void LoopLayers(Action<int, int, int> action) {
        for (var layer = 0; layer < Layers; layer++) {
            for (var x = 0; x < Width; x++) {
                for (var y = 0; y < Height; y++) {
                    action(layer, x, y);
                }
            }
        }
    }

    public void Loop(ITileHandler handler) {
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {
                handler.Apply(this, x, y);
            }
        }
    }

    public void Loop(params ITileHandler[] handlers) {
        handlers.ForEach(Loop);
    }

    public void Loop(IEnumerable<ITileHandler> handlers) {
        handlers.ForEach(Loop);
    }

    public void DumpAtlasCoordsTo(global::Godot.TileMap godotTileMap) {
        LoopLayers((layer, x, y) => {
            ref var cellInfo = ref GetCellInfoRef(layer, x, y);
            if (!cellInfo.AtlasCoords.HasValue) return;
            godotTileMap.SetCell(layer, new Vector2I(x, y), cellInfo.SourceId, cellInfo.AtlasCoords.Value);
        });
    }
    
    public static TileMap Parse(string value, Dictionary<char, int> charToTerrain, int layers = 1) {
        var lines = Parse(value);
        var maxLength = lines[0].Length; // all lines have the same length
        var tileMap = new TileMap(layers, maxLength, lines.Count);
        var y = 0;
        foreach (var line in lines) {
            var x = 0;
            foreach (var terrain in line.Select(c => charToTerrain[c])) {
                tileMap.SetTerrain(x, y, terrain);
                x++;
            }
            y++;
        }
        return tileMap;
    }

    internal static List<string> Parse(string value) {
        var lines = value.Split('\n')
            .SkipWhile(string.IsNullOrWhiteSpace) // Remove empty lines at beginning
            .Reverse().SkipWhile(string.IsNullOrWhiteSpace).Reverse() // Remove empty lines at end
            .ToList();

        const char sep = ':';
        var maxLength = -1;
        if (lines.Any(s => s.Contains(sep))) {
            for (var i = 0; i < lines.Count; i++) {
                var line = lines[i].Trim();
                if (line.StartsWith(sep) && line.EndsWith(sep)) {
                    line = line.Substring(1, line.Length - 2);
                    lines[i] = line;
                    if (maxLength == -1) {
                        maxLength = line.Length;
                    } else if (maxLength != line.Length) {
                        throw new Exception("All lines must have the same size: " + lines[i]);
                    }
                } else {
                    throw new Exception("Line must contains 2 ':' separators: " + lines[i]);
                }
            }
        } else {
            maxLength = lines.Max(line => line.TrimEnd().Length);
            lines = lines.Select(line => line.PadRight(maxLength, ' ')).ToList();
        }
        return lines;
    }
}