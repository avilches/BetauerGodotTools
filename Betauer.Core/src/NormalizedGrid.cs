using System;

namespace Betauer.Core;

/// <summary>
/// A bidimensional array of float values that can be normalized to a given range
/// </summary>
public class NormalizedGrid {
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float RangeMin { get; private set; } = 0f;
    public float RangeMax { get; private set; } = 1.0f;
    public float[,] Values { get; set; }
    public Func<int, int, float> ValueFunc { get; }
    
    public NormalizedGrid(int width, int height, Func<int, int, float> value) {
        Resize(width, height);
        ValueFunc = value;
    }

    public void Resize(int width, int height) {
        Values = new float[height, width];
        Width = width;
        Height = height;
    }

    public void NormalizedRange(float min, float max) {
        RangeMin = min;
        RangeMax = max;
    }

    public void Load() {
        var min = float.MaxValue;
        var max = float.MinValue;
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) { 
                var value = ValueFunc(x, y);
                Values[y, x] = value;
                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }
        }
        var range = RangeMax - RangeMin;
        var minMaxRange = max - min;
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                Values[y, x] = (Values[y, x] - min) / minMaxRange * range + RangeMin;
            }
        }
    }
    
    public float GetValue(int x, int y) {
        return Values[y, x];
    }
}