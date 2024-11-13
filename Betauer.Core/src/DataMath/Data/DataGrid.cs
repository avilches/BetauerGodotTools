using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.DataMath.Data;

public readonly record struct DataCell<T>(Vector2I Position, T Value) {
    public readonly Vector2I Position = Position;
    public readonly T Value = Value;
}

/// <summary>
/// A bidimensional array (grid) where can be accessed in Column-major order. That means:
///
/// A DataGrid to store an image of 1920x1080 is created with new DataGrid<byte>(1920, 1080)
/// And accesses using (x, y) coordinates instead of new byte[1080, 1920] and accessed using (y, x) which
/// is how C# stores bidimensional arrays because it's Row-major order.  
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct DataGrid<T> : IEnumerable<DataCell<T>> {
    public int Width => Data.GetLength(1);
    public int Height => Data.GetLength(0);
    public T[,] Data { get; }

    public DataGrid(int width, int height) {
        Data = new T[height, width];
    }

    public DataGrid(T[,] data) {
        Data = data;
    }

    public DataGrid<T> Clone() {
        return new DataGrid<T>(Width, Height).Load(GetValue);
    }

    public DataGrid<T> Fill(T value) {
        return Fill(0, 0, Width, Height, value);
    }

    public DataGrid<T> Fill(Rect2I rect, T value) {
        return Fill(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y, value);
    }
    
    public DataGrid<T> Fill(int x, int y, int width, int height, T value) {
        for (var yy = y; yy < height - y; yy++) {
            for (var xx = x; xx < width - x; xx++) {
                this[xx, yy] = value;
            }
        }
        return this;
    }

    public DataGrid<T> Load(Func<int, int, T> valueFunc) {
        Load(0, 0, Width, Height, valueFunc);
        return this;
    }

    public DataGrid<T> Load(Func<Vector2I, T> valueFunc) {
        Load(0, 0, Width, Height, valueFunc);
        return this;
    }

    public DataGrid<T> Load(Rect2I rect, Func<int, int, T> valueFunc) {
        return Load(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y, valueFunc);
    }
    
    public DataGrid<T> Load(Rect2I rect, Func<Vector2I, T> valueFunc) {
        return Load(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y, valueFunc);
    }
    
    public DataGrid<T> Load(int x, int y, int width, int height, Func<Vector2I, T> valueFunc) {
        for (var yy = y; yy < height - y; yy++) {
            for (var xx = x; xx < width - x; xx++) {
                var value = valueFunc.Invoke(new Vector2I(xx, yy));
                this[xx, yy] = value;
            }
        }
        return this;
    }
        
    public DataGrid<T> Load(int x, int y, int width, int height, Func<int, int, T> valueFunc) {
        for (var yy = y; yy < height - y; yy++) {
            for (var xx = x; xx < width - x; xx++) {
                var value = valueFunc.Invoke(xx, yy);
                this[xx, yy] = value;
            }
        }
        return this;
    }

    public IEnumerator<DataCell<T>> GetEnumerator() {
        for (var yy = 0; yy < Height; yy++) {
            for (var xx = 0; xx < Width; xx++) {
                var value = this[xx, yy];
                yield return new DataCell<T>(new Vector2I(xx, yy), value);
            }
        }
    }

    public IEnumerable<DataCell<T>> GetEnumerator(Rect2I rect) {
        return GetEnumerator(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y);
    }
    
    public IEnumerable<DataCell<T>> GetEnumerator(int x, int y, int width, int height) {
        for (var yy = y; yy < height - y; yy++) {
            for (var xx = x; xx < width - x; xx++) {
                var value = this[xx, yy];
                yield return new DataCell<T>(new Vector2I(xx, yy), value);
            }
        }
    }
    
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public DataGrid<T> Transform(Func<T, T> action) {
        Transform(0, 0, Width, Height, action);
        return this;
    }

    public DataGrid<T> Transform(int x, int y, int width, int height, Func<T, T> action) {
        foreach (var cell in GetEnumerator(x, y, width, height)) {
            var transformed = action(cell.Value);
            SetValue(cell.Position, transformed);
        }
        return this;
    }

    public void SetValue(Vector2I pos, T value) {
        Data[pos.Y, pos.X] = value;
    }
    
    public T GetValue(Vector2I pos) {
        return Data[pos.Y, pos.X];
    }

    public T? GetValueSafe(int x, int y, T? defaultValue = default) {
        if (x < 0 || y < 0 || x >= Width || y >= Height) return defaultValue;
        return Data[y, x];
    }

    public T this[int x, int y] {
        get => Data[y, x];
        set => Data[y, x] = value;
    }

    public T this[Vector2I pos] {
        get => Data[pos.Y, pos.X];
        set => Data[pos.Y, pos.X] = value;
    }

    public T[,] GetRect(int startX, int startY, int width, int height, T defaultValue = default) {
        return Data.GetRect(startX, startY, width, height, defaultValue);
    }

    public TDest[,] GetRect<TDest>(int startX, int startY, int width, int height, Func<T, TDest> transformer, TDest defaultValue = default) {
        return Data.GetRect(startX, startY, width, height, transformer, defaultValue);
    }

    public TDest[,] GetRect<TDest>(Func<T, TDest> transformer, TDest defaultValue = default) {
        return Data.GetRect(transformer, defaultValue);
    }

    public T[,] GetRectCenter(int centerX, int centerY, int size, T defaultValue = default) {
        return Data.GetRectCenter(centerX, centerY, size, defaultValue);
    }

    public TDest[,] GetRectCenter<TDest>(int centerX, int centerY, int size, Func<T, TDest> transformer, TDest defaultValue = default) {
        return Data.GetRectCenter(centerX, centerY, size, transformer, defaultValue);
    }

    public void CopyRect(int startX, int startY, int width, int height, T[,] destination, T defaultValue = default) {
        Data.CopyRect(startX, startY, width, height, destination, defaultValue);
    }

    public void CopyRect<TDest>(TDest[,] destination, Func<T, TDest> transformer, TDest defaultValue = default) {
        Data.CopyRect(destination, transformer, defaultValue);
    }

    public void CopyRect<TDest>(int startX, int startY, int width, int height, TDest[,] destination, Func<T, TDest> transformer, TDest defaultValue = default) {
        Data.CopyRect(startX, startY, width, height, destination, transformer, defaultValue);
    }
    
    public void CopyCenterRect(int centerX, int centerY, T defaultValue, T[,] destination) {
        Data.CopyCenterRect(centerX, centerY, defaultValue, destination);
    }

    public void CopyCenterRect<TOut>(int centerX, int centerY, TOut defaultValue, TOut[,] destination, Func<T, TOut> transform) {
        Data.CopyCenterRect(centerX, centerY, defaultValue, destination, transform);
    }

    public static DataGrid<TT> Parse<TT>(string template, Dictionary<char, TT> mapping) {
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
        var dataGrid = new DataGrid<TT>(width, height);
        foreach (var line in lines) {
            var x = 0;
            foreach (var value in line.Select(c => mapping[c])) {
                dataGrid[x, y] = value;
                x++;
            }
            y++;
        }
        return dataGrid;
    }

}