using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using NUnit.Framework;
using Tools.Animation;

namespace Veronenger.Tests {
    [TestFixture]
    public class TweenPlayerTests : Node {
        [Test(Description = "Loops and callbacks")]
        public IEnumerator TweenPlayerLoops() {
            var firstLoop = 0;
            var secondLoop = 0;

            var promise = new TaskCompletionSource<object>();

            Stopwatch x = null;
            new TweenPlayer()
                .NewTween(this)
                .CreateSequence()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(0.1f)
                .Callback(() => firstLoop++)
                .SetLoops(9)
                .EndSequence()
                .CreateSequence()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(0.1f)
                .Callback(() => secondLoop++)
                .SetLoops(5)
                .EndSequence()
                .SetLoops(2)
                .AddOnTweenPlayerFinishAll(delegate() {
                    x?.Stop();
                    promise.TrySetResult(null);
                })
                .Start();
            x = Stopwatch.StartNew();
            yield return promise.Task;
            Console.WriteLine(x.ElapsedMilliseconds);

            Assert.That(firstLoop, Is.EqualTo(2 * 9));
            Assert.That(secondLoop, Is.EqualTo(2 * 5));
        }

        [Test(Description = "Loops and step callbacks")]
        public IEnumerator TweenPlayerStepLoops() {
            var callbackStep1 = 0;
            var callbackStep2 = 0;
            var loops = 2;

            var promise = new TaskCompletionSource<object>();

            Sprite sprite = new Sprite();
            sprite.Position = new Vector2(100, 100);
            AddChild(sprite);
            yield return GetTree().ToSignal(GetTree(), "physics_frame");

            new TweenPlayer()
                .NewTween(this)
                .CreateSequence()
                .AnimateSteps(sprite, Property.Opacity)
                .To(12, 0.1f, Easing.BackIn, (node) => callbackStep1++)
                .To(12, 0.1f, Easing.BackIn, (node) => callbackStep1++)
                .EndAnimate()
                .SetLoops(loops)
                .EndSequence()
                .AddOnTweenPlayerFinishAll(delegate() { promise.TrySetResult(null); })
                .Start();

            yield return promise.Task;

            Assert.That(callbackStep1, Is.EqualTo(loops));
            Assert.That(callbackStep2, Is.EqualTo(loops));
        }

        [Test(Description = "Loops and callbacks")]
        public IEnumerator TweenSequence() {
            Sprite sprite = new Sprite();
            sprite.Position = new Vector2(100, 100);
            AddChild(sprite);
            yield return null;
            var tweenPlayer = new TweenPlayer()
                .NewTween(this);

            var sequence = tweenPlayer
                .CreateSequence()
                .AnimateSteps(sprite, Property.PositionX)
                .To(120, 0.1f, Easing.BackIn)
                .To(-90, 0.1f, Easing.BackIn)
                .EndAnimate();
        }
    }
}