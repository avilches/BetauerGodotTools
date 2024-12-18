using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Betauer.Core.DataMath;
using Godot;

namespace Betauer.Core.PCG.GridTools;

/// <summary>
/// The Array2DEdge struct represents a weighted edge in an edge-weighted grid graph. 
/// </summary>
public readonly record struct Array2DEdge(Vector2I From, Vector2I To, float Weight) {
    public override string ToString() {
        return $"From: {From}, To: {To}, Weight: {Weight}";
    }
}

/// <summary>
/// The Array2DGraph class represents an edge-weighted directed graph based on a grid where vertices are Vector2I coordinates.
/// The graph structure is implicit in the grid, where each cell can be connected to its orthogonal neighbors if they are walkable.
/// Edge weights are determined by the cost function provided.
/// </summary>
public class Array2DGraph<T> {
    public Array2D<T> Array2D { get; }
    private readonly Func<T, bool> _isWalkable;
    private readonly Func<T, float> _getWeight;
    
    public int Width => Array2D.Width;
    public int Height => Array2D.Height;

    /// <summary>
    /// Constructs a grid graph from an Array2D with walkability and weight functions
    /// </summary>
    /// <param name="array2D">The grid that defines the graph structure</param>
    /// <param name="isWalkable">Function that determines if a cell is walkable</param>
    /// <param name="getWeight">Function that determines the movement cost for a cell (must be >= 1)</param>
    public Array2DGraph(Array2D<T> array2D, Func<T, bool> isWalkable, Func<T, float> getWeight) {
        ArgumentNullException.ThrowIfNull(isWalkable);
        ArgumentNullException.ThrowIfNull(getWeight);

        Array2D = array2D ?? throw new ArgumentNullException(nameof(array2D));
        _isWalkable = isWalkable;
        _getWeight = getWeight;
    }

    /// <summary>
    /// Returns an IEnumerable of the Array2DEdges incident from the specified vertex
    /// The edges are computed on-demand based on the walkable orthogonal neighbors
    /// </summary>
    /// <param name="vertex">The vertex to find incident Array2DEdges from</param>
    /// <returns>IEnumerable of the Array2DEdges incident from the specified vertex</returns>
    public IEnumerable<Array2DEdge> Adjacent(Vector2I vertex) {
        if (!IsWalkablePosition(vertex)) yield break;
        
        foreach (var neighbor in Array2D.GetOrtogonalPositions(vertex)) {
            if (IsWalkablePosition(neighbor)) {
                // El peso de moverse a una celda es el peso de la celda destino
                var weight = _getWeight(Array2D[neighbor]);
                yield return new Array2DEdge(vertex, neighbor, weight);
            }
        }
    }


    /// <summary>
    /// Returns an IEnumerable of all directed edges in the edge-weighted grid graph
    /// The edges are computed on-demand based on the walkable cells and their walkable neighbors
    /// </summary>
    /// <returns>IEnumerable of all directed edges in the edge-weighted grid graph</returns>
    public IEnumerable<Array2DEdge> Edges() {
        for (var y = 0; y < Array2D.Height; y++) {
            for (var x = 0; x < Array2D.Width; x++) {
                var pos = new Vector2I(x, y);
                if (IsWalkablePosition(pos)) {
                    foreach (var edge in Adjacent(pos)) {
                        yield return edge;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Returns the number of directed edges incident from the specified vertex
    /// This is known as the out-degree of the vertex
    /// </summary>
    /// <param name="vertex">The vertex to find the out-degree of</param>
    /// <returns>The number of directed edges incident from the specified vertex</returns>
    public int OutDegree(Vector2I vertex) {
        return IsWalkablePosition(vertex)
            ? Array2D.GetOrtogonalPositions(vertex).Count(IsWalkablePosition)
            : 0;
    }

    /// <summary>
    /// Returns whether the specified position is valid and walkable in the grid
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <returns>True if the position is valid and walkable, false otherwise</returns>
    public bool IsWalkablePosition(Vector2I pos) {
        return Array2D.IsValidPosition(pos) && _isWalkable(Array2D[pos]);
        
    }
    
    public List<Vector2I> FindPath(Vector2I start, Vector2I end,
        Func<Vector2I, Vector2I, float>? heuristic = null, Action<Vector2I>? onNodeVisited = null) {
        if (!IsWalkablePosition(start) || !IsWalkablePosition(end)) return null;
        return Array2DAStar<T>.FindPath(this, start, end, heuristic, onNodeVisited);
    }


    /// <summary>
    /// Returns a string that represents the current edge-weighted grid graph
    /// </summary>
    /// <returns>
    /// A string that represents the current edge-weighted grid graph
    /// </returns>
    public override string ToString() {
        var formattedString = new StringBuilder();
        formattedString.AppendLine($"Grid size: {Array2D.Width}x{Array2D.Height}");

        for (var y = 0; y < Array2D.Height; y++) {
            for (var x = 0; x < Array2D.Width; x++) {
                var pos = new Vector2I(x, y);
                if (IsWalkablePosition(pos)) {
                    formattedString.Append($"{pos}:");
                    foreach (var edge in Adjacent(pos)) {
                        formattedString.Append($" {edge.To}");
                    }
                    formattedString.AppendLine();
                }
            }
        }
        return formattedString.ToString();
    }
}