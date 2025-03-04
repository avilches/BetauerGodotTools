using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Betauer.Core;
using Betauer.Core.DataMath;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public class GenCityDemo {
    private const char PLAYER = '@'; // Jugador

    private readonly char[] BUILDING_CHARS = { '#', '■', '□', '▣', '▤', '▥', '▦', '▧', '▨', '▩' };

    private int _playerX;
    private int _playerY;
    private bool _running = true;
    private CityGenerator _generator;
    private CityRender _render;

    public const int Width = 140;
    public const int Height = 25;
    public const int Seed = 0;

    public static void Show() {
        new GenCityDemo().Start();
    }

    public static void Validate() {
        new GenCityDemo().ValidateStart();
    }

    public void ValidateStart() {
        _generator = new City(Width, Height).CreateGenerator(CreateOptions(Seed));

        for (var i = Seed; i < 100000; i++) {
            _generator.Options.Seed = i;
            if (i % 100 == 0) {
                Console.WriteLine(_generator.Options.Seed);
            }
            try {
                _generator.Start();
                _generator.Grow();

                _generator.City.ValidateIntersections(true);
                _generator.City.ValidateRoads();
                _generator.City.ValidateIntersectionPaths();

            } catch (Exception e) {
                Render();
                Console.WriteLine($"Seed:{_generator.Options.Seed}");
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public void Start() {
        Console.OutputEncoding = Encoding.UTF8; // Para caracteres especiales
        GenerateCity(Seed);
        InitializePlayer();
        while (_running) {
            HandleInput();
            Thread.Sleep(50); // Pequeña pausa para reducir el uso de CPU
        }
    }

    private void GenerateCity(int seed) {
        _generator = new City(Width, Height).CreateGenerator(CreateOptions(seed));
        _render = new CityRender(_generator.City);
        _generator.City.OnUpdate = (_) => {
            Render();
            _generator.City.ValidateRoads();
            _generator.City.ValidateIntersections(true);
        };
        _generator.Start();
        _generator.Grow();

        Render();

        _generator.City.ValidateIntersections(true);
        _generator.City.ValidateIntersectionPaths();
        _generator.City.ValidateRoads();

        _generator.GenerateBuildings();
        Render();
    }

    private static CityGenerationOptions CreateOptions(int seed) {
        List<Vector2I> startDirections = [Vector2I.Right, Vector2I.Down, Vector2I.Left, Vector2I.Up];

        var options = new CityGenerationOptions {
            Seed = seed,
            StartDirections = startDirections,

            StreetMinLength = 3,

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
        foreach (var building in _generator.City.Buildings) {
            var buildingChar = BUILDING_CHARS[buildingIndex % BUILDING_CHARS.Length];
            foreach (var position in building.GetPositions()) {
                _render.AsciiMap[position] = buildingChar;
            }
            buildingIndex++;
        }
    }

    private void InitializePlayer() {
        // Colocar al jugador en una carretera disponible
        foreach (var intersection in _generator.City.Intersections) {
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

        StringBuilder buffer = new StringBuilder();
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
        Console.WriteLine($"Seed: {_generator.Seed} R = Next seed - Q = Quit");

        var tile = _generator.City.Data[_playerY, _playerX];
        Func<Vector2I, string> t = d => d == Vector2I.Right ? "->" : d == Vector2I.Down ? "v" : d == Vector2I.Left ? "<-" : "^";

        Console.WriteLine($"Pos: {_playerX}, {_playerY} | {tile}");
    }

    private void HandleInput() {
        if (Console.KeyAvailable) {
            var key = Console.ReadKey(true).Key;

            int newX = _playerX;
            int newY = _playerY;

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
                case ConsoleKey.R:
                    // Regenerar la ciudad
                    GenerateCity(_generator.Seed + 1);
                    InitializePlayer();
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