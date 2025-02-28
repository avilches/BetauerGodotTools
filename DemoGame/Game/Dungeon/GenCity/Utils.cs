using System.Collections.Generic;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public static class Utils {

    // Returns left and right turn directions for a given direction vector
    public static IList<Vector2I> TurnDirection(Vector2I direction) {
        // If direction is horizontal (Right or Left), return vertical directions (Up and Down)
        if (direction.X != 0 && direction.Y == 0) {
            return [Vector2I.Down, Vector2I.Up];
        }
        // If direction is vertical (Up or Down), return horizontal directions (Right and Left)
        else if (direction.X == 0 && direction.Y != 0) {
            return [Vector2I.Right, Vector2I.Left];
        }
        // For diagonal directions or unexpected input, return default directions
        return [Vector2I.Right, Vector2I.Down];
    }

    // Returns current direction and perpendicular directions (fork)
    public static List<Vector2I> ForkDirection(Vector2I direction) {
        var turns = TurnDirection(direction);
        return [direction, ..turns];
    }


    public static bool IsHorizontal(Vector2I direction) {
        return direction.X != 0 && direction.Y == 0;
    }
}