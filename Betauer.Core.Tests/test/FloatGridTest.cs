using System;
using System.Collections.Generic;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests; 

[TestRunner.Test]
[Only]
public class FloatGridTest {

    [TestRunner.Test]
    public void FloatArrayTest() {
        var mapping = new Dictionary<char, string>() {
            { 'A', "A" },
            { 'B', "B" },
            { 'C', "C" },
            { 'D', "D" },
            { 'E', "E" },
            { 'F', "F" },
            { 'G', "G" },
            { 'H', "H" },
            { 'I', "I" },
        };
        var floatArray = FloatArray<string>.Parse("ABC", mapping);

        Assert.AreEqual("A", floatArray.Get(-1000f));
        Assert.AreEqual("A", floatArray.Get(-1f));
        Assert.AreEqual("B", floatArray.Get(-0.2f));
        Assert.AreEqual("B", floatArray.Get(0f));
        Assert.AreEqual("B", floatArray.Get(0.2f));
        Assert.AreEqual("C", floatArray.Get(1f));
        Assert.AreEqual("C", floatArray.Get(100f));
    }


    [TestRunner.Test]
    public void Test0() {
        
        
        var floatArray = FloatGrid<string>.Parse("""
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

        floatArray.MinX = -1f;
        floatArray.MaxX = 1;
        floatArray.MinY = -1f;
        floatArray.MaxY = 1;

        Assert.AreEqual( floatArray.Get(0f, 0f), "Empty");
        Assert.AreEqual( floatArray.Get(1f, 1f), "D");
        Assert.AreEqual( floatArray.Get(-1f, -1f), "A");
        Assert.AreEqual( floatArray.Get(0.5f, 0.5f), "A");
    }

    [TestRunner.Test]
    public void Test1() {
        var floatArray = FloatGrid<string>.Parse("""
                                                 :ABC:
                                                 :DEF:
                                                 :GHI:
                                                 """, new Dictionary<char, string>() {
            { 'A', "A" },
            { 'B', "B" },
            { 'C', "C" },
            { 'D', "D" },
            { 'E', "E" },
            { 'F', "F" },
            { 'G', "G" },
            { 'H', "H" },
            { 'I', "I" },
        });

        Assert.AreEqual("A", floatArray.Get(0, 0));
        Assert.AreEqual("B", floatArray.Get(0.5f, 0));
        Assert.AreEqual("E", floatArray.Get(0.5f, 0.5f));
        Assert.AreEqual("I", floatArray.Get(1f, 1f));
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
        
        floatArray.MinX = -1f;
        floatArray.MaxX = 1;
        floatArray.MinY = -1f;
        floatArray.MaxY = 1;

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

    [TestRunner.Test]
    public void Validate() {
        var mapping = new Dictionary<char, string>() {
            { 'A', "A" },
            { 'B', "B" },
            { 'C', "C" },
            { 'D', "D" },
            { 'E', "E" },
            { 'F', "F" },
            { 'G', "G" },
            { 'H', "H" },
            { 'I', "I" },
            { 'J', "J" },
            { 'K', "K" },
            { 'L', "L" },
            { 'M', "M" },
        };
        Assert.That(FloatGrid<string>.Parse("A", mapping).IsValid(), Is.True);
        Assert.That(FloatGrid<string>.Parse("AA", mapping).IsValid(), Is.True);
        Assert.That(FloatGrid<string>.Parse("AB", mapping).IsValid(), Is.True);
        Assert.That(FloatGrid<string>.Parse("ABBCCC", mapping).IsValid(), Is.True);
        Assert.That(FloatGrid<string>.Parse("""
                                            :A:
                                            :A:
                                            """, mapping).IsValid(), Is.True);
        Assert.That(FloatGrid<string>.Parse("""
                                            :A:
                                            :B:
                                            """, mapping).IsValid(), Is.True);
        Assert.That(FloatGrid<string>.Parse("""
                                            :AAA:
                                            :AAA:
                                            :AAA:
                                            """, mapping).IsValid(), Is.True);
        Assert.That(FloatGrid<string>.Parse("""
                                            :AA:
                                            :BB:
                                            """, mapping).IsValid(), Is.True);
        Assert.That(FloatGrid<string>.Parse("""
                                            :A:
                                            :B:
                                            :B:
                                            :C:
                                            :C:
                                            :C:
                                            """, mapping).IsValid(), Is.True);
        Assert.That(FloatGrid<string>.Parse("""
                                            :AAB:
                                            :AAC:
                                            :DDC:
                                            """, mapping).IsValid(), Is.True);

        Assert.That(FloatGrid<string>.Parse("""
                                            :AABBG:
                                            :AACFF:
                                            :DDCFF:
                                            :HIJJK:
                                            :HLJJM:
                                            """, mapping).IsValid(), Is.True);

        
        
        Assert.That(FloatGrid<string>.Parse("ABA", mapping).IsValid(), Is.False);
        
        Assert.That(FloatGrid<string>.Parse("""
                                            :AA:
                                            :AB:
                                            """, mapping).IsValid(), Is.False);

        Assert.That(FloatGrid<string>.Parse("""
                                            :ABA:
                                            :ABC:
                                            """, mapping).IsValid(), Is.False);

        Assert.That(FloatGrid<string>.Parse("""
                                            :AAA:
                                            :ABA:
                                            :AAA:
                                            """, mapping).IsValid(), Is.False);

    }
}

file static class FloatGridExtensions {
    public static bool IsValid<T>(this FloatGrid<T> grid) {
        try {
            grid.ValidateRectangles();
            return true;
        } catch (Exception e) {
            Console.WriteLine(e);
            return false;
        }
    }
}