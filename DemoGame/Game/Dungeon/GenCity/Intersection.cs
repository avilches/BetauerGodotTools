using System.Collections.Generic;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public enum IntersectionType {
    Turn,
    Cross,
    End
}

public class Intersection(int id, Vector2I position) : ICityTile {
    public Vector2I Position { get; private set; } = position;
    public int Id { get; private set; } = id;

    private readonly List<Path> _outputPaths = [];
    private readonly List<Path> _inputPaths = [];

    public List<Path> GetOutputPaths() => _outputPaths;

    public List<Path> GetInputPaths() => _inputPaths;

    public List<Path> GetAllPaths() => [.._outputPaths, .._inputPaths];

    public Path AddOutputPath(Vector2I direction) {
        Path path = new Path(this, direction);
        _outputPaths.Add(path);
        return path;
    }

    public void RemoveOutputPath(Path path) {
        _outputPaths.Remove(path);
    }

    public void AddInputPath(Path path) {
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
                2 => IntersectionType.Turn,
                1 => IntersectionType.End,
                _ => IntersectionType.Cross
            };
        }
    }
}