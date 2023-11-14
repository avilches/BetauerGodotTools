using System;

namespace Betauer.Core.Data;

/// <summary>
/// A bidimensional array of float values that can be normalized to a given range
/// </summary>
public class NormalizedDataGrid : DataGrid<float> {

    public NormalizedDataGrid(int width, int height) : base(width, height) {
    }

    public NormalizedDataGrid(float[,] data) : base(data) {
    }

    public void Normalize() {
        var min = float.MaxValue;
        var max = float.MinValue;
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var value = Data[x, y];
                Data[x, y] = value;
                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }
        }
        Normalize(min, max);
    }

    public void Normalize(float min, float max) {
        var minMaxRange = max - min;
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                Data[x, y] = (Data[x, y] - min) / minMaxRange;
            }
        }
    }
}