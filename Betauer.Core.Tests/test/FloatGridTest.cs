using System.Collections.Generic;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests; 

[TestRunner.Test]
public class FloatGridTest {

    [TestRunner.Test]
    public void Test1() {
        
        
        var x = FloatGrid<string>.Parse("""
                                           :AAAAA:
                                           :BCCCB:
                                           :.....:
                                           :...A.:
                                           :DDDDD:
                                           """, new Dictionary<char, string>() {
            {'A', "A"},
            {'B', "B"},
            {'C', "C"},
            {'D', "D"},
            {'.', "Empty"},
        });
        
        Assert.AreEqual( x.Get(0f, 0f), "Empty");
        Assert.AreEqual( x.Get(1f, 1f), "D");
        Assert.AreEqual( x.Get(-1f, -1f), "A");
        Assert.AreEqual( x.Get(0.5f, 0.5f), "A");
    }
    
    [TestRunner.Test]
    public void Test2() {
        var floatArray = FloatGrid<string>.Parse("""
                                           :ABC:
                                           :DEF:
                                           :GHI:
                                           """, new Dictionary<char, string>() {
            {'A', "A"},
            {'B', "B"},
            {'C', "C"},
            {'D', "D"},
            {'E', "E"},
            {'F', "F"},
            {'G', "G"},
            {'H', "H"},
            {'I', "I"},
        });
        
        Assert.AreEqual("A", floatArray.Get(-1000f, -1000f));
        Assert.AreEqual("A", floatArray.Get(-1f, -1f));
        Assert.AreEqual("B", floatArray.Get(0f, -1f));
        Assert.AreEqual("C", floatArray.Get(1f, -1f));
        
        Assert.AreEqual("D", floatArray.Get(-1f, 0f));

        Assert.AreEqual("E", floatArray.Get(-0.49f, -0.49f));
        Assert.AreEqual("E", floatArray.Get(0f, 0f));
        Assert.AreEqual("E", floatArray.Get(0.49f, 0.49f));
        Assert.AreEqual("F", floatArray.Get(1f, 0f));
        
        Assert.AreEqual("G", floatArray.Get(-1f, 1f));
        Assert.AreEqual("H", floatArray.Get(0f, 1f));
        Assert.AreEqual("I", floatArray.Get(1f, 1f));
        Assert.AreEqual("I", floatArray.Get(100f, 100f));

        floatArray.MinX = -50;
        floatArray.MaxX = 50;
        floatArray.MinY = 1000;
        floatArray.MaxY = 0;

        Assert.AreEqual("A", floatArray.Get(-50, 1000f));
        Assert.AreEqual("E", floatArray.Get(0, 500f));
        Assert.AreEqual("I", floatArray.Get(50f, 0f));

    }
}