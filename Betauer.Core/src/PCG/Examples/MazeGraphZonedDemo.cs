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

        /*
        var mc = new MazeGraph(template.Width, template.Height) {
               IsValidPositionFunc = pos => template[pos] != '·'
           };
           var start = template.FirstOrDefault(dataCell => dataCell.Value == 'o')!.Position;
        mc.GrowZoned(start, new MazeZonedConstraints(4, 200)
            // .SetNodesPerZones(5)
            .SetRandomNodesPerZone(4, 10, rng)
            .SetPartsPerZone(2)
            .SetMaxExitNodes(2), rng);
        // PrintGraph(mc);
        */

        for (var i = 0; i < 20; i++) {
            var mc = new MazeGraph() {
                // IsValidPositionFunc = true // pos => template[pos] != '·'
            };
            var start = template.FirstOrDefault(dataCell => dataCell.Value == 'o')!.Position;
            var corridor = false;
            var constraints = new MazePerZoneConstraints()
                    .Zone(nodes: 18, corridor: false)
                    .Zone(nodes: 8, parts: 2, corridor: true)
                    .Zone(nodes: 2, parts: 1, maxExitNodes: 0, corridor: true, flexibleParts: false)
                    .Zone(nodes: 8, parts: 2, corridor: true)
                    .Zone(nodes: 8, parts: 2, corridor: false)
                    .Zone(nodes: 8, parts: 2, corridor: false)
                    .Zone(nodes: 8, parts: 2, corridor: false)
                    .Zone(nodes: 2, parts: 1, maxExitNodes: 0, corridor: true, flexibleParts: false)
                ;
            /*
            var constraints = new MazePerZoneConstraints()
                    .Zone(nodes: 5)
                    .Zone(nodes: 6, parts: 1, maxExitNodes: 0, corridor: true)
                    .Zone(nodes: 6, parts: 1, maxExitNodes: 0, corridor: true)
                    .Zone(nodes: 6, parts: 1, maxExitNodes: 0, corridor: true)
                    .Zone(nodes: 6, parts: 1, maxExitNodes: 0, corridor: true)
                    // .Zone(nodes: 6, parts: 1, maxExitNodes: 0, corridor: true)
                ;
                */

            // PENDIENTE DE HACER
            // 4 colocar objeto llave donde toca, el ultimo es la salida o el jefe
            // calcular linearidad: calcular las llaves en orden y calcular la ruta entre ellas
            // [salida, llave1, llave2, llave3, jefe]
            // cuantas veces pasa por cada nodo, y si se deja nodos sin pasar
            // calcular dificultad segun cercania al boss?

            mc.OnNodeCreated += (node) => {
                // PrintGraph(mc);
            };

            // Console.WriteLine(i);
            var zones = mc.GrowZoned(new Vector2I(0, 0), constraints, rng);

            // PrintGraph(mc);

            // foreach (var zone in zones.Zones) {
            //     Console.WriteLine($"Zone {zone.ZoneId} Nodes: {zone.NodeCount} Parts: {zone.Parts.Count}/{constraints.GetParts(zone.ZoneId)} Exits: {zone.GetAllExitNodes().Count()}/{constraints.GetMaxExitNodes(zone.ZoneId)}");
            // }

            // PrintGraph(mc);

            // NeverConnectZone(mc, 2);
            // Console.WriteLine("Connecting nodes with symbols");
            // ConnectNodes(template, mc);

            foreach (var zone in zones.Zones) {
                Enumerable.Range(0, zone.Parts.Count).ForEach(_ => {
                    var (nodeA, nodeB, distance) = ConnectZone(mc, zone.ZoneId);
                    if (distance > 0) {
                        // Console.WriteLine($"Zone {zone.ZoneId}: Connected {nodeA.Id} to {nodeB.Id} with distance {distance}");
                    }
                });
            }
            // Console.WriteLine("Longest cycles");
            AddLongestCycles(mc, zones.Zones.Count);
            // Console.WriteLine("Shortest cycles");
            // AddShortestCycles(mc, 3);

            zones.CalculateNodeScores();
            PrintGraph(mc, zones);
        }
    }

    private static void NeverConnectZone(MazeGraph mc, int zone) {
        var neverConnect = mc.GetNodes().Where(n => n.ZoneId == zone).Select(node => node.Position).ToList();
        mc.IsValidEdgeFunc = (from, to) => {
            if (neverConnect.Contains(from) || neverConnect.Contains(to)) return false;
            return true;
        };
    }

    private static (MazeNode nodeA, MazeNode nodeB, int distance) ConnectZone(MazeGraph mc, int zone) {
        var cycles = mc.GetPotentialCycles();
        var connection = cycles
            .GetCyclesGreaterThan(0)
            .FirstOrDefault(c => c.nodeA.ZoneId == c.nodeB.ZoneId && c.nodeA.ZoneId == zone);
        if (connection == default) return (null, null, 0);
        connection.nodeA.ConnectTo(connection.nodeB, new Metadata(true));
        connection.nodeB.ConnectTo(connection.nodeA, new Metadata(true));
        return connection;
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
        var keys = zones?.GetBestLocationsByZone(KeyFormula);
        var loot = zones?.SpreadLocationsByZone(0.3f, LootFormula);
        var mazeOffset = mc.GetOffset();
        var width = mc.GetSize().X;
        foreach (var node in mc.GetNodes()) {
            if (node == null) continue;
            var canvas = new TextCanvas();
            if (node.Up != null) canvas.Write(1, 0, node.GetEdgeTowards(Vector2I.Up)!.GetMetadataOrNew<Metadata>().Added ? "·" : "|");
            if (node.Right != null) canvas.Write(2, 1, node.GetEdgeTowards(Vector2I.Right)!.GetMetadataOrNew<Metadata>().Added ? "·" : "-");
            if (node.Down != null) canvas.Write(1, 2, node.GetEdgeTowards(Vector2I.Down)!.GetMetadataOrNew<Metadata>().Added ? "·" : "|");
            if (node.Left != null) canvas.Write(0, 1, node.GetEdgeTowards(Vector2I.Left)!.GetMetadataOrNew<Metadata>().Added ? "·" : "-");
            if (zones != null) {
                var zone = zones.Zones[node.ZoneId];
                var nodeWithKeyInZone = keys[zone.ZoneId];
                var isKey = nodeWithKeyInZone == node;
                canvas.Write(1, 1, node.ZoneId.ToString() + (isKey ? "!" : ""));
                // canvas.Write(1, 1, node.ZoneId.ToString()+node.PartId.ToString());
            } else {
                canvas.Write(1, 1, node.ZoneId.ToString());
                // canvas.Write(1, 1, node.ZoneId.ToStri/**/ng()+node.PartId.ToString());
            }
            allCanvas.Write(offset + (node.Position.X - mazeOffset.X) * 3, (node.Position.Y - mazeOffset.Y) * 3, canvas.ToString());
        }
        offset += (width * 3 + 5);
        foreach (var node in mc.GetNodes()) {
            if (node == null) continue;
            var canvas = new TextCanvas();
            if (node.Up != null) canvas.Write(1, 0, node.GetEdgeTowards(Vector2I.Up)!.GetMetadataOrNew<Metadata>().Added ? "·" : "|");
            if (node.Right != null) canvas.Write(2, 1, node.GetEdgeTowards(Vector2I.Right)!.GetMetadataOrNew<Metadata>().Added ? "·" : "-");
            if (node.Down != null) canvas.Write(1, 2, node.GetEdgeTowards(Vector2I.Down)!.GetMetadataOrNew<Metadata>().Added ? "·" : "|");
            if (node.Left != null) canvas.Write(0, 1, node.GetEdgeTowards(Vector2I.Left)!.GetMetadataOrNew<Metadata>().Added ? "·" : "-");
            canvas.Write(1, 1, node.Id.ToString());

            allCanvas.Write(offset + (node.Position.X - mazeOffset.X) * 3, (node.Position.Y - mazeOffset.Y) * 3, canvas.ToString());
        }

        if (zones != null) {
            List<MazeNode> route = [mc.Root];
            foreach (var zone in zones.Zones) {
                route.Add(keys[zone.ZoneId]);
            }

            zones.CalculateSolution(KeyFormula);
            // zones.CalculateSolution(KeyFormula, [1, 2, 3, 4, 5, 6, 7]);
            // zones.CalculateSolution(KeyFormula, [0, 1, 2, 3, 4, 5, 6, 7]);
            // zones.CalculateSolution(KeyFormula, [0, 1, 3, 4, 5, 6, 7, 2]);
            // zones.CalculateSolution(KeyFormula, [1, 4, 6, 5, 3, 7, 2]); // This fail if you don't take into account the key usage

            offset += width * 3 + 5;
            foreach (var node in mc.GetNodes()) {
                if (node == null) continue;
                var canvas = new TextCanvas();
                if (node.Up != null) canvas.Write(1, 0, node.GetEdgeTowards(Vector2I.Up)!.GetMetadataOrNew<Metadata>().Added ? "·" : "|");
                if (node.Right != null) canvas.Write(2, 1, node.GetEdgeTowards(Vector2I.Right)!.GetMetadataOrNew<Metadata>().Added ? "····" : "----");
                if (node.Down != null) canvas.Write(1, 2, node.GetEdgeTowards(Vector2I.Down)!.GetMetadataOrNew<Metadata>().Added ? "·" : "|");
                if (node.Left != null) canvas.Write(0, 1, node.GetEdgeTowards(Vector2I.Left)!.GetMetadataOrNew<Metadata>().Added ? "·" : "-");

                var zone = zones.Zones[node.ZoneId];

                var nodeScore = zones.Scores[node];
                // var score = Mathf.RoundToInt(KeyFormula(nodeScore) * 100);
                // var score = nodeScore.ExitDistanceScore * 100;
                var score = nodeScore.SolutionTraversals;

                var lootScore = Mathf.RoundToInt(LootFormula(nodeScore) * 100);
                var nodeWithKeyInZone = keys[zone.ZoneId];
                var isKey = nodeWithKeyInZone == node;

                // var score = Mathf.RoundToInt(zone.NodeScores[node].DeadEndScore * 100);
                var isLoot = loot[node.ZoneId].Contains(node);

                /*
                if (isLoot) {
                    canvas.Write(1, 1, ""+lootScore+"$");
                } else {
                    canvas.Write(1, 1, ""+lootScore);
                }
                */

                var isPartOfMainPath = zones.Scoring.SolutionPath.Contains(node);
                if (isLoot && isPartOfMainPath) {
                    canvas.Write(1, 1, $"{score:0}$");
                } else if (isLoot) {
                    canvas.Write(1, 1, $"{score:0}*");
                } else if (isPartOfMainPath) {
                    canvas.Write(1, 1, $"{score:0}");
                } else {
                    canvas.Write(1, 1, $"+");
                }
                allCanvas.Write(offset + (node.Position.X - mazeOffset.X) * 3, (node.Position.Y - mazeOffset.Y) * 3, canvas.ToString());
            }
            Console.WriteLine(allCanvas.ToString());

            zones.Scoring.VisitDistribution.ForEach((kv) => {
                // Console.WriteLine($"{kv.Key}: {kv.Value*100:00.0}%");
            });

            Console.WriteLine($"{mc.GetNodes().Count} nodes, solution path: {zones.Scoring.SolutionPath.Count} goal path: {zones.Scoring.GoalPath.Count}, redundancy: {zones.Scoring.Redundancy * 100:0}% gini {zones.Scoring.ConcentrationIndex * 100:0}%. {zones.Scoring.VisitDistribution.GetValueOrDefault(0, 0) * 100:0}% isolated from solution");
        } else {
            Console.WriteLine(allCanvas.ToString());
        }
    }

    public static float KeyFormula(NodeScore score) => (score.DeadEndScore * 0.4f + score.EntryDistanceScore * 0.3f + score.ExitDistanceScore * 0.3f) * 0.5f + (score.BelongsToPathToExit ? 0.0f : 0.5f);
    public static float LootFormula(NodeScore score) => score.DeadEndScore * 0.6f + score.EntryDistanceScore * 0.4f;
}