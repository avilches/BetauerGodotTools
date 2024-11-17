using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.DataMath.Maze;

public enum MazeCarveType { Start, Path, End, Empty }

public abstract class BaseMazeCarver {
    protected static readonly Vector2I[] Directions = { Vector2I.Up, Vector2I.Down, Vector2I.Right, Vector2I.Left };

    public int Width { get; }
    public int Height { get; }
    
    protected BaseMazeCarver(int width, int height) {
        Width = width;
        Height = height;
    }

    public int FillMazes(float windyRatio, int startRegion, Random rng) {
        var mazes = 0;
        for (var y = 1; y < Height; y += 2) {
            for (var x = 1; x < Width; x += 2) {
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
    public int GrowMaze(Vector2I start, float windyRatio, int label, Random rng) {
        if (IsCarved(start)) return 0;
        
        var cells = new Stack<Vector2I>();
        Vector2I? lastDir = null;

        Carve(start, MazeCarveType.Start, label);
        var size = 1;

        cells.Push(start);
        while (cells.Count > 0) {
            var cell = cells.Peek();
            var nextCellsAvailable = Directions.Where(dir => CanCarve(cell, dir)).ToList();
            if (nextCellsAvailable.Count == 0) {
                cells.Pop();
                lastDir = null;
                Carve(start, MazeCarveType.End, label);
                continue;
            }
            Vector2I dir = lastDir.HasValue && nextCellsAvailable.Contains(lastDir.Value) && rng.NextSingle() < windyRatio
                ? lastDir.Value // Windy path, keep the same direction
                : rng.Next(nextCellsAvailable); // Random path

            Carve(cell + dir, MazeCarveType.Path, label);
            Carve(cell + dir * 2, MazeCarveType.Path, label);
            cells.Push(cell + dir * 2);
            lastDir = dir;
            size += 2;
        }
        return size;
    }

    public void RemoveDeadEnds() {
        bool done;
        do {
            done = true;
            for (var y = 1; y < Height - 1; y++) {
                for (var x = 1; x < Width - 1; x++) {
                    var pos = new Vector2I(x, y);
                    if (!IsCarved(pos)) continue; // Ignore walls, we only want to remove dead ends carved paths
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

    public abstract bool IsCarved(Vector2I pos);

    public abstract void UnCarve(Vector2I pos);

    public abstract void Carve(Vector2I pos, MazeCarveType start, int region);

    public abstract bool CanCarve(Vector2I pos, Vector2I direction);
}