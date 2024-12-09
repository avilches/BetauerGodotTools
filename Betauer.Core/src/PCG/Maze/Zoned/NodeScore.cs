namespace Betauer.Core.PCG.Maze.Zoned;

public class NodeScore(MazeNode node, float deadEndScore, float entryDistanceScore, float exitDistanceScore = 0) {
    public MazeNode Node { get; } = node;
    
    /// <summary>
    /// 1 means the node is a dead end (only one connection)
    /// 0 means the node has the maximum number of connection.
    /// 
    /// So: if the maximum number of connections is 1, all nodes will have a score of 1.
    /// if the maximum number of connections is 3, the score will be
    ///    1 if the node is a dead end
    ///    0.5 if it has 1 connection
    ///    0 if it has 2 connection (like the maximum node)
    /// </summary>
    public float DeadEndScore { get; } = deadEndScore;

    /// <summary>
    /// Returns a score between 0 and 1.
    /// A door in is a connection from a zone with a lower id than the current one.
    /// 
    /// 1 means the node is the farthest from any door in,
    /// 0 means the node is the closest to any door in.
    ///
    /// The bigger, the more distance to walk to from the entrance (compared with other nodes in the same zone).
    /// </summary>
    /// <returns></returns>
    public float EntryDistanceScore { get; } = entryDistanceScore;
    
    
    /// <summary>
    /// Returns a score between 0 and 1.
    /// A door out is a connection to a zone with a higher id than the current one.
    /// 
    /// 1 means the node is the farthest from any door out,
    /// 0 means the node is the closest to any door out.
    ///
    /// The bigger, the more distance to walk to reach the exit (compared with other nodes in the same zone).
    /// </summary>
    /// <returns></returns>
    public float ExitDistanceScore { get; } = exitDistanceScore;
}