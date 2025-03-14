using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Godot;

namespace Betauer.Core.DataMath;

public abstract class Array2D {
    public abstract int Width { get; }
    public abstract int Height { get; }
    public abstract Rect2I Bounds { get; }
    public abstract Vector2I Size { get; }

    public static readonly ImmutableArray<Vector2I> VonNeumannDirections = [
        Vector2I.Up, Vector2I.Right, Vector2I.Down, Vector2I.Left
    ];

    public static readonly ImmutableArray<Vector2I> MooreDirections = [
        Vector2I.Up,
        Vector2I.Up + Vector2I.Right,
        Vector2I.Right,
        Vector2I.Down + Vector2I.Right,
        Vector2I.Down,
        Vector2I.Down + Vector2I.Left,
        Vector2I.Left,
        Vector2I.Up + Vector2I.Left
    ];

    public static readonly ImmutableArray<Vector2I> DiagonalDirections = [
        Vector2I.Up + Vector2I.Right,
        Vector2I.Down + Vector2I.Right,
        Vector2I.Down + Vector2I.Left,
        Vector2I.Up + Vector2I.Left
    ];

    public static Array2D<T> Parse<T>(string template, Dictionary<char, T> transform) {
        return Parse(template, c => transform[c]);
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
        // Modified validation with detailed error message
        for (var i = 0; i < lines.Length; i++) {
            var line = lines[i];
            if (line.Length != width) {
                throw new ArgumentException(
                    $"Line {i + 1} has incorrect length: {line.Length} (expected {width}). Line content: '{line}'");
            }
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

/// <summary>
/// A bidimensional array (grid) where can be accessed in Column-major order. That means:
///
/// An Array2D to store an image of 1920x1080 is created with new Array2D<byte>(1920, 1080)
/// And accesses using (x, y) coordinates instead of new byte[1080, 1920] and accessed using (y, x) which
/// is how C# stores bidimensional arrays because it's Row-major order.  
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class Array2D<T>(T[,] data) : Array2D, IEnumerable<T> {
    public override int Width => Data.GetLength(1);
    public override int Height => Data.GetLength(0);
    public override Rect2I Bounds => new(0, 0, Size);
    public override Vector2I Size => new(Width, Height);
    public T[,] Data { get; set; } = data;

    public Array2D(Vector2I size) : this(size.X, size.Y) {
    }

    public Array2D(Vector2I size, T defaultValue) : this(size.X, size.Y, defaultValue) {
    }

    public Array2D(int width, int height) : this(new T[height, width]) {
    }

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
                Data[yy, xx] = value;
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
        foreach (var cell in GetIndexedValues(x, y, width, height)) {
            var value = valueFunc.Invoke(cell.Position);
            this[cell.Position] = value;
        }
    }

    public void Load(int x, int y, int width, int height, Func<int, int, T> valueFunc) {
        foreach (var cell in GetIndexedValues(x, y, width, height)) {
            var value = valueFunc.Invoke(cell.Position.X, cell.Position.Y);
            this[cell.Position] = value;
        }
    }

    public Array2D<T> Clone() {
        return Clone(v => v);
    }

    public Array2D<T> Expand(int scale) {
        if (scale < 1) throw new ArgumentException("Scale must be greater than 0");
        if (scale == 1) return this;

        var array2D = new Array2D<T>(Width * scale, Height * scale);
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var value = Data[y, x];

                for (var dy = 0; dy < scale; dy++) {
                    for (var dx = 0; dx < scale; dx++) {
                        array2D[y * scale + dy, x * scale + dx] = value;
                    }
                }
            }
        }
        return array2D;
    }

    public Array2D<TCell> Expand<TCell>(int scale, Action<Vector2I, T, Array2D<TCell>> valueFunc) {
        if (scale < 1) throw new ArgumentException("Scale must be greater than 0");

        var buffer = new Array2D<TCell>(scale, scale);
        var array2D = new Array2D<TCell>(Width * scale, Height * scale);
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                buffer.Fill(default);
                valueFunc.Invoke(new Vector2I(x, y), Data[y, x], buffer);
                for (var dy = 0; dy < scale; dy++) {
                    for (var dx = 0; dx < scale; dx++) {
                        array2D[y * scale + dy, x * scale + dx] = buffer[dy, dx];
                    }
                }
            }
        }
        return array2D;
    }

    public Array2D<TCell> Expand<TCell>(int scale, Func<Vector2I, T, Array2D<TCell>> valueFunc) {
        if (scale < 1) throw new ArgumentException("Scale must be greater than 0");

        var array2D = new Array2D<TCell>(Width * scale, Height * scale);
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var value = valueFunc.Invoke(new Vector2I(x, y), Data[y, x]);

                if (value == null) continue;
                for (var dy = 0; dy < scale; dy++) {
                    for (var dx = 0; dx < scale; dx++) {
                        array2D[y * scale + dy, x * scale + dx] = value[dy, dx];
                    }
                }
            }
        }
        return array2D;
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
        foreach (var cell in GetIndexedValues(x, y, width, height)) {
            var transformed = transformer.Invoke(cell.Value);
            array2D[cell.Position - startPos] = transformed;
        }
        return array2D;
    }

    public IEnumerator<T> GetEnumerator() {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                yield return Data[y, x];
            }
        }
    }

    public IEnumerable<Vector2I> GetPositions() {
        return GetPositions(0, 0, Width, Height);
    }

    public IEnumerable<Vector2I> GetPositions(Rect2I rect) {
        return GetPositions(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y);
    }

    public IEnumerable<Vector2I> GetPositions(int x, int y, int width, int height) {
        for (var yy = y; yy < height + y; yy++) {
            for (var xx = x; xx < width + x; xx++) {
                yield return new Vector2I(xx, yy);
            }
        }
    }

    public IEnumerable<(Vector2I Position, T Value)> GetIndexedValues() {
        return GetIndexedValues(0, 0, Width, Height);
    }

    public IEnumerable<(Vector2I Position, T Value)> GetIndexedValues(Rect2I rect) {
        return GetIndexedValues(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y);
    }

    public IEnumerable<(Vector2I Position, T Value)> GetIndexedValues(int x, int y, int width, int height) {
        for (var yy = y; yy < height + y; yy++) {
            for (var xx = x; xx < width + x; xx++) {
                var pos = new Vector2I(xx, yy);
                yield return (Position: pos, Value: Data[yy, xx]);
            }
        }
    }

    public IEnumerable<(Vector2I Position, TO Value)> GetIndexedValues<TO>() {
        return GetIndexedValues<TO>(0, 0, Width, Height);
    }

    public IEnumerable<(Vector2I Position, TO Value)> GetIndexedValues<TO>(Rect2I rect) {
        return GetIndexedValues<TO>(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y);
    }

    public IEnumerable<(Vector2I Position, TO Value)> GetIndexedValues<TO>(int x, int y, int width, int height) {
        for (var yy = y; yy < height + y; yy++) {
            for (var xx = x; xx < width + x; xx++) {
                if (Data[yy, xx] is not TO value) continue;
                var pos = new Vector2I(xx, yy);
                yield return (Position: pos, Value: value);
            }
        }
    }

    public IEnumerable<T> GetValues() {
        return GetValues(0, 0, Width, Height);
    }

    public IEnumerable<T> GetValues(Rect2I rect) {
        return GetValues(rect.Position.X, rect.Position.Y, rect.End.X, rect.End.Y);
    }

    public IEnumerable<T> GetValues(int x, int y, int width, int height) {
        for (var yy = y; yy < height + y; yy++) {
            for (var xx = x; xx < width + x; xx++) {
                yield return Data[yy, xx];
            }
        }
    }
    
    /// <summary>
    /// Divides the array into smaller sub-arrays of the specified width and height.
    /// The width and height parameters must be divisible by the array's dimensions.
    /// </summary>
    /// <param name="width">The width of each sub-array. Must be a divisor of the array's width.</param>
    /// <param name="height">The height of each sub-array. Must be a divisor of the array's height.</param>
    /// <returns>An enumerable of sub-arrays with the specified dimensions.</returns>
    public IEnumerable<(Vector2I Position, Array2D<T> Section)> GetSubArray2D(int width, int height) {
        foreach (var (pos, rect) in GetRects(width, height)) {
            var data = new Array2D<T>(width, height);
            CopyTo(rect.Position.X, rect.Position.Y, width, height, data.Data);
            yield return (pos, data);
        }
    }

    public IEnumerable<(Vector2I Position, Rect2I Section)> GetRects(int width, int height) {
        if (Width % width != 0) {
            throw new ArgumentException($"Width {width} is not a divisor of the array width {Width}");
        }
        if (Height % height != 0) {
            throw new ArgumentException($"Height {height} is not a divisor of the array height {Height}");
        }
    
        var numHorizontalArrays = Width / width;
        var numVerticalArrays = Height / height;
    
        for (var y = 0; y < numVerticalArrays; y++) {
            for (var x = 0; x < numHorizontalArrays; x++) {
                yield return (new Vector2I(x, y), new Rect2I(x * width, y * height, width, height));
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
        foreach (var cell in GetIndexedValues(x, y, width, height)) {
            var transformed = action(cell.Value);
            this[cell.Position] = transformed;
        }
    }

    public void Transform(int x, int y, int width, int height, Func<Vector2I, T, T> action) {
        foreach (var cell in GetIndexedValues(x, y, width, height)) {
            var transformed = action(cell.Position, cell.Value);
            this[cell.Position] = transformed;
        }
    }

    public void Transform(int x, int y, int width, int height, Func<int, int, T, T> action) {
        foreach (var cell in GetIndexedValues(x, y, width, height)) {
            var transformed = action(cell.Position.X, cell.Position.Y, cell.Value);
            this[cell.Position] = transformed;
        }
    }

    public bool IsInBounds(Vector2I position) {
        return IsInBounds(position.X, position.Y);
    }

    public bool IsInBounds(int x, int y) {
        return Geometry.Geometry.IsPointInRectangle(x, y, 0, 0, Width, Height);
    }

    public T? GetValueSafe(Vector2I pos, T? defaultValue = default) {
        return GetValueSafe(pos.X, pos.Y, defaultValue);
    }

    public T GetOrSet(Vector2I pos, Func<T> create) {
        var value = this[pos];
        if (value != null) return value;
        return this[pos] = create();
    }

    public T GetOrSet(Vector2I pos, Func<Vector2I, T> create) {
        var value = this[pos];
        if (value != null) return value;
        return this[pos] = create(pos);
    }

    private T GetOrSet(int x, int y, Func<T> create) {
        var value = Data[y, x];
        if (value != null) return value;
        return Data[y, x] = create();
    }

    private T GetOrSet(int x, int y, Func<int, int, T> create) {
        var value = Data[y, x];
        if (value != null) return value;
        return Data[y, x] = create(x, y);
    }

    public T? GetValueSafe(int x, int y, T? defaultValue = default) {
        if (x < 0 || y < 0 || x >= Width || y >= Height) return defaultValue;
        return Data[y, x];
    }

    public T this[int y, int x] {
        get => Data[y, x];
        set => Data[y, x] = value;
    }

    public T this[Vector2I pos] {
        get => Data[pos.Y, pos.X];
        set => Data[pos.Y, pos.X] = value;
    }

    public void CopyChebyshevRegion(Vector2I center, T[,] destination, T defaultValue = default) {
        CopyChebyshevRegion(center.X, center.Y, destination, value => value, defaultValue);
    }

    public void CopyChebyshevRegion(int centerX, int centerY, T[,] destination, T defaultValue = default) {
        CopyChebyshevRegion(centerX, centerY, destination, value => value, defaultValue);
    }

    public void CopyChebyshevRegion<TDest>(Vector2I center, TDest[,] destination, Func<T, TDest> transformer, TDest defaultValue = default) {
        CopyChebyshevRegion(center.X, center.Y, destination, transformer, defaultValue);
    }

    public void CopyChebyshevRegion<TDest>(int centerX, int centerY, TDest[,] destination, Func<T, TDest> transformer, TDest defaultValue = default) {
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
                    ? transformer(Data[sourceY, sourceX])
                    : defaultValue;
            }
        }
    }

    /// <summary>
    /// Returns the 4 positions around the center (up, down, left and right)
    /// if they exist and the (optional) predicate returns tru
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public IEnumerable<Vector2I> GetVonNeumannPositions(Vector2I pos) {
        return VonNeumannDirections
            .Select(dir => pos + dir)
            .Where(IsInBounds);
    }

    /// <summary>
    /// Returns the 4 positions around the center (up, down, left and right)
    /// if they exist
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Vector2I> GetVonNeumannPositions(int x, int y) {
        return GetVonNeumannPositions(new Vector2I(x, y));
    }

    /// <summary>
    /// Returns the 8 positions around the center (up, up-right, up-left, down, down-right, down-left, left and right)
    /// if they exist
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public IEnumerable<Vector2I> GetMoorePositions(Vector2I pos) {
        return MooreDirections
            .Select(dir => pos + dir)
            .Where(IsInBounds);
    }

    /// <summary>
    /// Returns the 8 positions around the center (up, up-right, up-left, down, down-right, down-left, left and right)
    /// if they exist
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Vector2I> GetMoorePositions(int x, int y) {
        return GetMoorePositions(new Vector2I(x, y));
    }


    /// <summary>
    /// Returns the 4 diagonal positions around the center (up-right, up-left, down-right, down-left)
    /// if they exist
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <returns></returns>
    public IEnumerable<Vector2I> GetDiagonalPositions(Vector2I pos) {
        return DiagonalDirections
            .Select(dir => pos + dir)
            .Where(IsInBounds);
    }

    /// <summary>
    /// Returns the 4 diagonal positions around the center (up-right, up-left, down-right, down-left)
    /// if they exist
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Vector2I> GetDiagonalPositions(int x, int y) {
        return GetDiagonalPositions(new Vector2I(x, y));
    }

    public override string ToString() {
        return GetString(value => value?.ToString());
    }

    public string GetString(Func<(Vector2I Position, T Value), string> transform, char lineSeparator = '\n') {
        var s = new StringBuilder();
        foreach (var item in GetIndexedValues()) {
            if (item.Position.Y > 0 && item.Position.X == 0) s.Append(lineSeparator);
            s.Append(transform(item));
        }
        return s.ToString();
    }

    public string GetString(Func<T, string> transform, char lineSeparator = '\n') {
        return GetString(item => transform(item.Value), lineSeparator);
    }

    public void ForEach(Action<T> action) {
        foreach (var o in this) {
            action(o);
        }
    }
}