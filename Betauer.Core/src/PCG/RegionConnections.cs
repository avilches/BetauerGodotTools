using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Godot;

namespace Betauer.Core.PCG;

public class RegionConnections {
    public readonly Array2D<bool> Grid;
    public readonly Array2D<int> Labels;
    public int Width => Grid.Width;
    public int Height => Grid.Height;
    private readonly Dictionary<int, List<Vector2I>> _regionCells = new();
    private readonly List<Vector2I> _noRegion = new();

    public RegionConnections(int width, int height): this(new Array2D<bool>(width, height)){
    }

    public RegionConnections(Array2D<bool> grid) {
        Grid = grid;
        Labels = new Array2D<int>(grid.Width, grid.Height).Fill(0);
        ProcessGrid();
    }

    public void Update() {
        Labels.Fill(0);
        _regionCells.Clear();
        _noRegion.Clear();
        ProcessGrid();
    }

    private void ProcessGrid() {
        var label = 1;
        foreach (var ((x, y), value) in Grid) {
            if (value && Labels[x, y] == 0) {
                var cells = new List<Vector2I>();
                MarkRegion(new Vector2I(x, y), label, cells);
                _regionCells[label] = cells;
                label++;
            } else if (!value) {
                Labels[x, y] = 0;
                _noRegion.Add(new Vector2I(x, y));
            }
        }
    }

    // Método para etiquetar una región con BFS
    private void MarkRegion(Vector2I start, int label, List<Vector2I> cells) {
        var queue = new Queue<Vector2I>();
        queue.Enqueue(start);
        Labels[start.X, start.Y] = label;
        cells.Add(start);

        while (queue.Count > 0) {
            var current = queue.Dequeue();
            foreach (var neighbor in GetNeighbors(current)) {
                if (Grid[neighbor.X, neighbor.Y] &&
                    Labels[neighbor.X, neighbor.Y] == 0) {
                    Labels[neighbor.X, neighbor.Y] = label;
                    cells.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    public int GetRegions() {
        return _regionCells.Count;
    }

    public List<int> GetRegionsIds() {
        return _regionCells.Keys.ToList();
    }

    public List<Vector2I> GetRegionCells(int region) {
        if (region == 0) {
            return _noRegion;
        }
        if (_regionCells.TryGetValue(region, out List<Vector2I>? value)) {
            return value;
        }
        return new List<Vector2I>();
    }

    /// <summary>
    /// Returns all empty cells with their adjacent regions.
    /// 0 adjacent regions means the cell is isolated. You can toggle the cell and it will create a new region itself alone
    /// 1 adjacent region means the cell is outside a region, you can toggle the cell and it will join the region, increasing
    /// the size of it without touching other regions.
    /// 2 or more adjacent regions means the cell is a border cell between two or three regions. You can toggle the cell and
    /// it will connect these regions in one region bigger. 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<(Vector2I Position, int[] Regions)> GetNoRegionCells() {
        foreach (var x in _noRegion) {
            var adjacentRegions = GetNeighbors(x)
                .Where(n => Grid[n.X, n.Y])
                .Select(n => Labels[n.X, n.Y])
                .ToHashSet();
            yield return (x, adjacentRegions.ToArray());
        }
    }

    public IEnumerable<Vector2I> GetIsolatedCells() {
        return GetNoRegionCells().Where(b => b.Regions.Length == 0).Select(b => b.Position);
    }

    public IEnumerable<(Vector2I Position, int Region)> GetExpandableCells() {
        return GetNoRegionCells().Where(b => b.Regions.Length == 1).Select(b => (b.Position, b.Regions[0]));
    }

    public Dictionary<Vector2I, int> GetExpandableCellsByPosition() {
        var expandable = new Dictionary<Vector2I, int>();
        foreach (var (position, region) in GetExpandableCells()) {
            expandable[position] = region;
        }
        return expandable;
    }

    public Dictionary<int, List<Vector2I>> GetExpandableCellsByRegion() {
        var expandable = new Dictionary<int, List<Vector2I>>();
        foreach (var (position, region) in GetExpandableCells()) {
            if (expandable.TryGetValue(region, out var positions)) {
                positions.Add(position);
            } else {
                expandable[region] = new List<Vector2I> { position };
            }
        }
        return expandable;
    }

    public IEnumerable<(Vector2I Position, int[] Regions)> GetConnectingCells() {
        return GetNoRegionCells().Where(b => b.Regions.Length >= 2);
    }

    public Dictionary<Vector2I, int[]> GetConnectingCellsByPosition() {
        var connectingCells = new Dictionary<Vector2I, int[]>();
        foreach (var (position, regions) in GetConnectingCells()) {
            connectingCells[position] = regions;
        }
        return connectingCells;
    }

    /// <summary>
    /// The key is a string with the regions separated by commas like "1,3" or "2,3,5"
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, List<Vector2I>> GetConnectingCellsByRegion() {
        var regionsConnected = new Dictionary<string, List<Vector2I>>();
        foreach (var (position, regionArray) in GetConnectingCells()) {
            var regions = regionArray.ToList();
            regions.Sort();
            var r = string.Join(",", regions);
            if (!regionsConnected.TryGetValue(r, out List<Vector2I>? list)) {
                regionsConnected[r] = list = new List<Vector2I>();
            }
            list.Add(position);
        }
        return regionsConnected;
    }


    public void ToggleCell(Vector2I cell, bool fill) {
        if (fill) {
            if (Grid[cell.X, cell.Y]) return;
            _noRegion.Remove(cell);
            Grid[cell.X, cell.Y] = true;
            UpdateRegionAfterFill(cell);
        } else {
            if (!Grid[cell.X, cell.Y]) return;
            Grid[cell.X, cell.Y] = false;
            Update();
        }
    }

    private void UpdateRegionAfterFill(Vector2I cell) {
        var adjacentRegions = new HashSet<int>();
        foreach (var (x, y) in GetNeighbors(cell)) {
            if (Grid[x, y]) {
                adjacentRegions.Add(Labels[x, y]);
            }
        }

        // Si hay regiones adyacentes, une la celda a la primera región encontrada
        if (adjacentRegions.Count > 0) {
            var regionLabel = adjacentRegions.First();
            Labels[cell.X, cell.Y] = regionLabel;
            _regionCells[regionLabel].Add(cell);

            // Fusionar otras regiones en la principal
            foreach (var label in adjacentRegions) {
                if (label != regionLabel) {
                    foreach (var regionCell in _regionCells[label]) {
                        Labels[regionCell.X, regionCell.Y] = regionLabel;
                        _regionCells[regionLabel].Add(regionCell);
                    }
                    _regionCells.Remove(label);
                }
            }
        } else {
            // Nueva región aislada sin vecinos
            var nums = _regionCells.Keys.ToHashSet();
            var newLabel = Enumerable.Range(1, _regionCells.Keys.Count + 1)
                .First(n => !nums.Contains(n)); // First unused number
            Labels[cell.X, cell.Y] = newLabel;
            _regionCells[newLabel] = new List<Vector2I> { cell };
        }
    }

    private IEnumerable<Vector2I> GetNeighbors(Vector2I cell) {
        var directions = new[] { -1, 0, 1, 0, 0, -1, 0, 1 };
        for (var dir = 0; dir < directions.Length; dir += 2) {
            var newX = cell.X + directions[dir];
            var newY = cell.Y + directions[dir + 1];
            if (newX >= 0 && newX < Width && newY >= 0 && newY < Height) {
                yield return new Vector2I(newX, newY);
            }
        }
    }
}