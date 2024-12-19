using System;
using System.Collections.Generic;
using Betauer.Core.DataMath;
using Godot;

namespace Betauer.Core.PCG.GridTools;

public static class SymmetricRecursiveShadowCasting {
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

    public static void Scan(Vector2I origin, int radius, bool lightWalls, Action<Vector2I> reveal, Func<Vector2I, bool> isOpaqueFunc) {
        reveal(origin);
        foreach (var transform in Transforms) {
            _Scan(origin, 1, 0, 1, transform, radius, lightWalls, reveal, isOpaqueFunc);
        }
    }

    private static void _Scan(Vector2I origin, int y, double start, double end, Transform transform, int radius, bool lightWalls, Action<Vector2I> reveal, Func<Vector2I, bool> isOpaqueFunc) {
        // Early exit conditions
        if (start >= end || y > radius) return;

        var xmin = (int)Math.Round((y - 0.5f) * start);
        var xmax = (int)Math.Ceiling((y + 0.5f) * end - 0.5f);
        var radiusSquared = radius * radius;

        for (var x = xmin; x <= xmax; x++) {
            var realX = origin.X + transform.Xx * x + transform.Xy * y;
            var realY = origin.Y + transform.Yx * x + transform.Yy * y;
            var pos = new Vector2I(realX, realY);

            var distanceSquared = x * x + y * y;
            if (distanceSquared > radiusSquared) continue;

            var isOpaque = isOpaqueFunc(pos);
            if (!isOpaque) {
                if (x >= y * start && x <= y * end) {
                    reveal(pos);
                }
            } else {
                if (x >= (y - 0.5) * start && x - 0.5 <= y * end) {
                    if (lightWalls) reveal(pos);
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
            }
        }

        // Only recurse if we're within radius
        if (y + 1 <= radius) {
            _Scan(origin, y + 1, start, end, transform, radius, lightWalls, reveal, isOpaqueFunc);
        }
    }
}