using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Represents a maze as a graph structure with nodes and edges in a 2d grid.
/// </summary>
public class MazeGraph {
    protected readonly Dictionary<Vector2I, MazeNode> NodeGrid = [];
    protected readonly Dictionary<int, MazeNode> Nodes = [];

    public Func<Vector2I, IEnumerable<Vector2I>> GetAdjacentPositions { get; set; }

    private readonly List<Func<Vector2I, bool>> _positionValidators = [];
    private readonly List<Func<MazeNode, MazeNode, bool>> _edgeValidators = [];

    public void AddPositionValidator(Func<Vector2I, bool> validator) => _positionValidators.Add(validator);
    public void RemovePositionValidator(Func<Vector2I, bool> validator) => _positionValidators.Remove(validator);
    public void AddEdgeValidator(Func<MazeNode, MazeNode, bool> validator) => _edgeValidators.Add(validator);
    public void RemoveEdgeValidator(Func<MazeNode, MazeNode, bool> validator) => _edgeValidators.Remove(validator);
    public bool IsValidPosition(Vector2I position) => _positionValidators.Count == 0 || _positionValidators.All(validator => validator(position));
    public bool CanConnect(MazeNode from, MazeNode to) => _edgeValidators.Count == 0 || _edgeValidators.All(validator => validator(from, to));
    public void ClearPositionValidators() => _positionValidators.Clear();
    public void ClearEdgeValidators() => _edgeValidators.Clear();

    public event Action<MazeEdge>? OnEdgeCreated;
    public event Action<MazeEdge>? OnEdgeRemoved;
    public event Action<MazeNode>? OnNodeCreated;
    public event Action<MazeNode>? OnNodeRemoved;

    protected int LastId = 0;

    /// <summary>
    /// Initializes a new instance of the MazeGraph class.
    /// </summary>
    /// Optional, by default, it uses ortogonal positions up, down, left, right </param>
    public MazeGraph() {
        GetAdjacentPositions = GetOrtogonalPositions;
    }

    public readonly record struct AttributeKey(object Instance, string Key);

    private readonly Dictionary<AttributeKey, object> _attributes = [];

    internal void SetAttribute(object instance, string key, object value) => _attributes[new(instance, key)] = value;
    internal object? GetAttribute(object instance, string key) => _attributes.GetValueOrDefault(new(instance, key));
    internal object GetAttributeOr(object instance, string key, object defaultValue) => _attributes.GetValueOrDefault(new(instance, key), defaultValue);
    internal T? GetAttributeAs<T>(object instance, string key) => _attributes.TryGetValue(new(instance, key), out var value) && value is T typedValue ? typedValue : default;
    internal T GetAttributeAsOrDefault<T>(object instance, string key, T defaultValue) => _attributes.TryGetValue(new(instance, key), out var value) && value is T typedValue ? typedValue : defaultValue;
    internal T GetAttributeAsOrNew<T>(object instance, string key) where T : new() => _attributes.TryGetValue(new(instance, key), out var value) && value is T typedValue ? typedValue : new T();
    internal T GetAttributeAsOr<T>(object instance, string key, Func<T> factory) => _attributes.TryGetValue(new(instance, key), out var value) && value is T typedValue ? typedValue : factory();
    internal bool RemoveAttribute(object instance, string key) => _attributes.Remove(new(instance, key));
    internal bool HasAttribute(object instance, string key) => _attributes.ContainsKey(new(instance, key));
    internal bool HasAttributeWithValue(object instance, string key, object value) => _attributes.TryGetValue(new(instance, key), out var existingValue) && Equals(existingValue, value);
    internal bool HasAttributeOfType<T>(object instance, string key) => _attributes.TryGetValue(new(instance, key), out var value) && value is T;

    internal IEnumerable<KeyValuePair<string, object>> GetAttributes(object instance) {
        return _attributes
            .Where(kv => kv.Key.Instance == instance)
            .Select(kv => new KeyValuePair<string, object>(kv.Key.Key, kv.Value));
    }

    internal int GetAttributeCount(object instance) => _attributes.Count(kv => kv.Key.Instance == instance);
    internal bool HasAnyAttribute(object instance) => _attributes.Keys.Any(k => k.Instance == instance);

    internal void ClearAttributes(object instance) {
        var keys = _attributes.Keys.Where(k => k.Instance == instance).ToList();
        foreach (var key in keys) {
            _attributes.Remove(key);
        }
    }

    public void Clear() {
        NodeGrid.Clear();
        Nodes.Clear();
        _attributes.Clear();
        LastId = 0;
    }

    public MazeNode GetNode(int id) {
        return Nodes[id];
    }

    public MazeNode? GetNodeOrNull(int id) {
        return Nodes.GetValueOrDefault(id);
    }

    public IReadOnlyCollection<MazeNode> GetNodes() {
        return Nodes.Values;
    }

    public MazeNode GetOrCreateNode(Vector2I position) {
        ValidatePosition(position, nameof(GetOrCreateNode));
        return NodeGrid.TryGetValue(position, out var node)
            ? node
            : CreateNode(position);
    }

    private void ValidatePosition(Vector2I position, string message) {
        if (!IsValidPosition(position)) {
            throw new InvalidNodeException($"Invalid position {position} in {message}", position);
        }
    }

    public MazeNode CreateNode(Vector2I position, MazeNode? parent = null, object metadata = default, float weight = 0f, int? id = null) {
        ValidatePosition(position, nameof(CreateNode));
        if (NodeGrid.TryGetValue(position, out var existingNode)) {
            throw new InvalidOperationException($"Can't create node at {position}: there is already a node there with id {existingNode.Id}");
        }
        if (id.HasValue && Nodes.ContainsKey(id.Value)) {
            throw new InvalidOperationException($"Can't create node: id {id.Value} already exists");
        }

        var nodeId = id ?? LastId;
        if (!id.HasValue || id.Value >= LastId) {
            LastId = nodeId + 1;
        }

        var node = new MazeNode(this, nodeId, position) {
            Parent = parent,
            Metadata = metadata!,
            Weight = weight
        };
        NodeGrid[position] = node;
        Nodes[node.Id] = node;
        OnNodeCreated?.Invoke(node);
        return node;
    }

    public MazeNode GetNodeAt(Vector2I position) {
        ValidatePosition(position, nameof(GetNodeAt));
        return NodeGrid[position];
    }

    public bool HasNodeAt(Vector2I position) {
        return NodeGrid.ContainsKey(position);
    }

    public bool HasNode(int id) {
        return Nodes.ContainsKey(id);
    }

    public MazeNode? GetNodeAtOrNull(Vector2I position) {
        return IsValidPosition(position) ? NodeGrid.GetValueOrDefault(position) : null;
    }

    internal bool InternalRemoveNode(MazeNode node) {
        if (!Nodes.Remove(node.Id)) return false;
        NodeGrid.Remove(node.Position);
        OnNodeRemoved?.Invoke(node);
        return true;
    }

    public bool RemoveNode(int id) {
        return Nodes.TryGetValue(id, out var node) && node.RemoveNode();
    }

    public bool RemoveNodeAt(Vector2I position) {
        var node = GetNodeAtOrNull(position);
        return node != null && node.RemoveNode();
    }


    public void DisconnectNodes(int fromId, int toId) {
        var from = GetNode(fromId);
        var to = GetNode(toId);
        from.RemoveEdgeTo(to);
    }

    public void DisconnectNodes(Vector2I fromPos, Vector2I toPos) {
        var from = GetNodeAt(fromPos)!;
        var to = GetNodeAt(toPos)!;
        from.RemoveEdgeTo(to);
    }

    public MazeEdge ConnectNodes(int fromId, int toId, object metadata = default, float weight = 0f) {
        var from = GetNode(fromId);
        var to = GetNode(toId);
        return from.ConnectTo(to, metadata, weight);
    }

    public MazeEdge ConnectNodes(Vector2I fromPosition, Vector2I toPosition, object metadata = default, float weight = 0f) {
        var from = GetNodeAt(fromPosition)!;
        var to = GetNodeAt(toPosition)!;
        return from.ConnectTo(to, metadata, weight);
    }

    public MazeEdge ConnectNodes(MazeNode from, MazeNode to, object metadata = default, float weight = 0f) {
        return from.ConnectTo(to, metadata, weight);
    }

    internal void InvokeOnEdgeCreated(MazeEdge edge) {
        OnEdgeCreated?.Invoke(edge);
    }

    internal void InvokeOnEdgeRemoved(MazeEdge edge) {
        OnEdgeRemoved?.Invoke(edge);
    }

    /// <summary>
    /// Returns all the adjacent nodes to the specified node, no matter if they are connected or not, or if one
    /// is the parent of the other.
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public IEnumerable<MazeNode> GetAdjacentNodes(Vector2I from) {
        return GetAdjacentPositions(from)
            .Where(IsValidPosition)
            .Select(pos => NodeGrid.GetValueOrDefault(pos)!)
            .Where(node => node != null!);
    }

    /// <summary>
    /// Returns the adjacent positions to the specified node that are free (no node in that position) and valid, so a
    /// new node could be placed there.
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public IEnumerable<Vector2I> GetAvailableAdjacentPositions(Vector2I from) {
        return GetAdjacentPositions(from)
            .Where(adjacentPos => IsValidPosition(adjacentPos) && !NodeGrid.ContainsKey(adjacentPos));
    }

    /// <summary>
    /// Returns the directions to the adjacent positions to the specified node that are free (no node in that position)
    /// and valid, so a new node could be placed there.
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public IEnumerable<Vector2I> GetValidFreeAdjacentDirections(Vector2I from) {
        return GetAvailableAdjacentPositions(from).Select(position => position - from);
    }

    public static IEnumerable<Vector2I> GetOrtogonalPositions(Vector2I from) {
        return Array2D.Directions.Select(dir => from + dir);
    }

    public Vector2I GetOffset() {
        return new Vector2I(GetNodes().Min(v => v.Position.X), GetNodes().Min(v => v.Position.Y));
    }

    public Vector2I GetSize() {
        return new Vector2I(
            GetNodes().Max(v => v.Position.X) - GetNodes().Min(v => v.Position.X),
            GetNodes().Max(v => v.Position.Y) - GetNodes().Min(v => v.Position.Y));
    }

    public string Draw() {
        var allCanvas = new TextCanvas();
        foreach (var node in GetNodes()) {
            var canvas = new TextCanvas();
            canvas.Write(1, 1, node.Id.ToString());
            if (node.Up != null) canvas.Write(1, 0, "|");
            if (node.Right != null) canvas.Write(2, 1, "--");
            if (node.Down != null) canvas.Write(1, 2, "|");
            if (node.Left != null) canvas.Write(0, 1, "-");
            allCanvas.Write(node.Position.X * 4, node.Position.Y * 3, canvas.ToString());
        }
        return allCanvas.ToString();
    }

    public string Print(char c = '\u2588') {
        var allCanvas = new TextCanvas();
        foreach (var node in GetNodes()) {
            var canvas = new TextCanvas();
            canvas.Write(1, 1, "\u2588");
            if (node.Up != null) canvas.Write(1, 0, "\u2588");
            if (node.Left != null) canvas.Write(0, 1, "\u2588");
            allCanvas.Write(node.Position.X * 2, node.Position.Y * 2, canvas.ToString());
        }
        return allCanvas.ToString();
    }

    /// <summary>
    /// Returns all the nodes without Parent
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MazeNode> GetRoots() {
        return GetNodes().Where(n => n.Parent == null);
    }

    /// <summary>
    /// Parse a string with a maze representation into a MazeGraph where "#" are nodes and "-" and "|" are double connections. Example:
    ///     #
    ///     |
    /// #-#-#   #
    /// | |     |
    /// #-#-#-#-#
    ///   |   | |
    /// #-#   # #
    /// </summary>
    /// <param name="text"></param>
    /// <param name="lineSeparator"></param>
    /// <returns></returns>
    public static MazeGraph Parse(string text, char lineSeparator = '\n') {
        var maze = new MazeGraph();
        if (text == null || text.Trim().Length == 0) return maze;
        var lines = text.Split(lineSeparator);

        // Remove empty lines at start and end
        while (lines.Length > 0 && string.IsNullOrWhiteSpace(lines[0])) lines = lines.Skip(1).ToArray();
        while (lines.Length > 0 && string.IsNullOrWhiteSpace(lines[^1])) lines = lines.SkipLast(1).ToArray();

        // Remove minimum common left padding
        var minPadding = lines.Where(l => !string.IsNullOrWhiteSpace(l))
            .Min(l => l.TakeWhile(c => c == ' ').Count());
        lines = lines.Select(line => line.Length >= minPadding ? line[minPadding..] : line).ToArray();

        // Keep track of positions marked with '#'
        var hashPositions = new List<Vector2I>();

        // First pass: Create nodes with IDs and collect '#' positions
        for (var y = 0; y < lines.Length; y += 2) { // Only odd lines can have nodes
            var line = lines[y];
            for (var x = 0; x < line.Length; x += 2) { // Only odd columns can have nodes
                var c = line[x];
                var position = new Vector2I(x / 2, y / 2);
                if (char.IsDigit(c)) {
                    maze.CreateNode(position, id: c - '0');
                } else if (char.IsLetter(c)) {
                    maze.CreateNode(position, id: (int)c);
                } else if (c == '#') {
                    hashPositions.Add(position);
                }
            }
        }

        // Create nodes for remaining '#' positions that don't have a node yet
        foreach (var position in hashPositions) {
            maze.CreateNode(position);
        }

        // Create horizontal connections
        for (var y = 0; y < lines.Length; y += 2) { // Only odd lines can have nodes and horizontal connections
            var line = lines[y];
            for (var x = 0; x < line.Length - 2; x += 2) {
                var fromChar = line[x];
                var toChar = line[x + 2];
                if ((fromChar == '#' || char.IsLetterOrDigit(fromChar)) &&
                    line[x + 1] == '-' &&
                    (toChar == '#' || char.IsLetterOrDigit(toChar))) {
                    var fromNode = maze.GetNodeAt(new Vector2I(x / 2, y / 2));
                    var toNode = maze.GetNodeAt(new Vector2I((x + 2) / 2, y / 2));
                    fromNode.ConnectTo(toNode);
                }
            }
        }

        // Create vertical connections
        for (var y = 1; y < lines.Length; y += 2) { // Even lines can only have vertical connections
            var line = lines[y];
            for (var x = 0; x < line.Length; x += 2) {
                if (line[x] == '|') {
                    var fromNode = maze.GetNodeAt(new Vector2I(x / 2, (y - 1) / 2));
                    var toNode = maze.GetNodeAt(new Vector2I(x / 2, (y + 1) / 2));
                    fromNode.ConnectTo(toNode);
                }
            }
        }

        return maze;
    }

    public string Export(bool showIds = false, char lineSeparator = '\n') {
        if (GetNodes().Count == 0) return "";

        var offset = GetOffset();
        var size = GetSize();
        var width = (size.X + 1) * 2 - 1;
        var height = (size.Y + 1) * 2 - 1;

        // Create empty grid
        var grid = new char[height][];
        for (var y = 0; y < height; y++) {
            grid[y] = new char[width];
            for (var x = 0; x < width; x++) {
                grid[y][x] = ' ';
            }
        }

        // Place nodes
        foreach (var node in GetNodes()) {
            var x = (node.Position.X - offset.X) * 2;
            var y = (node.Position.Y - offset.Y) * 2;
            if (showIds) {
                // Si es un número del 0-9, usar el caracter directamente
                if (node.Id is >= 0 and <= 9) {
                    grid[y][x] = (char)('0' + node.Id);
                }
                // Si es un caracter ASCII (65-90 = A-Z, 97-122 = a-z), usar el caracter
                else if (node.Id is >= 65 and <= 90 || node.Id is >= 97 and <= 122) {
                    grid[y][x] = (char)node.Id;
                }
                // Para cualquier otro ID, usar #
                else {
                    grid[y][x] = '#';
                }
            } else {
                grid[y][x] = '#';
            }
        }

        // Place horizontal connections
        foreach (var node in GetNodes()) {
            var x = (node.Position.X - offset.X) * 2;
            var y = (node.Position.Y - offset.Y) * 2;
            if (node.Right != null) {
                grid[y][x + 1] = '-';
            }
        }

        // Place vertical connections
        foreach (var node in GetNodes()) {
            var x = (node.Position.X - offset.X) * 2;
            var y = (node.Position.Y - offset.Y) * 2;
            if (node.Down != null) {
                grid[y + 1][x] = '|';
            }
        }

        return string.Join(lineSeparator, grid.Select(row => new string(row)));
    }

    /// <summary>
    /// Creates a new MazeGraph with the specified dimensions.
    /// </summary>
    public static MazeGraph Create(int width, int height) {
        var mazeGraph = new MazeGraph();
        mazeGraph.AddPositionValidator(pos => Geometry.IsPointInRectangle(pos.X, pos.Y, 0, 0, width, height));
        return mazeGraph;
    }

    /// <summary>
    /// Creates a MazeGraph from a boolean template array.
    /// In the template, true values represent valid positions for nodes. false values are forbidden (invalid)
    /// </summary>
    public static MazeGraph Create(bool[,] template) {
        var mazeGraph = new MazeGraph();
        mazeGraph.AddPositionValidator(pos => Geometry.IsPointInRectangle(pos.X, pos.Y, 0, 0, template.GetLength(1), template.GetLength(0)) && template[pos.Y, pos.X]);
        return mazeGraph;
    }

    /// <summary>
    /// Creates a MazeGraph from a boolean Array2D template.
    /// In the template, true values represent valid positions for nodes. false values are forbidden (invalid)
    /// </summary>
    public static MazeGraph Create(Array2D<bool> template) {
        var mazeGraph = new MazeGraph();
        mazeGraph.AddPositionValidator(pos => Geometry.IsPointInRectangle(pos.X, pos.Y, 0, 0, template.Width, template.Height) && template[pos.Y, pos.X]);
        return mazeGraph;
    }

    /// <summary>
    /// Creates a MazeGraph from a BitArray2D template.
    /// In the template, true values represent valid positions for nodes. false values are forbidden (invalid)
    /// </summary>
    public static MazeGraph Create(BitArray2D template) {
        var mazeGraph = new MazeGraph();
        mazeGraph.AddPositionValidator(pos => Geometry.IsPointInRectangle(pos.X, pos.Y, 0, 0, template.Width, template.Height) && template[pos.Y, pos.X]);
        return mazeGraph;
    }
}