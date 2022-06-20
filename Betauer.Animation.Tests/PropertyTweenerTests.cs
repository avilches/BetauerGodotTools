using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Godot;
using NUnit.Framework;
using Betauer.Animation;
using Betauer.Animation.Tests;
using Betauer.TestRunner;
using Vector2 = Godot.Vector2;

namespace Betauer.Animation.Tests {
    [TestFixture]
    public class PropertyTweenerTests : NodeTest {
        [SetUp]
        public void SetUp() {
            Engine.TimeScale = 10;
        }

        [TearDown]
        public void TearDown() {
            Engine.TimeScale = 1;
        }

        [Test]
        public async Task SequenceLoops() {
            var x = 0;
            SequenceBuilder sequence = SequenceBuilder.Create()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Callback(() => x++);

            await sequence.Play(await CreateTween(), this).Await();
            Assert.That(x, Is.EqualTo(1));

            await sequence.Play(await CreateTween(), 5, this).Await();
            Assert.That(x, Is.EqualTo(6));
        }

        private int _calls1 = 0;
        public void Method() {
            _calls1++;
        }

        private const int X = 1283;
        private const bool V = false;
        private const string S1 = "asdasd";
        private const string S2 = "alkjdlaskjd";
        private const string S3 = "123781";

        private int _calls2 = 0;
        public void Method(int x, bool v, string s1, string s2, string s3) {
            Assert.That(x, Is.EqualTo(X));
            Assert.That(v, Is.EqualTo(V));
            Assert.That(s1, Is.EqualTo(S1));
            Assert.That(s2, Is.EqualTo(S2));
            Assert.That(s3, Is.EqualTo(S3));
            _calls2++;
        }

        [Test(Description = "sequence empty should fail")]
        public async Task SequenceEmptyShouldFail() {
            var sprite = await CreateSprite();
            InvalidDataException e = Assert.Throws<InvalidDataException>(() =>
                SequenceBuilder.Create()
                    .AnimateSteps(sprite, Property.PositionX)
                    .EndAnimate());
            Assert.That(e.Message, Is.EqualTo("Animation without steps"));
        }

        [Test(Description = "Callback with method name")]
        public async Task MethodCallbackWithOverloadAndParameters() {
            SequenceBuilder sequence = SequenceBuilder.Create()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Callback(this, nameof(Method))
                .Callback(this, nameof(Method), 0, X,V,S1,S2,S3);

            await sequence.Play(await CreateTween(), this).Await();
            Assert.That(_calls1, Is.EqualTo(1));
            Assert.That(_calls2, Is.EqualTo(1));

            await sequence.Play(await CreateTween(), 5, this).Await();
            Assert.That(_calls1, Is.EqualTo(6));
            Assert.That(_calls2, Is.EqualTo(6));
        }

        [Test(Description = "Callback with lambda")]
        public async Task MethodCallback() {
            int called = 0;
            SequenceBuilder sequence = SequenceBuilder.Create()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .Callback(() => called++);

            await sequence.Play(await CreateTween(), this).Await();
            Assert.That(called, Is.EqualTo(1));

            await sequence.Play(await CreateTween(), 5, this).Await();
            Assert.That(called, Is.EqualTo(6));
        }

        /**
         * DefaultTarget behaviour
         */
        [Test(Description = "Target is not defined by the animation or the sequence. It uses the player but the sequence is not changed")]
        public async Task TargetNotDefinedByAnimationOrSequence() {
            var spritePlayer = await CreateSprite();

            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var sequence = SequenceBuilder.Create()
                .AnimateSteps(null, Property.PositionX)
                .SetDebugSteps(steps)
                .To(120, 0.1f, Easing.BackIn)
                .To(-90, 0.2f)
                .EndAnimate()
                .AnimateSteps(null, Property.PositionY)
                .SetDebugSteps(steps)
                .To(120, 0.1f, Easing.BackIn)
                .To(-90, 0.2f)
                .EndAnimate();

            await new Launcher().WithParent(this).Play(sequence, spritePlayer).Await();
            Assert.That(sequence.DefaultTarget, Is.Null);
            Assert.That(steps[0].Target, Is.EqualTo(spritePlayer));
            Assert.That(steps[1].Target, Is.EqualTo(spritePlayer));
        }

        [Test(Description = "Target is not defined by the animation or the sequence. It should fail with multiple")]
        public async Task TargetNotDefinedByAnimationOrSequenceMultiple() {
            var spritePlayer = await CreateSprite();

            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var sequence = SequenceBuilder.Create()
                .AnimateSteps(null, Property.PositionX)
                .SetDebugSteps(steps)
                .To(120, 0.1f, Easing.BackIn)
                .To(-90, 0.2f)
                .EndAnimate();

            InvalidDataException e = Assert.ThrowsAsync<InvalidDataException>(async () =>
                await new MultipleSequencePlayer()
                    .WithParent(spritePlayer, true)
                    .AddSequence(sequence)
                    .Play()
                    .Await());
            Assert.That(e.Message, Is.EqualTo("No target defined for the animation"));
            Assert.That(sequence.DefaultTarget, Is.Null);
            Assert.That(steps.Count, Is.EqualTo(0));
        }

        [Test(Description = "Target is not defined by the animation or the sequence. It should fail with single")]
        public async Task TargetNotDefinedByAnimationOrSequenceSingle() {
            var spritePlayer = await CreateSprite();

            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var sequence = SequenceBuilder.Create()
                .AnimateSteps(null, Property.PositionX)
                .SetDebugSteps(steps)
                .To(120, 0.1f, Easing.BackIn)
                .To(-90, 0.2f)
                .EndAnimate();

            InvalidDataException e = Assert.ThrowsAsync<InvalidDataException>(async () =>
                await new SingleSequencePlayer()
                    .WithParent(spritePlayer, true)
                    .WithSequence(sequence)
                    .Play()
                    .Await());
            Assert.That(e.Message, Is.EqualTo("No target defined for the animation"));
            Assert.That(sequence.DefaultTarget, Is.Null);
            Assert.That(steps.Count, Is.EqualTo(0));
        }

        [Test(Description = "Target is defined by animation")]
        public async Task TargetDefinedByAnimation() {
            var spriteAnimation = await CreateSprite();
            var spriteSequence = await CreateSprite();
            var spritePlayer = await CreateSprite();

            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var sequence = SequenceBuilder.Create()
                .AnimateSteps(spriteAnimation, Property.PositionX)
                .SetDebugSteps(steps)
                .To(120, 0.1f, Easing.BackIn)
                .To(-90, 0.2f)
                .EndAnimate();

            await new Launcher().WithParent(this).Play(sequence, spritePlayer).Await();

            Assert.That(sequence.DefaultTarget, Is.Null);
            Assert.That(steps[0].Target, Is.EqualTo(spriteAnimation));
        }

        [Test(Description = "Target not defined by animation, it's defined by sequence")]
        public async Task TargetDefinedBySequence() {
            var spriteAnimation = await CreateSprite();
            var spriteSequence = await CreateSprite();
            var spritePlayer = await CreateSprite();

            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var sequence = SequenceBuilder.Create()
                .SetDefaultTarget(spriteSequence)
                .AnimateSteps(null, Property.PositionX)
                .SetDebugSteps(steps)
                .To(120, 0.1f, Easing.BackIn)
                .To(-90, 0.2f)
                .EndAnimate();

            await new Launcher().WithParent(this).Play(sequence, spritePlayer).Await();
            Assert.That(sequence.DefaultTarget, Is.EqualTo(spriteSequence));
            Assert.That(steps[0].Target, Is.EqualTo(spriteSequence));
        }


        [Test(Description = "Template doesn't have target, so it takes the target from the single player")]
        public async Task TargetNotDefinedBySequenceUsingTemplate() {
            var spritePlayer = await CreateSprite();

            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var template = TemplateBuilder.Create()
                .AnimateSteps(Property.PositionX)
                .SetDebugSteps(steps)
                .To(120, 0.1f, Easing.BackIn)
                .To(-90, 0.2f)
                .EndAnimate()
                .BuildTemplate();

            await new Launcher().WithParent(this).Play(template, spritePlayer).Await();
            Assert.That(template.DefaultTarget, Is.Null); // the sequence is cloned inside with the spritePlayer
            Assert.That(steps[0].Target, Is.EqualTo(spritePlayer));
        }

        [Test(Description =
            "Template doesn't have target, so it takes the target from the player with a multiple player")]
        public async Task TargetNotDefinedBySequenceUsingTemplateWithMultiple() {
            var spritePlayer = await CreateSprite();

            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var template = TemplateBuilder.Create()
                .AnimateSteps(Property.PositionX)
                .SetDebugSteps(steps)
                .To(120, 0.1f, Easing.BackIn)
                .To(-90, 0.2f)
                .EndAnimate()
                .BuildTemplate();

            await new MultipleSequencePlayer()
                .WithParent(spritePlayer, true)
                .ImportTemplate(template, spritePlayer).EndSequence()
                .Play()
                .Await();

            Assert.That(template.DefaultTarget, Is.Null); // the sequence is cloned inside with the spritePlayer
            Assert.That(steps[0].Target, Is.EqualTo(spritePlayer));
        }

        /**
         * Step animation tests
         */
        [Test(Description = "step to")]
        public async Task SequenceStepsTo() {
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var sprite = await CreateSprite();
            var executed1 = false;
            var executed2 = false;
            await SequenceBuilder.Create()
                .AnimateSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .To(120, 0.1f, Easing.BackIn)
                // Add the same absolute step is allowed, it can be used as pause and as a callback
                .To(120, 0.1f, Easing.BackIn, (node) => executed1 = true)
                // Add an absolute with no time assign the from value for the next step and execute the callback
                .To(200, 0f, Easing.BackIn, (node) => executed2 = true)
                .To(-90, 0.2f)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
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
        public async Task SequenceStepsToWithFrom() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .From(80)
                .To(120, 0.1f, Easing.BackIn)
                .To(-90, 0.2f)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 80f, 120f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 120f, -90f, 0.1f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(-90));
        }

        [Test(Description = "step to with 0 seconds in the first step, it works like the first value was the from")]
        public async Task SequenceStepsToWithFirstStepTo0() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .From(1180)
                .To(80, 0f, Easing.BackIn)
                .To(120, 0.1f, Easing.BackIn)
                .To(-90, 0.2f)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 80f, 120f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 120f, -90f, 0.1f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(-90));
        }

        /**
         * Step offset tests
         */
        [Test(Description = "step offset")]
        public async Task SequenceStepsOffset() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var executed1 = false;
            var executed2 = false;
            await SequenceBuilder.Create()
                .AnimateStepsBy(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .Offset(120, 0.1f, Easing.BackIn)
                // Offset 0 and duration is like a pause with the callback is executed
                .Offset(0, 0.1f, Easing.BackIn, node => executed1 = true)
                .Offset(-90, 0.2f)
                // Offset 0 and duration 0 is ignored, but the callback is executed
                .Offset(0, 0f, Easing.BackIn, node => executed2 = true)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();


            Assert.That(executed1);
            Assert.That(executed2);
            AssertStep(steps[0], 100f, 220f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 220f, 130f, 0.2f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(130));
        }

        [Test(Description = "step offset with from")]
        public async Task SequenceStepsOffsetWithFrom() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateStepsBy(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .From(80)
                .Offset(120, 0.1f, Easing.BackIn)
                .Offset(-90, 0.2f)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 80f, 200f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 200f, 110f, 0.1f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(110));
        }

        [Test(Description = "step offset with from and a 0s initial offset works like a from")]
        public async Task SequenceStepsOffsetWithFromAnd0sOffset() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateStepsBy(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .From(80)
                .Offset(-30, 0f, Easing.BackIn)
                .Offset(120, 0.1f, Easing.BackIn)
                .Offset(-90, 0.2f)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 50f, 170, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 170f, 80f, 0.1f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(80));
        }

        /**
         * Step relative offset tests
         */
        [Test(Description = "step relative offset")]
        public async Task SequenceStepsZeroOffset() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateRelativeSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .Offset(120, 0.1f, Easing.BackIn)
                .Offset(-90, 0.2f)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 100f, 220f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 220f, 10, 0.1f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(10));
        }

        [Test(Description = "step relative offset with from")]
        public async Task SequenceStepsZeroOffsetWithFrom() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateRelativeSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .From(80)
                .Offset(120, 0.1f, Easing.BackIn)
                .Offset(-90, 0.2f)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 80f, 200f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 200f, -10, 0.1f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(-10));
        }

        [Test(Description = "step relative offset with from and duplicated offset")]
        public async Task SequenceStepsZeroOffsetWithFromAndDuplicateOffset() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var executed1 = false;
            var executed2 = false;

            await SequenceBuilder.Create()
                .AnimateRelativeSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .From(80)
                .Offset(120, 0.1f, Easing.BackIn)
                // This value is ignored because same offset means no movement, but it works like a pause
                .Offset(120, 0.1f, Easing.BackIn, (node => executed1 = true))
                .Offset(120, 0f, Easing.BackIn, (node => executed2 = true))
                .Offset(-90, 0.2f)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            Assert.That(executed1);
            Assert.That(executed2);
            AssertStep(steps[0], 80f, 200f, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], 200f, -10, 0.2f, 0.2f, Easing.LinearInOut);
            Assert.That(steps.Count, Is.EqualTo(2));

            Assert.That(sprite.Position.x, Is.EqualTo(-10));
        }

        /**
         * Keyframe to tests
         */
        [Test(Description = "keyframe to")]
        public async Task SequenceKeysTo() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateKeys(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .KeyframeTo(0.5f, 120, Easing.BackIn)
                .KeyframeTo(0.8f, -90)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 100f, 120f, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 120f, -90f, 1f, 0.6f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(-90));
        }

        [Test(Description = "keyframe to with from")]
        public async Task SequenceKeysToWithFrom() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateKeys(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .From(20)
                .KeyframeTo(0.5f, 120, Easing.BackIn)
                .KeyframeTo(0.8f, -90)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 20f, 120f, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 120f, -90f, 1f, 0.6f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(-90));
        }

        [Test(Description = "keyframe to with from where the first and second keyframe are equals")]
        /*
         * In this case, a frame with 0 duration must be created to ensure the frame 0 has the initial value set
         * at the beginning of the animation
         */
        public async Task SequenceKeysToWithFrom2() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateKeys(sprite, Property.PositionX)
                .Duration(1)
                .SetDebugSteps(steps)
                .From(0.7f)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1f)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 0.7f, 0.7f, 0f, 0f, Easing.LinearInOut);
            AssertStep(steps[1], 0.7f, 1, 0.8f, 0.2f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(1f));
        }

        [Test(Description = "keyframe to with key 0 (same as from)")]
        public async Task SequenceKeysToWithKey0() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateKeys(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .From(20) // from is overriden with the Keyframe 0
                .KeyframeTo(0f, 30, Easing.CubicOut) // easing is ignored
                .KeyframeTo(0.5f, 120, Easing.BackIn)
                .KeyframeTo(0.8f, -90)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 30f, 30f, 0f, 0f, Easing.CubicOut);
            AssertStep(steps[1], 30f, 120f, 0f, 1f, Easing.BackIn);
            AssertStep(steps[2], 120f, -90f, 1f, 0.6f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(-90));
        }

        /**
         * Keyframe offset tests
         */
        [Test(Description = "keyframe offset")]
        public async Task SequenceKeysOffset() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateKeysBy(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .KeyframeOffset(0.5f, 120, Easing.BackIn)
                .KeyframeOffset(0.8f, -90)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 100f, 220f, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 220f, 130f, 1f, 0.6f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(130));
        }

        [Test(Description = "keyframe offset with from")]
        public async Task SequenceKeysOffsetWithFrom() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateKeysBy(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .From(80)
                .KeyframeOffset(0.5f, 120, Easing.BackIn)
                .KeyframeOffset(0.8f, -90)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 80, 200, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 200f, 110f, 1f, 0.6f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(110));
        }

        [Test(Description = "keyframe offset with key 0 (similar as from but adding the offset)")]
        public async Task SequenceKeysOffsetWithKey0() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateKeysBy(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .From(30) // from is overriden with the Keyframe 0
                .KeyframeOffset(0f, 80, Easing.CubicOut) // easing is ignored
                .KeyframeOffset(0.5f, 120, Easing.BackIn)
                .KeyframeOffset(0.8f, -90)
                .KeyframeOffset(1f, 0)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 110, 110f, 0f, 0f, Easing.CubicOut);
            AssertStep(steps[1], 110, 230, 0f, 1f, Easing.BackIn);
            AssertStep(steps[2], 230f, 140f, 1f, 0.6f, Easing.LinearInOut);
            Assert.That(sprite.Position.x, Is.EqualTo(140));

            Assert.That(steps.Count, Is.EqualTo(3)); // Last offset 0 is ignored
        }

        /**
         * Keyframe relative offset tests
         */
        [Test(Description = "keyframe relative offset")]
        public async Task SequenceKeysRelativeOffset() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateRelativeKeys(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .KeyframeOffset(0.5f, 120, Easing.BackIn)
                .KeyframeOffset(0.8f, -90)
                .KeyframeOffset(1f, 0)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 100f, 220f, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 220f, 10f, 1f, 0.6f, Easing.LinearInOut);
            AssertStep(steps[2], 10f, 100f, 1.6f, 0.4f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(100)); // returns to the original value
        }

        [Test(Description = "keyframe relative offset with from")]
        public async Task SequenceKeysRelativeOffsetWithFrom() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateRelativeKeys(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .From(80)
                .KeyframeOffset(0.5f, 120, Easing.BackIn)
                .KeyframeOffset(0.8f, -90)
                .KeyframeOffset(1f, 0)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 80, 200, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 200f, -10f, 1f, 0.6f, Easing.LinearInOut);
            AssertStep(steps[2], -10f, 80, 1.6f, 0.4f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(80)); // returns to the original value
        }

        [Test(Description = "keyframe relative offset with key 0 (same as from)")]
        public async Task SequenceKeysRelativeOffsetWithKey0() {
            var sprite = await CreateSprite();
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            await SequenceBuilder.Create()
                .AnimateRelativeKeys(sprite, Property.PositionX)
                .Duration(2)
                .SetDebugSteps(steps)
                .From(80) // from is overriden with the Keyframe 0
                // .KeyframeOffset(0f, 80, Easing.CubicOut) // easing is ignored
                .KeyframeOffset(0.5f, 120, Easing.BackIn)
                .KeyframeOffset(0.8f, -90)
                .KeyframeOffset(1f, 0)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            AssertStep(steps[0], 80, 200, 0f, 1f, Easing.BackIn);
            AssertStep(steps[1], 200f, -10f, 1f, 0.6f, Easing.LinearInOut);
            AssertStep(steps[2], -10f, 80, 1.6f, 0.4f, Easing.LinearInOut);

            Assert.That(sprite.Position.x, Is.EqualTo(80)); // returns to the original value
        }

        /**
         * Callbacks
         */
        [Test(Description = "with bezier curve using the SetVale as a callback tween")]
        public async Task SequenceStepsToWithBezier() {
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var sprite = await CreateSprite();
            await SequenceBuilder.Create()
                .AnimateSteps(sprite, Property.PositionX)
                .SetDebugSteps(steps)
                .To(120, 0.1f, BezierCurve.Create(Vector2.One, Vector2.One))
                // Add the same absolute step is allowed, it can be used as pause and as a callback
                .To(-90, 0.2f)
                .EndAnimate()
                .Play(await CreateTween(), sprite)
                .Await();

            Assert.That(sprite.Position.x, Is.EqualTo(-90));
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