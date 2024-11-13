using System;

namespace Betauer.Core.DataMath.Data;

public static partial class YxDataGridExtensions {
    
    /// <summary>
    /// Normalizes the values in the given YxDataGrid to a range between 0 and 1.
    /// </summary>
    /// <param name="grid">The YxDataGrid to normalize.</param>
    /// <returns>The normalized YxDataGrid.</returns>
    public static DataGrid<float> Normalize(this DataGrid<float> grid) {
        var min = float.MaxValue;
        var max = float.MinValue;
        foreach (var cell in grid) {
            min = Math.Min(min, cell.Value);
            max = Math.Max(max, cell.Value);
        }
        var range = max - min;
        grid.Transform(value => (value - min) / range);
        return grid;
    }

    
    /// <summary>
    /// Normalizes the values in the given YxDataGrid to a specified range.
    /// </summary>
    /// <param name="grid">The YxDataGrid to normalize.</param>
    /// <param name="newMin">The new minimum value of the normalized range.</param>
    /// <param name="newMax">The new maximum value of the normalized range.</param>
    /// <returns>The normalized YxDataGrid.</returns>
    public static DataGrid<float> Normalize(this DataGrid<float> grid, float newMin, float newMax) {
        var min = float.MaxValue;
        var max = float.MinValue;
        foreach (var cell in grid) {
            min = Math.Min(min, cell.Value);
            max = Math.Max(max, cell.Value);
        }
        var range = max - min;
        var normalizedRange = newMax - newMin;
        grid.Transform(value => (value - min) / range * normalizedRange + newMin);
        return grid;
    }

    /// <summary>
    /// Loads values into the YxDataGrid using a function and normalizes them to a specified range.
    /// </summary>
    /// <param name="grid">The YxDataGrid to load and normalize.</param>
    /// <param name="valueFunc">A function that provides values based on grid coordinates.</param>
    /// <param name="newMin">The new minimum value of the normalized range.</param>
    /// <param name="newMax">The new maximum value of the normalized range.</param>
    /// <returns>The loaded and normalized YxDataGrid.</returns>
    public static DataGrid<float> LoadNormalized(this DataGrid<float> grid, Func<int, int, float> valueFunc, float newMin, float newMax) {
        var min = float.MaxValue;
        var max = float.MinValue;
        foreach (var cell in grid) {
            var value = valueFunc.Invoke(cell.Position.X, cell.Position.Y);
            grid.SetValue(cell.Position, value);
            min = Math.Min(min, value);
            max = Math.Max(max, value);
        }
        var range = max - min;
        var normalizedRange = newMax - newMin;
        grid.Transform(value => (value - min) / range * normalizedRange + newMin);
        return grid;
    }

    /// <summary>
    /// Loads values into the YxDataGrid using a function and normalizes them to a range between 0 and 1.
    /// </summary>
    /// <param name="grid">The YxDataGrid to load and normalize.</param>
    /// <param name="valueFunc">A function that provides values based on grid coordinates.</param>
    /// <returns>The loaded and normalized YxDataGrid.</returns>
    public static DataGrid<float> LoadNormalized(this DataGrid<float> grid, Func<int, int, float> valueFunc) {
        var min = float.MaxValue;
        var max = float.MinValue;
        foreach (var cell in grid) {
            var value = valueFunc.Invoke(cell.Position.X, cell.Position.Y);
            grid.SetValue(cell.Position, value);
            min = Math.Min(min, value);
            max = Math.Max(max, value);
        }
        var range = max - min;
        grid.Transform(value => (value - min) / range);
        return grid;
    }
}