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
        var gol = Automatas.CreateGameOfLife(data);
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

}