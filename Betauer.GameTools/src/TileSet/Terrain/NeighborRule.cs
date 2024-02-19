using System;
using Godot;

namespace Betauer.TileSet.Terrain;

public class NeighborRulePos {
    public int X { get; }
    public int Y { get; }
    public NeighborRule NeighborRule { get; }

    public NeighborRulePos(NeighborRule rule, int x, int y) {
        NeighborRule = rule;
        X = x;
        Y = y;
    }
}

public class NeighborRule {
    public ConditionType ConditionType { get; }
    public int ExpectedTerrain { get; }
    public bool IsTemplate => ConditionType is ConditionType.TemplateEqualsTo or ConditionType.TemplateNotEqualsTo;
    public Func<int, bool> UserCondition { get; }
    public Func<int, int, bool> UserConditionPosition { get; }

    public static readonly NeighborRule TemplateEquals = new NeighborRule(ConditionType.TemplateEqualsTo, -1);
    public static readonly NeighborRule TemplateNotEquals = new NeighborRule(ConditionType.TemplateNotEqualsTo, -1);

    public static readonly NeighborRule Empty = new NeighborRule(ConditionType.EqualsTo, -1);
    public static readonly NeighborRule Equals0 = new NeighborRule(ConditionType.EqualsTo, 0);
    public static readonly NeighborRule Equals1 = new NeighborRule(ConditionType.EqualsTo, 1);
    public static readonly NeighborRule Equals2 = new NeighborRule(ConditionType.EqualsTo, 2);
    public static readonly NeighborRule Equals3 = new NeighborRule(ConditionType.EqualsTo, 3);
    public static readonly NeighborRule Equals4 = new NeighborRule(ConditionType.EqualsTo, 4);
    public static readonly NeighborRule Equals5 = new NeighborRule(ConditionType.EqualsTo, 5);
    public static readonly NeighborRule Equals6 = new NeighborRule(ConditionType.EqualsTo, 6);
    public static readonly NeighborRule Equals7 = new NeighborRule(ConditionType.EqualsTo, 7);
    public static readonly NeighborRule Equals8 = new NeighborRule(ConditionType.EqualsTo, 8);
    public static readonly NeighborRule Equals9 = new NeighborRule(ConditionType.EqualsTo, 9);
    public static readonly NeighborRule Equals10 = new NeighborRule(ConditionType.EqualsTo, 10);
    public static readonly NeighborRule Equals11 = new NeighborRule(ConditionType.EqualsTo, 11);
    public static readonly NeighborRule Equals12 = new NeighborRule(ConditionType.EqualsTo, 12);
    public static readonly NeighborRule Equals13 = new NeighborRule(ConditionType.EqualsTo, 13);
    public static readonly NeighborRule Equals14 = new NeighborRule(ConditionType.EqualsTo, 14);
    public static readonly NeighborRule Equals15 = new NeighborRule(ConditionType.EqualsTo, 15);
    public static readonly NeighborRule Equals16 = new NeighborRule(ConditionType.EqualsTo, 16);
    public static readonly NeighborRule Equals17 = new NeighborRule(ConditionType.EqualsTo, 17);
    public static readonly NeighborRule Equals18 = new NeighborRule(ConditionType.EqualsTo, 18);
    public static readonly NeighborRule Equals19 = new NeighborRule(ConditionType.EqualsTo, 19);
    public static readonly NeighborRule Equals20 = new NeighborRule(ConditionType.EqualsTo, 20);

    public static readonly NeighborRule NotEmpty = new NeighborRule(ConditionType.NotEqualsTo, -1);
    public static readonly NeighborRule NotEquals0 = new NeighborRule(ConditionType.NotEqualsTo, 0);
    public static readonly NeighborRule NotEquals1 = new NeighborRule(ConditionType.NotEqualsTo, 1);
    public static readonly NeighborRule NotEquals2 = new NeighborRule(ConditionType.NotEqualsTo, 2);
    public static readonly NeighborRule NotEquals3 = new NeighborRule(ConditionType.NotEqualsTo, 3);
    public static readonly NeighborRule NotEquals4 = new NeighborRule(ConditionType.NotEqualsTo, 4);
    public static readonly NeighborRule NotEquals5 = new NeighborRule(ConditionType.NotEqualsTo, 5);
    public static readonly NeighborRule NotEquals6 = new NeighborRule(ConditionType.NotEqualsTo, 6);
    public static readonly NeighborRule NotEquals7 = new NeighborRule(ConditionType.NotEqualsTo, 7);
    public static readonly NeighborRule NotEquals8 = new NeighborRule(ConditionType.NotEqualsTo, 8);
    public static readonly NeighborRule NotEquals9 = new NeighborRule(ConditionType.NotEqualsTo, 9);
    public static readonly NeighborRule NotEquals10 = new NeighborRule(ConditionType.NotEqualsTo, 10);
    public static readonly NeighborRule NotEquals11 = new NeighborRule(ConditionType.NotEqualsTo, 11);
    public static readonly NeighborRule NotEquals12 = new NeighborRule(ConditionType.NotEqualsTo, 12);
    public static readonly NeighborRule NotEquals13 = new NeighborRule(ConditionType.NotEqualsTo, 13);
    public static readonly NeighborRule NotEquals14 = new NeighborRule(ConditionType.NotEqualsTo, 14);
    public static readonly NeighborRule NotEquals15 = new NeighborRule(ConditionType.NotEqualsTo, 15);
    public static readonly NeighborRule NotEquals16 = new NeighborRule(ConditionType.NotEqualsTo, 16);
    public static readonly NeighborRule NotEquals17 = new NeighborRule(ConditionType.NotEqualsTo, 17);
    public static readonly NeighborRule NotEquals18 = new NeighborRule(ConditionType.NotEqualsTo, 18);
    public static readonly NeighborRule NotEquals19 = new NeighborRule(ConditionType.NotEqualsTo, 19);
    public static readonly NeighborRule NotEquals20 = new NeighborRule(ConditionType.NotEqualsTo, 20);


    private NeighborRule(ConditionType conditionType, int expectedTerrain) {
        ConditionType = conditionType;
        ExpectedTerrain = expectedTerrain;
    }

    private NeighborRule(Func<int, bool> userCondition) {
        ConditionType = ConditionType.UserDefined;
        UserCondition = userCondition;
    }

    private NeighborRule(Func<int, int, bool> userCondition) {
        ConditionType = ConditionType.UserDefinedPosition;
        UserConditionPosition = userCondition;
    }

    public static NeighborRule CreateEqualsTo(int terrain) {
        return new NeighborRule(ConditionType.EqualsTo, terrain);
    }

    public static NeighborRule Create(Func<int, bool> extraRule) {
        return new NeighborRule(extraRule);
    }

    public static NeighborRule CreateByPosition(Func<int, int, bool> extraRule) {
        return new NeighborRule(extraRule);
    }

    public static NeighborRule CreateNotEqualsTo(int terrain) {
        return new NeighborRule(ConditionType.NotEqualsTo, terrain);
    }

    public static NeighborRule? ParseRule(string neighbourRule) {
        switch (neighbourRule) {
            case TilePattern.Ignore: return null; // ignore
            case TilePattern.IsEmpty: return Empty; // is empty (terrain = -1)
            case TilePattern.IsNotEmpty: return NotEmpty; // is not empty (terrain != -1
            case TilePattern.TemplateNotEqualsTo: return TemplateNotEquals; // equals to something not defined yet
            case TilePattern.TemplateEqualsTo: return TemplateEquals; // not equals to something not defined yet
        }
        if (neighbourRule.StartsWith(TilePattern.NotEqualsToPrefix)) {
            var terrain = neighbourRule[1..];
            if (!terrain.IsValidInt()) throw new Exception($"Rule not recognized: {neighbourRule}");
            return terrain.ToInt() switch {
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
                _ => new NeighborRule(ConditionType.NotEqualsTo, terrain.ToInt())
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
            _ => new NeighborRule(ConditionType.EqualsTo, neighbourRule.ToInt())
        };
    }
}