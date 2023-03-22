using System;
using Betauer.Application.Screen;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests; 

[TestRunner.Test]
public class ResolutionTests {

    [TestRunner.Test]
    public void AspectRatioEqualityTests() {
        Console.WriteLine("== true");
        var aspectRatio = new AspectRatio(21, 9);

        Assert.That((AspectRatio)null == null);
        Assert.That(null == (AspectRatio)null);
        Assert.That((AspectRatio)null == (AspectRatio)null);
        Assert.That(aspectRatio == aspectRatio);
        Assert.That(aspectRatio == new AspectRatio(21, 9));
        Assert.That(aspectRatio.Equals(aspectRatio));
        Assert.That(aspectRatio.Equals(new AspectRatio(21, 9)));
        // Special cases: aspect ratio are equal if the ratio is equals, no matter the size
        Assert.That(aspectRatio.Equals(new AspectRatio(21 * 2, 9 * 2)));
        Assert.That(aspectRatio == new AspectRatio(21 * 2, 9 * 2));
            
        Console.WriteLine("== false");
        Assert.That(aspectRatio == null, Is.False);
        Assert.That(aspectRatio == (AspectRatio)null, Is.False);
        Assert.That(null == aspectRatio, Is.False);
        Assert.That((AspectRatio)null == aspectRatio, Is.False);
        Assert.That(aspectRatio.Equals((AspectRatio)null), Is.False);
        Assert.That(aspectRatio.Equals((object)null), Is.False);
        Assert.That(aspectRatio.Equals(null), Is.False);
        Assert.That(aspectRatio.Equals(new AspectRatio(21, 10)), Is.False);
        Assert.That(aspectRatio == new AspectRatio(21, 10), Is.False);

        Console.WriteLine("!= false");
        Assert.That((AspectRatio)null != null, Is.False);
        Assert.That(null != (AspectRatio)null, Is.False);
        Assert.That((AspectRatio)null != (AspectRatio)null, Is.False);
        Assert.That(aspectRatio != aspectRatio, Is.False);
        // Special cases: aspect ratio are equal if the ratio is equals, no matter the size
        Assert.That(aspectRatio != new AspectRatio(21, 9), Is.False);
        Assert.That(aspectRatio != new AspectRatio(21*2, 9*2), Is.False);
            
        Console.WriteLine("!= true");
        Assert.That(aspectRatio != null);
        Assert.That(aspectRatio != (AspectRatio)null);
        Assert.That(null != aspectRatio);
        Assert.That((AspectRatio)null != aspectRatio);
        Assert.That(aspectRatio != new AspectRatio(21, 10));
    }

    [TestRunner.Test]
    public void ResolutionEqualityTests() {
        var res = new Resolution(2, 3);
        Console.WriteLine("== true");
        Assert.That((Resolution)null == null);
        Assert.That((Resolution)null == (Resolution)null);
        Assert.That(null == (Resolution)null);
        Assert.That(res == res);
        Assert.That(res == new Resolution(2, 3));
        Assert.That(res.Equals(res));
        Assert.That(res.Equals(new Resolution(2, 3)));

        Console.WriteLine("== false");
        Assert.That(res == null, Is.False);
        Assert.That(res == (Resolution)null, Is.False);
        Assert.That(null == res, Is.False);
        Assert.That((Resolution)null == res, Is.False);
        Assert.That(res.Equals((Resolution)null), Is.False);
        Assert.That(res.Equals((object)null), Is.False);
        Assert.That(res.Equals(null), Is.False);
        Assert.That(res.Equals(new Resolution(21, 10)), Is.False);
        Assert.That(res == new Resolution(21, 10), Is.False);

        Console.WriteLine("!= false");
        Assert.That((Resolution)null != null, Is.False);
        Assert.That((Resolution)null != (Resolution)null, Is.False);
        Assert.That(null != (Resolution)null, Is.False);
        Assert.That(res != res, Is.False);
        Assert.That(res != new Resolution(2, 3), Is.False);

        Console.WriteLine("!= true");
        Assert.That(new Resolution(21, 9) != null);
        Assert.That(new Resolution(21, 9) != (Resolution)null);
        Assert.That(null != new Resolution(21, 9));
        Assert.That((Resolution)null != new Resolution(21, 9));
        Assert.That(new Resolution(21, 9) != new Resolution(21, 10));

    }
    [TestRunner.Test]
    public void ScaledResolutionEqualityTests() {
        var base1 = new Vector2I(2, 3);
        var size1 = new Vector2I(4, 5);
        var res = new ScaledResolution(base1, size1);
        Console.WriteLine("== true");
        Assert.That((ScaledResolution)null == null);
        Assert.That((ScaledResolution)null == (ScaledResolution)null);
        Assert.That(null == (ScaledResolution)null);
        Assert.That(res == res);
        Assert.That(res == new ScaledResolution(base1, size1));
        Assert.That(res.Equals(res));
        Assert.That(res.Equals(new ScaledResolution(base1, size1)));

        Console.WriteLine("== false");
        Assert.That(res == null, Is.False);
        Assert.That(res == (ScaledResolution)null, Is.False);
        Assert.That(null == res, Is.False);
        Assert.That((ScaledResolution)null == res, Is.False);
        Assert.That(res.Equals((ScaledResolution)null), Is.False);
        Assert.That(res.Equals((object)null), Is.False);
        Assert.That(res.Equals(null), Is.False);
        Assert.That(res.Equals(new ScaledResolution(base1, size1 + Vector2I.One)), Is.False);
        Assert.That(res.Equals(new ScaledResolution(base1 + Vector2I.One, size1)), Is.False);
        Assert.That(res == new ScaledResolution(base1, size1 + Vector2I.One), Is.False);
        Assert.That(res == new ScaledResolution(base1 + Vector2I.One, size1), Is.False);
            
        Console.WriteLine("!= false");
        Assert.That((ScaledResolution)null != null, Is.False);
        Assert.That((ScaledResolution)null != (ScaledResolution)null, Is.False);
        Assert.That(null != (ScaledResolution)null, Is.False);
        Assert.That(res != res, Is.False);
        Assert.That(res != new ScaledResolution(base1, size1), Is.False);

        Console.WriteLine("!= true");
        Assert.That(res != null);
        Assert.That(res != (ScaledResolution)null);
        Assert.That(null != res);
        Assert.That((ScaledResolution)null != res);
        Assert.That(res != new ScaledResolution(base1, size1 + Vector2I.One));
        Assert.That(res != new ScaledResolution(base1 + Vector2I.One, size1));

    }


}