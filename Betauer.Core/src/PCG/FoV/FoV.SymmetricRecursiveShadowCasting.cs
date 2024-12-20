using System;
using Godot;

namespace Betauer.Core.PCG.FoV;

public class SymmetricRecursiveShadowCasting : IFovScanner {
    private readonly record struct Transform(int Xx, int Xy, int Yx, int Yy);
    
    private static readonly Transform[] Transforms = [
        new(1, 0, 0, 1), // 0
        new(1, 0, 0, -1), // 1
        new(-1, 0, 0, 1), // 2
        new(-1, 0, 0, -1), // 3
        new(0, 1, 1, 0), // 4
        new(0, 1, -1, 0), // 5
        new(0, -1, 1, 0), // 6
        new(0, -1, -1, 0) // 7
    ];

    public void Scan(Vector2I origin, int radius, Action<Vector2I> reveal, Func<Vector2I, bool> isOpaqueFunc) {
        reveal(origin);
        foreach (var transform in Transforms) {
            _Scan(origin, 1, 0, 1, transform, radius, true, reveal, isOpaqueFunc);
        }
    }

    private static void _Scan(Vector2I origin, int y, double start, double end, Transform transform, int radius, bool lightWalls, Action<Vector2I> reveal, Func<Vector2I, bool> isOpaqueFunc) {
        // Early exit conditions
        if (start >= end || y > radius) return;

        var xmin = (int)Math.Round((y - 0.5f) * start);
        var xmax = (int)Math.Ceiling((y + 0.5f) * end - 0.5f);
        var radiusSquared = radius * radius;

        for (var x = xmin; x <= xmax; x++) {
            var pos = new Vector2I(origin.X + transform.Xx * x + transform.Xy * y, origin.Y + transform.Yx * x + transform.Yy * y);

            if (x * x + y * y >= radiusSquared) continue;

            if (isOpaqueFunc(pos)) {
                if (lightWalls) {
                    if (x >= (y - 0.5) * start && x - 0.5 <= y * end) reveal(pos);
                }
                // Only recurse if we're within radius
                if (y + 1 <= radius) {
                    var newStart = start;
                    var newEnd = (x - 0.5f) / y;
                    // Only recurse if slopes are valid
                    if (newEnd > newStart) {
                        _Scan(origin, y + 1, newStart, newEnd, transform, radius, lightWalls, reveal, isOpaqueFunc);
                    }
                }
                start = (x + 0.5f) / y;
                if (start >= end) return;
            } else {
                if (x >= y * start && x <= y * end) {
                    reveal(pos);
                }
            }
        }

        // Only recurse if we're within radius
        if (y + 1 <= radius) {
            _Scan(origin, y + 1, start, end, transform, radius, lightWalls, reveal, isOpaqueFunc);
        }
    }
}