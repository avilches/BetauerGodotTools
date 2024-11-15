using System.Linq;

namespace Betauer.Core.DataMath.Maze;

using System;
using System.Collections.Generic;
using Godot;


public readonly record struct BorderCell(Vector2I Position, int[] Regions) {
    public readonly Vector2I Position = Position;
    public readonly int[] Regions = Regions;
}

public class GridConnector {
    public readonly Array2D<bool> Grid;
    public readonly Array2D<int> Labels;
    public int Width => Grid.Width;
    public int Height => Grid.Height;
    private readonly Dictionary<int, List<Vector2I>> _regionCells = new();
    private readonly List<Vector2I> _noRegion = new();

    public GridConnector(Array2D<bool> grid) {
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

    // 2) Obtener el número de regiones
    public int GetRegions() {
        return _regionCells.Count;
    }

    public List<int> GetRegionsIds() {
        return _regionCells.Keys.ToList();
    }

    // 3) Obtener las celdas de una región específica
    public List<Vector2I> GetRegionCells(int region) {
        if (region == 0) {
            return _noRegion;
        }
        if (_regionCells.TryGetValue(region, out List<Vector2I>? value)) {
            return value;
        }
        return new List<Vector2I>();
    }

    public class Connections {
        public Dictionary<Vector2I, HashSet<int>> ConnectingCells = new();
        public List<Vector2I> Isolated = new();
    }

    // 4) Encontrar celdas vacías que conecten dos regiones distintas

    public IEnumerator<BorderCell> GetBorderCellEnumerator() {
        foreach (var (position, value) in Grid) {
            if (value) continue;
            var adjacentRegions = new HashSet<int>();
            foreach (var neighbor in GetNeighbors(position)) {
                if (Grid[neighbor.X, neighbor.Y]) {
                    adjacentRegions.Add(Labels[neighbor.X, neighbor.Y]);
                }
            }
            yield return new BorderCell(position, adjacentRegions.ToArray());
        }
    }

    public Connections FindConnectingCells() {
        var data = new Connections();
        foreach (var (position, value) in Grid) {
            if (value) continue;
            var adjacentRegions = new HashSet<int>();
            foreach (var neighbor in GetNeighbors(position)) {
                if (Grid[neighbor.X, neighbor.Y]) {
                    adjacentRegions.Add(Labels[neighbor.X, neighbor.Y]);
                }
            }
            if (adjacentRegions.Count > 1) {
                data.ConnectingCells[position] = adjacentRegions;
            } else if (adjacentRegions.Count == 0) {
                data.Isolated.Add(position);
            }
        }
        return data;
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