using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Betauer.Core.PCG.Maze.Zoned;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class Metadata {
    public bool added = false;

    public Metadata(bool added) {
        this.added = added;
    }
}

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
                    ·#########·
                    ###########
                    ##·##·##·##
                    ###########
                    ##·##o##·##
                    <<<<<<<<<<<
                    """;

        var template = Array2D.Parse(temp3);
        var mc = new MazeGraphZoned<Metadata>(template.Width, template.Height) {
            IsValidPositionFunc = pos => template[pos] != '·'
        };
        var start = template.FirstOrDefault(dataCell => dataCell.Value == 'o')!.Position;
        // mc.OnNodeConnected += (i) => { PrintGraph(mc); };

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
                    .Zone(nodes: 10, corridor: false)
                    .Zone(nodes: 10, maxDoorsOut: 0)
                    .Zone(nodes: 4, maxDoorsOut: 0, autoSplitOnExpand: false, corridor: true)
                    .Zone(nodes: 5, parts: 4)
                    .Zone(nodes: 3)
                    .Zone(nodes: 4)
                    .Zone(nodes: 4, parts: 2)
                    .Zone(nodes: 4)
                    .Zone(nodes: 3, parts: 2)
                ;

            mc.Clear();
            var zones = mc.GrowZoned(start, constraints, rng);

            foreach (var zone in zones) {
                Console.WriteLine($"Zone {zone.Id} Nodes: {zone.Nodes} Parts: {zone.Parts}/{zone.ConfigParts} DoorsOut: {zone.DoorsOut}/{zone.MaxDoorsOut}");
            }
            
            ConnectNodes(template, mc);

            PrintGraph(mc);

            foreach (var zone in zones) {
                ConnectZone(mc, zone.Id);
            };
            AddLongestCycles(mc, 5);
            AddShortestCycles(mc, 3);
            PrintGraph(mc);

        }
    }

    private static void ConnectZone(MazeGraphZoned<Metadata> mc, int zone) {
        var cycles = mc.GetPotentialCycles();
        var connection = cycles
            .GetCyclesGreaterThan(0)
            .FirstOrDefault(c => c.nodeA.Zone == c.nodeB.Zone && c.nodeA.Zone == zone);
        if (connection == default) return;

        mc.ConnectNodes(connection.nodeA, connection.nodeB).Metadata = new Metadata(true);
        mc.ConnectNodes(connection.nodeB, connection.nodeA).Metadata = new Metadata(true);
    }

    private static void AddLongestCycles(MazeGraphZoned<Metadata> mc, int maxCycles) {
        var cycles = mc.GetPotentialCycles();
        var cyclesAdded = 0;
        while (cyclesAdded < maxCycles) {
            var connection = cycles
                .GetAllCycles()
                .FirstOrDefault(c => c.nodeA.Zone != c.nodeB.Zone);
            if (connection == default) break;

            mc.ConnectNodes(connection.nodeA, connection.nodeB).Metadata = new Metadata(true);
            mc.ConnectNodes(connection.nodeB, connection.nodeA).Metadata = new Metadata(true);
            cyclesAdded++;
            cycles.RemoveCycle(connection.nodeA, connection.nodeB);
        }
    }

    private static void AddShortestCycles(MazeGraphZoned<Metadata> mc, int maxCycles) {
        var cycles = mc.GetPotentialCycles();
        var cyclesAdded = 0;
        while (cyclesAdded < maxCycles) {
            var connection = cycles
                .GetAllCycles(false)
                .FirstOrDefault(c => c.nodeA.Zone != c.nodeB.Zone);
            if (connection == default) break;

            mc.ConnectNodes(connection.nodeA, connection.nodeB).Metadata = new Metadata(true);
            mc.ConnectNodes(connection.nodeB, connection.nodeA).Metadata = new Metadata(true);
            cyclesAdded++;
            cycles.RemoveCycle(connection.nodeA, connection.nodeB);
        }
    }

    private static void ConnectNodes(Array2D<char> template, MazeGraphZoned<Metadata> mc) {
        template
            .Where(dataCell => dataCell.Value == '<')
            .Select(dataCell => mc.GetNodeAtOrNull(dataCell.Position)!).Where(node => node != null!)
            .ForEach(from => {
                var to = mc.GetNodeAtOrNull(from.Position + Vector2I.Left);
                if (to != null) {
                    mc.ConnectNodes(from, to).Metadata = new Metadata(true);;
                    mc.ConnectNodes(to, from).Metadata = new Metadata(true);;
                }
                
            });

        template
            .Where(dataCell => dataCell.Value == '+' || dataCell.Value == 'o')
            .Select(dataCell => mc.GetNodeAt(dataCell.Position)).Where(node => node != null).Select(node => node!)
            .ForEach(from => {
                var left = mc.GetNodeAtOrNull(from.Position + Vector2I.Left);
                var right = mc.GetNodeAtOrNull(from.Position + Vector2I.Right);
                var up = mc.GetNodeAtOrNull(from.Position + Vector2I.Up);
                var down = mc.GetNodeAtOrNull(from.Position + Vector2I.Down);
                
                if (left != null) mc.ConnectNodes(from, left).Metadata = new Metadata(true);;
                if (right != null) mc.ConnectNodes(from, right).Metadata = new Metadata(true);;
                if (up != null) mc.ConnectNodes(from, up).Metadata = new Metadata(true);;
                if (down != null) mc.ConnectNodes(from, down).Metadata = new Metadata(true);;
                
                if (left != null) mc.ConnectNodes(left, from).Metadata = new Metadata(true);;
                if (right != null) mc.ConnectNodes(right, from).Metadata = new Metadata(true);;
                if (up != null) mc.ConnectNodes(up, from).Metadata = new Metadata(true);;
                if (down != null) mc.ConnectNodes(down, from).Metadata = new Metadata(true);;
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

    private static void PrintGraph(MazeGraphZoned<Metadata> mc) {
        var allCanvas = new TextCanvas();
        foreach (var node in mc.GetNodes()) {
            if (node == null) continue;
            var canvas = new TextCanvas();
            if (node.Up != null) canvas.Write(1, 0, node.GetEdgeTowards(Vector2I.Up)!.Metadata?.added ?? false ? "·" : "|");
            if (node.Right != null) canvas.Write(2, 1, node.GetEdgeTowards(Vector2I.Right)!.Metadata?.added ?? false ? "·" : "-");
            if (node.Down != null) canvas.Write(1, 2, node.GetEdgeTowards(Vector2I.Down)!.Metadata?.added ?? false ? "·" : "|");
            if (node.Left != null) canvas.Write(0, 1, node.GetEdgeTowards(Vector2I.Left)!.Metadata?.added ?? false ? "·" : "-");
            canvas.Write(1, 1, node.Zone.ToString());

            allCanvas.Write(node.Position.X * 3, node.Position.Y * 3, canvas.ToString());
        }
        Console.WriteLine(allCanvas.ToString());
    }
}