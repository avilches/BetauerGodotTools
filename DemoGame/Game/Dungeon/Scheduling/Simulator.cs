using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Betauer.Core.DataMath;
using Betauer.Core.Examples;
using Betauer.Core.PCG.GridTemplate;
using Betauer.Core.PCG.Maze;

namespace Veronenger.Game.Dungeon.Scheduling;

public class Simulator {
    private bool _running = true;
    const string TemplatePath = "/Users/avilches/Library/Mobile Documents/com~apple~CloudDocs/Shared/Godot/Betauer4/DemoGame/Game/Dungeon/MazeTemplateDemos.txt";

    public static void Main() {
        TaskScheduler.UnobservedTaskException += (sender, e) => {
            Console.WriteLine($"Unobserved Task Exception: {e.Exception}");
            e.SetObserved(); // Marca la excepción como observada
        };

        ActionConfig.RegisterAction(ActionType.Walk, 1000);
        ActionConfig.RegisterAction(ActionType.Attack, 1200);
        ActionConfig.RegisterAction(ActionType.Run, 2000);

        var seed = 1;
        var game = new Simulator();
        game.CreateMap(seed);
        game.CreatePlayer();
        game.CreateEntities();
        game.RunGameLoop(100);
    }


    public readonly TurnSystem TurnSystem = new(new TurnContext());
    public Array2D<char> Array2D;

    private void CreateMap(int seed) {
        var rng = new Random(seed);
        var zones = MazeGraphCatalog.BigCycle(rng, mc => {
            // mc.OnNodeCreated += (node) => PrintGraph(mc);
        });
        zones.CalculateSolution(MazeGraphCatalog.KeyFormula);

        // AddFlags(zones);

        var templateSet = new TemplateSet(cellSize: 7);

        // Cargar patrones de diferentes archivos
        try {
            var content = File.ReadAllText(TemplatePath);
            templateSet.LoadTemplates(content);

            Array2D = zones.MazeGraph.Render(TemplateSelector.Create(templateSet));

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
            .DecideAction(ActionType.Walk)
            .Build();

        var quickRat = EntityBuilder.Create("Goblin", new EntityStats { BaseSpeed = 80 })
            .DecideAction(ActionType.Walk)
            .Build();

        TurnSystem.Context.AddEntity(goblin);
        TurnSystem.Context.AddEntity(quickRat);
    }

    private void CreatePlayer() {
        var player = EntityBuilder.Create("Player", new EntityStats { BaseSpeed = 100 })
            .BuildAsync();
        TurnSystem.Context.AddEntity(player.Entity);
        Task.Run(() => HandlePlayerInput(player));
    }

    public void RunGameLoop(int turns) {
        var ticks = turns * TurnContext.TicksPerTurn;
        var turnSystemProcess = new TurnSystemProcess(TurnSystem);
        while (_running && TurnSystem.Context.CurrentTick < ticks) {
            turnSystemProcess._Process();
        }
        _running = false;
        PrintStatistics(turns);
    }

    private void PrintStatistics(int totalTurns) {
        Console.WriteLine("\n=== Execution Statistics ===");
        Console.WriteLine($"Total Turns: {totalTurns}");
        Console.WriteLine($"Total Ticks: {TurnSystem.Context.CurrentTick}");

        foreach (var entity in TurnSystem.Context.Entities) {
            var executionCount = entity.History.Count;
            var executionsPerTurn = (float)executionCount / totalTurns;
            var percentage = executionsPerTurn * 100;

            // Calculamos el coste total de energía de todas las acciones
            var totalEnergyCost = entity.History.Sum(action => action.EnergyCost);

            // Calculamos la energía total recargada (velocidad base * número de ticks)
            var totalEnergyRecharged = entity.BaseStats.BaseSpeed * TurnSystem.Context.CurrentTick;

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

    private void HandlePlayerInput(EntityAsync player) {
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
            Console.Clear();
            PrintArray2D(Array2D);
            Console.WriteLine("\nSeleccione una acción:");
            Console.Write("1) Walk 2) Attack: ");
            if (int.TryParse(Console.ReadLine(), out var choice)) {
                switch (choice) {
                    case 1:
                        return new EntityAction(ActionType.Walk, player);
                    case 2:
                        return new EntityAction(ActionType.Attack, player);
                    default:
                        Console.WriteLine("Opción no válida");
                        break;
                }
            } else {
                Console.WriteLine("Por favor, introduce un número");
            }
        }
    }

    private static void PrintArray2D(Array2D<char> array2D) {
        for (var y = 0; y < array2D.Height; y++) {
            for (var x = 0; x < array2D.Width; x++) {
                var value = array2D[y, x];
                if (value == '.') value = ' ';
                Console.Write(value);
            }
            Console.WriteLine();
        }
    }
}