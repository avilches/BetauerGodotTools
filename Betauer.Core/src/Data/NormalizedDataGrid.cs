using System;

namespace Betauer.Core.Data;

public class NormalizedDataGrid {
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float MinValue { get; private set; } = float.MaxValue;
    public float MaxValue { get; private set; } = float.MinValue;

    public float[,] Data { get; private set; }

    public NormalizedDataGrid(int width, int height, Func<int, int, float> valueFunc) {
        Data = new float[width, height];
        Width = Data.GetLength(0);
        Height = Data.GetLength(1);
        Load(valueFunc);
    }

    public NormalizedDataGrid(int width, int height, float defaultValue) {
        Data = new float[width, height];
        Width = Data.GetLength(0);
        Height = Data.GetLength(1);
        Fill(defaultValue);
    }

    public NormalizedDataGrid(float[,] data) {
        SetAll(data);
    }

    public NormalizedDataGrid Fill(float value) {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                Data[x, y] = value;
            }
        }
        MinValue = value;
        MaxValue = value;
        return this;
    }

    public NormalizedDataGrid Fill(int x, int y, int width, int height, float value) {
        for (var xx = x; xx < width - x; xx++) {
            for (var yy = y; yy < height - y; yy++) {
                Data[xx, yy] = value;
            }
        }
        MinValue = Math.Min(MinValue, value);
        MaxValue = Math.Max(MaxValue, value);
        return this;
    }

    public NormalizedDataGrid SetAll(float[,] data) {
        MinValue = float.MaxValue;
        MaxValue = float.MinValue;
        Width = data.GetLength(0);
        Height = data.GetLength(1);
        Data = new float[Width, Height];
        for (var y = 0; y < data.GetLength(1); y++) {
            for (var x = 0; x < data.GetLength(0); x++) {
                var value = data[x, y];
                Data[x, y] = value;
                MinValue = Math.Min(MinValue, value);
                MaxValue = Math.Max(MaxValue, value);
            }
        }
        return this;
    }

    public NormalizedDataGrid Load(Func<int, int, float> valueFunc) {
        MinValue = float.MaxValue;
        MaxValue = float.MinValue;
        Load(0, 0, Width, Height, valueFunc);
        return this;
    }

    public NormalizedDataGrid Load(int x, int y, int width, int height, Func<int, int, float> valueFunc) {
        for (var xx = x; xx < width - x; xx++) {
            for (var yy = y; yy < height - y; yy++) {
                var value = valueFunc.Invoke(xx, yy);
                Data[xx, yy] = value;
                MinValue = Math.Min(MinValue, value);
                MaxValue = Math.Max(MaxValue, value);
            }
        }
        return this;
    }

    public NormalizedDataGrid UpdateMinMax() {
        MinValue = float.MaxValue;
        MaxValue = float.MinValue;
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var value = Data[x, y];
                Data[x, y] = value;
                MinValue = Math.Min(MinValue, value);
                MaxValue = Math.Max(MaxValue, value);
            }
        }
        return this;
    }

    public NormalizedDataGrid Normalize(float newMin, float newMax) {
        var minMaxRange = MaxValue - MinValue;
        var normalizedRange = newMax - newMin;
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                Data[x, y] = (Data[x, y] - MinValue) / minMaxRange * normalizedRange + newMin;
            }
        }
        MinValue = newMin;
        MaxValue = newMax;
        return this;
    }

    public NormalizedDataGrid Normalize() {
        var minMaxRange = MaxValue - MinValue;
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                Data[x, y] = (Data[x, y] - MinValue) / minMaxRange;
            }
        }
        MinValue = 0;
        MaxValue = 1;
        return this;
    }

    public NormalizedDataGrid Loop(Action<float, int, int> action) {
        Loop(0, 0, Width, Height, action);
        return this;
    }

    public NormalizedDataGrid Transform(Func<float, float> action) {
        MinValue = float.MaxValue;
        MaxValue = float.MinValue;
        Transform(0, 0, Width, Height, action);
        return this;
    }

    public NormalizedDataGrid Transform(int x, int y, int width, int height, Func<float, float> action) {
        for (var xx = x; xx < width - x; xx++) {
            for (var yy = y; yy < height - y; yy++) {
                var value = GetValue(xx, yy);
                SetValue(xx, yy, action(value));
            }
        }
        return this;
    }

    public NormalizedDataGrid Loop(int x, int y, int width, int height, Action<float, int, int> action) {
        for (var xx = x; xx < width - x; xx++) {
            for (var yy = y; yy < height - y; yy++) {
                action.Invoke(GetValue(xx, yy), xx, yy);
            }
        }
        return this;
    }

    public void SetValue(int x, int y, float value) {
        Data[x, y] = value;
        MinValue = Math.Min(MinValue, value);
        MaxValue = Math.Max(MaxValue, value);
    }

    public float GetValue(int x, int y) {
        return Data[x, y];
    }
}