using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core;

public class FloatGrid<T> {
    public T[,] Grid { get; set; }
    public float MinX { get; set; } = -1.0f;
    public float MaxX { get; set; } = 1.0f;
    public float MinY { get; set; } = -1.0f;
    public float MaxY { get; set; } = 1.0f;

    public FloatGrid(int sizeX, int sizeY) {
        Grid = new T[sizeY, sizeX];
    }

    public FloatGrid(int sizeX, int sizeY, float minX, float maxX, float minY, float maxY) {
        Grid = new T[sizeY, sizeX];
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
    }

    public void Set(int x, int y, T value) {
        Grid[y, x] = value;
    }

    public void Set(int x, int y, int width, int height, T value) {
        for (var yy = 0; yy < height; yy++) {
            for (var xx = 0; xx < width; xx++) {
                Grid[y + yy, x + xx] = value;
            }
        }
    }

    public T Get(float x, float y) {
        var maxXValue = Grid.GetLength(1) - 1;
        var maxYValue = Grid.GetLength(0) - 1;
        var posX = Mathf.RoundToInt(Mathf.Lerp(0, maxXValue, (x - MinX) / (MaxX - MinX)));
        var posY = Mathf.RoundToInt(Mathf.Lerp(0, maxYValue, (y - MinY) / (MaxY - MinY)));
        return Grid[Math.Clamp(posY, 0, maxYValue), Math.Clamp(posX, 0, maxYValue)];
    }

    public static FloatGrid<TT> Parse<TT>(string template, Dictionary<char, TT> mapping) {
        var lines = Parse(template);
        var width = lines[0].Length; // all lines have the same length
        var y = 0;
        var height = lines.Count;
        var floatGrid = new FloatGrid<TT>(width, height);
        foreach (var line in lines) {
            var x = 0;
            foreach (var value in line.Select(c => mapping[c])) {
                floatGrid.Set(x, y, value);
                x++;
            }
            y++;
        }
        return floatGrid;
    }

    internal static List<string> Parse(string template) {
        var lines = template.Split('\n')
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

    public void ValidateRectangles() {
        var rects = new Dictionary<T, (Vector2I, Vector2I)>();
        var width = Grid.GetLength(1);
        var height = Grid.GetLength(0);
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                T value = Grid[y, x];
                if (!rects.ContainsKey(value)) {
                    rects[value] = (new Vector2I(x, y), new Vector2I(x, y));
                } else {
                    var (start, end) = rects[value];
                    if (x < start.X) start = new Vector2I(x, start.Y);
                    if (y < start.Y) start = new Vector2I(start.X, y);

                    if (x > end.X) end = new Vector2I(x, end.Y);
                    if (y > end.Y) end = new Vector2I(end.X, y);

                    rects[value] = (start, end);
                }
            }
        }
        foreach (var (value, (start, end)) in rects) {
            for (var y = start.Y; y <= end.Y; y++) {
                for (var x = start.X; x <= end.X; x++) {
                    T gridValue = Grid[y, x];
                    if (gridValue.Equals(value)) continue;
                    throw new Exception($"Wrong value {gridValue} in position ({x},{y}). Expected value: {value}");
                }
            }
        }
    }
}