using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class MazeGraphDemo {
    public static void Main() {
        var seed = 3;
        var rng = new Random(seed);


        var mc = new MazeGraph<object>(8, 8);
        var start = new Vector2I(0, 0);
        mc.OnEdgeCreated += (i) => {
            // PrintGraph(mc);
        };

        var constraints = BacktrackConstraints.CreateRandom(rng)
            .With(c => {
                c.MaxTotalCells = 25;
                c.MaxCellsPerPath = 3;
                c.MaxPaths = 13;
            });

        mc.Grow(start, constraints);
        PrintGraph(mc);
    }

    public static void MainOld() {
        var seed = 3;
        var rng = new Random(seed);

        const int width = 41, height = 21;
        var template = new BitArray2D(width, height, true);
        var mc = MazeGraph.Create<object>(template);
        var start = new Vector2I(4, 4);
        mc.OnNodeCreated += (i) => {
            // PrintGraphAsCarved(mc);
        };

        var constraints = BacktrackConstraints.CreateCounterClockwiseBias(0.5f, rng)
            .With(c => {
                // c.MaxTotalCells = 141;
                c.MaxCellsPerPath = 10;
                c.MaxPaths = 3;
            });

        mc.Grow(start, constraints);
        PrintGraphAsCarved(mc);
    }

    private static void PrintGraph<T>(MazeGraph<T> mc) {
        var allCanvas = new TextCanvas();
        foreach (var node in mc.GetNodes()) {
            if (node == null) continue;
            var canvas = new TextCanvas();
            canvas.Write(1, 1, node.Id.ToString());
            if (node.Up != null) canvas.Write(1, 0, "|");
            if (node.Right != null) canvas.Write(2, 1, "--");
            if (node.Down != null) canvas.Write(1, 2, "|");
            if (node.Left != null) canvas.Write(0, 1, "-");

            allCanvas.Write(node.Position.X * 4, node.Position.Y * 3, canvas.ToString());
        }
        Console.WriteLine(allCanvas.ToString());
    }

    private static void PrintGraphAsCarved<T>(MazeGraph<T> mc) {
        var allCanvas = new TextCanvas();
        foreach (var node in mc.GetNodes()) {
            var canvas = new TextCanvas("""
                                        ██
                                        ██
                                        """);
            if (node != null) {
                canvas.Write(1, 1, " "); // node.Metadata?.ToString());
                if (node.Up != null) canvas.Write(1, 0, " ");
                if (node.Left != null) canvas.Write(0, 1, " ");
            }
            allCanvas.Write(node.Position.X * 2, node.Position.Y * 2, canvas.ToString());
        }
        Console.WriteLine(allCanvas.ToString());
    }
}