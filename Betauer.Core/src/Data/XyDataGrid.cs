using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Data;

public class XyDataGrid<T> {
    public int Width { get; private set; }
    public int Height { get; private set; }

    public T[,] Data { get; private set; }

    public XyDataGrid(int width, int height) {
        Data = new T[width, height];
        Width = Data.GetLength(0);
        Height = Data.GetLength(1);
    }

    public XyDataGrid(T[,] data) {
        SetAll(data);
    }

    public XyDataGrid<T> Fill(T value) {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                Data[x, y] = value;
            }
        }
        return this;
    }

    public XyDataGrid<T> Fill(int x, int y, int width, int height, T value) {
        for (var xx = x; xx < width - x; xx++) {
            for (var yy = y; yy < height - y; yy++) {
                Data[xx, yy] = value;
            }
        }
        return this;
    }

    public XyDataGrid<T> SetAll(T[,] data) {
        Width = data.GetLength(0);
        Height = data.GetLength(1);
        Data = new T[Width, Height];
        for (var y = 0; y < data.GetLength(1); y++) {
            for (var x = 0; x < data.GetLength(0); x++) {
                var value = data[x, y];
                Data[x, y] = value;
            }
        }
        return this;
    }

    public XyDataGrid<T> Load(Func<int, int, T> valueFunc) {
        Load(0, 0, Width, Height, valueFunc);
        return this;
    }

    public XyDataGrid<T> Load(int x, int y, int width, int height, Func<int, int, T> valueFunc) {
        for (var xx = x; xx < width - x; xx++) {
            for (var yy = y; yy < height - y; yy++) {
                var value = valueFunc.Invoke(xx, yy);
                Data[xx, yy] = value;
            }
        }
        return this;
    }

    public XyDataGrid<T> Loop(Action<T, int, int> action) {
        Loop(0, 0, Width, Height, action);
        return this;
    }

    public XyDataGrid<T> Transform(Func<T, T> action) {
        Transform(0, 0, Width, Height, action);
        return this;
    }

    public XyDataGrid<T> Transform(int x, int y, int width, int height, Func<T, T> action) {
        for (var xx = x; xx < width - x; xx++) {
            for (var yy = y; yy < height - y; yy++) {
                var value = GetValue(xx, yy);
                SetValue(xx, yy, action(value));
            }
        }
        return this;
    }

    public XyDataGrid<T> Loop(int x, int y, int width, int height, Action<T, int, int> action) {
        for (var xx = x; xx < width - x; xx++) {
            for (var yy = y; yy < height - y; yy++) {
                action.Invoke(GetValue(xx, yy), xx, yy);
            }
        }
        return this;
    }

    public void SetValue(int x, int y, T value) {
        Data[x, y] = value;
    }

    public T GetValue(int x, int y) {
        return Data[x, y];
    }

    public T? GetValueSafe(int x, int y, T? defaultValue = default) {
        if (x < 0 || y < 0 || x >= Width || y >= Height) return defaultValue;
        return Data[x, y];
    }

    public T this[int x, int y] {
        get => Data[x, y];
        set => Data[x, y] = value;
    }
    
    public static XyDataGrid<TT> Parse<TT>(string template, Dictionary<char, TT> mapping) {
        var lines = template.Split('\n')
            .Select(v => v.Trim())
            .Where(v => v.Length > 0)
            .ToArray();
        if (lines.Length == 0) throw new Exception("Empty template");
        var width = lines[0].Length; // all lines have the same length
        foreach (var l in lines) {
            if (l.Length != width) throw new Exception($"This line doesn't have a length of {width}: {l}");
        }
        var y = 0;
        var height = lines.Length;
        var dataGrid = new XyDataGrid<TT>(width, height);
        foreach (var line in lines) {
            var x = 0;
            foreach (var value in line.Select(c => mapping[c])) {
                dataGrid.SetValue(x, y, value);
                x++;
            }
            y++;
        }
        return dataGrid;
    }
}