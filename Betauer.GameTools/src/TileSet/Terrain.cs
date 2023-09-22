using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Godot;

namespace Betauer.TileSet;

public class Terrain {
    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public int[,] Grid { get; }

    public Terrain(int height, int width) {
        Width = width;
        Height = height;
        Grid = new int[Height, Width];
        Clear();
    }

    public void Clear() {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                Grid[y, x] = (int)TileType.None;
            }
        }
    }
    
    public enum TileType {
        None = -1,
        Auto = -2
    }

    public void SetCell(int x, int y, int tileId) => Grid[y, x] = tileId;
    
    public void RemoveCell(int x, int y) => Grid[y, x] = (int)TileType.None;
    
    public int GetCell(int x, int y) => Grid[y, x];
    
    public void SetCells(int x, int y, int[,] grid) {
        for (var yy = 0; yy < grid.GetLength(0); yy++) {
            for (var xx = 0; xx < grid.GetLength(1); xx++) {
                Grid[y + yy, x + xx] = grid[yy, xx];
            }
        }
    }

    public int[,] GetCells(int x, int y, int width, int height) {
        var grid = new int[height, width];
        for (var yy = 0; yy < height; yy++) {
            for (var xx = 0; xx < width; xx++) {
                grid[yy, xx] = Grid[y + yy, x + xx];
            }
        }
        return grid;
    }

    public void RemoveCells(int x, int y, int width, int height) {
        for (var yy = 0; yy < height; yy++) {
            for (var xx = 0; xx < width; xx++) {
                Grid[y + yy, x + xx] = (int)TileType.None;
            }
        }
    }

    private static readonly Dictionary<char, int> Defaults = new() {
        { ' ', (int)TileType.None },
        { '#', (int)TileType.Auto },
    };

    public static Terrain Parse(string value, Dictionary<char, int>? charToTileId = null) {
        var lines = value.Split('\n')
            .SkipWhile(string.IsNullOrWhiteSpace) // Remove empty lines at beginning
            .Reverse().SkipWhile(string.IsNullOrWhiteSpace).Reverse() // Remove empty lines at end
            .ToList();

        var maxLength = -1;
        if (lines.Any(s => s.Contains('|'))) {
            for (var i = 0; i < lines.Count; i++) {
                var line = lines[i].Trim();
                if (line.Count(c => c == '|') != 2) {
                    throw new Exception("Line must contains only 2 bars: " + (line.Length == 0 ? "(empty line)" : lines[i]));
                }
                if (line.StartsWith("|") && line.EndsWith("|")) {
                    line = line.Substring(1, line.Length - 2);
                    lines[i] = line;
                    if (maxLength == -1) {
                        maxLength = line.Length;
                    } else if (maxLength != line.Length) {
                        throw new Exception("All lines must have the same size: " + lines[i]);
                    }
                } else {
                    throw new Exception("Line must contains only 2 bars: "+lines[i]);
                }
            }
        } else {
            maxLength = lines.Max(line => line.TrimEnd().Length);
            lines = lines.Select(line => line.PadRight(maxLength, ' ')).ToList();
        }
        var y = 0;
        var tileMap = new Terrain(lines.Count, maxLength);
        foreach (var line in lines) {
            var x = 0;
            foreach (var tileId in line.Select(c => charToTileId?.TryGetValue(c, out var t) ?? false ? t : Defaults[c])) {
                tileMap.SetCell(x, y, tileId);
                x++;
            }
            y++;
        }
        
        return tileMap;
    }

    public void Expand(IReadOnlyCollection<int> availableTileIds) {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var tileId = Grid[y, x];
                if (tileId == (int)TileType.Auto) {
                    var neighbours = GetNeighbours(x, y);
                    var mask = CreateMask(neighbours);
                    if (availableTileIds.Contains(mask)) {
                        Grid[y, x] = mask;
                    } else {
                        Grid[y, x] = FindClosestMask(availableTileIds, mask);
                    }
                }
            }
        }
    }

    private int FindClosestMask(IReadOnlyCollection<int> tileIds, int mask) {
        // tileIds.
        return mask;
    }

    public static int CreateMask(int[] neighbours) {
        var bits = 0;
        for (var i = 0; i < neighbours.Length; i++) {
            if (neighbours[i] != (int)TileType.None) {
                bits = BitTools.EnableBit(bits, i + 1);
            }
        }
        return bits;
    }

    /// <summary>
    /// Returns an array with the values of the neighbours of the cell in the following order: start in the top side and go clockwise. 
    /// +-----------+
    /// | 8 | 1 | 2 |
    /// |---+---+---|
    /// | 7 |   | 3 |
    /// |---+---+---|
    /// | 6 | 5 | 4 |
    /// +-----------+
    /// The array starts position 0, so bitmask = position + 1
    /// (The first element is the top side, position 0, bitmask 1)
    ///  
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int[] GetNeighbours(int x, int y) {
        var neighbours = new int[8];
        neighbours[0] = GetCellOrDefault(x,       y - 1);   // TopSide
        neighbours[1] = GetCellOrDefault(x + 1, y - 1);   // TopRightCorner
        neighbours[2] = GetCellOrDefault(x + 1, y);         // RightSide
        neighbours[3] = GetCellOrDefault(x + 1, y + 1);   // BottomRightCorner
        neighbours[4] = GetCellOrDefault(x,       y + 1);   // BottomSide
        neighbours[5] = GetCellOrDefault(x - 1, y + 1);   // BottomLeftCorner
        neighbours[6] = GetCellOrDefault(x - 1, y);         // LeftSide
        neighbours[7] = GetCellOrDefault(x - 1, y - 1);   // TopLeftCorner
        return neighbours;
        int GetCellOrDefault(int x, int y) {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return (int)TileType.None;
            return GetCell(x, y);
        }
    }
    
    public static int[,] CreateNeighboursGrid(int mask) {
        var neighbours = new int[3, 3];
        const int x = 1;
        const int y = 1;
        neighbours[y - 1, x    ] = BitTools.HasBit(mask, 1) ? 0 : -1; // TopSide
        neighbours[y - 1, x + 1] = BitTools.HasBit(mask, 2) ? 0 : -1; // TopRightCorner
        neighbours[y    , x + 1] = BitTools.HasBit(mask, 3) ? 0 : -1; // RightSide
        neighbours[y + 1, x + 1] = BitTools.HasBit(mask, 4) ? 0 : -1; // BottomRightCorner
        neighbours[y + 1, x    ] = BitTools.HasBit(mask, 5) ? 0 : -1; // BottomSide
        neighbours[y + 1, x - 1] = BitTools.HasBit(mask, 6) ? 0 : -1; // BottomLeftCorner
        neighbours[y    , x - 1] = BitTools.HasBit(mask, 7) ? 0 : -1; // LeftSide
        neighbours[y - 1, x - 1] = BitTools.HasBit(mask, 8) ? 0 : -1; // TopLeftCorner
        neighbours[y    , x    ] = 0; // Center
        return neighbours;
    }
    
    
}

public static class TerrainExtensions {
    public static void PrintTileIdsArray(this Terrain terrain) {
        var tiles = terrain.Grid;
        Console.WriteLine("new[,] {");
        for (var y = 0; y < tiles.GetLength(0); y++) {
            Console.Write("  {");
            for (var x = 0; x < tiles.GetLength(1); x++) {
                var tileId = tiles[y, x];
                Console.Write(tileId.ToString().PadLeft(3) + ",");
            }
            Console.WriteLine("},");
        }
        Console.WriteLine("};");
    }

    public static void PrintBlocks(this Terrain terrain) {
        var tiles = terrain.Grid;
        for (var y = 0; y < tiles.GetLength(0); y++) {
            for (var x = 0; x < tiles.GetLength(1); x++) {
                var tileId = tiles[y, x];
                Console.Write(tileId >= 0 ? "#" : " ");
            }
            Console.WriteLine();
        }
    }

}