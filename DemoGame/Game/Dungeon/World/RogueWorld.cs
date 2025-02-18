using System;
using System.Text;
using Betauer.Core.Examples;
using Godot;
using Veronenger.Game.Dungeon.World.Generation;

namespace Veronenger.Game.Dungeon.World;

public class RogueWorld {

    public TurnWorld TurnWorld { get; private set; }
    public SchedulingEntity Player { get; private set; }

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
            new MapTypeConfig(MapType.OfficeEasy, TemplateSetType.Office, (seed) => MazeGraphCatalog.CogmindLong(new Random(seed)))
        );
        MapGenerator.Validate();

    }

    public void Create() {
        var seed = 1;
        var result = MapGenerator.CreateMap(MapType.OfficeEasy, seed);

        TurnWorld = new TurnWorld(result.WorldCellMap) {
            TicksPerTurn = 10,
        };
        CreatePlayer();
        CreateEntities();
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


    private void CreatePlayer() {
        Player = new SchedulingEntity("Player", new EntityStats { BaseSpeed = 100 });
        Player.OnExecute += (command) => {
            Console.WriteLine($"Player executed {command.Type}");
        };
    }


    public string PrintArray2D() {
        var stringBuilder = new StringBuilder();
        for (var y = 0; y < TurnWorld.Height; y++) {
            for (var x = 0; x < TurnWorld.Width; x++) {
                var cell = TurnWorld[y, x];
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