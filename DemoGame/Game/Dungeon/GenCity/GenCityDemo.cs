using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Betauer.Core;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public class GenCityDemo {
    private const char PLAYER = '@'; // Jugador

    private readonly char[] BUILDING_CHARS = { '#', '■', '□', '▣', '▤', '▥', '▦', '▧', '▨', '▩' };

    private int _playerX;
    private int _playerY;
    private bool _running = true;
    private CityGenerator _generator;
    private City _city;
    private CityRender _render;

    public const int Width = 140;
    public const int Height = 25;
    public const int Seed = 0;

    public static void Show() {
        Console.OutputEncoding = Encoding.UTF8; // Para caracteres especiales
        new GenCityDemo().Start();
    }

    public static void Validate() {
        Console.OutputEncoding = Encoding.UTF8; // Para caracteres especiales
        new GenCityDemo().ValidateStart();
    }

    public void ValidateStart() {
        _city = new City(Width, Height);
        _generator = _city.CreateGenerator(CreateOptions());
        _render = new CityRender(_city);
        var accDensity = 0f;
        var minDensity = float.MaxValue;
        var maxDensity = float.MinValue;

        const int count = 10;
        for (var i = Seed; i < count; i++) {
            _generator.Options.Seed = i;
            if (i % 100 == 0) {
                Console.WriteLine(_generator.Options.Seed);
            }
            try {
                _generator.Start();
                _generator.Grow();
                _generator.FillGaps();

                /*
                if (!_generator.Generate(0.26f, 0.5f)) {
                    Console.WriteLine(_generator.Options.Seed + " nop");
                }
                */

                var density = _city.GetDensity();
                accDensity += density;
                minDensity = Math.Min(minDensity, density);
                maxDensity = Math.Max(maxDensity, density);

                _city.ValidateIntersections(true);
                _city.ValidateRoads();
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

    public void Start() {
        _city = new City(Width, Height);
        _generator = _city.CreateGenerator(CreateOptions());
        _render = new CityRender(_city);
        GenerateCity(Seed);
        InitializePlayer();
        Render();
        while (_running) {
            HandleInput();
            Thread.Sleep(50); // Pequeña pausa para reducir el uso de CPU
        }
    }

    private void GenerateCity(int seed) {
        _city.OnUpdate = (_) => {
            // Render();
            _city.ValidateRoads();
            _city.ValidateIntersections();
        };
        _generator.Options.Seed = seed;
        _generator.Start();
        _generator.Grow();

        _city.ValidateIntersections(true);
        _city.ValidateIntersectionPaths();
        _city.ValidateRoads();

        // _generator.GenerateBuildings();
    }

    private static CityGenerationOptions CreateOptions() {
        List<Vector2I> startDirections = [Vector2I.Right, Vector2I.Down, Vector2I.Left, Vector2I.Up];

        var options = new CityGenerationOptions {
            StartDirections = startDirections,

            StreetMinLength = 6,

            ProbabilityIntersection = 0.20f,

            ProbabilityCross = 0.42f,
            ProbabilityFork = 0.42f,
            ProbabilityTurn = 0.12f,

            ProbabilityStreetEnd = 0.001f,

            BuildingMinSize = 2, //2,
            BuildingMaxSize = 5, //3,
            BuildingMinSpace = 1,
            BuildingMaxSpace = 2, // 2
        };
        return options;
    }

    private void Render() {
        _render.Render();
        RenderBuildings();
        RenderGame();
    }

    private void RenderBuildings() {
        var buildingIndex = 0;
        foreach (var building in _city.Buildings) {
            var buildingChar = BUILDING_CHARS[buildingIndex % BUILDING_CHARS.Length];
            foreach (var position in building.GetPositions()) {
                _render.AsciiMap[position] = buildingChar;
            }
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
        Console.WriteLine($"Seed: {_generator.Seed} | J/K/L = Seed | F = Fill ({_city.GetDensity() * 100:0}%)f | Q = Quit");

        var tile = _city.Data[_playerY, _playerX];
        Console.WriteLine($"Pos: {_playerX}, {_playerY} | {tile}");
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
                    GenerateCity(_generator.Seed);
                    _generator.FillGaps(0.15f);
                    Render();
                    return;
                case ConsoleKey.D:
                    GenerateCity(_generator.Seed);
                    _generator.FillGaps(0.18f);
                    Render();
                    return;
                case ConsoleKey.F:
                    GenerateCity(_generator.Seed);
                    _generator.FillGaps(0.25f);
                    Render();
                    return;
                case ConsoleKey.G:
                    GenerateCity(_generator.Seed);
                    _generator.FillGaps(0.35f);
                    Render();
                    return;
                case ConsoleKey.H:
                    _generator.Generate(0.22f);
                    Render();
                    return;
                case ConsoleKey.J:
                    GenerateCity(_generator.Seed - 1);
                    InitializePlayer();
                    Render();
                    return;
                case ConsoleKey.K:
                    GenerateCity(_generator.Seed);
                    InitializePlayer();
                    Render();
                    return;
                case ConsoleKey.L:
                    GenerateCity(_generator.Seed + 1);
                    InitializePlayer();
                    Render();
                    return;

                case ConsoleKey.Z:
                    _generator.GenerateBuildings();
                    Render();
                    return;

                case ConsoleKey.X:
                    _city.RemoveBuildings();
                    Render();
                    return;
            }

            if (newX >= 0 && newX < Width && newY >= 0 && newY < Height) {
                _playerX = newX;
                _playerY = newY;
            }
            RenderGame();
        }
    }
}