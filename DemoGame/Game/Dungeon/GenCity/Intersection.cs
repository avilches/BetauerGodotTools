using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public enum IntersectionType {
    Turn,
    Cross,
    End
}

public class Intersection(int id, Vector2I position) : ICityTile {
    public Vector2I Position { get; } = position;
    public int Id { get; } = id;

    private readonly List<Path> _outputPaths = [];
    private readonly List<Path> _inputPaths = [];

    public List<Path> GetOutputPaths() => _outputPaths;

    public List<Path> GetInputPaths() => _inputPaths;

    public List<Path> GetAllPaths() => [.._outputPaths, .._inputPaths];

    public Path? Up => FindPathTo(Vector2I.Up);
    public Path? Down => FindPathTo(Vector2I.Down);
    public Path? Right => FindPathTo(Vector2I.Right);
    public Path? Left => FindPathTo(Vector2I.Left);

    public Path CreatePathTo(Vector2I direction) {
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

    public Path? FindPathTo(Vector2I direction) {
        foreach (var path in _inputPaths.Where(path => -path.Direction == direction)) {
            return path;
        }
        return _outputPaths.FirstOrDefault(path => path.Direction == direction);
    }

    public (Path? inPath, Path? outPath) GetOppositeDirectionPaths() {
        if (_inputPaths.Count + _outputPaths.Count != 2) return (null, null);

        if (_inputPaths.Count == 2 && -_inputPaths[0].Direction == _inputPaths[1].Direction) {
            return (_inputPaths[0], _inputPaths[1]);
        }

        if (_outputPaths.Count == 2 && -_outputPaths[0].Direction == _outputPaths[1].Direction) {
            return (_outputPaths[0], _outputPaths[1]);
        }

        if (_inputPaths.Count == 1 && _outputPaths.Count == 1 && _inputPaths[0].Direction == _outputPaths[0].Direction) {
            return (_inputPaths[0], _outputPaths[0]);
        }

        return (null, null);
    }

    public override string ToString() {
        return $"Intersection {Id} at {Position} ({IntersectionType})";
    }
}