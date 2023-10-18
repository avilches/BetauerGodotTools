using System;

namespace Betauer.TileSet.Terrain;

public class NeighborRule {
    public TilePattern.ConditionType ConditionType { get; set; }
    public int ExpectedTerrain { get; set; }
    public bool IsTemplate => ConditionType is TilePattern.ConditionType.TemplateEqualsTo or TilePattern.ConditionType.TemplateNotEqualsTo;
    public int X;
    public int Y;

    public NeighborRule(TilePattern.ConditionType conditionType, int expectedTerrain, int x, int y) {
        ConditionType = conditionType;
        ExpectedTerrain = expectedTerrain;
        X = x;
        Y = y;
    }

    public void Deconstruct(out TilePattern.ConditionType conditionType, out int expectedTerrain) {
        conditionType = ConditionType;
        expectedTerrain = ExpectedTerrain;
    }

    public bool Matches(int terrain) {
        return ConditionType switch {
            TilePattern.ConditionType.EqualsTo => terrain == ExpectedTerrain,
            TilePattern.ConditionType.NotEqualsTo => terrain != ExpectedTerrain,
            _ => throw new Exception($"Template rules cannot be checked {Enum.GetName(ConditionType)}")
        };
    }

    public bool MatchesTemplate(int terrain, int templateTerrain) {
        return ConditionType switch {
            TilePattern.ConditionType.EqualsTo => terrain == ExpectedTerrain,
            TilePattern.ConditionType.NotEqualsTo => terrain != ExpectedTerrain,
            TilePattern.ConditionType.TemplateEqualsTo => terrain == templateTerrain,
            TilePattern.ConditionType.TemplateNotEqualsTo => terrain != templateTerrain,
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
}