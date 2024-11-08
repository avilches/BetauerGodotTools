using System;

namespace Betauer.Core.DataMath.Data;

public static partial class DataGridExtensions {
    public static DataGrid<float> Normalize(this DataGrid<float> grid, float newMin, float newMax) {
        var min = float.MaxValue;
        var max = float.MinValue;
        grid.Loop((value, x, y) => {
            min = System.Math.Min(min, value);
            max = System.Math.Max(max, value);
        });
        var range = max - min;
        var normalizedRange = newMax - newMin;
        grid.Transform(value => (value - min) / range * normalizedRange + newMin);
        return grid;
    }

    public static DataGrid<float> LoadNormalized(this DataGrid<float> grid, Func<int, int, float> valueFunc, float newMin, float newMax) {
        var min = float.MaxValue;
        var max = float.MinValue;
        grid.Loop((_, x, y) => {
            var value = valueFunc.Invoke(x, y);
            grid.SetValue(x, y, value);
            min = System.Math.Min(min, value);
            max = System.Math.Max(max, value);
        });
        var range = max - min;
        var normalizedRange = newMax - newMin;
        grid.Transform(value => (value - min) / range * normalizedRange + newMin);
        return grid;
    }

    public static DataGrid<float> Normalize(this DataGrid<float> grid) {
        var min = float.MaxValue;
        var max = float.MinValue;
        grid.Loop((value, x, y) => {
            min = System.Math.Min(min, value);
            max = System.Math.Max(max, value);
        });
        var range = max - min;
        grid.Transform(value => (value - min) / range);
        return grid;
    }

    public static DataGrid<float> LoadNormalized(this DataGrid<float> grid, Func<int, int, float> valueFunc) {
        var min = float.MaxValue;
        var max = float.MinValue;
        grid.Loop((_, x, y) => {
            var value = valueFunc.Invoke(x, y);
            grid.SetValue(x, y, value);
            min = System.Math.Min(min, value);
            max = System.Math.Max(max, value);
        });
        var range = max - min;
        grid.Transform(value => (value - min) / range);
        return grid;
    }
}