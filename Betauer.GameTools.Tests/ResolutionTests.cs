using System;
using Betauer.Application.Screen;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    [TestFixture]
    public class ResolutionTests {

        [Test]
        public void AspectRatioEqualityTests() {
            Console.WriteLine("== true");
            Assert.That((AspectRatio)null == null);
            Assert.That(null == (AspectRatio)null);
            Assert.That((AspectRatio)null == (AspectRatio)null);
            Assert.That(new AspectRatio(21, 9) == new AspectRatio(21, 9));
            Assert.That(new AspectRatio(21 * 2, 9 * 2) == new AspectRatio(21, 9));
            
            Console.WriteLine("== false");
            Assert.That(new AspectRatio(21, 9) == null, Is.False);
            Assert.That(new AspectRatio(21, 9) == (AspectRatio)null, Is.False);
            Assert.That(null == new AspectRatio(21, 9), Is.False);
            Assert.That((AspectRatio)null == new AspectRatio(21, 9), Is.False);
            Assert.That(new AspectRatio(21, 9) == new AspectRatio(21, 10), Is.False);

            Console.WriteLine("!= false");
            Assert.That((AspectRatio)null != null, Is.False);
            Assert.That(null != (AspectRatio)null, Is.False);
            Assert.That((AspectRatio)null != (AspectRatio)null, Is.False);
            Assert.That(new AspectRatio(21, 9) != new AspectRatio(21, 9), Is.False);
            Assert.That(new AspectRatio(21 * 2, 9 * 2) != new AspectRatio(21, 9), Is.False);
            Assert.That(new AspectRatio(21, 9) != new AspectRatio(21 * 2, 9 * 2), Is.False);
            Console.WriteLine("!= true");
            Assert.That(new AspectRatio(21, 9) != null);
            Assert.That(new AspectRatio(21, 9) != (AspectRatio)null);
            Assert.That(null != new AspectRatio(21, 9));
            Assert.That((AspectRatio)null != new AspectRatio(21, 9));
            Assert.That(new AspectRatio(21, 9) != new AspectRatio(21, 10));
        }

        [Test]
        public void ResolutionEqualityTests() {
            Console.WriteLine("== true");
            Assert.That((Resolution)null == null);
            Assert.That((Resolution)null == (Resolution)null);
            Assert.That(null == (Resolution)null);
            Assert.That(new Resolution(2, 3) == new Resolution(2, 3));

            Console.WriteLine("== false");
            Assert.That(new Resolution(21, 9) == null, Is.False);
            Assert.That(new Resolution(21, 9) == (Resolution)null, Is.False);
            Assert.That(null == new Resolution(21, 9), Is.False);
            Assert.That((Resolution)null == new Resolution(21, 9), Is.False);
            Assert.That(new Resolution(21, 9) == new Resolution(21, 10), Is.False);

            Console.WriteLine("!= false");
            Assert.That((Resolution)null != null, Is.False);
            Assert.That((Resolution)null != (Resolution)null, Is.False);
            Assert.That(null != (Resolution)null, Is.False);
            Assert.That(new Resolution(2, 3) != new Resolution(2, 3), Is.False);

            Console.WriteLine("!= true");
            Assert.That(new Resolution(21, 9) != null);
            Assert.That(new Resolution(21, 9) != (Resolution)null);
            Assert.That(null != new Resolution(21, 9));
            Assert.That((Resolution)null != new Resolution(21, 9));
            Assert.That(new Resolution(21, 9) != new Resolution(21, 10));

        }
        [Test]
        public void ScaledResolutionEqualityTests() {
            var base1 = new Vector2(2, 3);
            var size1 = new Vector2(4, 5);
            Console.WriteLine("== true");
            Assert.That((ScaledResolution)null == null);
            Assert.That((ScaledResolution)null == (ScaledResolution)null);
            Assert.That(null == (ScaledResolution)null);
            Assert.That(new ScaledResolution(base1, size1) == new ScaledResolution(base1, size1));

            Console.WriteLine("== false");
            Assert.That(new ScaledResolution(base1, size1) == null, Is.False);
            Assert.That(new ScaledResolution(base1, size1) == (ScaledResolution)null, Is.False);
            Assert.That(null == new ScaledResolution(base1, size1), Is.False);
            Assert.That((ScaledResolution)null == new ScaledResolution(base1, size1), Is.False);
            Assert.That(new ScaledResolution(base1, size1) == new ScaledResolution(base1, size1 + Vector2.One), Is.False);
            Assert.That(new ScaledResolution(base1, size1) == new ScaledResolution(base1 + Vector2.One, size1), Is.False);
            
            Console.WriteLine("!= false");
            Assert.That((ScaledResolution)null != null, Is.False);
            Assert.That((ScaledResolution)null != (ScaledResolution)null, Is.False);
            Assert.That(null != (ScaledResolution)null, Is.False);
            Assert.That(new ScaledResolution(base1, size1) != new ScaledResolution(base1, size1), Is.False);

            Console.WriteLine("!= true");
            Assert.That(new ScaledResolution(base1, size1) != null);
            Assert.That(new ScaledResolution(base1, size1) != (ScaledResolution)null);
            Assert.That(null != new ScaledResolution(base1, size1));
            Assert.That((ScaledResolution)null != new ScaledResolution(base1, size1));
            Assert.That(new ScaledResolution(base1, size1) != new ScaledResolution(base1, size1 + Vector2.One));
            Assert.That(new ScaledResolution(base1, size1) != new ScaledResolution(base1 + Vector2.One, size1));

        }


    }
}