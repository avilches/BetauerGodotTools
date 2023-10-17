using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Betauer.Core;
using Godot;

namespace Betauer.TileSet.Terrain;

public partial class TerrainRule {
    public enum ConditionType {
        EqualsTo,
        NotEqualsTo,
        TemplateEqualsTo,
        TemplateNotEqualsTo
    }

    public class TileRule {
        public ConditionType Type { get; set; }
        public int ExpectedTerrainId { get; set; }
        public bool IsTemplate => Type is ConditionType.TemplateEqualsTo or ConditionType.TemplateNotEqualsTo;
        public int X;
        public int Y;

        public TileRule(ConditionType type, int expectedTerrainId, int x, int y) {
            Type = type;
            ExpectedTerrainId = expectedTerrainId;
            X = x;
            Y = y;
        }
        public void Deconstruct(out ConditionType type, out int expectedTerrainId) {
            type = Type;
            expectedTerrainId = ExpectedTerrainId;
        }

        public bool CheckTile(int terrainId) {
            return Type switch {
                ConditionType.EqualsTo => terrainId == ExpectedTerrainId,
                ConditionType.NotEqualsTo => terrainId != ExpectedTerrainId,
                _ => throw new Exception($"Template rules cannot be checked {Enum.GetName(Type)}")
            };
        }

        public override bool Equals(object? obj) {
            return obj is TileRule rule && Type == rule.Type && X == rule.X && Y == rule.Y;
        }
    }
    
    public static TileRule? ParseRule(string neighbourRule, int x, int y) {
        switch (neighbourRule) {
            case "?": return null; // ignore
            case ".": return new TileRule(ConditionType.EqualsTo, -1, x, y); // is empty
            case "X": return new TileRule(ConditionType.NotEqualsTo, -1, x, y); // is not empty
            case "!": return new TileRule(ConditionType.TemplateNotEqualsTo, -1, x, y); // equals to something not defined yet
            case "#": return new TileRule(ConditionType.TemplateEqualsTo, -1, x, y); // not equals to something not defined yet
        }
        if (neighbourRule.StartsWith("!")) {
            var terrainId = neighbourRule[1..];
            if (!terrainId.IsValidInt()) {
                throw new Exception($"Rule not recognized: {neighbourRule}");
            }
            return new TileRule(ConditionType.NotEqualsTo, terrainId.ToInt(), x, y);
        }
        if (!neighbourRule.IsValidInt()) {
            throw new Exception($"Rule not recognized: {neighbourRule}");
        }
        return new TileRule(ConditionType.EqualsTo, neighbourRule.ToInt(), x, y);
    }

    private List<TileRule> Rules { get; } = new();
    public int TileId { get; }
    public int GridSize { get; }
    public bool IsTemplate { get; private set; }

    public override bool Equals(object? obj) {
        if (obj is not TerrainRule other ||
            TileId != other.TileId || 
            GridSize != other.GridSize || 
            Rules.Count != other.Rules.Count || 
            IsTemplate != other.IsTemplate) return false;
        for (var i = 0; i < Rules.Count; i++) {
            if (!Rules[i].Equals(other.Rules[i])) return false;
        }
        return true;
    }

    public TerrainRule(int tileId, int gridSize) {
        // 3 means 3x3, 5 means 5x5
        if (gridSize % 2 == 0) {
            throw new Exception($"Size {gridSize}x{gridSize} is not valid: only odd sizes are allowed: 1x1, 3x3, 5x5...");
        }
        GridSize = gridSize;
        TileId = tileId;
    }

    public static TerrainRule Parse(int tileId, string value) {
        var lines = value.Split('\n')
            .Select(v => v.Trim())
            .Where(v => v.Length > 0)
            .ToArray();
        var gridSize = lines.Length;
        if (gridSize % 2 == 0) {
            throw new Exception($"Size {gridSize}x{gridSize} is not valid: only odd sizes are allowed: 1x1, 3x3, 5x5...");
        }
        var y = 0;
        var tileMap = new TerrainRule(tileId, gridSize);
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
    
    public TileRule? FindRuleByPosition(int x, int y) {
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
                var conditionType = tileRule.Type;
                var tileRuleExpectedTerrain = tileRule.ExpectedTerrainId;
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

    public bool Check<TType>(TileMap.TileMap<TType> tileMap, int x, int y) where TType : Enum {
        var center = GridSize / 2;
        foreach (var rule in Rules) {
            var ruleX = x + rule.X - center;
            var ruleY = y + rule.Y - center;
            var tileId = ruleX < 0 || ruleY < 0 || ruleX >= tileMap.Width || ruleY >= tileMap.Height ? -1 : tileMap.GetTypeAsTerrain(ruleX, ruleY);
            if (!rule.CheckTile(tileId)) return false;
        }
        return true;
    }

    public TerrainRule ApplyTerrain(int terrainId) {
        if (!IsTemplate) return this;
        var terrainRule = new TerrainRule(TileId, GridSize);
        Rules.Select(rule => rule.Type switch {
            ConditionType.TemplateEqualsTo => new TileRule(ConditionType.EqualsTo, terrainId, rule.X, rule.Y),
            ConditionType.TemplateNotEqualsTo => new TileRule(ConditionType.NotEqualsTo, terrainId, rule.X, rule.Y),
            _ => rule
        }).ForEach(rule => terrainRule.Rules.Add(rule));
        return terrainRule;
    }
}