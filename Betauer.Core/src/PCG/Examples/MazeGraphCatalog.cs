using System;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Betauer.Core.PCG.Maze.Zoned;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class MazeGraphCatalog {
    public static float KeyFormula(NodeScore score) => (score.DeadEndScore * 0.4f + score.EntryDistanceScore * 0.3f + score.ExitDistanceScore * 0.3f) * 0.5f + (score.BelongsToPathToExit ? 0.0f : 0.5f);
    public static float LootFormula(NodeScore score) => score.DeadEndScore * 0.6f + score.EntryDistanceScore * 0.4f;

    public static MazeZones BigCycle(Random rng, Action<MazeGraph>? config = null) {
        var template = Array2D.Parse("""
                                     ···#######···
                                     ··#########··
                                     ·###+---+####
                                     ####|·#·|##·#
                                     #·##|#·#|##·#
                                     #·##|·o·|####
                                     ####+---+###·
                                     ··####·####··
                                     ···#######···
                                     """);
        var limit = template.Count(d => d.Value != '·');
        var start = template.FirstOrDefault(dataCell => dataCell.Value == 'o')!.Position;
        var constraints = new MazeZonedConstraints(4, limit)
            .SetCorridors(false)
            .SetPartsPerZone(3);
        var maze = MazeGraph.Create(template.Width, template.Height);
        maze.AddPositionValidator(pos => template[pos] != '·');
        config?.Invoke(maze);
        var zones = maze.GrowZoned(start, constraints, rng);
        ConnectNodes(template, maze);
        // MazeGraphTools.ConnectLongestCycles(maze, zones.Zones.Count * 2).ForEach(c => {
        //     c.nodeA.GetEdgeTo(c.nodeB)!.SetAttribute("cycle", true);
        //     c.nodeB.GetEdgeTo(c.nodeA)!.SetAttribute("cycle", true);
        // });
        return zones;
    }

    /// <summary>
    /// Create a maze where the final goal is a two nodes corridor very close to the zone 0.
    /// </summary>      aa
    /// <param name="rng"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static MazeZones BigWithShortDirectPath(Random rng, Action<MazeGraph>? config = null) {
        var constraints = new MazePerZoneConstraints()
            .Zone(nodes: 8)
            .Zone(nodes: 2, parts: 1, maxExitNodes: 0, corridor: true, flexibleParts: false)
            .Zone(nodes: 8, parts: 2)
            .Zone(nodes: 8, parts: 2)
            .Zone(nodes: 8, parts: 2)
            .Zone(nodes: 8, parts: 2);

        var maze = new MazeGraph();
        config?.Invoke(maze);
        var zones = maze.GrowZoned(Vector2I.Zero, constraints, rng);

        // Zone 1 is the goal, so we shift the zone 2 until the last one. The goal is the last zone now, but it's still close to the zone 0
        for (var i = 1; i < zones.Zones.Count - 1; i++) {
            zones.SwapZoneParts(i, i + 1);
        }
        maze.NeverConnectZone(zones.Zones.Count - 1);

        MazeGraphTools.ConnectLongestCyclesInAllZones(zones).ForEach(c => {
            c.nodeA.GetEdgeTo(c.nodeB)!.SetAttribute("cycle", true);
            c.nodeB.GetEdgeTo(c.nodeA)!.SetAttribute("cycle", true);
        });
        return zones;
    }

    /// <summary>
    /// Medium start node and a lot of corridors 
    /// </summary>
    /// <param name="rng"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static MazeZones Labyrinth(Random rng, Action<MazeGraph>? config = null) {
        var constraints = new MazePerZoneConstraints()
            .Zone(nodes: 5, corridor: true)
            .Zone(nodes: 6, parts: 1, maxExitNodes: 0, corridor: true)
            .Zone(nodes: 6, parts: 1, maxExitNodes: 0, corridor: true)
            .Zone(nodes: 6, parts: 1, maxExitNodes: 0, corridor: true)
            .Zone(nodes: 6, parts: 1, maxExitNodes: 0, corridor: true)
            .Zone(nodes: 2, parts: 1, maxExitNodes: 0, corridor: true, flexibleParts: false);
        var maze = new MazeGraph();
        config?.Invoke(maze);
        var zones = maze.GrowZoned(Vector2I.Zero, constraints, rng);
        maze.ConnectLongestCycles(zones.Zones.Count).ForEach(c => {
            c.nodeA.GetEdgeTo(c.nodeB)!.SetAttribute("cycle", true);
            c.nodeB.GetEdgeTo(c.nodeA)!.SetAttribute("cycle", true);
        });
        // Add as many cycles as zones
        // MazeGraphTools.AddLongestCycles(maze, zones.Zones.Count);
        return zones;
    }

    /// <summary>
    /// Medium start node and a lot of corridors 
    /// </summary>
    /// <param name="rng"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static MazeZones Cogmind(Random rng, Action<MazeGraph>? config = null) {
        var constraints = new MazePerZoneConstraints()
            .Zone(nodes: 4, corridor: true)
            .Zone(nodes: 9, parts: 3, maxExitNodes: 0)
            .Zone(nodes: 4, parts: 2, maxExitNodes: 0)
            .Zone(nodes: 4, corridor: true)
            .Zone(nodes: 9, parts: 3, maxExitNodes: 0)
            .Zone(nodes: 4, parts: 2, maxExitNodes: 0);
        var maze = new MazeGraph();
        config?.Invoke(maze);
        var zones = maze.GrowZoned(Vector2I.Zero, constraints, rng);
        maze.NeverConnectZone(0);
        maze.NeverConnectZone(3);
        maze.ConnectLongestCycles(zones.Zones.Count).ForEach(c => {
            c.nodeA.GetEdgeTo(c.nodeB)!.SetAttribute("cycle", true);
            c.nodeB.GetEdgeTo(c.nodeA)!.SetAttribute("cycle", true);
        });
        // Add as many cycles as zones
        // MazeGraphTools.AddLongestCycles(maze, zones.Zones.Count);
        return zones;
    }

    /// <summary>
    /// Medium start node and a lot of corridors 
    /// </summary>
    /// <param name="rng"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static MazeZones CogmindLong(Random rng, Action<MazeGraph>? config = null) {
        var constraints = new MazePerZoneConstraints()
            .Zone(nodes: 12, corridor: true)
            .Zone(nodes: 9, parts: 3, maxExitNodes: 0)
            .Zone(nodes: 9, parts: 3, maxExitNodes: 0)
            .Zone(nodes: 9, parts: 3, maxExitNodes: 0)
            .Zone(nodes: 9, parts: 3, maxExitNodes: 0);
        var maze = new MazeGraph();
        config?.Invoke(maze);
        var zones = maze.GrowZoned(Vector2I.Zero, constraints, rng);
        maze.NeverConnectZone(0);
        maze.NeverConnectZone(3);
        maze.ConnectLongestCycles(zones.Zones.Count).ForEach(c => {
            c.nodeA.GetEdgeTo(c.nodeB)!.SetAttribute("cycle", true);
            c.nodeB.GetEdgeTo(c.nodeA)!.SetAttribute("cycle", true);
        });
        // Add as many cycles as zones
        // MazeGraphTools.AddLongestCycles(maze, zones.Zones.Count);
        return zones;
    }

    private static void ConnectNodes(Array2D<char> template, MazeGraph mc) {
        template
            .Where(dataCell => dataCell.Value == '-')
            .Select(dataCell => mc.GetNodeAtOrNull(dataCell.Position)!)
            .Where(node => node != null!)
            .ForEach(from => {
                from.TryConnectTowards(Vector2I.Left)?.SetAttribute("cycle", true);
                from.Left?.TryConnectTowards(Vector2I.Right)?.SetAttribute("cycle", true);
                from.TryConnectTowards(Vector2I.Right)?.SetAttribute("cycle", true);
                from.Right?.TryConnectTowards(Vector2I.Left)?.SetAttribute("cycle", true);
            });

        template
            .Where(dataCell => dataCell.Value == '|')
            .Select(dataCell => mc.GetNodeAtOrNull(dataCell.Position)!)
            .Where(node => node != null!)
            .ForEach(from => {
                from.TryConnectTowards(Vector2I.Up)?.SetAttribute("cycle", true);
                from.Up?.TryConnectTowards(Vector2I.Down)?.SetAttribute("cycle", true);
                from.TryConnectTowards(Vector2I.Down)?.SetAttribute("cycle", true);
                from.Down?.TryConnectTowards(Vector2I.Up)?.SetAttribute("cycle", true);
            });

        template
            .Where(dataCell => dataCell.Value == '+' || dataCell.Value == 'o')
            .Select(dataCell => mc.GetNodeAt(dataCell.Position))
            .Where(node => node != null!)
            .ForEach(from => {
                from.TryConnectTowards(Vector2I.Up)?.SetAttribute("cycle", true);
                from.Up?.TryConnectTowards(Vector2I.Down)?.SetAttribute("cycle", true);
                from.TryConnectTowards(Vector2I.Down)?.SetAttribute("cycle", true);
                from.Down?.TryConnectTowards(Vector2I.Up)?.SetAttribute("cycle", true);
                from.TryConnectTowards(Vector2I.Left)?.SetAttribute("cycle", true);
                from.Left?.TryConnectTowards(Vector2I.Right)?.SetAttribute("cycle", true);
                from.TryConnectTowards(Vector2I.Right)?.SetAttribute("cycle", true);
                from.Right?.TryConnectTowards(Vector2I.Left)?.SetAttribute("cycle", true);
            });
    }
}