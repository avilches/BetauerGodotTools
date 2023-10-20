using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Betauer.TileSet.TileMap.Handlers;

namespace Betauer.TileSet.Terrain;

public enum ConditionType {
    // Values are sorted by priority. Higher value will be evaluated first.
    // The theory behind is, in a random distribution or tiles in the map, "equals" comparision
    // has a higher chance to be false than "not equals". So, the sooner it fails, the better because it can
    // be discarded.
    EqualsTo = 5,
    TemplateEqualsTo = 4,
    NotEqualsTo = 3,
    TemplateNotEqualsTo = 2
}

public partial class TilePattern {

    public const string NotEqualsToPrefix = "!"; 

    public const string Ignore = "?"; 
    public const string IsEmpty = "."; 
    public const string IsNotEmpty = "X"; 
    public const string TemplateNotEqualsTo = "!"; 
    public const string TemplateEqualsTo = "#"; 

    private readonly NeighborRule[] _rules;
    public int GridSize { get; }

    public bool IsTemplate => _rules.Any(rule => rule.IsTemplate);

    public override bool Equals(object? obj) {
        if (obj is not TilePattern other ||
            GridSize != other.GridSize ||
            _rules.Length != other._rules.Length ||
            IsTemplate != other.IsTemplate) return false;
        for (var i = 0; i < _rules.Length; i++) {
            if (!_rules[i].Equals(other._rules[i])) return false;
        }
        return true;
    }

    public override int GetHashCode() {
        return HashCode.Combine(_rules, GridSize, IsTemplate);
    }

    public TilePattern(int gridSize, NeighborRule[] rules) {
        // 3 means 3x3, 5 means 5x5
        if (gridSize % 2 == 0) {
            throw new Exception($"Size {gridSize}x{gridSize} is not valid: only odd sizes are allowed: 1x1, 3x3, 5x5...");
        }
        GridSize = gridSize;
        _rules = rules;
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
        var rules = new List<NeighborRule>();
        foreach (var line in lines) {
            var parts = SplitWords().Split(line);
            if (parts.Length != gridSize) {
                throw new Exception($"Line has to be {gridSize} parts instead of {parts.Length}: {line}");
            }
            var x = 0;
            foreach (var neighbourRule in parts) {
                var tileRule = NeighborRule.ParseRule(neighbourRule, x, y);
                if (tileRule != null) {
                    rules.Add(tileRule);
                }
                x++;
            }
            y++;
        }
        rules.Sort((a, b) => ((int)b.ConditionType).CompareTo((int)a.ConditionType));
        var tileMap = new TilePattern(gridSize, rules.ToArray());
        return tileMap;
    }

    
    /// <summary>
    /// Creates a new TilePattern without templates, but with the same rules.
    /// If the TilePattern is not a template, it returns the same TilePattern.
    /// </summary>
    /// <param name="terrainId"></param>
    /// <returns></returns>
    public TilePattern WithTerrain(int terrainId) {
        if (!IsTemplate) return this;
        var rules = _rules.Select(rule => rule.ConditionType switch {
            ConditionType.TemplateEqualsTo => new NeighborRule(ConditionType.EqualsTo, terrainId, rule.X, rule.Y),
            ConditionType.TemplateNotEqualsTo => new NeighborRule(ConditionType.NotEqualsTo, terrainId, rule.X, rule.Y),
            _ => rule
        }).ToArray();
        return new TilePattern(GridSize, rules);
    }

    public NeighborRule? FindRuleByPosition(int x, int y) {
        foreach (var rule in _rules) {
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
                    sb.Append(' ').Append(Ignore);
                    continue;
                }
                var conditionType = tileRule.ConditionType;
                var tileRuleExpectedTerrain = tileRule.ExpectedTerrain;
                if (conditionType == ConditionType.EqualsTo) {
                    if (tileRuleExpectedTerrain == -1) {
                        sb.Append(' ').Append(IsEmpty);
                    } else {
                        sb.Append(' ').Append(tileRuleExpectedTerrain);
                    }
                } else if (conditionType == ConditionType.NotEqualsTo) {
                    if (tileRuleExpectedTerrain == -1) {
                        sb.Append(' ').Append(IsNotEmpty);
                    } else {
                        sb.Append(NotEqualsToPrefix).Append(tileRuleExpectedTerrain);
                    }
                } else if (conditionType == ConditionType.TemplateEqualsTo) {
                    sb.Append(' ').Append(TemplateEqualsTo);
                } else if (conditionType == ConditionType.TemplateNotEqualsTo) {
                    sb.Append(' ').Append(TemplateNotEqualsTo);
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
        var span = _rules.AsSpan();
        for (var idx = 0; idx < span.Length; idx++) {
            var rule = span[idx];
            var ruleX = x + rule.X - center;
            var ruleY = y + rule.Y - center;
            var terrain = ruleX < 0 || ruleY < 0 || ruleX >= tileMap.Width || ruleY >= tileMap.Height ? -1 : tileMap.GetTerrain(ruleX, ruleY);
            if (!rule.Matches(terrain)) return false;
        }
        return true;
    }
    
    public bool MatchesTemplate(TileMap.TileMap tileMap, int templateTerrain, int x, int y) {
        var center = GridSize / 2;
        var span = _rules.AsSpan();
        for (var idx = 0; idx < span.Length; idx++) {
            var rule = span[idx];
            var ruleX = x + rule.X - center;
            var ruleY = y + rule.Y - center;
            var terrain = ruleX < 0 || ruleY < 0 || ruleX >= tileMap.Width || ruleY >= tileMap.Height ? -1 : tileMap.GetTerrain(ruleX, ruleY);
            if (!rule.MatchesTemplate(terrain, templateTerrain)) return false;
        }
        return true;
    }
}