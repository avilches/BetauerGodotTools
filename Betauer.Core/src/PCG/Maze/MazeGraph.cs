using System;
using System.Collections.Generic;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Represents a maze as a graph structure with nodes and edges in a 2d grid.
/// </summary>
public partial class MazeGraph {
    public int Width { get; }
    public int Height { get; }
    public Array2D<NodeGrid> NodeGrid { get; }
    public Dictionary<int, NodeGrid> Nodes { get; } = [];
    public NodeGrid NodeGridRoot { get; private set; }
    public Func<Vector2I, bool> IsValid { get; set;  }

    public event Action<NodeGridEdge>? OnConnect;
    public event Action<NodeGrid>? OnCreateNode;

    /// <summary>
    /// Initializes a new instance of the MazeGraph class.
    /// </summary>
    /// <param name="width">The width of the maze.</param>
    /// <param name="height">The height of the maze.</param>
    /// <param name="isValid">Optional function to determine if a position is valid for node creation.</param>
    /// <param name="onCreateNode">Optional action to execute when nodes are created.</param>
    /// <param name="onConnect">Optional action to execute when nodes are connected.</param>
    /// <exception cref="ArgumentException">Thrown when width or height are not positive.</exception>
    public MazeGraph(int width, int height, Func<Vector2I, bool>? isValid = null, Action<NodeGrid>? onCreateNode = null, Action<NodeGridEdge>? onConnect = null) {
        Width = width;
        Height = height;
        NodeGrid = new Array2D<NodeGrid>(width, height);
        IsValid = isValid ?? (_ => true);
        OnCreateNode = onCreateNode;
        OnConnect = onConnect;
    }

    private int _lastId = 0;

    public NodeGrid GetOrCreateNode(Vector2I position) {
        if (!IsValid(position)) throw new ArgumentException("Invalid position", nameof(position));
        var node = NodeGrid[position];
        if (node != null) return node;

        node = new NodeGrid(this, _lastId++, position);
        NodeGrid[position] = node;
        Nodes[node.Id] = node;
        OnCreateNode?.Invoke(node);
        return node;
    }

    /// <summary>
    /// Gets an existing node at the specified position.
    /// </summary>
    public NodeGrid? GetNode(Vector2I position) => NodeGrid[position];

    /// <summary>
    /// Connect a Node to the one (if exists) in the specified direction.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="direction"></param>
    /// <param name="twoWays"></param>
    public void ConnectNode(NodeGrid from, Vector2I direction, bool twoWays) {
        var to = GetNode(from.Position + direction);
        if (to == null) return;
        _ConnectNode(from, direction, to);
        if (twoWays) _ConnectNode(to, direction.Inverse(), from);
    }

    private void _ConnectNode(NodeGrid from, Vector2I direction, NodeGrid to) {
        if (from.HasEdgeTo(direction, to)) return;
        var edge = from.SetEdge(direction, to);
        OnConnect?.Invoke(edge);
    }
    
    private readonly List<Vector2I> _availableDirections = new(4);

    private List<Vector2I> GetAvailableDirections(Vector2I pos) {
        _availableDirections.Clear();
        foreach (var dir in Array2D.Directions) {
            var target = pos + dir;
            if (Geometry.IsPointInRectangle(target.X, target.Y, 0, 0, Width, Height) &&
                IsValid(target) &&
                GetNode(target) == null) {
                _availableDirections.Add(dir);
            }
        }
        return _availableDirections;
    }

    /// <summary>
    /// Creates a new MazeGraph with the specified dimensions.
    /// </summary>
    public static MazeGraph Create(int width, int height) {
        return new MazeGraph(width, height);
    }

    /// <summary>
    /// Creates a MazeGraph from a boolean template array.
    /// In the template, true values represent valid positions for nodes. false values are forbidden (invalid)
    /// </summary>
    public static MazeGraph Create(bool[,] template) {
        ArgumentNullException.ThrowIfNull(template);
        return new MazeGraph(template.GetLength(1), template.GetLength(0),
            pos => template[pos.Y, pos.X]);
    }

    /// <summary>
    /// Creates a MazeGraph from a boolean Array2D template.
    /// In the template, true values represent valid positions for nodes. false values are forbidden (invalid)
    /// </summary>
    public static MazeGraph Create(Array2D<bool> template) {
        ArgumentNullException.ThrowIfNull(template);
        return new MazeGraph(template.Width, template.Height, pos => template[pos]);
    }

    /// <summary>
    /// Creates a MazeGraph from a BitArray2D template.
    /// In the template, true values represent valid positions for nodes. false values are forbidden (invalid)
    /// </summary>
    public static MazeGraph Create(BitArray2D template) {
        ArgumentNullException.ThrowIfNull(template);
        return new MazeGraph(template.Width, template.Height, pos => template[pos]);
    }
}