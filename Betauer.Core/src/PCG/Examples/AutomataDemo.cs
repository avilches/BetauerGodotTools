using Betauer.Core.DataMath;

namespace Betauer.Core.PCG.Examples;

using System;

public class AutomataDemo {
    public static void Main() {
        while (true) {
            var data = Array2D.ParseAsBool("""
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ·····································#·····································
                                           ····································##·····································
                                           ·····································##····································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           ···········································································
                                           """, '#');
            var gol = Automatas.CreateGameOfLife(data);
            for (var n = 0; n < 500; n++) {
                Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
                gol.Update();
                Console.Clear();
            }
        }
    }
    
}


/*
// 2. Game of Life
public class GameOfLife {
    private CellularAutomata<bool> automata;

    public GameOfLife(int width, int height) {
        automata = new CellularAutomata<bool>(height, width, UpdateCell);
    }

    private bool UpdateCell(bool[,] grid, int y, int x) {
        var neighbors = CountLiveNeighbors(grid, y, x);
        var isAlive = grid[y, x];

        if (isAlive)
            return neighbors == 2 || neighbors == 3;
        else
            return neighbors == 3;
    }

    private int CountLiveNeighbors(bool[,] grid, int y, int x) {
        var count = 0;
        for (int dy = -1; dy <= 1; dy++) {
            for (int dx = -1; dx <= 1; dx++) {
                if (dx == 0 && dy == 0) continue;
                int newY = y + dy, newX = x + dx;
                if (newY >= 0 && newY < grid.GetLength(0) &&
                    newX >= 0 && newX < grid.GetLength(1) &&
                    grid[newY, newX]) {
                    count++;
                }
            }
        }
        return count;
    }

    public void Update() => automata.Update();

    public void SetCell(int y, int x, bool value) => automata.SetCell(y, x, value);
}

// 3. Sand and Water Simulator
public enum ParticleType {
    Empty,
    Sand,
    Water
}

public class SandWaterSimulator {
    private CellularAutomata<ParticleType> automata;
    private Random random = new Random();

    public SandWaterSimulator(int width, int height) {
        automata = new CellularAutomata<ParticleType>(height, width, UpdateParticle);
    }

    private ParticleType UpdateParticle(ParticleType[,] grid, int y, int x) {
        var current = grid[y, x];
        if (current == ParticleType.Empty) return ParticleType.Empty;

        int below = y + 1;
        bool canFall = below < grid.GetLength(0);

        if (!canFall) return current; // En el fondo

        if (current == ParticleType.Sand) {
            // Intentar caer directamente abajo
            if (canFall && grid[below, x] == ParticleType.Empty)
                return ParticleType.Empty;

            // Intentar caer diagonal
            if (canFall) {
                bool tryLeft = random.Next(2) == 0; // Aleatorizar dirección
                int[] directions = tryLeft ? new[] { -1, 1 } : new[] { 1, -1 };

                foreach (int dx in directions) {
                    int newX = x + dx;
                    if (newX >= 0 && newX < grid.GetLength(1) &&
                        grid[below, newX] == ParticleType.Empty &&
                        grid[y, newX] == ParticleType.Empty)
                        return ParticleType.Empty;
                }
            }
        } else if (current == ParticleType.Water) {
            // Intentar caer
            if (canFall && grid[below, x] == ParticleType.Empty)
                return ParticleType.Empty;

            // Intentar fluir a los lados
            int[] directions = random.Next(2) == 0 ? new[] { -1, 1 } : new[] { 1, -1 };

            foreach (int dx in directions) {
                int newX = x + dx;
                if (newX >= 0 && newX < grid.GetLength(1) &&
                    grid[y, newX] == ParticleType.Empty)
                    return ParticleType.Empty;
            }
        }

        return current;
    }

    public void Update() {
        automata.Update();

        // Mover partículas a nuevas posiciones
        var state = automata.GetCurrentState();
        for (int y = state.GetLength(0) - 1; y >= 0; y--) {
            for (int x = 0; x < state.GetLength(1); x++) {
                if (state[y, x] == ParticleType.Empty) {
                    // Buscar partícula que cayó aquí
                    for (int dy = -1; dy <= 1; dy++) {
                        for (int dx = -1; dx <= 1; dx++) {
                            int sourceY = y + dy;
                            int sourceX = x + dx;
                            if (sourceY >= 0 && sourceY < state.GetLength(0) &&
                                sourceX >= 0 && sourceX < state.GetLength(1)) {
                                var sourceCell = state[sourceY, sourceX];
                                if (sourceCell != ParticleType.Empty) {
                                    automata.SetCell(y, x, sourceCell);
                                    automata.SetCell(sourceY, sourceX, ParticleType.Empty);
                                    goto nextCell;
                                }
                            }
                        }
                    }
                }
                nextCell:
                continue;
            }
        }
    }

    public void SetParticle(int y, int x, ParticleType type) =>
        automata.SetCell(y, x, type);
}
*/