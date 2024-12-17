using System;
using System.IO;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.GridTemplate;

namespace Betauer.Core.PCG.Examples;

public class MazePatternDemo {
    public static void MainGenerate() {
        var seed = 1;
        var rng = new Random(seed);
        var zones = MazeGraphCatalog.Mini(rng, mc => {
            // mc.OnNodeCreated += (node) => PrintGraph(mc);
        });
        zones.CalculateSolution(MazeGraphCatalog.KeyFormula);

        // Crear el gestor de patrones con un tamaño de celda de 5x5
        var patterns = new TemplateSet(cellSize: 5);

        // Cargar patrones de diferentes archivos
        var content = File.ReadAllText("/Users/avilches/Library/Mobile Documents/com~apple~CloudDocs/Shared/Godot/Betauer4/Betauer.Core/src/PCG/GridTemplate/basic_patterns.txt");
        patterns.LoadTemplates(content);

        // El resto del código permanece igual

        var array2D = MazeGraphToArray2D.Convert(zones.MazeGraph, patterns);
        MazeGraphZonedDemo.PrintGraph(zones.MazeGraph, zones);
        PrintArray2D(array2D);
    }

    private static void PrintArray2D(Array2D<char> array2D) {
        for (var y = 0; y < array2D.Height; y++) {
            for (var x = 0; x < array2D.Width; x++) {
                Console.Write(array2D[y, x]);
            }
            Console.WriteLine();
        }
    }
}