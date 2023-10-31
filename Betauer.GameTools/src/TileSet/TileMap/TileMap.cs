using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.TileSet.Image;
using Betauer.TileSet.TileMap.Handlers;
using Godot;

namespace Betauer.TileSet.TileMap;

public class TileMap<TTerrain> : TileMap where TTerrain : Enum {
    private readonly IReadOnlyDictionary<TTerrain, int>? _enumToTerrainMap;
    private readonly IReadOnlyDictionary<int, TTerrain>? _terrainToEnumMap;

    public TileMap(int layers, int width, int height, TTerrain defaultTerrain = default) : base(layers, width, height, defaultTerrain.ToInt()) {
    }

    public TileMap(int layers, int width, int height, IReadOnlyDictionary<TTerrain, int>? enumToTerrainMap, TTerrain defaultTerrain = default) : base(layers, width,
        height,
        enumToTerrainMap != null && enumToTerrainMap.TryGetValue(defaultTerrain, out var terrain) ? terrain : defaultTerrain.ToInt()) {
        _enumToTerrainMap = enumToTerrainMap;
        _terrainToEnumMap = enumToTerrainMap?.ToDictionary(kv => kv.Value, kv => kv.Key);
    }

    public int EnumToTerrain(TTerrain terrainEnum) {
        return _enumToTerrainMap == null ? terrainEnum.ToInt() : _enumToTerrainMap.TryGetValue(terrainEnum, out var terrain) ? terrain : terrainEnum.ToInt();
    }

    public TTerrain TerrainToEnum(int terrain) {
        return _terrainToEnumMap == null ? terrain.ToEnum<TTerrain>() : _terrainToEnumMap.TryGetValue(terrain, out var terrainEnum) ? terrainEnum : terrain.ToEnum<TTerrain>();
    }

    public TTerrain GetTerrainEnum(int x, int y) {
        return TerrainToEnum(TerrainGrid[y, x]);
    }

    public void SetTerrain(int x, int y, TTerrain terrain) {
        TerrainGrid[y, x] = EnumToTerrain(terrain);
    }

    public void SetTerrainGrid(int x, int y, TTerrain[,] grid) {
        for (var yy = 0; yy < grid.GetLength(0); yy++) {
            for (var xx = 0; xx < grid.GetLength(1); xx++) {
                TerrainGrid[y + yy, x + xx] = EnumToTerrain(grid[yy, xx]);
            }
        }
    }

    public void SetTerrainGrid(int x, int y, int width, int height, TTerrain terrain) {
        SetTerrainGrid(x, y, width, height, EnumToTerrain(terrain));
    }

    public static TileMap<T> Parse<T>(string value, Dictionary<char, T> charToEnum, int layers = 1, IReadOnlyDictionary<T, int> enumToTerrainMap = null)
        where T : Enum {
        var lines = TileMap.Parse(value);
        var maxLength = lines[0].Length; // all lines have the same length
        var tileMap = new TileMap<T>(layers, maxLength, lines.Count, enumToTerrainMap);
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

    private readonly List<TileActionList> _pendingPipelines = new();

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
        TerrainGrid = new int[height, width];
        TileId = new int[height, width];
        _defaultTerrain = defaultTerrain;
        
        for (var yy = 0; yy < height; yy++) {
            for (var xx = 0; xx < width; xx++) {
                TerrainGrid[yy, xx] = _defaultTerrain;
                TileId[yy, xx] = -1;
            }
        }

        for (var layer = 0; layer < Layers; layer++) {
            TileInfoGrid[layer] = new TileInfo[height, width];
        }
    }

    public TileMapSource CreateSource(int sourceId, ITileSetLayout tileSetLayout) {
        return new TileMapSource(this, sourceId, tileSetLayout);
    }
    
    public ref TileInfo GetCellInfoRef(int layer, int x, int y) {
        return ref TileInfoGrid[layer][y, x];
    }

    public void Clear() {
        Clear(0, 0, Width, Height);
    }

    public void Clear(int x, int y, int width, int height) {
        for (var yy = 0; yy < height; yy++) {
            for (var xx = 0; xx < width; xx++) {
                TerrainGrid[yy, xx] = _defaultTerrain;
                TileId[yy, xx] = -1;
            }
        }
        for (var layer = 0; layer < Layers; layer++) {
            for (var yy = 0; yy < height; yy++) {
                for (var xx = 0; xx < width; xx++) {
                    ref var currentInfo = ref TileInfoGrid[layer][y + yy, x + xx];
                    currentInfo.Clear();
                }
            }
        }
    }

    public void ClearCell(int x, int y) {
        TerrainGrid[y, x] = _defaultTerrain;
        TileId[y, x] = -1;
        for (var layer = 0; layer < Layers; layer++) {
            ref var currentInfo = ref TileInfoGrid[layer][y, x];
            currentInfo.Clear();
        }
    }

    public int GetTerrain(int x, int y) {
        return TerrainGrid[y, x];
    }

    public bool SetTerrain(int x, int y, int terrain) {
        if (TerrainGrid[y, x] == terrain) return false;
        TerrainGrid[y, x] = terrain;
        return true;
    }

    public void SetTerrainGrid(int x, int y, int[,] grid) {
        for (var yy = 0; yy < grid.GetLength(0); yy++) {
            for (var xx = 0; xx < grid.GetLength(1); xx++) {
                TerrainGrid[y + yy, x + xx] = grid[yy, xx];
            }
        }
    }

    public void SetTerrainGrid(int x, int y, int width, int height, int terrain) {
        for (var yy = 0; yy < height; yy++) {
            for (var xx = 0; xx < width; xx++) {
                TerrainGrid[y + yy, x + xx] = terrain;
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
        ref var currentInfo = ref TileInfoGrid[layer][y, x];
        if (currentInfo.AtlasCoords == atlasCoords && currentInfo.SourceId == sourceId) return false;
        currentInfo.AtlasCoords = atlasCoords;
        currentInfo.SourceId = sourceId;
        return true;
    }

    public int GetTileId(int x, int y) {
        return TileId[y, x];
    }

    public bool SetTileId(int x, int y, int tileId) {
        if (TileId[y, x] == tileId) return false;
        TileId[y, x] = tileId;
        return true;
    }

    public void SetTileIdGrid(int layer, int x, int y, int width, int height, int tileId) {
        for (var yy = 0; yy < height; yy++) {
            for (var xx = 0; xx < width; xx++) {
                TileId[y, x] = tileId;
            }
        }
    }

    public void SetTileIdGrid(int layer, int x, int y, int[,] tileIdGrid) {
        for (var yy = 0; yy < tileIdGrid.GetLength(0); yy++) {
            for (var xx = 0; xx < tileIdGrid.GetLength(1); xx++) {
                TileId[y, x] = tileIdGrid[yy, xx];
            }
        }
    }
    
    public TileActionList CreateTileActionList() {
        var pipeline = new TileActionList(this);
        _pendingPipelines.Add(pipeline);
        return pipeline;
    }
    
    public void Flush() {
        _pendingPipelines.ForEach(pipeline => pipeline.Apply());
        _pendingPipelines.Clear();
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