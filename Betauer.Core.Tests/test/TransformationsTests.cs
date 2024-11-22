using Betauer.Core.DataMath;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class TransformationsTests {
    
    [Test]
    public void TransformationsRotationsTests() {
        var original = new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        };
        AreEqual(original.Rotate90(), new[,] {
            { 20, 10, 0 },
            { 21, 11, 1 },
            { 22, 12, 2 },
            { 23, 13, 3 }
        });
        
        AreEqual(original.RotateMinus90(), new[,] {
            { 3, 13, 23 },
            { 2, 12, 22 },
            { 1, 11, 21 },
            { 0, 10, 20 }
        });
        
        AreEqual(original.Rotate180(), new[,] {
            { 23, 22, 21, 20},
            { 13, 12, 11, 10},
            {  3,  2,  1,  0}
        });
    }

    [Test]
    public void TransformationsFlipTests() {
        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.FlipH(), new[,] {
            {  3,  2,  1,  0 },
            { 13, 12, 11, 10 },
            { 23, 22, 21, 20 }
        });
        
        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.FlipV(), new[,] {
            { 20, 21, 22, 23 },
            { 10, 11, 12, 13 },
            {  0,  1,  2,  3 }
        });

        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.FlipDiagonal(), new[,] {
            { 0, 10, 20 },
            { 1, 11, 21 },
            { 2, 12, 22 },
            { 3, 13, 23 }
        });

        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.FlipDiagonalSecondary(), new[,] {
            { 23, 13, 3 },
            { 22, 12, 2 },
            { 21, 11, 1 },
            { 20, 10, 0 }
        });
    }

    [Test]
    public void TransformationsMirrorTests() {
        AreEqual(new[,] {
            {  0,  1,  2,  3,  4 },
            { 10, 11, 12, 13, 14 },
            { 20, 21, 22, 23, 15 }
        }.MirrorLeftToRight(), new[,] {
            {  0,  1,  2,  1,  0 },
            { 10, 11, 12, 11, 10 },
            { 20, 21, 22, 21, 20 }
        });
        
        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.MirrorLeftToRight(), new[,] {
            {  0,  1,  1,  0 },
            { 10, 11, 11, 10 },
            { 20, 21, 21, 20 }
        });

        AreEqual(new[,] {
            {  0,  1,  2,  3,  4 },
            { 10, 11, 12, 13, 14 },
            { 20, 21, 22, 23, 15 }
        }.MirrorRightToLeft(), new[,] {
            {  4,  3,  2,  3,  4 },
            { 14, 13, 12, 13, 14 },
            { 15, 23, 22, 23, 15 }
        });
        
        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.MirrorRightToLeft(), new[,] {
            {  3,  2,  2,  3 },
            { 13, 12, 12, 13 },
            { 23, 22, 22, 23 }
        });
        
        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.MirrorTopToBottom(), new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            {  0,  1,  2,  3 }
        });

        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 },
            { 30, 31, 32, 33 }
        }.MirrorTopToBottom(), new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 10, 11, 12, 13 },
            {  0,  1,  2,  3 }
        });

        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 },
            { 30, 31, 32, 33 }
        }.MirrorBottomToTop(), new[,] {
            { 30, 31, 32, 33 },
            { 20, 21, 22, 23 },
            { 20, 21, 22, 23 },
            { 30, 31, 32, 33 }
        });
    }

    [Test]
    public void TransformationsResizeTests() {
        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.Resize(3, 4, -1), new[,] {
            {  0,  1,  2 },
            { 10, 11, 12 },
            { 20, 21, 22 },
            { -1, -1, -1 },
        });
        
        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.Resize(3, 4, -1), new[,] {
            {  0,  1,  2 },
            { 10, 11, 12 },
            { 20, 21, 22 },
            { -1, -1, -1 },
        });
    }

    public static bool AreEqual<T>(T[,] array1, T[,] array2) {
        Assert.AreEqual(array1.GetLength(0), array2.GetLength(0),
            $"Array dimension [{array1.GetLength(0)},{array1.GetLength(1)}] wrong. Expected: [{array2.GetLength(0)},{array2.GetLength(0)}]");

        Assert.AreEqual(array1.GetLength(1), array2.GetLength(1),
            $"Array dimension [{array1.GetLength(0)},{array1.GetLength(1)}] wrong. Expected: [{array2.GetLength(0)},{array2.GetLength(0)}]");

        for (var i = 0; i < array1.GetLength(0); i++) {
            for (var j = 0; j < array1.GetLength(1); j++) {
                Assert.That(array1[i, j], Is.EqualTo(array2[i, j]), "Element position: [" + i + "," + j + "]");
            }
        }
        return true;
    }
}