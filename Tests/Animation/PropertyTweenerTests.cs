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

        [Test(Description = "step to")]
        public async Task TweenSequenceStepsTo() {
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var sprite = await CreateSprite();
            await TweenSequenceBuilder.Create()
                .AnimateSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .To(120, 0.1f, Easing.BackIn)
                .To(-90, 0.2f)
                .EndAnimate()
                .Play(sprite)
                .Await();

            Assert.That(sprite.Position.x, Is.EqualTo(-90));

            AssertStep(steps[0], 100f, 120f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 120f, -90f, 0.1f, 0.2f, Easing.LinearInOut);
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

            Assert.That(sprite.Position.x, Is.EqualTo(-90));

            AssertStep(steps[0], 80f, 120f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 120f, -90f, 0.1f, 0.2f, Easing.LinearInOut);
        }

        [Test(Description = "step offset")]
        public async Task TweenSequenceStepsOffset() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .Offset(120, 0.1f, Easing.BackIn)
                .Offset(-90, 0.2f)
                .EndAnimate()
                .Play(sprite)
                .Await();

            Assert.That(sprite.Position.x, Is.EqualTo(130));

            AssertStep(steps[0], 100f, 220f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 220f, 130f, 0.1f, 0.2f, Easing.LinearInOut);
        }

        [Test(Description = "step offset with from")]
        public async Task TweenSequenceStepsOffsetWithFrom() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await TweenSequenceBuilder.Create()
                .AnimateSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .From(80)
                .Offset(120, 0.1f, Easing.BackIn)
                .Offset(-90, 0.2f)
                .EndAnimate()
                .Play(sprite)
                .Await();

            Assert.That(sprite.Position.x, Is.EqualTo(110));
            AssertStep(steps[0], 80f, 200f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 200f, 110f, 0.1f, 0.2f, Easing.LinearInOut);
        }

        private static void AssertStep<T>(DebugStep<T> step, T from, T to, float start, float duration, Easing easing) {
            Assert.That(step.From, Is.EqualTo(from));
            Assert.That(step.To, Is.EqualTo(to));
            Assert.That(step.Start, Is.EqualTo(start));
            Assert.That(step.Duration, Is.EqualTo(duration));
            Assert.That(step.Easing, Is.EqualTo(easing));
        }
    }
}