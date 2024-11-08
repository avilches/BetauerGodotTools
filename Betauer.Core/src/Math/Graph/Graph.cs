using System.Collections.Generic;

namespace Betauer.Core.Math.Graph;

public class Graph<T> where T : notnull {
    public Dictionary<T, List<T>> Data { get; } = new();

    public void Connect(T from, T to) {
        if (Data.TryGetValue(from, out var list)) {
            if (!list.Contains(to)) {
                list.Add(to);
            }
        } else {
            Data[from] = new List<T> { to };
        }
    }

    public List<T> GetConnections(T point) {
        return Data.TryGetValue(point, out var points) ? points : new List<T>(0);
    }
}
