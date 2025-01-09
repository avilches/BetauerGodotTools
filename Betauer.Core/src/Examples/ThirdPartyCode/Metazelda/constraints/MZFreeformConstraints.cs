using System;
using System.Collections.Generic;
using Betauer.Core.Examples.ThirdPartyCode.Metazelda.util;
using Godot;

namespace Betauer.Core.Examples.ThirdPartyCode.Metazelda.constraints;

public class FreeformConstraints : IMZDungeonConstraints {
    
    public static readonly int DefaultMaxKeys = 8;
    
    protected class Group {
        public int id;
        public List<Vector2I> coords;
        public List<int> adjacentGroups;
        
        public Group(int id) {
            this.id = id;
            this.coords = new List<Vector2I>();

            // This was a List in the java version so look here if there are performance problems.
            this.adjacentGroups = new List<int>();
        }
    }
    
    protected ColorMap colorMap;
    protected MZIntMap<Group> groups;
    protected int maxKeys;

    public FreeformConstraints(ColorMap colorMap) {
        this.colorMap = colorMap;
        this.groups = new MZIntMap<Group>();
        this.maxKeys = DefaultMaxKeys;
        
        AnalyzeMap();
    }
    
    protected void AnalyzeMap() {
        colorMap.CheckConnected();
        
        for (int x = colorMap.GetLeft(); x <= colorMap.GetRight(); ++x)
            for (int y = colorMap.GetTop(); y <= colorMap.GetBottom(); ++y) {
                int val = colorMap.Get(x,y);
                if (val == /* null? */ 0) continue;
                Group group = groups[val];
                if (group == null) {
                    group = new Group(val);
                    groups[val] = group;
                }
                group.coords.Add(new Vector2I(x,y));
            }
        // Console.WriteLine(groups.Count + " groups");
        
        foreach (Group group in groups.Values) {
            foreach (Vector2I xy in group.coords) {
                Vector2I[] directions = [ Vector2I.Up, Vector2I.Right, Vector2I.Down, Vector2I.Left ];
                foreach (Vector2I d in directions) {
                    Vector2I neighbor = xy + d;
                    if (group.coords.Contains(neighbor)) continue;
                    int val = colorMap.Get(neighbor.X, neighbor.Y);
                    if (val != /* null? */ 0 && AllowRoomsToBeAdjacent(group.id, val)) {
                        group.adjacentGroups.Add(val);
                    }
                }
            }
        }
        
        CheckConnected();
    }
    
    protected bool IsConnected() {
        // This is different from ColorMap.CheckConnected because it also checks
        // what the client says for AllowRoomsToBeAdjacent allows the map to be
        // full connected.
        // Do a breadth first search starting at the top left to check if
        // every position is reachable.
        List<int> world = new List<int>(groups.Keys);
        Queue<int> queue = new Queue<int>();

        int first = world[0];
        world.RemoveAt(0);
        queue.Enqueue(first);

        while (queue.Count > 0) {
            int pos = queue.Dequeue();
            
            foreach (KeyValuePair<Double,int> adjacent in GetAdjacentRooms(pos, GetMaxKeys()+1)) {
                int adjId = adjacent.Value;
                
                if (world.Contains(adjId)) {
                    world.Remove(adjId);
                    queue.Enqueue(adjId);
                }
            }
        }
        
        return world.Count == 0;
    }
    
    protected void CheckConnected() {
        if (!IsConnected()) {
            // Parts of the map are unreachable!
            throw new MZGenerationFailureException("ColorMap is not fully connected");
        }
    }
    
    public int GetMaxRooms() {
        return groups.Count;
    }

    public int GetMaxKeys() {
        return maxKeys;
    }
    
    public void SetMaxKeys(int maxKeys) {
        this.maxKeys = maxKeys;
    }

    public int GetMaxSwitches() {
        return 0;
    }

    public List<int> InitialRooms() {
        List<int> result = new List<int>
        {
            // TODO place the initial room elsewhere?
            groups.Values.GetEnumerator().Current.id
        };

        return result;
    }

    public List<KeyValuePair<Double, int>> GetAdjacentRooms(int id, int keyLevel) {
        List<KeyValuePair<Double, int>> options = new List<KeyValuePair<Double, int>>();
        foreach (int i in groups[id].adjacentGroups) {
            options.Add(new KeyValuePair<Double,int>(1.0, i));
        }
        return options;
    }

    /* The reason for this being separate from GetAdjacentRooms is that this
     * method is called at most once for each pair of rooms during AnalyzeMap,
     * while GetAdjacentRooms is called many times during generation under the
     * assumption that it's simply a cheap "getter". Subclasses may override
     * this method to perform more expensive checks than with GetAdjacentRooms.
     */
    protected bool AllowRoomsToBeAdjacent(int id0, int id1) {
        return true;
    }
    
    public List<Vector2I> GetCoords(int id) {
        return groups[id].coords;
    }

    public bool IsAcceptable(MZDungeon dungeon) {
        return true;
    }

    public double EdgeGraphifyProbability(int id, int nextId) {
        return 0.2;
    }

    public bool RoomCanFitItem(int id, MZSymbol key) {
        return true;
    }
}