namespace Veronenger.Game.Dungeon.World;

using System;
using System.Collections.Generic;

public enum CellType : byte { // 1 byte = 256 cell types; short = 2 bytes = 65536 values
    Wall,
    Floor,
    Door,
}

public class CellTypeConfig {

    private static readonly Dictionary<CellType, CellTypeConfig> Cells = new();

    public CellType Type { get; }

    public CellTypeConfig(CellType type) {
        Type = type;
        if (Cells.ContainsKey(type)) {
            throw new Exception($"Cell type {type} already registered!");
        }
        Cells[type] = this;
    }

    public static void Remove(CellType type) {
        Cells.Remove(type);
    }

    public static void RemoveAll() {
        Cells.Clear();
    }

    public static void Verify() {
        foreach (var type in Enum.GetValues<CellType>()) {
            if (!Cells.ContainsKey(type)) {
                throw new Exception($"Cell type {type} not registered!");
            }
        }
    }

    public static CellTypeConfig Get(CellType type) {
        return Cells.TryGetValue(type, out var cell)
            ? cell
            : throw new Exception($"Cell type {type} not registered!");
    }
}