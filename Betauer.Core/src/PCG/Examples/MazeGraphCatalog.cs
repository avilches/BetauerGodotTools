using System;
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

        MazeGraphTools.ConnectLongestCyclesInAllZones(zones).ForEach(c => {
            c.nodeA.GetEdgeTo(c.nodeA)!.SetAttribute("cycle", true);
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
        MazeGraphTools.ConnectLongestCycles(maze, zones.Zones.Count).ForEach(c => {
            c.nodeA.GetEdgeTo(c.nodeB)!.SetAttribute("cycle", true);
            c.nodeB.GetEdgeTo(c.nodeA)!.SetAttribute("cycle", true);
        });
        // Add as many cycles as zones
        // MazeGraphTools.AddLongestCycles(maze, zones.Zones.Count);
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
                    from.ConnectTo(to).SetAttribute("cycle", true);
                    to.ConnectTo(from).SetAttribute("cycle", true);
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