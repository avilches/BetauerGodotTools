using System.Linq;
using Godot;

namespace Betauer.Core.DataMath;

public static class Automatas {
    public static Vector2I[] Directions = new[] { Vector2I.Up, Vector2I.Right, Vector2I.Down, Vector2I.Left };

    public static void RemoveAllDeadEnds(Array2D<bool> grid) {
        RemoveAllDeadEnds(grid, true, false);
    }

    public static void RemoveAllDeadEnds<T>(Array2D<T> grid, T fill, T empty) {
        var automata = new CellularAutomata<T>(grid).OnUpdate((g, x, y) => {
            var value = g.GetValueSafe(x, y);
            if (!Equals(value, fill)) return value;
            var pathNeighbors = Directions.Count(dir => {
                var neighbor = g.GetValueSafe(x + dir.X, y + dir.Y);
                return Equals(neighbor, fill);
            });
            return pathNeighbors > 1 ? value : empty; // Keep only if it has more than 1 neighbor
        });
    
        while (automata.SingleUpdate() > 0) {
            // Keep updating until no changes
        }
    }

    public static CellularAutomata<bool> GameOfLife(Array2D<bool> grid) {
        return GameOfLife(grid, true, false);
    }

    public static CellularAutomata<T> GameOfLife<T>(Array2D<T> grid, T alive, T dead) {
        return new CellularAutomata<T>(grid).OnUpdate(neighbors => {
            var centerIsAlive = Equals(neighbors[1, 1], alive); // Centro de la matriz 3x3
            var liveNeighbors = CountLiveNeighbors(neighbors, alive);
        
            return centerIsAlive ? 
                (liveNeighbors is 2 or 3 ? alive : dead) : // Supervivencia
                (liveNeighbors == 3 ? alive : dead); // Nacimiento
        }, dead);
    }

    private static int CountLiveNeighbors<T>(T[,] neighbors, T alive) {
        var count = 0;
        for (var y = 0; y < 3; y++) {
            for (var x = 0; x < 3; x++) {
                if (x == 1 && y == 1) continue; // Skip center
                if (Equals(neighbors[y, x], alive)) count++;
            }
        }
        return count;
    }
}