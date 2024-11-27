using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Betauer.Core.PCG.Maze;

public class Edge(MazeNode from, MazeNode to, Vector2I direction) {
    public MazeNode From { get; } = from;
    public MazeNode To { get; } = to;
    public Vector2I Direction { get; } = direction;
    public object? Metadata { get; set; } = null;
}

public class MazeNode(Vector2I position) {
    public Vector2I Position { get; } = position;
    public Edge? Up { get; private set; }
    public Edge? Right { get; private set; }
    public Edge? Down { get; private set; }
    public Edge? Left { get; private set; }
    public object? Metadata { get; set; } = null;

    public Edge Connect(MazeNode other, Vector2I direction) {
        var edge = new Edge(this, other, direction);
        
        if (direction == Vector2I.Up) {
            Up = edge;
        } else if (direction == Vector2I.Right) {
            Right = edge;
        } else if (direction == Vector2I.Down) {
            Down = edge;
        } else if (direction == Vector2I.Left) {
            Left = edge;
        }
        return edge;
    }

    public void RemoveEdge(Vector2I direction) {
        if (direction == Vector2I.Up) {
            Up = null;
        } else if (direction == Vector2I.Right) {
            Right = null;
        } else if (direction == Vector2I.Down) {
            Down = null;
        } else if (direction == Vector2I.Left) {
            Left = null;
        }
    }

    public bool HasEdge(Vector2I direction) {
        return direction switch {
            var d when d == Vector2I.Up => Up != null,
            var d when d == Vector2I.Right => Right != null,
            var d when d == Vector2I.Down => Down != null,
            var d when d == Vector2I.Left => Left != null,
            _ => false
        };
    }

    public Edge? GetEdge(Vector2I direction) {
        return direction switch {
            var d when d == Vector2I.Up => Up,
            var d when d == Vector2I.Right => Right,
            var d when d == Vector2I.Down => Down,
            var d when d == Vector2I.Left => Left,
            _ => null
        };
    }
}

public class MazeGraph(int width, int height, Func<Vector2I, bool>? isValid = null, Action<Edge>? onConnect = null) {
    public int Width { get; } = width;
    public int Height { get; } = height;
    public Array2D<MazeNode> Nodes { get; } = new(width, height);
    public Func<Vector2I, bool> IsValid { get; } = isValid ?? (_ => true);
    public event Action<Edge>? OnConnect = onConnect;
    public event Action<MazeNode>? OnCreateNode;

    private MazeNode GetOrCreateNode(Vector2I position) {
        MazeNode node = Nodes[position];
        if (node != null) return node;
        node = new MazeNode(position);
        Nodes[position] = node;
        OnCreateNode?.Invoke(node);
        return node;
    }

    public MazeNode? GetNode(Vector2I position) => Nodes[position];

    private void ConnectNodes(MazeNode from, Vector2I direction, MazeNode to) {
        var edge = from.Connect(to, direction);
        OnConnect?.Invoke(edge);
        var edgeBack = to.Connect(from, direction.Inverse());
        OnConnect?.Invoke(edgeBack);
    }

    /// <summary>
    /// Generates a maze using the backtracker algorithm with directional bias
    /// </summary>
    /// <param name="start">Starting position for the maze generation</param>
    /// <param name="directionalBias">Value between 0 and 1 that determines how likely the algorithm is to continue in the same direction</param>
    /// <param name="maxPaths">Maximum number of paths to create. If -1, creates as many paths as possible</param>
    /// <param name="rng">Random number generator to use. If null, creates a new one</param>
    /// <returns>The number of paths created</returns>
    public int GrowBacktracker(Vector2I start, float directionalBias, int maxPaths = -1, Random? rng = null) {
        rng ??= new Random();
        return Grow(start, DirectionSelectors.CreateRandom(directionalBias, rng), maxPaths);
    }

    /// <summary>
    /// Grows a maze from a starting position using the specified direction and cell selectors
    /// </summary>
    /// <param name="start">Starting position for the maze generation</param>
    /// <param name="directionSelector">Function that selects the next direction to grow</param>
    /// <param name="cellSelector">Function that selects which cell to grow from next</param>
    /// <param name="maxPaths">Maximum number of paths to create. If -1, creates as many paths as possible</param>
    /// <returns>The number of paths created</returns>
    public int Grow(Vector2I start, Func<Vector2I?, IList<Vector2I>, Vector2I> directionSelector, int maxPaths = -1) {
        if (!IsValid(start)) throw new ArgumentException("Invalid start position");

        var pendingPositions = new Stack<Vector2I>();
        var startNode = GetOrCreateNode(start);
        Vector2I? lastDir = null;
        var pathsCreated = 0;

        pendingPositions.Push(start);

        while (pendingPositions.Count > 0) {
            var currentPos = pendingPositions.Peek();
            var currentNode = GetNode(currentPos)!;

            var availableDirections = Array2D.Directions.Where(dir => {
                var target = currentPos + dir;
                return Geometry.IsPointInRectangle(target.X, target.Y, 0, 0, Width, Height) && 
                       IsValid(target) && 
                       GetNode(target) == null;
            }).ToList();

            if (availableDirections.Count == 0) {
                pathsCreated++;
                pendingPositions.Pop();
                lastDir = null;
                continue;
            }

            var validCurrentDir = lastDir.HasValue && availableDirections.Contains(lastDir.Value)
                ? lastDir
                : null;

            var nextDir = directionSelector(validCurrentDir, availableDirections);
            lastDir = nextDir;

            var nextPos = currentPos + nextDir;
            var nextNode = GetOrCreateNode(nextPos);
            ConnectNodes(currentNode, nextDir, nextNode);
            
            pendingPositions.Push(nextPos);
        }

        return pathsCreated;
    }

    public static MazeGraph Create(int width, int height) {
        return new MazeGraph(width, height);
    }

    public static MazeGraph Create(bool[,] template) {
        return new MazeGraph(template.GetLength(1), template.GetLength(0), pos => template[pos.Y, pos.X]);
    }

    public static MazeGraph Create(Array2D<bool> template) {
        return new MazeGraph(template.Width, template.Height, pos => template[pos]);
    }

    public static MazeGraph Create(BitArray2D template) {
        return new MazeGraph(template.Width, template.Height, pos => template[pos]);
    }
}