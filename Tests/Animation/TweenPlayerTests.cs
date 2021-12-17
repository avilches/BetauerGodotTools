using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using NUnit.Framework;
using Tools;
using Tools.Animation;
using Veronenger.Tests.Runner;

namespace Veronenger.Tests.Animation {
    [TestFixture]
    public class TweenPlayerTests : Node {
        [Test(Description = "Loops and callbacks")]
        public IEnumerator TweenPlayerLoops() {
            var firstLoop = 0;
            var secondLoop = 0;
            const int loops = 2;

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
                .SetLoops(loops)
                .SetAutoKill(true)
                .AddOnTweenPlayerFinishAll(delegate() {
                    x?.Stop();
                    promise.TrySetResult(null);
                })
                .Start();
            x = Stopwatch.StartNew();
            yield return promise.Task;
            Console.WriteLine(x.ElapsedMilliseconds);

            Assert.That(firstLoop, Is.EqualTo(loops * 9));
            Assert.That(secondLoop, Is.EqualTo(loops * 5));
        }

        [Test(Description = "Test values in steps")]
        public async Task TweenPlayerStepLoops() {
            var callbackStep1 = 0;
            var callbackStep2 = 0;
            const int loops = 2;

            Sprite sprite = new Sprite();
            sprite.Position = new Vector2(100, 100);
            AddChild(sprite);
            await this.AwaitPhysicsFrame();

            TweenPlayer tweenPlayer = await new TweenPlayer()
                .NewTween(this)
                .CreateSequence()
                .AnimateSteps(sprite, Property.Opacity)
                .To(12, 0.1f, Easing.BackIn, (node) => callbackStep1++)
                .To(12, 0.1f, Easing.BackIn, (node) => callbackStep2++)
                .EndAnimate()
                .SetLoops(loops)
                .EndSequence()

                .SetAutoKill(true)
                .SetLoops(loops)
                .Start()
                .Await();

            Assert.That(callbackStep1, Is.EqualTo(loops * loops));
            Assert.That(callbackStep2, Is.EqualTo(loops * loops));
        }

        [Test(Description = "Test values in steps")]
        public IEnumerator TweenSequenceSteps() {
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
                .To(-90, 0.2f)
                .EndAnimate();

            var tweener = GetTweener<PropertyKeyStepTweener<float>>(sequence, 0);
            var steps = tweener.CreateStepList();
            Assert.That(steps[0].Duration, Is.EqualTo(0.1f));
            Assert.That(steps[0].Easing, Is.EqualTo(Easing.BackIn));

            Assert.That(steps[1].Duration, Is.EqualTo(0.2f));
            Assert.That(steps[1].Easing, Is.Null);
        }

        [Test(Description = "Test values in keyframes")]
        public IEnumerator TweenSequenceKey() {
            Sprite sprite = new Sprite();
            sprite.Position = new Vector2(100, 100);
            AddChild(sprite);
            yield return null;
            var tweenPlayer = new TweenPlayer()
                .NewTween(this);

            var sequence = tweenPlayer
                .CreateSequence()
                .AnimateKeys(sprite, Property.PositionX)
                .Duration(0.5f)
                .KeyframeTo(0.1f, 120f, Easing.BackIn)
                .KeyframeTo(0.6f, -90f)
                .EndAnimate();

            var tweener = GetTweener<PropertyKeyPercentTweener<float>>(sequence, 0);
            Assert.That(tweener.AllStepsDuration, Is.EqualTo(0.5f));

            var steps = tweener.CreateStepList();
            Assert.That(steps[0].Percent, Is.EqualTo(0.1f));
            Assert.That(steps[0].Easing, Is.EqualTo(Easing.BackIn));

            Assert.That(steps[1].Percent, Is.EqualTo(0.6f));
            Assert.That(steps[1].Easing, Is.Null);
        }

        private List<ITweener> GetParallelGroup(ITweenSequence sequence, int group) {
            return sequence.TweenList.ToList()[group].ToList();
        }
        private T GetTweener<T>(ITweenSequence sequence, int group, int pos = 0) where T : class {
            return sequence.TweenList.ToList()[group].ToList()[pos] as T;
        }
    }
}