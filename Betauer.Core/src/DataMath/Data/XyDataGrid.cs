using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.DataMath.Data;

public readonly struct XyDataGrid<T> {
    public int Width => Data.GetLength(0);
    public int Height => Data.GetLength(1);
    public T[,] Data { get; }

    public XyDataGrid(int width, int height) {
        Data = new T[width, height];
    }

    public XyDataGrid(T[,] data) {
        Data = data;
    }

    public YxDataGrid<T> Transpose() {
        return new YxDataGrid<T>(Data.YxFlipDiagonal());
    } 

    public XyDataGrid<T> Clone() {
        return new XyDataGrid<T>(Width, Width).Load(GetValue);
    }
    
    public XyDataGrid<T> Fill(T value) {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                SetValue(x, y, value);
            }
        }
        return this;
    }

    public XyDataGrid<T> Fill(int x, int y, int width, int height, T value) {
        for (var yy = y; yy < height - y; yy++) {
            for (var xx = x; xx < width - x; xx++) {
                SetValue(xx, yy, value);
            }
        }
        return this;
    }

    public XyDataGrid<T> Load(Func<int, int, T> valueFunc) {
        Load(0, 0, Width, Height, valueFunc);
        return this;
    }

    public XyDataGrid<T> Load(int x, int y, int width, int height, Func<int, int, T> valueFunc) {
        for (var yy = y; yy < height - y; yy++) {
            for (var xx = x; xx < width - x; xx++) {
                var value = valueFunc.Invoke(xx, yy);
                SetValue(xx, yy, value);
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
        for (var yy = y; yy < height - y; yy++) {
            for (var xx = x; xx < width - x; xx++) {
                var value = GetValue(xx, yy);
                var transformed = action(value);
                SetValue(xx, yy, transformed);
            }
        }
        return this;
    }

    public XyDataGrid<T> Loop(int x, int y, int width, int height, Action<T, int, int> action) {
        for (var yy = y; yy < height - y; yy++) {
            for (var xx = x; xx < width - x; xx++) {
                var value = GetValue(xx, yy);
                action(value, xx, yy);
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

    public T this[Vector2I pos] {
        get => Data[pos.X, pos.Y];
        set => Data[pos.X, pos.Y] = value;
    }

    public T[,] GetRect(int startX, int startY, int width, int height, T defaultValue = default) {
        return Data.GetXyRect(startX, startY, width, height, defaultValue);
    }

    public TDest[,] GetRect<TDest>(int startX, int startY, int width, int height, Func<T, TDest> transformer, TDest defaultValue = default) {
        return Data.GetXyRect(startX, startY, width, height, transformer, defaultValue);
    }

    public TDest[,] GetRect<TDest>(Func<T, TDest> transformer, TDest defaultValue = default) {
        return Data.GetXyRect(transformer, defaultValue);
    }

    public T[,] GetRectCenter(int centerX, int centerY, int size, T defaultValue = default) {
        return Data.GetXyRectCenter(centerX, centerY, size, defaultValue);
    }

    public TDest[,] GetRectCenter<TDest>(int centerX, int centerY, int size, Func<T, TDest> transformer, TDest defaultValue = default) {
        return Data.GetXyRectCenter(centerX, centerY, size, transformer, defaultValue);
    }

    public void CopyRect(int startX, int startY, int width, int height, T[,] destination, T defaultValue = default) {
        Data.CopyXyRect(startX, startY, width, height, destination, defaultValue);
    }

    public void CopyRect<TDest>(TDest[,] destination, Func<T, TDest> transformer, TDest defaultValue = default) {
        Data.CopyXyRect(destination, transformer, defaultValue);
    }

    public void CopyRect<TDest>(int startX, int startY, int width, int height, TDest[,] destination, Func<T, TDest> transformer, TDest defaultValue = default) {
        Data.CopyXyRect(startX, startY, width, height, destination, transformer, defaultValue);
    }
    
    public void CopyCenterRect(int centerX, int centerY, T defaultValue, T[,] destination) {
        Data.CopyXyCenterRect(centerX, centerY, defaultValue, destination);
    }

    public void CopyCenterRect<TOut>(int centerX, int centerY, TOut defaultValue, TOut[,] destination, Func<T, TOut> transform) {
        Data.CopyXyCenterRect(centerX, centerY, defaultValue, destination, transform);
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