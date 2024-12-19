using Betauer.Core.PCG.GridTools;

namespace Betauer.Core.PCG.Examples;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Betauer.Core.DataMath;
using Godot;

public class FovDemo {
    public static void Main() {
        // Varios escenarios de prueba
        var scenarios = new[] {
            @"
### Empty room with central light ###
#########
#.......#
#..o....#
#.......#
#########",
            @"
### Empty room with central light ###
###############
#.............#
#.............#
#......o......#
#.............#
#.............#
###############",
            @"
### Empty room with central light ###
############################
#..........................#
#..........................#
#..........................#
#..........................#
#..........................#
#..........................#
#..........................#
#..........................#
#............o.............#
#..........................#
#..........................#
#..........................#
#..........................#
#..........................#
#..........................#
#..........................#
#..........................#
#..........................#
############################",
            @"
### Room with columns ###
###########
#....#....#
#.o..#....#
#....#....#
#....#....#
###########",
            @"
### Corridor with multiple lights ###
#############
#.o.#.o.#.o.#
#...#...#...#
#############",
            @"
### Complex room ###
###############
#..#.......#..#
#.o..#####...o#
#...#.....#...#
#.....o.......#
#...#.....#...#
#..#.......#..#
###############",
            @"
### Diagonal walls ###
##########
#o....#..#
#.#...#..#
#..#..#..#
#...#.#..#
#....#.o.#
##########"
        };

        // Procesar cada escenario
        foreach (var scenario in scenarios) {
            ProcessScenario(scenario);
        }
    }


    private static void ProcessScenario(string scenario) {
        // Separar el título del mapa
        var parts = scenario.Trim().Split('\n');
        var title = parts[0].Trim('#', ' ');
        var mapString = string.Join('\n', parts.Skip(1));

        Console.WriteLine($"=== {title} ===\n");

        // Crear el mapa usando Array2D.Parse
        var map = Array2D.Parse(mapString);
        // Crear el graph y FOV
        var graph = new Array2DGraph<char>(map, null, pos => map[pos] != '#');
        var fov = new FoV<char>(graph, pos => map[pos] == '#');

        // Encontrar todas las fuentes de luz
        var lightSources = new List<Vector2I>();
        for (var y = 0; y < map.Height; y++) {
            for (var x = 0; x < map.Width; x++) {
                if (map[y, x] == 'o') {
                    lightSources.Add(new Vector2I(x, y));
                }
            }
        }

        // Calcular y mostrar FOV para cada fuente por separado
        for (var i = 0; i < lightSources.Count; i++) {
            var source = lightSources[i];
            var visibleCells = fov.ComputeFov(source, radius: 8, lightWalls: true);
            Console.WriteLine($"FOV desde fuente {i + 1} ({source.X}, {source.Y}):");
            PrintFov(map, visibleCells);
            Console.WriteLine();
        }

        // Si hay múltiples fuentes, mostrar FOV combinado
        if (lightSources.Count > 1) {
            Console.WriteLine("FOV combinado de todas las fuentes:");
            fov.ClearFov();
            foreach (var source in lightSources) {
                fov.AppendFov(source, radius: 5, lightWalls: true);
            }
            PrintFov(map, fov.Fov);
        }
    }

    private static void PrintMap(Array2D<char> map) {
        for (var y = 0; y < map.Height; y++) {
            for (var x = 0; x < map.Width; x++) {
                Console.Write(map[y, x]);
            }
            Console.WriteLine();
        }
    }

    private static void PrintFov(Array2D<char> map, IReadOnlySet<Vector2I> visibleCells) {
        for (var y = 0; y < map.Height; y++) {
            for (var x = 0; x < map.Width; x++) {
                var pos = new Vector2I(x, y);
                var baseChar = map[y, x];
            
                // Set color based on visibility
                Console.ForegroundColor = visibleCells.Contains(pos) 
                    ? ConsoleColor.White 
                    : ConsoleColor.DarkGray;
            
                // Print character (keep # and . as is)
                Console.Write(baseChar == 'o' ? 'o' : baseChar);
            }
            Console.WriteLine();
        }
        // Reset color
        Console.ResetColor();
    }}