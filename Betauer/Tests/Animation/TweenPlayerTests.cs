using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using NUnit.Framework;
using Betauer.Animation;
using Betauer.TestRunner;

namespace Betauer.Tests.Animation {
    [TestFixture]
    public class TweenPlayerTests : Node {
        [SetUp]
        public void SetUp() {
            Engine.TimeScale = 10;
        }

        [TearDown]
        public void TearDown() {
            Engine.TimeScale = 1;
        }

        [Test(Description = "SingleSequence Loops and callbacks, using CreateSequence() builder")]
        public async Task SingleSequencePlayerWithLoopsAndCallbackWithBuilder() {
            var firstLoop = 0;
            var finished = 0;

            const float pause = 0.1f;
            const int seq1Loops = 9;

            await new SingleSequencePlayer()
                .CreateNewTween(this)
                .CreateSequence()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(pause)
                .Callback(() => firstLoop++)
                .SetLoops(seq1Loops)
                .EndSequence()
                .AddOnFinishAll(delegate() { finished++; })
                .Start()
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
                .CreateNewTween(this)
                .WithSequence(sequence)
                .AddOnFinishAll(() => finished++)
                .Start()
                .Await();

            Assert.That(firstLoop, Is.EqualTo(seq1Loops));
            Assert.That(finished, Is.EqualTo(1));
        }

        [Test(Description = "SingleSequence Loops and callbacks, with Play")]
        public async Task PlayWithLoopsAndCallback() {
            var firstLoop = 0;

            const float pause = 0.1f;
            const int seq1Loops = 9;

            Stopwatch x = Stopwatch.StartNew();
            var sequence = await SequenceBuilder.Create()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(pause)
                .Callback(() => { firstLoop++; })
                .SetLoops(seq1Loops)
                .Play(this);

            Assert.That(firstLoop, Is.EqualTo(seq1Loops));
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
                .CreateNewTween(this)
                .CreateSequence()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(pause)
                .Callback(() => firstLoop++)
                .EndSequence()
                .SetLoops(seq1Loops)
                .AddOnFinishAll(() => finished++)
                .Start()
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
                .CreateNewTween(this)
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
                .SetFreeTweenOnFinish(true)
                .AddOnFinishAll(delegate() { finished++; })
                .Start()
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
                .CreateNewTween(this)
                .CreateSequence()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Pause(1f)
                .Callback(() => callback++)
                .EndSequence()
                .AddOnFinishAll(delegate() { finished++; })
                .Start();

            await Task.Delay(200);
            tweenPlayer.Stop();
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
                .CreateNewTween(this)
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
                .SetFreeTweenOnFinish(true)
                .SetLoops(loops)
                .Start()
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