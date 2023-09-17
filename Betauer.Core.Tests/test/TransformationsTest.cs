using NUnit.Framework;

namespace Betauer.Core.Tests;


[Betauer.TestRunner.Test]
public class TransformationsTests {

    [Betauer.TestRunner.Test]
    public void TransformationsTest() {

        var data = new[,] {
            {  0,  1,  2 },
            { 10, 11, 12 },
            { 20, 21, 22 }
        };
        
        AreEqual(data.FlipDiagonal(), new[,] {
            { 0, 10, 20 },
            { 1, 11, 21 },
            { 2, 12, 22 }
        });

        AreEqual(data.FlipDiagonalSecondary(), new[,] {
            { 22, 12, 2 },
            { 21, 11, 1 },
            { 20, 10, 0 }
        });

        AreEqual(data.FlipV(), new[,] {
            { 20, 21, 22 },
            { 10, 11, 12 },
            {  0,  1,  2 }
        });

        AreEqual(data.FlipH(), new[,] {
            {  2,  1,  0 },
            { 12, 11, 10 },
            { 22, 21, 20 }
        });
        
        AreEqual(data.Rotate90(), new[,] {
            { 20, 10, 0 },
            { 21, 11, 1 },
            { 22, 12, 2 }
        });
        
        AreEqual(data.RotateMinus90(), new[,] {
            { 2, 12, 22 },
            { 1, 11, 21 },
            { 0, 10, 20 }
        });
        AreEqual(data.Rotate180(), new[,] {
            { 22, 21, 20},
            { 12, 11, 10},
            {  2,  1,  0}
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