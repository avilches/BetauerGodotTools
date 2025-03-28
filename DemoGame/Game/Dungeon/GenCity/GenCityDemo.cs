using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Betauer.Core;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Betauer.Core.Examples;
using Betauer.Core.PCG.GridTemplate;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public class GenCityDemo {
    private const char PLAYER = '@'; // Jugador

    const string TemplatePath = "/Users/avilches/Library/Mobile Documents/com~apple~CloudDocs/Shared/Godot/Betauer4/DemoGame/Game/Dungeon/CityTemplateDemos3-line.txt";
    const int TemplateCellSize = 3;

    // private readonly char[] BUILDING_CHARS = { '#', '■', '□', '▣', '▤', '▥', '▦', '▧', '▨', '▩' };
    private readonly char[] BUILDING_CHARS = { '▓', '█', '▒', '▞' };

    private int _playerX;
    private int _playerY;
    private bool _running = true;
    private CityGenerator _generator;
    private CityMaze _cityMaze;
    private City _city;
    private CityRender _render;

    const int SectionSize = 24;
    public const int Width = 140;
    public const int Height = 25;
    public const int Seed = 0;

    public static GenCityDemo Instance;

    public static void Show() {
        Console.OutputEncoding = Encoding.UTF8; // Para caracteres especiales
        Instance = new GenCityDemo();
        Instance.Start();
    }

    public static void Validate() {
        Console.OutputEncoding = Encoding.UTF8; // Para caracteres especiales
        new GenCityDemo().ValidateGenerateCityMaze();
    }

    public void ValidateAndFindDensity() {
        _city = new City(Width, Height);
        _generator = _city.CreateGenerator(CreateOptions());
        _render = new CityRender(_city);
        var accDensity = 0f;
        var minDensity = float.MaxValue;
        var maxDensity = float.MinValue;

        const int count = 1000;
        for (var i = Seed; i < count; i++) {
            _generator.Options.Seed = i;
            if (i % 100 == 0) {
                Console.WriteLine(_generator.Options.Seed);
            }
            try {
                _generator.City.RemovePathsAndBuildings();
                _generator.Start();
                _generator.Grow();
                _generator.FillGaps();

                /*
                if (!_generator.Generate(0.26f, 0.5f)) {
                    Console.WriteLine(_generator.Options.Seed + " nop");
                }
                */

                var density = _city.GetPathDensity();
                accDensity += density;
                minDensity = Math.Min(minDensity, density);
                maxDensity = Math.Max(maxDensity, density);

                _city.ValidateIntersections(true);
                _city.ValidatePaths();
                _city.ValidateIntersectionPaths();
            } catch (Exception e) {
                Render();
                Console.WriteLine($"Seed:{_generator.Options.Seed}");
                Console.WriteLine(e);
                throw;
            }
        }
        Console.WriteLine($"Average density {(accDensity * 100 / count):0.00}%");
        Console.WriteLine($"Min density {minDensity * 100:0.00}%");
        Console.WriteLine($"Max density {maxDensity * 100:0.00}%");
    }

    public void ValidateGenerateCityMaze() {

        const int count = 1000;
        for (var i = Seed; i < count; i++) {
            _cityMaze = new CityMaze(MazeGraphCatalog.City(new Random(i)), SectionSize);
            _city = _cityMaze.City;
            _render = new CityRender(_city);
            _generator = _cityMaze.Generator;
            _generator.Options.Seed = i;
            // if (i % 10 == 0) {
                Console.WriteLine($"Seed:{_generator.Options.Seed}");
            // }
            try {
                _cityMaze.AddLimits(new Other('#'));

                _generator.Options = CreateOptions();
                _generator.Options.StartPosition = _cityMaze.GetStartPosition();
                _generator.Options.Seed = Seed;
                _generator.Options.SeedOffset = 0;
                
                _generator.Generate(() => _cityMaze.Validate(), 0.22f);

                _city.ValidateIntersections(true);
                _city.ValidatePaths();
                _city.ValidateIntersectionPaths();
            } catch (Exception e) {
                Render();
                Console.WriteLine($"Seed:{_generator.Options.Seed}");
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public void Start() {
        GenerateCityMaze(Seed, 0);
        InitializePlayer();
        Render();
        while (_running) {
            HandleInput();
            Thread.Sleep(50); // Pequeña pausa para reducir el uso de CPU
        }
    }

    private void GenerateSingleCity(int seed, int seedOffset) {
        _city.OnUpdate = (_) => {
            // Render();
            _city.ValidatePaths();
            _city.ValidateIntersections();
        };
        _generator.Options.Seed = seed;
        _generator.Options.SeedOffset = seedOffset;
        _generator.Start();
        _generator.Grow();

        _city.ValidateIntersections(true);
        _city.ValidateIntersectionPaths();
        _city.ValidatePaths();
    }

    private void GenerateCityMaze(int seed, int seedOffset) {
        _cityMaze = new CityMaze(MazeGraphCatalog.City(new Random(seed)), SectionSize);
        _city = _cityMaze.City;
        _render = new CityRender(_city);
        _generator = _cityMaze.Generator;

        MazeGraphZonedDemo.PrintGraph(_cityMaze.MazeGraph, _cityMaze.MazeZones);

        _cityMaze.AddLimits(new Other('#'));

        _generator.Options = CreateOptions();
        _generator.Options.StartPosition = _cityMaze.GetStartPosition();
        _generator.Options.Seed = seed;
        _generator.Options.SeedOffset = seedOffset;

        _generator.Start();
        _generator.Grow();

        /*
        zones.MazeGraph.ToArray2D<bool>((nodePos, node) => {
            for (var x = 0; x < zoneSize; x++) {
                for (var y = 0; y < zoneSize; y++) {
                    var p = new Vector2I(nodePos.X * zoneSize + x, nodePos.Y * zoneSize + y);
                    if (_city.Data[p] == null) {
                        // _city.Data[p] = node == null ? new Other(z % 2 == 0 ?'·' : '%') : null;
                        // _city.Data[p] = node != null ? new Other((char)(node.ZoneId+'0')) : null;
                    }
                }
            }
            return false;
        });
        */

        _city.ValidateIntersections(false);
        _city.ValidateIntersectionPaths();
        _city.ValidatePaths();
    }

    public void GenerateBuildings() {
        var paths = _city.GetAllPaths().ToList();
        var big = BuildingGenerator.GenerateBuildings(_city, paths, new BuildingGenerationOptions {
            Total = 6,
            MinSize = 5, //2,
            MaxSize = 12, //3,
            MinSpace = 2,
            MaxSpace = 2, // 2
            Sidewalk = 2 
        });
        var med = BuildingGenerator.GenerateBuildings(_city, paths, new BuildingGenerationOptions {
            MinSize = 4, //2,
            MaxSize = 5, //3,
            MinSpace = 1,
            MaxSpace = 2, // 2
            Sidewalk = 2 
        });
        var sm = BuildingGenerator.GenerateBuildings(_city, paths, new BuildingGenerationOptions {
            MinSize = 2, //2,
            MaxSize = 3, //3,
            MinSpace = 0,
            MaxSpace = 0, // 2
            Sidewalk = 1
        });
        Console.WriteLine($"Big: {big.Count} med: {med.Count} small: {sm.Count}");
        
        
        foreach (var path in _city.GetAllPaths().Where(p => p.Buildings.Count > 0)) {
            foreach (var p in _city.GetPathSidewalk(path, true).Concat(_city.GetPathSidewalk(path, false))) {
                if (_city.Data[p] == null) {
                    _city.Data[p] = new Other('·');
                }
            }
            
        }
    }

    private static CityGenerationOptions CreateOptions() {
        List<Vector2I> startDirections = [Vector2I.Right, Vector2I.Down, Vector2I.Left, Vector2I.Up];

        var options = new CityGenerationOptions {
            StartDirections = startDirections,

            StreetMinLength = 8,

            ProbabilityIntersection = 0.20f,

            ProbabilityCross = 0.42f,
            ProbabilityFork = 0.42f,
            ProbabilityTurn = 0.12f,

            ProbabilityStreetEnd = 0.001f,
        };
        return options;
    }

    private void Render() {
        // RenderScaled();
        _render.Render();
        RenderBuildings();
        RenderGame();
    }

    private void RenderScaled() {
        var rng = new Random(_generator.Options.Seed);
        var templateSet = new TemplateSet(TemplateCellSize);

        var content = File.ReadAllText(TemplatePath);
        templateSet.LoadFromString(content);
        // templateSet.ValidateAll(c => c is '.', true);
        // templateSet.ApplyTransformations(c => c is '.');

        var empty = new Array2D<char>(TemplateCellSize, TemplateCellSize);
        empty.Fill(' ');
        var array2D = _city.Data.Expand(TemplateCellSize, (pos, tile) => {
            if (tile == null) return empty;
            var templates = templateSet.FindTemplates(tile.GetDirectionFlags()).ToArray();
            return rng.Next(templates).Body;
        });
        Console.WriteLine(array2D);
    }

    private void RenderBuildings() {
        var buildingIndex = 0;
        foreach (var building in _city.Buildings) {
            var buildingChar = BUILDING_CHARS[buildingIndex % BUILDING_CHARS.Length];
            foreach (var position in building.GetPositions()) {
                _render.AsciiMap[position] = buildingChar;
            }
            _render.AsciiMap[building.Entrance] = '#';
            _render.AsciiMap[building.PathEntrance] = '*';
            buildingIndex++;
        }
    }

    private void InitializePlayer() {
        // Colocar al jugador en una carretera disponible
        foreach (var intersection in _city.Intersections) {
            _playerX = intersection.Position.X;
            _playerY = intersection.Position.Y;

            // Verificar que esté dentro de los límites
            if (_playerX >= 0 && _playerX < Width && _playerY >= 0 && _playerY < Height) {
                break; // Posición válida encontrada
            }
        }
    }

    private void RenderGame() {
        // Console.Clear();

        var asciiMap = _render.AsciiMap;

        var (width, height) = (asciiMap.Width, asciiMap.Height);

        // Crear una copia del mapa para añadir al jugador
        var displayMap = new char[height, width];
        Array.Copy(asciiMap.Data, displayMap, asciiMap.Data.Length);

        // Añadir jugador
        displayMap[_playerY, _playerX] = PLAYER;

        var buffer = new StringBuilder();
        buffer.AppendLine("┌" + new string('─', width) + "┐");
        for (var y = 0; y < height; y++) {
            buffer.Append('│');
            for (var x = 0; x < width; x++) {
                buffer.Append(displayMap[y, x]);
            }
            buffer.AppendLine("│");
        }
        buffer.AppendLine("└" + new string('─', width) + "┘");

        Console.Write(buffer.ToString());
        Console.WriteLine($"Seed: {_generator.Seed}/{_generator.Options.SeedOffset} | {_cityMaze.CrossingPaths.Count} J/K/L = Seed | F = Fill ({_city.GetPathDensity() * 100:0}% paths) ({_city.Buildings.Count} buildings) | Q = Quit");

        var tile = _city.Data[_playerY, _playerX];
        Console.WriteLine($"Pos: {_playerX}, {_playerY} | {tile}");
        var mazeNode = tile?.Attributes().GetAs<MazeNode>("mazeNode");
        Console.WriteLine($"MazeNode {mazeNode} | {(tile is Path p && _cityMaze.CrossingPaths.ContainsKey(p)?"Crossing":"")}");
    }

    private void HandleInput() {
        if (Console.KeyAvailable) {
            var key = Console.ReadKey(true).Key;
            var newX = _playerX;
            var newY = _playerY;
            switch (key) {
                case ConsoleKey.UpArrow:
                    newY--;
                    break;
                case ConsoleKey.DownArrow:
                    newY++;
                    break;
                case ConsoleKey.LeftArrow:
                    newX--;
                    break;
                case ConsoleKey.RightArrow:
                    newX++;
                    break;
                case ConsoleKey.Q:
                    _running = false;
                    return;
                case ConsoleKey.P:
                    _cityMaze.ProcessSectionsWithMazeNodes();
                    Render();
                    break;
                case ConsoleKey.A:
                    var c = 'A';
                    foreach (var gap in _generator.FindGaps()) {
                        var ratio = (float)gap.Size.X / gap.Size.Y;
                        if (ratio < 1) ratio = 1f / ratio;
                        if (ratio < 2) continue;
                        Console.WriteLine($"{c}: {ratio:0.00}");
                        foreach (var p in gap.GetPositions()) _city.Data[p] = new Other(c);
                        c++;
                    }
                    Render();
                    return;
                case ConsoleKey.S:
                    GenerateCityMaze(_generator.Seed, _generator.Options.SeedOffset);
                    _generator.FillGaps(0.15f);
                    Render();
                    return;
                case ConsoleKey.D:
                    GenerateCityMaze(_generator.Seed, _generator.Options.SeedOffset);
                    _generator.FillGaps(0.18f);
                    Render();
                    return;
                case ConsoleKey.F:
                    GenerateCityMaze(_generator.Seed, _generator.Options.SeedOffset);
                    _generator.FillGaps(0.25f);
                    Render();
                    return;
                case ConsoleKey.G:
                    GenerateCityMaze(_generator.Seed, _generator.Options.SeedOffset);
                    _generator.FillGaps(0.35f);
                    Render();
                    return;
                case ConsoleKey.H:
                    _city.RemoveAllBuildings();
                    _generator.Generate(() => _cityMaze.Validate(), 0.13f);
                    Render();
                    return;
                case ConsoleKey.J:
                    GenerateCityMaze(_generator.Seed - 1, 0);
                    InitializePlayer();
                    Render();
                    return;
                case ConsoleKey.K:
                    GenerateCityMaze(_generator.Seed, 0);
                    InitializePlayer();
                    Render();
                    return;
                case ConsoleKey.L:
                    GenerateCityMaze(_generator.Seed + 1, 0);
                    InitializePlayer();
                    Render();
                    return;

                case ConsoleKey.Z:
                    GenerateBuildings();
                    Render();
                    return;

                case ConsoleKey.X:
                    _city.RemoveAllBuildings();
                    Render();
                    return;
                
                case ConsoleKey.C:
                    _cityMaze.FixBuilding();
                    Render();
                    return;
            }

            if (newX >= 0 && newX < _city.Width && newY >= 0 && newY < _city.Height) {
                _playerX = newX;
                _playerY = newY;
            }
            RenderGame();
        }
    }
}