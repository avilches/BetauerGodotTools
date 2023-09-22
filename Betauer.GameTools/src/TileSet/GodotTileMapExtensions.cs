using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Godot;

namespace Betauer.TileSet;

public static class GodotTileMapExtensions {
    public static Dictionary<Godot.TileSet.CellNeighbor, int> MaskValues = new()  {
        { Godot.TileSet.CellNeighbor.TopSide, 1 },
        { Godot.TileSet.CellNeighbor.TopRightCorner, 2 },
        { Godot.TileSet.CellNeighbor.RightSide, 3 },
        { Godot.TileSet.CellNeighbor.BottomRightCorner, 4 },
        { Godot.TileSet.CellNeighbor.BottomSide, 5 },
        { Godot.TileSet.CellNeighbor.BottomLeftCorner, 6 },
        { Godot.TileSet.CellNeighbor.LeftSide, 7 },
        { Godot.TileSet.CellNeighbor.TopLeftCorner, 8 },
    };
    
    public static int GetTerrainMask(this TileData tileData) {
        var bits = 0;
        foreach (var maskValue in MaskValues) {
            if (tileData.GetTerrainPeeringBit(maskValue.Key) == 0) {
                bits = BitTools.EnableBit(bits, maskValue.Value);
            }
        }
        return bits;
    }
    
    public static T[,] GetGrid<T>(this Godot.TileMap godotTileMap, int layer, Func<Vector2I, T> func) {
        var usedCells = godotTileMap.GetUsedCellsById(layer).ToList();
        var maxX = usedCells.Select(v => v.X).Max();
        var minX = usedCells.Select(v => v.X).Min();
        var maxY = usedCells.Select(v => v.Y).Max();
        var minY = usedCells.Select(v => v.Y).Min();
        var rows = maxY - minY + 1;
        var columns = maxX - minX + 1;
        return GetGrid(godotTileMap, layer, minX, minY, columns, rows, func);
    }

    public static T[,] GetGrid<T>(this Godot.TileMap godotTileMap, int layer, int startX, int startY, int width, int height, Func<Vector2I, T> func) {
        var tiles = new T[height, width];
        var cleanY = 0;
        var maxX = startX + width;
        var maxY = startY + height;
        
        for (var y = startY; y < maxY; y++) {
            var cleanX = 0;
            for (var x = startX; x < maxX; x++) {
                tiles[cleanY, cleanX] = func(new Vector2I(x , y));
                cleanX++;
            }
            cleanY++;
        }
        return tiles;
    }

    public static Vector2I[,] GetAtlasCoordsGrid(this Godot.TileMap godotTileMap, int layer) {
        return GetGrid(godotTileMap, layer, (pos) => godotTileMap.GetCellAtlasCoords(0, pos));
    }

    public static Vector2I[,] GetAtlasCoordsGrid(this Godot.TileMap godotTileMap, int layer, int startX, int startY, int width, int height) {
        return GetGrid(godotTileMap, layer, startX, startY, width, height, (pos) => godotTileMap.GetCellAtlasCoords(0, pos));
    }

    public static int[,] GetTerrainMasksGrid(this Godot.TileMap godotTileMap, int layer, int startX, int startY, int width, int height) {
        var terrainMasks = GetGrid(godotTileMap, layer, startX, startY, width, height, (pos) => {
            var tileData = godotTileMap.GetCellTileData(0, pos);
            return tileData?.GetTerrainMask() ?? -1;
        });
        return terrainMasks;
    }

    public static int[,] GetTerrainMasksGrid(this Godot.TileMap godotTileMap, int layer) {
        var terrainMasks = GetGrid(godotTileMap, layer, (pos) => {
            var tileData = godotTileMap.GetCellTileData(0, pos);
            return tileData?.GetTerrainMask() ?? -1;
        });
        return terrainMasks;
    }

    public static void PrintTerrainMaskValues(this Godot.TileMap godotTileMap, int layer) {
        var tiles = godotTileMap.GetTerrainMasksGrid(layer);
        for (var y = 0; y < tiles.GetLength(0); y++) {
            Console.Write("|");
            for (var x = 0; x < tiles.GetLength(1); x++) {
                var tileId = tiles[y, x];
                Console.Write(tileId >= 0 ? tileId.ToString().PadLeft(3) + "|" : "   |");
            }
            Console.WriteLine();
        }
    }

    public static void PrintBlocks(this Godot.TileMap godotTileMap, int layer) {
        var visual = godotTileMap.GetGrid(layer, (pos) => godotTileMap.GetCellTileData(layer, pos) != null ? "#" : " ");
        for (var y = 0; y < visual.GetLength(0); y++) {
            for (var x = 0; x < visual.GetLength(1); x++) {
                Console.Write(visual[y, x]);
            }
            Console.WriteLine();
        }
    }
}
