using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Betauer.Core.PCG.Maze;

public enum MazeCarveType {
    Start,
    Path,
    End,
    Empty
}

public class MazeCarver(int width, int height) {
    protected static readonly Vector2I[] Directions = [Vector2I.Up, Vector2I.Down, Vector2I.Right, Vector2I.Left];

    public int Width { get; } = width;
    public int Height { get; } = height;

    // Func delegates for customizable behavior
    public Func<Vector2I, bool>? IsCarvedFunc { get; set; }
    public Action<Vector2I>? UnCarveAction { get; set; }
    public Action<Vector2I, MazeCarveType>? CarveAction { get; set; }
    public Func<Vector2I, Vector2I, bool>? CanCarveFunc { get; set; }


    public int GrowBacktracker(Vector2I start, float directionalBias, Random? rng = null) {
        rng ??= new Random();
        return Grow(start, DirectionSelectors.CreateWilson(directionalBias, rng), CellSelectors.Backtracker);
    }

    public int GrowPrim(Vector2I start, Random? rng = null) {
        rng ??= new Random();
        return Grow(start, DirectionSelectors.CreateRandom(rng), CellSelectors.CreateRandom(rng));
    }

    public int GrowBreadthFirst(Vector2I start, float directionalBias, Random? rng = null) {
        rng ??= new Random();
        return Grow(start, DirectionSelectors.CreateWilson(directionalBias, rng), CellSelectors.BreadthFirst);
    }

    public int GrowBinaryTree(Vector2I start, Random? rng = null) {
        rng ??= new Random();
        return Grow(start, DirectionSelectors.CreateBinaryTree(rng), CellSelectors.Backtracker); // En Binary Tree la selección de celda no es relevante
    }

    public int GrowHuntAndKill(Vector2I start, Random? rng = null) {
        rng ??= new Random();
        return Grow(start, DirectionSelectors.CreateHuntAndKill(rng), CellSelectors.Backtracker);
    }

    public int GrowEller(Vector2I start, float directionalBias, Random? rng = null) {
        rng ??= new Random();
        return Grow(start, DirectionSelectors.CreateWilson(directionalBias, rng), CellSelectors.CreateEller(rng));
    }

    public int Grow(Vector2I start, Func<Vector2I?, IList<Vector2I>, Vector2I> directionSelector, Func<IList<Vector2I>, int> cellSelector) {
        if (IsCarved(start)) return 0;

        var cells = new List<Vector2I>();
        Vector2I? lastDir = null;
        Vector2I lastPos = start;

        Carve(start, MazeCarveType.Start);
        var size = 1;

        cells.Add(start);
        while (cells.Count > 0) {
            var cellIndex = cellSelector(cells);
            var cell = cells[cellIndex];

            var nextCellsAvailable = Directions.Where(dir => CanCarve(cell, dir)).ToList();
            if (nextCellsAvailable.Count == 0) {
                cells.RemoveAt(cellIndex);
                lastDir = null;
                continue;
            }

            var validLastDir = lastDir.HasValue && nextCellsAvailable.Contains(lastDir.Value) ? lastDir : null;
            Vector2I dir = directionSelector(validLastDir, nextCellsAvailable);
            lastDir = dir;

            var nextCell = cell + dir * 2;
            Carve(cell + dir, MazeCarveType.Path);
            Carve(nextCell, MazeCarveType.Path);
            lastPos = nextCell;
            cells.Add(nextCell);
            size += 2;
        }

        Carve(lastPos, MazeCarveType.End);
        return size;
    }


    public void RemoveDeadEnds() {
        bool done;
        do {
            done = true;
            for (var y = 1; y < Height - 1; y++) {
                for (var x = 1; x < Width - 1; x++) {
                    var pos = new Vector2I(x, y);
                    if (!IsCarved(pos)) continue;
                    var exits = Directions.Count(dir => IsCarved(pos + dir));
                    if (exits == 1) {
                        done = false;
                        UnCarve(pos);
                    }
                }
            }
        } while (!done);
        for (var y = 1; y < Height - 1; y++) {
            for (var x = 1; x < Width - 1; x++) {
                var pos = new Vector2I(x, y);
                if (IsCarved(pos)) {
                    var exits = Directions.Count(dir => IsCarved(pos + dir));
                    if (exits == 0) {
                        UnCarve(pos);
                    }
                }
            }
        }
    }

    public bool IsCarved(Vector2I pos) {
        if (IsCarvedFunc == null) throw new InvalidOperationException($"{nameof(IsCarvedFunc)} not set");
        return IsCarvedFunc!.Invoke(pos);
    }

    public void UnCarve(Vector2I pos) {
        if (UnCarveAction == null) throw new InvalidOperationException($"{nameof(UnCarveAction)} not set");
        UnCarveAction!.Invoke(pos);
    }

    public void Carve(Vector2I pos, MazeCarveType type) {
        if (CarveAction == null) throw new InvalidOperationException($"{nameof(CarveAction)} not set");
        CarveAction!.Invoke(pos, type);
    }

    public bool CanCarve(Vector2I pos, Vector2I direction) {
        if (CanCarveFunc == null) throw new InvalidOperationException($"{nameof(CanCarveFunc)} not set");
        return CanCarveFunc!.Invoke(pos, direction);
    }

    public static MazeCarver Create(Array2D<bool> grid) {
        return new MazeCarver(grid.Width, grid.Height) {
            IsCarvedFunc = pos => grid[pos],
            UnCarveAction = pos => grid[pos] = false,
            CarveAction = (pos, type) => grid[pos] = true,
            CanCarveFunc = (pos, dir) => {
                var target = pos + dir * 2;
                return Geometry.IsPointInRectangle(target.X, target.Y, 0, 0, grid.Width, grid.Height) && !grid[target];
            }
        };
    }

    public static MazeCarver Create(Array2D<MazeCarveType> grid) {
        return new MazeCarver(grid.Width, grid.Height) {
            IsCarvedFunc = pos => grid[pos] != MazeCarveType.Empty,
            UnCarveAction = pos => grid[pos] = MazeCarveType.Empty,
            CarveAction = (pos, type) => grid[pos] = type,
            CanCarveFunc = (pos, dir) => {
                var target = pos + dir * 2;
                return Geometry.IsPointInRectangle(target.X, target.Y, 0, 0, grid.Width, grid.Height) && grid[target] == MazeCarveType.Empty;
            }
        };
    }
}