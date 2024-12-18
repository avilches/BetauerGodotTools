using System;
using System.Collections.Generic;
using Betauer.Core.DataMath;
using Godot;

namespace Betauer.Core.PCG.GridTools;

/// <summary>
/// Array2DAstar implements the A* pathfinding algorithm for finding shortest paths in a grid-based graph.
/// It uses Manhattan distance as heuristic and supports weighted movements costs.
/// </summary>
public class Array2DAStar<T> {
    private readonly Dictionary<Vector2I, float> _gScore = new();
    private readonly Dictionary<Vector2I, Array2DEdge?> _edgeTo = new();
    private readonly IndexMinPriorityQueue<float> _pq;
    private readonly Array2DGraph<T> _graph;
    private readonly int _width;
    private readonly Vector2I _goal;
    private readonly Func<Vector2I, Vector2I, float> _heuristic;
    private readonly Action<Vector2I> _onNodeVisited;

    public Array2DAStar(Array2DGraph<T> graph, Vector2I start, Vector2I goal, 
        Func<Vector2I, Vector2I, float>? heuristic = null, Action<Vector2I>? onNodeVisited = null) {
        
        ArgumentNullException.ThrowIfNull(graph);
        if (!graph.IsWalkablePosition(start)) throw new ArgumentException("Start position is not walkable", nameof(start));
        if (!graph.IsWalkablePosition(goal)) throw new ArgumentException("Goal position is not walkable", nameof(goal));
        _onNodeVisited = onNodeVisited;

        _graph = graph;
        _width = graph.Width;
        _goal = goal;
        _heuristic = heuristic ?? Heuristics.Manhattan;
        _pq = new IndexMinPriorityQueue<float>(graph.Width * graph.Height);

        // Initialize start
        _gScore[start] = 0;
        _edgeTo[start] = null;
        _pq.Insert(ToIndex(start), GetFScore(start));

        while (!_pq.IsEmpty()) {
            var index = _pq.DeleteMin();
            var current = ToVector2I(index);
            
            _onNodeVisited?.Invoke(current); // Registrar nodo visitado
            
            if (current == goal) break;
            
            foreach (var edge in _graph.Adjacent(current)) {
                Relax(edge);
            }
        }
    }

    private int ToIndex(Vector2I pos) => pos.Y * _width + pos.X;

    private Vector2I ToVector2I(int index) => new(index % _width, index / _width);

    /// <summary>
    /// Manhattan distance heuristic, scaled to not dominate over weights
    /// </summary>
    private float GetHScore(Vector2I pos) {
        // La heurística debe ser una subestimación admisible del coste real
        // Usamos el peso mínimo (1.0f) para escalar la distancia Manhattan
        return _heuristic(pos, _goal);
    }

    /// <summary>
    /// Gets the real cost from start to the position
    /// </summary>
    private float GetGScore(Vector2I pos) {
        return _gScore.GetValueOrDefault(pos, float.PositiveInfinity);
    }

    /// <summary>
    /// Gets f(n) = g(n) + h(n)
    /// Where g(n) is the real cost from start to n
    /// and h(n) is the estimated cost from n to goal
    /// </summary>
    private float GetFScore(Vector2I pos) {
        return GetGScore(pos) + GetHScore(pos);
    }

    private void Relax(Array2DEdge edge) {
        var to = edge.To;
        var newGScore = GetGScore(edge.From) + edge.Weight;
        var toIndex = ToIndex(to);

        if (!_gScore.ContainsKey(to) || newGScore < GetGScore(to)) {
            _gScore[to] = newGScore;
            _edgeTo[to] = edge;
            
            var fScore = newGScore + GetHScore(to);
            if (_pq.Contains(toIndex)) {
                _pq.DecreaseKey(toIndex, fScore);
            } else {
                _pq.Insert(toIndex, fScore);
            }
        }
    }
    

    /// <summary>
    /// Returns the sequence of positions representing the shortest path from start to goal
    /// </summary>
    /// <returns>List of positions representing the path, or null if no path exists</returns>
    public List<Vector2I> GetPath() {
        if (!_edgeTo.ContainsKey(_goal)) return null;

        var path = new List<Vector2I>();
        var current = _goal;
        path.Add(current);

        while (_edgeTo.TryGetValue(current, out var edge) && edge.HasValue) {
            current = edge.Value.From;
            path.Add(current);
        }

        path.Reverse();
        return path;
    }

    /// <summary>
    /// Returns the total cost of the path from start to goal
    /// </summary>
    /// <returns>The total cost, or float.PositiveInfinity if no path exists</returns>
    public float GetPathCost() => GetGScore(_goal);


    /// <summary>
    /// Static method to find a path between two positions in a grid using the specified heuristic
    /// </summary>
    public static List<Vector2I> FindPath(Array2DGraph<T> graph, Vector2I start, Vector2I goal, 
        Func<Vector2I, Vector2I, float>? heuristic = null, Action<Vector2I>? onNodeVisited = null) {
        var astar = new Array2DAStar<T>(graph, start, goal, heuristic, onNodeVisited);
        return astar.GetPath();
    }
}