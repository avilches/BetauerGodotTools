using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Veronenger.Game.Dungeon.GenCity;

public class Program {
    // Caracteres ASCII para la representación
    private const char EMPTY = ' '; // Espacio vacío
    private const char ROAD_H = '═'; // Carretera horizontal
    private const char ROAD_V = '║'; // Carretera vertical
    private const char CROSS = '╬'; // Intersección
    private const char TURN_NE = '╚'; // Giro noreste
    private const char TURN_NW = '╝'; // Giro noroeste
    private const char TURN_SE = '╔'; // Giro sureste
    private const char TURN_SW = '╗'; // Giro suroeste
    private const char END = '•'; // Fin de carretera
    private const char BUILDING = '#'; // Edificio
    private const char PLAYER = '@'; // Jugador

    private static readonly char[] BUILDING_CHARS = {
        '#', '■', '□', '▣', '▤', '▥', '▦', '▧', '▨', '▩',  // Caracteres de bloque
        // '&', '%', '$', '@', '¤', '¥', '£'                   // Símbolos alternativos
    };


    private static char[,] _asciiMap;
    private static int _playerX;
    private static int _playerY;
    private static bool _running = true;
    private static City _city;

    public static void Main() {
        Console.OutputEncoding = Encoding.UTF8; // Para caracteres especiales

        // Obtener tamaño de la consola o usar valores predeterminados
        int width = 180;
        int height = 24;

        /*
        try {
            width = Console.WindowWidth - 2;
            height = Console.WindowHeight - 3;
        } catch (Exception) {
            // En caso de error, usar los valores predeterminados
        }
        */

        // Asegurar un tamaño mínimo
        width = Math.Max(width, 40);
        height = Math.Max(height, 20);

        // Inicializar mapa ASCII
        _asciiMap = new char[height, width];

        // Generar la ciudad
        GenerateCity(width, height);

        // Inicializar jugador
        InitializePlayer(width, height);

        // Bucle principal del juego
        RenderGame(width, height);
        while (_running) {
            HandleInput(width, height);
            Thread.Sleep(50); // Pequeña pausa para reducir el uso de CPU
        }
    }

    private static void GenerateCity(int width, int height) {
        Console.WriteLine("Generando ciudad roguelike... Por favor espera.");

        // Inicializar mapa
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                _asciiMap[y, x] = EMPTY;
            }
        }

        // Crear y generar la ciudad
        _city = new City(new CityData { Width = width, Height = height });

        var parameters = new CityGenerationParameters {
            StreetMinLength = 6,
            ProbabilityIntersection = 0.32f,  //0.5f, // 0.15f,
            ProbabilityTurn = 0.08f, // 0.08f,
            BuildingMinSize = 2, //2,
            BuildingMaxSize = 5, //3,
            BuildingMinSpace = 1,
            BuildingMaxSpace = 2 // 2
        };

        _city.Generate(parameters);

        // Renderizar elementos de la ciudad
        RenderRoads(width, height);
        RenderNodes(width, height);
        RenderBuildings(width, height);
    }

    private static void RenderBuildings(int width, int height) {
        // Renderizar edificios primero
        int buildingIndex = 0;
        foreach (var building in _city.GetAllBuildings()) {
            char buildingChar = BUILDING_CHARS[buildingIndex % BUILDING_CHARS.Length];
            foreach (var position in building.Each()) {
                var (x, y) = position;
                if (x >= 0 && x < width && y >= 0 && y < height) {
                    _asciiMap[y, x] = buildingChar;
                }
            }
            buildingIndex++;
        }
    }

    private static void RenderRoads(int width, int height) {

        // Renderizar carreteras
        foreach (var path in _city.GetAllPaths()) {
            // Determinar si la carretera es horizontal o vertical
            var isHorizontal = Math.Abs(path.Direction) == 0 || Math.Abs(path.Direction) == 180;
            var roadChar = isHorizontal ? ROAD_H : ROAD_V;

            foreach (var position in path.Each()) {
                var (x, y) = position;
                if (x >= 0 && x < width && y >= 0 && y < height) {
                    _asciiMap[y, x] = roadChar;
                }
            }
        }
    }

    private static void RenderNodes(int width, int height) {

        // Renderizar nodos
        foreach (var node in _city.GetAllNodes()) {
            var (x, y) = node.Position;

            if (x < 0 || x >= width || y < 0 || y >= height)
                continue;

            // Obtener todos los caminos conectados al nodo
            var inputPaths = node.GetInputPaths();
            var outputPaths = node.GetOutputPaths();

            bool hasNorth = false, hasSouth = false, hasEast = false, hasWest = false;

            // Procesar caminos de entrada
            foreach (var dir in inputPaths.Select(path => (path.Direction + 180) % 360)) {
                UpdateDirectionFlags(dir, ref hasNorth, ref hasSouth, ref hasEast, ref hasWest);
            }

            // Procesar caminos de salida
            foreach (var dir in outputPaths.Select(path => path.Direction)) {
                UpdateDirectionFlags(dir, ref hasNorth, ref hasSouth, ref hasEast, ref hasWest);
            }

            // Determinar el tipo de nodo y asignar el carácter apropiado
            char nodeChar;

            if (inputPaths.Count == 0 && outputPaths.Count == 0) {
                nodeChar = END;
            }
            else if (inputPaths.Count + outputPaths.Count == 1) {
                nodeChar = END;
            }
            else {
                nodeChar = DetermineNodeChar(hasNorth, hasSouth, hasEast, hasWest);
            }

            _asciiMap[y, x] = nodeChar;
        }

    }

    private static void UpdateDirectionFlags(int direction, ref bool hasNorth, ref bool hasSouth,
        ref bool hasEast, ref bool hasWest) {
        switch (direction) {
            case 0: hasEast = true; break;
            case 90: hasSouth = true; break;
            case 180: hasWest = true; break;
            case 270: hasNorth = true; break;
        }
    }

    private static char DetermineNodeChar(bool hasNorth, bool hasSouth, bool hasEast, bool hasWest) {
        int connectionCount = (hasNorth ? 1 : 0) + (hasSouth ? 1 : 0) +
                              (hasEast ? 1 : 0) + (hasWest ? 1 : 0);

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
        if (hasNorth && hasSouth) return ROAD_V;
        if (hasEast && hasWest) return ROAD_H;

        // Caso por defecto para un solo camino o casos no manejados
        return END;
    }

    private static void InitializePlayer(int width, int height) {
        // Colocar al jugador en una carretera disponible
        foreach (var node in _city.GetAllNodes()) {
            _playerX = (int)node.Position.X;
            _playerY = (int)node.Position.Y;

            // Verificar que esté dentro de los límites
            if (_playerX >= 0 && _playerX < width && _playerY >= 0 && _playerY < height) {
                break; // Posición válida encontrada
            }
        }
    }

    private static void RenderGame(int width, int height) {
        Console.Clear();

        // Crear una copia del mapa para añadir al jugador
        char[,] displayMap = new char[height, width];
        Array.Copy(_asciiMap, displayMap, _asciiMap.Length);

        // Añadir jugador
        displayMap[_playerY, _playerX] = PLAYER;

        // Construir la representación ASCII
        StringBuilder mapOutput = new StringBuilder();

        // Añadir borde superior
        mapOutput.AppendLine("┌" + new string('─', width) + "┐");

        // Añadir filas del mapa
        for (int y = 0; y < height; y++) {
            mapOutput.Append("│");
            for (int x = 0; x < width; x++) {
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
    }

    private static void HandleInput(int width, int height) {
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
                    GenerateCity(width, height);
                    InitializePlayer(width, height);
                    RenderGame(width, height);
                    return;
            }

            // Comprobar si el movimiento es válido (dentro de límites y no es un edificio)
            if (newX >= 0 && newX < width && newY >= 0 && newY < height
                && _asciiMap[newY, newX] != BUILDING) {
                _playerX = newX;
                _playerY = newY;
            }
            RenderGame(width, height);
        }
    }
}