using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Betauer.Core;
using Godot;

namespace Veronenger.Game.Dungeon.World.Generation;

public enum CellDefinitionType : byte {
    Empty,
    Wall,

    Floor,
    Door,
    Loot,
    Key
}

/// <summary>
/// Cell definition comes from the template. It tries to explain the behaviour expected from the cell.
/// For instance, the definition could be "loot", but the cell will be a Floor with, or without a loot.
/// </summary>
/// <param name="Type"></param>
public record CellDefinitionConfig(CellDefinitionType Type) : EnumConfig<CellDefinitionType, CellDefinitionConfig>(Type) {
    // Blocking means the cell is not traversable by the player or enemies.
    public required bool Blocking { get; init; }


    public required char TemplateCharacter { get; init; }
    public required Func<Vector2I, WorldCell?> Factory { get; init; }

    public static HashSet<char> AllChars { get; private set; }
    public static HashSet<char> BlockingChars { get; private set; }

    public static void InitializeDefaults() {
        RegisterAll(
            new CellDefinitionConfig(CellDefinitionType.Empty) { Blocking = true, TemplateCharacter = '.', Factory = (_) => null },
            new CellDefinitionConfig(CellDefinitionType.Wall) { Blocking = true, TemplateCharacter = '#', Factory = (pos) => new WorldCell(CellType.Wall, pos) },
            new CellDefinitionConfig(CellDefinitionType.Floor) { Blocking = false, TemplateCharacter = '·', Factory = (pos) => new WorldCell(CellType.Floor, pos) },
            new CellDefinitionConfig(CellDefinitionType.Door) { Blocking = false, TemplateCharacter = 'd', Factory = (pos) => new WorldCell(CellType.Door, pos) },
            new CellDefinitionConfig(CellDefinitionType.Loot) { Blocking = false, TemplateCharacter = '$', Factory = (pos) => new WorldCell(CellType.Floor, pos) },
            new CellDefinitionConfig(CellDefinitionType.Key) { Blocking = false, TemplateCharacter = 'k', Factory = (pos) => new WorldCell(CellType.Floor, pos) }
        );

        // Avoid duplicated characters
        HashSet<char> used = [];
        foreach (var config in All) {
            if (!used.Add(config.TemplateCharacter)) {
                throw new InvalidDataException($"Character {config.TemplateCharacter} is duplicated in the CellDefinitionConfig");
            }
        }

        AllChars = All.Select(c => c.TemplateCharacter).ToHashSet();
        BlockingChars = All.Where(c => c.Blocking).Select(c => c.TemplateCharacter).ToHashSet();
    }

    public static CellDefinitionType Find(char cell) {
        return All.First(config => config.TemplateCharacter == cell).Type;
    }

    public static bool IsValid(char c) => AllChars.Contains(c);
    public static bool IsBlockingChar(char c) => BlockingChars.Contains(c);

    public static WorldCell? CreateCell(char c, Vector2I pos) {
        var cellDef = All.First(config => config.TemplateCharacter == c);
        var worldCell = cellDef.Factory(pos);
        if (worldCell != null) {
            worldCell.CellDefinitionConfig = cellDef;
        }
        return worldCell;
    }
}