using System.IO;
using Betauer.Core.Image;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class ImageTransformationsTests {
    
    [OneTimeSetUp]
    public void SetUp() {
        if (!Directory.Exists(".tmp")) {
            Directory.CreateDirectory(".tmp");
        }
    }

    [Test]
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

        var mirrorTToB = ImageFrom(rect);
        mirrorTToB.MirrorTopToBottom(1, 1, 3, 2);
        AreEqual(GetData(mirrorTToB), new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 11, 12, 13 },
            { 30, 31, 32, 33 }
        });

        var mirrorBToT = ImageFrom(rect);
        mirrorBToT.MirrorBottomToTop(1, 1, 3, 2);
        AreEqual(GetData(mirrorBToT), new[,] {
            {  0,  1,  2,  3 },
            { 10, 21, 22, 23 },
            { 20, 21, 22, 23 },
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
            
        var mirrorRToL = ImageFrom(rect);
        mirrorRToL.MirrorRightToLeft(1, 1, 3, 2);
        AreEqual(GetData(mirrorRToL), new[,] {
            {  0,  1,  2,  3 },
            { 10, 13, 12, 13 },
            { 20, 23, 22, 23 },
            { 30, 31, 32, 33 }
        });
            
        var mirrorLToR = ImageFrom(rect);
        mirrorLToR.MirrorLeftToRight(1, 1, 3, 2);
        AreEqual(GetData(mirrorLToR), new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 11 },
            { 20, 21, 22, 21 },
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
    
    [Test]
    public void GetSetRegionTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        var region = textures.GetRegion(90, 10, 180, 340);
        textures.SetRegion(region, 120, 120);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-get-set-region.png");
    }

    [Test]
    public void Rotate90Test() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.Rotate90(90, 10, 180, 340);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-Rotate90.png");
    }

    [Test]
    public void RotateMinus90Test() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.RotateMinus90(90, 10, 180, 340);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-RotateMinus90.png");
    }

    [Test]
    public void Rotate180Test() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.Rotate180(90, 10, 180, 340);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-Rotate180.png");
    }

    [Test]
    public void FlipHTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.FlipH(90, 10, 180, 340);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-FlipH.png");
    }

    [Test]
    public void FlipHOddTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.FlipH(90, 10, 181, 341);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-FlipH-odd.png");
    }

    [Test]
    public void MirrorLeftToRightTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.MirrorLeftToRight(90, 10, 180, 340);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-MirrorLeftToRight.png");
    }

    [Test]
    public void MirrorLeftToRightOddTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.MirrorLeftToRight(90, 10, 181, 341);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-MirrorLeftToRight-odd.png");
    }

    [Test]
    public void MirrorRightToLeftTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.MirrorRightToLeft(90, 10, 180, 340);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-MirrorRightToLeft.png");
    }

    [Test]
    public void MirrorRightToLeftOddTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.MirrorRightToLeft(90, 10, 181, 341);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-MirrorRightToLeft-odd.png");
    }

    [Test]
    public void FlipVTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.FlipV(90, 10, 180, 340);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-FlipV.png");
    }

    [Test]
    public void FlipVOddTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.FlipV(90, 10, 181, 341);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-FlipV-odd.png");
    }

    [Test]
    public void MirrorTopToBottomTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.MirrorTopToBottom(90, 10, 180, 340);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-MirrorTopToBottom.png");
    }

    [Test]
    public void MirrorTopToBottomOddTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.MirrorTopToBottom(90, 10, 181, 341);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-MirrorTopToBottom-odd.png");
    }

    [Test]
    public void MirrorBottomToTopTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.MirrorBottomToTop(90, 10, 180, 340);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-MirrorBottomToTop.png");
    }

    [Test]
    public void MirrorBottomToTopOddTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.MirrorBottomToTop(90, 10, 181, 341);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-MirrorBottomToTop-odd.png");
    }

    [Test]
    public void FlipDiagonalTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.FlipDiagonal(90, 50, 300, 260);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-FlipDiagonal.png");
    }

    [Test]
    public void FlipDiagonalSecondaryTest() {
        var textures = new FastImage().LoadResource("test-resources/imagen2.png", Godot.Image.Format.Rgba8);
        textures.FlipDiagonalSecondary(90, 50, 300, 260);
        textures.Flush();
        textures.Image.SavePng(".tmp/imagen-FlipDiagonalSecondary.png");
    }

    private static FastImage ImageFrom(int[,] data) {
        var fastImage = new FastImage().Create(data.GetLength(1), data.GetLength(0), false, Godot.Image.Format.Rgba8);
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