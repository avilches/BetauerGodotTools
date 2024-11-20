using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class MazeDemo {
    public static void Main() {
        var random = new Random(1);

        const int width = 11, height = 7;

        var grid = new Array2D<bool>(width, height).Fill(false);

        var mc = new MazeCarverBool(grid);
        Console.WriteLine("Mazes: "+mc.FillMazes(0.7f, random));

        PrintMaze(grid);
    }

    private static void PrintMaze(Array2D<bool> grid) {
        foreach (var b in grid) {
            Console.Write(b.Value ? " " : "█");
            if (b.Position.X == grid.Width - 1) {
                Console.WriteLine();
            }
        }
    }
}