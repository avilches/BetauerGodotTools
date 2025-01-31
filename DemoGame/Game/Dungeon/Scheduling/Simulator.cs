using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Veronenger.Game.Dungeon.Scheduling;

public class Simulator {
    private readonly TurnSystem TurnSystem = new();
    private bool _running = true;

    public static void Main() {

        TaskScheduler.UnobservedTaskException += (sender, e) => {
            Console.WriteLine($"Unobserved Task Exception: {e.Exception}");
            e.SetObserved(); // Marca la excepción como observada
        };

        ActionConfig.RegisterAction(ActionType.Walk, 1000);
        ActionConfig.RegisterAction(ActionType.Attack, 1200);
        ActionConfig.RegisterAction(ActionType.Run, 2000);

        var game = new Simulator();
        var player = new EntityAsync("Player", new EntityStats {
        BaseSpeed = 100,
        });
        // var player = new Dummy(ActionType.Walk,"Player", new EntityStats {
            // BaseSpeed = 100,
        // });
        var goblin = new Dummy(ActionType.Walk, "Goblin", new EntityStats {
            BaseSpeed = 80,
        });
        var quickRat = new Dummy(ActionType.Walk, "Quick Rat", new EntityStats {
            BaseSpeed = 120,
        });

        game.TurnSystem.AddEntity(player);
        game.TurnSystem.AddEntity(goblin);
        game.TurnSystem.AddEntity(quickRat);
        game.RunGameLoop(100);
    }

    public void RunGameLoop(int turns) {
        Task.Run(HandlePlayerInput);

        var ticks = turns * TurnSystem.TicksPerTurn;
        var turnSystenProcess = new TurnSystemProcess(TurnSystem);
        while (_running && TurnSystem.CurrentTick < ticks) {
            turnSystenProcess._Process();
        }
        _running = false;

        PrintStatistics(turns);
    }

    private void PrintStatistics(int totalTurns) {
        Console.WriteLine("\n=== Execution Statistics ===");
        Console.WriteLine($"Total Turns: {totalTurns}");
        Console.WriteLine($"Total Ticks: {TurnSystem.CurrentTick}");

        foreach (var entity in TurnSystem.Entities) {
            var executionCount = entity.History.Count;
            var executionsPerTurn = (float)executionCount / totalTurns;
            var percentage = (executionsPerTurn * 100);

            // Calculamos el coste total de energía de todas las acciones
            var totalEnergyCost = entity.History.Sum(action => action.EnergyCost);

            // Calculamos la energía total recargada (velocidad base * número de ticks)
            var totalEnergyRecharged = entity.BaseStats.BaseSpeed * TurnSystem.CurrentTick;

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

    private static EntityAction HandleMenuInput(EntityAsync player) {
        while (true) {
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

    private void HandlePlayerInput() {
        while (_running) {
            foreach (var player in TurnSystem.Entities.OfType<EntityAsync>()) {
                if (player.IsWaiting) {
                    var action = HandleMenuInput(player);
                    player.SetResult(action);
                }
            }
            Thread.Sleep(100); // Prevent tight loop
        }
    }
}