using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.PCG.Maze;
using Betauer.Core.PCG.Maze.Zoned;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class MazeGraphZonedDemo {
    public static void MainValidate() {
        var seed = 1;
        var rng = new Random(seed);
        var zones = ValidateMaze(() => MazeGraphCatalog.CogmindLong(rng, mc => {
            // mc.OnNodeCreated += (node) => PrintGraph(mc);
        }));
        zones.CalculateSolution(MazeGraphCatalog.KeyFormula);
        PrintGraph(zones.MazeGraph, zones);
    }

    public static void MainGenerate() {
        var seed = 1;
        var rng = new Random(seed);
        var zones = MazeGraphCatalog.CogmindLong(rng, mc => {
            // mc.OnNodeCreated += (node) => PrintGraph(mc);
        });
        zones.CalculateSolution(MazeGraphCatalog.KeyFormula);
        PrintGraph(zones.MazeGraph, zones);
        // var array2D = MazeGraphToArray2D.Convert(zones.MazeGraph, cellSize: 12, seed: 12345, expandProbability:0.3f);
        // Console.WriteLine(array2D.ToString());
    }

    private static MazeZones ValidateMaze(Func<MazeZones> func) {
        var totalNodes = 0;
        var totalSolutionPaths = 0;
        var totalGoalPaths = 0;
        var totalRedundancy = 0f;
        var totalConcentrationIndex = 0f;
        var totalIsolated = 0f;
        var exceptions = 0;
        var validMazes = 0;
        MazeZones lastValidMaze = null;
        const int tries = 40;

        for (var i = 0; i < tries; i++) {
            try {
                var zones = func();
                // PrintGraph(zones.MazeGraph);
                zones.CalculateSolution(MazeGraphCatalog.KeyFormula);
                Console.WriteLine($"{zones.GetNodes().Count:00} nodes, solution path: {zones.Scoring.SolutionPath.Count:00} goal path: {zones.Scoring.GoalPath.Count:00}, redundancy: {zones.Scoring.Redundancy * 100:00}% gini {zones.Scoring.ConcentrationIndex * 100:00}%. {zones.Scoring.VisitDistribution.GetValueOrDefault(0, 0) * 100:00}% isolated from solution");
                PrintGraph(zones.MazeGraph, zones);

                lastValidMaze = zones;
                validMazes++;
                totalNodes += zones.MazeGraph.GetNodes().Count;
                totalSolutionPaths += zones.Scoring.SolutionPath.Count;
                totalGoalPaths += zones.Scoring.GoalPath.Count;
                totalRedundancy += zones.Scoring.Redundancy;
                totalConcentrationIndex += zones.Scoring.ConcentrationIndex;
                totalIsolated += zones.Scoring.VisitDistribution.GetValueOrDefault(0, 0);
            } catch (NoMoreNodesException e) {
                Console.WriteLine(e.Message);
                exceptions++;
            }
        }

        if (validMazes > 0) {
            Console.WriteLine($"Valid mazes: {validMazes}/{tries} ({exceptions} exceptions)");
            Console.WriteLine($"-----------------------------------------------------------------------------------------------");
            Console.WriteLine($"{totalNodes / validMazes:00} nodes, solution path: {totalSolutionPaths / validMazes:00} goal path: {totalGoalPaths / validMazes:00}, redundancy: {(totalRedundancy / validMazes) * 100:00}% gini {(totalConcentrationIndex / validMazes) * 100:00}%. {(totalIsolated / validMazes) * 100:00}% isolated from solution");
            Console.WriteLine($"-----------------------------------------------------------------------------------------------");
        } else {
            Console.WriteLine("No valid mazes generated");
        }

        return lastValidMaze;
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
        var keys = zones?.GetBestLocationsByZone(MazeGraphCatalog.KeyFormula);
        var loot = zones?.SpreadLocationsByZone(0.3f, MazeGraphCatalog.LootFormula);
        var mazeOffset = mc.GetOffset();
        var width = mc.GetSize().X;
        foreach (var node in mc.GetNodes()) {
            if (node == null) continue;
            var canvas = new TextCanvas();
            if (node.Up != null) canvas.Write(1, 0, node.GetEdgeTowards(Vector2I.Up)!.HasAttribute("cycle") ? "·" : "|");
            if (node.Right != null) canvas.Write(2, 1, node.GetEdgeTowards(Vector2I.Right)!.HasAttribute("cycle") ? "····" : "----");
            if (node.Down != null) canvas.Write(1, 2, node.GetEdgeTowards(Vector2I.Down)!.HasAttribute("cycle") ? "·" : "|");
            if (node.Left != null) canvas.Write(0, 1, node.GetEdgeTowards(Vector2I.Left)!.HasAttribute("cycle") ? "·" : "-");
            if (zones != null) {
                var zone = zones.Zones[node.ZoneId];
                var nodeWithKeyInZone = keys[zone.ZoneId];
                var isKey = nodeWithKeyInZone == node;
                var isLoot = loot[node.ZoneId].Contains(node);
                canvas.Write(1, 1, node.ZoneId.ToString() + (isKey && isLoot ? "&" : isKey ? "!" : isLoot ? "$" : ""));
                // canvas.Write(1, 1, node.ZoneId.ToString()+node.PartId.ToString());
            } else {
                canvas.Write(1, 1, node.ZoneId.ToString());
                // canvas.Write(1, 1, node.ZoneId.ToStri/**/ng()+node.PartId.ToString());
            }
            allCanvas.Write(offset + (node.Position.X - mazeOffset.X) * 6, (node.Position.Y - mazeOffset.Y) * 3, canvas.ToString());
        }
        offset += (width * 6 + 8);
        foreach (var node in mc.GetNodes()) {
            if (node == null) continue;
            var canvas = new TextCanvas();
            if (node.Up != null) canvas.Write(1, 0, node.GetEdgeTowards(Vector2I.Up)!.HasAttribute("cycle") ? "·" : "|");
            if (node.Right != null) canvas.Write(2, 1, node.GetEdgeTowards(Vector2I.Right)!.HasAttribute("cycle") ? "····" : "----");
            if (node.Down != null) canvas.Write(1, 2, node.GetEdgeTowards(Vector2I.Down)!.HasAttribute("cycle") ? "·" : "|");
            if (node.Left != null) canvas.Write(0, 1, node.GetEdgeTowards(Vector2I.Left)!.HasAttribute("cycle") ? "·" : "-");
            canvas.Write(1, 1, node.Id.ToString());

            allCanvas.Write(offset + (node.Position.X - mazeOffset.X) * 6, (node.Position.Y - mazeOffset.Y) * 3, canvas.ToString());
        }

        if (zones != null) {
            List<MazeNode> route = [mc.Root];
            foreach (var zone in zones.Zones) {
                route.Add(keys[zone.ZoneId]);
            }

            // zones.CalculateSolution(KeyFormula, [1, 2, 3, 4, 5, 6, 7]);
            // zones.CalculateSolution(KeyFormula, [0, 1, 2, 3, 4, 5, 6, 7]);
            // zones.CalculateSolution(KeyFormula, [0, 1, 3, 4, 5, 6, 7, 2]);
            // zones.CalculateSolution(KeyFormula, [1, 4, 6, 5, 3, 7, 2]); // This fail if you don't take into account the key usage

            offset += width * 6 + 8;
            foreach (var node in mc.GetNodes()) {
                if (node == null) continue;
                var canvas = new TextCanvas();
                if (node.Up != null) canvas.Write(1, 0, node.GetEdgeTowards(Vector2I.Up)!.HasAttribute("cycle") ? "·" : "|");
                if (node.Right != null) canvas.Write(2, 1, node.GetEdgeTowards(Vector2I.Right)!.HasAttribute("cycle") ? "····" : "----");
                if (node.Down != null) canvas.Write(1, 2, node.GetEdgeTowards(Vector2I.Down)!.HasAttribute("cycle") ? "·" : "|");
                if (node.Left != null) canvas.Write(0, 1, node.GetEdgeTowards(Vector2I.Left)!.HasAttribute("cycle") ? "·" : "-");

                var zone = zones.Zones[node.ZoneId];

                var nodeScore = zones.Scores[node];
                // var score = Mathf.RoundToInt(KeyFormula(nodeScore) * 100);
                // var score = nodeScore.ExitDistanceScore * 100;
                var score = nodeScore.SolutionTraversals;

                var lootScore = Mathf.RoundToInt(MazeGraphCatalog.LootFormula(nodeScore) * 100);
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
                    canvas.Write(1, 1, $"*");
                } else if (isLoot) {
                    canvas.Write(1, 1, $"{score:0}$");
                } else if (isPartOfMainPath) {
                    canvas.Write(1, 1, $"*");
                } else {
                    canvas.Write(1, 1, $"+");
                }
                allCanvas.Write(offset + (node.Position.X - mazeOffset.X) * 6, (node.Position.Y - mazeOffset.Y) * 3, canvas.ToString());
            }
            Console.WriteLine(allCanvas.ToString());

            // foreach (var zone in zones.Zones) {
            // Console.WriteLine($"Zone {zone.ZoneId} Nodes: {zone.NodeCount} Parts: {zone.Parts.Count}/{zones.Constraints.GetParts(zone.ZoneId)} Exits: {zone.GetAllExitNodes().Count()}/{zones.Constraints.GetMaxExitNodes(zone.ZoneId)}");
            // }

            zones.Scoring.VisitDistribution.ForEach((kv) => {
                // Console.WriteLine($"{kv.Key}: {kv.Value*100:00.0}%");
            });
        } else {
            Console.WriteLine(allCanvas.ToString());
        }
    }
}