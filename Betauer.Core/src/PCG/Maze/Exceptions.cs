using System;
using Godot;

namespace Betauer.Core.PCG.Maze;

public class InvalidEdgeException(Vector2I from, Vector2I to) : Exception($"Invalid edge: {from} -> {to}") {
    public Vector2I From { get; } = from;
    public Vector2I To { get; } = to;
}

public class InvalidNodeException(string message, Vector2I position) : Exception(message) {
    public Vector2I Position { get; } = position;

}
