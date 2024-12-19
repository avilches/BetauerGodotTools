using System.Collections.Immutable;

namespace Betauer.Core.PCG.GridTools;

using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Field of View calculator using Recursive Shadowcasting algorithm
/// </summary>
public class FoV<T>(Array2DGraph<T> graph, Func<Vector2I, bool>? isOpaqueFunc = null) {
    public Array2DGraph<T> Graph { get; } = graph;
    public HashSet<Vector2I> Fov { get; } = [];
    public Func<Vector2I, bool>? IsOpaqueFunc { get; set; } = isOpaqueFunc;
    
    public IReadOnlySet<Vector2I> ComputeFov(Vector2I origin, int radius, bool lightWalls = true) {
        if (!Graph.Array2D.IsValidPosition(origin)) return ImmutableHashSet<Vector2I>.Empty;
        var fov = new HashSet<Vector2I> { origin };
        SymmetricRecursiveShadowCasting.Scan(origin, radius, lightWalls, pos => fov.Add(pos), IsOpaque);
        return fov;
    }

    public void ClearFov() {
        Fov.Clear();
    }

    public IReadOnlySet<Vector2I> AppendFov(Vector2I origin, int radius, bool lightWalls = true) {
        if (!Graph.Array2D.IsValidPosition(origin)) return Fov;
        Fov.Add(origin);
        SymmetricRecursiveShadowCasting.Scan(origin, radius, lightWalls, AddToFoV, IsOpaque);
        return Fov;
        
        void AddToFoV(Vector2I pos) => Fov.Add(pos);
    }

    public bool IsInFov(Vector2I position) {
        return Fov.Contains(position);
    }

    public bool IsOpaque(Vector2I pos) {
        return !Graph.Array2D.IsValidPosition(pos) || (IsOpaqueFunc?.Invoke(pos) ?? !Graph.IsWalkablePosition(pos));
    }
}

internal readonly struct Transform(int xx, int xy, int yx, int yy) {
    public readonly int Xx = xx, Xy = xy, Yx = yx, Yy = yy;
}