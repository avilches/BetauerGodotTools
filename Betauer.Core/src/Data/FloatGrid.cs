using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.Data;

/// <summary>
/// A bidimensional array of T values that can be accessed by float normalized coordinates (0.0..1.0), no matter of the size
/// </summary>
/// <typeparam name="T"></typeparam>
public class FloatGrid<T> {
    public T[,] Grid { get; set; }
    public readonly Dictionary<T, Rect2> Rects = new();

    public FloatGrid(int sizeX, int sizeY) {
        Grid = new T[sizeY, sizeX];
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
        var posX = GetPosX(x);
        var posY = GetPosY(y);
        return Grid[posY, posX];
    }

    public void Set(float x, float y, T value) {
        var posX = GetPosX(x);
        var posY = GetPosY(y);
        Grid[posY, posX] = value;
    }

    private int GetPosY(float y) {
        var maxValue = Grid.GetLength(0) - 1;
        var pos = Mathf.RoundToInt(Mathf.Lerp(0, maxValue, y));
        return Math.Clamp(pos, 0, maxValue);    
    }

    private int GetPosX(float x) {
        var maxValue = Grid.GetLength(1) - 1;
        var pos = Mathf.RoundToInt(Mathf.Lerp(0, maxValue, x));
        return Math.Clamp(pos, 0, maxValue);    
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

    public Rect2 GetRect(T value) => Rects[value];

    /// <summary>
    /// First validate all elements has a unique rectangle without overlapping others.
    /// So, this is valid:
    ///
    ///    AAxxCxw
    ///    AAxxDDD
    ///    AA..FFF
    ///
    /// But these are not valid:
    ///
    ///    AAxA
    ///    AAxD Because A appears twice
    ///
    ///    AAxx
    ///    AAxD Because x is not a rectangle (it has a D inside)
    ///
    /// If validation is ok, then create the Rects dictionary with all the rectangles 
    /// 
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void CreateRectangles() {
        var width = Grid.GetLength(1);
        var height = Grid.GetLength(0);
        var rectPositions = new Dictionary<T, (Vector2I, Vector2I)>();
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                T value = Grid[y, x];
                if (!rectPositions.ContainsKey(value)) {
                    rectPositions[value] = (new Vector2I(x, y), new Vector2I(x, y));
                } else {
                    var (start, end) = rectPositions[value];
                    if (x < start.X) start = new Vector2I(x, start.Y);
                    if (y < start.Y) start = new Vector2I(start.X, y);

                    if (x > end.X) end = new Vector2I(x, end.Y);
                    if (y > end.Y) end = new Vector2I(end.X, y);

                    rectPositions[value] = (start, end);
                }
            }
        }
        var rectWidth = 1f / width;
        var rectHeight = 1f / height;
        foreach (var (value, (start, end)) in rectPositions) {
            for (var y = start.Y; y <= end.Y; y++) {
                for (var x = start.X; x <= end.X; x++) {
                    T gridValue = Grid[y, x];
                    if (gridValue.Equals(value)) continue;
                    throw new Exception($"Wrong value {gridValue} in position ({x},{y}). Expected value: {value}");
                }
            }
            var rectStart = new Vector2(start.X * rectWidth, start.Y * rectHeight);
            var rectEnd = new Vector2(end.X * rectWidth + rectWidth, end.Y * rectHeight + rectHeight);
            Rects[value] = new Rect2(rectStart, rectEnd - rectStart);
        }
    }
}