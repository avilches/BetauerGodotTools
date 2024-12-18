using System;
using Godot;

namespace Betauer.Core.PCG.GridTools;

/// <summary>
/// Common heuristic functions for grid-based pathfinding
/// </summary>
public static class Heuristics {
    /// <summary>
    /// Manhattan distance: |x1-x2| + |y1-y2|
    /// Best for grids with only orthogonal movement
    /// </summary>
    public static float Manhattan(Vector2I a, Vector2I b) {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    /// <summary>
    /// Squared Euclidean distance: (x1-x2)² + (y1-y2)²
    /// Best for grids with any-angle movement
    /// Note: Returns squared distance for efficiency, since we only need
    /// to compare distances and sqrt is not necessary for A*
    /// </summary>
    public static float Euclidean(Vector2I a, Vector2I b) {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return dx * dx + dy * dy;
    }

    /// <summary>
    /// Chebyshev distance: max(|x1-x2|, |y1-y2|)
    /// Best for grids with orthogonal and diagonal movement
    /// </summary>
    public static float Chebyshev(Vector2I a, Vector2I b) {
        return Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
    }
}