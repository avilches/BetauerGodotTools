using System;
using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public class TilePatternRuleSet {
    public int? TemplateTerrain { get; set; }
    public List<(int, TilePattern)> Rules { get; }

    public TilePatternRuleSet(List<(int, TilePattern)> rules, int? templateTerrain = null) {
        Rules = rules;
        TemplateTerrain = templateTerrain;
    }
    
    public void Do(int[,] data, int x, int y, Action<int> action) {
        var buffer = new int[3, 3]; 
        foreach (var rule in Rules) {
            CopyCenterRectTo(data, x, y, -1, buffer);
            if (rule.Item2.Matches(buffer, TemplateTerrain)) action(rule.Item1);
        }
    }

    public bool MatchAnyRule(int[,] data, int x, int y) {
        var buffer = new int[3, 3]; 
        foreach (var rule in Rules) {
            CopyCenterRectTo(data, x, y, -1, buffer);
            if (rule.Item2.Matches(buffer, TemplateTerrain)) return true;
        }
        return false;
    }

    public int? FindRuleId(int[,] data, int x, int y) {
        var buffer = new int[3, 3]; 
        foreach (var rule in Rules) {
            CopyCenterRectTo(data, x, y, -1, buffer);
            if (rule.Item2.Matches(buffer, TemplateTerrain)) return rule.Item1;
        }
        return null;
    }
    
    public static int[,] CopyCenterRectTo(int[,] data, int centerX, int centerY, int defaultValue, int[,] destination) {
        var Width = data.GetLength(0);
        var Height = data.GetLength(1);
        var width = destination.GetLength(0);
        var height = destination.GetLength(1);
        var startX = centerX - width / 2;
        var startY = centerY - height / 2;
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                var xx = startX + x;
                var yy = startY + y;
                destination[x, y] = xx < 0 || yy < 0 || xx >= Width || yy >= Height ? defaultValue : data[xx, yy];
            }
        }
        return destination;
    }
}