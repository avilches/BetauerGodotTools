using System;

namespace Betauer.Core.Data;

public class NormalizedVirtualDataGrid : INormalizedDataGrid {
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Func<int, int, float> ValueFunc { get; }
    public float Min { get; private set; } = float.MaxValue;
    public float Max { get; private set; } = float.MinValue;
    
    public NormalizedVirtualDataGrid(int width, int height, Func<int, int, float> value) {
        Resize(width, height);
        ValueFunc = value;
    }

    public void Resize(int width, int height) {
        Width = width;
        Height = height;
    }

    public void Load() {
        Min = float.MaxValue;
        Max = float.MinValue;
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) { 
                var value = ValueFunc(x, y);
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