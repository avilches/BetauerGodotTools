using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze;

public static class DirectionSelectors {
    // Aldous-Broder: Completamente aleatorio
    public static Func<Vector2I?, IList<Vector2I>, Vector2I> CreateRandom(Random? rng = null) {
        rng ??= new Random();
        return (lastDir, available) => available[rng.Next(available.Count)];
    }

    // Wilson: Preferencia por continuar en la misma dirección
    public static Func<Vector2I?, IList<Vector2I>, Vector2I> CreateWilson(float directionalBias, Random? rng = null) {
        rng ??= new Random();
        return (lastDir, available) => {
            if (lastDir.HasValue && rng.NextSingle() < directionalBias) {
                return lastDir.Value;
            }
            return available[rng.Next(available.Count)];
        };
    }

    // Hunt-and-Kill: Preferencia por dirección horizontal
    public static Func<Vector2I?, IList<Vector2I>, Vector2I> CreateHuntAndKill(Random? rng = null) {
        rng ??= new Random();
        return (lastDir, available) => {
            var horizontal = available.Where(d => d.Y == 0).ToList();
            return horizontal.Count > 0 ? horizontal[rng.Next(horizontal.Count)] : available[rng.Next(available.Count)];
        };
    }

    // Binary Tree: Preferencia por dirección vertical
    public static Func<Vector2I?, IList<Vector2I>, Vector2I> CreateBinaryTree(Random? rng = null) {
        rng ??= new Random();
        return (lastDir, available) => {
            var vertical = available.Where(d => d.X == 0).ToList();
            return vertical.Count > 0 ? vertical[rng.Next(vertical.Count)] : available[rng.Next(available.Count)];
        };
    }
}