using System;
using System.Text;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class MazeDemo {
    public static void Main() {
        var seed = 3;

        const int width = 21, height = 21;
        var grid = new Array2D<bool>(width, height, true);

        var mc = MazeGraph.Create(grid);
        var start = new Vector2I(0, 0);
        mc.OnCreateNode += (i) => {
            PrintGraph(mc);
        };

        var col = 0;
        var row = 0;
        col++;
        mc.GrowBacktracker(start, 0f, -1, new Random(seed));
        PrintGraph(mc);
    }

    private static void PrintGraph(MazeGraph mc) {
        var canvas = new TextCanvas();
        foreach (var node in mc.Nodes) {
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

    public static void Main2() {
        var seed = 3;

        const int width = 21, height = 21;

        var template = new Array2D<bool>(width, height, false);

        var mc = MazeCarver.Create(template);
        var start = new Vector2I(5, 5);
        mc.OnCarve += (i) => {
            // PrintMaze(grid);
        };

        var canvas = new TextCanvas();

        var col = 0;
        var row = 0;
        template.Fill(false);
        mc.GrowBacktracker(start, 0f, -1, -1, 5, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "5 cells per path");

        col++;
        template.Fill(false);
        mc.GrowBacktracker(start, 0f, 3, -1, -1, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "3 paths only");

        col++;
        template.Fill(false);
        mc.GrowBacktracker(start, 0.5f, 3, -1, -1, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "Windy 0.5");

        col++;
        template.Fill(false);
        mc.GrowBacktracker(start, 1f, 2, -1, -1, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "Windy 1");

        row++;
        col = 0;
        template.Fill(false);
        mc.GrowVerticalBias(start, 1f, -1, -1, -1, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "Vertical");

        col++;
        template.Fill(false);
        mc.GrowHorizontalBias(start, 1f, -1, -1, -1, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "Horizontal");

        col++;
        template.Fill(false);
        mc.GrowClockwiseBias(start, 1f, -1, -1, -1, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "Clockwise");

        col++;
        template.Fill(false);
        mc.GrowCounterClockwiseBias(start, 1f, -1, -1, -1, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "CounterClockwise");

        Console.WriteLine(canvas.ToString());
    }

    private static string PrintMaze(Array2D<bool> grid) {
        return grid.GetString((v) => v ? "·" : "█");
    }
}