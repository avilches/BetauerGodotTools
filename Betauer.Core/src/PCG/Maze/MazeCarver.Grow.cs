using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.PCG.Maze;

public partial class MazeCarver {
    /// <summary>
    /// Generates a maze starting from a specific cell, following the given constraints.
    /// </summary>
    /// <param name="start">The starting position for maze generation.</param>
    /// <param name="constraints">Constraints that control the maze generation process.</param>
    /// <param name="backtracker">A function to locate the next cell to backtrack. By default, it takes the last one (LIFO)</param>
    /// <returns>The number of cells carved during maze generation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when constraints is null.</exception>
    public void Grow(Vector2I start, BacktrackConstraints constraints, Func<List<Vector2I>, Vector2I>? backtracker = null) {
        ArgumentNullException.ThrowIfNull(constraints);
        if (IsCarved(start)) return;
        var maxTotalCells = constraints.MaxTotalCells == -1 ? int.MaxValue : constraints.MaxTotalCells;
        var maxCellsPerPath = constraints.MaxCellsPerPath == -1 ? int.MaxValue : constraints.MaxCellsPerPath;
        var maxTotalPaths = constraints.MaxPaths == -1 ? int.MaxValue : constraints.MaxPaths;
        if (maxTotalCells == 0 || maxCellsPerPath == 0 || maxTotalPaths == 0) return;

        var carvedCells = new List<Vector2I>();
        Vector2I? lastDirection = null;
        var pathsCreated = 0;
        var totalCellsCarved = 1;
        var cellsCarvedInCurrentPath = 1;

        CarveCell(start);
        carvedCells.Add(start);
        var currentCell = start;

        while (carvedCells.Count > 0) {
            var availableDirections = GetAvailableDirections(currentCell);

            if (availableDirections.Count == 0 || cellsCarvedInCurrentPath >= maxCellsPerPath || totalCellsCarved == maxTotalCells) {
                // path stopped, backtracking
                carvedCells.Remove(currentCell);
                if (carvedCells.Count > 0) {
                    currentCell = backtracker?.Invoke(carvedCells) ?? carvedCells[carvedCells.Count - 1];
                    if (GetAvailableDirections(currentCell).Count == 0) {
                        continue;
                    }
                }
                // No more nodes to backtrack (end of the path and the maze) or next node has no available directions (end of the path)
                pathsCreated++;
                // Console.WriteLine($"Path #{pathsCreated} finished: Cells: {cellsCarvedInCurrentPath}");
                if (pathsCreated == maxTotalPaths || totalCellsCarved == maxTotalCells) break;
                cellsCarvedInCurrentPath = 1;
                lastDirection = null;
                continue;
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

            carvedCells.Add(nextCellToCarve);
            totalCellsCarved += 2;
            cellsCarvedInCurrentPath += 2;
            currentCell = nextCellToCarve;
        }

        // Console.WriteLine($"Cells created: {totalCellsCarved} Paths created: {pathsCreated}");
    }

    public void GrowRandom(Vector2I start, int maxTotalCells = -1, Random? rng = null) {
        if (maxTotalCells == 0) return;
        if (IsCarved(start)) return;
        maxTotalCells = maxTotalCells == -1 ? int.MaxValue : maxTotalCells;

        rng ??= new Random();
        var pendingCells = new List<Vector2I>();
        Vector2I? lastDirection = null;
        var totalCellsCarved = 1;

        CarveCell(start);
        pendingCells.Add(start);

        while (pendingCells.Count > 0 && totalCellsCarved < maxTotalCells) {
            var currentCell = rng.Next(pendingCells);
            var availableDirections = GetAvailableDirections(currentCell);
            if (availableDirections.Count == 0) {
                pendingCells.Remove(currentCell);
            } else {
                var nextDirection = rng.Next(availableDirections);
                var intermediateCell = currentCell + nextDirection;
                var nextCellToCarve = currentCell + nextDirection * 2;
                CarveCell(intermediateCell);
                CarveCell(nextCellToCarve);
                pendingCells.Add(nextCellToCarve);
                totalCellsCarved += 2;
            }
        }
        // Console.WriteLine($"Cells created: {totalCellsCarved} Paths created: {pathsCreated}");
    }
}