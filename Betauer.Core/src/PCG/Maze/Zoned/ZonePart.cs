using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze.Zoned;

public class ZonePart(Zone zone, int partId, MazeNode startNode, List<MazeNode> nodes) {
    public Zone Zone { get; internal set; } = zone;
    public int PartId { get; } = partId;
    public MazeNode StartNode { get; } = startNode;
    public List<MazeNode> Nodes { get; } = nodes;
    public int NodeCount => Nodes.Count;

    /// <summary>
    /// Contains all the shortest path between every entry and every exit node in the part.
    /// It doesn't contain all possible paths between entry and exit nodes, only the shortest ones.
    /// </summary>
    public List<List<MazeNode>> EntryExitPaths = [];
    
    /// <summary>
    /// Contains all the nodes that are part of the shortest path between every entry and every exit node in the part.
    /// It's useful to know if a node is part of a path between entry and exit nodes.
    /// </summary>
    public HashSet<MazeNode> EntryExitPathNodes = [];
    
    public void CalculateAllEntryToExitPaths() {
        var entryNodes = GetEntryNodesFromPreviousZone().ToList();
        var exitNodes = GetExitNodesToNextZone().ToList();
        EntryExitPaths.Clear();
        EntryExitPathNodes.Clear();
        foreach (var entryNode in entryNodes) {
            foreach (var exitNode in exitNodes) {
                var shortestPath = entryNode.FindShortestPath(exitNode);
                EntryExitPathNodes.UnionWith(shortestPath);
                if (shortestPath.Count > 1) { // 0 = no path, 1 = entry and exit are the same node
                    EntryExitPaths.Add(shortestPath);
                }
            }
        }
    }
    
    /// <summary>
    /// Entry nodes are nodes with an edge from a node in a zone with a zoneId lower than the current one.
    /// This method returns all possible entry nodes in the part from a zone with a lower zoneId.
    /// </summary>
    public IEnumerable<MazeNode> GetAllEntryNodes() {
        // The zone 0 or an isolated zone doesn't have entry nodes from previous zones, so we add the start node
        return Nodes.Where(n => n.GetInEdges().Any(edge => edge.From.ZoneId < Zone.ZoneId) || n == StartNode);
    }

    public IEnumerable<MazeNode> GetEntryNodesFromPreviousZone() {
        // The zone 0 or an isolated zone doesn't have entry nodes from previous zones, so we add the start node
        return Nodes.Where(n => n.GetInEdges().Any(edge => edge.From.ZoneId == Zone.ZoneId - 1) || n == StartNode);
    }

    public IEnumerable<MazeNode> GetEntryNodesFrom(int zoneId) {
        return Nodes.Where(n => n.GetInEdges().Any(edge => edge.From.ZoneId == zoneId));
    }

    /// <summary>
    /// Exit nodes are nodes with an edge to a node in a zone with a zoneId higher than the current one.
    /// This method returns all possible exit nodes in the part to a zone with a higher zoneId.
    /// </summary>
    public IEnumerable<MazeNode> GetAllExitNodes() {
        return Nodes.Where(n => n.GetOutEdges().Any(edge => edge.To.ZoneId > Zone.ZoneId));
    }
    
    public IEnumerable<MazeNode> GetExitNodesToNextZone() {
        return Nodes.Where(n => n.GetOutEdges().Any(edge => edge.To.ZoneId == Zone.ZoneId + 1));
    }

    public IEnumerable<MazeNode> GetExitNodesTo(int zoneId) {
        return Nodes.Where(n => n.GetOutEdges().Any(edge => edge.To.ZoneId == zoneId));
    }

    /// <summary>
    /// Returns all node scores within a specific part of a zone.
    /// </summary>
    public IEnumerable<NodeScore> GetScores() {
        return Zone.MazeZones.Scores.Values.Where(score => score.Node.ZoneId == Zone.ZoneId && score.Node.PartId == PartId);
    }

        /// <summary>
    /// Returns the node with the highest score within a specific zone based on the provided scoring function.
    /// 
    /// Example usage:
    /// ```
    /// // Get the node furthest from exits in zone 1
    /// var bestNode = GetBestLocationInZone(1, score => score.ExitDistanceScore);
    /// ```
    /// </summary>
    public MazeNode GetBestLocation(Func<NodeScore, float> scoreCalculator) {
        return GetScores().OrderByDescending(scoreCalculator).First().Node;
    }

    /// <summary>
    /// Spreads a proportional number of locations within the part based on the part's size and the provided ratio.
    /// 
    /// Example usage:
    /// ```
    /// // Select 30% of the nodes, prioritizing nodes far from exits
    /// var locations = SpreadLocations(0.3f, score => score.ExitDistanceScore);
    /// ```
    /// </summary>
    public List<MazeNode> SpreadLocations(float ratio, Func<NodeScore, float> scoreCalculator) {
        var total = Mathf.RoundToInt(Nodes.Count * ratio);
        return SpreadLocations(total, scoreCalculator);
    }

    /// <summary>
    /// Spreads a specific number of locations within the part, trying to maintain
    /// even distribution across zone parts.
    /// 
    /// Example usage:
    /// ```
    /// // Place 5 items, where the nodes far from entrances has the highest chance
    /// var locations = SpreadLocations(5, score => score.EntryDistanceScore);
    /// ```
    /// </summary>
    /// <param name="desired">The total number of locations to place</param>
    /// <param name="scoreCalculator">The function used to score potential locations</param>
    public List<MazeNode> SpreadLocations(int desired, Func<NodeScore, float> scoreCalculator) {
        return SpreadLocationsAlgorithm.GetLocations(GetScores().ToList(), desired, scoreCalculator);
    }
}