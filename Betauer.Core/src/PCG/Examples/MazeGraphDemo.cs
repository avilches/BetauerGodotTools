using System;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class MazeGraphDemo {
    public static void Main() {
        var seed = 3;
        var rng = new Random(seed);

        const int width = 41, height = 21;
        var template = new BitArray2D(width, height, true);
        var mc = MazeGraph.Create(template);
        var start = new Vector2I(4, 4);
        mc.OnCreateNode += (i) => {
            // PrintGraph(mc);
        };

        var constraints = MazeConstraints.CreateClockwiseBias(1)
            .With(c => {
                // c.MaxTotalCells = 141;
                c.MaxCellsPerPath = 10;
                c.MaxPaths = 3;
            });

        mc.Grow(start, constraints);
        PrintGraph(mc);
    }

    private static void PrintGraph(MazeGraph mc) {
        var canvas = new TextCanvas();
        foreach (var node in mc.NodeGrid) {
            if (node.Value == null) continue;
            var nodeCanvas = new TextCanvas();
            nodeCanvas.Write(1, 1, "+");
            if (node.Value.Up != null) nodeCanvas.Write(1, 0, "|");
            if (node.Value.Right != null) nodeCanvas.Write(2, 1, "-");
            if (node.Value.Down != null) nodeCanvas.Write(1, 2, "|");
            if (node.Value.Left != null) nodeCanvas.Write(0, 1, "-");

            canvas.Write(node.Position.X * 3, node.Position.Y * 3, nodeCanvas.ToString());
        }
        Console.WriteLine(canvas.ToString());
    }
}