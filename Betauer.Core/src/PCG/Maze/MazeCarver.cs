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
    
    private readonly List<Vector2I> _availableDirections = new(4);

    public List<Vector2I> GetAvailableDirections(Vector2I currentCell) {
        _availableDirections.Clear();
        foreach (var dir in Array2D.VonNeumannDirections) {
            var target = currentCell + dir * 2;
            if (IsValidPosition(target) && !IsCarved(target)) {
                _availableDirections.Add(dir);
            }
        }
        return _availableDirections;
    }

    public bool IsValidPosition(Vector2I position) {
        return Geometry.IsPointInRectangle(position.X, position.Y, 0, 0, Width, Height);
    }

    public void CarveCell(Vector2I position) {
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