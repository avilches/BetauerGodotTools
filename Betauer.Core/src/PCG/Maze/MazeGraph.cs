using System;
using System.Collections.Generic;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Represents an edge in the maze graph, connecting two nodes with a specific direction.
/// </summary>
/// <param name="From">The source node of the edge.</param>
/// <param name="To">The destination node of the edge.</param>
/// <param name="Direction">The direction from the source to the destination node.</param>
public class Edge {
    public MazeNode From { get; }
    public MazeNode To { get; }
    public Vector2I Direction { get; }
    public object? Metadata { get; set; }

    public Edge(MazeNode from, MazeNode to, Vector2I direction) {
        From = from ?? throw new ArgumentNullException(nameof(from));
        To = to ?? throw new ArgumentNullException(nameof(to));
        Direction = direction;
    }
}

/// <summary>
/// Represents a node in the maze graph, containing connections to adjacent nodes.
/// </summary>
public class MazeNode {
    public Vector2I Position { get; }
    public Edge? Up { get; private set; }
    public Edge? Right { get; private set; }
    public Edge? Down { get; private set; }
    public Edge? Left { get; private set; }
    public object? Metadata { get; set; }

    public MazeNode(Vector2I position) {
        Position = position;
    }

    /// <summary>
    /// Creates a connection between this node and another node in the specified direction.
    /// </summary>
    /// <param name="other">The node to connect to.</param>
    /// <param name="direction">The direction of the connection.</param>
    /// <returns>The created edge.</returns>
    /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
    public Edge Connect(MazeNode other, Vector2I direction) {
        if (other == null) throw new ArgumentNullException(nameof(other));

        var edge = new Edge(this, other, direction);

        if (direction == Vector2I.Up) Up = edge;
        else if (direction == Vector2I.Right) Right = edge;
        else if (direction == Vector2I.Down) Down = edge;
        else if (direction == Vector2I.Left) Left = edge;

        return edge;
    }

    /// <summary>
    /// Removes the connection in the specified direction.
    /// </summary>
    public void RemoveEdge(Vector2I direction) {
        if (direction == Vector2I.Up) Up = null;
        else if (direction == Vector2I.Right) Right = null;
        else if (direction == Vector2I.Down) Down = null;
        else if (direction == Vector2I.Left) Left = null;
    }

    /// <summary>
    /// Checks if there is a connection in the specified direction.
    /// </summary>
    public bool HasEdge(Vector2I direction) {
        if (direction == Vector2I.Up) return Up != null;
        if (direction == Vector2I.Right) return Right != null;
        if (direction == Vector2I.Down) return Down != null;
        if (direction == Vector2I.Left) return Left != null;
        return false;
    }

    /// <summary>
    /// Gets the edge in the specified direction, if it exists.
    /// </summary>
    public Edge? GetEdge(Vector2I direction) {
        if (direction == Vector2I.Up) return Up;
        if (direction == Vector2I.Right) return Right;
        if (direction == Vector2I.Down) return Down;
        if (direction == Vector2I.Left) return Left;
        return null;
    }
}

/// <summary>
/// Represents a maze as a graph structure with nodes and edges.
/// </summary>
public class MazeGraph {
    public int Width { get; }
    public int Height { get; }
    public Array2D<MazeNode> Nodes { get; }
    public Func<Vector2I, bool> IsValid { get; }

    public event Action<Edge>? OnConnect;
    public event Action<MazeNode>? OnCreateNode;

    /// <summary>
    /// Initializes a new instance of the MazeGraph class.
    /// </summary>
    /// <param name="width">The width of the maze.</param>
    /// <param name="height">The height of the maze.</param>
    /// <param name="isValid">Optional function to determine if a position is valid for node creation.</param>
    /// <param name="onConnect">Optional action to execute when nodes are connected.</param>
    /// <exception cref="ArgumentException">Thrown when width or height are not positive.</exception>
    public MazeGraph(int width, int height, Func<Vector2I, bool>? isValid = null, Action<Edge>? onConnect = null) {
        if (width <= 0) throw new ArgumentException("Width must be positive", nameof(width));
        if (height <= 0) throw new ArgumentException("Height must be positive", nameof(height));

        Width = width;
        Height = height;
        Nodes = new Array2D<MazeNode>(width, height);
        IsValid = isValid ?? (_ => true);
        OnConnect = onConnect;
    }

    private MazeNode GetOrCreateNode(Vector2I position) {
        var node = Nodes[position];
        if (node != null) return node;

        node = new MazeNode(position);
        Nodes[position] = node;
        OnCreateNode?.Invoke(node);
        return node;
    }

    /// <summary>
    /// Gets an existing node at the specified position.
    /// </summary>
    public MazeNode? GetNode(Vector2I position) => Nodes[position];

    private void ConnectNodes(MazeNode from, Vector2I direction, MazeNode to) {
        var edge = from.Connect(to, direction);
        OnConnect?.Invoke(edge);
        var edgeBack = to.Connect(from, direction.Inverse());
        OnConnect?.Invoke(edgeBack);
    }

    /// <summary>
    /// Grows a maze from a starting position using the specified constraints.
    /// </summary>
    /// <param name="start">Starting position for the maze generation.</param>
    /// <param name="constraints">Constraints for the maze generation.</param>
    /// <returns>The number of paths created.</returns>
    /// <exception cref="ArgumentException">Thrown when start position is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when constraints is null.</exception>
    public int Grow(Vector2I start, MazeConstraints constraints) {
        ArgumentNullException.ThrowIfNull(constraints);
        if (!IsValid(start)) throw new ArgumentException("Invalid start position", nameof(start));
        var constraintsMaxTotalCells = constraints.MaxTotalCells == -1 ? int.MaxValue : constraints.MaxTotalCells;
        var constraintsMaxCellsPerPath = constraints.MaxCellsPerPath == -1 ? int.MaxValue : constraints.MaxCellsPerPath;
        var constraintsMaxPaths = constraints.MaxPaths == -1 ? int.MaxValue : constraints.MaxPaths;
        if (constraintsMaxTotalCells == 0 || constraintsMaxCellsPerPath == 0 || constraintsMaxPaths == 0) return 0;

        var usedNodes = new Stack<Vector2I>();
        Vector2I? lastDirection = null;
        
        
        var pathsCreated = 0;
        var nodesCreated = 1;
        var nodesInCurrentPath = 1;
        
        usedNodes.Push(start);
        while (usedNodes.Count > 0 && nodesCreated <= constraintsMaxTotalCells) {
            var currentPos = usedNodes.Peek();
            var currentNode = GetNode(currentPos)!;

            var availableDirections = GetAvailableDirections(currentPos);

            if (availableDirections.Count == 0 || nodesInCurrentPath >= constraintsMaxCellsPerPath) {
                if (availableDirections.Count != 0) {
                    if (++pathsCreated >= constraintsMaxPaths) break;
                }
                usedNodes.Pop();
                lastDirection = null;
                nodesInCurrentPath = 0;
                continue;
            }

            var validCurrentDir = lastDirection.HasValue && availableDirections.Contains(lastDirection.Value)
                ? lastDirection
                : null;

            var nextDir = constraints.DirectionSelector(validCurrentDir, availableDirections);
            lastDirection = nextDir;

            var nextPos = currentPos + nextDir;
            var nextNode = GetOrCreateNode(nextPos);
            ConnectNodes(currentNode, nextDir, nextNode);

            usedNodes.Push(nextPos);
            nodesCreated++;
            nodesInCurrentPath++;
        }
        Console.WriteLine("Cells created: " + nodesCreated+" Paths created: "+pathsCreated);

        return nodesCreated;
    }

    private readonly List<Vector2I> _availableDirections = new(4);

    private List<Vector2I> GetAvailableDirections(Vector2I currentPos) {
        _availableDirections.Clear();
        foreach (var dir in Array2D.Directions) {
            var target = currentPos + dir;
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
    /// </summary>
    public static MazeGraph Create(bool[,] template) {
        ArgumentNullException.ThrowIfNull(template);
        return new MazeGraph(template.GetLength(1), template.GetLength(0),
            pos => template[pos.Y, pos.X]);
    }

    /// <summary>
    /// Creates a MazeGraph from a boolean Array2D template.
    /// </summary>
    public static MazeGraph Create(Array2D<bool> template) {
        ArgumentNullException.ThrowIfNull(template);
        return new MazeGraph(template.Width, template.Height, pos => template[pos]);
    }

    /// <summary>
    /// Creates a MazeGraph from a BitArray2D template.
    /// </summary>
    public static MazeGraph Create(BitArray2D template) {
        ArgumentNullException.ThrowIfNull(template);
        return new MazeGraph(template.Width, template.Height, pos => template[pos]);
    }
}