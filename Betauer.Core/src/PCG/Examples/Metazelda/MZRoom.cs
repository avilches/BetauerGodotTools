using System.Collections.Generic;
using Godot;

namespace Betauer.Core.PCG.Examples.Metazelda;

/**
* Represents an individual space within the dungeon.
* <p>
* A MZRoom contains:
* <ul>
* <li>an item ({@link MZSymbol}) that the player may (at his or her choice)
*      collect by passing through this MZRoom;
* <li>an intensity, which is a measure of the relative difficulty of the room
*      and ranges from 0.0 to 1.0;
* <li>{@link MZEdge}s for each door to an adjacent MZRoom.
* </ul>
*/
public class MZRoom {

    protected MZCondition precond;
    public readonly int id;
    protected List<Vector2I> coords;
    protected Vector2I center;
    protected MZSymbol? item;
    protected List<MZEdge> edges;
    protected double intensity;
    protected MZRoom parent;
    protected List<MZRoom> children;
    protected MZDungeon dungeon;
    
    /**
 * Creates a MZRoom at the given coordinates, with the given parent,
 * containing a specific item, and having a certain pre-{@link MZCondition}.
 * <p>
 * The parent of a room is the parent node of this MZRoom in the initial
 * tree of the dungeon during
 * {@link generators.MZDungeonGenerator#Generate()}, and
 * before
 * {@link generators.MZDungeonGenerator#Graphify()}.
 *
 * @param coords    the coordinates of the new room
 * @param parent    the parent room or null if it is the root / entry room
 * @param item      the symbol to place in the room or null if no item
 * @param precond   the precondition of the room
 * @see MZCondition
 */
    public MZRoom(MZDungeon dungeon, int id, List<Vector2I> coords, MZRoom parent, MZSymbol item, MZCondition precond) {
        this.dungeon = dungeon;
        this.id = id;
        this.coords = coords;
        this.item = item;
        this.edges = new List<MZEdge>();
        this.precond = precond;
        this.intensity = 0.0;
        this.parent = parent;
        this.children = new List<MZRoom>(3);
        // all edges initially null
        int x = 0, y = 0;
        foreach (Vector2I xy in coords)
        {
            x += xy.X; y += xy.Y;
        }
        center = new Vector2I(x / coords.Count, y / coords.Count);
    }

    public MZRoom(MZDungeon dungeon, int id, Vector2I coords, MZRoom parent, MZSymbol item, MZCondition precond) :
        this(dungeon, id, new List<Vector2I>{coords}, parent, item, precond) {}
    
    /**
 * @return the intensity of the MZRoom
 * @see MZRoom
 */
    public double GetIntensity() {
        return intensity;
    }
    
    /**
 * @param intensity the value to set the MZRoom's intensity to
 * @see MZRoom
 */
    public void SetIntensity(double intensity) {
        this.intensity = intensity;
    }

    /**
 * @return  the item contained in the MZRoom, or null if there is none
 */
    public MZSymbol? GetItem() {
        return item;
    }

    /**
 * @param item  the item to place in the MZRoom
 */
    public void SetItem(MZSymbol item) {
        this.item = item;
    }

    /**
 * Gets the array of {@link MZEdge} slots this MZRoom has. There is one slot
 * for each compass {@link Direction}. Non-null slots in this array
 * represent links between this MZRoom and adjacent MZRooms.
 *
 * @return the array of MZEdges
 */
    public List<MZEdge> GetEdges() {
        return edges;
    }
    
    /**
 * Gets the MZEdge object for a link in a given direction.
 *
 * @param d the compass {@link Direction} of the MZEdge for the link from this
 *          MZRoom to an adjacent MZRoom
 * @return  the {@link MZEdge} for the link in the given direction, or null if
 *          there is no link from this MZRoom in the given direction
 */
    public MZEdge? GetEdge(int targetRoomId) {
        foreach (MZEdge e in edges) {
            if (e.GetTargetRoomId() == targetRoomId)
                return e;
        }
        return null;
    }
    
    public MZEdge SetEdge(int targetRoomId, MZSymbol? symbol) {
        MZEdge? e = GetEdge(targetRoomId);
        if (e != null) {
            e.Symbol = symbol;
        } else {
            e = new MZEdge(targetRoomId, symbol);
            edges.Add(e);
        }
        return e;
    }
    
    /**
 * Gets the number of MZRooms this MZRoom is linked to.
 *
 * @return  the number of links
 */
    public int LinkCount() {
        return edges.Count;
    }
    
    /**
 * @return whether this room is the entry to the dungeon.
 */
    public bool IsStart() {
        return item != null && item.IsStart();
    }
    
    /**
 * @return whether this room is the goal room of the dungeon.
 */
    public bool IsGoal() {
        return item != null && item.IsGoal();
    }
    
    /**
 * @return whether this room contains the dungeon's boss.
 */
    public bool IsBoss() {
        return item != null && item.IsBoss();
    }
    
    /**
 * @return whether this room contains the dungeon's switch object.
 */
    public bool IsSwitch() {
        return item != null && item.IsSwitch();
    }
    
    /**
 * @return the precondition for this MZRoom
 * @see MZCondition
 */
    public MZCondition GetPrecond() {
        return precond;
    }
    
    /**
 * @param precond   the precondition to set this MZRoom's to
 * @see MZCondition
 */
    public void SetPrecond(MZCondition precond) {
        this.precond = precond;
    }

    /**
 * @return the parent of this MZRoom
 * @see MZRoom#Room
 */
    public MZRoom GetParent() {
        return parent;
    }

    /**
 * @param parent the MZRoom to set this MZRoom's parent to
 * @see MZRoom#Room
 */
    public void SetParent(MZRoom parent) {
        this.parent = parent;
    }
    
    /**
 * @return the collection of MZRooms this MZRoom is a parent of
 * @see MZRoom#Room
 */
    public List<MZRoom> GetChildren() {
        return children;
    }
    
    /**
 * Registers this MZRoom as a parent of another.
 * Does not modify the child room's parent property.
 *
 * @param child the room to parent
 */
    public void AddChild(MZRoom child) {
        children.Add(child);
    }
    
    public List<Vector2I> GetCoords() {
        return coords;
    }
    
    public Vector2I GetCenter() {
        return center;
    }
    
    public override string ToString() {
        return "Room(" + coords.ToString() + ")";
    }
    
    
    
    public MZEdge? GetEdge(Vector2I direction) {
        foreach (var edge in edges) {
            var room = dungeon.Get(edge.GetTargetRoomId());
            if (center + direction == room.GetCenter())
                return edge;
        }
        return null;
    }
    
}