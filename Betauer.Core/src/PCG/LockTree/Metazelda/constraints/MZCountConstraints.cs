using System;
using System.Collections.Generic;
using Betauer.Core.PCG.LockTree.Metazelda.util;
using Godot;

namespace Betauer.Core.PCG.LockTree.Metazelda.constraints;

/**
* Limits the {@link generators.IMZDungeonGenerator} in
* the <i>number</i> of keys, switches and rooms it is allowed to place.
*
* Also restrict to a grid of 1x1 rooms.
*
* @see IMZDungeonConstraints
*/
public class CountConstraints : IMZDungeonConstraints {

    protected int maxSpaces, maxKeys, maxSwitches;
    
    protected MZIntMap<Vector2I> gridCoords;
    protected Dictionary<Vector2I, int> roomIds;
    protected int firstRoomId;
    
    public CountConstraints(int maxSpaces, int maxKeys, int maxSwitches) {
        this.maxSpaces = maxSpaces;
        this.maxKeys = maxKeys;
        this.maxSwitches = maxSwitches;

        gridCoords = new MZIntMap<Vector2I>();
        roomIds = new Dictionary<Vector2I, int>();
        Vector2I first = new Vector2I(0,0);
        firstRoomId = GetRoomId(first);
    }
    
    public int GetRoomId(Vector2I xy) {
        if (roomIds.ContainsKey(xy)) {
            return roomIds[xy];
        } else {
            int id = gridCoords.NewInt();
            gridCoords[id] = xy;
            roomIds[xy] = id;
            return id;
        }
    }
    
    public Vector2I GetRoomCoords(int id) {
        return gridCoords[id];
    }
    
    public int GetMaxRooms() {
        return maxSpaces;
    }
    
    public void SetMaxSpaces(int maxSpaces) {
        this.maxSpaces = maxSpaces;
    }
    
    public virtual List<int> InitialRooms() {
        return new List<int> { firstRoomId };
    }

    public int GetMaxKeys() {
        return maxKeys;
    }
    
    public void SetMaxKeys(int maxKeys) {
        this.maxKeys = maxKeys;
    }
    
    public bool IsAcceptable(MZDungeon dungeon) {
        return true;
    }

    public int GetMaxSwitches() {
        return maxSwitches;
    }

    public void SetMaxSwitches(int maxSwitches) {
        this.maxSwitches = maxSwitches;
    }

    protected virtual bool ValidRoomCoords(Vector2I c) {
        return c.Y >= 0;
    }
    
    public List<KeyValuePair<Double, int>> GetAdjacentRooms(int id, int keyLevel) {
        Vector2I xy = gridCoords[id];
        List<KeyValuePair<Double, int>> ids = new List<KeyValuePair<Double, int>>();
        Vector2I[] directions = [ Vector2I.Up, Vector2I.Right, Vector2I.Down, Vector2I.Left ];

        foreach (Vector2I d in directions) {
            Vector2I neighbor = xy + d;
            if (ValidRoomCoords(neighbor))
                ids.Add(new KeyValuePair<Double,int>(1.0,GetRoomId(neighbor)));
        }
        return ids;
    }

    public List<Vector2I> GetCoords(int id) {
        return new List<Vector2I> { GetRoomCoords(id) };
    }

    public double EdgeGraphifyProbability(int id, int nextId) {
        return 0.2;
    }

    public bool RoomCanFitItem(int id, MZSymbol key) {
        return true;
    }

}