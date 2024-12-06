using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Betauer.Core.PCG.Maze.Zoned;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class Metadata {
    public bool Added = false;
    public int Key = -1;

    public Metadata(bool added) {
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
                    ###############
                    ###############
                    ###############
                    ###############
                    ###############
                    ###############
                    ###############
                    #######o#######
                    """;

        var template = Array2D.Parse(temp4);
        var mc = new MazeGraphZoned<Metadata>(template.Width, template.Height) {
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
                    .Zone(nodes: 20, parts: 1, corridor: true)
                    .Zone(nodes: 2, parts: 1, maxDoorsOut: 0, corridor: true)
                    .Zone(nodes: 15, parts: 3, corridor: false)
                    .Zone(nodes: 15, parts: 3, corridor: false)
                    .Zone(nodes: 1, parts: 1, corridor: false)
                ;

            // PENDIENTE DE HACER
            // 4 colocar llaves, a ser posible, despues de ver la puerta
            // calcular linearidad: una IA que coja todas las llaves y llegue al boss, que mire a ver
            // cuantas veces pasa por cada nodo, y si se deja nodos sin pasar
            // calcular dificultad segun cercania al boss?

            mc.Clear();
            var zones = mc.GrowZoned(start, constraints, rng);
            
            PrintGraph(mc);
            
            
            
            foreach (var zone in zones) {
                Console.WriteLine($"Zone {zone.ZoneId} Nodes: {zone.Nodes} Parts: {zone.Parts.Count}/{zone.ConfigParts} DoorsOut: {zone.DoorsOut}/{zone.MaxDoorsOut}");
            }

            NeverConnectZone(mc, 2);

            mc.OnEdgeCreated += (i) => {
                // PrintGraph(mc);
            };
            
            Console.WriteLine("Connecting nodes with symbols");
            ConnectNodes(template, mc);
            
            // PrintGraph(mc);
            
            foreach (var zone in zones) {
                Console.WriteLine("Connection zone " + zone.ZoneId);
                ConnectZone(mc, zone.ZoneId);
            }
            Console.WriteLine("Longest cycles");
            AddLongestCycles(mc, 5);
            Console.WriteLine("Shortest cycles");
            AddShortestCycles(mc, 3);
            
            AssignKeyLocations(zones);
            AssignTreasures(zones, 0.7f);
            PrintGraph(mc, zones);
            return;


        }
    }
    
    public static void AssignKeyLocations(List<ZoneCreated<Metadata>> zones) {
        // Empezamos desde la zona 1 (la 0 no necesita llave)
        for (var i = 0; i < zones.Count; i++) {
            zones[i].SelectKeyLocation();
        }
    }
    
    public static void AssignTreasures(List<ZoneCreated<Metadata>> zones, float treasurePercentage) {
        foreach (var zone in zones) {
            zone.SelectTreasureLocations(treasurePercentage);
        }
    }


    private static void NeverConnectZone(MazeGraphZoned<Metadata> mc, int zone) {
        var neverConnect = mc.GetNodes().Where(n => n.Zone == zone).Select(node => node.Position).ToList();
        mc.IsValidEdgeFunc = (from, to) => {
            if (neverConnect.Contains(from) || neverConnect.Contains(to)) return false;
            return true;
        };
    }

    private static void ConnectZone(MazeGraphZoned<Metadata> mc, int zone) {
        var cycles = mc.GetPotentialCycles();
        var connection = cycles
            .GetCyclesGreaterThan(0)
            .FirstOrDefault(c => c.nodeA.Zone == c.nodeB.Zone && c.nodeA.Zone == zone);
        if (connection == default) return;

        connection.nodeA.ConnectTo(connection.nodeB, new Metadata(true));
        connection.nodeB.ConnectTo(connection.nodeA, new Metadata(true));
    }

    private static void AddLongestCycles(MazeGraphZoned<Metadata> mc, int maxCycles) {
        var cycles = mc.GetPotentialCycles();
        var cyclesAdded = 0;
        while (cyclesAdded < maxCycles) {
            var connection = cycles
                .GetAllCycles()
                .FirstOrDefault(c => c.nodeA.Zone != c.nodeB.Zone);
            if (connection == default) break;

            connection.nodeA.ConnectTo(connection.nodeB, new Metadata(true));
            connection.nodeB.ConnectTo(connection.nodeA, new Metadata(true));
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

            connection.nodeA.ConnectTo(connection.nodeB, new Metadata(true));
            connection.nodeB.ConnectTo(connection.nodeA, new Metadata(true));
            cyclesAdded++;
            cycles.RemoveCycle(connection.nodeA, connection.nodeB);
        }
    }

    private static void ConnectNodes(Array2D<char> template, MazeGraphZoned<Metadata> mc) {
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

    private static void PrintGraph(MazeGraphZoned<Metadata> mc, List<ZoneCreated<Metadata>>? zones = null) {
        var allCanvas = new TextCanvas();
        var offset = 0;
        foreach (var node in mc.GetNodes()) {
            if (node == null) continue;
            var canvas = new TextCanvas();
            if (node.Up != null) canvas.Write(1, 0, node.GetEdgeTowards(Vector2I.Up)!.Metadata?.Added ?? false ? "·" : "|");
            if (node.Right != null) canvas.Write(2, 1, node.GetEdgeTowards(Vector2I.Right)!.Metadata?.Added ?? false ? "·" : "-");
            if (node.Down != null) canvas.Write(1, 2, node.GetEdgeTowards(Vector2I.Down)!.Metadata?.Added ?? false ? "·" : "|");
            if (node.Left != null) canvas.Write(0, 1, node.GetEdgeTowards(Vector2I.Left)!.Metadata?.Added ?? false ? "·" : "-");
            canvas.Write(1, 1, node.Zone.ToString());

            allCanvas.Write(offset + node.Position.X * 3, node.Position.Y * 3, canvas.ToString());
        }
        offset += (mc.Width * 3 + 5);
        foreach (var node in mc.GetNodes()) {
            if (node == null) continue;
            var canvas = new TextCanvas();
            if (node.Up != null) canvas.Write(1, 0, node.GetEdgeTowards(Vector2I.Up)!.Metadata?.Added ?? false ? "·" : "|");
            if (node.Right != null) canvas.Write(2, 1, node.GetEdgeTowards(Vector2I.Right)!.Metadata?.Added ?? false ? "·" : "-");
            if (node.Down != null) canvas.Write(1, 2, node.GetEdgeTowards(Vector2I.Down)!.Metadata?.Added ?? false ? "·" : "|");
            if (node.Left != null) canvas.Write(0, 1, node.GetEdgeTowards(Vector2I.Left)!.Metadata?.Added ?? false ? "·" : "-");
            canvas.Write(1, 1, node.Id.ToString());

            allCanvas.Write(offset + node.Position.X * 3, node.Position.Y * 3, canvas.ToString());
        }

        if (zones != null) {
            offset += (mc.Width * 3 + 5);
            foreach (var node in mc.GetNodes()) {
                if (node == null) continue;
                var canvas = new TextCanvas();
                if (node.Up != null) canvas.Write(1, 0, node.GetEdgeTowards(Vector2I.Up)!.Metadata?.Added ?? false ? "·" : "|");
                if (node.Right != null) canvas.Write(2, 1, node.GetEdgeTowards(Vector2I.Right)!.Metadata?.Added ?? false ? "····" : "----");
                if (node.Down != null) canvas.Write(1, 2, node.GetEdgeTowards(Vector2I.Down)!.Metadata?.Added ?? false ? "·" : "|");
                if (node.Left != null) canvas.Write(0, 1, node.GetEdgeTowards(Vector2I.Left)!.Metadata?.Added ?? false ? "·" : "-");

                var zone = zones[node.Zone];
                // var score = Mathf.RoundToInt(zone.NodeScores[node].GetKeyScore() * 100);
                var score = Mathf.RoundToInt(zone.NodeScores[node].ExitDistanceScore * 100);
                var loot = zone.TreasureLocations.Contains(node);
                /*if (zone.KeyLocation == node && loot) {
                    canvas.Write(1, 1, "!" + score);
                } else if (zone.KeyLocation == node) {
                    canvas.Write(1, 1, "*"+score);
                } else */if (loot) {
                    canvas.Write(1, 1, "$"+score);
                } else {
                    canvas.Write(1, 1, ""+score);
                }
                
                

                allCanvas.Write(offset + node.Position.X * 6, node.Position.Y * 3, canvas.ToString());
            }
        }
    
        Console.WriteLine(allCanvas.ToString());
    }


}