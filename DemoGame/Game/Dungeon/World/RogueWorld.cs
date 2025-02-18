using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Betauer.Core.Examples;
using Veronenger.Game.Dungeon.World.Generation;

namespace Veronenger.Game.Dungeon.World;

public class RogueWorld {
    public WorldMap WorldMap { get; private set; }

    public static void Configure(string templateContent) {
        ActionTypeConfig.RegisterAll(
            new ActionTypeConfig(ActionType.EndGame) { EnergyCost = 0 },
            new ActionTypeConfig(ActionType.Wait) { EnergyCost = 1000 },
            new ActionTypeConfig(ActionType.Walk) { EnergyCost = 1000 },
            new ActionTypeConfig(ActionType.Attack) { EnergyCost = 1000 },
            new ActionTypeConfig(ActionType.Run) { EnergyCost = 500 }
        );

        CellTypeConfig.DefaultConfig();

        CellDefinitionConfig.InitializeDefaults();

        TemplateSetTypeConfig.RegisterAll(
            new TemplateSetTypeConfig(TemplateSetType.Office).Create(9, templateContent)
        );

        MapTypeConfig.RegisterAll(
            new MapTypeConfig(MapType.OfficeEasy, TemplateSetType.Office, (seed) => MazeGraphCatalog.Mini(new Random(seed)))
        );
        MapGenerator.Validate();
    }

    public MapGenerator.MapGenerationResult GenerateWorldMap(int seed) {
        var result = MapGenerator.GenerateMap(MapType.OfficeEasy, seed);

        WorldMap = new WorldMap(result.WorldCellMap) {
            TicksPerTurn = 10,
        };
        CreateEntities();
        return result;
    }

    private void CreateEntities() {
        /*
        var goblin = EntityBuilder.Create("Goblin", new EntityStats { BaseSpeed = 80 })
            .Build();
            */

        /*
    goblin.OnDecideAction = async () => {
        var distance = GameWorld.GetDistance(player, goblin);
        if (distance > 1) {
            return new EntityAction(ActionType.Walk, player);
        }
        return new EntityAction(ActionType.Attack, player);
    };
    */

        /*
        var quickRat = EntityBuilder.Create("Goblin", new EntityStats { BaseSpeed = 80 })
            .DecideAction(ActionType.Walk)
            .Build();

        TurnWorld.AddEntity(goblin, Vector2I.Zero);
        TurnWorld.AddEntity(quickRat, Vector2I.Zero);
    */
    }

    public string PrintArray2D() {
        var stringBuilder = new StringBuilder();
        for (var y = 0; y < WorldMap.Height; y++) {
            for (var x = 0; x < WorldMap.Width; x++) {
                var cell = WorldMap[y, x];
                // Console.WriteLine($"Glyph: {y},{x} : {cell?.Type}");
                stringBuilder.Append(Glyph(cell));
            }
            stringBuilder.Append('\n');
        }
        return stringBuilder.ToString();
    }

    private static char Glyph(WorldCell? cell) {
        if (cell == null) return ' ';
        if (cell.Type == CellType.Wall) return '#';
        if (cell.Type == CellType.Floor) {
            return cell.Entities.Count switch {
                0 => '·',
                1 => '1',
                2 => '2',
                3 => '3',
                4 => '4',
                5 => '5',
                6 => '6',
                7 => '7',
                8 => '8',
                9 => '9',
                _ => 'X',
            };
        }
        return '-';
    }
}