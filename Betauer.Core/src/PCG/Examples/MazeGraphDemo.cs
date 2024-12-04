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


        var mc = new MazeGraph(8, 8);
        var start = new Vector2I(0, 0);
        mc.OnNodeConnected += (i) => {
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
        var mc = MazeGraph.Create(template);
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

    private static void PrintGraph(MazeGraph mc) {
        var allCanvas = new TextCanvas();
        foreach (var dataCell in mc.NodeGrid) {
            var node = dataCell.Value;
            if (node == null) continue;
            var canvas = new TextCanvas();
            canvas.Write(1, 1, node.Id.ToString());
            if (node.Up != null) canvas.Write(1, 0, "|");
            if (node.Right != null) canvas.Write(2, 1, "--");
            if (node.Down != null) canvas.Write(1, 2, "|");
            if (node.Left != null) canvas.Write(0, 1, "-");

            allCanvas.Write(dataCell.Position.X * 4, dataCell.Position.Y * 3, canvas.ToString());
        }
        Console.WriteLine(allCanvas.ToString());
    }

    private static void PrintGraphAsCarved(MazeGraph mc) {
        var allCanvas = new TextCanvas();
        foreach (var dataCell in mc.NodeGrid) {
            var canvas = new TextCanvas("""
                                        ██
                                        ██
                                        """);
            var node = dataCell.Value;
            if (node != null) {
                canvas.Write(1, 1, " "); // node.Metadata?.ToString());
                if (node.Up != null) canvas.Write(1, 0, " ");
                if (node.Left != null) canvas.Write(0, 1, " ");
            }
            allCanvas.Write(dataCell.Position.X * 2, dataCell.Position.Y * 2, canvas.ToString());
        }
        Console.WriteLine(allCanvas.ToString());
    }
}