using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.DataMath.Maze.ExampleDart;

public class MazeDungeon {
    private static readonly Vector2I[] Directions = { Vector2I.Up, Vector2I.Down, Vector2I.Right, Vector2I.Left };
    public static Random rng = new Random(0);

    public int ExtraConnectorChance => 20;
    public Array2D<Cell> Stage;
    public Rect2I Bounds;

    public MazeDungeon(int width, int height) {
        if (width % 2 == 0 || height % 2 == 0) {
            throw new ArgumentException("The stage must be odd-sized.");
        }
        Stage = new Array2D<Cell>(width, height);
        Bounds = new Rect2I(0, 0, width, height);
        Fill(TileType.Wall, -1);
    }


    public void Fill(TileType tile, int region) {
        for (int y = 0; y < Stage.Height; y++) {
            for (int x = 0; x < Stage.Width; x++) {
                Stage[x, y] = new Cell(x, y, tile, 0);
            }
        }
    }

    // public int LastRegion = -1;

    /// <summary>
    /// Generate a maze filling the stage with winding paths
    /// </summary>
    /// <param name="windyRatio">0 means straight lines, 1 means winding paths... 0.7f means 70% winding paths, 30% change straight line</param>
    public int FillMazes(float windyRatio, int startRegion) {
        var mazes = 0;
        for (var y = 1; y < Stage.Height; y += 2) {
            for (var x = 1; x < Stage.Width; x += 2) {
                var pos = new Vector2I(x, y);
                if (Stage[x, y].Type != TileType.Wall) continue;
                GrowMaze(pos, windyRatio, startRegion + mazes);
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
    /// <param name="section">The maze section, just a number to identify the mazo</param>
    public void GrowMaze(Vector2I start, float windyRatio, int section) {
        var cells = new Stack<Vector2I>();
        Vector2I? lastDir = null;

        Carve(start, TileType.OpenDoor, section);

        cells.Push(start);
        while (cells.Count > 0) {
            var cell = cells.Peek();
            var nextCellsAvailable = Directions.Where(dir => CanCarve(cell, dir)).ToList();
            if (nextCellsAvailable.Count == 0) {
                cells.Pop();
                lastDir = null;
                Carve(start, TileType.ClosedDoor, section);
                continue;
            }
            Vector2I dir = lastDir.HasValue && nextCellsAvailable.Contains(lastDir.Value) && rng.NextSingle() < windyRatio
                ? lastDir.Value // Windy path, keep the same direction
                : rng.Next(nextCellsAvailable); // Random path

            Carve(cell + dir, TileType.Path, section);
            Carve(cell + dir * 2, TileType.Path, section);
            cells.Push(cell + dir * 2);
            lastDir = dir;
        }
    }

    public void ConnectRegions(int totalRegions) {
        var connectorRegions = DetectConnectorRegions();
        AttemptToConnectRegions(connectorRegions, totalRegions);
    }

    private Dictionary<Vector2I, HashSet<int>> DetectConnectorRegions() {
        var connectorRegions = new Dictionary<Vector2I, HashSet<int>>();
        for (int y = 1; y < Stage.Height - 1; y++) {
            for (int x = 1; x < Stage.Width - 1; x++) {
                var pos = new Vector2I(x, y);
                if (Stage[pos].Type != TileType.Wall) continue;

                var regionsSet = new HashSet<int>();
                foreach (var dir in Directions) {
                    var neighborPos = pos + dir;
                    if (neighborPos.X >= 0 && neighborPos.X < Stage.Width && neighborPos.Y >= 0 && neighborPos.Y < Stage.Height) {
                        var region = Stage[neighborPos.X, neighborPos.Y].Region;
                        if (region != 0) regionsSet.Add(region);
                    }
                }

                if (regionsSet.Count < 2) continue;

                connectorRegions[pos] = regionsSet;
            }
        }
        return connectorRegions;
    }

    private void AttemptToConnectRegions(Dictionary<Vector2I, HashSet<int>> connectorRegions, int totalRegions) {
        var connectors = new List<Vector2I>(connectorRegions.Keys);
        if (connectors.Count == 0) return; // Add this check to prevent out of range exception

        var merged = new Dictionary<int, int>();
        var openRegions = new HashSet<int>();
        for (int i = 0; i <= totalRegions; i++) {
            merged[i] = i;
            openRegions.Add(i);
        }

        while (openRegions.Count > 1 && connectors.Count > 0) {
            var index = rng.Next(connectors.Count);
            var connector = connectors[index];

            AddJunction(connector);

            var regions = new HashSet<int>(connectorRegions[connector].Select(region => merged[region]));
            int dest = regions.First();
            var sources = regions.Skip(1).ToList();

            for (int i = 0; i <= totalRegions; i++) {
                if (sources.Contains(merged[i])) {
                    merged[i] = dest;
                }
            }

            openRegions.ExceptWith(sources);

            connectors.RemoveAll(pos => {
                if ((connector - pos).Length() < 2) return true;

                var regions = new HashSet<int>(connectorRegions[pos].Select(region => merged[region]));
                if (regions.Count > 1) return false;

                if (rng.Next(100) < ExtraConnectorChance) AddJunction(pos);

                return true;
            });
        }
    }

    public void RemoveDeadEnds() {
        bool done;
        do {
            done = true;
            for (int y = 1; y < Stage.Height - 1; y++) {
                for (int x = 1; x < Stage.Width - 1; x++) {
                    var pos = new Vector2I(x, y);
                    if (Stage[pos].Type == TileType.Wall) continue;

                    int exits = 0;
                    foreach (var dir in Directions) {
                        if (Stage[pos + dir].Type != TileType.Wall) exits++;
                    }

                    if (exits == 1) {
                        done = false;
                        Stage[pos].Type = TileType.Wall;
                    }
                }
            }
        } while (!done);
    }

    private bool CanCarve(Vector2I pos, Vector2I direction) {
        return Geometry.Geometry.IsPointInRectangle(pos + direction * 3, Bounds) && 
               Stage[pos + direction * 2].Type == TileType.Wall;
    }


    private void AddJunction(Vector2I pos) {
        if (rng.Next(4) == 0) {
            Stage[pos].Type = rng.Next(3) == 0 ? TileType.OpenDoor : TileType.Floor;
        } else {
            Stage[pos].Type = TileType.ClosedDoor;
        }
        // stage[pos] = TileType.ClosedDoor;
    }

    
    public void Carve(Rect2I rect, TileType type, int region) {
        foreach (var pos in Geometry.Geometry.GetEnumerator(rect)) {
            // start regions from 1, because 0 is reserved for walls (the whole grid is filled with walls in the beginning)
            try {
                Carve(pos, TileType.Floor, region);
            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
    }
        
    public void Carve(Vector2I pos, TileType type, int region) {
        Stage[pos].Type = type;
        Stage[pos].Region = region;
    }
}
