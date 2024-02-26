using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestRunner.Test]
public class XyTransformationsTests {
    
    /*
    [TestRunner.Test]
    public void TransformationsRotationsTests() {
        var original = new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        };
        AreEqual(original.XyRotate90(), new[,] {
            { 20, 10, 0 },
            { 21, 11, 1 },
            { 22, 12, 2 },
            { 23, 13, 3 }
        });
        
        AreEqual(original.XyRotateMinus90(), new[,] {
            { 3, 13, 23 },
            { 2, 12, 22 },
            { 1, 11, 21 },
            { 0, 10, 20 }
        });
        
        AreEqual(original.XyRotate180(), new[,] {
            { 23, 22, 21, 20},
            { 13, 12, 11, 10},
            {  3,  2,  1,  0}
        });
    }
    */

    /*
    [TestRunner.Test]
    public void TransformationsFlipTests() {
        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.XyFlipH(), new[,] {
            {  3,  2,  1,  0 },
            { 13, 12, 11, 10 },
            { 23, 22, 21, 20 }
        });
        
        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.XyFlipV(), new[,] {
            { 20, 21, 22, 23 },
            { 10, 11, 12, 13 },
            {  0,  1,  2,  3 }
        });

        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.XyFlipDiagonal(), new[,] {
            { 0, 10, 20 },
            { 1, 11, 21 },
            { 2, 12, 22 },
            { 3, 13, 23 }
        });

        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.XyFlipDiagonalSecondary(), new[,] {
            { 23, 13, 3 },
            { 22, 12, 2 },
            { 21, 11, 1 },
            { 20, 10, 0 }
        });
    }
    

    [TestRunner.Test]
    public void TransformationsMirrorTests() {
        AreEqual(new[,] {
            {  0,  1,  2,  3,  4 },
            { 10, 11, 12, 13, 14 },
            { 20, 21, 22, 23, 15 }
        }.XyMirrorLeftToRight(), new[,] {
            {  0,  1,  2,  1,  0 },
            { 10, 11, 12, 11, 10 },
            { 20, 21, 22, 21, 20 }
        });
        
        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.XyMirrorLeftToRight(), new[,] {
            {  0,  1,  1,  0 },
            { 10, 11, 11, 10 },
            { 20, 21, 21, 20 }
        });

        AreEqual(new[,] {
            {  0,  1,  2,  3,  4 },
            { 10, 11, 12, 13, 14 },
            { 20, 21, 22, 23, 15 }
        }.XyMirrorRightToLeft(), new[,] {
            {  4,  3,  2,  3,  4 },
            { 14, 13, 12, 13, 14 },
            { 15, 23, 22, 23, 15 }
        });
        
        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.XyMirrorRightToLeft(), new[,] {
            {  3,  2,  2,  3 },
            { 13, 12, 12, 13 },
            { 23, 22, 22, 23 }
        });
        
        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.XyMirrorTopToBottom(), new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            {  0,  1,  2,  3 }
        });

        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 },
            { 30, 31, 32, 33 }
        }.XyMirrorTopToBottom(), new[,] {
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
        }.XyMirrorBottomToTop(), new[,] {
            { 30, 31, 32, 33 },
            { 20, 21, 22, 23 },
            { 20, 21, 22, 23 },
            { 30, 31, 32, 33 }
        });
    }

    [TestRunner.Test]
    public void TransformationsResizeTests() {
        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.XyResize(3, 4, -1), new[,] {
            {  0,  1,  2 },
            { 10, 11, 12 },
            { 20, 21, 22 },
            { -1, -1, -1 },
        });
        
        AreEqual(new[,] {
            {  0,  1,  2,  3 },
            { 10, 11, 12, 13 },
            { 20, 21, 22, 23 }
        }.XyResize(3, 4, -1), new[,] {
            {  0,  1,  2 },
            { 10, 11, 12 },
            { 20, 21, 22 },
            { -1, -1, -1 },
        });
    }
    */
    [TestRunner.Test]
    public void CopyGridTests() {
        var original = new[,] {
            {  0,  1,  2,  3,  4 },
            { 10, 11, 12, 13, 14 },
            { 20, 21, 22, 23, 24 },
            { 30, 31, 32, 33, 34 },
        };
            
        AreEqual(original.GetXyRect(0, 0, 4, 5), original);
        AreEqual(original.GetXyRectCenter(0, 0, 3, 4), new[,] {
            { 4,  4, 4 },
            { 4,  0, 1 },
            { 4, 10, 11 },
        });
        AreEqual(original.GetXyRect(1, 2, 2, 2), new[,] {
            { 12, 13 },
            { 22, 23 },
        });

        AreEqual(original.GetXyRect(1, 2, 2, 8, -1), new[,] {
            { 12, 13, 14, -1, -1, -1, -1, -1 },
            { 22, 23, 24, -1, -1, -1, -1, -1 },
        });

        var dest = new int[4, 4];
        original.CopyXyRect(0, 0, 2, 3, dest);
        AreEqual(dest, new[,] {
            {  0,  1,  2, 0 },
            { 10, 11, 12, 0 },
            {  0,  0, 0, 0 },
            {  0,  0, 0, 0 },
        });

        original.CopyXyRect(1, 2, 2, 3, dest);
        AreEqual(dest, new[,] {
            { 12, 13, 14, 0 },
            { 22, 23, 24, 0 },
            {  0,  0,  0, 0 },
            {  0,  0,  0, 0 },
        });
        original.CopyXyRect(1, 2, 10, 10, dest);
        AreEqual(dest, new[,] {
            { 12, 13, 14,  0 },
            { 22, 23, 24,  0 },
            { 32, 33, 34,  0 },
            {  0,  0,  0,  0 },
        });
        
        dest = new int[2, 3];
        original.CopyXyRect(3, 2, 1, 1, dest);
        AreEqual(dest, new[,] {
            { 32,  0,  0 },
            { 0 ,  0,  0 },
        });
        original.CopyXyRect(3, 3, 8, 8, dest);
        AreEqual(dest, new[,] {
            { 33, 34,  0 },
            {  0,  0,  0 },
        });
        original.CopyXyRect(0, 0, 8, 8, dest);
        AreEqual(dest, new[,] {
            {  0,  1,  2 },
            { 10, 11, 12 },
        });
        original.CopyXyRect(-1, -1, 8, 8, dest);
        AreEqual(dest, new[,] {
            {  0,  0,  0 },
            {  0,  0,  1 },
        });
        
        AreEqual(original.GetXyRect(v => v.ToString()), new string[,] {
            {  "0",  "1",  "2",  "3",  "4" },
            { "10", "11", "12", "13", "14" },
            { "20", "21", "22", "23", "24" },
            { "30", "31", "32", "33", "34" },
        });

        var buffer = new int[3, 3];
        original.CopyXyCenterRect(0, 0, -1, buffer);
        AreEqual(buffer, new int[,] {
            { -1, -1, -1 },
            { -1,  0,  1 },
            { -1, 10, 11 },
        });

        original.CopyXyCenterRect(1, 2, -1, buffer);
        AreEqual(buffer, new int[,] {
            {  1,  2,  3 },
            { 11, 12, 13 },
            { 21, 22, 23 },
        });
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