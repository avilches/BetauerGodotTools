using System;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Betauer.Core.Examples;

public class MazeGraphDemo {
    public static void Main() {
        var seed = 3;
        var rng = new Random(seed);


        var mc = MazeGraph.Create(8, 8);
        var start = new Vector2I(0, 0);
        mc.OnEdgeCreated += (i) => {
            // PrintGraph(mc);
        };

        var constraints = BacktrackConstraints.CreateRandom(rng)
            .With(c => {
                c.MaxTotalCells = 25;
                c.MaxCellsPerPath = 5;
                c.MaxPaths = 3;
            });

        mc.Grow(start, constraints);
        Console.WriteLine(mc.Draw());
        Console.WriteLine(mc.Print());
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
        Console.WriteLine(mc.Draw());
        Console.WriteLine(mc.Print());
    }
}