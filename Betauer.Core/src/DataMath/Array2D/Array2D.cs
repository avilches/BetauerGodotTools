using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.DataMath.Array2D;

public readonly record struct DataCell<T>(Vector2I Position, T Value) {
    public readonly Vector2I Position = Position;
    public readonly T Value = Value;
}

/// <summary>
/// A bidimensional array (grid) where can be accessed in Column-major order. That means:
///
/// An Array2D to store an image of 1920x1080 is created with new Array2D<byte>(1920, 1080)
/// And accesses using (x, y) coordinates instead of new byte[1080, 1920] and accessed using (y, x) which
/// is how C# stores bidimensional arrays because it's Row-major order.  
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Array2D<T> : IEnumerable<DataCell<T>> {
    public int Width => Data.GetLength(1);
    public int Height => Data.GetLength(0);
    public T[,] Data { get; }

    public Array2D(int width, int height) {
        Data = new T[height, width];
    }

    public Array2D(T[,] data) {
        Data = data;
    }

    public Array2D<T> Clone() {
        return new Array2D<T>(Width, Height).Load(GetValue);
    }
    
    public Array2D<T> Fill(T value) {
        return Fill(0, 0, Width, Height, value);
    }

    public Array2D<T> Fill(Rect2I rect, T value) {
        return Fill(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y, value);
    }
    
    public Array2D<T> Fill(int x, int y, int width, int height, T value) {
        for (var yy = y; yy < height - y; yy++) {
            for (var xx = x; xx < width - x; xx++) {
                this[xx, yy] = value;
            }
        }
        return this;
    }

    public Array2D<T> Load(Func<int, int, T> valueFunc) {
        Load(0, 0, Width, Height, valueFunc);
        return this;
    }

    public Array2D<T> Load(Func<Vector2I, T> valueFunc) {
        Load(0, 0, Width, Height, valueFunc);
        return this;
    }

    public Array2D<T> Load(Rect2I rect, Func<int, int, T> valueFunc) {
        return Load(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y, valueFunc);
    }
    
    public Array2D<T> Load(Rect2I rect, Func<Vector2I, T> valueFunc) {
        return Load(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y, valueFunc);
    }
    
    public Array2D<T> Load(int x, int y, int width, int height, Func<Vector2I, T> valueFunc) {
        for (var yy = y; yy < height - y; yy++) {
            for (var xx = x; xx < width - x; xx++) {
                var value = valueFunc.Invoke(new Vector2I(xx, yy));
                this[xx, yy] = value;
            }
        }
        return this;
    }
        
    public Array2D<T> Load(int x, int y, int width, int height, Func<int, int, T> valueFunc) {
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

    public Array2D<T> Transform(Func<T, T> action) {
        Transform(0, 0, Width, Height, action);
        return this;
    }

    public Array2D<T> Transform(int x, int y, int width, int height, Func<T, T> action) {
        foreach (var cell in GetEnumerator(x, y, width, height)) {
            var transformed = action(cell.Value);
            this[cell.Position] = transformed;
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

    public static Array2D<TT> Parse<TT>(string template, Dictionary<char, TT> mapping) {
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
        var array2d = new Array2D<TT>(width, height);
        foreach (var line in lines) {
            var x = 0;
            foreach (var value in line.Select(c => mapping[c])) {
                array2d.Data[y, x] = value;
                x++;
            }
            y++;
        }
        return array2d;
    }
    
    public T[,] GetRect(int startX, int startY, int width, int height, T defaultValue = default) {
        var destination = new T[height, width];
        CopyRect(startX, startY, destination, value => value, defaultValue);
        return destination;
    }

    public TDest[,] GetRect<TDest>(int startX, int startY, int width, int height, Func<T, TDest> transformer, TDest defaultValue = default) {
        var destination = new TDest[height, width];
        CopyRect(startX, startY, destination, transformer, defaultValue);
        return destination;
    }

    public TDest[,] GetRect<TDest>(Rect2I rect, Func<T, TDest> transformer, TDest defaultValue = default) {
        var destination = new TDest[rect.Size.Y, rect.Size.X];
        CopyRect(rect.Position.X, rect.Position.Y, destination, transformer, defaultValue);
        return destination;
    }

    public T[,] GetCenter(int x, int y, int size, T defaultValue = default) {
        var destination = new T[size, size];
        var startX = x - size / 2;
        var startY = y - size / 2;
        CopyRect(startX, startY,  destination, value => value, defaultValue);
        return destination;
    }

    public TDest[,] GetCenter<TDest>(int x, int y, int size, Func<T, TDest> transformer, TDest defaultValue = default) {
        var destination = new TDest[size, size];
        var startX = x - size / 2;
        var startY = y - size / 2;
        CopyRect(startX, startY, destination, transformer, defaultValue);
        return destination;
    }

    public void CopyRect(int startX, int startY, T[,] destination, T defaultValue = default) {
        CopyRect(startX, startY, destination, value => value, defaultValue);
    }

    public void CopyRect<TDest>(TDest[,] destination, Func<T, TDest> transformer, TDest defaultValue = default) {
        CopyRect(0, 0, destination, transformer, defaultValue);
    }

    public void CopyRect<TDest>(int startX, int startY, TDest[,] destination, Func<T, TDest> transformer, TDest defaultValue = default) {
        var height = destination.GetLength(0);
        var width = destination.GetLength(1);
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                var sourceX = startX + x;
                var sourceY = startY + y;
                if (sourceX >= 0 && sourceX < Width &&
                    sourceY >= 0 && sourceY < Height) {
                    destination[y, x] = transformer(Data[sourceY, sourceX]);
                } else {
                    destination[y, x] = defaultValue;
                }
            }
        }
    }
    
    public void CopyCenterRect(int centerX, int centerY, T defaultValue, T[,] destination) {
        CopyCenterRect(centerX, centerY, defaultValue, destination, value => value);
    }

    public void CopyCenterRect<TOut>(int centerX, int centerY, TOut defaultValue, TOut[,] destination, Func<T, TOut> transform) {
        var width = destination.GetLength(1);
        var height = destination.GetLength(0);
        var startX = centerX - width / 2;
        var startY = centerY - height / 2;
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                var xx = startX + x;
                var yy = startY + y;
                destination[y, x] = xx < 0 || yy < 0 || xx >= Width || yy >= Height ? defaultValue : transform(Data[yy, xx]);
            }
        }
    }
}