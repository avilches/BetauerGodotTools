using System.Collections.Generic;
using Godot;

namespace Betauer.Core.Data;

public class Graph {
    private readonly Dictionary<Vector2, List<Vector2>> _adjacencyList = new();

    public void Connect(Vector2 from, Vector2 to) {
        if (_adjacencyList.TryGetValue(from, out var list)) {
            if (!list.Contains(to)) {
                list.Add(to);
            }
        } else {
            _adjacencyList[from] = new List<Vector2> { to };
        }
    }

    public List<Vector2> GetConnections(Vector2 point) {
        return _adjacencyList.TryGetValue(point, out var points) ? points : new List<Vector2>(0);
    }
}