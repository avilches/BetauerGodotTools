using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Godot;

namespace Betauer.TileSet.Terrain;

public partial class TilePattern {
    public enum ConditionType {
        EqualsTo,
        NotEqualsTo,
        TemplateEqualsTo,
        TemplateNotEqualsTo
    }

    public static NeighborRule? ParseRule(string neighbourRule, int x, int y) {
        switch (neighbourRule) {
            case "?": return null; // ignore
            case ".": return new NeighborRule(ConditionType.EqualsTo, -1, x, y); // is empty
            case "X": return new NeighborRule(ConditionType.NotEqualsTo, -1, x, y); // is not empty
            case "!": return new NeighborRule(ConditionType.TemplateNotEqualsTo, -1, x, y); // equals to something not defined yet
            case "#": return new NeighborRule(ConditionType.TemplateEqualsTo, -1, x, y); // not equals to something not defined yet
        }
        if (neighbourRule.StartsWith("!")) {
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

    private List<NeighborRule> Rules { get; } = new();
    public int GridSize { get; }
    public bool IsTemplate { get; private set; }

    public override bool Equals(object? obj) {
        if (obj is not TilePattern other ||
            GridSize != other.GridSize ||
            Rules.Count != other.Rules.Count ||
            IsTemplate != other.IsTemplate) return false;
        for (var i = 0; i < Rules.Count; i++) {
            if (!Rules[i].Equals(other.Rules[i])) return false;
        }
        return true;
    }

    public override int GetHashCode() {
        return HashCode.Combine(Rules, GridSize, IsTemplate);
    }

    public TilePattern(int gridSize) {
        // 3 means 3x3, 5 means 5x5
        if (gridSize % 2 == 0) {
            throw new Exception($"Size {gridSize}x{gridSize} is not valid: only odd sizes are allowed: 1x1, 3x3, 5x5...");
        }
        GridSize = gridSize;
    }

    public static TilePattern Parse(string value) {
        var lines = value.Split('\n')
            .Select(v => v.Trim())
            .Where(v => v.Length > 0)
            .ToArray();
        var gridSize = lines.Length;
        if (gridSize % 2 == 0) {
            throw new Exception($"Size {gridSize}x{gridSize} is not valid: only odd sizes are allowed: 1x1, 3x3, 5x5...");
        }
        var y = 0;
        var tileMap = new TilePattern(gridSize);
        foreach (var line in lines) {
            var parts = SplitWords().Split(line);
            if (parts.Length != gridSize) {
                throw new Exception($"Line has to be {gridSize} parts instead of {parts.Length}: {line}");
            }
            var x = 0;
            foreach (var neighbourRule in parts) {
                var tileRule = ParseRule(neighbourRule, x, y);
                if (tileRule != null) {
                    tileMap.Rules.Add(tileRule);
                    tileMap.IsTemplate = tileMap.IsTemplate || tileRule.IsTemplate;
                }
                x++;
            }
            y++;
        }
        return tileMap;
    }

    public NeighborRule? FindRuleByPosition(int x, int y) {
        foreach (var rule in Rules) {
            if (rule.X == x && rule.Y == y) return rule;
        }
        return null;
    }

    public string Export() {
        var sb = new StringBuilder(GridSize * 3 * GridSize - 1);
        for (var y = 0; y < GridSize; y++) {
            for (var x = 0; x < GridSize; x++) {
                var tileRule = FindRuleByPosition(x, y);
                if (tileRule == null) {
                    sb.Append(" ?");
                    continue;
                }
                var conditionType = tileRule.ConditionType;
                var tileRuleExpectedTerrain = tileRule.ExpectedTerrain;
                if (conditionType == ConditionType.EqualsTo) {
                    if (tileRuleExpectedTerrain == -1) {
                        sb.Append(" .");
                    } else {
                        sb.Append(' ').Append(tileRuleExpectedTerrain);
                    }
                } else if (conditionType == ConditionType.NotEqualsTo) {
                    if (tileRuleExpectedTerrain == -1) {
                        sb.Append(" X");
                    } else {
                        sb.Append('!').Append(tileRuleExpectedTerrain);
                    }
                } else if (conditionType == ConditionType.TemplateEqualsTo) {
                    sb.Append(" #");
                } else if (conditionType == ConditionType.TemplateNotEqualsTo) {
                    sb.Append(" !");
                } else {
                    throw new Exception($"Unknown neighbour rule: {tileRule}");
                }
                if (x < GridSize - 1) sb.Append(' ');
            }
            if (y < GridSize - 1) sb.Append('\n');
        }
        return sb.ToString();
    }


    [GeneratedRegex("\\s+")]
    private static partial Regex SplitWords();

    public bool Matches(TileMap.TileMap tileMap, int x, int y) {
        var center = GridSize / 2;
        foreach (var rule in Rules) {
            var ruleX = x + rule.X - center;
            var ruleY = y + rule.Y - center;
            var terrain = ruleX < 0 || ruleY < 0 || ruleX >= tileMap.Width || ruleY >= tileMap.Height ? -1 : tileMap.GetTerrain(ruleX, ruleY);
            if (!rule.Matches(terrain)) return false;
        }
        return true;
    }
    
    public bool MatchesTemplate(TileMap.TileMap tileMap, int templateTerrain, int x, int y) {
        var center = GridSize / 2;
        foreach (var rule in Rules) {
            var ruleX = x + rule.X - center;
            var ruleY = y + rule.Y - center;
            var terrainId = ruleX < 0 || ruleY < 0 || ruleX >= tileMap.Width || ruleY >= tileMap.Height ? -1 : tileMap.GetTerrain(ruleX, ruleY);
            if (!rule.MatchesTemplate(terrainId, templateTerrain)) return false;
        }
        return true;
    }
}