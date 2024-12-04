using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Betauer.Core.PCG.Maze.Zoned;
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
                    ·#########
                    ##·#######
                    ##·####·##
                    ####·##·##
                    ####o##<<#
                    <#######x#
                    """;

        var template = Array2D.Parse(temp3);
        var mc = new MazeGraphZoned(template.Width, template.Height) {
            IsValidPositionFunc = pos => template[pos] != '·'
            
        };
        var start = template.FirstOrDefault(dataCell => dataCell.Value == 'o')!.Position;
        mc.OnConnect += (i) => { PrintGraph(mc); };

        mc.GrowZoned(start, new MazeZonedConstraints(4, 200)
            // .SetNodesPerZones(5)
            .SetRandomNodesPerZone(2, 5, rng)
            .SetPartsPerZone(2)
            .SetMaxDoorsOut(2), rng);
        PrintGraph(mc);

        for (var i = 0; i < 1; i++) {
            var autoSplitOnExpand = true;
            var corridor = false;
            var constraints = new MazePerZoneConstraints()
                    // .Zone(3, 2, 16, false)
                    // .Zone(9, 1, 5, true)
                    .Zone(nodes: 10, corridor: true)
                    .Zone(nodes: 1, maxDoorsOut: 0)
                    .Zone(nodes: 4, maxDoorsOut: 0, autoSplitOnExpand: false, corridor: true)
                    .Zone(nodes: 5, parts: 4)
                    .Zone(nodes: 3)
                    .Zone(nodes: 4)
                    .Zone(nodes: 4, parts: 2)
                    .Zone(nodes: 4)
                    .Zone(nodes: 3, parts: 2)
                    .Zone(nodes: 3)
                    .Zone(nodes: 3)
                    .Zone(nodes: 3)
                    .Zone(nodes: 3)
                ;

            var zones = mc.GrowZoned(start, constraints, rng);

            foreach (var zone in zones) {
                Console.WriteLine($"Zone {zone.Id} Nodes: {zone.Nodes} Parts: {zone.Parts}/{zone.ConfigParts} DoorsOut: {zone.DoorsOut}/{zone.MaxDoorsOut}");
            }
        }
        // ConnectNodes(template, mc);

        PrintGraph(mc);
    }

    private static void ConnectNodes(Array2D<char> template, MazeGraphZoned mc) {
        template
            .Where(dataCell => dataCell.Value == '<')
            .Select(dataCell => mc.GetNodeAt(dataCell.Position)).Where(node => node != null).Select(node => node!)
            .ForEach(from => mc.ConnectNodes(from, mc.GetNodeAt(from.Position + Vector2I.Left)!, true));

        template
            .Where(dataCell => dataCell.Value == '+' || dataCell.Value == 'o')
            .Select(dataCell => mc.GetNodeAt(dataCell.Position)).Where(node => node != null).Select(node => node!)
            .ForEach(from => {
                mc.ConnectNodeTowards(from,  Vector2I.Left, true);
                mc.ConnectNodeTowards(from,  Vector2I.Right, true);
                mc.ConnectNodeTowards(from,  Vector2I.Up, true);
                mc.ConnectNodeTowards(from,  Vector2I.Down, true);
            });
    }

    /*
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
    */

    private static void PrintGraph(MazeGraphZoned mc) {
        var allCanvas = new TextCanvas();
        foreach (var dataCell in mc.NodeGrid) {
            var node = dataCell.Value;
            if (node == null) continue;
            var canvas = new TextCanvas();
            if (node.Up != null) canvas.Write(1, 0, "|");
            if (node.Right != null) canvas.Write(2, 1, "-");
            if (node.Down != null) canvas.Write(1, 2, "|");
            if (node.Left != null) canvas.Write(0, 1, "-");
            canvas.Write(1, 1, node.Zone.ToString());

            allCanvas.Write(dataCell.Position.X * 3, dataCell.Position.Y * 3, canvas.ToString());
        }
        Console.WriteLine(allCanvas.ToString());
    }
}