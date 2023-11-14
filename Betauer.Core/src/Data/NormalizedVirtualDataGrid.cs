using System;

namespace Betauer.Core.Data;

public class NormalizedVirtualDataGrid : IDataGrid<float> {
    public Func<int, int, float> ValueFunc { get; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float Min { get; private set; }
    public float Max { get; private set; }

    public NormalizedVirtualDataGrid(int width, int height, Func<int, int, float> valueFunc) {
        Resize(width, height);
        ValueFunc = valueFunc;
    }

    public void Resize(int width, int height) {
        Width = width;
        Height = height;
    }

    public void Normalize() {
        Min = float.MaxValue;
        Max = float.MinValue;
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) { 
                var value = ValueFunc.Invoke(x, y);
                Min = Math.Min(Min, value);
                Max = Math.Max(Max, value);
            }
        }
    }

    public float GetValue(int x, int y) {
        var minMaxRange = Max - Min;
        var value = ValueFunc(x, y);
        return (value - Min) / minMaxRange;
    }
}