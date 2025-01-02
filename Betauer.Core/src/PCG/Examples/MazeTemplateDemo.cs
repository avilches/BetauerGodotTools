using System;
using System.IO;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.GridTemplate;
using Betauer.Core.PCG.Maze;

namespace Betauer.Core.PCG.Examples;

public class MazeTemplateDemo {
    const string TemplatePath = "Betauer.Core/src/PCG/Examples/MazeTemplateDemos.txt";
    public static void Main() {
        var seed = 1;
        var rng = new Random(seed);
        var zones = MazeGraphCatalog.BigCycle(rng, mc => {
            // mc.OnNodeCreated += (node) => PrintGraph(mc);
        });
        zones.CalculateSolution(MazeGraphCatalog.KeyFormula);

        // Crear el gestor de patrones con un tamaño de celda de 5x5
         var templateSet = new TemplateSet(cellSize: 7);

        // Cargar patrones de diferentes archivos
        try {
            var content = File.ReadAllText(TemplatePath);
            templateSet.LoadTemplates(content);

            // El resto del código permanece igual

            var array2D = zones.MazeGraph.Render(TemplateSelector.Create(templateSet));
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
                Console.Write(array2D[y, x]);
            }
            Console.WriteLine();
        }
    }
}