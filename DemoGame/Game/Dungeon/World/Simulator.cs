using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Betauer.Core;
using Betauer.Core.DataMath;
using Betauer.Core.Examples;
using Betauer.Core.PCG.GridTemplate;
using Betauer.Core.PCG.Maze;
using Godot;
using Veronenger.Game.Dungeon.World.Generation;

namespace Veronenger.Game.Dungeon.World;

public class Simulator {
    const string TemplatePath = "/Users/avilches/Library/Mobile Documents/com~apple~CloudDocs/Shared/Godot/Betauer4/DemoGame/Game/Dungeon/MazeTemplateDemos.txt";

    public static void Main() {
        TaskScheduler.UnobservedTaskException += (sender, e) => {
            Console.WriteLine($"Unobserved Task Exception: {e.Exception}");
            e.SetObserved(); // Marca la excepción como observada
        };

        new Simulator().RunGameLoop(100);
    }

    private bool _running = true;

    private DungeonWorld dungeonWorld;

    public void RunGameLoop(int turns) {
        dungeonWorld = new DungeonWorld();
        var templateContent = LoadTemplateContent(TemplatePath);
        dungeonWorld.Configure(templateContent);
        dungeonWorld.CreateMap();

        Task.Run(() => HandlePlayerInput(new EntityBlocking(dungeonWorld.Player)));

        var ticks = turns * dungeonWorld.TurnWorld.TicksPerTurn;
        while (_running && dungeonWorld.TurnWorld.CurrentTick < ticks) {
            dungeonWorld.TurnSystemProcess._Process();
        }
        _running = false;
    }

    public static string LoadTemplateContent(string templatePath) {
        try {
            return File.ReadAllText(templatePath);
        } catch (FileNotFoundException e) {
            Console.WriteLine(e.Message);
            Console.WriteLine($"{nameof(templatePath)} is '{templatePath}'");
            Console.WriteLine("Ensure the working directory is the root of the project");
            throw;
        }
    }

    private void HandlePlayerInput(EntityBlocking player) {
        while (_running) {
            if (player.IsWaiting) {
                try {
                    var action = HandleMenuInput(player.Entity);
                    player.SetResult(action);
                } catch (Exception e) {
                    Console.WriteLine(e);
                    player.SetResult(null);
                }
                Thread.Sleep(100); // Prevent tight loop
            }
        }
    }

    private ActionCommand HandleMenuInput(Entity player) {
        while (true) {
            // Console.Clear();
            Console.WriteLine(dungeonWorld.PrintArray2D());
            // Console.WriteLine("\nSeleccione una acción:");
            Console.Write("1) Walk 2) Attacks1: ");
            var keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key) {
                case ConsoleKey.UpArrow:
                    Console.WriteLine("Flecha arriba presionada.");
                    break;
                case ConsoleKey.DownArrow:
                    Console.WriteLine("Flecha abajo presionada.");
                    break;
                case ConsoleKey.LeftArrow:
                    Console.WriteLine("Flecha izquierda presionada.");
                    break;
                case ConsoleKey.RightArrow:
                    Console.WriteLine("Flecha derecha presionada.");
                    break;
                case ConsoleKey.Spacebar:
                    Console.WriteLine("Espacio presionado.");
                    break;
                case ConsoleKey.Enter:
                    Console.WriteLine("Enter presionado.");
                    break;
                default:
                    Console.WriteLine($"Tecla presionada: {keyInfo.KeyChar}");
                    break;
            }
        }
    }
}

public class DungeonWorld {
    public TurnWorld TurnWorld;
    public TurnSystemProcess TurnSystemProcess;

    public void Configure(string templateContent) {
        ActionTypeConfig.RegisterAll(
            new ActionTypeConfig(ActionType.Wait) { EnergyCost = 1000 },
            new ActionTypeConfig(ActionType.Walk) { EnergyCost = 1000 },
            new ActionTypeConfig(ActionType.Attack) { EnergyCost = 1000 },
            new ActionTypeConfig(ActionType.Run) { EnergyCost = 500 }
        );

        CellTypeConfig.RegisterAll(
            new CellTypeConfig(CellType.Floor),
            new CellTypeConfig(CellType.Wall),
            new CellTypeConfig(CellType.Door)
        );

        CellDefinitionConfig.InitializeDefaults();

        TemplateSetTypeConfig.RegisterAll(
            new TemplateSetTypeConfig(TemplateSetType.Office).Create(9, templateContent)
        );

        MapTypeConfig.RegisterAll(
            new MapTypeConfig(MapType.OfficeEasy, TemplateSetType.Office, (seed) => MazeGraphCatalog.CogmindLong(new Random(seed)))
        );
        MapGenerator.Validate();

    }

    public void CreateMap() {
        var seed = 1;
        var result = MapGenerator.CreateMap(MapType.OfficeEasy, seed);

        TurnWorld = new TurnWorld(result.WorldCellMap) {
            TicksPerTurn = 10,
            DefaultCellType = CellType.Floor
        };
        TurnSystemProcess = TurnWorld.CreateTurnSystem().CreateTurnSystemProcess();
        CreatePlayer();
        CreateEntities();
    }

    public Entity Player { get; private set; }

    private void CreateEntities() {
        var goblin = EntityBuilder.Create("Goblin", new EntityStats { BaseSpeed = 80 })
            .Build();

        /*
    goblin.OnDecideAction = async () => {
        var distance = GameWorld.GetDistance(player, goblin);
        if (distance > 1) {
            return new EntityAction(ActionType.Walk, player);
        }
        return new EntityAction(ActionType.Attack, player);
    };
    */

        var quickRat = EntityBuilder.Create("Goblin", new EntityStats { BaseSpeed = 80 })
            .DecideAction(ActionType.Walk)
            .Build();

        TurnWorld.AddEntity(goblin);
        TurnWorld.AddEntity(quickRat);
    }


    private void CreatePlayer() {
        var player = new Entity("Player", new EntityStats { BaseSpeed = 100 });
        TurnWorld.AddEntity(player);
        Player = player;
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