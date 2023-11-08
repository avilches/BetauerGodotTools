using System;

namespace Betauer.Core.Data;

/// <summary>
/// A bidimensional array of float values that can be normalized to a given range
/// </summary>
public class NormalizedDataGrid : INormalizedDataGrid<float> {
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float[,] Values { get; set; }
    public Func<int, int, float> ValueFunc { get; }
    public float Min { get; private set; } = float.MaxValue;
    public float Max { get; private set; } = float.MinValue;

    public NormalizedDataGrid(int width, int height, Func<int, int, float> value) {
        Resize(width, height);
        ValueFunc = value;
    }

    public void Resize(int width, int height) {
        Values = new float[height, width];
        Width = width;
        Height = height;
    }

    public void Load() {
        Min = float.MaxValue;
        Max = float.MinValue;
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var value = ValueFunc(x, y);
                Values[y, x] = value;
                Min = Math.Min(Min, value);
                Max = Math.Max(Max, value);
            }
        }
        var minMaxRange = Max - Min;
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                Values[y, x] = (Values[y, x] - Min) / minMaxRange;
            }
        }
    }

    public float GetValue(int x, int y) {
        return Values[y, x];
    }
}