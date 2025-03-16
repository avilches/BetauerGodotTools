using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Betauer.Core;
using Betauer.Core.PCG.GridTemplate;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public enum IntersectionType {
    Cross,  // 4
    Fork,   // 3
    Road,   // 2 straight
    Turn,   // 2 turn right or left
    DeadEnd // 1
}

public class Intersection(int id, Vector2I position) : ICityTile {
    public int Id { get; } = id;
    public Vector2I Position { get; } = position;

    private readonly List<Path> _outputPaths = [];
    private readonly List<Path> _inputPaths = [];

    public List<Path> GetOutputPaths() => _outputPaths;

    public List<Path> GetInputPaths() => _inputPaths;

    public List<Path> GetAllPaths() => [.._outputPaths, .._inputPaths];

    public List<Intersection> GetAllPathIntersections() => [
        .._outputPaths.Where(p=> p.IsCompleted()).Select(p => p.End!), 
        .._inputPaths.Select(p => p.Start)
    ];

    public Path? Up => FindPathTo(Vector2I.Up);
    public Path? Down => FindPathTo(Vector2I.Down);
    public Path? Right => FindPathTo(Vector2I.Right);
    public Path? Left => FindPathTo(Vector2I.Left);

    public byte GetDirectionFlags() {
        byte directions = 0;
        if (Left != null) directions |= (byte)DirectionFlag.Left;
        if (Right != null) directions |= (byte)DirectionFlag.Right;
        if (Up != null) directions |= (byte)DirectionFlag.Up;
        if (Down != null) directions |= (byte)DirectionFlag.Down;
        return directions;
    }

    private int _pathId = 0;

    public Path CreatePathTo(Vector2I direction) {
        if (_outputPaths.Any(existingPath => existingPath.Direction == direction)) {
            throw new ArgumentException($"Can't add an output path, there is another input path from the same direction: {direction.ToDirectionString()}");
        }
        var path = new Path($"{id}-{_pathId++}", this, direction);
        _outputPaths.Add(path);
        return path;
    }

    public void RemoveOutputPath(Path path) {
        _outputPaths.Remove(path);
    }

    public void AddInputPath(Path path) {
        if (!path.Start.Position.IsSameDirection(path.Direction, Position)) {
            throw new ArgumentException($"Intersection is not in the same line than the input path (starting from {path.Start.Position} to the {path.Direction.ToDirectionString()})");
        }

        if (_inputPaths.Any(existingPath => existingPath.Direction == path.Direction)) {
            throw new ArgumentException($"Can't add an input path, there is another input path from the same direction: {path.Direction.ToDirectionString()}");
        }

        if (_outputPaths.Any(existingPath => existingPath.Direction == -path.Direction)) {
            throw new ArgumentException($"Can't add an input path from {path.Direction.ToDirectionString()}. There is already an output path to the direction {path.Direction.ToDirectionString()}, which means two paths shares the same direction");
        }
        path.End?.RemoveInputPath(path);
        _inputPaths.Add(path);
    }

    public void RemoveInputPath(Path path) {
        _inputPaths.Remove(path);
    }

    public IntersectionType IntersectionType {
        get {
            var pathsCount = _outputPaths.Count + _inputPaths.Count;
            return pathsCount switch {
                4 => IntersectionType.Cross,
                3 => IntersectionType.Fork,
                2 => Up != null && Down != null ? IntersectionType.Road : IntersectionType.Turn,
                1 => IntersectionType.DeadEnd,
            };
        }
    }

    public Path? FindPathTo(Vector2I direction) {
        foreach (var path in _inputPaths.Where(path => -path.Direction == direction)) {
            return path;
        }
        return _outputPaths.FirstOrDefault(path => path.Direction == direction);
    }

    public IEnumerable<Path> FindPathsTo(Vector2I direction) {
        return _inputPaths.Where(path => -path.Direction == direction).Concat(_outputPaths.Where(path => path.Direction == direction));
    }

    public bool CanBeFlatten() {
        return CanBeFlatten(out _, out _);
    }

    public bool CanBeFlatten([MaybeNullWhen(false)] out Path path1, [MaybeNullWhen(false)] out Path path2) {
        // Primero verificar que hay exactamente 2 caminos en total
        if (_inputPaths.Count + _outputPaths.Count != 2) {
            path1 = null;
            path2 = null;
            return false;
        }

        var up = FindPathTo(Vector2I.Up);
        var down = FindPathTo(Vector2I.Down);
        var right = FindPathTo(Vector2I.Right);
        var left = FindPathTo(Vector2I.Left);

        // Case 1: Vertical straight path (up and down)
        if (up != null && down != null &&
            right == null && left == null &&
            up.IsCompleted() && down.IsCompleted()) {
            path1 = up;
            path2 = down;
            return true;
        }

        // Case 2: Horizontal straight path (left and right)
        if (up == null && down == null &&
            right != null && left != null &&
            right.IsCompleted() && left.IsCompleted()) {
            path1 = left;
            path2 = right;
            return true;
        }

        // No straight path found
        path1 = null;
        path2 = null;
        return false;
    }

    public override string ToString() {
        return $"Intersection \"{Id}\" {Position} - ({IntersectionType})";
    }

}