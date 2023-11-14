using System;

namespace Betauer.Core.Data;

public class DataGrid<T> : IDataGrid<T> {
    public int Width { get; }
    public int Height { get; }
    
    public T[,] Data { get; }

    public DataGrid(int width, int height) : this(new T[width, height]) {
    }

    public DataGrid(T[,] data) {
        Data = data;
        Width = Data.GetLength(0);
        Height =  Data.GetLength(1);
    }

    public void Load(Func<int, int, T> valueFunc) {
        Load(0, 0, Width, Height, valueFunc);
    }

    public void SetAll(T value) {
        Set(0, 0, Width, Height, value);
    }

    public void Set(int x, int y, int width, int height, T value) {
        for (var xx = x; xx < width - x; xx++) {
            for (var yy = y; yy < height - y; yy++) {
                Data[xx, yy] = value;
            }
        }
    }

    public void Load(int x, int y, int width, int height, Func<int, int, T> valueFunc) {
        for (var xx = x; xx < width-x; xx++) {
            for (var yy = y; yy < height-y; yy++) {
                Data[xx, yy] = valueFunc.Invoke(xx, yy);
            }
        }
    }

    public T GetValue(int x, int y) => Data[x, y];

    public void SetValue(int x, int y, T value) => Data[x, y] = value;
}