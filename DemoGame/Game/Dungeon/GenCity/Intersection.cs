using System.Collections.Generic;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public class Intersection(int id, Vector2I position) : ICityTile {
    public Vector2I Position { get; private set; } = position;
    public int Id { get; private set; } = id;

    private readonly List<Path> _outputPaths = [];
    private readonly List<Path> _inputPaths = [];

    public List<Path> GetOutputPaths() => _outputPaths;

    public List<Path> GetInputPaths() => _inputPaths;

    public List<Path> GetAllPaths() => [.._outputPaths, .._inputPaths];

    public Path AddOutputPath(int direction) {
        Path path = new Path(this, direction);
        _outputPaths.Add(path);
        return path;
    }

    public void RemoveOutputPath(Path path) {
        var index = _outputPaths.IndexOf(path);
        if (index != -1) {
            _outputPaths.RemoveAt(index);
        }
    }

    public void AddInputPath(Path path) {
        var existNode = path.GetNodeEnd();
        existNode?.RemoveInputPath(path);
        _inputPaths.Add(path);
    }

    public void RemoveInputPath(Path path) {
        var index = _inputPaths.IndexOf(path);
        if (index != -1) {
            _inputPaths.RemoveAt(index);
        }
    }

    public NodeType GetType() {
        var pathsCount = _outputPaths.Count + _inputPaths.Count;
        return pathsCount switch {
            2 => NodeType.TURN,
            1 => NodeType.END,
            _ => NodeType.CROSS
        };
    }
}