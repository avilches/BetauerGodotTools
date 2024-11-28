using System;
using System.Text;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class MazeCarverDemo {
    public static void Main() {
        var seed = 3;
        var rng = new Random(seed);

        const int width = 121, height = 21;
        var template = new Array2D<bool>(width, height, false);
        var mc = MazeCarver.Create(template);
        var start = new Vector2I(5, 5);
        mc.OnCarve += (i) => {
            // Console.WriteLine(PrintMaze(template));
        };

        var canvas = new TextCanvas();
        var col = 0;
        var row = 0;

        // 5 cells per path
        template.Fill(false);
        mc.Grow(start, MazeConstraints.CreateWindy(0f, rng)
            .With(c => {
                // c.MaxTotalCells = 11;
                c.MaxCellsPerPath = 15;
                c.MaxPaths = 13;
            }));
        Console.WriteLine("--");
        canvas.Write(col * width, row * height, PrintMaze(template));
        // canvas.Write(col * width, row * height, "5 cells per path");
        Console.WriteLine(canvas.ToString());
        return;

        // 3 paths only
        col++;
        template.Fill(false);
        mc.Grow(start, MazeConstraints.CreateWindy(0f, rng)
            .With(c => {
                c.MaxPaths = 3;
                // c.MaxCellsPerPath = 9;
                // c.MaxTotalCells = 67;
            }
        ));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "3 paths only");


        // Windy 0.5
        col++;
        template.Fill(false);
        mc.Grow(start, MazeConstraints.CreateWindy(0.5f, rng)
            .With(c => c.MaxPaths = 3));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "Windy 0.5");

        // Windy 1
        col++;
        template.Fill(false);
        mc.Grow(start, MazeConstraints.CreateWindy(1f, rng)
            .With(c => c.MaxPaths = 2));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "Windy 1");

        row++;
        col = 0;

        // Vertical
        template.Fill(false);
        mc.Grow(start, MazeConstraints.CreateVerticalBias(1f, rng));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "Vertical");

        // Horizontal
        col++;
        template.Fill(false);
        mc.Grow(start, MazeConstraints.CreateHorizontalBias(1f, rng));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "Horizontal");

        // Clockwise
        col++;
        template.Fill(false);
        mc.Grow(start, MazeConstraints.CreateClockwiseBias(1f, rng));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "Clockwise");

        // CounterClockwise
        col++;
        template.Fill(false);
        mc.Grow(start, MazeConstraints.CreateCounterClockwiseBias(1f, rng));
        canvas.Write(col * width, row * height, PrintMaze(template));
        canvas.Write(col * width, row * height, "CounterClockwise");

        Console.WriteLine(canvas.ToString());
    }

    private static string PrintMaze(Array2D<bool> grid) {
        return grid.GetString((v) => v ? "·" : "█");
    }
}