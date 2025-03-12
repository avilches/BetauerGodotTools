using System;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Betauer.Core.PCG.Maze.Zoned;
using Godot;

namespace Betauer.Core.Examples;

public class MazeGraphCatalog {
    public static float KeyFormula(NodeScore score) => (score.DeadEndScore * 0.4f + score.EntryDistanceScore * 0.3f + score.ExitDistanceScore * 0.3f) * 0.5f +
                                                       (score.BelongsToEntryExitPath ? 0.0f : 0.5f);

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
        var limit = template.Count(c => c != '·');
        var start = template.GetIndexedValues().FirstOrDefault(dataCell => dataCell.Value == 'o')!.Position;
        var constraints = new MazeZonedConstraints(4, limit)
            .SetCorridors(false)
            .SetPartsPerZone(3);
        var maze = MazeGraph.Create(template.Width, template.Height);
        maze.AddPositionValidator(pos => template[pos] != '·');
        config?.Invoke(maze);
        var zones = maze.GrowZoned(start, constraints, rng);
        ConnectNodes(template, maze);
        maze.ConnectLongestCycles(zones.Zones.Count * 2).ForEach(c => {
             c.nodeA.GetEdgeTo(c.nodeB)!.Attributes().Set("cycle", true);
             c.nodeB.GetEdgeTo(c.nodeA)!.Attributes().Set("cycle", true);
        });
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

        // Zone 1 is the goal, so we shift the zone 2 with the next one until the zone 2 becomes the last zone.
        // After this change, the goal (the zone 1) is the last zone now, but it's still reached from the zone 0. So, the player will see the entry of
        // the goal zone when it enters the zone 0.
        for (var i = 1; i < zones.Zones.Count - 1; i++) {
            zones.SwapZoneParts(i, i + 1);
        }
        maze.NeverConnectZone(zones.Zones.Count - 1);

        MazeGraphTools.ConnectLongestCyclesInAllZones(zones).ForEach(c => {
            c.nodeA.GetEdgeTo(c.nodeB)!.Attributes().Set("cycle", true);
            c.nodeB.GetEdgeTo(c.nodeA)!.Attributes().Set("cycle", true);
        });
        return zones;
    }


    public static MazeZones Mini(Random rng, Action<MazeGraph>? config = null) {
        var constraints = new MazePerZoneConstraints()
            .Zone(nodes: 5)
            .Zone(nodes: 8, parts: 4)
            .Zone(nodes: 8, parts: 4);

        var maze = new MazeGraph();
        config?.Invoke(maze);
        var zones = maze.GrowZoned(Vector2I.Zero, constraints, rng);

        MazeGraphTools.ConnectLongestCyclesInAllZones(zones).ForEach(c => {
            c.nodeA.GetEdgeTo(c.nodeB)!.Attributes().Set("cycle", true);
            c.nodeB.GetEdgeTo(c.nodeA)!.Attributes().Set("cycle", true);
        });
        return zones;
    }

    /// <summary>
    /// 4 zone of one single corridor, all of them start in the zone 0 
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
            c.nodeA.GetEdgeTo(c.nodeB)!.Attributes().Set("cycle", true);
            c.nodeB.GetEdgeTo(c.nodeA)!.Attributes().Set("cycle", true);
        });
        // Add as many cycles as zones
        // MazeGraphTools.AddLongestCycles(maze, zones.Zones.Count);
        return zones;
    }

    /// <summary>
    /// 4 zone of one single corridor, all of them start in the zone 0 
    /// </summary>
    /// <param name="rng"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static MazeZones City(Random rng, Action<MazeGraph>? config = null) {
        var constraints = new MazePerZoneConstraints()
            .Zone(nodes: 3, corridor: true)
            .Zone(nodes: 3, parts: 2, corridor: true)
            .Zone(nodes: 6, parts: 1, corridor: false)
            .Zone(nodes: 2, parts: 1, corridor: true, flexibleParts: false);
        var maze = new MazeGraph();
        config?.Invoke(maze);
        var zones = maze.GrowZoned(Vector2I.Zero, constraints, rng);
        maze.GetPotentialCycles().Query().WhereNodesInSameZone().ConnectAll().ForEach(c => {
            c.nodeA.GetEdgeTo(c.nodeB)!.Attributes().Set("cycle", true);
            c.nodeB.GetEdgeTo(c.nodeA)!.Attributes().Set("cycle", true);
        });
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
            c.nodeA.GetEdgeTo(c.nodeB)!.Attributes().Set("cycle", true);
            c.nodeB.GetEdgeTo(c.nodeA)!.Attributes().Set("cycle", true);
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
            c.nodeA.GetEdgeTo(c.nodeB)!.Attributes().Set("cycle", true);
            c.nodeB.GetEdgeTo(c.nodeA)!.Attributes().Set("cycle", true);
        });
        // Add as many cycles as zones
        // MazeGraphTools.AddLongestCycles(maze, zones.Zones.Count);
        return zones;
    }

    /*
     * Matches every node position with a char in the template.
     * If the template has:
     * "-" = connects the node with the right and left nodes (if they exist)
     * "|" = connects the node with the up and down nodes (if they exist)
     * "+" = connects the node with the right, left, up and down nodes (if they exist)
     */
    private static void ConnectNodes(Array2D<char> template, MazeGraph mc) {
        foreach (var from in template
                     .GetIndexedValues()
                     .Where(dataCell => dataCell.Value == '-')
                     .Select(dataCell => mc.GetNodeAtOrNull(dataCell.Position)!)
                     .Where(node => node != null!)) {
            from.TryConnectTowards(Vector2I.Left)?.Attributes().Set("cycle", true);
            from.Left?.TryConnectTowards(Vector2I.Right)?.Attributes().Set("cycle", true);
            from.TryConnectTowards(Vector2I.Right)?.Attributes().Set("cycle", true);
            from.Right?.TryConnectTowards(Vector2I.Left)?.Attributes().Set("cycle", true);
        }

        foreach (var from in template
                     .GetIndexedValues()
                     .Where(dataCell => dataCell.Value == '|')
                     .Select(dataCell => mc.GetNodeAtOrNull(dataCell.Position)!)
                     .Where(node => node != null!)) {
            from.TryConnectTowards(Vector2I.Up)?.Attributes().Set("cycle", true);
            from.Up?.TryConnectTowards(Vector2I.Down)?.Attributes().Set("cycle", true);
            from.TryConnectTowards(Vector2I.Down)?.Attributes().Set("cycle", true);
            from.Down?.TryConnectTowards(Vector2I.Up)?.Attributes().Set("cycle", true);
        }

        foreach (var from in template
                     .GetIndexedValues()
                     .Where(dataCell => dataCell.Value == '+' || dataCell.Value == 'o')
                     .Select(dataCell => mc.GetNodeAt(dataCell.Position))
                     .Where(node => node != null!)) {
            from.TryConnectTowards(Vector2I.Up)?.Attributes().Set("cycle", true);
            from.Up?.TryConnectTowards(Vector2I.Down)?.Attributes().Set("cycle", true);
            from.TryConnectTowards(Vector2I.Down)?.Attributes().Set("cycle", true);
            from.Down?.TryConnectTowards(Vector2I.Up)?.Attributes().Set("cycle", true);
            from.TryConnectTowards(Vector2I.Left)?.Attributes().Set("cycle", true);
            from.Left?.TryConnectTowards(Vector2I.Right)?.Attributes().Set("cycle", true);
            from.TryConnectTowards(Vector2I.Right)?.Attributes().Set("cycle", true);
            from.Right?.TryConnectTowards(Vector2I.Left)?.Attributes().Set("cycle", true);
        }
    }
}