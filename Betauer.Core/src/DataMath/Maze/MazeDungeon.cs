using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath.Data;
using Godot;

namespace Betauer.Core.DataMath.Bsp.Dart;

public class MazeDungeon {
    public static IEnumerable<Vector2I> GetEnumerator(Rect2I rect) {
        for (var x = rect.Position.X; x < rect.End.X; x++) {
            for (var y = rect.Position.Y; y < rect.End.Y; y++) {
                yield return new Vector2I(x, y);
            }
        }
    }

    
    private static readonly Vector2I[] Directions = { Vector2I.Up, Vector2I.Down, Vector2I.Right, Vector2I.Left };
    public static Random rng = new Random(0);

    public XyDataGrid<Cell> Stage;
    public Rect2I Bounds;

    public MazeDungeon(int width, int height) {
        if (width % 2 == 0 || height % 2 == 0) {
            throw new ArgumentException("The stage must be odd-sized.");
        }
        Stage = new XyDataGrid<Cell>(width, height);
        Bounds = new Rect2I(0, 0, width, height);
        Fill(TileType.Wall);
        LastRegion = 0;
    }

    public int ExtraConnectorChance => 20;
    public int WindingPercent => 0;

    public void Fill(TileType tile) {
        for (int y = 0; y < Stage.Height; y++) {
            for (int x = 0; x < Stage.Width; x++) {
                Stage[new Vector2I(x, y)] = new Cell(x, y, tile, 0);
            }
        }
    }

    public int LastRegion = -1;

    public static List<Rect2I> AddRooms(int numRoomTries, int roomExtraSize, int boundsWidth, int boundsHeight) {
        List<Rect2I> Rooms = new List<Rect2I>();
        for (int i = 0; i < numRoomTries; i++) {
            int size = (rng.Next(1, 3 + roomExtraSize) * 2) + 1;
            int rectangularity = rng.Next(0, 1 + size / 2) * 2;
            int width = size;
            int height = size;
            if (rng.Next(2) == 0) {
                width += rectangularity;
            } else {
                height += rectangularity;
            }

            var x = rng.Next((boundsWidth - width) / 2) * 2 + 1;
            var y = rng.Next((boundsHeight - height) / 2) * 2 + 1;

            var room = new Rect2I(x, y, width, height);

            bool overlaps = false;
            foreach (var other in Rooms) {
                if (Geometry.Geometry.IntersectRectangles(other, room)) {
                    overlaps = true;
                    break;
                }
            }

            if (overlaps) continue;

            Rooms.Add(room);
        }
        return Rooms;
    }

    public void FillMazes() {
        for (int y = 1; y < Stage.Height; y += 2) {
            for (int x = 1; x < Stage.Width; x += 2) {
                var pos = new Vector2I(x, y);
                if (Stage[pos].Type != TileType.Wall) continue;
                LastRegion++;

                GrowMaze(pos);
            }
        }
    }
    
    private void GrowMaze(Vector2I start) {
        var cells = new List<Vector2I>();
        Vector2I? lastDir = null;

        Carve(start, TileType.Path, LastRegion);

        cells.Add(start);
        while (cells.Count > 0) {
            var cell = cells[cells.Count - 1];

            var unmadeCells = new List<Vector2I>();
            foreach (var dir in Directions) {
                if (CanCarve(cell, dir)) unmadeCells.Add(dir);
            }

            if (unmadeCells.Count > 0) {
                Vector2I dir;
                if (lastDir.HasValue && unmadeCells.Contains(lastDir.Value) && rng.Next(100) > WindingPercent) {
                    dir = lastDir.Value;
                } else {
                    dir = rng.Next(unmadeCells);
                }

                Carve(cell + dir, TileType.Path, LastRegion);
                Carve(cell + dir * 2, TileType.Path, LastRegion);

                cells.Add(cell + dir * 2);
                lastDir = dir;
            } else {
                Carve(cell, TileType.Path, LastRegion);
                cells.RemoveAt(cells.Count - 1);
                lastDir = null;
            }
        }
    }

    public void ConnectRegions() {
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

        var connectors = new List<Vector2I>(connectorRegions.Keys);
        if (connectors.Count == 0) return; // Add this check to prevent out of range exception

        var merged = new Dictionary<int, int>();
        var openRegions = new HashSet<int>();
        for (int i = 0; i <= LastRegion; i++) {
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

            for (int i = 0; i <= LastRegion; i++) {
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
        foreach (var pos in GetEnumerator(rect)) {
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