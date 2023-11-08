using Betauer.Core.Image;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests; 

[TestRunner.Test]
public class FastNoiseTest {

    [TestRunner.Test]
    public void Test() {
        var fastNoiseLite = new Godot.FastNoiseLite() {
            Seed = 1
        };
        var noise = new FastNoise(fastNoiseLite, 100, 100, 4);
        // noise.GetImage().SavePng("test0.png");
        // noise.GetImage(1).SavePng("test1.png");
        // noise.GetImage(2).SavePng("test2.png");
        // noise.GetImage(3).SavePng("test3.png");
        Assert.That(noise.GetNoise(10, 20, 3), Is.EqualTo(0.278431386f));
    }
    
}