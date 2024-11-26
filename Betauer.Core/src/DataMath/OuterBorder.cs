using System;

namespace Betauer.Core.DataMath;

public abstract class OuterBorderDetector {
    public static OuterBorderDetector<bool> Create(Array2D<bool> grid) {
        return Create(grid, true);
    }

    public static OuterBorderDetector<T> Create<T>(Array2D<T> grid, T fill) {
        return new OuterBorderDetector<T>(grid, v => Equals(v, fill));
    }
}

public class OuterBorderDetector<T>(Array2D<T> region, Func<T, bool> isEnabled) : OuterBorderDetector {
    private readonly Array2D<bool> _borderGrid = new(region.Width, region.Height);
    private readonly Array2D<bool> _visited = new(region.Width, region.Height);

    public OuterBorderDetector(T[,] region, Func<T, bool> isEnabled) : this(new Array2D<T>(region), isEnabled) {
    }

    public Array2D<bool> DetectBorders() {
        _borderGrid.Fill(false);
        _visited.Fill(false);

        for (var y = 0; y < region.Height; y++) {
            for (var x = 0; x < region.Width; x++) {
                if (isEnabled(region[y, x]) || _visited[y, x]) continue;

                if (IsOuterBorder(y, x)) {
                    _borderGrid[y, x] = true;
                    FloodFillOuterBorders(y, x);
                }
            }
        }
        return _borderGrid;
    }

    private bool IsOuterBorder(int y, int x) {
        if (isEnabled(region[y, x])) return false; // Solo nos interesan los píxeles !isEnabled

        // Comprueba si algún vecino es enabled (parte de la figura)
        if (y > 0 && isEnabled(region[y - 1, x])) return true; // Norte
        if (y < region.Height - 1 && isEnabled(region[y + 1, x])) return true; // Sur
        if (x > 0 && isEnabled(region[y, x - 1])) return true; // Oeste
        if (x < region.Width - 1 && isEnabled(region[y, x + 1])) return true; // Este

        return false;
    }

    private void FloodFillOuterBorders(int y, int x) {
        if (y < 0 || y >= region.Height || x < 0 || x >= region.Width ||
            _visited[y, x] || isEnabled(region[y, x])) return;

        _visited[y, x] = true;

        if (IsOuterBorder(y, x)) {
            _borderGrid[y, x] = true;
            // Continuamos el flood fill solo desde los bordes exteriores
            FloodFillOuterBorders(y - 1, x); // Norte
            FloodFillOuterBorders(y + 1, x); // Sur
            FloodFillOuterBorders(y, x - 1); // Oeste
            FloodFillOuterBorders(y, x + 1); // Este
        }
    }
}