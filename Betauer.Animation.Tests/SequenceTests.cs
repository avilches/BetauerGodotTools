using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Animation.Easing;
using Betauer.Animation.Tween;
using Betauer.Signal;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Animation.Tests {
    [TestFixture]
    public class SequenceTests : NodeTest {
        [SetUp]
        public void SetUp() {
            Engine.TimeScale = 10;
        }

        [TearDown]
        public void TearDown() {
            Engine.TimeScale = 1;
        }

        [Test(Description = "Test OnAction method is executed immediately even with initialDelay and loops")]
        public async Task OnStart() {

            var sprite = await CreateSprite();
            var x = 0;
            
            var t = SequenceAnimation.Create(sprite)
                .Pause(1f)
                .OnStart(target => x ++)
                .SetLoops(2)
                .AnimateSteps(Property.Opacity)
                .To(12, 0.1f, Easings.BackIn)
                .To(12, 0.1f, Easings.BackIn)
                .EndAnimate()
                .Play(initialDelay: 1f); // No await!
            
            Assert.That(x, Is.EqualTo(1));
        }

        [Test(Description = "Sequence await works, multiple executions, Loops, no disposed")]
        public async Task SequenceMultipleExecutions() {
            var x = 0;

            // when created, it's not running
            var t = SequenceAnimation.Create(this)
                .SetSpeed(4)
                .SetProcessMode(Godot.Tween.TweenProcessMode.Physics)
                .SetLoops(2)
                .Callback(() => x++);

            // When started, it's running
            SceneTreeTween sceneTreeTween = t.Play();
            // TODO: can't assert for ProcessMode or SpeedScale
            Assert.That(sceneTreeTween.IsRunning(), Is.True);

            // Await, then it's not running and results are ok
            await sceneTreeTween.AwaitFinished();
            Assert.That(sceneTreeTween.IsRunning(), Is.False);
            Assert.That(x, Is.EqualTo(2));

            // If start + await again, it will work properly
            sceneTreeTween = t.Play();
            Assert.That(sceneTreeTween.IsRunning(), Is.True);
            await sceneTreeTween.AwaitFinished();
            Assert.That(sceneTreeTween.IsRunning(), Is.False);
            Assert.That(x, Is.EqualTo(4));

        }

        [Test(Description = "Error if sequence is empty")]
        public async Task SequenceEmptyTests() {
            var sprite = await CreateSprite();
            var ke = Assert.ThrowsAsync<Exception>(async () => await KeyframeAnimation.Create(sprite)
                .SetDuration(1)
                .Play()
                .AwaitFinished());
            Assert.That(ke.Message, Contains.Substring("Can't start a keyframe animation without animations"));

            var se = Assert.ThrowsAsync<Exception>(async () => await SequenceAnimation.Create(sprite)
                .Play()
                .AwaitFinished());
            Assert.That(se.Message, Contains.Substring("Can't start a sequence without animations"));
        }

        [Test(Description = "Error if keyframe animations without keyframes")]
        public async Task KeyframeSequenceWithoutAnimationTests() {
            var sprite = await CreateSprite();
            var kea = Assert.Throws<Exception>(() => KeyframeAnimation.Create(sprite)
                .SetDuration(1)
                .AnimateKeys(Property.Modulate)
                .EndAnimate());
            Assert.That(kea.Message, Contains.Substring("Animation without absolute keyframes"));

            var seao = Assert.Throws<Exception>(() => KeyframeAnimation.Create(sprite)
                .SetDuration(1)
                .AnimateKeysBy(Property.Modulate)
                .EndAnimate());
            Assert.That(seao.Message, Contains.Substring("Animation without offset keyframes"));
        }

        [Test(Description = "Error if step animations without steps")]
        public async Task StepSequenceWithoutAnimationTests() {
            var sprite = await CreateSprite();
            var seaa = Assert.Throws<Exception>(() => SequenceAnimation.Create(sprite)
                .AnimateSteps(Property.Modulate)
                .EndAnimate());
            Assert.That(seaa.Message, Contains.Substring("Animation without steps"));

            var seab = Assert.Throws<Exception>(() => SequenceAnimation.Create(sprite)
                .AnimateStepsBy(Property.Modulate)
                .EndAnimate());
            Assert.That(seab.Message, Contains.Substring("Animation without steps"));
        }

        [Test(Description = "Keyframe without duration fails")]
        public async Task AnimationKeyMustHaveDurationTests() {
            var sprite = await CreateSprite();
            var e = Assert.ThrowsAsync<Exception>(async () => await KeyframeAnimation.Create(sprite)
                .AnimateKeys(Property.PositionX)
                .From(80).KeyframeTo(1f, 100)
                .EndAnimate()
                .Play()
                .AwaitFinished());
            Assert.That(e.Message, Contains.Substring("Keyframe animation duration should be more than 0"));
        }

        [Test(Description = "Keyframe duration with SetDuration() + parallel")]
        public async Task KeyframeParallelDurationTests() {
            List<DebugStep<float>> steps1 = new List<DebugStep<float>>();
            List<DebugStep<float>> steps2 = new List<DebugStep<float>>();
            var sprite = await CreateSprite();
            await KeyframeAnimation.Create(sprite)
                .SetDuration(2f)
                .AnimateKeys(Property.PositionX)
                .SetDebugSteps(steps1)
                .From(80)
                .KeyframeTo(0.6f, 100)
                .KeyframeTo(1f, 200)
                .EndAnimate()
                .AnimateKeys(Property.PositionY)
                .SetDebugSteps(steps2)
                .From(80)
                .KeyframeTo(0.5f, 100)
                .KeyframeTo(1f, 200)
                .EndAnimate()
                .Play()
                .AwaitFinished();

            AssertStepTime(steps1[0], 0f, 1.2f);
            AssertStepTime(steps1[1],  1.2f, 0.8f);

            // Assert second animation is in parallel with the same duration
            AssertStepTime(steps2[0], 0f, 1f);
            AssertStepTime(steps2[1], 1f, 1f);
        }

        [Test(Description = "Keyframe duration with Play(). All animations are parallel")]
        public async Task KeyframeParallelDurationInPlayAndInitialDelayTests() {
            List<DebugStep<float>> steps1 = new List<DebugStep<float>>();
            List<DebugStep<float>> steps2 = new List<DebugStep<float>>();
            var sprite = await CreateSprite();
            await KeyframeAnimation.Create()
                .AnimateKeys(Property.PositionX)
                .SetDebugSteps(steps1)
                .From(80)
                .KeyframeTo(0.6f, 100)
                .KeyframeTo(1f, 200)
                .EndAnimate()
                .AnimateKeys(Property.PositionY)
                .SetDebugSteps(steps2)
                .From(80)
                .KeyframeTo(0.5f, 100)
                .KeyframeTo(1f, 200)
                .EndAnimate()
                .Play(sprite, initialDelay: 1, duration: 2f)
                .AwaitFinished();

            AssertStepTime(steps1[0], 1f, 1.2f);
            AssertStepTime(steps1[1],  2.2f, 0.8f);

            // Assert second animation is in parallel with the same duration
            AssertStepTime(steps2[0], 1f, 1f);
            AssertStepTime(steps2[1], 2f, 1f);
        }

        [Test(Description = "Keyframe multiple nodes")]
        public async Task KeyframeMultipleNodesTests() {
            List<DebugStep<float>> steps1 = new List<DebugStep<float>>();
            var sprite1 = await CreateSprite();
            var sprite2 = await CreateSprite();
            await KeyframeAnimation.Create()
                .AnimateKeys(Property.PositionX)
                .SetDebugSteps(steps1)
                .From(80)
                .KeyframeTo(0.6f, 100)
                .KeyframeTo(1f, 200)
                .EndAnimate()
                .Play(new [] { sprite1, sprite2 }, delayBetweenNodes:0.5f, initialDelay: 1, duration: 2f)
                .AwaitFinished();

            AssertStepTime(steps1[0], 1f, 1.2f);
            AssertStepTime(steps1[1],  2.2f, 0.8f);

            // Second run uses the same steps... only difference the delayBetweeenNodes
            AssertStepTime(steps1[2], 1.5f, 1.2f);
            AssertStepTime(steps1[3],  2.7f, 0.8f);

        }

        [Test(Description = "Sequence multiple nodes")]
        public async Task SequenceMultipleNodesTests() {
            List<DebugStep<float>> steps1 = new List<DebugStep<float>>();
            var sprite1 = await CreateSprite();
            var sprite2 = await CreateSprite();
            await SequenceAnimation.Create()
                .AnimateSteps(Property.PositionX)
                .SetDebugSteps(steps1)
                .From(80)
                .To(100, 1.2f)
                .To(200, 0.8f)
                .EndAnimate()
                .Play(new [] { sprite1, sprite2 }, delayBetweenNodes:0.5f, initialDelay: 1)
                .AwaitFinished();

            AssertStepTime(steps1[0], 1f, 1.2f);
            AssertStepTime(steps1[1],  2.2f, 0.8f);

            // Second run uses the same steps... only difference the delayBetweeenNodes
            AssertStepTime(steps1[2], 1.5f, 1.2f);
            AssertStepTime(steps1[3],  2.7f, 0.8f);

        }

        [Test(Description = "Sequence steps parallel and chain duration with initialDelay")]
        public async Task AnimateStepsChainParallelAndDelay() {
            var steps1 = new List<DebugStep<float>>();
            var steps2a = new List<DebugStep<float>>();
            var steps2b = new List<DebugStep<float>>();
            var steps2c = new List<DebugStep<float>>();
            var steps3 = new List<DebugStep<float>>();
            var sprite = await CreateSprite();
            await SequenceAnimation.Create(sprite)
                .AnimateSteps(Property.PositionX)
                .SetDebugSteps(steps1)
                .From(80).To(100, 0.6f)
                .EndAnimate()
                
                // No Parallel() by default, so next animation will be chained
                
                .AnimateRelativeSteps(Property.PositionX)
                .SetDebugSteps(steps2a)
                .From(80).Offset(0.5f, 0.4f)
                .EndAnimate()
                
                // Activate Parallel()
                
                .Parallel()
                .AnimateStepsBy(Property.PositionX)
                .SetDebugSteps(steps2b)
                .From(80).Offset(0.5f, 0.5f)
                .EndAnimate()
                
                // Next will be parallel too
                
                .AnimateRelativeSteps(Property.PositionX)
                .SetDebugSteps(steps2c)
                .From(80).Offset(0.5f, 0.4f)
                .EndAnimate()

                // Return to sequential
                
                .Chain()
                
                .AnimateSteps(Property.PositionX)
                .SetDebugSteps(steps3)
                .From(80).To(0.5f, 1.4f)
                .EndAnimate()
                
                .Play(1)
                .AwaitFinished();

            AssertStepTime(steps1[0], 1f, 0.6f);
            AssertStepTime(steps2a[0], 1.6f, 0.4f);
            AssertStepTime(steps2b[0], 1.6f, 0.5f);
            AssertStepTime(steps2c[0], 1.6f, 0.4f);
            AssertStepTime(steps3[0], 2.1f, 1.4f);

        }

        private static void AssertStepTime<T>(DebugStep<T> step, float start, float duration) {
            Assert.That(step.Start, Is.EqualTo(start));
            Assert.That(step.Duration, Is.EqualTo(duration).Within(0.0000001f));
        }

    }
}