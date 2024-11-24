namespace Betauer.Core.DataMath;

public static class Automatas {

    public static void RemoveAllDeadEnds(Array2D<bool> grid) {
        RemoveAllDeadEnds(grid, true, false);
    }

    public static void RemoveAllDeadEnds<T>(Array2D<T> grid, T fill, T empty) {
        var automata = new CellularAutomata<T>(grid).AddRule((g, pos) => {
            var value = g.GetValueSafe(pos);
            if (!Equals(value, fill)) return value;
            var pathNeighbors = grid.CountPathNeighbors(pos, fill);
            return pathNeighbors > 1 ? value : empty;
        });

        while (automata.SingleUpdate() > 0) {
            // Keep updating until no changes
        }
    }

    public static CellularAutomata<bool> CreateGameOfLife(Array2D<bool> grid) {
        return CreateGameOfLife(grid, true, false);
    }

    public static CellularAutomata<T> CreateGameOfLife<T>(Array2D<T> grid, T alive, T dead) {
        return new CellularAutomata<T>(grid).AddRule(neighbors => {
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
        var count = 0;
        for (var y = 0; y < 3; y++) {
            for (var x = 0; x < 3; x++) {
                if (x == 1 && y == 1) continue; // Skip center
                if (Equals(neighbors[y, x], matching)) count++;
            }
        }
        return count;
    }

    public static CellularAutomata<T> CreateSmoothCorners<T>(Array2D<T> grid, T fill, T empty, int deleteIfLessThan = 5, int addIfMoreThan = 5) {
        return new CellularAutomata<T>(grid)
            .AddRule(neighbors => {
                // Remove cells with less than minNeighborsToKeep neighbors
                if (Equals(neighbors[1, 1], empty)) return empty; // ignore dead cells
                var liveNeighbors = CountMooreNeighborhood(neighbors, fill);
                return liveNeighbors >= deleteIfLessThan ? fill : empty;
            }, empty)
            .AddRule(neighbors => {
                // Add cells with more than minNeighborsToAdd neighbors
                if (Equals(neighbors[1, 1], fill)) return fill; // ignore alive cells
                var liveNeighbors = CountMooreNeighborhood(neighbors, fill);
                return liveNeighbors > addIfMoreThan ? fill : empty;
            }, empty);
    }


    public static CellularAutomata<bool> CreateSmoothCorners(Array2D<bool> grid, int deleteIfLessThan = 5, int addIfMoreThan = 5) {
        return CreateSmoothCorners(grid, true, false, deleteIfLessThan, addIfMoreThan);
    }
}