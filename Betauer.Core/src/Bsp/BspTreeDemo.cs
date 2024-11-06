using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Collision;
using Betauer.Core.Collision.Spatial2D;
using Betauer.Core.Image;
using Godot;
using FastNoiseLite = Betauer.Core.Data.FastNoiseLite;

namespace Betauer.Core.Bsp;

public class BspTreeDemo {
    public static void Main() {
        var random = new Random(4);
        const int padding = 4;
        const float shrink = 0.5f;
        const int width = 180;
        const int height = 130;

        var bsp = new BspTree {
            Retries = 10,
            Width = width,
            Height = height,
            MinRoomWidth = 12,
            MinRoomHeight = 12,
            MaxRatio = 16f / 9,
            CreateRoom = (x, y, width, height) => {
                // return new Rect2I(x, y, width, height);
                var newRect = Geometry.ShrinkRectProportional(x, y, width, height, random.Range(shrink, 1f));
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
            Stop = (node, depth) => depth >= 5 && random.NextDouble() < 0.2 // Stop after 5 divisions with 20% chance
        };
        bsp.Generate();


        var rooms = bsp.GetRooms();
        
        // rooms = ExpandRooms(rooms);

        var map = CreateMap(width, height);
        CarveRooms(width, height, rooms, map);
        Stats(rooms, bsp.MaxRatio);
        CarveCorridors(width, height, rooms, map, random);
        PrintMap(width, height, map);
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
                var sizeText = $"{rect.Size.X}/{rect.Size.Y}({((double)Math.Max(rect.Size.X, rect.Size.Y) / Math.Min(rect.Size.X, rect.Size.Y)):0.0})";
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
            var rect = new Rect2I(center, Vector2I.Zero).Grow(3);
            return random.Next(rect);
        }).ToList();

        var a = Geometry.GetConnections(centers);
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
        if (showCenters) centers.ForEach(v => { map[v.X, v.Y] = '*'; });
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
                Console.Write(map[x, y]);
            }
            Console.WriteLine();
        }
    }

    private static void Stats(IList<Rect2I> rooms, float bspMaxRatio) {
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
            var ratio = (float)Math.Max(width, height) / Math.Min(width, height);
            ratioSum += ratio;
            minRatio = Math.Min(minRatio, ratio);
            maxRatio = Math.Max(maxRatio, ratio);
            minHeight = Math.Min(minHeight, Math.Min(width, height));
            minWidth = Math.Min(minWidth, Math.Max(width, height));
            if (width < height) verticals++;
            else horizontals++;
            // Console.WriteLine(width + " / " + height + " = " + ratio.ToString("0.00"));
        });
        Console.WriteLine($"Average ratio: {(ratioSum / (verticals + horizontals)):0.00}");
        Console.WriteLine($"Min/max Ratio: {minRatio:0.00}/{maxRatio:0.00} (ratio limit was {bspMaxRatio})");
        Console.WriteLine("Verticals: " + verticals + "/" + rooms.Count);
        Console.WriteLine("Horizontals: " + horizontals + "/" + rooms.Count);
        Console.WriteLine("Squares: " + (square) + "/" + rooms.Count);
        Console.WriteLine("Min Height: " + minHeight);
        Console.WriteLine("Min Width: " + minWidth);
    }

    public static void Main2() {
        ValidateDivision(5, 4, 0.55, 0.4, 0.6);
        ValidateDivision(4, 5, 0.45, 0.4, 0.6);
        ValidateDivision(4, 5, 0.45, 0.1, 0.9);
        ValidateDivision(4, 5, 0.45, 0.45, 0.55);
        ValidateDivision(4, 5, 0.45, 0.49, 0.6);
    }

    public static bool HasRatioOrLess(double width, double height, double maxRatio) {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive numbers.");

        // Current ratio as long side / short side, it will return 1 or more, like 1.7777 for a 16:9 ratio
        var ratio = Math.Max(width, height) / Math.Min(width, height);

        // If ratio sent is in the range of 0 to 1, we need to invert it. So, a ratio of 0.33 (1/3) will be 3 (3:1)
        var ratioLimit = maxRatio < 1 ? 1.0 / maxRatio : maxRatio;

        // True if current ratio is less or equal to the limit ratio
        return ratio <= ratioLimit;
    }

    public static bool ValidateDivision(int width, int height, double minRatio, double minRandom, double maxRandom) {
        // Determina cuál es el lado más largo y el lado más corto del rectángulo
        var longSide = Math.Max(width, height);
        var shortSide = Math.Min(width, height);

        var lastSplitPoint = -1;
        // Recorre el rango aleatorio del 40% al 60% del lado más largo
        for (var percent = minRandom; percent <= maxRandom; percent += 0.01) {
            // Calcula la posición de división dentro del rango
            var splitPoint = (int)(longSide * percent);
            if (lastSplitPoint != -1 && lastSplitPoint == splitPoint) continue;
            lastSplitPoint = splitPoint;

            // Calcula las dimensiones de los dos subrectángulos
            int part1Long = splitPoint;
            int part2Long = longSide - splitPoint;
            if (part1Long < 1 || part2Long < 1) continue;

            // Calcula los ratios de ambas partes
            double ratioPart1 = (double)part1Long / shortSide;
            double ratioPart2 = (double)part2Long / shortSide;

            Console.WriteLine("Part 1: " + part1Long + "/" + shortSide + " ratio " + ratioPart1 + " HasRatioOrLess " + HasRatioOrLess(part1Long, shortSide, minRatio));
            Console.WriteLine("Part 2: " + part2Long + "/" + shortSide + " ratio " + ratioPart2 + " HasRatioOrLess " + HasRatioOrLess(part2Long, shortSide, minRatio));

            // Verifica si ambos ratios cumplen con el mínimo requerido
            if (ratioPart1 >= minRatio && ratioPart2 >= minRatio) {
                Console.WriteLine(":) Validating division for " + width + "/" + height + " ratio " + minRatio + " random " + minRandom + "-" + maxRandom + " is possible at " + percent);
                return true;
            }
        }

        // Si no encontró una división válida, devuelve false
        Console.WriteLine("!!!!!!! Validating division for " + width + "/" + height + " ratio " + minRatio + " random " + minRandom + "-" + maxRandom + " is not possible");
        return false;
    }
}