using System;
using System.Collections.Generic;
using Betauer.Core.PCG.GridTools;
using Godot;

namespace Betauer.Core.PCG.FoV;

/// <summary>
/// Field of View calculator
/// </summary>
public class FieldOfView(FovAlgorithm alg, Func<Vector2I, bool> isOpaqueFunc) {
    public static FieldOfView Adammil(Func<Vector2I, bool> isOpaqueFunc) => new(FovAlgorithm.Adammil, isOpaqueFunc);
    public static FieldOfView SymmetricRecursiveShadowCasting(Func<Vector2I, bool> isOpaqueFunc) => new(FovAlgorithm.SymmetricRecursiveShadowCasting, isOpaqueFunc);

    public static readonly Dictionary<FovAlgorithm, IFovScanner> Scanners = new() {
        { FovAlgorithm.SymmetricRecursiveShadowCasting, new SymmetricRecursiveShadowCasting() },
        { FovAlgorithm.Adammil, new FovAdammil() }
    };
    
    private readonly IFovScanner _algorithm = Scanners[alg];
    private readonly List<(Vector2I origin, int radius)> _sources = new();
    
    public HashSet<Vector2I> Fov { get; } = [];
    public Func<Vector2I, bool> IsOpaqueFunc { get; set; } = isOpaqueFunc;
    
    public IReadOnlySet<Vector2I> AppendFov(Vector2I origin, int radius) {
        _sources.Add((origin, radius));
        _algorithm.Scan(origin, radius, AddToFoV, IsOpaque);
        return Fov;
        
        void AddToFoV(Vector2I pos) => Fov.Add(pos);
    }

    public void ClearFov() {
        Fov.Clear();
        _sources.Clear();
    }

    public float GetNormalizedFov(Vector2I position) {
        if (!IsInFov(position)) return 0f;

        var maxIntensity = 0f;
        foreach (var (origin, radius) in _sources) {
            if (position == origin) return 1f;
            
            var distance = origin.DistanceTo(position);
            if (distance <= radius) {
                // Linear falloff from origin (1.0) to radius edge (0.0)
                var intensity = 1f - distance / radius;
                maxIntensity = Math.Max(maxIntensity, intensity);
            }
        }
        
        return maxIntensity;
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