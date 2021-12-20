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
            const float pause = 0.1f;
            const int seq1loops = 9;
            const int seq2loops = 5;
            const int playerLoops = 2;

            var promise = new TaskCompletionSource<object>();

            Stopwatch x = null;
            new TweenPlayer()
                .NewTween(this)
                .CreateSequence()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(pause)
                .Callback(() => firstLoop++)
                .SetLoops(seq1loops)
                .EndSequence()
                .CreateSequence()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(pause)
                .Callback(() => secondLoop++)
                .SetLoops(seq2loops)
                .EndSequence()
                .SetLoops(playerLoops)
                .SetAutoKill(true)
                .AddOnTweenPlayerFinishAll(delegate() {
                    x?.Stop();
                    promise.TrySetResult(null);
                })
                .Start();
            x = Stopwatch.StartNew();
            yield return promise.Task;
            Console.WriteLine("It should take: " + ((seq1loops * pause + seq2loops * pause) * playerLoops) +
                              "s Elapsed time: " + (x.ElapsedMilliseconds / 1000f) + "s");

            Assert.That(firstLoop, Is.EqualTo(playerLoops * seq1loops));
            Assert.That(secondLoop, Is.EqualTo(playerLoops * seq2loops));
        }

        [Test(Description = "Cancel callbacks")]
        public async Task TweenPlayerCancelCallbacks() {
            var callback = 0;
            var finished = 0;

            Stopwatch x = null;
            var tweenPlayer = new TweenPlayer()
                .NewTween(this)
                .CreateSequence()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(1f)
                .Callback(() => callback++)
                .EndSequence()
                .AddOnTweenPlayerFinishAll(delegate() {
                    x?.Stop();
                    finished++;
                })
                .Start();
            x = Stopwatch.StartNew();

            await Task.Delay(200);
            tweenPlayer.Stop();
            Console.WriteLine(x.ElapsedMilliseconds);
            Assert.That(callback, Is.EqualTo(0));
            Assert.That(finished, Is.EqualTo(0));

            // If the player is stopped, the callback shouldn't execute
            await Task.Delay(1000);
            Assert.That(callback, Is.EqualTo(0));
            Assert.That(finished, Is.EqualTo(0));

            // Player resume, callbacks were executed
            tweenPlayer.Start();
            await Task.Delay(900);
            Assert.That(callback, Is.EqualTo(1));
            Assert.That(finished, Is.EqualTo(1));
        }

        [Test(Description = "Test callbacks in steps")]
        public async Task TweenPlayerStepLoops() {
            var callbackStep1 = 0;
            var callbackStep2 = 0;
            var callbackStep3 = 0;
            var callbackStep4 = 0;
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
                .Parallel()
                .AnimateKeys(sprite, Property.PositionX)
                .Duration(0.1f)
                .KeyframeTo(0.1f, 120f, Easing.BackIn, (node) => callbackStep3++)
                .KeyframeTo(0.6f, -90f, Easing.BackIn, (node) => callbackStep4++)
                .EndAnimate()
                .SetLoops(loops)
                .EndSequence()
                .SetAutoKill(true)
                .SetLoops(loops)
                .Start()
                .Await();

            Assert.That(callbackStep1, Is.EqualTo(loops * loops));
            Assert.That(callbackStep2, Is.EqualTo(loops * loops));
            Assert.That(callbackStep3, Is.EqualTo(loops * loops));
            Assert.That(callbackStep4, Is.EqualTo(loops * loops));
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