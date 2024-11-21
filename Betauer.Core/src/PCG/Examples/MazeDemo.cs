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

        var grid = new Array2D<bool>(width, height).Fill(false);
        
        var mc = MazeCarver.Create(grid);
        var start = new Vector2I(5, 5);
        var old = mc.CarveAction;
        mc.CarveAction = (i, type) => {
            old(i, type);
            // PrintMaze(grid);
        };

        var canvas = new TextCanvas(width * 4, height * 2);

        var col = 0;
        var row = 0;
        grid.Fill(false);
        mc.GrowRandom(start, 10, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(grid));
        canvas.Write(col * width, row * height, "Random");

        col++;
        grid.Fill(false);
        mc.GrowBacktracker(start, 0f, 2, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(grid));
        canvas.Write(col * width, row * height, "Backtracker 0");
        
        col++;
        grid.Fill(false);
        mc.GrowBacktracker(start, 0.5f, 3, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(grid));
        canvas.Write(col * width, row * height, "Backtracker 0.5");
        
        col++;
        grid.Fill(false);
        mc.GrowBacktracker(start, 1f, 2, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(grid));
        canvas.Write(col * width, row * height, "Backtracker 1");
        
        row++;
        col = 0;
        grid.Fill(false);
        mc.GrowVerticalBias(start, 1f, -1, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(grid));
        canvas.Write(col * width, row * height, "Vertical");

        col++;
        grid.Fill(false);
        mc.GrowHorizontalBias(start, 1f, -1, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(grid));
        canvas.Write(col * width, row * height, "Horizontal");

        col++;
        grid.Fill(false);
        mc.GrowClockwiseBias(start, 1f, -1, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(grid));
        canvas.Write(col * width, row * height, "Clockwise");

        col++;
        grid.Fill(false);
        mc.GrowCounterClockwiseBias(start, 1f, -1, new Random(seed));
        canvas.Write(col * width, row * height, PrintMaze(grid));
        canvas.Write(col * width, row * height, "CounterClockwise");

        Console.WriteLine(canvas.ToString());
    }

    private static string PrintMaze(Array2D<bool> grid) {
        var sb = new StringBuilder();
        foreach (var b in grid) {
            sb.Append(b.Value ? " " : "█");
            if (b.Position.X == grid.Width - 1) {
                sb.AppendLine();
            }
        }
        return sb.ToString();
    }
}