using System;
using Godot;

namespace Betauer.TileSet.Terrain;

public class NeighborRule {
    public ConditionType ConditionType { get;  }
    public int ExpectedTerrain { get; }
    public int X { get; }
    public int Y { get; }
    public bool IsTemplate => ConditionType is ConditionType.TemplateEqualsTo or ConditionType.TemplateNotEqualsTo;

    public NeighborRule(ConditionType conditionType, int expectedTerrain, int x, int y) {
        ConditionType = conditionType;
        ExpectedTerrain = expectedTerrain;
        X = x;
        Y = y;
    }

    public void Deconstruct(out ConditionType conditionType, out int expectedTerrain) {
        conditionType = ConditionType;
        expectedTerrain = ExpectedTerrain;
    }

    public bool Matches(int terrain) {
        return ConditionType switch {
            ConditionType.EqualsTo => terrain == ExpectedTerrain,
            ConditionType.NotEqualsTo => terrain != ExpectedTerrain,
            _ => throw new Exception($"Template rules cannot be checked {Enum.GetName(ConditionType)}")
        };
    }

    public bool MatchesTemplate(int terrain, int templateTerrain) {
        return ConditionType switch {
            ConditionType.EqualsTo => terrain == ExpectedTerrain,
            ConditionType.NotEqualsTo => terrain != ExpectedTerrain,
            ConditionType.TemplateEqualsTo => terrain == templateTerrain,
            ConditionType.TemplateNotEqualsTo => terrain != templateTerrain,
            _ => throw new Exception($"Template rules cannot be checked {Enum.GetName(ConditionType)}")
        };
    }

    public override bool Equals(object? obj) {
        return obj is NeighborRule rule && Equals(rule);
    }

    protected bool Equals(NeighborRule other) {
        return X == other.X && Y == other.Y && ConditionType == other.ConditionType && ExpectedTerrain == other.ExpectedTerrain;
    }

    public override int GetHashCode() {
        return HashCode.Combine(X, Y, (int)ConditionType, ExpectedTerrain);
    }
    
    public static NeighborRule? ParseRule(string neighbourRule, int x, int y) {
        switch (neighbourRule) {
            case TilePattern.Ignore: return null; // ignore
            case TilePattern.IsEmpty: return new NeighborRule(ConditionType.EqualsTo, -1, x, y); // is empty
            case TilePattern.IsNotEmpty: return new NeighborRule(ConditionType.NotEqualsTo, -1, x, y); // is not empty
            case TilePattern.TemplateNotEqualsTo: return new NeighborRule(ConditionType.TemplateNotEqualsTo, -1, x, y); // equals to something not defined yet
            case TilePattern.TemplateEqualsTo: return new NeighborRule(ConditionType.TemplateEqualsTo, -1, x, y); // not equals to something not defined yet
        }
        if (neighbourRule.StartsWith(TilePattern.NotEqualsToPrefix)) {
            var terrain = neighbourRule[1..];
            if (!terrain.IsValidInt()) {
                throw new Exception($"Rule not recognized: {neighbourRule}");
            }
            return new NeighborRule(ConditionType.NotEqualsTo, terrain.ToInt(), x, y);
        }
        if (!neighbourRule.IsValidInt()) {
            throw new Exception($"Rule not recognized: {neighbourRule}");
        }
        return new NeighborRule(ConditionType.EqualsTo, neighbourRule.ToInt(), x, y);
    }

    
}