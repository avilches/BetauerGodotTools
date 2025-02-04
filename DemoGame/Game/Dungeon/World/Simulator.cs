using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Betauer.Core.DataMath;
using Betauer.Core.Examples;
using Betauer.Core.PCG.GridTemplate;
using Betauer.Core.PCG.Maze;

namespace Veronenger.Game.Dungeon.World;

public class Simulator {
    private bool _running = true;
    const string TemplatePath = "/Users/avilches/Library/Mobile Documents/com~apple~CloudDocs/Shared/Godot/Betauer4/DemoGame/Game/Dungeon/MazeTemplateDemos.txt";

    public static void Main() {
        TaskScheduler.UnobservedTaskException += (sender, e) => {
            Console.WriteLine($"Unobserved Task Exception: {e.Exception}");
            e.SetObserved(); // Marca la excepción como observada
        };

        TurnWorld.TicksPerTurn = 10;

        _ = new ActionConfig(ActionType.Wait) { EnergyCost = 1000 };
        _ = new ActionConfig(ActionType.Walk) { EnergyCost = 1000 };
        _ = new ActionConfig(ActionType.Attack) { EnergyCost = 1000 };
        _ = new ActionConfig(ActionType.Run) { EnergyCost = 500 };
        ActionConfig.Verify();

        _ = new CellConfig(CellType.Floor);
        _ = new CellConfig(CellType.Wall);
        CellConfig.Verify();

        var seed = 1;
        var game = new Simulator();
        game.CreateMap(seed);
        game.CreatePlayer();
        game.CreateEntities();
        game.RunGameLoop(100);
    }

    public TurnWorld World;
    public GameWorld GameWorld;

    private void CreateMap(int seed) {
        var rng = new Random(seed);
        var zones = MazeGraphCatalog.BigCycle(rng, mc => {
            // mc.OnNodeCreated += (node) => PrintGraph(mc);
        });
        zones.CalculateSolution(MazeGraphCatalog.KeyFormula);

        World = new TurnWorld(100, 100);
        GameWorld = new GameWorld();

        // AddFlags(zones);

        var templateSet = new TemplateSet(cellSize: 7);

        // Cargar patrones de diferentes archivos
        try {
            var content = File.ReadAllText(TemplatePath);
            templateSet.LoadTemplates(content);

            var array2D = zones.MazeGraph.Render(TemplateSelector.Create(templateSet));

            /*
            var array2D = zones.MazeGraph.Render(node => {
                var type = TemplateSelector.GetNodeType(node);
                List<object> requiredFlags = [];
                if (node.IsCorridor()) {
                    node.SetAttribute();
                    // Corridor
                    return templateSet.FindTemplates(type, new[] { "deadend" })[0];
                }


            });
            */
            MazeGraphZonedDemo.PrintGraph(zones.MazeGraph, zones);
        } catch (FileNotFoundException e) {
            Console.WriteLine(e.Message);
            Console.WriteLine($"{nameof(TemplatePath)} is '{TemplatePath}'");
            Console.WriteLine("Ensure the working directory is the root of the project");
        }
    }

    private void CreateEntities() {
        var goblin = EntityBuilder.Create("Goblin", new EntityStats { BaseSpeed = 80 })
            .Build();

        goblin.OnDecideAction = async () => {
            var player = GameWorld.CurrentPlayer;
            var distance = GameWorld.GetDistance(player, goblin);
            if (distance > 1) {
                return new EntityAction(ActionType.Walk, player);
            }
            return new EntityAction(ActionType.Attack, player);
        };

        var quickRat = EntityBuilder.Create("Goblin", new EntityStats { BaseSpeed = 80 })
            .DecideAction(ActionType.Walk)
            .Build();

        World.AddEntity(goblin);
        World.AddEntity(quickRat);
    }


    private void CreatePlayer() {
        var player = new Entity("Player", new EntityStats { BaseSpeed = 100 });
        World.AddEntity(player);
        GameWorld.CurrentPlayer = player;
        Task.Run(() => HandlePlayerInput(new EntityBlocking(player)));
    }

    public void RunGameLoop(int turns) {
        var ticks = turns * TurnWorld.TicksPerTurn;
        var turnSystemProcess = new TurnSystemProcess(new TurnSystem(World));
        while (_running && World.CurrentTick < ticks) {
            turnSystemProcess._Process();
        }
        _running = false;
        PrintStatistics(turns);
    }

    private void PrintStatistics(int totalTurns) {
        Console.WriteLine("\n=== Execution Statistics ===");
        Console.WriteLine($"Total Turns: {totalTurns}");
        Console.WriteLine($"Total Ticks: {World.CurrentTick}");

        foreach (var entity in World.Entities) {
            var executionCount = entity.History.Count;
            var executionsPerTurn = (float)executionCount / totalTurns;
            var percentage = executionsPerTurn * 100;

            // Calculamos el coste total de energía de todas las acciones
            var totalEnergyCost = entity.History.Sum(action => action.EnergyCost);

            // Calculamos la energía total recargada (velocidad base * número de ticks)
            var totalEnergyRecharged = entity.BaseStats.BaseSpeed * World.CurrentTick;

            Console.WriteLine($"\n{entity.Name}:");
            Console.WriteLine($"- Total executions: {executionCount}");
            Console.WriteLine($"- Executions per turn: {executionsPerTurn:F2}");
            Console.WriteLine($"- Percentage per turn: {percentage:F1}%");
            Console.WriteLine($"- Base Speed: {entity.BaseStats.BaseSpeed}");
            Console.WriteLine($"- Total Energy Cost: {totalEnergyCost}");
            Console.WriteLine($"- Total Energy Recharged: {totalEnergyRecharged}");
            Console.WriteLine($"- Energy Balance: {totalEnergyRecharged - totalEnergyCost}");
        }
        Console.WriteLine("\n=========================");
    }

    private void HandlePlayerInput(EntityBlocking player) {
        while (_running) {
            if (player.IsWaiting) {
                var action = HandleMenuInput(player.Entity);
                player.SetResult(action);
            }
            Thread.Sleep(100); // Prevent tight loop
        }
    }

    private EntityAction HandleMenuInput(Entity player) {
        while (true) {
            // Console.Clear();
            // PrintArray2D(World);
            Console.WriteLine("\nSeleccione una acción:");
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

    private static void PrintArray2D(Array2D<WorldCell> array2D) {
        for (var y = 0; y < array2D.Height; y++) {
            for (var x = 0; x < array2D.Width; x++) {
                var value = array2D[y, x];
                // if (value == '.') value = ' ';
                Console.Write(value);
            }
            Console.WriteLine();
        }
    }
}

public class GameWorld {
    public Entity CurrentPlayer { get; set; }
    public Array2D<Entity> EntityGrid { get; set; }

    public static float GetDistance(Entity player, Entity goblin) {
        // var playerPosition = player.GetComponent<PositionComponent>().Position;
        // var goblinPosition = goblin.GetComponent<PositionComponent>().Position;
        // return Heuristics.Euclidean(playerPosition, goblinPosition);
        return 0f;
    }
}