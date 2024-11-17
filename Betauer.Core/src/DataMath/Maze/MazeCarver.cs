using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath.Maze.ExampleDart;
using Godot;

namespace Betauer.Core.DataMath.Maze;

public class MazeCarver {
    public Array2D<bool> Grid { get; init; }
    private static readonly Vector2I[] Directions = { Vector2I.Up, Vector2I.Down, Vector2I.Right, Vector2I.Left };

    public MazeCarver(Array2D<bool> grid) {
        Grid = grid;
    }

    public int FillMazes(float windyRatio, int startRegion, Random rng) {
        var mazes = 0;
        for (var y = 1; y < Grid.Height; y += 2) {
            for (var x = 1; x < Grid.Width; x += 2) {
                var pos = new Vector2I(x, y);
                if (IsCarved(pos)) continue;
                GrowMaze(pos, windyRatio, startRegion + mazes, rng);
                mazes++;
            }
        }
        return mazes;
    }

    /// <summary>
    /// Generate a maze starting from a given position
    /// </summary>
    /// <param name="start"></param>
    /// <param name="windyRatio">0 means straight lines, 1 means winding paths... 0.7f means 70% winding paths, 30% change straight line</param>
    /// <param name="label">The maze section, just a number to identify the mazo</param>
    /// <param name="rng">The maze section, just a number to identify the mazo</param>
    public void GrowMaze(Vector2I start, float windyRatio, int label, Random rng) {
        var cells = new Stack<Vector2I>();
        Vector2I? lastDir = null;

        Carve(start, TileType.OpenDoor, label);

        cells.Push(start);
        while (cells.Count > 0) {
            var cell = cells.Peek();
            var nextCellsAvailable = Directions.Where(dir => CanCarve(cell, dir)).ToList();
            if (nextCellsAvailable.Count == 0) {
                cells.Pop();
                lastDir = null;
                Carve(start, TileType.ClosedDoor, label);
                continue;
            }
            Vector2I dir = lastDir.HasValue && nextCellsAvailable.Contains(lastDir.Value) && rng.NextSingle() < windyRatio
                ? lastDir.Value // Windy path, keep the same direction
                : rng.Next(nextCellsAvailable); // Random path

            Carve(cell + dir, TileType.Path, label);
            Carve(cell + dir * 2, TileType.Path, label);
            cells.Push(cell + dir * 2);
            lastDir = dir;
        }
    }

    public void RemoveDeadEnds() {
        bool done;
        do {
            done = true;
            for (int y = 1; y < Grid.Height - 1; y++) {
                for (int x = 1; x < Grid.Width - 1; x++) {
                    var pos = new Vector2I(x, y);
                    if (!IsCarved(pos)) continue;

                    var exits = 0;
                    foreach (var dir in Directions) {
                        if (IsCarved(pos + dir)) exits++;
                    }

                    if (exits == 1) {
                        done = false;
                        UnCarve(pos);
                    }
                }
            }
        } while (!done);
    }

    private bool IsCarved(Vector2I pos) {
        return Grid[pos];;
    }

    private void UnCarve(Vector2I pos) {
        var dataCells = Grid;
        dataCells[pos] = false;
    }

    public void Carve(Vector2I pos, TileType type, int region) {
        var dataCells = Grid;
        dataCells[pos] = true;
    }

    private bool CanCarve(Vector2I pos, Vector2I direction) {
        var vector2I = pos + direction * 3;
        return Geometry.Geometry.IsPointInRectangle(vector2I.X, vector2I.Y, 0f, 0f, Grid.Width, Grid.Height) &&
               !IsCarved(pos + direction * 2);
    }
}