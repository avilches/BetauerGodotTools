using System;
using Godot;

namespace Betauer.TileSet.Terrain;

public class NeighborRulePos {
    public int X { get; }
    public int Y { get; }
    public bool EqualsTo { get; }
    public int Value { get; }

    public NeighborRulePos(NeighborRule rule, int x, int y) {
        EqualsTo = rule.EqualsTo;
        Value = rule.Value;
        X = x;
        Y = y;
    }
}

public struct NeighborRule {
    public bool EqualsTo { get; }
    public int Value { get; }

    public static readonly NeighborRule Equals0 = new NeighborRule(true, 0);
    public static readonly NeighborRule Equals1 = new NeighborRule(true, 1);
    public static readonly NeighborRule Equals2 = new NeighborRule(true, 2);
    public static readonly NeighborRule Equals3 = new NeighborRule(true, 3);
    public static readonly NeighborRule Equals4 = new NeighborRule(true, 4);
    public static readonly NeighborRule Equals5 = new NeighborRule(true, 5);
    public static readonly NeighborRule Equals6 = new NeighborRule(true, 6);
    public static readonly NeighborRule Equals7 = new NeighborRule(true, 7);
    public static readonly NeighborRule Equals8 = new NeighborRule(true, 8);
    public static readonly NeighborRule Equals9 = new NeighborRule(true, 9);
    public static readonly NeighborRule Equals10 = new NeighborRule(true, 10);
    public static readonly NeighborRule Equals11 = new NeighborRule(true, 11);
    public static readonly NeighborRule Equals12 = new NeighborRule(true, 12);
    public static readonly NeighborRule Equals13 = new NeighborRule(true, 13);
    public static readonly NeighborRule Equals14 = new NeighborRule(true, 14);
    public static readonly NeighborRule Equals15 = new NeighborRule(true, 15);
    public static readonly NeighborRule Equals16 = new NeighborRule(true, 16);
    public static readonly NeighborRule Equals17 = new NeighborRule(true, 17);
    public static readonly NeighborRule Equals18 = new NeighborRule(true, 18);
    public static readonly NeighborRule Equals19 = new NeighborRule(true, 19);
    public static readonly NeighborRule Equals20 = new NeighborRule(true, 20);

    public static readonly NeighborRule NotEquals0 = new NeighborRule(false, 0);
    public static readonly NeighborRule NotEquals1 = new NeighborRule(false, 1);
    public static readonly NeighborRule NotEquals2 = new NeighborRule(false, 2);
    public static readonly NeighborRule NotEquals3 = new NeighborRule(false, 3);
    public static readonly NeighborRule NotEquals4 = new NeighborRule(false, 4);
    public static readonly NeighborRule NotEquals5 = new NeighborRule(false, 5);
    public static readonly NeighborRule NotEquals6 = new NeighborRule(false, 6);
    public static readonly NeighborRule NotEquals7 = new NeighborRule(false, 7);
    public static readonly NeighborRule NotEquals8 = new NeighborRule(false, 8);
    public static readonly NeighborRule NotEquals9 = new NeighborRule(false, 9);
    public static readonly NeighborRule NotEquals10 = new NeighborRule(false, 10);
    public static readonly NeighborRule NotEquals11 = new NeighborRule(false, 11);
    public static readonly NeighborRule NotEquals12 = new NeighborRule(false, 12);
    public static readonly NeighborRule NotEquals13 = new NeighborRule(false, 13);
    public static readonly NeighborRule NotEquals14 = new NeighborRule(false, 14);
    public static readonly NeighborRule NotEquals15 = new NeighborRule(false, 15);
    public static readonly NeighborRule NotEquals16 = new NeighborRule(false, 16);
    public static readonly NeighborRule NotEquals17 = new NeighborRule(false, 17);
    public static readonly NeighborRule NotEquals18 = new NeighborRule(false, 18);
    public static readonly NeighborRule NotEquals19 = new NeighborRule(false, 19);
    public static readonly NeighborRule NotEquals20 = new NeighborRule(false, 20);

    private NeighborRule(bool equalsTo, int value) {
        EqualsTo = equalsTo;
        Value = value;
    }

    public static NeighborRule CreateEqualsTo(int value) {
        return new NeighborRule(true, value);
    }

    public static NeighborRule CreateNotEqualsTo(int value) {
        return new NeighborRule(false, value);
    }

    public static NeighborRule? ParseRule(string neighbourRule) {
        switch (neighbourRule) {
            case TilePattern.Ignore: return null; // ignore
        }
        if (neighbourRule.StartsWith(TilePattern.NotEqualsToPrefix)) {
            var value = neighbourRule[1..];
            if (!value.IsValidInt()) throw new Exception($"Rule not recognized: {neighbourRule}");
            return value.ToInt() switch {
                0 => NotEquals0,
                1 => NotEquals1,
                2 => NotEquals2,
                3 => NotEquals3,
                4 => NotEquals4,
                5 => NotEquals5,
                6 => NotEquals6,
                7 => NotEquals7,
                8 => NotEquals8,
                9 => NotEquals9,
                10 => NotEquals10,
                11 => NotEquals11,
                12 => NotEquals12,
                13 => NotEquals13,
                14 => NotEquals14,
                15 => NotEquals15,
                16 => NotEquals16,
                17 => NotEquals17,
                18 => NotEquals18,
                19 => NotEquals19,
                20 => NotEquals20,
                _ => new NeighborRule(true, value.ToInt())
            };
        }
        if (!neighbourRule.IsValidInt()) throw new Exception($"Rule not recognized: {neighbourRule}");
        return neighbourRule.ToInt() switch {
            0 => Equals0,
            1 => Equals1,
            2 => Equals2,
            3 => Equals3,
            4 => Equals4,
            5 => Equals5,
            6 => Equals6,
            7 => Equals7,
            8 => Equals8,
            9 => Equals9,
            10 => Equals10,
            11 => Equals11,
            12 => Equals12,
            13 => Equals13,
            14 => Equals14,
            15 => Equals15,
            16 => Equals16,
            17 => Equals17,
            18 => Equals18,
            19 => Equals19,
            20 => Equals20,
            _ => new NeighborRule(true, neighbourRule.ToInt())
        };
    }
}