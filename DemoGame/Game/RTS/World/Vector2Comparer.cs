using System.Collections.Generic;
using Godot;

namespace Veronenger.Game.RTS.World;

public class Vector2Comparer : IComparer<Vector2> {
    public static readonly Vector2Comparer Instance = new Vector2Comparer();

    public static void Sort(List<Vector2> list) => list.Sort(Instance);

    public int Compare(Vector2 v1, Vector2 v2){
        return v1.X.CompareTo(v2.X);
    }
}