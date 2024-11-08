using System;
using System.Collections.Generic;
using Betauer.Core.DataMath.Data;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestRunner.Test]
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
        var floatArray = FloatArray<string>.Parse("ABBBBBCCCD", mapping);

        Assert.AreEqual("A", floatArray.Get(-1f));
        
        Assert.AreEqual("A", floatArray.Get(0f));
        Assert.AreEqual("B", floatArray.Get(0.1f));
        Assert.AreEqual("B", floatArray.Get(0.2f));
        Assert.AreEqual("B", floatArray.Get(0.3f));
        Assert.AreEqual("B", floatArray.Get(0.4f));
        Assert.AreEqual("B", floatArray.Get(0.5f));
        Assert.AreEqual("B", floatArray.Get(0.6f));
        Assert.AreEqual("C", floatArray.Get(0.7f));
        Assert.AreEqual("C", floatArray.Get(0.8f));
        Assert.AreEqual("D", floatArray.Get(0.945f));
        Assert.AreEqual("D", floatArray.Get(1f));
    }


    [TestRunner.Test]
    public void NoRectanglesTest() {
        var floatArray = FloatGrid<string>.Parse("""
                                                 AAAAA
                                                 BCCCB
                                                 .....
                                                 ...A.
                                                 DDDDD
                                                 """, new Dictionary<char, string>() {
            { 'A', "A" },
            { 'B', "B" },
            { 'C', "C" },
            { 'D', "D" },
            { '.', "Empty" },
        });

        Assert.AreEqual(floatArray.Get(-1f, -1f), "A");
        Assert.AreEqual(floatArray.Get(0.1f, 0.1f), "A");
        Assert.AreEqual(floatArray.Get(0.5f, 0.5f), "Empty");
        Assert.AreEqual(floatArray.Get(1f, 1f), "D");
    }

    [TestRunner.Test]
    public void RectanglesTest() {
        var floatArray = FloatGrid<string>.Parse("""
                                                 AABB
                                                 CCBB
                                                 CCBB
                                                 """, new Dictionary<char, string>() {
            { 'A', "A" },
            { 'B', "B" },
            { 'C', "C" },
        });
        
        floatArray.CreateRectangles();
        Assert.AreEqual(floatArray.GetRect("A"), new Rect2(0f, 0f, 1f / 2, 1f / 3));
        Assert.AreEqual(floatArray.GetRect("B"), new Rect2(0.5f, 0f, 1f / 2, 1f));
        Assert.AreEqual(floatArray.GetRect("C"), new Rect2(0f, 1f/3, 1f / 2, 0.666666627f));
    }

    [TestRunner.Test]
    public void SimpleTest() {
        var floatArray = FloatGrid<string>.Parse("""
                                                 ABC
                                                 DEF
                                                 GHI
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

        // Assert.AreEqual(floatArray.GetRect("A"), new Rect2());

        Assert.AreEqual("A", floatArray.Get(-10f, -10f));
        
        Assert.AreEqual("A", floatArray.Get(-1f, -1f));
        Assert.AreEqual("B", floatArray.Get(0.5f, -1f));
        Assert.AreEqual("C", floatArray.Get(1f, -1f));
        Assert.AreEqual("C", floatArray.Get(10f, -1f));

        Assert.AreEqual("D", floatArray.Get(0f, 0.5f));
        Assert.AreEqual("E", floatArray.Get(0.5f, 0.5f));
        Assert.AreEqual("F", floatArray.Get(0.75f, 0.5f));

        Assert.AreEqual("G", floatArray.Get(0f, 0.75f));
        Assert.AreEqual("H", floatArray.Get(0.5f, 0.75f));
        Assert.AreEqual("I", floatArray.Get(0.75f, 0.75f));
        
        Assert.AreEqual("I", floatArray.Get(10f, 10f));
    }

    [TestRunner.Test]
    public void ValidateRectanglesTest() {
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
                                            A
                                            A
                                            """, mapping).IsValid(), Is.True);
        Assert.That(FloatGrid<string>.Parse("""
                                            A
                                            B
                                            """, mapping).IsValid(), Is.True);
        Assert.That(FloatGrid<string>.Parse("""
                                            AAA
                                            AAA
                                            AAA
                                            """, mapping).IsValid(), Is.True);
        Assert.That(FloatGrid<string>.Parse("""
                                            AA
                                            BB
                                            """, mapping).IsValid(), Is.True);
        Assert.That(FloatGrid<string>.Parse("""
                                            A
                                            B
                                            B
                                            C
                                            C
                                            C
                                            """, mapping).IsValid(), Is.True);
        Assert.That(FloatGrid<string>.Parse("""
                                            AAB
                                            AAC
                                            DDC
                                            """, mapping).IsValid(), Is.True);

        Assert.That(FloatGrid<string>.Parse("""
                                            AABBG
                                            AACFF
                                            DDCFF
                                            HIJJK
                                            HLJJM
                                            """, mapping).IsValid(), Is.True);

        
        
        Assert.That(FloatGrid<string>.Parse("ABA", mapping).IsValid(), Is.False);
        
        Assert.That(FloatGrid<string>.Parse("""
                                            AA
                                            AB
                                            """, mapping).IsValid(), Is.False);

        Assert.That(FloatGrid<string>.Parse("""
                                            ABA
                                            ABC
                                            """, mapping).IsValid(), Is.False);

        Assert.That(FloatGrid<string>.Parse("""
                                            AAA
                                            ABA
                                            AAA
                                            """, mapping).IsValid(), Is.False);

    }
}

file static class FloatGridExtensions {
    public static bool IsValid<T>(this FloatGrid<T> grid) {
        try {
            grid.CreateRectangles();
            return true;
        } catch (Exception e) {
            Console.WriteLine(e);
            return false;
        }
    }
}