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
                    ·####o####
                    ·#########
                    ···##·###·
                    """;

        var template = Array2D.Parse(temp3);
        var mc = new MazeGraph(template.Width, template.Height) {
            IsValid = pos => template[pos] != '·'
        };
        var start = template.FirstOrDefault(dataCell => dataCell.Value == 'o')!.Position;
        mc.OnCreateNode += (i) => {
            PrintGraph(mc);
        };

        mc.GrowZoned(start, 8,-1, rng); //, list => rng.Next(list));

        // ConnectNodes(template, mc);

        CreateZones(mc, 8);

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

    public static void CreateZones(MazeGraph mc, int nodesPerZone) {
        if (mc.NodeGridRoot == null) return;
        if (nodesPerZone <= 0) throw new ArgumentException("nodesPerZone must be greater than 0");

        var currentZone = 0;
        var nodesInCurrentZone = 0;
        var visited = new HashSet<NodeGrid>();
        var queue = new Queue<NodeGrid>();

        // Empieza con el nodo raíz
        queue.Enqueue(mc.NodeGridRoot);
        mc.NodeGridRoot.Metadata = currentZone;
        visited.Add(mc.NodeGridRoot);
        nodesInCurrentZone = 1;

        while (queue.Count > 0) {
            var node = queue.Dequeue();

            foreach (var edge in node.GetEdges()) {
                var neighbor = edge.To;
                if (visited.Contains(neighbor)) continue;

                // Si llegamos al límite de nodos en la zona actual
                if (nodesInCurrentZone >= nodesPerZone) {
                    currentZone++;
                    nodesInCurrentZone = 0;
                }

                neighbor.Metadata = currentZone;
                visited.Add(neighbor);
                queue.Enqueue(neighbor);
                nodesInCurrentZone++;
            }
        }
    }

    public static void MarkDoors(MazeGraph mc) {
        foreach (var node in mc.Nodes.Values) {
            var nodeZone = (int)node.Metadata!;
            foreach (var edge in node.GetEdges()) {
                var neighborZone = (int)edge.To.Metadata!;
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
            canvas.Write(1, 1, node.Metadata?.ToString());
            if (node.Up != null) canvas.Write(1, 0, "|");
            if (node.Right != null) canvas.Write(2, 1, "-");
            if (node.Down != null) canvas.Write(1, 2, "|");
            if (node.Left != null) canvas.Write(0, 1, "-");

            allCanvas.Write(dataCell.Position.X * 3, dataCell.Position.Y * 3, canvas.ToString());
        }
        Console.WriteLine(allCanvas.ToString());
    }
}