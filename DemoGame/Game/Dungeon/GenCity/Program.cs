using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Betauer.Core.DataMath;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public class Program {
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

    private readonly char[] BUILDING_CHARS = {
        '#', '■', '□', '▣', '▤', '▥', '▦', '▧', '▨', '▩', // Caracteres de bloque
        // '&', '%', '$', '@', '¤', '¥', '£'                   // Símbolos alternativos
    };


    private Array2D<char> _asciiMap;
    private int _playerX;
    private int _playerY;
    private bool _running = true;
    private City _city;

    public static void Main() {
        var program = new Program();
        program.Start(140, 40);
        // program.Validate(60, 20);
    }

    public void Validate(int width, int height) {
        int turns = 0;
        int fork2Count = 0;
        int fork3Count = 0;
        int noPath = 0;

        var options = new CityGenerationOptions {
            StreetMinLength = 6,

            ProbabilityExtend = 0.80f,

            ProbabilityCross = 0.42f,
            ProbabilityFork = 0.42f,
            ProbabilityTurn = 0.12f,

            ProbabilityStreetEnd = 0.001f,
        };

        const int seedStart = 0;
        const int seedOffset = 0;

        _city = new City(width, height);
        _city.Configure(options);

        _asciiMap = new Array2D<char>(width, height);

        for (var i = seedStart; i < 1000; i++) {
            options.Seed = i + seedOffset;
            if (i % 100 == 0) {
                Console.WriteLine(options.Seed);
            }
            try {
                _city.Reset();
                _city.Start();
                _city.Grow();
            } catch (Exception e) {
                Console.WriteLine($"Seed:{options.Seed}");
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public void Start(int width, int height) {
        Console.OutputEncoding = Encoding.UTF8; // Para caracteres especiales
        GenerateCity(width, height, 2);
        InitializePlayer(width, height);
        while (_running) {
            HandleInput(width, height);
            Thread.Sleep(50); // Pequeña pausa para reducir el uso de CPU
        }
    }

    private void GenerateCity(int width, int height, int seed) {
        // Inicializar mapa
        _city = new City(width, height);
        _asciiMap = new Array2D<char>(width, height);

        List<Vector2I> startDirections = [Vector2I.Right, Vector2I.Down, Vector2I.Left, Vector2I.Up];
        startDirections.RemoveAt(seed % 4);

        var options = new CityGenerationOptions {
            Seed = seed,
            StartDirections = startDirections,

            StreetMinLength = 6,

            ProbabilityExtend = 0.80f,

            ProbabilityCross = 0.42f,
            ProbabilityFork = 0.42f,
            ProbabilityTurn = 0.12f,

            ProbabilityStreetEnd = 0.001f,

            BuildingMinSize = 2, //2,
            BuildingMaxSize = 5, //3,
            BuildingMinSpace = 1,
            BuildingMaxSpace = 2, // 2

            OnUpdate = (_) => {
                // Render();
            }
        };

        _city.Configure(options);
        _city.Start();
        _city.Grow();
        Render();
        _city.CreatePath(new Vector2I(0, 16), new Vector2I(width - 1, 16));
        Render();
        _city.CreatePath(new Vector2I(0, 17), new Vector2I(width - 1, 17));
        Render();
        _city.CreatePath(new Vector2I(0, 3), new Vector2I(width - 1, 3));
        Render();
        _city.CreatePath(new Vector2I(0, 2), new Vector2I(width - 1, 2));
        Render();
        _city.CreatePath(new Vector2I(0, 0), new Vector2I(width - 1, 0));
        Render();
        _city.CreatePath(new Vector2I(131, 0), new Vector2I(131, height -1));
        Render();
        _city.CreatePath(new Vector2I(111, 0), new Vector2I(111, height -1));
        Render();

        // _city.GenerateBuildings();
        // Render();

        /*
        var path = _city.GetAllPaths().FirstOrDefault();
        while (path != null) {
            _city.RemovePath(path);
            path = _city.GetAllPaths().FirstOrDefault();
            Render();
        }
    */
    }

    private void Render() {
        ClearRender();
        RenderRoads();
        RenderIntersections();
        // RenderBuildings();
        RenderGame();
    }

    private void ClearRender() {
        _asciiMap.Fill(EMPTY);
    }

    private void RenderBuildings() {
        var buildingIndex = 0;
        foreach (var building in _city.Buildings) {
            var buildingChar = BUILDING_CHARS[buildingIndex % BUILDING_CHARS.Length];
            foreach (var position in building.GetPositions()) {
                _asciiMap[position] = buildingChar;
            }
            buildingIndex++;
        }
    }

    public static bool IsHorizontal(Vector2I direction) {
        return direction.X != 0 && direction.Y == 0;
    }

    private void RenderRoads() {
        foreach (var path in _city.GetAllPaths()) {
            var isHorizontal = IsHorizontal(path.Direction);
            var roadChar = isHorizontal ? ROAD_H : ROAD_V;
            var start = path.Start.Position;
            var end = path.End?.Position ?? path.GetCursor();

            foreach (var position in path.GetPositions()) {
                if (position == start && _city.Data[position] != path.Start) {
                    throw new Exception("Wrong start position");
                }
                if (position == end && path.End == null && _city.Data[position] != path) {
                    throw new Exception("Wrong end position (cursor)");
                }
                if (position == end && path.End != null && _city.Data[position] != path.End) {
                    throw new Exception("Wrong end position");
                }
                if (position != start && position != end && _city.Data[position] != path) {
                    throw new Exception("Wrong path position");
                }
                _asciiMap[position] = roadChar;
            }
        }
    }

    private void RenderIntersections() {
        foreach (var intersection in _city.Intersections) {

            if (_city.Data[intersection.Position] != intersection) {
                throw new Exception("Wrong intersection position");
            }

            var hasNorth = intersection.FindPathTo(Vector2I.Up) != null;
            var hasSouth = intersection.FindPathTo(Vector2I.Down) != null ;
            var hasEast = intersection.FindPathTo(Vector2I.Right) != null ;
            var hasWest = intersection.FindPathTo(Vector2I.Left) != null ;
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

    private void InitializePlayer(int width, int height) {
        // Colocar al jugador en una carretera disponible
        foreach (var intersection in _city.Intersections) {
            _playerX = intersection.Position.X;
            _playerY = intersection.Position.Y;

            // Verificar que esté dentro de los límites
            if (_playerX >= 0 && _playerX < width && _playerY >= 0 && _playerY < height) {
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
        Console.WriteLine("Leyenda: @ = Tú | "
                          + $"{ROAD_H}/{ROAD_V} = Calles | {CROSS} = Intersección | {BUILDING} = Edificio");

        var tile = _city.Data[_playerY, _playerX];

        Func<Vector2I, string> t = d => d == Vector2I.Right ? "->" : d == Vector2I.Down ? "v" : d == Vector2I.Left ? "<-" : "^";

        if (tile is Intersection i) {
            Console.WriteLine($"Intersección: {i.Position} | Caminos entrantes: {i.GetInputPaths().Count} {string.Join(",", i.GetInputPaths().Select(p => t(p.Direction)))}| Caminos salientes: {i.GetOutputPaths().Count} {string.Join(",", i.GetOutputPaths().Select(p => t(p.Direction)))}");
        } else {
            Console.WriteLine($"Posición: {_playerX}, {_playerY} | Tipo de tile: {tile?.GetType().Name ?? "N/A"}");
        }
    }

    private void HandleInput(int width, int height) {
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
                    GenerateCity(width, height, _city.Seed + 1);
                    InitializePlayer(width, height);
                    return;
            }

            // Comprobar si el movimiento es válido (dentro de límites y no es un edificio)
            if (newX >= 0 && newX < width && newY >= 0 && newY < height
                && _asciiMap[newY, newX] != BUILDING) {
                _playerX = newX;
                _playerY = newY;
            }
            RenderGame();
        }
    }
}