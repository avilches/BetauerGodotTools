using System;

namespace Betauer.Core.DataMath.Data;

public static partial class XyDataGridExtensions {
    public static XyDataGrid<float> Normalize(this XyDataGrid<float> grid, float newMin, float newMax) {
        var min = float.MaxValue;
        var max = float.MinValue;
        grid.Loop((value, x, y) => {
            min = Math.Min(min, value);
            max = Math.Max(max, value);
        });
        var range = max - min;
        var normalizedRange = newMax - newMin;
        grid.Transform(value => (value - min) / range * normalizedRange + newMin);
        return grid;
    }

    public static XyDataGrid<float> LoadNormalized(this XyDataGrid<float> grid, Func<int, int, float> valueFunc, float newMin, float newMax) {
        var min = float.MaxValue;
        var max = float.MinValue;
        grid.Loop((_, x, y) => {
            var value = valueFunc.Invoke(x, y);
            grid.SetValue(x, y, value);
            min = Math.Min(min, value);
            max = Math.Max(max, value);
        });
        var range = max - min;
        var normalizedRange = newMax - newMin;
        grid.Transform(value => (value - min) / range * normalizedRange + newMin);
        return grid;
    }

    public static XyDataGrid<float> Normalize(this XyDataGrid<float> grid) {
        var min = float.MaxValue;
        var max = float.MinValue;
        grid.Loop((value, x, y) => {
            min = Math.Min(min, value);
            max = Math.Max(max, value);
        });
        var range = max - min;
        grid.Transform(value => (value - min) / range);
        return grid;
    }

    public static XyDataGrid<float> LoadNormalized(this XyDataGrid<float> grid, Func<int, int, float> valueFunc) {
        var min = float.MaxValue;
        var max = float.MinValue;
        grid.Loop((_, x, y) => {
            var value = valueFunc.Invoke(x, y);
            grid.SetValue(x, y, value);
            min = Math.Min(min, value);
            max = Math.Max(max, value);
        });
        var range = max - min;
        grid.Transform(value => (value - min) / range);
        return grid;
    }
}