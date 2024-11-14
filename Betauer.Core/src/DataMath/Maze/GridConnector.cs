using System.Linq;

namespace Betauer.Core.DataMath.Maze;

using System;
using System.Collections.Generic;
using Godot;

public class GridConnector {
    public readonly Array2D<bool> Grid;
    public readonly Array2D<int> Labels;
    public int Width => Grid.Width;
    public int Height => Grid.Height;
    private readonly Dictionary<int, List<Vector2I>> _regions = new();
    private readonly List<Vector2I> _noRegion = new();

    public GridConnector(Array2D<bool> grid) {
        Grid = grid;
        Labels = new Array2D<int>(grid.Width, grid.Height).Fill(0);
        ProcessGrid();
    }

    public void Update() {
        Labels.Fill(0);
        _regions.Clear();
        _noRegion.Clear();
        ProcessGrid();
    }

    private void ProcessGrid() {
        var label = 1;
        foreach (var ((x, y), value) in Grid) {
            if (value && Labels[x, y] == 0) {
                var cells = new List<Vector2I>();
                MarkRegion(new Vector2I(x, y), label, cells);
                _regions[label] = cells;
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
                if (Grid[neighbor.X, neighbor.Y] && Labels[neighbor.X, neighbor.Y] == 0) {
                    Labels[neighbor.X, neighbor.Y] = label;
                    cells.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    // 2) Obtener el número de regiones
    public int GetRegions() {
        return _regions.Count;
    }

    public List<int> GetRegionsIds() {
        return _regions.Keys.ToList();
    }

    // 3) Obtener las celdas de una región específica
    public List<Vector2I> GetRegionCells(int region) {
        if (region == 0) {
            return _noRegion;
        }
        if (_regions.TryGetValue(region, out List<Vector2I>? value)) {
            return value;
        }
        return new List<Vector2I>();
    }

    public class Connections {
        public List<(Vector2I, HashSet<int>)> ConnectingCells = new();
        public List<Vector2I> Isolated = new();
    }

    // 4) Encontrar celdas vacías que conecten dos regiones distintas
    public Connections FindConnectingCells() {
        var data = new Connections();
        foreach (var (position, value) in Grid) {
            if (!value) {
                var adjacentRegions = new HashSet<int>();
                foreach (var neighbor in GetNeighbors(position)) {
                    if (Grid[neighbor.X, neighbor.Y]) {
                        adjacentRegions.Add(Labels[neighbor.X, neighbor.Y]);
                    }
                }
                if (adjacentRegions.Count > 1) {
                    data.ConnectingCells.Add((position, adjacentRegions));
                } else if (adjacentRegions.Count == 0) {
                    data.Isolated.Add(position);
                }
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
            _regions[regionLabel].Add(cell);

            // Fusionar otras regiones en la principal
            foreach (var label in adjacentRegions) {
                if (label != regionLabel) {
                    foreach (var regionCell in _regions[label]) {
                        Labels[regionCell.X, regionCell.Y] = regionLabel;
                        _regions[regionLabel].Add(regionCell);
                    }
                    _regions.Remove(label);
                }
            }
        } else {
            // Nueva región aislada
            var newLabel = _regions.Count + 1;
            Labels[cell.X, cell.Y] = newLabel;
            _regions[newLabel] = new List<Vector2I> { cell };
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

public class Demo {
    public static void Main() {
        // Uso de la clase
        var xYGrid = Array2D.Parse("""
                                   10111
                                   00011
                                   11000
                                   01101
                                   11010
                                   00000
                                   """, new Dictionary<char, bool>() {
            { '1', true },
            { '0', false },
        });

        // var xYGrid = YxDataGrid<bool>.Parse("""
        //                                   10111
        //                                   00011
        //                                   11000
        //                                   01101
        //                                   11010
        //                                   00000
        //                                   """, new Dictionary<char,bool>() {
        //     {'1', true},
        //     {'0', false},
        //     
        // });    

        foreach (var (position, value) in xYGrid) {
            if (!value) {
                Console.Write("·");
            } else {
                Console.Write("#");
            }
            if (position.X == xYGrid.Width - 1) {
                Console.WriteLine();
            }
        }
        Console.WriteLine("--------------------");

        var connector = new GridConnector(xYGrid);

        PrintState(connector);

        Console.WriteLine("Marking 0,0, ignore");
        connector.ToggleCell(new Vector2I(0, 0), true);
        PrintState(connector);
        Console.WriteLine("Marking 1,1, expand without change");
        connector.ToggleCell(new Vector2I(1, 1), true);
        PrintState(connector);
        Console.WriteLine("Marking 0,1, join regions");
        connector.ToggleCell(new Vector2I(0, 1), true);
        PrintState(connector);
        Console.WriteLine("Marking 1,0, join regions");
        connector.ToggleCell(new Vector2I(1, 0), true);
        PrintState(connector);
        Console.WriteLine("Marking 3,3, join regions");
        connector.ToggleCell(new Vector2I(3, 3), true);
        PrintState(connector);
        Console.WriteLine("Unmark 0,2, unjoin regions");
        connector.ToggleCell(new Vector2I(1, 1), false);
        connector.ToggleCell(new Vector2I(0, 1), false);
        connector.ToggleCell(new Vector2I(1, 0), false);
        connector.ToggleCell(new Vector2I(3, 3), false);
        PrintState(connector);
    }

    private static void PrintState(GridConnector connector) {
        Console.WriteLine("Total Regions: " + connector.GetRegions() + ", Ids: " + string.Join(", ", connector.GetRegionsIds()));

        foreach (var id in connector.GetRegionsIds()) {
            Console.WriteLine($"Region {id}: {string.Join(", ", connector.GetRegionCells(id))}");
        }
        Console.WriteLine($"No region (0): {string.Join(", ", connector.GetRegionCells(0))}");

        var connectingCells = connector.FindConnectingCells();
        Console.WriteLine("Isolated: " + string.Join(", ", connectingCells.Isolated));
        foreach (var (cell, regions) in connectingCells.ConnectingCells) {
            Console.WriteLine($"Cell ({cell.X}, {cell.Y}) connects regions: {string.Join(", ", regions)}");
        }

        foreach (var cell in connector.Labels) {
            if (cell.Value == 0) {
                if (connectingCells.Isolated.Contains(cell.Position)) {
                    Console.Write("i");
                } else {
                    Console.Write("·");
                }
            } else {
                Console.Write(cell.Value.ToString("x8").Substring(7, 1));
            }
            if (cell.Position.X == connector.Width - 1) {
                Console.WriteLine();
            }
        }
    }
}