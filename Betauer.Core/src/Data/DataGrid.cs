using System;

namespace Betauer.Core.Data;

public class DataGrid<T> : IDataGrid<T> {
    public T[,] Data { get; }

    public DataGrid(int width, int height, IDataGrid<T> valueFunc) : this(width, height, valueFunc.GetValue) {
    }

    public DataGrid(int width, int height, Func<int, int, T> valueFunc) {
        Data = new T[width, height];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                var value = valueFunc(x, y);
                Data[x, y] = value;
            }
        }
    }

    public DataGrid(T[,] data) {
        Data = data;
    }

    public T GetValue(int x, int y) => Data[x, y];
}