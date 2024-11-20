using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.PCG.Maze;

public static class CellSelectors {
    // Recursive Backtracker: Siempre toma la última celda (LIFO)
    public static Func<IList<Vector2I>, int> Backtracker 
        => cells => cells.Count - 1;
        
    // Prim's Algorithm: Selección aleatoria
    public static Func<IList<Vector2I>, int> CreateRandom(Random rng) 
        => cells => rng.Next(cells.Count);
        
    // Breadth-First: Toma la celda más antigua (FIFO)
    public static Func<IList<Vector2I>, int> BreadthFirst 
        => cells => 0;
        
    // Eller's Algorithm: Alterna entre la más nueva y la más antigua
    public static Func<IList<Vector2I>, int> CreateEller(Random rng) 
        => cells => rng.NextSingle() < 0.5 ? cells.Count - 1 : 0;
}