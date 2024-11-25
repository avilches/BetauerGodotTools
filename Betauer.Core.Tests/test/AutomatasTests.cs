using System;
using Betauer.Core.DataMath;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class AutomatasTests {
    [Test]
    public void GameOfLifeTest() {
        var data = Array2D.ParseAsBool("""
                                       ············
                                       ······#·····
                                       ·····##·····
                                       ······##····
                                       ············
                                       """, '#');
        var gol = Automatas.CreateGameOfLife(data);
        gol.Update();
        gol.Update();
        gol.Update();
        gol.Update();
        var result = data.GetString((v) => v ? "#" : "·");

        Assert.That(result, Is.EqualTo("""
                                       ············
                                       ·····##·····
                                       ····#··#····
                                       ····#··#····
                                       ·····#·#····
                                       """));
    }

    [Test]
    public void DeadEndTest() {
        var data = Array2D.ParseAsBool("""
                                       ···###···
                                       ····#·#··
                                       ····##···
                                       ·#####···
                                       ··######·    
                                       ·· ###···    
                                       ···#··###    
                                       #··#··#·#    
                                       ·· #··###    
                                       """, '#');
        
        Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
        Automatas.RemoveAllDeadEnds(data);

        var result = data.GetString((v) => v ? "#" : "·");

        Console.WriteLine(result);

        Assert.That(result, Is.EqualTo(
            """
            ·········
            ·········
            ····##···
            ··####···
            ··####···
            ···###···
            ······###
            ······#·#
            ······###
            """));
    }

    [Test]
    public void SmoothCornersTest() {
        var data = new Array2D<bool>(41, 21);
        data.Fill(false);

        var region = ShapeGenerator.GenerateOverlappingSquares(
            width: data.Width,
            height: data.Height,
            numSquares: 10,
            minSize: 4,
            maxSize: 6
        );
        data.Data = region;
        var other = data.Clone();

        Console.WriteLine("Shapes:");
        var result = data.GetString((v) => v ? "#" : "·");
        Console.WriteLine(result);

        for (int i = 0; i < 1; i++) {
            region = ShapeGenerator.SmoothRegion(
                region,
                minNeighborsToKeep: 5,
                minNeighborsToAdd: 6
            );
            data.Data = region;
        }
        result = data.GetString((v) => v ? "#" : "·");
        Console.WriteLine("3 iterations smooth");
        Console.WriteLine(result);

        var s = Automatas.CreateSmoothCorners(other);
        s.Update(1,1,data.Width-2,data.Height-2);
        // s.Update(1,1,data.Width-2,data.Height-2);
        // s.Update();
        var result2 = other.GetString((v) => v ? "#" : "·");
        Console.WriteLine("3 iterations automata");
        Console.WriteLine(result2);
        
        Assert.That(result, Is.EqualTo(result2));
    }
}

public class ShapeGenerator {
    private static Random random = new Random(1);

    // Generador de formas básicas
    public static bool[,] GenerateOverlappingSquares(
        int height,
        int width,
        int numSquares,
        int minSize = 5,
        int maxSize = 15) {
        var region = new bool[height, width];

        for (int i = 0; i < numSquares; i++) {
            // Generar cuadrado aleatorio
            int size = random.Next(minSize, maxSize + 1);
            int y = random.Next(height - size);
            int x = random.Next(width - size);

            // Añadir todos los píxeles del cuadrado
            for (int dy = 0; dy < size; dy++) {
                for (int dx = 0; dx < size; dx++) {
                    region[y + dy, x + dx] = true;
                }
            }
        }

        return region;
    }

    // Suavizado de regiones
    public static bool[,] SmoothRegion(
        bool[,] input,
        int minNeighborsToKeep = 5,
        int minNeighborsToAdd = 6) {
        int height = input.GetLength(0);
        int width = input.GetLength(1);
        var result = new bool[height, width];

        // Copiar región original
        Array.Copy(input, result, input.Length);

        // Crear buffer temporal para no modificar mientras leemos
        var temp = new bool[height, width];
        Array.Copy(result, temp, result.Length);

        // Primera fase: eliminar píxeles con pocos vecinos
        for (int y = 1; y < height - 1; y++) {
            for (int x = 1; x < width - 1; x++) {
                if (result[y, x]) {
                    int neighbors = CountNeighbors(result, y, x);
                    temp[y, x] = neighbors >= minNeighborsToKeep;
                }
            }
        }

        // Segunda fase: añadir píxeles en huecos
        Array.Copy(temp, result, temp.Length);
        for (int y = 1; y < height - 1; y++) {
            for (int x = 1; x < width - 1; x++) {
                if (!result[y, x]) {
                    int neighbors = CountNeighbors(result, y, x);
                    temp[y, x] = neighbors >= minNeighborsToAdd;
                }
            }
        }

        return temp;
    }

    // Método auxiliar para contar vecinos
    private static int CountNeighbors(bool[,] region, int y, int x) {
        int count = 0;
        for (int dy = -1; dy <= 1; dy++) {
            for (int dx = -1; dx <= 1; dx++) {
                if (dx == 0 && dy == 0) continue;
                if (region[y + dy, x + dx]) {
                    count++;
                }
            }
        }
        return count;
    }
}