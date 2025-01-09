using System;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Betauer.Core.Examples;

public class MazeCarverDemo {
    public static void Main() {
        var seed = 3;
        var rng = new Random(seed);

        const int width = 17, height = 17;
        var maze = new Array2D<bool>(width, height, false);
        var mc = MazeCarver.Create(maze);
        var start = new Vector2I(1, 1);
        mc.OnCarve += (i) => {
            // Console.WriteLine(PrintMaze(template));
        };

        var canvas = new TextCanvas();
        var col = 0;
        var row = 0;

        // 5 cells per path
        maze.Fill(false);
        mc.GrowRandom(start, -1, rng);
        canvas.Write(col * width, row * height, PrintMaze(maze));
        canvas.Write(col * width, row * height, "Random backtrack");

        // 5 cells per path
        col++;
        maze.Fill(false);
        mc.Grow(start, BacktrackConstraints.CreateRandom(rng)
            .With(c => {
                c.MaxCellsPerPath = 5;
            }));
        canvas.Write(col * width, row * height, PrintMaze(maze));
        canvas.Write(col * width, row * height, "5 cells per path");

        // 3 paths only
        col++;
        maze.Fill(false);
        mc.Grow(start, BacktrackConstraints.CreateWindy(0f, rng)
            .With(c => {
                c.MaxPaths = 3;
                // c.MaxCellsPerPath = 9;
                // c.MaxTotalCells = 67;
            }
        ));
        canvas.Write(col * width, row * height, PrintMaze(maze));
        canvas.Write(col * width, row * height, "3 paths only");


        // Windy 0.5
        col++;
        maze.Fill(false);
        mc.Grow(start, BacktrackConstraints.CreateWindy(0.5f, rng)
            .With(c => c.MaxPaths = 3));
        canvas.Write(col * width, row * height, PrintMaze(maze));
        canvas.Write(col * width, row * height, "Windy 0.5");

        // Windy 1
        col++;
        maze.Fill(false);
        mc.Grow(start, BacktrackConstraints.CreateWindy(1f, rng)
            .With(c => c.MaxPaths = 2));
        canvas.Write(col * width, row * height, PrintMaze(maze));
        canvas.Write(col * width, row * height, "Windy 1");

        col++;

        // Vertical
        maze.Fill(false);
        mc.Grow(start, BacktrackConstraints.CreateVerticalBias(1f, rng));
        canvas.Write(col * width, row * height, PrintMaze(maze));
        canvas.Write(col * width, row * height, "Vertical");

        // Horizontal
        col++;
        maze.Fill(false);
        mc.Grow(start, BacktrackConstraints.CreateHorizontalBias(1f, rng));
        canvas.Write(col * width, row * height, PrintMaze(maze));
        canvas.Write(col * width, row * height, "Horizontal");

        // Clockwise
        col++;
        maze.Fill(false);
        mc.Grow(start, BacktrackConstraints.CreateClockwiseBias(1f, rng));
        canvas.Write(col * width, row * height, PrintMaze(maze));
        canvas.Write(col * width, row * height, "Clockwise");

        // CounterClockwise
        col++;
        maze.Fill(false);
        mc.Grow(start, BacktrackConstraints.CreateCounterClockwiseBias(1f, rng));
        canvas.Write(col * width, row * height, PrintMaze(maze));
        canvas.Write(col * width, row * height, "CounterClockwise");

        Console.WriteLine(canvas.ToString());
    }

    private static string PrintMaze(Array2D<bool> grid) {
        return grid.GetString((v) => v ? "·" : "█");
    }
}