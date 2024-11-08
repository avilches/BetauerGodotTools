using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Image;
using Betauer.Core.Math.Collision;
using Betauer.Core.Math.Collision.Spatial2D;
using Godot;
using FastNoiseLite = Betauer.Core.Math.Data.FastNoiseLite;

namespace Betauer.Core.Math.Bsp;

public class BspTreeDemo {
    public static void Main() {
        var random = new Random(4);
        const int padding = 4;
        const float shrink = 0.5f;
        const int width = 140;
        const int height = 130;

        var bsp = new BspTree {
            Retries = 30,
            Width = width,
            Height = height,
            MinRoomWidth = 6,
            MinRoomHeight = 6,
            MaxRatio = 3f/7,
            CreateRoom = (x, y, width, height) => {
                // return new Rect2I(x, y, width, height);
                var r = Geometry.Geometry.ShrinkRectToEnsureRatio(x, y, width, height, 3f/7);
                var newRect = Geometry.Geometry.ShrinkRectProportional(r, random.Range(shrink, 1f));
                var offsetX = width - newRect.Size.X;
                var offsetY = height - newRect.Size.Y;
                return new Rect2I(new Vector2I(x + random.Next(0, offsetX + 1), y + random.Next(0, offsetY + 1)), newRect.Size);
            },
            // CreateRoom = (x, y, width, height) => Geometry.ShrinkRect(x, y, width, height, 1),
            // CreateRoom = (x, y, width, height) => Geometry.ShrinkRect(x, y, width, height, 1),
            // CreateRoom = (x, y, width, height) => new Rect2I(x, y, width, height),
            Random = random,
            Splitter = (node, i) => (
                horizontal: node.Height == node.Width ? random.NextBool() : node.Height > node.Width, // Split horizontally if height is greater than width.
                splitBy: random.Range(0.4f, 0.6f)), // Split using a value 40% to 60% of the size.
            Stop = (node, depth) => depth >= 7 && random.NextDouble() < 0.5 // Stop after 5 divisions with 20% chance
        };
        bsp.Generate();


        var rooms = bsp.Rooms;
        
        // rooms = ExpandRooms(rooms);

        RemoveBiggerRooms(rooms, 0.2f);


        var map = CreateMap(width, height);
        CarveRooms(width, height, rooms, map);
        Stats(rooms, bsp.MaxRatio, bsp.MinRoomWidth, bsp.MinRoomHeight);
        CarveCorridors(width, height, rooms, map, random, true, true);
        PrintMap(width, height, map);
    }

    private static void RemoveBiggerRooms(List<Rect2I> rooms, float percent) {
        rooms.Sort((a, b) => a.Area);
        rooms.RemoveRange(0, Mathf.RoundToInt(rooms.Count * percent));
    }


    private static List<Rect2I> ExpandRooms(List<Rect2I> rooms) {
        // Use this code to expand the rooms using Spatial
        
        var sp = new SpatialGrid(10);
        sp.AddAll(rooms.Select(r => new Rectangle(r)));
        var rectangles = sp.FindShapes<Rectangle>().Cast<Shape>().ToList();
        while (rectangles.Count > 0) {
            // Console.WriteLine("Expanding... "+rectangles.Count);
            sp.ExpandAll(rectangles, 1f);
        }
        return sp.FindShapes<Rectangle>().Select(shape => shape.ToRect2I()).ToList();
    }

    private static void CarveRooms(int width, int height, IList<Rect2I> rooms, char[,] map, bool showSize = false) {
        rooms.ForEach(rect => {
            for (var x = rect.Position.X; x < rect.Position.X + rect.Size.X; x++) {
                for (var y = rect.Position.Y; y < rect.Position.Y + rect.Size.Y; y++) {
                    if (x >= 0 && x < width && y >= 0 && y < height)
                        map[x, y] = ' ';
                }
            }
            if (showSize) {
                var sizeText = $"{rect.Size.X}/{rect.Size.Y}({((double)System.Math.Max(rect.Size.X, rect.Size.Y) / System.Math.Min(rect.Size.X, rect.Size.Y)):0.0})";
                for (var i = 0; i < sizeText.Length; i++) {
                    if (rect.Position.X + i >= 0 && rect.Position.X + i < width && rect.Position.Y >= 0 && rect.Position.Y < height)
                        map[rect.Position.X + i, rect.Position.Y] = sizeText[i];
                }
            }
        });
    }

    private static void CarveCorridors(int width, int height, IList<Rect2I> rooms, char[,] map, Random random, bool showPaths = false, bool showCenters = false) {
        var centers = rooms.Select(r => {
            var center = r.GetCenter();
            if (r.Size.X <= 3 || r.Size.Y <= 3) return center;
            if (r.Size.X < 8 || r.Size.Y < 8) return random.Next(r);
            var rect = new Rect2I(center, Vector2I.Zero).Grow(3); // grow by 3 means a 6x6 square
            return random.Next(rect);
        }).ToList();

        var a = PrimMST.GetConnections(centers);
        var cc = showPaths ? '*' : ' ';
        var f = new FastNoiseLite();
        a.ForEach((connection, v2) => {
                var start = connection.Item1;
                var end = connection.Item2;
                var roadType = random.Next(5);
                if (roadType == 0) {
                    Draw.Line(start.X, start.Y, end.X, end.Y, 1, (x, y) => map[x, y] = cc);
                } else if (roadType == 1) {
                    Draw.LineOneTurn(start.X, start.Y, end.X, end.Y, (x, y) => map[x, y] = cc, random.NextBool());
                } else if (roadType == 2) {
                    Draw.LineTwoTurns(start.X, start.Y, end.X, end.Y, (x, y) => {
                        try {
                            map[x, y] = cc;
                        } catch (Exception e) {
                            Console.WriteLine(e);
                            throw;
                        }
                    }, random.NextBool());
                } else if (roadType == 3) {
                    Draw.LineRandom(start.X, start.Y, end.X, end.Y, (x, y) => map[x, y] = cc, 1, new Random(0));
                } else if (roadType == 4) {
                    Draw.LineNoise(start.X, start.Y, end.X, end.Y, f, 1, (x, y, f) => {
                        if (x > 0 && x < width && y > 0 && y < height) map[x, y] = cc;
                    });
                }
                //cc++;
            });
        if (showCenters) centers.ForEach(v => { map[v.X, v.Y] = '+'; });
    }

    private static char[,] CreateMap(int width, int height) {
        var map = new char[width, height];
        for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                map[x, y] = '#';
        return map;
    }

    private static void PrintMap(int width, int height, char[,] map) {
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                Console.Write(map[x, y]+""+map[x, y]);
            }
            Console.WriteLine();
        }
    }

    private static void Stats(IList<Rect2I> rooms, float bspMaxRatio, int bspMinWidth, int bspMinHeight) {
        var ratioSum = 0f;
        var verticals = 0;
        var horizontals = 0;
        var minHeight = int.MaxValue;
        var minWidth = int.MaxValue;
        var minRatio = float.MaxValue;
        var maxRatio = float.MinValue;
        var square = 0;
        rooms.ForEach(room => {
            var width = room.Size.X;
            var height = room.Size.Y;
            if (width == height) {
                square++;
                return;
            }
            var ratio = (float)System.Math.Max(width, height) / System.Math.Min(width, height);
            ratioSum += ratio;
            minRatio = System.Math.Min(minRatio, ratio);
            maxRatio = System.Math.Max(maxRatio, ratio);
            minHeight = System.Math.Min(minHeight, System.Math.Min(width, height));
            minWidth = System.Math.Min(minWidth, System.Math.Max(width, height));
            if (width < height) verticals++;
            else horizontals++;
            // Console.WriteLine(width + " / " + height + " = " + ratio.ToString("0.00"));
        });
        Console.WriteLine($"Average ratio: {(ratioSum / (verticals + horizontals)):0.00}");
        Console.WriteLine($"Min/max Ratio: {minRatio:0.00}/{maxRatio:0.00} (BPS ratio limit was {bspMaxRatio:0.00})");
        Console.WriteLine("H/V/square: " + horizontals+"/"+verticals + "/" + square+" (total "+rooms.Count+")");
        Console.WriteLine("Min Width: " + minWidth+ " (BPS min width was: "+bspMinWidth+")");
        Console.WriteLine("Min Height: " + minHeight+ " (BPS min height was: "+bspMinHeight+")");
    }
}