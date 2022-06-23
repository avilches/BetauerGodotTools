using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using NUnit.Framework;
using Betauer.Animation;
using Betauer.Animation.Tests;
using Betauer.TestRunner;

namespace Betauer.Animation.Tests {
    [TestFixture]
    public class TweenPlayerTests : NodeTest {
        [SetUp]
        public void SetUp() {
            Engine.TimeScale = 10;
        }

        [TearDown]
        public void TearDown() {
            Engine.TimeScale = 1;
        }

        [Test(Description = "Test OnAction method")]
        public async Task OnStart() {

            var sprite = await CreateSprite();
            Property.Opacity.SetValue(sprite, 1f);
            Assert.That(sprite.Modulate.a, Is.EqualTo(1f));
            
            // when created, it's not running
            var t = new SingleSequencePlayer()
                .WithParent(this, false)
                .CreateSequence(sprite)
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .EndSequence()
                .Play();
            
            Assert.That(sprite.Modulate.a, Is.EqualTo(0f));
        }

        [Test(Description = "SingleSequencePlayer await works, multiple executions, no disposed")]
        public async Task SingleSequencePlayerAwait() {
            var l1 = 0;
            var l2 = 0;

            // when created, it's not running
            var t = new SingleSequencePlayer()
                .WithParent(this, false)
                .CreateSequence()
                .Callback(() => l1++)
                .EndSequence()
                .AddOnFinishAll(() => l2++);
            Assert.That(t.IsRunning, Is.EqualTo(false));

            // When started, it's running
            t.Play();
            Assert.That(t.IsRunning, Is.EqualTo(true));

            // Await, then it's not running and results are ok
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(1));

            // Await again, it produces the same state: not running and same results
            await t.Await();
            await t.Await();
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(1));

            // If start + await again, it will work properly
            t.Play();
            Assert.That(t.IsRunning, Is.EqualTo(true));
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            Assert.That(l1, Is.EqualTo(2));
            Assert.That(l2, Is.EqualTo(2));

            // If start + await again multiple times, it will work properly
            t.Play();
            t.Play();
            t.Play();
            t.Play();
            Assert.That(t.IsRunning, Is.EqualTo(true));
            await t.Await();
            await t.Await();
            await t.Await();
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            Assert.That(l1, Is.EqualTo(3));
            Assert.That(l2, Is.EqualTo(3));
        }

        [Test(Description = "SingleSequencePlayer await works, multiple executions, disposed")]
        public async Task SingleSequencePlayerAwaitDisposed() {
            var l1 = 0;
            var l2 = 0;

            // when created, it's not running
            var t = new SingleSequencePlayer()
                .WithParent(this, true)
                .CreateSequence()
                .Callback(() => l1++)
                .EndSequence()
                .AddOnFinishAll(() => l2++);
            Assert.That(t.IsRunning, Is.EqualTo(false));

            // When started, it's running
            t.Play();
            Assert.That(t.IsRunning, Is.EqualTo(true));

            // Await, then it's not running and results are ok
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(1));

            // Await again, it produces the same state: not running and same results
            await t.Await();
            await t.Await();
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(1));

            // If start + await again (the tween has been disposed) it will not run again, but at least it doesn't fail
            t.Play();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            // Data is not modified because the sequence is not executed
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(1));

            // If start + await again multiple times, it will not run again, but at least it doesn't fail
            t.Play();
            t.Play();
            t.Play();
            t.Play();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            await t.Await();
            await t.Await();
            await t.Await();
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            // Data is not modified because the sequence is not executed
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(1));
        }

        [Test(Description = "MultipleSequencePlayer await works, multiple executions. No disposed")]
        public async Task MultipleSequencePlayerAwait() {
            var l1 = 0;
            var l2 = 0;

            // when created, it's not running
            var t = new MultipleSequencePlayer()
                .WithParent(this, false)
                .CreateSequence()
                .Callback(() => l1++)
                .EndSequence()
                .AddOnFinishAll(() => l2++);
            Assert.That(t.IsRunning, Is.EqualTo(false));

            // When started, it's running
            t.Play();
            Assert.That(t.IsRunning, Is.EqualTo(true));

            // Await, then it's not running and results are ok
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(1));

            // Await again, it produces the same state: not running and same results
            await t.Await();
            await t.Await();
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(1));

            // If start + await again, it will work properly
            t.Play();
            Assert.That(t.IsRunning, Is.EqualTo(true));
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            Assert.That(l1, Is.EqualTo(2));
            Assert.That(l2, Is.EqualTo(2));

            // If start + await again multiple times, it will work properly
            t.Play();
            t.Play();
            t.Play();
            t.Play();
            Assert.That(t.IsRunning, Is.EqualTo(true));
            await t.Await();
            await t.Await();
            await t.Await();
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            Assert.That(l1, Is.EqualTo(3));
            Assert.That(l2, Is.EqualTo(3));
        }

        [Test(Description = "MultipleSequencePlayer await works, multiple executions. Auto disposed")]
        public async Task MultipleSequencePlayerAwaitDisposed() {
            var l1 = 0;
            var l2 = 0;

            // when created, it's not running
            var t = new MultipleSequencePlayer()
                .WithParent(this, true)
                .CreateSequence()
                .Callback(() => l1++)
                .EndSequence()
                .AddOnFinishAll(() => l2++);
            Assert.That(t.IsRunning, Is.EqualTo(false));

            // When started, it's running
            t.Play();
            Assert.That(t.IsRunning, Is.EqualTo(true));

            // Await, then it's not running and results are ok
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(1));

            // Await again, it produces the same state: not running and same results
            await t.Await();
            await t.Await();
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(1));

            // If start + await again, it will not be executed but at least it will not fail
            t.Play();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            // Data is not modified because the sequence is not executed
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(1));

            // If start + await again multiple times, it will not be executed but at least it will not fail
            t.Play();
            t.Play();
            t.Play();
            t.Play();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            await t.Await();
            await t.Await();
            await t.Await();
            await t.Await();
            Assert.That(t.IsRunning, Is.EqualTo(false));
            // Data is not modified because the sequence is not executed
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(1));
        }

        [Test(Description = "LoopStatus await, callbacks and an OnFinish")]
        public async Task LauncherTests() {
            var launcher = new Launcher().WithParent(this);
            var l1 = 0;
            var l2 = 0;
            var s1 = SequenceBuilder.Create().Callback(() => l1++);
            var s2 = SequenceBuilder.Create().Callback(() => l2++).SetLoops(2);
            var t1 = launcher.Play(s1, this);
            var t2 = launcher.Play(s2, this);
            var finished1 = 0;
            var finished2 = 0;

            t1.OnFinish(() => finished1++);
            t2.OnFinish(() => finished2++);

            Assert.That(finished1, Is.EqualTo(0));
            Assert.That(finished2, Is.EqualTo(0));

            Assert.That(t1.LoopCounter, Is.EqualTo(0));
            Assert.That(t2.LoopCounter, Is.EqualTo(0));

            await Task.WhenAll(t1.Await(), t2.Await());
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(2));
            Assert.That(finished1, Is.EqualTo(1));
            Assert.That(finished2, Is.EqualTo(1));
            Assert.That(t1.LoopCounter, Is.EqualTo(1));
            Assert.That(t2.LoopCounter, Is.EqualTo(2));

            // Everything remains the same
            t1.Start();
            t1.Start();
            t1.Start();
            await Task.WhenAll(t1.Await(), t2.Await());
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(2));
            Assert.That(finished1, Is.EqualTo(1));
            Assert.That(finished2, Is.EqualTo(1));
            Assert.That(t1.LoopCounter, Is.EqualTo(1));
            Assert.That(t2.LoopCounter, Is.EqualTo(2));
        }

        [Test(Description = "LoopStatus loops and callbacks")]
        public async Task PlayWithLoopsAndCallback() {
            var firstLoop = 0;

            const float pause = 0.1f;
            const int seq1Loops = 9;

            Stopwatch x = Stopwatch.StartNew();
            var loopStatus = await SequenceBuilder.Create()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(pause)
                .Callback(() => { firstLoop++; })
                .SetLoops(seq1Loops)
                .Play(await CreateTween(), this)
                .Await();

            Assert.That(firstLoop, Is.EqualTo(seq1Loops));
        }

        [Test(Description = "LoopStatus can finish infinite loops with the End() method")]
        [Ignore("It never ends")]
        public async Task LoopStatusFinishAction() {
            var firstLoop = 0;
            var finished = 0;

            const float pause = 0.1f;

            LoopStatus loopStatus = null;
            Stopwatch x = Stopwatch.StartNew();
            var looped = SequenceBuilder.Create()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(pause)
                .Callback(() => {
                    firstLoop++;
                    if (firstLoop == 200) {
                        loopStatus.End();
                    }
                })
                .SetInfiniteLoops()
                .Play(await CreateTween(), this);
            loopStatus = looped;
            loopStatus.OnFinish(() => finished++);
            await loopStatus.Await();
            Assert.That(firstLoop, Is.EqualTo(200));
        }

        [Test(Description = "LoopStatus can be finish infinite loops with the Finish() method")]
        public async Task LoopStatusFinishActionVersion2() {
            var firstLoop = 0;
            var finished = 0;

            const float pause = 0.1f;

            LoopStatus loopStatus = null;
            Stopwatch x = Stopwatch.StartNew();
            var sequence = SequenceBuilder.Create()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(pause)
                .Callback(() => {
                    firstLoop++;
                    if (firstLoop == 10) {
                        loopStatus.End();
                    }
                })
                .SetLoops(1);
            var looped = new Launcher()
                .WithParent(this)
                .PlayForever(sequence);

            loopStatus = looped;
            loopStatus.OnFinish(() => finished++);
            await loopStatus.Await();
            Assert.That(firstLoop, Is.EqualTo(10));
        }

        [Test(Description = "SingleSequence Loops and callbacks, using CreateSequence() builder")]
        public async Task SingleSequencePlayerWithLoopsAndCallbackWithBuilder() {
            var firstLoop = 0;
            var finished = 0;

            const float pause = 0.1f;
            const int seq1Loops = 9;

            await new SingleSequencePlayer()
                .WithParent(this, true)
                .CreateSequence()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(pause)
                .Callback(() => firstLoop++)
                .SetLoops(seq1Loops)
                .EndSequence()
                .AddOnFinishAll(() => finished++)
                .Play()
                .Await();

            Assert.That(firstLoop, Is.EqualTo(seq1Loops));
            Assert.That(finished, Is.EqualTo(1));
        }


        [Test(Description = "SingleSequence Loops and callbacks, using SequenceBuilder.Create() builder")]
        public async Task SingleSequencePlayerWithLoopsAndCallback() {
            var firstLoop = 0;
            var finished = 0;

            const float pause = 0.1f;
            const int seq1Loops = 9;

            var sequence = SequenceBuilder.Create()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(pause)
                .Callback(() => firstLoop++)
                .SetLoops(seq1Loops);

            await new SingleSequencePlayer()
                .WithParent(this, true)
                .WithSequence(sequence)
                .AddOnFinishAll(() => finished++)
                .Play()
                .Await();

            Assert.That(firstLoop, Is.EqualTo(seq1Loops));
            Assert.That(finished, Is.EqualTo(1));
        }

        [Test(Description = "SingleSequence Loops can be overriden by the player")]
        public async Task SingleSequencePlayerWithLoopsOverriden() {
            var firstLoop = 0;
            var finished = 0;

            const float pause = 0.1f;
            const int seq1Loops = 9;

            const float estimatedDuration = (seq1Loops * pause);

            Stopwatch x = Stopwatch.StartNew();
            await new SingleSequencePlayer()
                .WithParent(this, true)
                .CreateSequence()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(pause)
                .Callback(() => firstLoop++)
                .EndSequence()
                .SetLoops(seq1Loops)
                .AddOnFinishAll(() => finished++)
                .Play()
                .Await();

            Console.WriteLine("It should take: " + estimatedDuration +
                              "s Elapsed time: " + (x.ElapsedMilliseconds / 1000f) + "s");

            Assert.That(firstLoop, Is.EqualTo(seq1Loops));
            Assert.That(finished, Is.EqualTo(1));
        }

        [Test(Description = "MultipleSequence with loops and callbacks")]
        public async Task MultipleSequencePlayerWithLoopsAndCallback() {
            var firstLoop = 0;
            var secondLoop = 0;
            var finished = 0;

            const float pause = 0.1f;
            const int seq1Loops = 9;
            const int seq2Loops = 5;
            const int playerLoops = 2;

            const float estimatedDuration = (seq1Loops * pause + seq2Loops * pause) * playerLoops;

            Stopwatch x = Stopwatch.StartNew();
            await new MultipleSequencePlayer()
                .WithParent(this, true)
                .CreateSequence()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(pause)
                .Callback(() => firstLoop++)
                .SetLoops(seq1Loops)
                .EndSequence()
                .CreateSequence()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(pause)
                .Callback(() => secondLoop++)
                .SetLoops(seq2Loops)
                .EndSequence()
                .SetLoops(playerLoops)
                .AddOnFinishAll(() => finished++)
                .Play()
                .Await();

            Console.WriteLine("It should take: " + estimatedDuration +
                              "s Elapsed time: " + (x.ElapsedMilliseconds / 1000f) + "s");

            Assert.That(firstLoop, Is.EqualTo(playerLoops * seq1Loops));
            Assert.That(secondLoop, Is.EqualTo(playerLoops * seq2Loops));
            Assert.That(finished, Is.EqualTo(1));
        }

        [Test(Description = "Stop and resume callbacks")]
        public async Task MultipleSequencePlayerCancelCallbacks() {
            Engine.TimeScale = 1;

            var callback = 0;
            var finished = 0;

            var tweenPlayer = new MultipleSequencePlayer()
                .WithParent(this, true)
                .CreateSequence()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(1f)
                .Callback(() => callback++)
                .EndSequence()
                .AddOnFinishAll(() => finished++)
                .Play();

            await Task.Delay(200);
            tweenPlayer.Stop();
            tweenPlayer.Stop();
            tweenPlayer.Stop();
            Assert.That(callback, Is.EqualTo(0));
            Assert.That(finished, Is.EqualTo(0));

            // If the player is stopped, the callback shouldn't execute
            await Task.Delay(1000);
            Assert.That(callback, Is.EqualTo(0));
            Assert.That(finished, Is.EqualTo(0));

            // Player resume, callbacks were executed
            tweenPlayer.Play();
            tweenPlayer.Play();
            tweenPlayer.Play();
            tweenPlayer.Play();
            await Task.Delay(900);
            Assert.That(callback, Is.EqualTo(1));
            Assert.That(finished, Is.EqualTo(1));
        }

        [Test(Description = "Test callbacks in steps")]
        public async Task CallbacksInSteps() {
            var callbackStep1 = 0;
            var callbackStep2 = 0;
            var callbackStep3 = 0;
            var callbackStep4 = 0;
            const int loops = 2;

            Sprite sprite = new Sprite();
            sprite.Position = new Vector2(100, 100);
            AddChild(sprite);
            await this.AwaitIdleFrame();

            await new MultipleSequencePlayer()
                .WithParent(this, true)
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
                .SetLoops(loops)
                .Play()
                .Await();

            Assert.That(callbackStep1, Is.EqualTo(loops * loops));
            Assert.That(callbackStep2, Is.EqualTo(loops * loops));
            Assert.That(callbackStep3, Is.EqualTo(loops * loops));
            Assert.That(callbackStep4, Is.EqualTo(loops * loops));
        }

        [Test(Description = "Test values in steps")]
        public void TweenSequenceSteps() {
            var sequence = SequenceBuilder.Create()
                .AnimateSteps(new Sprite(), Property.PositionX)
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
        public void TweenSequenceKeyframe() {
            var sequence = SequenceBuilder.Create()
                .AnimateKeys(new Sprite(), Property.PositionX)
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

        private T GetTweener<T>(ISequence sequence, int group, int pos = 0) where T : class {
            return sequence.TweenList.ToList()[group].ToList()[pos] as T;
        }
    }
}