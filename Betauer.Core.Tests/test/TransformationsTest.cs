using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;


[Betauer.TestRunner.Test]
public class TransformationsTests {

    [Betauer.TestRunner.Test]
    public void TransformationsSquareTests() {
        var square = new[,] {
            { 0, 1, 2 },
            { 10, 11, 12 },
            { 20, 21, 22 }
        };

        AreEqual(square.FlipDiagonal(), new[,] {
            { 0, 10, 20 },
            { 1, 11, 21 },
            { 2, 12, 22 }
        });

        AreEqual(square.FlipDiagonalSecondary(), new[,] {
            { 22, 12, 2 },
            { 21, 11, 1 },
            { 20, 10, 0 }
        });

    }
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
    }
    
    public bool AreEqual<T>(T[,] array1, T[,] array2) {
        Assert.AreEqual(array1.GetLength(0), array2.GetLength(0));
        Assert.AreEqual(array1.GetLength(1), array2.GetLength(1));
        for (int i = 0; i < array1.GetLength(0); i++) {
            for (int j = 0; j < array1.GetLength(1); j++) {
                Assert.That(array1[i, j], Is.EqualTo(array2[i, j]), "Element position: [" + i + "," + j + "]");
            }
        }

        return true;
    }
}