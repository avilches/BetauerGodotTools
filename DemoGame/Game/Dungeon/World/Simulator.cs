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

namespace Veronenger.Game.Dungeon.World;

public class Simulator {
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

    private bool _running = true;
    public TurnWorld World;
    public GameWorld GameWorld;
    public Array2D<Template?> TemplateArray;

    private void Configure() {
        _ = new ActionTypeConfig(ActionType.Wait) { EnergyCost = 1000 };
        _ = new ActionTypeConfig(ActionType.Walk) { EnergyCost = 1000 };
        _ = new ActionTypeConfig(ActionType.Attack) { EnergyCost = 1000 };
        _ = new ActionTypeConfig(ActionType.Run) { EnergyCost = 500 };
        ActionTypeConfig.Verify();

        _ = new CellTypeConfig(CellType.Floor);
        _ = new CellTypeConfig(CellType.Wall);
        _ = new CellTypeConfig(CellType.Door);
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


    public const char DefinitionLoot = 'l';
    public const char DefinitionDoor = 'd';
    public const char DefinitionWall = '#';
    public const char DefinitionFloor = '·';
    public const char DefinitionEmpty = '.';

    private Array2D<WorldCell?> CreateMap(int seed) {
        var rng = new Random(seed);
        var zones = MazeGraphCatalog.CogmindLong(rng, mc => {
            // mc.OnNodeCreated += (node) => PrintGraph(mc);
        });
        var solution = zones.CalculateSolution(MazeGraphCatalog.KeyFormula);
        var keys = solution.KeyLocations;
        var loot = zones.SpreadLocationsByZone(0.3f, MazeGraphCatalog.LootFormula);

        var templateSet = new TemplateSet(cellSize: 9);

        // Cargar patrones de diferentes archivos
        try {
            var content = File.ReadAllText(TemplatePath);
            templateSet.LoadFromString(content);
            templateSet.FindTemplates(tags: ["disabled"]).ToArray().ForEach(t => templateSet.RemoveTemplate(t));
            templateSet.ApplyTransformations();

            // Creates an empty array2D of templates. This will be used in the Render method to track the template used for
            // each node during the selection the next one. The first one will be random, but the next one should try to match the
            // previous: the down side of the top template should match to the up side, the some for the node at the left: the right side of it
            // should match the left side of the current node.
            TemplateArray = zones.MazeGraph.ToArray2D(Template? (_, _) => null);

            var rngMap = new Random(seed);

            var nodeLootAdded = false;
            var templateNodes = zones.MazeGraph.Render((nodePos, node) => {
                    nodeLootAdded = false;
                    return NodeRenderer(rngMap, TemplateArray, templateSet, nodePos, node);
                },
                (nodePos, node, cellPosition, cell) => {
                var hasLoot = loot[node.ZoneId].Contains(node);
                if (cell == DefinitionLoot) {
                    var worldCell = WorldCellTypeConverter(cellPosition, DefinitionFloor);
                    if (hasLoot) {
                        nodeLootAdded = true;
                        worldCell.Entities.Add(new Entity("Loot", new EntityStats { BaseSpeed = 0 }));

                    }
                    return worldCell;
                }
                return WorldCellTypeConverter(cellPosition, cell);
            });

            MazeGraphZonedDemo.PrintGraph(zones.MazeGraph, zones);
            // PrintTemplates(templateSet.FindTemplates().ToList());

            return templateNodes;
        } catch (FileNotFoundException e) {
            Console.WriteLine(e.Message);
            Console.WriteLine($"{nameof(TemplatePath)} is '{TemplatePath}'");
            Console.WriteLine("Ensure the working directory is the root of the project");
            throw;
        }
    }

    private static void PrintTemplates(List<Template> templates) {
        foreach (var template in templates.OrderBy(t => t.Attributes["Template"]).ThenBy(t => t.Attributes.GetValueOrDefault("shared.id"))) {
            Console.WriteLine(template);
            Console.WriteLine(template.Body);
        }
    }

    private static Array2D<char> NodeRenderer(Random rngMap, Array2D<Template?> templateArray, TemplateSet templateSet, Vector2I pos, MazeNode node) {
        var upNodeTemplate = templateArray.GetValueSafe(pos + Vector2I.Up);
        var leftNodeTemplate = templateArray.GetValueSafe(pos + Vector2I.Left);
        var allTemplates = templateSet.FindTemplates(node.GetDirectionFlags()).ToList();
        var candidates = allTemplates
            .Where(t => MatchesNodeUpDown(upNodeTemplate, t) && MatchesNodeLeftRight(leftNodeTemplate, t))
            .ToList();
        var x = candidates.Count;

        RemoveSimilarCandidates(candidates, templateArray);
        Console.WriteLine($"Candidates before: {x} - after: {candidates.Count} = {x - candidates.Count} delted");
        Template template = null!;
        if (candidates.Count == 0) {
            Console.WriteLine($"Warning: no matching template for node {node} at position {pos}. Direction: {DirectionFlagTools.FlagsToString(node.GetDirectionFlags())} ");
            template = rngMap.Next(allTemplates.ToArray());
        } else {
            template = rngMap.Next(candidates);
        }
        templateArray[pos] = template;
        return template.Body;

        static bool MatchesNodeUpDown(Template? upNodeTemplate, Template? downNodeTemplate) {
            return upNodeTemplate == null || downNodeTemplate == null ||
                   Equals(upNodeTemplate.GetAttribute(DirectionFlag.Down), downNodeTemplate.GetAttribute(DirectionFlag.Up));
        }

        static bool MatchesNodeLeftRight(Template? leftNodeTemplate, Template? rightNodeTemplate) {
            return leftNodeTemplate == null || rightNodeTemplate == null ||
                   Equals(leftNodeTemplate.GetAttribute(DirectionFlag.Right), rightNodeTemplate.GetAttribute(DirectionFlag.Left));
        }

        // Removes all templates from the templateArray array that are similar to the check template
        // the matchingTemplates will end up with at least one element (we don't want to delete all the templates!)
        static void RemoveSimilarCandidates(List<Template> matchingTemplates, Array2D<Template?> templateArray) {
            foreach (var check in templateArray.GetIndexedValues()
                         .Select(tuple => tuple.Value)
                         .Where(t => t != null).Cast<Template>().Reverse()) {
                foreach (var similar in matchingTemplates.Where(t => TemplateSet.ShareOrigin(t, check)).ToArray()) {
                    if (matchingTemplates.Remove(similar) && matchingTemplates.Count == 1) return;
                }
            }
        }
    }


    private static WorldCell? WorldCellTypeConverter(Vector2I position, char c) {
        return c switch {
            '.' => null,
            '·' => new WorldCell(CellType.Floor, position),
            '#' => new WorldCell(CellType.Wall, position),
            'l' => new WorldCell(CellType.Wall, position),
            'd' => new WorldCell(CellType.Door, position),
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