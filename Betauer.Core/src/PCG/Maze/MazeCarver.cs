using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Betauer.Core.PCG.Maze;

public class MazeCarver(int width, int height, Func<Vector2I, bool>? isCarved, Action<Vector2I> carveAction) {
    public int Width { get; } = width;
    public int Height { get; } = height;

    public Func<Vector2I, bool>? IsCarved { get; set; } = isCarved;
    public event Action<Vector2I>? OnCarve = carveAction;

    public int GrowBacktracker(Vector2I start, float directionalBias, int paths = -1, Random? rng = null) {
        rng ??= new Random();
        return Grow(start, DirectionSelectors.CreateRandom(directionalBias, rng), CellSelectors.Backtracker, paths);
    }

    public int GrowVerticalBias(Vector2I start, float verticalBias, int paths = -1, Random? rng = null) {
        rng ??= new Random();
        return Grow(start, DirectionSelectors.CreateVerticalSelector(verticalBias, rng), CellSelectors.Backtracker, paths);
    }

    public int GrowHorizontalBias(Vector2I start, float horizontalBias, int paths = -1, Random? rng = null) {
        rng ??= new Random();
        return Grow(start, DirectionSelectors.CreateHorizontalSelector(horizontalBias, rng), CellSelectors.Backtracker, paths);
    }

    public int GrowClockwiseBias(Vector2I start, float bias, int paths = -1, Random? rng = null) {
        rng ??= new Random();
        return Grow(start, DirectionSelectors.CreateClockwiseSelector(bias, rng), CellSelectors.Backtracker, paths);
    }

    public int GrowCounterClockwiseBias(Vector2I start, float bias, int paths = -1, Random? rng = null) {
        rng ??= new Random();
        return Grow(start, DirectionSelectors.CreateCounterClockwiseSelector(bias, rng), CellSelectors.Backtracker, paths);
    }

    public int GrowRandom(Vector2I start, int paths = -1, Random? rng = null) {
        rng ??= new Random();
        return Grow(start, DirectionSelectors.CreateRandom(rng), CellSelectors.CreateRandom(rng), paths);
    }

    /// <summary>
    /// Generate a maze starting in a specific cell. The directionSelector is used to choose which direction must be carved based on the possible candidates.
    ///
    /// CellSelector is used to choose which cell must be selected when the current path reaches a dead end. It receives the list of the available cells in order
    /// and it must return the index of the cell to select. Returning the candidates size - 1 is the default behavior (backtracking): it means it will use
    /// the last cell before the dead to continue the path.  
    /// </summary>
    /// <param name="start"></param>
    /// <param name="directionSelector"></param>
    /// <param name="cellSelector"></param>
    /// <param name="paths">Number of paths to create in the maze. -1 means create paths until no more cells left. N paths = N-1 branches</param>
    /// <returns></returns>
    public int Grow(Vector2I start, Func<Vector2I?, IList<Vector2I>, Vector2I> directionSelector, Func<IList<Vector2I>, int>? cellSelector = null, int paths = -1) {
        if (IsCarved(start)) return 0;
        cellSelector ??= CellSelectors.Backtracker;

        var usedCells = new List<Vector2I>();
        Vector2I lastPos = start;
        Vector2I? lastDir = null;

        OnCarve(start);
        var size = 1;

        usedCells.Add(start);
        while (usedCells.Count > 0) {
            var cellIndex = cellSelector(usedCells);
            var currentCell = usedCells[cellIndex];
            // Console.WriteLine("Selecting cell "+currentCell+" from "+string.Join(", ", usedCells));

            var availableDirections = Array2D.Directions.Where(dir => {
                var target = currentCell + dir * 2;
                return Geometry.IsPointInRectangle(target.X, target.Y, 0, 0, Width, Height) && !IsCarved(target);
            }).ToList();
            if (availableDirections.Count == 0) {
                if (--paths == 0) break;
                usedCells.RemoveAt(cellIndex); // discard the current cell
                lastDir = null; // Reset direction when we hit a dead end
                continue;
            }

            var validCurrentDir = lastDir.HasValue && availableDirections.Contains(lastDir.Value)
                ? lastDir
                : null;

            // Console.WriteLine("Available dir: "+string.Join(", ", availableDirections)+ " lastDir: "+lastDir+ " valid last dir");

            var nextDir = directionSelector(validCurrentDir, availableDirections);
            lastDir = nextDir;

            var nextCell1 = currentCell + nextDir;
            var nextCell2 = currentCell + nextDir * 2;
            OnCarve(nextCell1);
            OnCarve(nextCell2);
            lastPos = nextCell2;
            usedCells.Add(nextCell2);
            size += 2;
        }

        OnCarve(lastPos);
        return size;
    }

    /// <summary>
    /// Creates a MazeCarver for a boolean grid.
    /// - true represents carved cells
    /// - false represents uncarved cells
    /// </summary>
    public static MazeCarver Create(Array2D<bool> grid) {
        return Create(grid, true, false);
    }

    /// <summary>
    /// Creates a MazeCarver for a boolean BitArray2D.
    /// - true represents carved cells
    /// - false represents uncarved cells
    /// </summary>
    public static MazeCarver Create(BitArray2D grid) {
        return new MazeCarver(grid.Width, grid.Height, pos => grid[pos], pos => grid[pos] = true);
    }

    /// <summary>
    /// Creates a MazeCarver for a grid of any type.
    /// - Carves by setting the cell to filled.
    /// - Only carves in no empty cells.
    /// </summary>
    public static MazeCarver Create<T>(Array2D<T> grid, T filled, T empty) {
        return new MazeCarver(grid.Width, grid.Height, pos => !Equals(grid[pos], empty), pos => grid[pos] = filled);
    }
}