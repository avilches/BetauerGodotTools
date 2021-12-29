using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using NUnit.Framework;
using Betauer.Animation;
using Betauer.TestRunner;
using Vector2 = Godot.Vector2;

namespace Betauer.Tests.Animation {
    [TestFixture]
    public class PropertyTests : Node {
        [SetUp]
        public void SetUp() {
            Engine.TimeScale = 10;
        }

        [TearDown]
        public void TearDown() {
            Engine.TimeScale = 1;
        }

        public async Task<Sprite> CreateSprite(int width = 100) {
            Sprite sprite = new Sprite();
            sprite.Position = new Vector2(100, 100);
            // var gradientTexture = new GradientTexture();
            var imageTexture = new ImageTexture();
            imageTexture.SetSizeOverride(new Vector2(width, width));
            sprite.Texture = imageTexture;
            AddChild(sprite);
            await this.AwaitIdleFrame();
            return sprite;
        }

        public async Task<Node2D> CreateNode2D() {
            Node2D node2D = new Node2D();
            node2D.Position = new Vector2(100, 100);
            AddChild(node2D);
            await this.AwaitIdleFrame();
            return node2D;
        }

        public async Task<Node> CreateNode() {
            Node node = new Node();
            AddChild(node);
            await this.AwaitIdleFrame();
            return node;
        }

        public async Task<Label> CreateLabel(int width = 100) {
            Label control = new Label();
            control.RectPosition = new Vector2(100, 100);
            control.RectSize = new Vector2(width, width);
            AddChild(control);
            await this.AwaitIdleFrame();
            return control;
        }

        [Test]
        public async Task InternalTest_CreateSpriteWithTextureSize() {
            var sprite = await CreateSprite(300);
            Assert.That(sprite.GetSpriteSize().x, Is.EqualTo(300));
        }

        [Test(Description = "Property Rotate")]
        public async Task TweenPropertyRotate() {
            foreach (var property in new [] {  Property.Rotate2D, Property.Rotate2DByCallback }) {
                const float from = 1f;
                const float to = 3f;
                var node2D = await CreateNode2D();
                node2D.RotationDegrees = 0f;
                await CreateTweenPropertyVariants(node2D, property, from, to);
                Assert.That(node2D.RotationDegrees, Is.EqualTo(to));

                var control = await CreateLabel();
                control.RectRotation = 0f;
                await CreateTweenPropertyVariants(control, property, from, to);
                Assert.That(control.RectRotation, Is.EqualTo(to));

                var node = await CreateNode();
                await CreateEmptyTweenPropertyVariants(node, property, from, to);
            }
        }

        [Test(Description = "Property PositionX, PositionY")]
        public async Task TweenPropertyPositionX_Y() {
            const float from = 90f;
            const float to = 120f;

            var node2D = await CreateNode2D();
            node2D.Position = Vector2.Zero;
            await CreateTweenPropertyVariants(node2D, Property.PositionX, from, to);
            Assert.That(node2D.Position.x, Is.EqualTo(to));

            node2D.Position = Vector2.Zero;
            await CreateTweenPropertyVariants(node2D, Property.PositionY, from, to);
            Assert.That(node2D.Position.y, Is.EqualTo(to));

            var control = await CreateLabel();
            control.RectPosition = Vector2.Zero;
            await CreateTweenPropertyVariants(control, Property.PositionX, from, to);
            Assert.That(control.RectPosition.x, Is.EqualTo(to));

            control.RectPosition = Vector2.Zero;
            await CreateTweenPropertyVariants(control, Property.PositionY, from, to);
            Assert.That(control.RectPosition.y, Is.EqualTo(to));

            var node = await CreateNode();
            await CreateEmptyTweenPropertyVariants(node, Property.PositionX, from, to);
            await CreateEmptyTweenPropertyVariants(node, Property.PositionY, from, to);
        }

        [Test(Description = "Property PositionBySizeX, PositionBySizeY")]
        public async Task TweenPropertyPositionPercentX_Y() {
            const float percentFrom = 0f;
            const float percentTo = 0.1f;
            const float initialPosition = 50f;
            const float endPosition = 120f;
            const int width = 300;

            var spriteX = await CreateSprite(width);
            spriteX.Position = new Vector2(initialPosition, 0);
            await TweenSequenceBuilder.Create()
                .AnimateSteps(spriteX, Property.PositionBySizeX)
                .From(percentFrom)
                .To(percentTo, 0.1f)
                .To(percentTo * 2, 0.1f)
                .EndAnimate()
                .Play(spriteX)
                .Await();
            Assert.That(spriteX.Position.x, Is.EqualTo(initialPosition + width * percentTo * 2));

            var spriteY = await CreateSprite(width);
            spriteY.Position = new Vector2(0, initialPosition);
            await TweenSequenceBuilder.Create()
                .AnimateSteps(spriteY, Property.PositionBySizeY)
                .From(percentFrom)
                .To(percentTo, 0.1f)
                .To(percentTo * 2, 0.1f)
                .EndAnimate()
                .Play(spriteY)
                .Await();
            Assert.That(spriteY.Position.y, Is.EqualTo(initialPosition + width * percentTo * 2));

            var sprite2D = await CreateSprite(width);
            sprite2D.Position = new Vector2(initialPosition, initialPosition);
            await TweenSequenceBuilder.Create()
                .AnimateSteps(sprite2D, Property.PositionBySize2D)
                .From(new Vector2(percentFrom, percentFrom))
                .To(new Vector2(percentTo, percentTo), 0.1f)
                .To(new Vector2(percentTo * 2, percentTo * 2), 0.1f)
                .EndAnimate()
                .Play(sprite2D)
                .Await();
            Assert.That(sprite2D.Position,
                Is.EqualTo(
                    new Vector2(initialPosition + width * percentTo * 2, initialPosition + width * percentTo * 2)));

            var controlX = await CreateLabel(width);
            controlX.RectPosition = new Vector2(initialPosition, 0);
            await TweenSequenceBuilder.Create()
                .AnimateSteps(controlX, Property.PositionBySizeX)
                .From(percentFrom)
                .To(percentTo, 0.1f)
                .To(percentTo * 2, 0.1f)
                .EndAnimate()
                .Play(controlX)
                .Await();
            Assert.That(controlX.RectPosition.x, Is.EqualTo(initialPosition + width * percentTo * 2));

            var controlY = await CreateLabel(width);
            controlY.RectPosition = new Vector2(0, initialPosition);
            await TweenSequenceBuilder.Create()
                .AnimateSteps(controlY, Property.PositionBySizeY)
                .From(percentFrom)
                .To(percentTo, 0.1f)
                .To(percentTo * 2, 0.1f)
                .EndAnimate()
                .Play(controlY)
                .Await();
            Assert.That(controlY.RectPosition.y, Is.EqualTo(initialPosition + width * percentTo * 2));

            var control2D = await CreateLabel(width);
            control2D.RectPosition = new Vector2(initialPosition, initialPosition);
            await TweenSequenceBuilder.Create()
                .AnimateSteps(control2D, Property.PositionBySize2D)
                .From(new Vector2(percentFrom, percentFrom))
                .To(new Vector2(percentTo, percentTo), 0.1f)
                .To(new Vector2(percentTo * 2, percentTo * 2), 0.1f)
                .EndAnimate()
                .Play(control2D)
                .Await();
            Assert.That(control2D.RectPosition, Is.EqualTo(
                new Vector2(initialPosition + width * percentTo * 2, initialPosition + width * percentTo * 2)));

            var node = await CreateNode();
            await CreateEmptyTweenPropertyVariants(node, Property.PositionBySizeX, percentFrom, percentTo);
            await CreateEmptyTweenPropertyVariants(node, Property.PositionBySizeY, percentFrom, percentTo);
            await CreateEmptyTweenPropertyVariants(node, Property.PositionBySize2D, Vector2.One, Vector2.Zero);

            var node2D = await CreateNode2D();
            await CreateEmptyTweenPropertyVariants(node2D, Property.PositionBySizeX, percentFrom, percentTo);
            await CreateEmptyTweenPropertyVariants(node2D, Property.PositionBySizeY, percentFrom, percentTo);
            await CreateEmptyTweenPropertyVariants(node2D, Property.PositionBySize2D, Vector2.One, Vector2.Zero);
        }

        [Test(Description = "Property Scale2DX, Scale2DXByCallback, Scale2DY, Scale2DYByCallback")]
        public async Task TweenPropertyScaleX_Y() {
            const float from = 0.9f;
            const float to = 1.2f;
            foreach (var property in new[] { Property.Scale2DX, Property.Scale2DXByCallback }) {

                var node2D = await CreateNode2D();
                node2D.Scale = Vector2.Zero;
                await CreateTweenPropertyVariants(node2D, property, from, to);
                Assert.That(node2D.Scale.x, Is.EqualTo(to));

                var control = await CreateLabel();
                control.RectScale = Vector2.Zero;
                await CreateTweenPropertyVariants(control, property, from, to);
                Assert.That(control.RectScale.x, Is.EqualTo(to));

                var node = await CreateNode();
                await CreateEmptyTweenPropertyVariants(node, property, from, to);
            }

            foreach (var property in new[] { Property.Scale2DY, Property.Scale2DYByCallback }) {
                var node2D = await CreateNode2D();
                node2D.Scale = Vector2.Zero;
                await CreateTweenPropertyVariants(node2D, property, from, to);
                Assert.That(node2D.Scale.y, Is.EqualTo(to));

                var control = await CreateLabel();
                control.RectScale = Vector2.Zero;
                await CreateTweenPropertyVariants(control, property, from, to);
                Assert.That(control.RectScale.y, Is.EqualTo(to));

                var node = await CreateNode();
                await CreateEmptyTweenPropertyVariants(node, property, from, to);
            }
        }

        [Test(Description = "Property SkewX, SkewY")]
        public async Task TweenPropertySkewX_Y() {
            const float from = 0.9f;
            const float to = 1.2f;

            var node2D = await CreateNode2D();
            node2D.SetIndexed("transform:y:x", 0);
            await CreateTweenPropertyVariants(node2D, Property.Skew2DX, from, to);
            Assert.That(node2D.GetIndexed("transform:y:x"), Is.EqualTo(to));

            node2D.SetIndexed("transform:x:y", 0);
            await CreateTweenPropertyVariants(node2D, Property.Skew2DY, from, to);
            Assert.That(node2D.GetIndexed("transform:x:y"), Is.EqualTo(to));

            var label = await CreateLabel();
            await CreateEmptyTweenPropertyVariants(label, Property.Skew2DX, from, to);
            await CreateEmptyTweenPropertyVariants(label, Property.Skew2DY, from, to);

            var node = await CreateNode();
            await CreateEmptyTweenPropertyVariants(node, Property.Skew2DX, from, to);
            await CreateEmptyTweenPropertyVariants(node, Property.Skew2DY, from, to);
        }

        [Test(Description = "Property Position2D")]
        public async Task TweenPropertyPosition2d() {
            var from = Vector2.Zero;
            var to = new Vector2(23f, -12f);

            var node2D = await CreateNode2D();
            node2D.Position = Vector2.Zero;
            await CreateTweenPropertyVariants(node2D, Property.Position2D, from, to);
            Assert.That(node2D.Position, Is.EqualTo(to));

            var control = await CreateLabel();
            control.RectPosition = Vector2.Zero;
            await CreateTweenPropertyVariants(control, Property.Position2D, from, to);
            Assert.That(control.RectPosition, Is.EqualTo(to));

            var node = await CreateNode();
            await CreateEmptyTweenPropertyVariants(node, Property.Position2D, from, to);
        }


        [Test(Description = "Property Scale2D")]
        public async Task TweenPropertyScale2d() {
            foreach (var property in new [] {  Property.Scale2D, Property.Scale2DByCallback }) {
                var from = Vector2.Zero;
                var to = new Vector2(23f, -12f);

                var node2D = await CreateNode2D();
                node2D.Scale = Vector2.Zero;
                await CreateTweenPropertyVariants(node2D, property, from, to);
                Assert.That(node2D.Scale, Is.EqualTo(to));

                var control = await CreateLabel();
                control.RectScale = Vector2.Zero;
                await CreateTweenPropertyVariants(control, property, from, to);
                Assert.That(control.RectScale, Is.EqualTo(to));

                var node = await CreateNode();
                await CreateEmptyTweenPropertyVariants(node, property, from, to);
            }
        }

        [Test(Description = "Property modulate")]
        public async Task TweenPropertyColor() {
            var from = new Color(0.1f, 0.1f, 0.1f, 0.1f);
            var fromA = new Color(0.1f, 0.1f, 0.1f, 1f);
            var fromB = new Color(0.1f, 0.1f, 1f, 0.1f);
            var fromG = new Color(0.1f, 1f, 0.1f, 0.1f);
            var fromR = new Color(1f, 0.1f, 0.1f, 0.1f);
            var to = new Color(1f, 1f, 1f, 1f);

            foreach (var node in new CanvasItem[] { await CreateNode2D(), await CreateSprite(), await CreateLabel() }) {
                node.Modulate = from;
                await CreateTweenPropertyVariants(node, Property.Modulate, -from, to);
                Assert.That(node.Modulate, Is.EqualTo(to));

                node.Modulate = from;
                await CreateTweenPropertyVariants(node, Property.ModulateR, 0f, 1f);
                Assert.That(node.Modulate, Is.EqualTo(fromR));

                node.Modulate = from;
                await CreateTweenPropertyVariants(node, Property.ModulateG, 0f, 1f);
                Assert.That(node.Modulate, Is.EqualTo(fromG));

                node.Modulate = from;
                await CreateTweenPropertyVariants(node, Property.ModulateB, 0f, 1f);
                Assert.That(node.Modulate, Is.EqualTo(fromB));

                node.Modulate = from;
                await CreateTweenPropertyVariants(node, Property.Opacity, 0f, 1f);
                Assert.That(node.Modulate, Is.EqualTo(fromA));
            }

            await CreateEmptyTweenPropertyVariants(await CreateNode(), Property.Modulate, from, to);
            await CreateEmptyTweenPropertyVariants(await CreateNode(), Property.ModulateR, 0f, 1f);
            await CreateEmptyTweenPropertyVariants(await CreateNode(), Property.ModulateG, 0f, 1f);
            await CreateEmptyTweenPropertyVariants(await CreateNode(), Property.ModulateB, 0f, 1f);
            await CreateEmptyTweenPropertyVariants(await CreateNode(), Property.Opacity, 0f, 1f);
        }


        public Vector2 follow;

        [Test(Description = "Custom IndexedProperty")]
        public async Task TweenPropertyBasicPropertyString() {
            var prop = (IndexedProperty<Vector2>)"follow";
            await CreateTweenPropertyVariants(this, prop, Vector2.Zero, Vector2.Up);
            Assert.That(follow, Is.EqualTo(Vector2.Up));
        }

        /**
         * Callbacks
         */
        [Test(Description = "non IndexedProperty using the SetVale as a callback tween")]
        public async Task TweenSequenceStepsToWithCallbackProperty() {
            List<DebugStep<float>> steps = new List<DebugStep<float>>();
            var sprite = await CreateSprite();
            var callbackProperty = new CallbackProperty();
            Assert.That(callbackProperty.Calls, Is.EqualTo(0));

            await CreateTweenPropertyVariants(sprite, callbackProperty, 0, -90);

            Assert.That(callbackProperty.Calls, Is.GreaterThan(0));
            Assert.That(sprite.Position.x, Is.EqualTo(-90));
        }

        private class CallbackProperty : IProperty<float> {
            public int Calls = 0;
            public float GetValue(Node node) {
                return (float)node.GetIndexed("position:x");
            }

            public bool IsCompatibleWith(Node node) {
                return true;
            }

            public void SetValue(Node node, float initialValue, float value) {
                Calls++;
                node.SetIndexed("position:x", value);
            }
        }

        private static async Task CreateTweenPropertyVariants<T>(Node node, IProperty<T> property, T from, T to) {
            List<DebugStep<T>> steps = new List<DebugStep<T>>();
            await TweenSequenceBuilder.Create()
                .AnimateSteps(node, property)
                .SetDebugSteps(steps)
                .From(from)
                .To(to, 0.1f, Easing.BackIn)
                .EndAnimate()
                .Play(node)
                .Await();

            AssertStep(steps[0], from, to, 0f, 0.1f, Easing.BackIn);
            Assert.That(steps.Count, Is.EqualTo(1));
            Assert.That(property.GetValue(node), Is.EqualTo(to));
        }

        private static async Task CreateEmptyTweenPropertyVariants<T>(Node node, IProperty<T> property, T from, T to) {
            List<DebugStep<T>> steps = new List<DebugStep<T>>();
            await TweenSequenceBuilder.Create()
                .AnimateSteps(node, property)
                .SetDebugSteps(steps)
                .To(to, 0.1f, Easing.BackIn)
                .EndAnimate()
                .Play(node)
                .Await();

            Assert.That(steps.Count, Is.EqualTo(0));
        }

        [Test]
        public void PivotControlRestoreTests() {
            var control = new Control();
            var original = new Vector2(2f, 3f);
            control.RectPivotOffset = original;
            GD.Print(control.RectPivotOffset);

            var pivotCenterBottom = control.SetRotateOriginToBottomCenter();
            GD.Print(control.RectPivotOffset);
            Assert.That(control.RectPivotOffset, Is.Not.EqualTo(original));

            pivotCenterBottom.Rollback();
            GD.Print(control.RectPivotOffset);
            Assert.That(control.RectPivotOffset, Is.EqualTo(original));
        }

        [Test]
        public async Task PivotSpriteRestoreTests() {
            var sprite = await CreateSprite();
            var original = new Vector2(2f, 3f);
            sprite.GlobalPosition = original;
            sprite.Offset = original;
            sprite.Scale = original;
            GD.Print(sprite.Offset);
            GD.Print(sprite.GlobalPosition);
            await this.AwaitIdleFrame();

            var pivotCenterBottom = sprite.SetRotateOriginToBottomCenter();
            GD.Print(sprite.Offset);
            GD.Print(sprite.GlobalPosition);
            Assert.That(sprite.Offset, Is.Not.EqualTo(original));
            Assert.That(sprite.GlobalPosition, Is.Not.EqualTo(original));

            pivotCenterBottom.Rollback();
            GD.Print(sprite.Offset);
            GD.Print(sprite.GlobalPosition);
            Assert.That(sprite.Offset, Is.EqualTo(original));
            Assert.That(sprite.GlobalPosition, Is.EqualTo(original));
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