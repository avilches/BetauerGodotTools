using System;

namespace Betauer.Core.DataMath;

public static class Automatas {

    public static void RemoveAllDeadEnds(Array2D<bool> grid) {
        RemoveAllDeadEnds(grid, true, false);
    }

    public static void RemoveAllDeadEnds<T>(Array2D<T> grid, T fill, T empty) {
        RemoveAllDeadEnds(grid, v => Equals(v, fill), (v, b) => b ? fill : empty, empty);
    }

    public static void RemoveAllDeadEnds<T>(Array2D<T> grid, Func<T, bool> isEnabled, Func<T, bool, T> update, T defaultValue) {
        var changes = 1;
        var automata = new CellularAutomata<T>(grid).AddRule((g, pos) => {
            var value = g.GetValueSafe(pos);
            if (!isEnabled(value)) return value;
            var pathNeighbors = grid.CountPathNeighbors(pos, isEnabled);
            value = update(value, pathNeighbors > 1);
            if (!isEnabled(value)) changes++;
            return value;
        });

        while (changes > 0) {
            changes = 0;
            automata.SingleUpdate();
        }
    }

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

    public static CellularAutomata<T> CreateSmoothCorners<T>(Array2D<T> grid, T fill, T empty, int deleteIfLessThan = 5, int addIfMoreThan = 5) {
        return CreateSmoothCorners(grid, 
            v => Equals(v, fill), 
            (v, b) => b ? fill : empty, empty, deleteIfLessThan, addIfMoreThan);
    }

    public static CellularAutomata<T> CreateSmoothCorners<T>(Array2D<T> grid, Func<T, bool> isEnabled, Func<T, bool, T> set, T defaultValue, int deleteIfLessThan = 5, int addIfMoreThan = 5) {
        return new CellularAutomata<T>(grid)
            .AddMooreNeighborhoodRule(neighbors => {
                // Remove cells with less than minNeighborsToKeep neighbors
                var value = neighbors[1, 1];
                if (!isEnabled(value)) return value; // ignore dead cells
                var liveNeighbors = CountMooreNeighborhood(neighbors, isEnabled);
                value = set(value, liveNeighbors >= deleteIfLessThan);
                return value;
            }, defaultValue)
            .AddMooreNeighborhoodRule(neighbors => {
                // Add cells with more than minNeighborsToAdd neighbors
                var value = neighbors[1, 1];
                if (isEnabled(value)) return value; // ignore alive cells
                var liveNeighbors = CountMooreNeighborhood(neighbors, isEnabled);
                value = set(value, liveNeighbors > addIfMoreThan);
                return value;
            }, defaultValue);
    }


    public static CellularAutomata<bool> CreateSmoothCorners(Array2D<bool> grid, int deleteIfLessThan = 5, int addIfMoreThan = 5) {
        return CreateSmoothCorners(grid, true, false, deleteIfLessThan, addIfMoreThan);
    }
}