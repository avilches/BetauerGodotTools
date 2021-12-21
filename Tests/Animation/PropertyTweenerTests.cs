using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using NUnit.Framework;
using Tools;
using Tools.Animation;
using Veronenger.Tests.Runner;

namespace Veronenger.Tests.Animation {
    [TestFixture]
    public class PropertyTests : Node {
        public async Task<Sprite> CreateSprite() {
            Sprite sprite = new Sprite();
            sprite.Position = new Vector2(100, 100);
            AddChild(sprite);
            await this.AwaitIdleFrame();
            return sprite;
        }

        /**
         * Step tests
         */
        [Test(Description = "step to")]
        public async Task TweenSequenceStepsTo() {
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var sprite = await CreateSprite();
            var executed1 = false;
            var executed2 = false;
            await TweenSequenceBuilder.Create()
                .AnimateSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .To(120, 0.1f, Easing.BackIn)
                // Add the same absolute step is allowed, it can be used as pause and as a callback
                .To(120, 0.1f, Easing.BackIn, (node) => executed1 = true)
                // Add an absolute with no time assign the from value for the next step and execute the callback
                .To(200, 0f, Easing.BackIn, (node) => executed2 = true)
                .To(-90, 0.2f)
                .EndAnimate()
                .Play(sprite)
                .Await();

            Assert.That(executed1);
            Assert.That(executed2);
            AssertStep(steps[0], 100f, 120f, 0f, 0.1f, Easing.BackIn);
            // There is a gap between 0.1 and 0.2 secs without tweens
            AssertStep(steps[1], 200f, -90f, 0.2f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(-90));
        }

        [Test(Description = "step to with from")]
        public async Task TweenSequenceStepsToWithFrom() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .From(80)
                .To(120, 0.1f, Easing.BackIn)
                .To(-90, 0.2f)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 80f, 120f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 120f, -90f, 0.1f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(-90));
        }

        [Test(Description = "step to with first 0 second value works like from")]
        public async Task TweenSequenceStepsToWithFirstStepTo0() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .From(1180)
                .To(80, 0f, Easing.BackIn)
                .To(120, 0.1f, Easing.BackIn)
                .To(-90, 0.2f)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 80f, 120f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 120f, -90f, 0.1f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(-90));
        }

        [Test(Description = "step offset")]
        public async Task TweenSequenceStepsOffset() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var executed1 = false;
            var executed2 = false;
            await TweenSequenceBuilder.Create()
                .AnimateStepsBy(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .Offset(120, 0.1f, Easing.BackIn)
                // Offset 0 and duration is like a pause with the callback is executed
                .Offset(0, 0.1f, Easing.BackIn, node => executed1 = true)
                .Offset(-90, 0.2f)
                // Offset 0 and duration 0 is ignored, but the callback is executed
                .Offset(0, 0f, Easing.BackIn, node => executed2 = true)
                .EndAnimate()
                .Play(sprite)
                .Await();

            Assert.That(executed1);
            Assert.That(executed2);
            AssertStep(steps[0], 100f, 220f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 220f, 130f, 0.2f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(130));
        }

        [Test(Description = "step offset with from")]
        public async Task TweenSequenceStepsOffsetWithFrom() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateStepsBy(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .From(80)
                .Offset(120, 0.1f, Easing.BackIn)
                .Offset(-90, 0.2f)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 80f, 200f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 200f, 110f, 0.1f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(110));
        }

        [Test(Description = "step offset with from and a 0s initial offset works like a from")]
        public async Task TweenSequenceStepsOffsetWithFromAnd0sOffset() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateStepsBy(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .From(80)
                .Offset(-30, 0f, Easing.BackIn)
                .Offset(120, 0.1f, Easing.BackIn)
                .Offset(-90, 0.2f)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 50f, 170, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 170f, 80f, 0.1f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(80));
        }

        [Test(Description = "step relative offset")]
        public async Task TweenSequenceStepsZeroOffset() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateRelativeSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .Offset(120, 0.1f, Easing.BackIn)
                .Offset(-90, 0.2f)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 100f, 220f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 220f, 10, 0.1f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(10));
        }

        [Test(Description = "step relative offset with from")]
        public async Task TweenSequenceStepsZeroOffsetWithFrom() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateRelativeSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .From(80)
                .Offset(120, 0.1f, Easing.BackIn)
                .Offset(-90, 0.2f)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 80f, 200f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 200f, -10, 0.1f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(-10));
        }

        [Test(Description = "step relative offset with from and duplicated offset")]
        public async Task TweenSequenceStepsZeroOffsetWithFromAndDuplicateOffset() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var executed1 = false;
            var executed2 = false;

            await TweenSequenceBuilder.Create()
                .AnimateRelativeSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .From(80)
                .Offset(120, 0.1f, Easing.BackIn)
                // This value is ignored because same offset means no movement, but it works like a pause
                .Offset(120, 0.1f, Easing.BackIn, (node => executed1 = true))
                .Offset(120, 0f, Easing.BackIn, (node => executed2 = true))
                .Offset(-90, 0.2f)
                .EndAnimate()
                .Play(sprite)
                .Await();

            Assert.That(executed1);
            Assert.That(executed2);
            AssertStep(steps[0], 80f, 200f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 200f, -10, 0.2f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(-10));
        }

        /**
         * Keyframe tests
         */
        [Test(Description = "keyframe to")]
        public async Task TweenSequenceKeysTo() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateKeys(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .KeyframeTo(0.5f, 120, Easing.BackIn)
                .KeyframeTo(0.8f, -90)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 100f, 120f, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 120f, -90f, 1f, 0.6f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(-90));
        }

        [Test(Description = "keyframe to with from")]
        public async Task TweenSequenceKeysToWithFrom() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateKeys(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .From(20)
                .KeyframeTo(0.5f, 120, Easing.BackIn)
                .KeyframeTo(0.8f, -90)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 20f, 120f, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 120f, -90f, 1f, 0.6f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(-90));
        }

        [Test(Description = "keyframe to with key 0 (same as from)")]
        public async Task TweenSequenceKeysToWithKey0() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateKeys(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .From(20) // from is overriden with the Keyframe 0
                .KeyframeTo(0f, 30, Easing.CubicOut) // easing is ignored
                .KeyframeTo(0.5f, 120, Easing.BackIn)
                .KeyframeTo(0.8f, -90)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 30f, 120f, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 120f, -90f, 1f, 0.6f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(-90));
        }


        [Test(Description = "keyframe offset")]
        public async Task TweenSequenceKeysOffset() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateKeysBy(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .KeyframeOffset(0.5f, 120, Easing.BackIn)
                .KeyframeOffset(0.8f, -90)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 100f, 220f, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 220f, 130f, 1f, 0.6f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(130));
        }

        [Test(Description = "keyframe offset with from")]
        public async Task TweenSequenceKeysOffsetWithFrom() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateKeysBy(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .From(80)
                .KeyframeOffset(0.5f, 120, Easing.BackIn)
                .KeyframeOffset(0.8f, -90)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 80, 200, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 200f, 110f, 1f, 0.6f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(110));
        }

        [Test(Description = "keyframe offset with key 0 (similar as from but adding the offset)")]
        public async Task TweenSequenceKeysOffsetWithKey0() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateKeysBy(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .From(30) // from is overriden with the Keyframe 0
                .KeyframeOffset(0f, 80, Easing.CubicOut) // easing is ignored
                .KeyframeOffset(0.5f, 120, Easing.BackIn)
                .KeyframeOffset(0.8f, -90)
                .KeyframeOffset(1f, 0)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 110, 230, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 230f, 140f, 1f, 0.6f, Easing.LinearInOut);
            Assert.That(sprite.Position.x, Is.EqualTo(140));

            Assert.That(steps.Count, Is.EqualTo(2)); // Last offset 0 is ignored
        }

        [Test(Description = "keyframe relative offset")]
        public async Task TweenSequenceKeysRelativeOffset() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateRelativeKeys(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .KeyframeOffset(0.5f, 120, Easing.BackIn)
                .KeyframeOffset(0.8f, -90)
                .KeyframeOffset(1f, 0)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 100f, 220f, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 220f, 10f, 1f, 0.6f, Easing.LinearInOut);
            AssertStep(steps[2], 10f, 100f, 1.6f, 0.4f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(100)); // returns to the original value
        }

        [Test(Description = "keyframe relative offset with from")]
        public async Task TweenSequenceKeysRelativeOffsetWithFrom() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateRelativeKeys(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .From(80)
                .KeyframeOffset(0.5f, 120, Easing.BackIn)
                .KeyframeOffset(0.8f, -90)
                .KeyframeOffset(1f, 0)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 80, 200, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 200f, -10f, 1f, 0.6f, Easing.LinearInOut);
            AssertStep(steps[2], -10f, 80, 1.6f, 0.4f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(80)); // returns to the original value
        }

        [Test(Description = "keyframe relative offset with key 0 (same as from)")]
        public async Task TweenSequenceKeysRelativeOffsetWithKey0() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateRelativeKeys(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .From(80) // from is overriden with the Keyframe 0
                // .KeyframeOffset(0f, 80, Easing.CubicOut) // easing is ignored
                .KeyframeOffset(0.5f, 120, Easing.BackIn)
                .KeyframeOffset(0.8f, -90)
                .KeyframeOffset(1f, 0)
                .EndAnimate()
                .Play(sprite)
                .Await();

            AssertStep(steps[0], 80, 200, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 200f, -10f, 1f, 0.6f, Easing.LinearInOut);
            AssertStep(steps[2], -10f, 80, 1.6f, 0.4f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(80)); // returns to the original value
        }

        private static void AssertStep<T>(DebugStep<T> step, T from, T to, float start, float duration, Easing easing) {
            Assert.That(step.From, Is.EqualTo(from).Within(0.0000001f));
            Assert.That(step.To, Is.EqualTo(to).Within(0.0000001f));
            Assert.That(step.Start, Is.EqualTo(start));
            Assert.That(step.Duration, Is.EqualTo(duration).Within(0.0000001f));
            Assert.That(step.Easing, Is.EqualTo(easing));
        }
    }
}