using System;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.GridTools;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class DeadEndRemoverTest {
    [Test]
    public void DeadEndTest() {
        var data = Array2D.ParseAsBool("""
                                       ··#··##·····
                                       ·#########··
                                       ····#····#··
                                       ····###··###
                                       ··######····
                                       """, '#');
        
        Console.WriteLine(data.GetString((v) => v ? "#" : "·"));
        var deadEndRemover = DeadEndRemover.Create(data);
        deadEndRemover.Update();

        var result = data.GetString((v) => v ? "#" : "·");

        Console.WriteLine(result);

        Assert.That(result, Is.EqualTo(
            """
            ·····##·····
            ····######··
            ····#····#··
            ····###··##·
            ····###·····
            """));
    }
    
    [Test]
    public void RemoveAllTest() {
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
        DeadEndRemover.Create(data).RemoveAll();

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
}