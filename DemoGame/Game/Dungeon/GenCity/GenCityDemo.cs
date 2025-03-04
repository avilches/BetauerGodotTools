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
    // Caracteres ASCII para la representación
    private const char EMPTY = ' '; // Espacio vacío
    private const char ROAD_H = '═'; // Carretera horizontal
    private const char CROSS_H = '-'; // Carretera horizontal
    private const char ROAD_V = '║'; // Carretera vertical
    private const char CROSS_V = '|'; // Carretera vertical
    private const char CROSS = '╬'; // Intersección
    private const char TURN_NE = '╚'; // Giro noreste
    private const char TURN_NW = '╝'; // Giro noroeste
    private const char TURN_SE = '╔'; // Giro sureste
    private const char TURN_SW = '╗'; // Giro suroeste
    private const char END = '•'; // Fin de carretera
    private const char BUILDING = '#'; // Edificio
    private const char PLAYER = '@'; // Jugador

    private readonly char[] BUILDING_CHARS = { '#', '■', '□', '▣', '▤', '▥', '▦', '▧', '▨', '▩' };

    private Array2D<char> _asciiMap;
    private int _playerX;
    private int _playerY;
    private bool _running = true;
    private CityGenerator _generator;

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
        _asciiMap = new Array2D<char>(Width, Height);
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
        _asciiMap = new Array2D<char>(Width, Height);
        _generator = new City(Width, Height).CreateGenerator(CreateOptions(seed));
        _generator.City.OnUpdate = (_) => {
            Render();
            _generator.City.ValidateRoads();
            _generator.City.ValidateIntersections();
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
        ClearRender();
        RenderRoads();
        RenderIntersections();
        RenderBuildings();
        RenderGame();
    }

    private void ClearRender() {
        _asciiMap.Fill(EMPTY);
    }

    private void RenderBuildings() {
        var buildingIndex = 0;
        foreach (var building in _generator.City.Buildings) {
            var buildingChar = BUILDING_CHARS[buildingIndex % BUILDING_CHARS.Length];
            foreach (var position in building.GetPositions()) {
                _asciiMap[position] = buildingChar;
            }
            buildingIndex++;
        }
    }

    private void RenderRoads() {
        foreach (var path in _generator.City.GetAllPaths()) {
            var isHorizontal = path.Direction.IsHorizontal();
            var roadChar = isHorizontal ? ROAD_H : ROAD_V;
            foreach (var position in path.GetPositions()) {
                _asciiMap[position] = roadChar;
            }
        }
    }

    private void RenderIntersections() {
        foreach (var intersection in _generator.City.Intersections) {
            var hasNorth = intersection.Up != null;
            var hasSouth = intersection.Down != null;
            var hasEast = intersection.Right != null;
            var hasWest = intersection.Left != null;
            var intersectionChar = DetermineIntersectionChar(hasNorth, hasSouth, hasEast, hasWest);
            _asciiMap[intersection.Position] = intersectionChar;
        }
    }

    private static char DetermineIntersectionChar(bool hasNorth, bool hasSouth, bool hasEast, bool hasWest) {
        // Intersección completa
        if (hasNorth && hasSouth && hasEast && hasWest) {
            return CROSS;
        }

        // Intersecciones en T
        if (hasNorth && hasSouth && hasEast) return '╠';
        if (hasNorth && hasSouth && hasWest) return '╣';
        if (hasNorth && hasEast && hasWest) return '╩';
        if (hasSouth && hasEast && hasWest) return '╦';

        // Giros
        if (hasNorth && hasEast) return TURN_NE; // ╚
        if (hasNorth && hasWest) return TURN_NW; // ╝
        if (hasSouth && hasEast) return TURN_SE; // ╔
        if (hasSouth && hasWest) return TURN_SW; // ╗

        // Calles rectas
        if (hasNorth && hasSouth) return CROSS_V;
        if (hasEast && hasWest) return CROSS_H;

        // Caso por defecto para un solo camino o casos no manejados
        return END;
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

        var (width, height) = (_asciiMap.Width, _asciiMap.Height);

        // Crear una copia del mapa para añadir al jugador
        var displayMap = new char[height, width];
        Array.Copy(_asciiMap.Data, displayMap, _asciiMap.Data.Length);

        // Añadir jugador
        displayMap[_playerY, _playerX] = PLAYER;

        // Construir la representación ASCII
        StringBuilder mapOutput = new StringBuilder();

        // Añadir borde superior
        mapOutput.AppendLine("┌" + new string('─', width) + "┐");

        // Añadir filas del mapa
        for (var y = 0; y < height; y++) {
            mapOutput.Append('│');
            for (var x = 0; x < width; x++) {
                mapOutput.Append(displayMap[y, x]);
            }
            mapOutput.AppendLine("│");
        }

        // Añadir borde inferior
        mapOutput.AppendLine("└" + new string('─', width) + "┘");

        // Mostrar en la consola
        Console.Write(mapOutput.ToString());

        // Añadir instrucciones
        Console.WriteLine("Muévete con las teclas de flecha. Pulsa 'Q' para salir.");
        Console.WriteLine($"Seed: {_generator.Seed} Leyenda: @ = Tú | "
                          + $"{ROAD_H}/{ROAD_V} = Calles | {CROSS} = Intersección | {BUILDING} = Edificio");

        var tile = _generator.City.Data[_playerY, _playerX];

        Func<Vector2I, string> t = d => d == Vector2I.Right ? "->" : d == Vector2I.Down ? "v" : d == Vector2I.Left ? "<-" : "^";

        if (tile is Intersection i) {
            Console.WriteLine($"Intersección: {i.Position} | Caminos entrantes: {i.GetInputPaths().Count} {string.Join(",", i.GetInputPaths().Select(p => t(p.Direction)))}| Caminos salientes: {i.GetOutputPaths().Count} {string.Join(",", i.GetOutputPaths().Select(p => t(p.Direction)))}");
        } else {
            Console.WriteLine($"Posición: {_playerX}, {_playerY} | Tipo de tile: {tile?.GetType().Name ?? "N/A"}");
        }
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