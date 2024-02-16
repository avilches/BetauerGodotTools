using System;

namespace Betauer.Core.Data;

public class DataGrid<T> {
    public int Width { get; private set; }
    public int Height { get; private set; }

    public T[,] Data { get; private set; }

    public DataGrid(int width, int height, Func<int, int, T> valueFunc) {
        Data = new T[width, height];
        Width = Data.GetLength(0);
        Height = Data.GetLength(1);
        Load(valueFunc);
    }

    public DataGrid(int width, int height) {
        Data = new T[width, height];
        Width = Data.GetLength(0);
        Height = Data.GetLength(1);
    }

    public DataGrid(int width, int height, T defaultValue) {
        Data = new T[width, height];
        Width = Data.GetLength(0);
        Height = Data.GetLength(1);
        Fill(defaultValue);
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
}