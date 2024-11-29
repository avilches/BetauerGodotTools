using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class MazeGraphDemo {
    public static Vector2I FindCharInAsciiPattern(string pattern, char target = 'o') {
        var lines = pattern.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        for (var y = 0; y < lines.Length; y++) {
            var line = lines[y];
            var x = line.IndexOf(target);
            if (x != -1) {
                return new Vector2I(x, y);
            }
        }
        throw new Exception($"Character '{target}' not found in pattern");
    }

    public static IEnumerable<Vector2I> FindAllCharsInAsciiPattern(string pattern, char target = 'o') {
        var lines = pattern.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        for (var y = 0; y < lines.Length; y++) {
            var line = lines[y];
            var x = 0;
            while ((x = line.IndexOf(target, x)) != -1) {
                yield return new Vector2I(x, y);
                x++; // Avanzamos para buscar la siguiente ocurrencia
            }
        }
    }

    public static void Main() {
        var seed = 5;
        var rng = new Random(seed);

        var temp = """
                   ···#####········
                   ··######········
                   ·#########······
                   ###······+#####·
                   ###<<<<<<+####··
                   ###·····########
                   ###<<+<<<##·····
                   #####+#####·····
                   ···##o##·····###
                   ···##·##·····###
                   """;
        var temp2 = """
                    ··········
                    ··######··
                    ·#########
                    ·###···###
                    ·####o####
                    ···##·###·
                    """;

        var template = Array2D.Parse(temp);
        var mc = new MazeGraph(template.Width, template.Height, pos => template[pos] != '·');
        var start = FindCharInAsciiPattern(temp);
        mc.OnCreateNode += (i) => { PrintGraph(mc); };

        var constraints = MazeConstraints.CreateRandom(rng)
            .With(c => {
                // c.MaxTotalCells = 25;
                // c.MaxCellsPerPath = 10;
                // c.MaxPaths = 3;
            });

        mc.Grow(start, constraints);

        template
            .Where(dataCell => dataCell.Value == '<')
            .Select(dataCell => mc.GetNode(dataCell.Position)).Where(node => node != null).Select(node => node!)
            .ForEach(from => {
                mc.ConnectNode(from, Vector2I.Left, true);
            });

        template
            .Where(dataCell => dataCell.Value == '+')
            .Select(dataCell => mc.GetNode(dataCell.Position)).Where(node => node != null).Select(node => node!)
            .ForEach(from => {
                mc.ConnectNode(from, Vector2I.Left, true);
                mc.ConnectNode(from, Vector2I.Right, true);
                mc.ConnectNode(from, Vector2I.Up, true);
                mc.ConnectNode(from, Vector2I.Down, true);
            });

        PrintGraph(mc);
    }

    public static void MainOld() {
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