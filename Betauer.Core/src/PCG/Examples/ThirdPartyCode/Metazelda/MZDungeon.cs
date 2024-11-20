using System;
using System.Collections.Generic;
using Betauer.Core.PCG.Examples.ThirdPartyCode.Metazelda.util;
using Godot;

namespace Betauer.Core.PCG.Examples.ThirdPartyCode.Metazelda;

/**
* @see IMZDungeon
*
* Due to the fact it uses MZIntMap to store the rooms, it makes the assumption
* that room ids are low in value, tight in range, and all positive.
*/
public class MZDungeon {

    protected int itemCount;
    protected MZIntMap<MZRoom> rooms;
    protected MZRect bounds;
    
    public MZDungeon() {
        rooms = new MZIntMap<MZRoom>();
        bounds = new MZRect(Int32.MaxValue,Int32.MaxValue,
            Int32.MinValue,Int32.MinValue);
    }
    
    public MZRect GetExtentBounds() {
        return bounds;
    }
    
    public Dictionary<int, MZRoom>.ValueCollection GetRooms() {
        return rooms.Values;
    }
    
    public int RoomCount() {
        return rooms.Count;
    }

    public MZRoom? Get(int id) {
        return rooms.GetValueOrDefault(id);
    }

    public MZRoom? Find(Vector2I pos) {
        foreach (MZRoom room in GetRooms()) {
            if (room.GetCenter() == pos) return room;
        }
        return null;
    }

    public void Add(MZRoom room) {
        rooms[room.id] = room;
        
        foreach (Vector2I xy in room.GetCoords()) {
            if (xy.X < bounds.Left) {
                bounds = new MZRect(xy.X, bounds.Top,
                    bounds.Right, bounds.Bottom);
            }
            if (xy.X >= bounds.Right) {
                bounds = new MZRect(bounds.Left, bounds.Top,
                    xy.X+1, bounds.Bottom);
            }
            if (xy.Y < bounds.Top) {
                bounds = new MZRect(bounds.Left, xy.Y,
                    bounds.Right, bounds.Bottom);
            }
            if (xy.Y >= bounds.Bottom) {
                bounds = new MZRect(bounds.Left, bounds.Top,
                    bounds.Right, xy.Y+1);
            }
        }
    }

    public void LinkOneWay(MZRoom room1, MZRoom room2, MZSymbol? cond = null) {
        room1.SetEdge(room2.id, cond);
    }

    public void Link(MZRoom room1, MZRoom room2, MZSymbol? cond = null) {
        LinkOneWay(room1, room2, cond);
        LinkOneWay(room2, room1, cond);
    }

    public bool RoomsAreLinked(MZRoom room1, MZRoom room2) {
        return room1.GetEdge(room2.id) != null ||
               room2.GetEdge(room1.id) != null;
    }

    public MZRoom? FindStart() {
        foreach (MZRoom room in GetRooms()) {
            if (room.IsStart()) return room;
        }
        return null;
    }

    public MZRoom? FindBoss() {
        foreach (MZRoom room in GetRooms()) {
            if (room.IsBoss()) return room;
        }
        return null;
    }

    public MZRoom? FindGoal() {
        foreach (MZRoom room in GetRooms()) {
            if (room.IsGoal()) return room;
        }
        return null;
    }

    public MZRoom? FindSwitch() {
        foreach (MZRoom room in GetRooms()) {
            if (room.IsSwitch()) return room;
        }
        return null;
    }
}