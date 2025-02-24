using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.Examples;
using Betauer.Core;
using Betauer.Core.PCG.GridTemplate;
using Betauer.Core.PCG.Maze;
using Betauer.Core.PCG.Maze.Zoned;

namespace Veronenger.Game.Dungeon;

public static class MazeNodeExtension {
    public static bool IsDeadEnd(this MazeNode node) => node.OutEdgesCount == 1;
    public static bool IsCorridor(this MazeNode node) => node.OutEdgesCount == 2;
    public static bool IsFork(this MazeNode node) => node.OutEdgesCount == 3;
    public static bool IsCrossroad(this MazeNode node) => node.OutEdgesCount == 4;
}

public class MazeTemplateDemo {
    const string TemplatePath = "/Users/avilches/Library/Mobile Documents/com~apple~CloudDocs/Shared/Godot/Betauer4/DemoGame/Game/Dungeon/MazeTemplateDemos.txt";

    public static void AddFlags(MazeZones zones) {
        foreach (var node in zones.GetNodes()) {
            var score = zones.GetScore(node);
            
            // if (score.) {
                // node.AddOptionalFlag("corridor");
            // }
        }
    }


    public static void Main() {
        var seed = 1;
        var rng = new Random(seed);
        var zones = MazeGraphCatalog.BigCycle(rng, mc => {
            // mc.OnNodeCreated += (node) => PrintGraph(mc);
        });
        zones.CalculateSolution(MazeGraphCatalog.KeyFormula);

        AddFlags(zones);

        // Crear el gestor de patrones con un tamaño de celda de 5x5
        var templateSet = new TemplateSet(cellSize: 9);

        // Cargar patrones de diferentes archivos
        try {
            var content = File.ReadAllText(TemplatePath);
            templateSet.LoadFromString(content);
            templateSet.ValidateAll(c => c is '#' or '.', true);
            templateSet.ApplyTransformations(c => c is '#' or '.');

            var array2D = zones.MazeGraph.Render((pos, node) => {
                var templates = templateSet.FindTemplates(node.GetDirectionFlags()).ToArray();
                return rng.Next(templates).Body;
            });

            MazeGraphZonedDemo.PrintGraph(zones.MazeGraph, zones);
            PrintArray2D(array2D);
        } catch (FileNotFoundException e) {
            Console.WriteLine(e.Message);
            Console.WriteLine($"{nameof(TemplatePath)} is '{TemplatePath}'");
            Console.WriteLine("Ensure the working directory is the root of the project");
        }
    }

    private static void PrintArray2D(Array2D<char> array2D) {
        for (var y = 0; y < array2D.Height; y++) {
            for (var x = 0; x < array2D.Width; x++) {
                var value = array2D[y, x];
                if (value == '.') value = ' ';
                Console.Write(value);
            }
            Console.WriteLine();
        }
    }
}