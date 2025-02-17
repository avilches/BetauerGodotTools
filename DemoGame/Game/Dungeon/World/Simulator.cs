using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Betauer.Core;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.GridTemplate;
using Betauer.Core.PCG.Maze;
using Godot;
using Environment = System.Environment;

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

    private RogueWorld _rogueWorld;

    public void RunGameLoop(int turns, int millisecondsPerFrame = 16) {
        _rogueWorld = new RogueWorld();
        var templateContent = LoadTemplateContent(TemplatePath);
        RogueWorld.Configure(templateContent);
        _rogueWorld.Create();

        Task.Run(() => HandlePlayerInput(_rogueWorld.Player));

        var ticks = turns * _rogueWorld.TurnWorld.TicksPerTurn;
        _rogueWorld.TurnWorld.CreateTurnSystem().Run().Wait();
        /*
        var lastTick = Environment.TickCount;
        while (_running && _rogueWorld.TurnWorld.CurrentTick < ticks) {
            var currentTick = Environment.TickCount;
            var deltaMilliseconds = currentTick - lastTick;
            var deltaSeconds = deltaMilliseconds / 1000f;
            process._Process(deltaSeconds);
            lastTick = currentTick;
            Thread.Sleep(millisecondsPerFrame);
        }
        _running = false;
    */
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
            Console.WriteLine(_rogueWorld.PrintArray2D());
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