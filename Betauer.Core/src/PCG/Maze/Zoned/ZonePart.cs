using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze.Zoned;

public class ZonePart<T>(Zone<T> zone, int partId, MazeNode<T> startNode, List<MazeNode<T>> nodes) {
    public Zone<T> Zone { get; } = zone;
    public int PartId { get; } = partId;
    public MazeNode<T> StartNode { get; } = startNode;
    public List<MazeNode<T>> Nodes { get; } = nodes;

    /// <summary>
    /// A door in node is a node that has an edge to a node from the previous zone. The GrowZoned method only creates one door in
    /// to every zone, stored in the StartNode field in MazeNodePart. But it could be possible to create more connections before, even
    /// from other zones (not only the previous one), so this method will return all possible doors in the part.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MazeNode<T>> GetDoorInNodes() {
        return Nodes.Where(n => n.GetInEdges().Any(edge => edge.From.ZoneId < Zone.ZoneId) || n == StartNode);
    }

    /// <summary>
    /// A doors out node is a node that has an edge to a node to next zone. The GrowZoned method only creates as many doors out
    /// as the MaxDoorsOut property of the zone, and store the number of doors out created in the DoorsOut field.
    /// But it could be possible to create more connections before, so this method will return all possible doors out.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MazeNode<T>> GetDoorOutNodes() {
        return Nodes.Where(n => n.GetOutEdges().Any(edge => edge.To.ZoneId > Zone.ZoneId));
    }
    
    /// <summary>
    /// Returns all node scores within a specific part of a zone.
    /// </summary>
    public IEnumerable<NodeScore<T>> GetScores() {
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
    public MazeNode<T> GetBestLocation(Func<NodeScore<T>, float> scoreCalculator) {
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
    public List<MazeNode<T>> SpreadLocations(float ratio, Func<NodeScore<T>, float> scoreCalculator) {
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
    public List<MazeNode<T>> SpreadLocations(int desired, Func<NodeScore<T>, float> scoreCalculator) {
        return SpreadLocationsAlgorithm.GetLocations(GetScores().ToList(), desired, scoreCalculator);
    }


}