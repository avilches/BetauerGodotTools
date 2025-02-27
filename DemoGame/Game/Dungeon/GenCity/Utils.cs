using System.Collections.Generic;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public static class Utils {

    public static float DegToRad(float degrees) {
        return degrees * (Mathf.Pi / 180);
    }

    public static List<int> TurnDirection(int direction) {
        return [(direction + 90) % 360, (direction - 90) % 360];
    }

    public static List<int> ForkDirection(int direction) {
        return [direction, ..TurnDirection(direction)];
    }

    public static Vector2I GetShift(int direction, int offset = 1) {
        return new Vector2I(
            Mathf.RoundToInt(Mathf.Cos(DegToRad(direction))) * offset,
            Mathf.RoundToInt(Mathf.Sin(DegToRad(direction))) * offset
        );
    }

    public static bool Between(int value, int min, int max) {
        return value >= min && value <= max;
    }
}