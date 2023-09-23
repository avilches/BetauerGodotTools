using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;

namespace Betauer.TileSet.Terrain;

public class SingleTerrain {
    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public int[,] Grid { get; }

    public SingleTerrain(int height, int width) {
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
        { '*', (int)TileType.Auto },
        { '0', 0 },
        
        { '<', 4 },
        { '-', 68 },
        { '>', 64 },
        
        { '^', 16 },
        { '|', 17 },
        { 'v', 1 },
        
        { '#', 255 },
    };

    public static SingleTerrain Parse(string value, Dictionary<char, int>? charToTileId = null) {
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
                    throw new Exception("Line must contains 2 ':' separators: "+lines[i]);
                }
            }
        } else {
            maxLength = lines.Max(line => line.TrimEnd().Length);
            lines = lines.Select(line => line.PadRight(maxLength, ' ')).ToList();
        }
        var y = 0;
        var tileMap = new SingleTerrain(lines.Count, maxLength);
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

    public void Transform(Func<int, int> transform) {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var tileId = Grid[y, x];
                if (tileId == (int)TileType.Auto) {
                    var mask = GetNeighbourOccupiedMasks(x, y);
                    Grid[y, x] = transform(mask);
                }
            }
        }
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
    public int[] GetNeighbourTiles(int x, int y) {
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
        
        int GetCellOrDefault(int x, int y) => 
            x >= 0 && x < Width && y >= 0 && y < Height ? GetCell(x, y) : (int)TileType.None;
    }

    /// If the x, y position (the center, where th 0 symbol) neighbours are these:
    /// |   |
    /// | 0*|
    /// | * |
    /// it will return { { false, false, false }, { false, true, true }, { false, true, false } }
    public bool[] GetOccupiedNeighbours(int x, int y) {
        var neighbours = new bool[8];
        neighbours[0] = GetCellOrDefault(x,       y - 1);   // TopSide
        neighbours[1] = GetCellOrDefault(x + 1, y - 1);   // TopRightCorner
        neighbours[2] = GetCellOrDefault(x + 1, y);         // RightSide
        neighbours[3] = GetCellOrDefault(x + 1, y + 1);   // BottomRightCorner
        neighbours[4] = GetCellOrDefault(x,       y + 1);   // BottomSide
        neighbours[5] = GetCellOrDefault(x - 1, y + 1);   // BottomLeftCorner
        neighbours[6] = GetCellOrDefault(x - 1, y);         // LeftSide
        neighbours[7] = GetCellOrDefault(x - 1, y - 1);   // TopLeftCorner
        return neighbours;
        
        bool GetCellOrDefault(int x, int y) => 
            x >= 0 && x < Width && y >= 0 && y < Height && GetCell(x, y) != (int)TileType.None;
    }
    
    /// If the x, y position (the center, where th 0 symbol) neighbours are these:
    /// |   |
    /// | 0*|
    /// | * |
    /// it will return 20
    public int GetNeighbourOccupiedMasks(int x, int y) {
        var bits = 0;
        bits = SetBitIfOccupied(bits, 1, x,       y - 1);   // TopSide
        bits = SetBitIfOccupied(bits, 2, x + 1, y - 1);   // TopRightCorner
        bits = SetBitIfOccupied(bits, 3, x + 1, y);         // RightSide
        bits = SetBitIfOccupied(bits, 4, x + 1, y + 1);   // BottomRightCorner
        bits = SetBitIfOccupied(bits, 5, x,       y + 1);   // BottomSide
        bits = SetBitIfOccupied(bits, 6, x - 1, y + 1);   // BottomLeftCorner
        bits = SetBitIfOccupied(bits, 7, x - 1, y);         // LeftSide
        bits = SetBitIfOccupied(bits, 8, x - 1, y - 1);   // TopLeftCorner
        return bits;
        
        int SetBitIfOccupied(int bits, int bitPosition, int x, int y) {
            if (x >= 0 && x < Width && y >= 0 && y < Height && GetCell(x, y) != (int)TileType.None) {
                bits = BitTools.EnableBit(bits, bitPosition);
            }
            return bits;
        }
    }
}