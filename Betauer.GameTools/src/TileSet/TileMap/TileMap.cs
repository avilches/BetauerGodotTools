using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Godot;

namespace Betauer.TileSet.TileMap;

public class TileMap<TType> where TType : Enum {
    public struct TileInfo {
        public int TileId { get; set; }

        public int SourceId { get; set; }
        public Vector2I? AtlasCoords { get; set; }
    }

    public TType[,] TypeGrid { get; }
    public TileInfo[][,] TileInfoGrid { get; }
    public int Layers { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    private Func<TType, int> _typeToTerrain = (type) => type.ToInt();
    public Dictionary<TType, int> TypeToTerrainMap { get; private set; } = new ();

    public TileMap(int width, int height) : this(1, width, height) {
    }

    public TileMap(int layers, int width, int height, TType defaultType = default) {
        Width = width;
        Height = height;
        Layers = layers;
        TileInfoGrid = new TileInfo[layers][,];
        TypeGrid = new TType[height, width];

        // Only fill the TypeGrid if the default value is different from the default value of the type
        TType originalDefaultType = default;
        if (defaultType.ToInt() != originalDefaultType.ToInt()) {
            for (var y = 0; y < height; y++) {
                for (var x = 0; x < width; x++) {
                    TypeGrid[y, x] = defaultType;
                }
            }
        }
        
        for (var layer = 0; layer < layers; layer++) {
            TileInfoGrid[layer] = new TileInfo[height, width];
            for (var y = 0; y < height; y++) {
                for (var x = 0; x < width; x++) {
                    ref var cell = ref TileInfoGrid[layer][y, x];
                    // cell.TerrainId = -1;
                    cell.TileId = -1;
                    cell.AtlasCoords = null;
                }
            }
        }
    }

    public void SetTypeToTerrain(Func<TType, int> typeToTerrain) {
        _typeToTerrain = typeToTerrain;
    }

    public void SetTypeToTerrain(Dictionary<TType, int> typeToTerrainMap) {
        TypeToTerrainMap = typeToTerrainMap;
    }

    public void SetTypeToTerrain(TType type, int terrainId) {
        TypeToTerrainMap[type] = terrainId;
    }

    public int TypeToTerrain(TType type) {
        return TypeToTerrainMap.TryGetValue(type, out var terrainId) ? terrainId : _typeToTerrain.Invoke(type);
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
        // currentInfo.TerrainId = -1;
        currentInfo.TileId = -1;
        currentInfo.AtlasCoords = null;
    }

    public TType GetType(int x, int y) {
        return TypeGrid[y, x];
    }

    public int GetTypeAsTerrain(int x, int y) {
        return TypeToTerrain(GetType(x, y));
    }

    public bool SetType(int x, int y, TType type) {
        if (TypeGrid[y, x].ToInt() == type.ToInt()) return false;
        TypeGrid[y, x] = type;
        return true;
    }

    public void SetTypeGrid(int x, int y, TType[,] grid) {
        for (var yy = 0; yy < grid.GetLength(0); yy++) {
            for (var xx = 0; xx < grid.GetLength(1); xx++) {
                TypeGrid[y + yy, x + xx] = grid[yy, xx];
            }
        }
    }

    public void SetTypeGrid(int x, int y, int width, int height, TType type) {
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
    
    public int[,] ExportTerrainIdGrid() => TypeGrid.GetGrid(TypeToTerrain);
    
    public Vector2I?[,] ExportAtlasCoordsGrid(int layer) => TileInfoGrid[layer].GetGrid(tileInfo => tileInfo.AtlasCoords);

    public static TileMap<T> Parse<T>(string value, Dictionary<char, T> charToType, int layers = 1) where T : Enum {
        var lines = Parse(value);
        var maxLength = lines[0].Length; // all lines have the same length
        var tileMap = new TileMap<T>(layers, maxLength, lines.Count);
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

    private static List<string> Parse(string value) {
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