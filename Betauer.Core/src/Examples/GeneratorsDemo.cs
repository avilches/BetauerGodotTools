using System;
using Betauer.Core.DataMath;
using Betauer.Core.PCG;
using Betauer.Core.PCG.GridTools;
using Godot;

namespace Betauer.Core.Examples;

public class GeneratorsDemo {
    public static void Main() {
        var data = new Array2D<bool>(50, 31, false);

        /*
        data.Fill(false);
        Generators.EdenGrowth(data, new Vector2I(data.Width / 2, data.Height / 2), 164, 0.7f, new Random(2));
        Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
        
        data.Fill(false);
        Generators.PercolationCluster(data, new Vector2I(data.Width / 2, data.Height / 2), 164, 0.3f, 0.5f, new Random(2));
        Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
        
        data.Fill(false);
        Generators.DiffusionLimitedAggregation(data, new Vector2I(data.Width / 2, data.Height / 2),164, 1, 1.5d, new Random(1));
        Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
        */
        
        // data.Fill(false);
        // Generators.Metaball(data, new Vector2I(data.Width/2, data.Height/2), 5, 3, 13D, 10, 10, new Random(1));

        /*for (var i = 10; i < 1000; i++) {
            data.Fill(false);
            Generators.EdenGrowth(data, new Vector2I(data.Width / 2, data.Height / 2), i, 0.7f, new Random(2));
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
            Automatas.Border4Detector(data);
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
            Automatas.ConnectBorderDiagonals(data);
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
            Automatas.RemoveAllDeadEnds(data);
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
            // Generators.PercolationCluster(data, new Vector2I(data.Width / 2, data.Height / 2), i, 0.3d, 0.5d, new Random(1));
            // Generators2.DiffusionLimitedAggregation(data, new Vector2I(data.Width / 2, data.Height / 2),i, 1, 1.5d, new Random(1));
            // Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
        }*/

        int size = 130;
        for (var j = 0; j < 1000; j++) {
            data.Fill(false);
            
            Generators.EdenGrowth(data, new Vector2I(data.Width / 2, data.Height / 2), 130, 0.7f, new Random(j));
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
            
            DeadEndRemover.Create(data).RemoveAll();
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));

            var outer = OuterBorderDetector.Create(data).DetectBorders();
            Console.WriteLine(outer.GetString());

            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(outer.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(outer.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(outer.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(outer.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));

            
            var inner = InnerBorderDetector.Create(data).DetectBorders();
            Console.WriteLine(inner.GetString((v) => v ? "#" : "·"));

            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(inner.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(inner.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(inner.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(inner.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
            Console.WriteLine(inner.GetString((v) => v ? "#" : "·"));

            

            ConnectDiagonals.ConnectBorderDiagonals(data);
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));

            DeadEndRemover.Create(data).RemoveAll();
            Console.WriteLine(data.GetString((v) => v ? "#" : "·"));

            
            // Generators.PercolationCluster(data, new Vector2I(data.Width / 2, data.Height / 2), i, 0.3d, 0.5d, new Random(1));
            // Generators2.DiffusionLimitedAggregation(data, new Vector2I(data.Width / 2, data.Height / 2),i, 1, 1.5d, new Random(1));
            // Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
        }



        /*
        for (var i = 0; i < 1000; i++) {
            data.Fill(false);
            // Generators.GenerateDLA(data.Data, data.Height / 2, data.Width / 2, i);
            // Generators.GeneratePerlinBlob(data.Data, data.Height / 2, data.Width / 2, i);
            // Generators.GenerateEden(data.Data, data.Height / 2, data.Width / 2, i);
            // Generators.GenerateBiasedRandomWalk(data.Data, data.Height / 2, data.Width / 2, i);
            for (int j = 0; j < 2000; j++) {
                data.Fill(false);
                Generators.PercolationCluster(data, new Vector2I(data.Width / 2, data.Height / 2), i, 0.3f, 0.5f, new Random(j));
                // Generators2.DiffusionLimitedAggregation(data, new Vector2I(data.Width / 2, data.Height / 2),34, 1, 1.5d, new Random(1));
            }
            Console.WriteLine(i);var occupiedCells = new Array2D<bool>(5, 5);
occupiedCells[new Vector2I(0, 0)] = true; // Celda ocupada en la esquina superior izquierda

var generator = new MazeGenerator(occupiedCells);
var maze = generator.Generate();
generator.PrintMaze();
        }
    */
    }
   
}