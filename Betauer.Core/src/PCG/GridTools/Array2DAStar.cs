using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.PCG.GridTools;

public class Array2DAStar<T> {
    private readonly Dictionary<Vector2I, float> _gScore = new();
    private readonly Dictionary<Vector2I, Array2DEdge?> _edgeTo = new();
    private readonly IndexMinPriorityQueue<float> _pq;
    private readonly Array2DGraph<T> _graph;
    private readonly int _width;

    private Vector2I _goal;
    private Func<Vector2I, Vector2I, float> _heuristic;

    public Array2DAStar(Array2DGraph<T> graph) {
        ArgumentNullException.ThrowIfNull(graph);
        _graph = graph;
        _width = graph.Width;
        _pq = new IndexMinPriorityQueue<float>(graph.Width * graph.Height);
    }

    private void Reset() {
        _gScore.Clear();
        _edgeTo.Clear();
        _pq.Clear();
    }

    public List<Vector2I> FindPath(Vector2I start, Vector2I goal, 
        Func<Vector2I, Vector2I, float>? heuristic = null,
        Action<Vector2I>? onNodeVisited = null) {
        
        if (!_graph.IsWalkablePosition(start)) throw new ArgumentException("Start position is not walkable", nameof(start));
        if (!_graph.IsWalkablePosition(goal)) throw new ArgumentException("Goal position is not walkable", nameof(goal));
        
        Reset();
        
        _goal = goal;
        _heuristic = heuristic ?? Heuristics.Manhattan;

        // Initialize start
        _gScore[start] = 0;
        _edgeTo[start] = null;
        _pq.Insert(ToIndex(start), GetFScore(start));

        while (!_pq.IsEmpty()) {
            var index = _pq.DeleteMin();
            var current = ToVector2I(index);
            
            onNodeVisited?.Invoke(current);
            
            if (current == goal) break;
            
            foreach (var edge in _graph.Adjacent(current)) {
                Relax(edge);
            }
        }

        return GetPath();
    }

    private int ToIndex(Vector2I pos) => pos.Y * _width + pos.X;

    private Vector2I ToVector2I(int index) => new(index % _width, index / _width);

    private float GetHScore(Vector2I pos) {
        return _heuristic(pos, _goal);
    }

    private float GetGScore(Vector2I pos) {
        return _gScore.GetValueOrDefault(pos, float.PositiveInfinity);
    }

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

    private List<Vector2I> GetPath() {
        if (!_edgeTo.ContainsKey(_goal)) return [];

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

    public float GetPathCost() => GetGScore(_goal);

    /// <summary>
    /// Static method to find a path between two positions in a grid using the specified heuristic.
    /// This method creates a new instance of Array2DAStar for compatibility with existing code.
    /// For better performance when doing multiple path searches, create an instance and reuse it.
    /// </summary>
    public static List<Vector2I> FindPath(Array2DGraph<T> graph, Vector2I start, Vector2I goal, 
        Func<Vector2I, Vector2I, float>? heuristic = null, Action<Vector2I>? onNodeVisited = null) {
        return new Array2DAStar<T>(graph).FindPath(start, goal, heuristic, onNodeVisited);
    }
}