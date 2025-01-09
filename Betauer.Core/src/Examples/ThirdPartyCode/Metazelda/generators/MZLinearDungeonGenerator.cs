using System.Collections;
using System.Collections.Generic;


/**
 * Extends MZDungeonGenerator to choose the least nonlinear one immediately
 * available. This saves the player from having to do a lot of backtracking.
 * 
 * Ignores switches for now.
 *
public class LinearMZDungeonGenerator extends MZDungeonGenerator {
    
    public static final int MAX_ATTEMPTS = 10;

    public LinearMZDungeonGenerator(ILogger logger, long seed,
            IMZDungeonConstraints constraints) {
        super(logger, seed, constraints);
    }
    
    public LinearMZDungeonGenerator(long seed, IMZDungeonConstraints constraints) {
        this(null, seed, constraints);
    }
    
    private class AStarClient implements AStar.IClient<int> {
        
        private int keyLevel;
        
        public AStarClient(int keyLevel) {
            this.keyLevel = keyLevel;
        }

        @Override
        public List<int> GetNeighbors(int roomId) {
            List<int> ids = new List<int>();
            for (MZEdge edge: dungeon.Get(roomId).GetEdges()) {
                if (!edge.HasSymbol() || edge.GetSymbol().GetValue() < keyLevel) {
                    ids.Add(edge.GetTargetRoomId());
                }
            }
            return ids;
        }

        @Override
        public Vector2I GetVector2I(int roomId) {
            return dungeon.Get(roomId).GetCenter();
        }
    }
    
    private List<int> astar(int start, int goal, final int keyLevel) {
        AStar<int> astar = new AStar<int>(new AStarClient(keyLevel), start, goal);
        return astar.solve();
    }
    
    /**
     * Nonlinearity is measured as the number of rooms the player would have to
     * pass through multiple times to Get to the goal room (collecting keys and
     * unlocking doors along the way).
     * 
     * Uses A* to find a path from the entry to the first key, from each key to
     * the next key and from the last key to the goal.
     * 
     * @return  The number of rooms passed through multiple times
     *
    public int measureNonlinearity() {
        List<MZRoom> keyRooms = new List<MZRoom>(constraints.GetMaxKeys());
        for (int i = 0; i < constraints.GetMaxKeys(); ++i) {
            keyRooms.Add(null);
        }
        foreach (MZRoom room in dungeon.GetRooms()) {
            if (room.GetItem() == null) continue;
            MZSymbol item = room.GetItem();
            if (item.GetValue() >= 0 && item.GetValue() < keyRooms.Count)
                keyRooms.Set(item.GetValue(), room);
        }
        // for N >= 0: keyRooms[N] = location of key N
        
        MZRoom current = dungeon.FindStart(),
                goal = dungeon.FindGoal();
        // Clients may disable generation of the goal room -- the equivalent
        // 'ending' room becomes the boss room.
        if (goal == null) goal = dungeon.FindBoss();
        assert current != null && goal != null;
        int nextKey = 0, nonlinearity = 0;
        
        List<int> visitedRooms = new List<int>();
        while (current != goal) {
            MZRoom intermediateGoal;
            if (nextKey == constraints.GetMaxKeys())
                intermediateGoal = goal;
            else
                intermediateGoal = keyRooms.Get(nextKey);
            
            List<int> steps = astar(current.id, intermediateGoal.id,
                    nextKey);
            for (int id: steps) {
                if (visitedRooms.Contains(id)) ++nonlinearity;
            }
            visitedRooms.addAll(steps);
            
            nextKey++;
            current = dungeon.Get(steps.Get(steps.Count-1));
        }
        return nonlinearity;
    }

    @Override
    public void Generate() {
        int attempts = 0, currentNonlinearity = Int32.MaxValue;
        int bestAttempt = 0;
        MZDungeon currentBest = null;
        while (attempts++ < MAX_ATTEMPTS) {
            base.Generate();
            
            int nonlinearity = measureNonlinearity();
            if (debug) Console.WriteLine("MZDungeon " + attempts + " nonlinearity: "+
                    nonlinearity);
            if (nonlinearity < currentNonlinearity) {
                currentNonlinearity = nonlinearity;
                bestAttempt = attempts;
                currentBest = dungeon;
            }
        }
        assert currentBest != null;
        if (debug) Console.WriteLine("Chose " + bestAttempt + " nonlinearity: "+
                currentNonlinearity);
        dungeon = currentBest;
    }

}
    */