using System;
using Betauer.Core.DataMath;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class AutomatasTests {

    [Test]
    public void GameOfLifeTest() {

        var data = Array2D.ParseAsBool("""
                                         ············
                                         ······#·····
                                         ·····##·····
                                         ······##····
                                         ············
                                         """, '#');
        var gol = Automatas.GameOfLife(data);
        gol.Update();
        gol.Update();
        gol.Update();
        gol.Update();
        var result = data.GetString((v) => v ? "#" : "·");
        
        Assert.That(result, Is.EqualTo("""
                                       ············
                                       ·····##·····
                                       ····#··#····
                                       ····#··#····
                                       ·····#·#····
                                       """));
    }

    [Test]
    public void DeadEndTest() {
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
            // var deadEndRemover = new DeadEndRemover(xYGrid);
            Automatas.RemoveAllDeadEnds(data);

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