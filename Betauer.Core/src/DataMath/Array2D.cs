using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace Betauer.Core.DataMath;

public readonly record struct DataCell<T>(Vector2I Position, T Value) {
    public readonly Vector2I Position = Position;
    public readonly T Value = Value;
}

public abstract class Array2D {

    public abstract int Width { get; }
    public abstract int Height { get; }
    public abstract Rect2I Bounds { get; }
    
    public static Vector2I[] Directions = new[] { Vector2I.Up, Vector2I.Right, Vector2I.Down, Vector2I.Left };

    public static Array2D<T> Parse<T>(string template, Dictionary<char, T> transform) {
        return Parse(template, c => transform[c]);;
    }

    public static Array2D<T> Parse<T>(string template, Dictionary<char, T> transform, T defaultValue) {
        return Parse(template, c => transform.GetValueOrDefault(c, defaultValue));
    }

    public static Array2D<T> Parse<T>(string template, Func<char, T> transform) {
        var lines = template.Split('\n')
            .Select(v => v.Trim())
            .Where(v => v.Length > 0)
            .ToArray();
        if (lines.Length == 0) throw new ArgumentException("Empty template");
        var width = lines[0].Length;
        if (lines.Any(l => l.Length != width)) {
            throw new ArgumentException("All lines must have the same length");
        }

        var array2d = new Array2D<T>(width, lines.Length);
        for (var y = 0; y < lines.Length; y++) {
            for (var x = 0; x < width; x++) {
                array2d[y, x] = transform(lines[y][x]);
            }
        }
        return array2d;
    }

    public static Array2D<char> Parse(string template) {
        return Parse(template, c => c);
    }

    public static Array2D<bool> ParseAsBool(string template, char trueChar) {
        return Parse(template, c => c == trueChar);
    }

    public static Array2D<bool> ParseAsBool(string template, params char[] trueChars) {
        return Parse(template, trueChars.Contains);
    }

    public static Array2D<int> ParseAsInt(string template, int defaultValue) {
        return Parse(template, c => char.IsDigit(c) ? c - '0' : defaultValue);
    }

    public static Array2D<int> ParseAsInt(string template) {
        return Parse(template, c => char.IsDigit(c) 
            ? c - '0' 
            : throw new ArgumentException($"Only digits are allowed: {c}"));
    }
}

/*
public interface IStorage<T> {
    void Set(int y, int x, T value);
    T Get(int y, int x);
    int GetLength(int dimension);
}

public class ArrayStorage<T>(T[,] data) : IStorage<T> {
    public T[,] Data { get; set; } = data;
    public void Set(int y, int x, T value) {
        Data[y, x] = value;
    }

    public T Get(int y, int x) {
        return Data[y, x];
    }

    public int GetLength(int dimension) {
        return Data.GetLength(dimension);
    }
}

public class BitArrayStorage(BitArray2D data) : IStorage<bool> {
    public BitArray2D Data { get; set; } = data;

    public void Set(int y, int x, bool value) {
        Data[y, x] = value;
    }

    public bool Get(int y, int x) {
        return Data[y, x];
    }

    public int GetLength(int dimension) {
        return dimension == 0 ? Data.Height : Data.Width;
    }
}
*/
/// <summary>
/// A bidimensional array (grid) where can be accessed in Column-major order. That means:
///
/// An Array2D to store an image of 1920x1080 is created with new Array2D<byte>(1920, 1080)
/// And accesses using (x, y) coordinates instead of new byte[1080, 1920] and accessed using (y, x) which
/// is how C# stores bidimensional arrays because it's Row-major order.  
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class Array2D<T> : Array2D, IEnumerable<DataCell<T>> {
    public override int Width => Data.GetLength(1);
    public override int Height => Data.GetLength(0);
    public override Rect2I Bounds => new(0, 0, Width, Height);
    public T[,] Data { get; set; }

    public Array2D(T[,] data) {
        Data = data;
    }

    public Array2D(int width, int height) {
        Data = new T[height, width];
    }
    
    /*
    public IStorage<T> Data { get; set; }

    private static IStorage<T> CreateStorage(int width, int height) {
        if (typeof(T) == typeof(bool)) {
            return (IStorage<T>)new BitArrayStorage(new BitArray2D(width, height));
        }
        return new ArrayStorage<T>(new T[height, width]);
    }

    public Array2D(T[,] data) {
        Data = new ArrayStorage<T>(data);
    }

    public Array2D(int width, int height) {
        Data = CreateStorage(width, height);
    }
    */


    public Array2D(int width, int height, T defaultValue) : this(width, height) {
        Fill(defaultValue);
    }

    public void Fill(T value) {
        Fill(0, 0, Width, Height, value);
    }

    public void Fill(Rect2I rect, T value) {
        Fill(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y, value);
    }

    public void Fill(int x, int y, int width, int height, T value) {
        for (var yy = y; yy < height + y; yy++) {
            for (var xx = x; xx < width + x; xx++) {
                this[yy, xx] = value;
            }
        }
    }

    public void Load(Func<Vector2I, T> valueFunc) {
        Load(0, 0, Width, Height, valueFunc);
    }

    public void Load(Func<int, int, T> valueFunc) {
        Load(0, 0, Width, Height, valueFunc);
    }

    public void Load(Rect2I rect, Func<Vector2I, T> valueFunc) {
        Load(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y, valueFunc);
    }

    public void Load(Rect2I rect, Func<int, int, T> valueFunc) {
        Load(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y, valueFunc);
    }

    public void Load(int x, int y, int width, int height, Func<Vector2I, T> valueFunc) {
        foreach (var cell in GetPositions(x, y, width, height)) {
            var value = valueFunc.Invoke(cell.Position);
            this[cell.Position] = value;
        }
    }

    public void Load(int x, int y, int width, int height, Func<int, int, T> valueFunc) {
        foreach (var cell in GetPositions(x, y, width, height)) {
            var value = valueFunc.Invoke(cell.Position.X, cell.Position.Y);
            this[cell.Position] = value;
        }
    }

    public Array2D<T> Clone() {
        return Clone(v => v);
    }

    public Array2D<TOut> Clone<TOut>(Func<T, TOut> transformer) {
        return Clone(0, 0, Width, Height, transformer);
    }

    public Array2D<TOut> Clone<TOut>(Rect2I rect, Func<T, TOut> transformer) {
        return Clone(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y, transformer);
    }

    public Array2D<TOut> Clone<TOut>(int x, int y, int width, int height, Func<T, TOut> transformer) {
        var startPos = new Vector2I(x, y);
        var array2D = new Array2D<TOut>(width, height);
        foreach (var cell in GetPositions(x, y, width, height)) {
            var transformed = transformer.Invoke(cell.Value);
            array2D[cell.Position - startPos] = transformed;
        }
        return array2D;
    }

    public IEnumerator<DataCell<T>> GetEnumerator() {
        for (var yy = 0; yy < Height; yy++) {
            for (var xx = 0; xx < Width; xx++) {
                var value = this[yy, xx];
                yield return new DataCell<T>(new Vector2I(xx, yy), value);
            }
        }
    }

    public IEnumerable<DataCell<T>> GetPositions() {
        return GetPositions(0, 0, Width, Height);        
    }
    
    public IEnumerable<DataCell<T>> GetPositions(Rect2I rect) {
        return GetPositions(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y);
    }

    public IEnumerable<DataCell<T>> GetPositions(int x, int y, int width, int height) {
        for (var yy = y; yy < height + y; yy++) {
            for (var xx = x; xx < width + x; xx++) {
                var value = this[yy, xx];
                yield return new DataCell<T>(new Vector2I(xx, yy), value);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public void Transform(Func<T, T> action) {
        Transform(0, 0, Width, Height, action);
    }

    public void Transform(Func<Vector2I, T, T> action) {
        Transform(0, 0, Width, Height, action);
    }

    public void Transform(Func<int, int, T, T> action) {
        Transform(0, 0, Width, Height, action);
    }

    public void Transform(Rect2I rect, Func<T, T> action) {
        Transform(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y, action);
    }

    public void Transform(Rect2I rect, Func<Vector2I, T, T> action) {
        Transform(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y, action);
    }

    public void Transform(Rect2I rect, Func<int, int, T, T> action) {
        Transform(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y, action);
    }

    public void Transform(int x, int y, int width, int height, Func<T, T> action) {
        foreach (var cell in GetPositions(x, y, width, height)) {
            var transformed = action(cell.Value);
            this[cell.Position] = transformed;
        }
    }

    public void Transform(int x, int y, int width, int height, Func<Vector2I, T, T> action) {
        foreach (var cell in GetPositions(x, y, width, height)) {
            var transformed = action(cell.Position, cell.Value);
            this[cell.Position] = transformed;
        }
    }

    public void Transform(int x, int y, int width, int height, Func<int, int, T, T> action) {
        foreach (var cell in GetPositions(x, y, width, height)) {
            var transformed = action(cell.Position.X, cell.Position.Y, cell.Value);
            this[cell.Position] = transformed;
        }
    }

    public bool IsValidPosition(Vector2I position) {
        return IsValidPosition(position.X, position.Y);
    }

    public bool IsValidPosition(int x, int y) {
        return Geometry.Geometry.IsPointInRectangle(x, y, 0, 0, Width, Height);
    }

    public T? GetValueSafe(Vector2I pos, T? defaultValue = default) {
        return GetValueSafe(pos.X, pos.Y, defaultValue);
    }

    public T? GetValueSafe(int x, int y, T? defaultValue = default) {
        if (x < 0 || y < 0 || x >= Width || y >= Height) return defaultValue;
        return this[y, x];
    }

    public T this[int y, int x] {
        get => Data[y, x];
        set => Data[y, x] = value;
    }
    
    public T this[Vector2I pos] {
        get => this[pos.Y, pos.X];
        set => this[pos.Y, pos.X] = value;
    }

    public void CopyNeighbors(Vector2I center, T[,] destination, T defaultValue = default) {
        CopyNeighbors(center.X, center.Y, destination, value => value, defaultValue);
    }

    public void CopyNeighbors(int centerX, int centerY, T[,] destination, T defaultValue = default) {
        CopyNeighbors(centerX, centerY, destination, value => value, defaultValue);
    }

    public void CopyNeighbors<TDest>(Vector2I center, TDest[,] destination, Func<T, TDest> transformer, TDest defaultValue = default) {
        CopyNeighbors(center.X, center.Y, destination, transformer, defaultValue);
    }

    public void CopyNeighbors<TDest>(int centerX, int centerY, TDest[,] destination, Func<T, TDest> transformer, TDest defaultValue = default) {
        var width = destination.GetLength(1);
        var height = destination.GetLength(0);
        if (width % 2 == 0 || height % 2 == 0) throw new Exception("width and height of destination array must be odd numbers, to ensure a center point");
        CopyTo(centerX - width / 2, centerY - height / 2, width, height, destination, transformer, defaultValue);
    }

    public void CopyTo(T[,] destination, T defaultValue = default) {
        var height = destination.GetLength(0);
        var width = destination.GetLength(1);
        CopyTo(0, 0, width, height, destination, defaultValue);
    }

    public void CopyTo(int startX, int startY, T[,] destination, T defaultValue = default) {
        var height = destination.GetLength(0);
        var width = destination.GetLength(1);
        CopyTo(startX, startY, width, height, destination, value => value, defaultValue);
    }

    public void CopyTo(Rect2I rect, int height, T[,] destination, T defaultValue = default) {
        CopyTo(rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y, destination, defaultValue);
    }

    public void CopyTo(int startX, int startY, int width, int height, T[,] destination, T defaultValue = default) {
        CopyTo(startX, startY, width, height, destination, value => value, defaultValue);
    }

    /// <summary>
    /// Copy all the values to the destination array.
    /// If destination array is bigger, the remaining values will be filled with defaultValue.
    /// </summary>
    public void CopyTo<TDest>(TDest[,] destination, Func<T, TDest> transformer, TDest defaultValue = default) {
        var height = destination.GetLength(0);
        var width = destination.GetLength(1);
        CopyTo(0, 0, width, height, destination, transformer, defaultValue);
    }

    public void CopyTo<TDest>(int startX, int startY, TDest[,] destination, Func<T, TDest> transformer, TDest defaultValue = default) {
        var height = destination.GetLength(0);
        var width = destination.GetLength(1);
        CopyTo(startX, startY, width, height, destination, transformer, defaultValue);
    }

    public void CopyTo<TDest>(Rect2I rect, int height, TDest[,] destination, Func<T, TDest> transformer, TDest defaultValue = default) {
        CopyTo(rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y, destination, transformer, defaultValue);
    }

    /// <summary>
    /// Copy the values from startX, startY to the destination array.
    /// If destination array is bigger, the remaining values will be filled with defaultValue.
    /// </summary>
    public void CopyTo<TDest>(int startX, int startY, int width, int height, TDest[,] destination, Func<T, TDest> transformer, TDest defaultValue = default) {
        height = Math.Min(height, destination.GetLength(0));
        width = Math.Min(width, destination.GetLength(1));
        for (var destY = 0; destY < height; destY++) {
            for (var destX = 0; destX < width; destX++) {
                var sourceX = startX + destX;
                var sourceY = startY + destY;
                destination[destY, destX] = sourceX >= 0 && sourceX < Width && sourceY >= 0 && sourceY < Height
                    ? transformer(this[sourceY, sourceX])
                    : defaultValue;
            }
        }
    }
    
    public IEnumerable<Vector2I> GetOrtogonalPositions(int x, int y, Func<Vector2I, bool>? predicate = null) {
        return GetOrtogonalPositions(new Vector2I(x, y), predicate);
    }

    public IEnumerable<Vector2I> GetOrtogonalPositions(Vector2I pos, Func<Vector2I, bool>? predicate = null) {
        return Directions
            .Select(dir => pos + dir)
            .Where(p => IsValidPosition(p) && (predicate == null || predicate.Invoke(p)));
    }

    public override string ToString() {
        return GetString(cell => cell.Value?.ToString());
    }

    public string GetString(Func<DataCell<T>, string> transform, char lineSeparator = '\n') {
        var s = new StringBuilder();
        foreach (var cell in this) {
            if (cell.Position.Y > 0 && cell.Position.X == 0) s.Append(lineSeparator);
            s.Append(transform(cell));
        }
        return s.ToString();
    }
    
    public string GetString(Func<T, string> transform, char lineSeparator = '\n') {
        return GetString(cell => transform(cell.Value), lineSeparator);
    }
}