using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Betauer.Core.DataMath;
using Betauer.Core.Examples;
using Betauer.Core.PCG.GridTemplate;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Veronenger.Game.Dungeon.World;

public class Simulator {
    private bool _running = true;
    const string TemplatePath = "/Users/avilches/Library/Mobile Documents/com~apple~CloudDocs/Shared/Godot/Betauer4/DemoGame/Game/Dungeon/MazeTemplateDemos.txt";

    public static void Main() {
        TaskScheduler.UnobservedTaskException += (sender, e) => {
            Console.WriteLine($"Unobserved Task Exception: {e.Exception}");
            e.SetObserved(); // Marca la excepción como observada
        };

        var simulator = new Simulator();
        simulator.Configure();
        simulator.RunGameLoop(100);
    }

    public TurnWorld World;
    public GameWorld GameWorld;

    private void Configure() {
        _ = new ActionTypeConfig(ActionType.Wait) { EnergyCost = 1000 };
        _ = new ActionTypeConfig(ActionType.Walk) { EnergyCost = 1000 };
        _ = new ActionTypeConfig(ActionType.Attack) { EnergyCost = 1000 };
        _ = new ActionTypeConfig(ActionType.Run) { EnergyCost = 500 };
        ActionTypeConfig.Verify();

        _ = new CellTypeConfig(CellType.Floor);
        _ = new CellTypeConfig(CellType.Wall);
        CellTypeConfig.Verify();

        var seed = 1;
        World = new TurnWorld(CreateMap(seed)) {
            TicksPerTurn = 10,
            DefaultCellType = CellType.Floor
        };
        GameWorld = new GameWorld();
        CreatePlayer();
        CreateEntities();
    }

    private Array2D<WorldCell> CreateMap(int seed) {
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

            var array2D = zones.MazeGraph.Render(TemplateSelector.Create(templateSet), WorldCellTypeConverter);

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
            // MazeGraphZonedDemo.PrintGraph(zones.MazeGraph, zones);
            return array2D;
        } catch (FileNotFoundException e) {
            Console.WriteLine(e.Message);
            Console.WriteLine($"{nameof(TemplatePath)} is '{TemplatePath}'");
            Console.WriteLine("Ensure the working directory is the root of the project");
            throw;
        }
    }

    private static WorldCell? WorldCellTypeConverter(Vector2I position, char c) {
        return c switch {
            '.' => null,
            '·' => new WorldCell(CellType.Floor, position),
            '#' => new WorldCell(CellType.Wall, position),
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, "Invalid character")
        };
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
        var ticks = turns * World.TicksPerTurn;
        var turnSystemProcess = new TurnSystemProcess(new TurnSystem(World));
        while (_running && World.CurrentTick < ticks) {
            turnSystemProcess._Process();
        }
        _running = false;
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
                } finally {
                }
                Thread.Sleep(100); // Prevent tight loop
            }
        }
    }

    private EntityAction HandleMenuInput(Entity player) {
        while (true) {
            // Console.Clear();
            Console.WriteLine(PrintArray2D());
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

    private string PrintArray2D() {
        var stringBuilder = new StringBuilder();
        Console.WriteLine(World.Width);
        Console.WriteLine(World.Height);
        for (var y = 0; y < World.Height; y++) {
            for (var x = 0; x < World.Width; x++) {
                var cell = World[y, x];
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

public class GameWorld {
    public Entity CurrentPlayer { get; set; }

    public static float GetDistance(Entity player, Entity goblin) {
        // var playerPosition = player.GetComponent<PositionComponent>().Position;
        // var goblinPosition = goblin.GetComponent<PositionComponent>().Position;
        // return Heuristics.Euclidean(playerPosition, goblinPosition);
        return 0f;
    }
}