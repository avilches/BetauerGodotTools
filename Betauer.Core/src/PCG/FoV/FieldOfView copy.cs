using System;
using System.Collections.Generic;
using Betauer.Core.PCG.GridTools;
using Godot;

namespace Betauer.Core.PCG.FoV;

/// <summary>
/// Field of View calculator
/// </summary>
public class LightZone(FovAlgorithm alg, Func<Vector2I, bool> isOpaqueFunc) {
    private readonly List<(Vector2I origin, int radius)> _lightSources = new();
    public static readonly Dictionary<FovAlgorithm, IFovScanner> Scanners = new() {
        { FovAlgorithm.SymmetricRecursiveShadowCasting, new SymmetricRecursiveShadowCasting() },
        { FovAlgorithm.Adammil, new FovAdammil() }
    };
    
    public static LightZone Adammil(Func<Vector2I, bool> isOpaqueFunc) => new(FovAlgorithm.Adammil, isOpaqueFunc);
    public static LightZone SymmetricRecursiveShadowCasting(Func<Vector2I, bool> isOpaqueFunc) => new(FovAlgorithm.SymmetricRecursiveShadowCasting, isOpaqueFunc);

    private readonly IFovScanner _algorithm = Scanners[alg];
    public HashSet<Vector2I> Fov { get; } = [];
    public Func<Vector2I, bool> IsOpaqueFunc { get; set; } = isOpaqueFunc;
    
    public IReadOnlySet<Vector2I> ComputeFov(Vector2I origin, int radius) {
        var fov = new HashSet<Vector2I>();
        _algorithm.Scan(origin, radius, pos => fov.Add(pos), IsOpaque);
        return fov;
    }

    public void ClearFov() {
        Fov.Clear();
    }

    public IReadOnlySet<Vector2I> AppendFov(Vector2I origin, int radius) {
        _algorithm.Scan(origin, radius, AddToFoV, IsOpaque);
        return Fov;
        
        void AddToFoV(Vector2I pos) => Fov.Add(pos);
    }

    public bool IsInFov(Vector2I position) {
        return Fov.Contains(position);
    }

    public bool IsOpaque(Vector2I pos) {
        return IsOpaqueFunc.Invoke(pos);
    }

    public bool IsTransparent(Vector2I pos) {
        return !IsOpaque(pos);
    }
}