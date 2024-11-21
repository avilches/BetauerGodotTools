using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.PCG.Maze;

public static class CellSelectors {
    public static Func<IList<Vector2I>, int> Backtracker => cells => cells.Count - 1;
    public static Func<IList<Vector2I>, int> CreateRandom(Random rng) => cells => rng.Next(cells.Count);
}