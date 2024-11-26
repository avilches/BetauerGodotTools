using System;
using System.Linq;

namespace Betauer.Core.DataMath;

public static class Automatas {

    public static CellularAutomata<bool> CreateGameOfLife(Array2D<bool> grid) {
        return CreateGameOfLife(grid, true, false);
    }

    public static CellularAutomata<T> CreateGameOfLife<T>(Array2D<T> grid, T alive, T dead) {
        return new CellularAutomata<T>(grid).AddMooreNeighborhoodRule(neighbors => {
            var centerIsAlive = Equals(neighbors[1, 1], alive); // Centro de la matriz 3x3
            var liveNeighbors = CountMooreNeighborhood(neighbors, alive);

            return centerIsAlive
                ? liveNeighbors is 2 or 3
                    ? alive // Keep alive
                    : dead // Too crowded or lonely, die 
                : liveNeighbors == 3
                    ? alive // Newborn
                    : dead; // Stay dead
        }, dead);
    }

    public static int CountMooreNeighborhood<T>(T[,] neighbors, T matching) {
        return CountMooreNeighborhood(neighbors, v => Equals(v, matching));
    }

    public static int CountMooreNeighborhood<T>(T[,] neighbors, Func<T, bool> isEnabled) {
        var count = 0;
        for (var y = 0; y < 3; y++) {
            for (var x = 0; x < 3; x++) {
                if (x == 1 && y == 1) continue; // Skip center
                if (isEnabled(neighbors[y, x])) count++;
            }
        }
        return count;
    }

}