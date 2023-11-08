using System;

namespace Betauer.Core.Data;

public class VirtualDataGrid<T> : IDataGrid<T> {
    public Func<int, int, T> ValueFunc { get; }
    
    public VirtualDataGrid(Func<int, int, T> value) {
        ValueFunc = value;
    }
    
    public T GetValue(int x, int y) => ValueFunc(x, y);
}