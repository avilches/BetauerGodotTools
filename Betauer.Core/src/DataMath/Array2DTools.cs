using System;
using System.Linq;

namespace Betauer.Core.DataMath;

public static partial class Array2DTools {
    public static Array2D<bool> Border4Detector(Array2D<bool> region) {
        var borderGrid = new Array2D<bool>(region.Width, region.Height);
        borderGrid.Fill(false);
        var visited = new Array2D<bool>(region.Width, region.Height);

        for (int y = 0; y < region.Height; y++) {
            for (int x = 0; x < region.Width; x++) {
                if (!region[y, x] || visited[y, x]) continue;

                if (IsBorder(region, y, x)) {
                    borderGrid[y, x] = true;
                    FloodFillBorders(region, borderGrid, visited, y, x);
                }
            }
        }
        return borderGrid;
    }

    private static bool IsBorder(Array2D<bool> grid, int y, int x) {
        if (!grid[y, x]) return false;

        // Es borde si tiene algún vecino false o está en el borde del grid
        if (y == 0 || y == grid.Height - 1 || x == 0 || x == grid.Width - 1) return true;

        return !grid[y - 1, x] || !grid[y + 1, x] || !grid[y, x - 1] || !grid[y, x + 1];
    }

    private static void FloodFillBorders(Array2D<bool> original, Array2D<bool> borders, Array2D<bool> visited, int y, int x) {
        if (y < 0 || y >= original.Height || x < 0 || x >= original.Width ||
            visited[y, x] || !original[y, x]) return;

        visited[y, x] = true;

        if (IsBorder(original, y, x)) {
            borders[y, x] = true;
            // Continuamos el flood fill solo desde los bordes
            FloodFillBorders(original, borders, visited, y - 1, x); // Norte
            FloodFillBorders(original, borders, visited, y + 1, x); // Sur
            FloodFillBorders(original, borders, visited, y, x - 1); // Oeste
            FloodFillBorders(original, borders, visited, y, x + 1); // Este
        }
    }


    public static void ConnectBorderDiagonals(Array2D<bool> region) {
        var automata = new CellularAutomata<bool>(region)
            .AddNeighborhoodRule(3, neighbors => {
                var current = neighbors[1, 1];

                // Si es true, se mantiene
                if (current) return true;

                // Vecinos ortogonales
                bool N = neighbors[0, 1];
                bool S = neighbors[2, 1];
                bool W = neighbors[1, 0];
                bool E = neighbors[1, 2];

                // Vecinos diagonales
                bool NE = neighbors[0, 2];
                bool NW = neighbors[0, 0];
                bool SE = neighbors[2, 2];
                bool SW = neighbors[2, 0];

                // Si hay dos ortogonales adyacentes, creamos una conexión
                // SOLO si no hay ya una conexión diagonal
                if (N && W && !NW) return true; // └
                if (N && E && !NE) return true; // ┘
                if (S && E && !SE) return true; // ┐
                if (S && W && !SW) return true; // ┌

                return false;
            }, false);

        automata.SingleUpdate();
    }
    
    
    public static Array2D<bool> OuterBorder4Detector(Array2D<bool> region) {
        var borderGrid = new Array2D<bool>(region.Width, region.Height);
        borderGrid.Fill(false);
        var visited = new Array2D<bool>(region.Width, region.Height);

        for (int y = 0; y < region.Height; y++) {
            for (int x = 0; x < region.Width; x++) {
                if (region[y, x] || visited[y, x]) continue;

                if (IsOuterBorder(region, y, x)) {
                    borderGrid[y, x] = true;
                    FloodFillOuterBorders(region, borderGrid, visited, y, x);
                }
            }
        }
        return borderGrid;
    }

    private static bool IsOuterBorder(Array2D<bool> grid, int y, int x) {
        if (grid[y, x]) return false; // Solo nos interesan los píxeles false

        // Comprueba si algún vecino es true (parte de la figura)
        if (y > 0 && grid[y - 1, x]) return true; // Norte
        if (y < grid.Height - 1 && grid[y + 1, x]) return true; // Sur
        if (x > 0 && grid[y, x - 1]) return true; // Oeste
        if (x < grid.Width - 1 && grid[y, x + 1]) return true; // Este

        return false;
    }

    private static void FloodFillOuterBorders(Array2D<bool> original, Array2D<bool> borders, Array2D<bool> visited, int y, int x) {
        if (y < 0 || y >= original.Height || x < 0 || x >= original.Width ||
            visited[y, x] || original[y, x]) return;

        visited[y, x] = true;

        if (IsOuterBorder(original, y, x)) {
            borders[y, x] = true;
            // Continuamos el flood fill solo desde los bordes exteriores
            FloodFillOuterBorders(original, borders, visited, y - 1, x); // Norte
            FloodFillOuterBorders(original, borders, visited, y + 1, x); // Sur
            FloodFillOuterBorders(original, borders, visited, y, x - 1); // Oeste
            FloodFillOuterBorders(original, borders, visited, y, x + 1); // Este
        }
    }
    

}