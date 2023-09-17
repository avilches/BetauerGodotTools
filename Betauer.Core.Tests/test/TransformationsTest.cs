using System.IO;
using Betauer.Core.Image;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;


[Betauer.TestRunner.Test]
[Only]
public class TransformationsTests {
    
    [Betauer.TestRunner.Test]
    public void TransformationsRectTests() {
        var rect = new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        };
        
        AreEqual(rect.FlipV(), new[,] {
            { 20, 21, 22, 23 },
            { 10, 11, 12, 13 },
            {  0,  1,  2,  3 }
        });

        AreEqual(rect.FlipH(), new[,] {
            {  3,  2,  1,  0 },
            { 13, 12, 11, 10 },
            { 23, 22, 21, 20 }
        });
        
        AreEqual(rect.Rotate90(), new[,] {
            { 20, 10, 0 },
            { 21, 11, 1 },
            { 22, 12, 2 },
            { 23, 13, 3 }
        });
        
        AreEqual(rect.RotateMinus90(), new[,] {
            { 3, 13, 23 },
            { 2, 12, 22 },
            { 1, 11, 21 },
            { 0, 10, 20 }
        });
        
        AreEqual(rect.Rotate180(), new[,] {
            { 23, 22, 21, 20},
            { 13, 12, 11, 10},
            {  3,  2,  1,  0}
        });
        
        AreEqual(rect.FlipDiagonal(), new[,] {
            { 0, 10, 20 },
            { 1, 11, 21 },
            { 2, 12, 22 },
            { 3, 13, 23 }
        });

        AreEqual(rect.FlipDiagonalSecondary(), new[,] {
            { 23, 13, 3 },
            { 22, 12, 2 },
            { 21, 11, 1 },
            { 20, 10, 0 }
        });

    }
    [Betauer.TestRunner.Test]
    public void TransformationsImageTests() {
        var rect = new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 },
            { 30, 31, 32, 33 },
        };

        AreEqual(rect, GetData(ImageFrom(rect)));

        var flipV = ImageFrom(rect);
        flipV.FlipV(1, 1, 2, 2);
        AreEqual(GetData(flipV), new[,] {
            {  0,  1,  2,  3 },
            { 10, 21, 22, 13 },
            { 20, 11, 12, 23 },
            { 30, 31, 32, 33 }
        });

        var flipH = ImageFrom(rect);
        flipH.FlipH(1, 1, 2, 2);
        AreEqual(GetData(flipH), new[,] {
            {  0,  1,  2,  3 },
            { 10, 12, 11, 13 },
            { 20, 22, 21, 23 },
            { 30, 31, 32, 33 }
        });
            
        var rotate90 = ImageFrom(rect);
        rotate90.Rotate90(1, 1, 2, 2);
        AreEqual(GetData(rotate90), new[,] {
            {  0,  1,  2,  3 },
            { 10, 21, 11, 13 },
            { 20, 22, 12, 23 },
            { 30, 31, 32, 33 }
        });
            
        var rotateMinus90 = ImageFrom(rect);
        rotateMinus90.RotateMinus90(1, 1, 2, 2);
        AreEqual(GetData(rotateMinus90), new[,] {
            {  0,  1,  2,  3 },
            { 10, 12, 22, 13 },
            { 20, 11, 21, 23 },
            { 30, 31, 32, 33 }
        });
            
        var rotate180 = ImageFrom(rect);
        rotate180.Rotate180(1, 1, 2, 2);
        AreEqual(GetData(rotate180), new[,] {
            {  0,  1,  2,  3 },
            { 10, 22, 21, 13 },
            { 20, 12, 11, 23 },
            { 30, 31, 32, 33 }
        });
        
        var diagonal = ImageFrom(rect);
        diagonal.FlipDiagonal(1, 1, 2, 2);
        AreEqual(GetData(diagonal), new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 21, 13 },
            { 20, 12, 22, 23 },
            { 30, 31, 32, 33 }
        });
        
        var diagonalSec = ImageFrom(rect);
        diagonalSec.FlipDiagonalSecondary(1, 1, 2, 2);
        AreEqual(GetData(diagonalSec), new[,] {
            {  0,  1,  2,  3 },
            { 10, 22, 12, 13 },
            { 20, 21, 11, 23 },
            { 30, 31, 32, 33 }
        });
    }
    
    [Betauer.TestRunner.SetUpClass]
    public void SetUp() {
        if (!Directory.Exists(".tmp")) {
            Directory.CreateDirectory(".tmp");
        }
    }
    
    [Betauer.TestRunner.Test]
    public void GetSetRegionTest() {
        var textures = new FastImage("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        var region = textures.GetRegion(90, 10, 180, 340);
        textures.SetRegion(region, 120, 120);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-get-set-region.png");
    }

    [Betauer.TestRunner.Test]
    public void Rotate90Test() {
        var textures = new FastImage("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.Rotate90(90, 10, 180, 340);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-rotate-90.png");
    }

    [Betauer.TestRunner.Test]
    public void RotateMinus90Test() {
        var textures = new FastImage("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.RotateMinus90(90, 10, 180, 340);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-rotate-minus-90.png");
    }

    [Betauer.TestRunner.Test]
    public void Rotate180Test() {
        var textures = new FastImage("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.Rotate180(90, 10, 180, 340);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-rotate-180.png");
    }

    [Betauer.TestRunner.Test]
    public void RotateFlipHTest() {
        var textures = new FastImage("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.FlipH(90, 10, 180, 340);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-fliph.png");
    }

    [Betauer.TestRunner.Test]
    public void RotateFlipVTest() {
        var textures = new FastImage("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.FlipV(90, 10, 180, 340);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-flipv.png");
    }

    [Betauer.TestRunner.Test]
    public void RotateFlipDiagonalTest() {
        var textures = new FastImage("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.FlipDiagonal(90, 50, 300, 260);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-FlipDiagonal.png");
    }

    [Betauer.TestRunner.Test]
    public void RotateFlipDiagonalSecondaryTest() {
        var textures = new FastImage("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.FlipDiagonalSecondary(90, 50, 300, 260);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-FlipDiagonalSecondary.png");
    }

    private static FastImage ImageFrom(int[,] data) {
        var fastImage = new FastImage(data.GetLength(1), data.GetLength(0), false, Godot.Image.Format.Rgba8);
        for (var y = 0; y < data.GetLength(0); y++) {
            for (var x = 0; x < data.GetLength(1); x++) {
                var color = new Color() {
                    R8 = (byte)data[y, x],
                    A = 1
                };
                fastImage.SetPixel(x, y, color);
            }
        }
        return fastImage;
    }

    private static int[,] GetData(FastImage image) {
        var data = new int[image.Height, image.Width];
        for (var y = 0; y < data.GetLength(0); y++) {
            for (var x = 0; x < data.GetLength(1); x++) {
                var value = 1f/data[y, x];
                data[y,x] = image.GetPixel(x, y).R8;
            }
        }
        return data;
    }

    public static bool AreEqual<T>(T[,] array1, T[,] array2) {
        Assert.AreEqual(array1.GetLength(0), array2.GetLength(0));
        Assert.AreEqual(array1.GetLength(1), array2.GetLength(1));
        for (var i = 0; i < array1.GetLength(0); i++) {
            for (var j = 0; j < array1.GetLength(1); j++) {
                Assert.That(array1[i, j], Is.EqualTo(array2[i, j]), "Element position: [" + i + "," + j + "]");
            }
        }

        return true;
    }
}