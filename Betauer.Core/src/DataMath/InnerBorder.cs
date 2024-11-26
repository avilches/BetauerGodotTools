using System;

namespace Betauer.Core.DataMath;

public abstract class InnerBorderDetector {
    public static InnerBorderDetector<bool> Create(Array2D<bool> grid) {
        return Create(grid, true);
    }

    public static InnerBorderDetector<T> Create<T>(Array2D<T> grid, T fill) {
        return new InnerBorderDetector<T>(grid, v => Equals(v, fill));
    }
}

public class InnerBorderDetector<T>(Array2D<T> region, Func<T, bool> isEnabled) : InnerBorderDetector {
    private readonly Array2D<bool> _borderGrid = new(region.Width, region.Height);
    private readonly Array2D<bool> _visited = new(region.Width, region.Height);

    public InnerBorderDetector(T[,] region, Func<T, bool> isEnabled) : this(new Array2D<T>(region), isEnabled) {
    }

    public Array2D<bool> DetectBorders() {
        _borderGrid.Fill(false);
        _visited.Fill(false);

        for (var y = 0; y < region.Height; y++) {
            for (var x = 0; x < region.Width; x++) {
                if (!isEnabled(region[y, x]) || _visited[y, x]) continue;

                if (IsInnerBorder(y, x)) {
                    _borderGrid[y, x] = true;
                    FloodFillBorders(y, x);
                }
            }
        }
        return _borderGrid;
    }

    private bool IsInnerBorder(int y, int x) {
        if (!isEnabled(region[y, x])) return false;

        // Es borde si tiene algún vecino !isEnabled o está en el borde del grid
        if (y == 0 || y == region.Height - 1 || x == 0 || x == region.Width - 1) return true;

        return !isEnabled(region[y - 1, x]) || !isEnabled(region[y + 1, x]) ||
               !isEnabled(region[y, x - 1]) || !isEnabled(region[y, x + 1]);
    }

    private void FloodFillBorders(int y, int x) {
        if (y < 0 || y >= region.Height || x < 0 || x >= region.Width ||
            _visited[y, x] || !isEnabled(region[y, x])) return;

        _visited[y, x] = true;

        if (IsInnerBorder(y, x)) {
            _borderGrid[y, x] = true;
            // Continuamos el flood fill solo desde los bordes
            FloodFillBorders(y - 1, x); // Norte
            FloodFillBorders(y + 1, x); // Sur
            FloodFillBorders(y, x - 1); // Oeste
            FloodFillBorders(y, x + 1); // Este
        }
    }
}