using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Betauer.Core.PCG.Maze.Zoned;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class Metadata() {
    public bool Added = false;
    public int Key = -1;

    public Metadata(bool added) : this() {
        Added = added;
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

        var temp4 = """
                    ###############
                    ###############
                    ###############
                    ###############
                    ###############
                    ###############
                    ###############
                    #######o#######
                    ###############
                    ###############
                    ###############
                    ###############
                    ###############
                    ###############
                    ###############
                    """;

        var template = Array2D.Parse(temp4);
        var mc = new MazeGraph(template.Width, template.Height) {
            IsValidPositionFunc = pos => template[pos] != '·'
        };
        var start = template.FirstOrDefault(dataCell => dataCell.Value == 'o')!.Position;
        mc.OnNodeCreated += (i) => { PrintGraph(mc); };

        mc.GrowZoned(start, new MazeZonedConstraints(4, 200)
            // .SetNodesPerZones(5)
            .SetRandomNodesPerZone(4, 10, rng)
            .SetPartsPerZone(2)
            .SetMaxDoorsOut(2), rng);
        PrintGraph(mc);

        for (var i = 0; i < 1; i++) {
            var autoSplitOnExpand = true;
            var corridor = false;
            var constraints = new MazePerZoneConstraints()
                    .Zone(nodes: 15, corridor: false)
                    .Zone(nodes: 20, parts: 3, corridor: true)
                    .Zone(nodes: 2, parts: 1, maxDoorsOut: 0, corridor: true)
                    // .Zone(nodes: 15, parts: 3, corridor: false)
                    // .Zone(nodes: 15, parts: 3, corridor: false)
                    // .Zone(nodes: 1, parts: 1, corridor: false)
                ;

            // PENDIENTE DE HACER
            // 4 colocar llaves, a ser posible, despues de ver la puerta
            // calcular linearidad: una IA que coja todas las llaves y llegue al boss, que mire a ver
            // cuantas veces pasa por cada nodo, y si se deja nodos sin pasar
            // calcular dificultad segun cercania al boss?

            mc.Clear();
            var zones = mc.GrowZoned(start, constraints, rng);
            
            PrintGraph(mc);
            
            
            
            foreach (var zone in zones.Zones) {
                Console.WriteLine($"Zone {zone.ZoneId} Nodes: {zone.NodeCount} Parts: {zone.Parts.Count}/{constraints.GetParts(zone.ZoneId)} DoorsOut: {zone.GetDoorOutNodes().Count()}/{constraints.GetMaxDoorsOut(zone.ZoneId)}");
            }

            NeverConnectZone(mc, 2);

            mc.OnEdgeCreated += (i) => {
                // PrintGraph(mc);
            };
            
            Console.WriteLine("Connecting nodes with symbols");
            ConnectNodes(template, mc);
            
            // PrintGraph(mc);
            
            foreach (var zone in zones.Zones) {
                Console.WriteLine("Connection zone " + zone.ZoneId);
                ConnectZone(mc, zone.ZoneId);
            }
            Console.WriteLine("Longest cycles");
            AddLongestCycles(mc, 5);
            Console.WriteLine("Shortest cycles");
            AddShortestCycles(mc, 3);
            
            zones.CalculateNodeScores();
            PrintGraph(mc, zones);
            return;


        }
    }
    
    private static void NeverConnectZone(MazeGraph mc, int zone) {
        var neverConnect = mc.GetNodes().Where(n => n.ZoneId == zone).Select(node => node.Position).ToList();
        mc.IsValidEdgeFunc = (from, to) => {
            if (neverConnect.Contains(from) || neverConnect.Contains(to)) return false;
            return true;
        };
    }

    private static void ConnectZone(MazeGraph mc, int zone) {
        var cycles = mc.GetPotentialCycles();
        var connection = cycles
            .GetCyclesGreaterThan(0)
            .FirstOrDefault(c => c.nodeA.ZoneId == c.nodeB.ZoneId && c.nodeA.ZoneId == zone);
        if (connection == default) return;

        connection.nodeA.ConnectTo(connection.nodeB, new Metadata(true));
        connection.nodeB.ConnectTo(connection.nodeA, new Metadata(true));
    }

    private static void AddLongestCycles(MazeGraph mc, int maxCycles) {
        var cycles = mc.GetPotentialCycles();
        var cyclesAdded = 0;
        while (cyclesAdded < maxCycles) {
            var connection = cycles
                .GetAllCycles()
                .FirstOrDefault(c => c.nodeA.ZoneId != c.nodeB.ZoneId);
            if (connection == default) break;

            connection.nodeA.ConnectTo(connection.nodeB, new Metadata(true));
            connection.nodeB.ConnectTo(connection.nodeA, new Metadata(true));
            cyclesAdded++;
            cycles.RemoveCycle(connection.nodeA, connection.nodeB);
        }
    }

    private static void AddShortestCycles(MazeGraph mc, int maxCycles) {
        var cycles = mc.GetPotentialCycles();
        var cyclesAdded = 0;
        while (cyclesAdded < maxCycles) {
            var connection = cycles
                .GetAllCycles(false)
                .FirstOrDefault(c => c.nodeA.ZoneId != c.nodeB.ZoneId);
            if (connection == default) break;

            connection.nodeA.ConnectTo(connection.nodeB, new Metadata(true));
            connection.nodeB.ConnectTo(connection.nodeA, new Metadata(true));
            cyclesAdded++;
            cycles.RemoveCycle(connection.nodeA, connection.nodeB);
        }
    }

    private static void ConnectNodes(Array2D<char> template, MazeGraph mc) {
        template
            .Where(dataCell => dataCell.Value == '<')
            .Select(dataCell => mc.GetNodeAtOrNull(dataCell.Position)!)
            .Where(node => node != null! && mc.IsValidEdge(node.Position, node.Position + Vector2I.Left))
            .ForEach(from => {
                var to = mc.GetNodeAtOrNull(from.Position + Vector2I.Left);
                if (to != null && !from.HasEdgeTo(to)) {
                    from.ConnectTo(to, new Metadata(true));
                    to.ConnectTo(from, new Metadata(true));
                }
            });

        template
            .Where(dataCell => dataCell.Value == '+' || dataCell.Value == 'o')
            .Select(dataCell => mc.GetNodeAt(dataCell.Position))
            .Where(node => node != null!)
            .ForEach(from => {
                mc.GetOrtogonalPositions(from.Position).Select(mc.GetNodeAtOrNull)
                    .Where(node => node != null && mc.IsValidEdge(node.Position, from.Position))
                    .ForEach(to => {
                        if (!from.HasEdgeTo(to)) {
                            from.ConnectTo(to, new Metadata(true));
                            to.ConnectTo(from, new Metadata(true));
                        }
                    });
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

    private static void PrintGraph(MazeGraph mc, MazeZones zones = null) {
        var allCanvas = new TextCanvas();
        var offset = 0;
        foreach (var node in mc.GetNodes()) {
            if (node == null) continue;
            var canvas = new TextCanvas();
            if (node.Up != null) canvas.Write(1, 0, node.GetEdgeTowards(Vector2I.Up)!.GetMetadataOrNew<Metadata>().Added ? "·" : "|");
            if (node.Right != null) canvas.Write(2, 1, node.GetEdgeTowards(Vector2I.Right)!.GetMetadataOrNew<Metadata>().Added ? "·" : "-");
            if (node.Down != null) canvas.Write(1, 2, node.GetEdgeTowards(Vector2I.Down)!.GetMetadataOrNew<Metadata>().Added ? "·" : "|");
            if (node.Left != null) canvas.Write(0, 1, node.GetEdgeTowards(Vector2I.Left)!.GetMetadataOrNew<Metadata>().Added ? "·" : "-");
            canvas.Write(1, 1, node.ZoneId.ToString());
            // canvas.Write(1, 1, node.ZoneId.ToString()+node.PartId.ToString());

            allCanvas.Write(offset + node.Position.X * 3, node.Position.Y * 3, canvas.ToString());
        }
        offset += (mc.Width * 3 + 5);
        foreach (var node in mc.GetNodes()) {
            if (node == null) continue;
            var canvas = new TextCanvas();
            if (node.Up != null) canvas.Write(1, 0, node.GetEdgeTowards(Vector2I.Up)!.GetMetadataOrNew<Metadata>().Added ? "·" : "|");
            if (node.Right != null) canvas.Write(2, 1, node.GetEdgeTowards(Vector2I.Right)!.GetMetadataOrNew<Metadata>().Added ? "·" : "-");
            if (node.Down != null) canvas.Write(1, 2, node.GetEdgeTowards(Vector2I.Down)!.GetMetadataOrNew<Metadata>().Added ? "·" : "|");
            if (node.Left != null) canvas.Write(0, 1, node.GetEdgeTowards(Vector2I.Left)!.GetMetadataOrNew<Metadata>().Added ? "·" : "-");
            canvas.Write(1, 1, node.Id.ToString());

            allCanvas.Write(offset + node.Position.X * 3, node.Position.Y * 3, canvas.ToString());
        }

        if (zones != null) {
            var keys = zones.GetBestLocationsByZone(KeyFormula);
            var loot = zones.SpreadLocationsByZone(0.3f, LootFormula);
            
            offset += mc.Width * 3 + 5;
            foreach (var node in mc.GetNodes()) {
                if (node == null) continue;
                var canvas = new TextCanvas();
                if (node.Up != null) canvas.Write(1, 0, node.GetEdgeTowards(Vector2I.Up)!.GetMetadataOrNew<Metadata>().Added ? "·" : "|");
                if (node.Right != null) canvas.Write(2, 1, node.GetEdgeTowards(Vector2I.Right)!.GetMetadataOrNew<Metadata>().Added ? "····" : "----");
                if (node.Down != null) canvas.Write(1, 2, node.GetEdgeTowards(Vector2I.Down)!.GetMetadataOrNew<Metadata>().Added ? "·" : "|");
                if (node.Left != null) canvas.Write(0, 1, node.GetEdgeTowards(Vector2I.Left)!.GetMetadataOrNew<Metadata>().Added ? "·" : "-");

                var zone = zones.Zones[node.ZoneId];

                var nodeScore = zones.GetScore(node);
                var keyScore = Mathf.RoundToInt(KeyFormula(nodeScore) * 100);
                var lootScore = Mathf.RoundToInt(LootFormula(nodeScore) * 100);
                var nodeWithKeyInZone = keys[zone.ZoneId];
                var isKey = nodeWithKeyInZone == node;
                    
                // var score = Mathf.RoundToInt(zone.NodeScores[node].DeadEndScore * 100);
                var isLoot = loot[node.ZoneId].Contains(node);
                
                if (isLoot) {
                    canvas.Write(1, 1, ""+lootScore+"$");
                } else {
                    canvas.Write(1, 1, ""+lootScore);
                }
                
                /*
                 if (isKey) {
                    canvas.Write(1, 1, $"{keyScore*1:0}k");
                } else {
                    canvas.Write(1, 1, $"{keyScore*1:0}");
                }
                */
                allCanvas.Write(offset + node.Position.X * 6, node.Position.Y * 3, canvas.ToString());
            }
        }
    
        Console.WriteLine(allCanvas.ToString());
    }

    public static float KeyFormula(NodeScore score) => score.DeadEndScore * 0.4f + score.EntryDistanceScore * 0.3f + score.ExitDistanceScore * 0.3f;
    public static float LootFormula(NodeScore score) => score.DeadEndScore * 0.6f + score.EntryDistanceScore * 0.4f;


}