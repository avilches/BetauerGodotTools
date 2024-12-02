using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class MazeGraphDungeonDemo {
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

        var temp3 = """
                    ··········
                    ··######··
                    ·#########
                    ·#########
                    ·#########
                    ·#########
                    ·####o####
                    ·#########
                    ·#########
                    ·#########
                    ···##·###·
                    """;

        var template = Array2D.Parse(temp3);
        var mc = new MazeGraph(template.Width, template.Height) {
            IsValid = pos => template[pos] != '·'
        };
        var start = template.FirstOrDefault(dataCell => dataCell.Value == 'o')!.Position;
        mc.OnConnect += (i) => {
            PrintGraph(mc);
        };

        // mc.GrowZoned(start, new MazeZonedConstraints(100, 200).SetNodesPerZones(9), rng);
        mc.GrowZoned(start, new MazePerZoneConstraints()
            .Zone(0, 4, 0, 1)
            .Zone(1, 4, 1, 3)
            .Zone(2, 4, 3, 0)
            , rng); 

        // ConnectNodes(template, mc);

        PrintGraph(mc);
    }

    private static void ConnectNodes(Array2D<char> template, MazeGraph mc) {
        template
            .Where(dataCell => dataCell.Value == '<')
            .Select(dataCell => mc.GetNode(dataCell.Position)).Where(node => node != null).Select(node => node!)
            .ForEach(from => { mc.ConnectNode(from, Vector2I.Left, true); });

        template
            .Where(dataCell => dataCell.Value == '+' || dataCell.Value == 'o')
            .Select(dataCell => mc.GetNode(dataCell.Position)).Where(node => node != null).Select(node => node!)
            .ForEach(from => {
                mc.ConnectNode(from, Vector2I.Left, true);
                mc.ConnectNode(from, Vector2I.Right, true);
                mc.ConnectNode(from, Vector2I.Up, true);
                mc.ConnectNode(from, Vector2I.Down, true);
            });
    }

    public static void MarkDoors(MazeGraph mc) {
        foreach (var node in mc.Nodes.Values) {
            var nodeZone = node.Zone;
            foreach (var edge in node.GetEdges()) {
                var neighborZone = edge.To.Zone;
                if (nodeZone != neighborZone) {
                    // Marca la puerta con el número mayor de zona
                    var doorNumber = Math.Max(nodeZone, neighborZone);
                    edge.Metadata = doorNumber;
                
                    // Marca también el edge inverso si existe
                    var reverseEdge = edge.To.GetEdgeTo(node);
                    if (reverseEdge != null) {
                        reverseEdge.Metadata = doorNumber;
                    }
                }
            }
        }
    }
    private static void PrintGraph(MazeGraph mc) {
        var allCanvas = new TextCanvas();
        foreach (var dataCell in mc.NodeGrid) {
            var node = dataCell.Value;
            if (node == null) continue;
            var canvas = new TextCanvas();
            canvas.Write(1, 1, node.Zone.ToString());
            if (node.Up != null) canvas.Write(1, 0, "|");
            if (node.Right != null) canvas.Write(2, 1, "-");
            if (node.Down != null) canvas.Write(1, 2, "|");
            if (node.Left != null) canvas.Write(0, 1, "-");

            allCanvas.Write(dataCell.Position.X * 3, dataCell.Position.Y * 3, canvas.ToString());
        }
        Console.WriteLine(allCanvas.ToString());
    }
}