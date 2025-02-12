using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.FoV;
using Betauer.Core.PCG.GridTools;
using Godot;

namespace Betauer.Core.Examples;

public class FovDemo {
    public static void Main() {
        // Varios escenarios de prueba
        var scenarios = new[] {
            @"
### Multiple lights ###
#####################################   #
#..........................##.......#   #
#..........................##..@....#   #
#..........................##.......#   #
#..........................##########   #
#..........................##############
#..........................##.o.#.o.#.o.#
#..........................##...#...#...#
#..........................##############
#............@..........................#
#.......................................#
#.......................................#
#.......................................#
#.............................o.........#
#.......................................#
#.......................................#
#.......................................#
#.......................................#
#.......................................#
###########################             #",
            @"
### Corner peeking 40 ###
ooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
Player only can peek 7 cells        Symmetric: the monster only can
in each direction and see M      see the player in the same 7 tiles
ooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
###################################################################
──3210123───────────────────┐┌──────3210123──∙└─────3210123────────
∙∙∙∙∙∙∙∙M∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙││∙∙∙∙∙o∙∙∙∙∙∙∙∙∙#∙#∙∙∙∙o∙∙∙∙∙∙∙∙∙∙∙∙∙∙
──┬┬─o┌┬────────────────────┴┴──────┬┬─.┌┬──────────┬┬─.┌┬─────────
∙∙├┘∙∙└┤∙∙∙########################∙∙├┘∙∙└┤∙∙∙#######├┘∙∙└┤########
∙┌┘∙∙│∙└┐∙∙########################∙┌┘∙∙│∙└┐∙∙######┌┘∙∙│∙└┐#######  
###################################################################",
@"
### Rooms 40 ###
──────────────┐┌───────────────────┐    ┌───────────┐ ┌───────────┐────────┬#─────┬┌───────────────────┐∙∙∙∙∙∙∙∙∙∙∙┌──────────
∙∙∙∙∙∙∙∙∙∙∙∙∙∙││∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙││   │∙∙∙∙∙∙∙∙∙∙∙│ │∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙│#∙∙∙∙∙││∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙
∙∙┼∙∙∙∙∙∙∙∙∙∙∙││∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙││  ┌┘∙──┬─────┐∙│┌┘∙──┬─────┐∙│∙∙∙∙∙∙∙∙│#∙∙∙∙∙││∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙
∙∙∙∙∙∙∙∙∙∙∙∙∙∙││∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙│└─┬┤∙∙∙∙│     │∙│┤∙∙∙∙│     │∙│∙∙∙┼∙│∙∙│#┼∙│∙∙││∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙
∙∙┼∙∙∙∙∙∙∙∙∙∙∙││∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙│∙∙└┘∙∙∙∙│     │∙│┘∙∙∙∙│     │∙│∙∙∙∙∙│∙∙│#∙∙│∙∙││∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙
∙∙∙∙∙∙∙∙∙∙∙∙∙∙│┤∙∙∙∙∙∙∙┼∙∙∙┼∙∙∙∙∙∙∙│┐∙∙∙∙∙∙∙└───┐ │∙│@∙∙∙∙└───┐ │∙│∙∙∙∙∙∙∙∙└#∙∙∙∙∙└┤∙∙∙∙∙∙∙┼∙∙∙┼∙∙∙∙∙∙∙│@+∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙
∙∙┼∙∙∙∙∙∙∙∙∙∙∙├#∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙├└──┐∙∙∙∙∙∙∙∙└─┘∙│┐∙∙∙∙∙∙∙∙└─┘∙│@∙∙∙│∙∙∙∙#@│∙∙∙∙│∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙├∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙
@∙∙∙∙∙∙∙∙∙∙∙∙∙┼┤∙∙∙∙∙∙∙┼∙@∙┼∙∙∙∙∙∙∙#∙∙∙│∙∙∙∙┌─┐∙∙∙∙@││∙∙∙∙┌─┐∙∙∙∙∙│∙∙∙∙∙∙∙∙∙#∙∙∙∙∙∙┤∙∙∙∙∙∙∙┼∙@∙┼∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙
∙∙┼∙∙∙∙∙∙∙∙∙∙∙├│∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙├∙∙∙│∙∙∙∙│ └─────┘│∙∙∙∙│ └─────┘∙∙∙∙∙∙∙∙∙#∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙├∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙
∙∙∙∙∙∙∙∙∙∙∙∙∙∙││∙∙∙∙∙∙∙┼∙∙∙┼∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙#┌───────┌───────┌────────             │∙∙∙∙∙∙∙┼∙∙∙┼∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙┼∙
∙∙┼∙∙∙∙∙∙∙∙∙∙∙││∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙#│∙∙∙∙∙∙∙│∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙             │∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙∙@
∙∙∙∙∙∙∙∙∙∙∙∙∙∙││∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙∙┼#│∙∙∙∙∙∙∙│∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙             │∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙│──────────────────────
∙∙┼∙∙∙∙∙∙∙∙∙∙∙││∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙∙┼∙#│∙∙∙∙∙∙∙│∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙             │∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙│###│∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙###
∙∙∙∙∙∙∙∙∙∙∙∙∙∙││∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙│∙∙∙∙∙┼∙∙#│∙∙∙∙∙∙∙│∙∙∙∙∙∙∙│∙∙∙∙∙∙∙∙             │∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙│###│∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙###
────────┼─────┤┴─────────┼─────────┴∙∙∙∙┼∙∙∙#│∙∙∙∙∙∙∙│∙∙∙∙∙∙∙│∙∙∙∙∙∙∙┼             ┴───────────────────┴───┤∙∙∙∙∙∙∙┼∙∙∙┼∙∙∙###
#                                   ∙∙∙┼∙∙∙∙#│∙∙∙∙∙∙∙│∙∙∙∙∙∙┼│∙∙∙∙∙∙┼∙                                  @∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙###
#                                   ∙∙┼∙∙∙∙∙#│∙∙∙∙∙∙┼│∙∙∙∙∙┼∙│∙∙∙∙∙┼∙∙                                  ───┤∙∙∙∙∙∙∙┼∙∙∙┼∙∙∙###
#                                   @┼∙∙∙∙∙∙#│∙∙∙∙∙┼@│∙∙∙∙┼∙@│∙∙∙∙┼∙∙@                                  ###│∙∙∙∙∙∙∙∙∙∙∙∙∙∙∙###
#                                                                                                       ###│∙∙∙∙∙∙∙┼∙∙∙┼∙∙∙###
",
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

        var radius = 8;
        if (!int.TryParse(title.Split(" ")[^1], out radius)) {
            radius = 8;
        }

        Console.WriteLine($"=== {title} ===");

        // Crear el mapa usando Array2D.Parse
        var map = Array2D.Parse(mapString);
        var graph = new GridGraph(map.Width, map.Height, pos => map[pos] != '.' && map[pos] != '\u2219');

        var lightSources = new List<Vector2I>();
        for (var y = 0; y < map.Height; y++) {
            for (var x = 0; x < map.Width; x++) {
                if (map[y, x] == 'o' || map[y, x] == '@') {
                    lightSources.Add(new Vector2I(x, y));
                }
            }
        }

        // for (var y = 0; y < map.Height; y++) {
        //     for (var x = 0; x < map.Width; x++) {
        //         Console.Write(graph.IsWalkablePosition(new Vector2I(x, y)) ? '.' : '#');
        //     }
        //     Console.WriteLine();
        // }

        foreach (var alg in Enum.GetValues<FovAlgorithm>()) {
            var fov = new FieldOfView(alg, graph.IsBlocked);
            Console.WriteLine("Alg: " + alg);
            foreach (var source in lightSources) {
                fov.AppendFov(source, radius);
            }
            PrintFov(map, fov.Fov);
            Console.WriteLine();
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
    }
}