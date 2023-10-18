using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Godot;

namespace Betauer.TileSet.TileMap;

public class TileMap<TTerrain> : TileMap where TTerrain : Enum {
    private readonly IReadOnlyDictionary<TTerrain, int>? _enumToTerrainMap;
    private readonly IReadOnlyDictionary<int, TTerrain>? _terrainToEnumMap;

    public TileMap(int layers, int width, int height, TTerrain defaultTerrain = default) : base(layers, width, height, defaultTerrain.ToInt()) {
    }

    public TileMap(int layers, int width, int height, IReadOnlyDictionary<TTerrain, int>? enumToTerrainMap, TTerrain defaultTerrain = default) : base(layers, width,
        height,
        enumToTerrainMap != null && enumToTerrainMap.TryGetValue(defaultTerrain, out var terrainId) ? terrainId : defaultTerrain.ToInt()) {
        _enumToTerrainMap = enumToTerrainMap;
        _terrainToEnumMap = enumToTerrainMap?.ToDictionary(kv => kv.Value, kv => kv.Key);
    }

    public int EnumToTerrain(TTerrain enumTerrain) {
        return _enumToTerrainMap == null ? enumTerrain.ToInt() : _enumToTerrainMap.TryGetValue(enumTerrain, out var terrainId) ? terrainId : enumTerrain.ToInt();
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

    public TTerrain TerrainToEnum(int terrain) {
        return _terrainToEnumMap == null ? terrain.ToEnum<TTerrain>() : _terrainToEnumMap.TryGetValue(terrain, out var terrainId) ? terrainId : terrain.ToEnum<TTerrain>();
    }

    public TTerrain GetTerrainEnum(int x, int y) {
        return TerrainToEnum(TerrainGrid[y, x]);
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
        public int TileId { get; set; }

        public int SourceId { get; set; }
        public Vector2I? AtlasCoords { get; set; }
    }

    public int[,] TerrainGrid { get; }
    public TileInfo[][,] TileInfoGrid { get; }
    public int Layers { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }

    public TileMap(int layers, int width, int height, int defaultTerrain = -1) {
        Width = width;
        Height = height;
        Layers = layers;
        TileInfoGrid = new TileInfo[layers][,];
        TerrainGrid = new int[height, width];

        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                TerrainGrid[y, x] = defaultTerrain;
            }
        }

        for (var layer = 0; layer < layers; layer++) {
            TileInfoGrid[layer] = new TileInfo[height, width];
            for (var y = 0; y < height; y++) {
                for (var x = 0; x < width; x++) {
                    ref var cell = ref TileInfoGrid[layer][y, x];
                    cell.TileId = -1;
                    cell.AtlasCoords = null;
                }
            }
        }
    }

    public ref TileInfo GetCellInfoRef(int layer, int x, int y) {
        return ref TileInfoGrid[layer][y, x];
    }

    public void Clear(int layer) {
        Clear(layer, 0, 0, Width, Height);
    }

    public void Clear(int layer, int x, int y, int width, int height) {
        for (var yy = 0; yy < height; yy++) {
            for (var xx = 0; xx < width; xx++) {
                RemoveCell(layer, x + xx, y + yy);
            }
        }
    }

    public void RemoveCell(int layer, int x, int y) {
        ref var currentInfo = ref TileInfoGrid[layer][y, x];
        currentInfo.TileId = -1;
        currentInfo.AtlasCoords = null;
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

    public bool SetAtlasCoords(int layer, int sourceId, int x, int y, Vector2I? atlasCoords) {
        ref var currentInfo = ref TileInfoGrid[layer][y, x];
        if (currentInfo.AtlasCoords == atlasCoords && currentInfo.SourceId == sourceId) return false;
        currentInfo.AtlasCoords = atlasCoords;
        currentInfo.SourceId = sourceId;
        return true;
    }

    public void SetAtlasCoordsGrid(int layer, int x, int y, Vector2I?[,] grid) {
        for (var yy = 0; yy < grid.GetLength(0); yy++) {
            for (var xx = 0; xx < grid.GetLength(1); xx++) {
                ref var currentInfo = ref TileInfoGrid[layer][y + yy, x + xx];
                currentInfo.AtlasCoords = grid[yy, xx];
            }
        }
    }

    public void SetAtlasCoordsGrid(int layer, int x, int y, int width, int height, Vector2I? atlasCoords) {
        for (var yy = 0; yy < height; yy++) {
            for (var xx = 0; xx < width; xx++) {
                ref var currentInfo = ref TileInfoGrid[layer][y + yy, x + xx];
                currentInfo.AtlasCoords = atlasCoords;
            }
        }
    }

    public bool SetTileId(int layer, int x, int y, int tileId) {
        ref var currentInfo = ref TileInfoGrid[layer][y, x];
        if (currentInfo.TileId == tileId) return false;
        currentInfo.TileId = tileId;
        return true;
    }

    public void SetTileIdGrid(int layer, int x, int y, int width, int height, int tileId) {
        for (var yy = 0; yy < height; yy++) {
            for (var xx = 0; xx < width; xx++) {
                ref var currentInfo = ref TileInfoGrid[layer][y + yy, x + xx];
                currentInfo.TileId = tileId;
            }
        }
    }

    public void SetTileIdGrid(int layer, int x, int y, int[,] tileIdGrid) {
        for (var yy = 0; yy < tileIdGrid.GetLength(0); yy++) {
            for (var xx = 0; xx < tileIdGrid.GetLength(1); xx++) {
                ref var currentInfo = ref TileInfoGrid[layer][y + yy, x + xx];
                currentInfo.TileId = tileIdGrid[yy, xx];
            }
        }
    }

    public int[,] ExportTileIdGrid(int layer) => TileInfoGrid[layer].GetGrid(tileInfo => tileInfo.TileId);

    public Vector2I?[,] ExportAtlasCoordsGrid(int layer) => TileInfoGrid[layer].GetGrid(tileInfo => tileInfo.AtlasCoords);

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