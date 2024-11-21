using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Defines the type of cell in the maze
/// </summary>
public enum MazeCarveType {
    Start, // Starting point of the maze
    Path, // Regular path in the maze
    End, // End point of the maze
    Empty // Uncarved cell
}

public class MazeCarver {
    public static readonly Vector2I[] Directions = [Vector2I.Up, Vector2I.Down, Vector2I.Right, Vector2I.Left];

    public int Width { get; }
    public int Height { get; }

    // Delegates for customizable behavior
    public Func<Vector2I, bool>? IsCarvedFunc { get; set; } // Checks if a cell is carved
    public Action<Vector2I>? UnCarveAction { get; set; } // Removes a cell from the maze
    public Action<Vector2I, MazeCarveType>? CarveAction { get; set; } // Carves a cell with a specific type
    public Func<Vector2I, Vector2I, bool>? CanCarveFunc { get; set; } // Checks if a direction can be carved

    public MazeCarver(int width, int height) {
        Width = width;
        Height = height;
    }

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

        Carve(start, MazeCarveType.Start);
        var size = 1;

        usedCells.Add(start);
        while (usedCells.Count > 0) {
            var cellIndex = cellSelector(usedCells);
            var currentCell = usedCells[cellIndex];
            // Console.WriteLine("Selecting cell "+currentCell+" from "+string.Join(", ", usedCells));

            var availableDirections = Directions.Where(dir => CanCarve(currentCell, dir)).ToList();
            if (availableDirections.Count == 0) {
                if (--paths == 0) break;
                usedCells.RemoveAt(cellIndex); // discard the current cell
                lastDir = null; // Reset direction when we hit a dead end
                continue;
            }

            // Intentamos usar la dirección actual si está disponible
            var validCurrentDir = lastDir.HasValue && availableDirections.Contains(lastDir.Value)
                ? lastDir
                : null;

            // Console.WriteLine("Available dir: "+string.Join(", ", availableDirections)+ " lastDir: "+lastDir+ " valid last dir");

            var nextDir = directionSelector(validCurrentDir, availableDirections);
            lastDir = nextDir; // Guardamos la dirección que acabamos de elegir

            var nextCell1 = currentCell + nextDir;
            var nextCell2 = currentCell + nextDir * 2;
            Carve(nextCell1, MazeCarveType.Path);
            Carve(nextCell2, MazeCarveType.Path);
            lastPos = nextCell2;
            usedCells.Add(nextCell2);
            size += 2;
        }

        Carve(lastPos, MazeCarveType.End);
        return size;
    }

    // Helper methods for cell operations
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

    /// <summary>
    /// Creates a MazeCarver for a boolean grid.
    /// - true represents carved cells
    /// - false represents uncarved cells
    /// </summary>
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

    /// <summary>
    /// Creates a MazeCarver for a MazeCarveType grid.
    /// - Supports different cell types (Start, Path, End, Empty)
    /// - Provides more detailed maze representation
    /// </summary>
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