using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Represents an edge in the maze graph, connecting two nodes with a specific direction.
/// </summary>
/// <param name="from">The source node of the edge.</param>
/// <param name="to">The destination node of the edge.</param>
/// <param name="direction">The direction from the source to the destination node.</param>
public class MazeNodeEdge(MazeNode from, MazeNode to, Vector2I direction) {
    public MazeNode From { get; } = from ?? throw new ArgumentNullException(nameof(from));
    public MazeNode To { get; } = to ?? throw new ArgumentNullException(nameof(to));
    public Vector2I Direction { get; } = direction;
    public object? Metadata { get; set; }
}

/// <summary>
/// Represents a node in the maze graph, containing connections to adjacent nodes.
/// </summary>
public class MazeNode {
    private readonly Vector2I _position;

    /// <summary>
    /// Represents a node in the maze graph, containing connections to adjacent nodes.
    /// </summary>
    internal MazeNode(MazeGraph mazeGraph, int id, Vector2I position) {
        _position = position;
        MazeGraph = mazeGraph;
        Id = id;
        Position = position;
    }

    public MazeGraph MazeGraph { get; }
    public int Id { get; }
    public Vector2I Position { get; }
    public MazeNode? Parent { get; set; }
    public MazeNodeEdge? Up { get; private set; }
    public MazeNodeEdge? Right { get; private set; }
    public MazeNodeEdge? Down { get; private set; }
    public MazeNodeEdge? Left { get; private set; }
    public object? Metadata { get; set; }

    /// <summary>
    /// Creates a connection between this node and another node in the specified direction.
    /// </summary>
    /// <param name="direction">The direction of the connection.</param>
    /// <param name="other">The node to connect to.</param>
    /// <returns>The created edge.</returns>
    /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
    public MazeNodeEdge SetEdge(Vector2I direction, MazeNode other) {
        ArgumentNullException.ThrowIfNull(other);
        if (other.Position != Position + direction) throw new ArgumentException("Invalid MazeNode. The position is not in the direction", nameof(other));

        var edge = new MazeNodeEdge(this, other, direction);

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

    public bool HasEdgeTo(Vector2I direction, MazeNode other) {
        if (direction == Vector2I.Up) return Up != null && Up.To == other;
        if (direction == Vector2I.Right) return Right != null && Right.To == other;
        if (direction == Vector2I.Down) return Down != null && Down.To == other;
        if (direction == Vector2I.Left) return Left != null && Left.To == other;
        return false;
    }

    public MazeNodeEdge? GetEdgeTo(MazeNode to) {
        return GetEdges().FirstOrDefault(edge => edge.To == to);
    }

    public IEnumerable<MazeNodeEdge> GetEdges() {
        return new[] { Up, Right, Down, Left }.Where(e => e != null)!;
    }

    /// <summary>
    /// Gets the edge in the specified direction, if it exists.
    /// </summary>
    public MazeNodeEdge? GetEdge(Vector2I direction) {
        if (direction == Vector2I.Up) return Up;
        if (direction == Vector2I.Right) return Right;
        if (direction == Vector2I.Down) return Down;
        if (direction == Vector2I.Left) return Left;
        return null;
    }

    /// <summary>
    /// Returns the four MazeNode around this node (up, down, left, right), no matter if they are connected or not.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MazeNode> GetNeighbors() {
        return Array2D.Directions.Select(dir => _position + dir)
            .Where(pos => MazeGraph.IsValid(pos))
            .Select(pos => MazeGraph.GetNode(pos))
            .Where(node => node != null);
    }

    public void Remove() {
        MazeGraph.Nodes.Remove(Id);
        MazeGraph.NodeGrid[Position] = null!;
        foreach (var node in MazeGraph.Nodes.Values) {
            var edgeToMe = node.GetEdgeTo(this);
            if (edgeToMe != null) {
                node.RemoveEdge(edgeToMe.Direction);
            }
            if (node.Parent == this) node.Parent = null;
        }
        Parent = null;
        Up = Right = Down = Left = null;
    }

    public Vector2I? GetDirectionToParent() {
        if (Parent == null) return null;
        return Position - Parent.Position;
    }

    public IEnumerable<MazeNode> GetChildren() {
        return MazeGraph.Nodes.Values.Where(n => n.Parent == this);
    }

    public IEnumerable<MazeNodeEdge> GetEdgesToChildren() {
        return GetEdges().Where(e => e.To.Parent == this);
    }

    public List<MazeNode> GetPathToRoot() {
        var path = new List<MazeNode>();
        var current = this;
        while (current != null) {
            path.Add(current);
            current = current.Parent;
        }
        return path;
    }
}

/// <summary>
/// Represents a maze as a graph structure with nodes and edges in a 2d grid.
/// </summary>
public class MazeGraph {
    public int Width { get; }
    public int Height { get; }
    public Array2D<MazeNode> NodeGrid { get; }
    public Dictionary<int, MazeNode> Nodes { get; } = [];
    public Func<Vector2I, bool> IsValid { get; }

    public event Action<MazeNodeEdge>? OnConnect;
    public event Action<MazeNode>? OnCreateNode;

    /// <summary>
    /// Initializes a new instance of the MazeGraph class.
    /// </summary>
    /// <param name="width">The width of the maze.</param>
    /// <param name="height">The height of the maze.</param>
    /// <param name="isValid">Optional function to determine if a position is valid for node creation.</param>
    /// <param name="onCreateNode">Optional action to execute when nodes are created.</param>
    /// <param name="onConnect">Optional action to execute when nodes are connected.</param>
    /// <exception cref="ArgumentException">Thrown when width or height are not positive.</exception>
    public MazeGraph(int width, int height, Func<Vector2I, bool>? isValid = null, Action<MazeNode>? onCreateNode = null, Action<MazeNodeEdge>? onConnect = null) {
        Width = width;
        Height = height;
        NodeGrid = new Array2D<MazeNode>(width, height);
        IsValid = isValid ?? (_ => true);
        OnCreateNode = onCreateNode;
        OnConnect = onConnect;
    }

    private int _lastId = 0;

    public MazeNode GetOrCreateNode(Vector2I position) {
        if (!IsValid(position)) throw new ArgumentException("Invalid position", nameof(position));
        var node = NodeGrid[position];
        if (node != null) return node;

        node = new MazeNode(this, _lastId++, position);
        NodeGrid[position] = node;
        Nodes[node.Id] = node;
        OnCreateNode?.Invoke(node);
        return node;
    }

    /// <summary>
    /// Gets an existing node at the specified position.
    /// </summary>
    public MazeNode? GetNode(Vector2I position) => NodeGrid[position];

    /// <summary>
    /// Connect a Node to the one (if exists) in the specified direction.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="direction"></param>
    /// <param name="twoWays"></param>
    public void ConnectNode(MazeNode from, Vector2I direction, bool twoWays) {
        var to = GetNode(from.Position + direction);
        if (to == null) return;
        _ConnectNode(from, direction, to);
        if (twoWays) _ConnectNode(to, direction.Inverse(), from);
        
    }
        
    private void _ConnectNode(MazeNode from, Vector2I direction, MazeNode to) {
        if (from.HasEdgeTo(direction, to)) return;
        var edge = from.SetEdge(direction, to);
        OnConnect?.Invoke(edge);
    }

    /// <summary>
    /// Grows a maze from a starting position using the specified constraints.
    /// </summary>
    /// <param name="start">Starting position for the maze generation.</param>
    /// <param name="constraints">Constraints for the maze generation.</param>
    /// <returns>The number of paths created.</returns>
    public void Grow(Vector2I start, MazeConstraints constraints) {
        ArgumentNullException.ThrowIfNull(constraints);
        if (!IsValid(start)) throw new ArgumentException("Invalid start position", nameof(start));
        var maxTotalCells = constraints.MaxTotalCells == -1 ? int.MaxValue : constraints.MaxTotalCells;
        var maxCellsPerPath = constraints.MaxCellsPerPath == -1 ? int.MaxValue : constraints.MaxCellsPerPath;
        var maxTotalPaths = constraints.MaxPaths == -1 ? int.MaxValue : constraints.MaxPaths;
        if (maxTotalCells == 0 || maxCellsPerPath == 0 || maxTotalPaths == 0) return;

        var usedNodes = new Stack<Vector2I>();
        Vector2I? lastDirection = null;

        var pathsCreated = 0;
        var totalNodesCreated = 1;
        var nodesCreatedInCurrentPath = 1;

        GetOrCreateNode(start);
        usedNodes.Push(start);
        while (usedNodes.Count > 0) {
            var currentPos = usedNodes.Peek();
            var currentNode = GetNode(currentPos)!;

            var availableDirections = GetAvailableDirections(currentPos);

            if (availableDirections.Count == 0 || nodesCreatedInCurrentPath >= maxCellsPerPath || totalNodesCreated == maxTotalCells) {
                // stop carving, backtracking
                usedNodes.Pop();
                if (usedNodes.Count > 0) {
                    var nextCell = usedNodes.Peek();
                    if (GetAvailableDirections(nextCell).Count == 0) {
                        continue;
                    }
                }
                pathsCreated++;
                Console.WriteLine($"Path #{pathsCreated} finished: Cells: {nodesCreatedInCurrentPath}");
                if (pathsCreated == maxTotalPaths || totalNodesCreated == maxTotalCells) break;
                nodesCreatedInCurrentPath = 1;
                lastDirection = null;
                continue;
            }

            var validCurrentDir = lastDirection.HasValue && availableDirections.Contains(lastDirection.Value)
                ? lastDirection
                : null;

            var nextDir = constraints.DirectionSelector(validCurrentDir, availableDirections);
            lastDirection = nextDir;

            var nextPos = currentPos + nextDir;
            var nextNode = GetOrCreateNode(nextPos);
            nextNode.Parent = currentNode;
            ConnectNode(currentNode, nextDir, true);
            usedNodes.Push(nextPos);
            totalNodesCreated++;
            nodesCreatedInCurrentPath++;
        }
        Console.WriteLine("Cells created: " + totalNodesCreated + " Paths created: " + pathsCreated);
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