using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze.Zoned;

public class Zone<T>(int zoneId) {
    public MazeZones<T> MazeZones { get; internal set; }
    public int ZoneId { get; } = zoneId;

    public List<ZonePart<T>> Parts { get; } = [];
    
    public float NodeCount => Parts.Sum(p => p.Nodes.Count);

    public IEnumerable<MazeNode<T>> GetNodes() => Parts.SelectMany(part => part.Nodes);
    
    /// <summary>
    /// A doors in node is a node that has an edge to a node from the previous zone. The GrowZoned method only creates one door in
    /// to every zone, stored in the StartNode field in MazeNodePart. But it could be possible to create more connections before, even
    /// from other zones (not only the previous one), so this method will return all possible doors in.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MazeNode<T>> GetDoorInNodes() {
        return Parts.SelectMany(p => p.GetDoorInNodes());
    }

    /// <summary>
    /// A doors out node is a node that has an edge to a node to next zone. The GrowZoned method only creates as many doors out
    /// as the MaxDoorsOut property of the zone, and store the number of doors out created in the DoorsOut field.
    /// But it could be possible to create more connections before, so this method will return all possible doors out.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MazeNode<T>> GetDoorOutNodes() {
        return Parts.SelectMany(p => p.GetDoorOutNodes());
    }
    
    /// <summary>
    /// Returns all node scores within a specific zone.
    /// </summary>
    public IEnumerable<NodeScore<T>> GetScores() {
        return MazeZones.Scores.Values.Where(score => score.Node.ZoneId == zoneId);
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
    /// Spreads a proportional number of locations within a zone based on the zone's size and the provided ratio.
    /// 
    /// Example usage:
    /// ```
    /// // Select 30% of the nodes in zone 1, prioritizing nodes far from exits
    /// var locations = SpreadLocationsInZone(1, 0.3f, score => score.ExitDistanceScore);
    /// ```
    /// </summary>
    public List<MazeNode<T>> SpreadLocations(float ratio, Func<NodeScore<T>, float> scoreCalculator) {
        var total = Mathf.RoundToInt(NodeCount * ratio);
        return SpreadLocations(total, scoreCalculator);
    }

    /// <summary>
    /// Spreads a specific number of locations within a zone, trying to maintain
    /// even distribution across zone parts.
    /// 
    /// Example usage:
    /// ```
    /// // Place 5 items in zone 2, where the nodes far from entrances has the highest chance
    /// var locations = SpreadLocationsInZone(2, 5, score => score.EntryDistanceScore);
    /// ```
    /// </summary>
    /// <param name="total">The total number of locations to place</param>
    /// <param name="scoreCalculator">The function used to score potential locations</param>
    public List<MazeNode<T>> SpreadLocations(int total, Func<NodeScore<T>, float> scoreCalculator) {
        var locations = new List<MazeNode<T>>();
        Console.WriteLine($"Zone {ZoneId} intentando colocar {total} tesoros en sus {NodeCount} nodos");
        if (total == 0) return locations;

        foreach (var part in Parts) {
            var partSize = part.Nodes.Count;
            var scoreNodesInPart = part.GetScores().ToList();
            // Calculate how many locations should go in this part based on its size relative to the zone
            var desired = Math.Max(1, Mathf.RoundToInt(total * (partSize / NodeCount)));
            SpreadLocationsAlgorithm.GetLocations(scoreNodesInPart, desired, scoreCalculator).ForEach(locations.Add);

            Console.WriteLine($"Zone {ZoneId} Part {part.PartId} treasures: {locations.Count(t => part.Nodes.Contains(t))}/{desired}");
        }

        Console.WriteLine($"Zone {ZoneId} total treasures: {locations.Count}/{total}");
        return locations;
    }
    
}