using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Betauer.Core.PCG.Maze.Zoned;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class MazeGraphCatalog {
    /// <summary>
    /// Create a maze where the zone 1 is a 2 nodes corridor. This will be the real goal of the maze instead of the last one.
    /// So, key in zone 0 opens the zone 2. And the last zone will have the zone 1 key.
    /// </summary>
    /// <param name="rng"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static MazeZones BigCycle(Random rng, Action<MazeGraph>? config = null) {
        var template = Array2D.Parse("""
                                     ###########
                                     ###########
                                     ###+---+###
                                     ###+···+###
                                     ###++o++###
                                     #####·#####
                                     ###########
                                     """);
        var constraints = new MazeZonedConstraints(4)
            .SetRandomNodesPerZone(4, 10)
            .SetPartsPerZone(2)
            .SetMaxExitNodes(2);
        var maze = new MazeGraph();
        maze.AddPositionValidator(pos => template[pos] != '·');
        config?.Invoke(maze);
        var zones = maze.GrowZoned(Vector2I.Zero, constraints, rng);
        MazeGraphTools.NeverConnectZone(maze, 1);
        MazeGraphTools.AddZonedLongestCycles(zones); // Tries one cycle per zone
        // Add as many cycles as zones
        MazeGraphTools.AddLongestCycles(maze, zones.Zones.Count);
        return zones;
    }

    /// <summary>
    /// Create a maze where the zone 1 is a 2 nodes corridor. This will be the real goal of the maze instead of the last one.
    /// So, key in zone 0 opens the zone 2. And the last zone will have the zone 1 key.
    /// </summary>
    /// <param name="rng"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static MazeZones BigWithShortDirectPath(Random rng, Action<MazeGraph>? config = null) {
        var constraints = new MazePerZoneConstraints()
            .Zone(nodes: 18)
            .Zone(nodes: 2, parts: 1, maxExitNodes: 0, corridor: true, flexibleParts: false)
            .Zone(nodes: 8, parts: 2)
            .Zone(nodes: 8, parts: 2)
            .Zone(nodes: 8, parts: 2);
        var maze = new MazeGraph();
        config?.Invoke(maze);
        var zones = maze.GrowZoned(Vector2I.Zero, constraints, rng);
        MazeGraphTools.NeverConnectZone(maze, 1);
        MazeGraphTools.AddZonedLongestCycles(zones); // Tries one cycle per zone
        // Add as many cycles as zones
        MazeGraphTools.AddLongestCycles(maze, zones.Zones.Count);
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
        MazeGraphTools.AddZonedLongestCycles(zones); // Tries one cycle per zone
        // Add as many cycles as zones
        MazeGraphTools.AddLongestCycles(maze, zones.Zones.Count);
        return zones;
    }

    private static void ConnectNodes(Array2D<char> template, MazeGraph mc) {
        template
            .Where(dataCell => dataCell.Value == '<')
            .Select(dataCell => mc.GetNodeAtOrNull(dataCell.Position)!)
            .Where(node => node != null!)
            .ForEach(from => {
                var to = mc.GetNodeAtOrNull(from.Position + Vector2I.Left);
                if (to != null && !from.HasEdgeTo(to) && mc.CanConnect(from, to)) {
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
                    .Where(to => to != null && mc.CanConnect(to, from))
                    .ForEach(to => {
                        if (!from.HasEdgeTo(to)) {
                            from.ConnectTo(to, new Metadata(true));
                            to.ConnectTo(from, new Metadata(true));
                        }
                    });
            });
    }
}

public static class MazeGraphTools {
    /// <summary>
    /// Find and connect the shortest cycles in the maze.
    /// </summary>
    /// <param name="mc"></param>
    /// <param name="maxCycles"></param>
    /// <param name="zone">Optional</param>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> AddShortestCycles(MazeGraph mc, int maxCycles = 1, int? zone = null) {
        var cycles = mc.GetPotentialCycles();
        List<(MazeNode nodeA, MazeNode nodeB, int distance)> cyclesAdded = [];

        while (cyclesAdded.Count < maxCycles) {
            var cycle = cycles
                .GetAllCycles(false)
                .FirstOrDefault(c => c.nodeA.ZoneId != c.nodeB.ZoneId && (!zone.HasValue || c.nodeA.ZoneId == zone.Value));
            if (cycle == default) break;

            cycle.nodeA.ConnectTo(cycle.nodeB, new Metadata(true));
            cycle.nodeB.ConnectTo(cycle.nodeA, new Metadata(true));
            cyclesAdded.Add(cycle);
            cycles.RemoveCycle(cycle.nodeA, cycle.nodeB);
        }
        return cyclesAdded;
    }

    /// <summary>
    /// Find and connect the longest cycles in the maze.
    /// </summary>
    /// <param name="mc"></param>
    /// <param name="maxCycles"></param>
    /// <param name="zone">Optional</param>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> AddLongestCycles(MazeGraph mc, int maxCycles = 1, int? zone = null) {
        var cycles = mc.GetPotentialCycles();
        List<(MazeNode nodeA, MazeNode nodeB, int distance)> cyclesAdded = [];

        while (cyclesAdded.Count < maxCycles) {
            var cycle = cycles
                .GetAllCycles()
                .FirstOrDefault(c => c.nodeA.ZoneId != c.nodeB.ZoneId && (!zone.HasValue || c.nodeA.ZoneId == zone.Value));
            if (cycle == default) break;

            cycle.nodeA.ConnectTo(cycle.nodeB, new Metadata(true));
            cycle.nodeB.ConnectTo(cycle.nodeA, new Metadata(true));
            cyclesAdded.Add(cycle);
            cycles.RemoveCycle(cycle.nodeA, cycle.nodeB);
        }
        return cyclesAdded;
    }

    /// <summary>
    /// Create cycles in each zone of the maze, connecting the longest paths. 
    /// </summary>
    /// <param name="zones"></param>
    /// <param name="maxCycles"></param>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> AddZonedLongestCycles(MazeZones zones, int maxCycles = 1) {
        List<(MazeNode nodeA, MazeNode nodeB, int distance)> cyclesAdded = [];
        foreach (var zone in zones.Zones) {
            Enumerable.Range(0, zone.Parts.Count).ForEach(_ => { cyclesAdded.AddRange(AddLongestCycles(zones.MazeGraph, maxCycles, zone.ZoneId)); });
        }
        return cyclesAdded;
    }

    /// <summary>
    /// Create cycles in each zone of the maze, connecting the shortest paths. 
    /// </summary>
    /// <param name="zones"></param>
    /// <param name="maxCycles"></param>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> AddZonedShortestCycles(MazeZones zones, int maxCycles = 1) {
        List<(MazeNode nodeA, MazeNode nodeB, int distance)> cyclesAdded = [];
        foreach (var zone in zones.Zones) {
            Enumerable.Range(0, zone.Parts.Count).ForEach(_ => { cyclesAdded.AddRange(AddShortestCycles(zones.MazeGraph, maxCycles, zone.ZoneId)); });
        }
        return cyclesAdded;
    }

    public static void NeverConnectZone(MazeGraph mc, int zone) {
        mc.AddEdgeValidator((from, to) => from.ZoneId != zone && to.ZoneId != zone);
    }
}