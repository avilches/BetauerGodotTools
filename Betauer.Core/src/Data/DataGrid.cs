using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Data;

public class DataGrid<T> {
    public int Width { get; private set; }
    public int Height { get; private set; }

    public T[,] Data { get; private set; }

    public DataGrid(int width, int height) {
        Data = new T[width, height];
        Width = Data.GetLength(0);
        Height = Data.GetLength(1);
    }

    public DataGrid(T[,] data) {
        SetAll(data);
    }

    public DataGrid<T> Fill(T value) {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                Data[x, y] = value;
            }
        }
        return this;
    }

    public DataGrid<T> Fill(int x, int y, int width, int height, T value) {
        for (var xx = x; xx < width - x; xx++) {
            for (var yy = y; yy < height - y; yy++) {
                Data[xx, yy] = value;
            }
        }
        return this;
    }

    public DataGrid<T> SetAll(T[,] data) {
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

    public DataGrid<T> Load(Func<int, int, T> valueFunc) {
        Load(0, 0, Width, Height, valueFunc);
        return this;
    }

    public DataGrid<T> Load(int x, int y, int width, int height, Func<int, int, T> valueFunc) {
        for (var xx = x; xx < width - x; xx++) {
            for (var yy = y; yy < height - y; yy++) {
                var value = valueFunc.Invoke(xx, yy);
                Data[xx, yy] = value;
            }
        }
        return this;
    }

    public DataGrid<T> Loop(Action<T, int, int> action) {
        Loop(0, 0, Width, Height, action);
        return this;
    }

    public DataGrid<T> Transform(Func<T, T> action) {
        Transform(0, 0, Width, Height, action);
        return this;
    }

    public DataGrid<T> Transform(int x, int y, int width, int height, Func<T, T> action) {
        for (var xx = x; xx < width - x; xx++) {
            for (var yy = y; yy < height - y; yy++) {
                var value = GetValue(xx, yy);
                SetValue(xx, yy, action(value));
            }
        }
        return this;
    }

    public DataGrid<T> Loop(int x, int y, int width, int height, Action<T, int, int> action) {
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
    
    public T[,] CopyRectTo(int startX, int startY, T[,] destination) {
        var width = destination.GetLength(0);
        var height = destination.GetLength(1);
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                destination[x, y] = Data[startX + x, startY + y];
            }
        }
        return destination;
    }
    
    public TOut[,] CopyRectTo<TOut>(int startX, int startY, TOut[,] destination, Func<T, TOut> transform) {
        var width = destination.GetLength(0);
        var height = destination.GetLength(1);
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                destination[x, y] = transform(Data[startX + x, startY + y]);
            }
        }
        return destination;
    }
    
    public T[,] CopyCenterRectTo(int centerX, int centerY, T defaultValue, T[,] destination) {
        var width = destination.GetLength(0);
        var height = destination.GetLength(1);
        var startX = centerX - width / 2;
        var startY = centerY - height / 2;
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                var xx = startX + x;
                var yy = startY + y;
                destination[x, y] = xx < 0 || yy < 0 || xx >= Width || yy >= Height ? defaultValue : Data[xx, yy];
            }
        }
        return destination;
    }

    public TOut[,] CopyCenterRectTo<TOut>(int centerX, int centerY, TOut defaultValue, TOut[,] destination, Func<T, TOut> transform) {
        var width = destination.GetLength(0);
        var height = destination.GetLength(1);
        var startX = centerX - width / 2;
        var startY = centerY - height / 2;
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                var xx = startX + x;
                var yy = startY + y;
                destination[x, y] = xx < 0 || yy < 0 || xx >= Width || yy >= Height ? defaultValue :  transform(Data[xx, yy]);
            }
        }
        return destination;
    }
    
    public static DataGrid<TT> Parse<TT>(string template, Dictionary<char, TT> mapping) {
        var lines = template.Split('\n')
            .SkipWhile(string.IsNullOrWhiteSpace) // Remove empty lines at beginning
            .Reverse().SkipWhile(string.IsNullOrWhiteSpace).Reverse() // Remove empty lines at end
            .Select(l => l.Trim())
            .ToList();
        if (lines.Count == 0) throw new Exception("Empty template");
        var width = lines[0].Length; // all lines have the same length
        foreach (var l in lines) {
            if (l.Length != width) throw new Exception($"This line doesn't have a length of {width}: {l}");
        }
        var y = 0;
        var height = lines.Count;
        var dataGrid = new DataGrid<TT>(width, height);
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