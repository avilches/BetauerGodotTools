using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Implements a maze generation algorithm using a carving technique.
/// This class creates mazes by carving paths through a grid, starting from a given position
/// and following configurable constraints and direction selection strategies.
/// </summary>
public class MazeCarver {
    /// <summary>
    /// Gets the width of the maze grid.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height of the maze grid.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Function that determines if a cell has been carved.
    /// </summary>
    public Func<Vector2I, bool>? IsCarved { get; set; }

    /// <summary>
    /// Event that is triggered when a cell is carved.
    /// </summary>
    public event Action<Vector2I>? OnCarve;

    /// <summary>
    /// Initializes a new instance of the MazeCarver class.
    /// </summary>
    /// <param name="width">The width of the maze grid. Must be positive.</param>
    /// <param name="height">The height of the maze grid. Must be positive.</param>
    /// <param name="isCarved">Optional function to determine if a cell is carved.</param>
    /// <param name="carveAction">Action to execute when carving a cell. Cannot be null.</param>
    /// <exception cref="ArgumentException">Thrown when width or height are not positive.</exception>
    /// <exception cref="ArgumentNullException">Thrown when carveAction is null.</exception>
    public MazeCarver(int width, int height, Func<Vector2I, bool>? isCarved, Action<Vector2I> carveAction) {
        if (width <= 0) throw new ArgumentException("Width must be positive", nameof(width));
        if (height <= 0) throw new ArgumentException("Height must be positive", nameof(height));

        Width = width;
        Height = height;
        IsCarved = isCarved ?? throw new ArgumentNullException(nameof(isCarved));
        OnCarve = carveAction ?? throw new ArgumentNullException(nameof(carveAction));
    }

    /// <summary>
    /// Generates a maze starting from a specific cell, following the given constraints.
    /// </summary>
    /// <param name="start">The starting position for maze generation.</param>
    /// <param name="constraints">Constraints that control the maze generation process.</param>
    /// <returns>The number of cells carved during maze generation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when constraints is null.</exception>
    public int Grow(Vector2I start, MazeConstraints constraints) {
        ArgumentNullException.ThrowIfNull(constraints);
        if (IsCarved(start)) return 0;
        var maxTotalCells = constraints.MaxTotalCells == -1 ? int.MaxValue : constraints.MaxTotalCells;
        var maxCellsPerPath = constraints.MaxCellsPerPath == -1 ? int.MaxValue : constraints.MaxCellsPerPath;
        var maxPaths = constraints.MaxPaths == -1 ? int.MaxValue : constraints.MaxPaths;
        var constraintsMaxDepth = constraints.MaxDepth == -1 ? int.MaxValue : constraints.MaxDepth;
        // if (constraintsMaxTotalCells == 0 || constraintsMaxCellsPerPath == 0 || constraintsMaxPaths == 0) return 0;

        var carvedCells = new Stack<Vector2I>();
        var pathDepths = new Dictionary<Vector2I, int>(); // Almacena la profundidad de cada punto de inicio de path
        Vector2I? lastDirection = null;

        CarveCell(start);
        var pathsCreated = 0;
        var totalCellsCarved = 1;
        var cellsCarvedInCurrentPath = 1;

        carvedCells.Push(start);
        var currentDepth = pathDepths[start] = 0; // El path inicial tiene profundidad 0
        var backtracking = false;

        while (carvedCells.Count > 0 &&
               totalCellsCarved < maxTotalCells &&
               pathsCreated < maxPaths) {
            var currentCell = carvedCells.Peek();
            var availableDirections = GetAvailableDirections(currentCell);

            // backtracking
            if (availableDirections.Count == 0 || cellsCarvedInCurrentPath >= maxCellsPerPath) {
                carvedCells.Pop();
                backtracking = true;
                cellsCarvedInCurrentPath = 1;
                lastDirection = null;
                continue;
            }

            if (backtracking) {
                backtracking = false;
                pathsCreated++;
                Console.WriteLine($"Path #{pathsCreated} finished. Depth: {currentDepth}: Cells: {cellsCarvedInCurrentPath}");
                if (pathsCreated == maxPaths) break;
                cellsCarvedInCurrentPath = 1;
            }

            var validCurrentDirection = lastDirection.HasValue && availableDirections.Contains(lastDirection.Value)
                ? lastDirection
                : null;

            var nextDirection = constraints.DirectionSelector(validCurrentDirection, availableDirections);
            lastDirection = nextDirection;

            var intermediateCell = currentCell + nextDirection;
            var nextCellToCarve = currentCell + nextDirection * 2;

            CarveCell(intermediateCell);
            CarveCell(nextCellToCarve);

            carvedCells.Push(nextCellToCarve);
            pathDepths[nextCellToCarve] = currentDepth; // Guardamos la profundidad de la nueva celda
            totalCellsCarved += 2;
            cellsCarvedInCurrentPath += 2;
        }

        // Obtenemos la profundidad máxima alcanzada
        var maxDepth = pathDepths.Values.Max();
        Console.WriteLine($"Cells created: {totalCellsCarved} Paths created: {pathsCreated} Max depth: {maxDepth}");
        return totalCellsCarved;
    }

    private readonly List<Vector2I> _availableDirections = new(4);

    private List<Vector2I> GetAvailableDirections(Vector2I currentCell) {
        _availableDirections.Clear();
        foreach (var dir in Array2D.Directions) {
            var target = currentCell + dir * 2;
            if (IsValidPosition(target) && !IsCarved(target)) {
                _availableDirections.Add(dir);
            }
        }
        return _availableDirections;
    }

    private bool IsValidPosition(Vector2I position) {
        return Geometry.IsPointInRectangle(position.X, position.Y, 0, 0, Width, Height);
    }

    private void CarveCell(Vector2I position) {
        OnCarve?.Invoke(position);
    }

    /// <summary>
    /// Creates a MazeCarver for a boolean grid.
    /// </summary>
    /// <param name="grid">The grid to carve the maze in.</param>
    /// <returns>A new MazeCarver instance configured for the boolean grid.</returns>
    /// <remarks>
    /// - true represents carved cells
    /// - false represents uncarved cells
    /// </remarks>
    public static MazeCarver Create(Array2D<bool> grid) {
        ArgumentNullException.ThrowIfNull(grid);
        return Create(grid, true, false);
    }

    /// <summary>
    /// Creates a MazeCarver for a boolean BitArray2D.
    /// </summary>
    /// <param name="grid">The bit array grid to carve the maze in.</param>
    /// <returns>A new MazeCarver instance configured for the bit array grid.</returns>
    /// <remarks>
    /// - true represents carved cells
    /// - false represents uncarved cells
    /// </remarks>
    public static MazeCarver Create(BitArray2D grid) {
        ArgumentNullException.ThrowIfNull(grid);
        return new MazeCarver(grid.Width, grid.Height, pos => grid[pos], pos => grid[pos] = true);
    }

    /// <summary>
    /// Creates a MazeCarver for a grid of any type.
    /// </summary>
    /// <typeparam name="T">The type of elements in the grid.</typeparam>
    /// <param name="grid">The grid to carve the maze in.</param>
    /// <param name="filled">The value representing a carved cell.</param>
    /// <param name="empty">The value representing an uncarved cell.</param>
    /// <returns>A new MazeCarver instance configured for the specified grid type.</returns>
    /// <remarks>
    /// - Carves by setting the cell to filled value
    /// - Only carves in cells that match the empty value
    /// </remarks>
    public static MazeCarver Create<T>(Array2D<T> grid, T filled, T empty) {
        ArgumentNullException.ThrowIfNull(grid);
        return new MazeCarver(
            grid.Width,
            grid.Height,
            pos => !Equals(grid[pos], empty),
            pos => grid[pos] = filled
        );
    }
}