using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Godot;

namespace Betauer.TileSet.TileMap;

public class TileMap<TType> : TileMap where TType : Enum {
    private readonly IReadOnlyDictionary<TType, int>? _typeToTerrainMap;
    private readonly IReadOnlyDictionary<int, TType>? _terrainToTypeMap;

    public TileMap(int layers, int width, int height, TType defaultType = default) : base(layers, width, height, defaultType.ToInt()) {
    }

    public TileMap(int layers, int width, int height, IReadOnlyDictionary<TType, int>? typeToTerrainMap, TType defaultType = default) : base(layers, width,
        height,
        typeToTerrainMap != null && typeToTerrainMap.TryGetValue(defaultType, out var terrainId) ? terrainId : defaultType.ToInt()) {
        _typeToTerrainMap = typeToTerrainMap;
        _terrainToTypeMap = typeToTerrainMap?.ToDictionary(kv => kv.Value, kv => kv.Key);
    }

    public int TypeToTerrain(TType type) {
        return _typeToTerrainMap == null ? type.ToInt() : _typeToTerrainMap.TryGetValue(type, out var terrainId) ? terrainId : type.ToInt();
    }

    public void SetType(int x, int y, TType type) {
        TypeGrid[y, x] = TypeToTerrain(type);
    }

    public void SetTypeGrid(int x, int y, TType[,] grid) {
        for (var yy = 0; yy < grid.GetLength(0); yy++) {
            for (var xx = 0; xx < grid.GetLength(1); xx++) {
                TypeGrid[y + yy, x + xx] = TypeToTerrain(grid[yy, xx]);
            }
        }
    }

    public TType TerrainToType(int type) {
        return _terrainToTypeMap == null ? type.ToEnum<TType>() : _terrainToTypeMap.TryGetValue(type, out var terrainId) ? terrainId : type.ToEnum<TType>();
    }

    public TType GetType(int x, int y) {
        return TerrainToType(TypeGrid[y, x]);
    }

    public void SetTypeGrid(int x, int y, int width, int height, TType type) {
        SetTypeGrid(x, y, width, height, TypeToTerrain(type));
    }

    public static TileMap<T> Parse<T>(string value, Dictionary<char, T> charToType, int layers = 1, IReadOnlyDictionary<T, int> typeToTerrainMap = null)
        where T : Enum {
        var lines = TileMap.Parse(value);
        var maxLength = lines[0].Length; // all lines have the same length
        var tileMap = new TileMap<T>(layers, maxLength, lines.Count, typeToTerrainMap);
        var y = 0;
        foreach (var line in lines) {
            var x = 0;
            foreach (var type in line.Select(c => charToType[c])) {
                tileMap.SetType(x, y, type);
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

    public int[,] TypeGrid { get; }
    public TileInfo[][,] TileInfoGrid { get; }
    public int Layers { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }

    public TileMap(int layers, int width, int height, int defaultType = -1) {
        Width = width;
        Height = height;
        Layers = layers;
        TileInfoGrid = new TileInfo[layers][,];
        TypeGrid = new int[height, width];

        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                TypeGrid[y, x] = defaultType;
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

    public int GetType(int x, int y) {
        return TypeGrid[y, x];
    }

    public bool SetType(int x, int y, int type) {
        if (TypeGrid[y, x] == type) return false;
        TypeGrid[y, x] = type;
        return true;
    }

    public void SetTypeGrid(int x, int y, int[,] grid) {
        for (var yy = 0; yy < grid.GetLength(0); yy++) {
            for (var xx = 0; xx < grid.GetLength(1); xx++) {
                TypeGrid[y + yy, x + xx] = grid[yy, xx];
            }
        }
    }

    public void SetTypeGrid(int x, int y, int width, int height, int type) {
        for (var yy = 0; yy < height; yy++) {
            for (var xx = 0; xx < width; xx++) {
                TypeGrid[y + yy, x + xx] = type;
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

    public static TileMap Parse(string value, Dictionary<char, int> charToType, int layers = 1) {
        var lines = Parse(value);
        var maxLength = lines[0].Length; // all lines have the same length
        var tileMap = new TileMap(layers, maxLength, lines.Count);
        var y = 0;
        foreach (var line in lines) {
            var x = 0;
            foreach (var type in line.Select(c => charToType[c])) {
                tileMap.SetType(x, y, type);
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