using System.Collections.Generic;
using Godot;

namespace Betauer.Core.Examples.ThirdPartyCode.Metazelda.constraints;

/**
* Controls which spaces are valid for an
* {@link generators.IMZDungeonGenerator} to create
* {@link MZRoom}s in.
* <p>
* Essentially just a List<{@link Vector2I}> with some convenience methods.
*
* @see Vector2I
* @see SpaceConstraints
*/
public class MZSpaceMap {
    protected List<Vector2I> spaces = new List<Vector2I>();
    
    public int NumberSpaces() {
        return spaces.Count;
    }
    
    public bool Get(Vector2I c) {
        return spaces.Contains(c);
    }
    
    public void Set(Vector2I c, bool val) {
        if (val)
            spaces.Add(c);
        else
            spaces.Remove(c);
    }
    
    private Vector2I GetFirst() {
        return spaces[0];
    }
    
    public List<Vector2I> GetBottomSpaces() {
        List<Vector2I> bottomRow = new List<Vector2I>{GetFirst()};
        int bottomY = GetFirst().Y;
        foreach (Vector2I space in spaces) {
            if (space.Y < bottomY) {
                bottomY = space.Y;
                bottomRow = new List<Vector2I> { space };
            } else if (space.Y == bottomY) {
                bottomRow.Add(space);
            }
        }
        return bottomRow;
    }
}