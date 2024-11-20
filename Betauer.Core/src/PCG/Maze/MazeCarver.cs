using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Betauer.Core.PCG.Maze;

public enum MazeCarveType { Start, Path, End, Empty }

public class MazeCarver(int width, int height) {
    protected static readonly Vector2I[] Directions = [Vector2I.Up, Vector2I.Down, Vector2I.Right, Vector2I.Left];

    public int Width { get; } = width;
    public int Height { get; } = height;

    // Func delegates for customizable behavior
    public Func<Vector2I, bool>? IsCarvedFunc { get; set; }
    public Action<Vector2I>? UnCarveAction { get; set; }
    public Action<Vector2I, MazeCarveType>? CarveAction { get; set; }
    public Func<Vector2I, Vector2I, bool>? CanCarveFunc { get; set; }

    public int FillMazes(float windyRatio, Random rng) {
        var mazes = 0;
        for (var y = 1; y < Height; y += 2) {
            for (var x = 1; x < Width; x += 2) {
                var pos = new Vector2I(x, y);
                if (IsCarved(pos)) continue;
                GrowMaze(pos, windyRatio, mazes, rng);
                mazes++;
            }
        }
        return mazes;
    }

    public int GrowMaze(Vector2I start, float windyRatio, int label, Random rng) {
        if (IsCarved(start)) return 0;

        var cells = new Stack<Vector2I>();
        Vector2I? lastDir = null;
        Vector2I lastPos = start;

        Carve(start, MazeCarveType.Start);
        var size = 1;

        cells.Push(start);
        while (true) {
            var cell = cells.Peek();
            var nextCellsAvailable = Directions.Where(dir => CanCarve(cell, dir)).ToList();
            if (nextCellsAvailable.Count == 0) {
                cells.Pop();
                if (cells.Count == 0) {
                    break;
                }
                lastDir = null;
                continue;
            }
            Vector2I dir = lastDir.HasValue && nextCellsAvailable.Contains(lastDir.Value) && rng.NextSingle() < windyRatio
                ? lastDir.Value
                : rng.Next(nextCellsAvailable);
            lastDir = dir;

            Carve(cell + dir, MazeCarveType.Path);
            Carve(cell + dir * 2, MazeCarveType.Path);
            lastPos = cell + dir * 2;
            cells.Push(lastPos);
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
        if (CanCarveFunc== null) throw new InvalidOperationException($"{nameof(CanCarveFunc)} not set");
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
