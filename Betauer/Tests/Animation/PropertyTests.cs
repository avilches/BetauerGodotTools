using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using NUnit.Framework;
using Betauer.Animation;
using Vector2 = Godot.Vector2;

namespace Betauer.Tests.Animation {
    [TestFixture]
    public class PropertyTests : Node {
        public async Task<Sprite> CreateSprite() {
            Sprite sprite = new Sprite();
            sprite.Position = new Vector2(100, 100);
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

        public async Task<Label> CreateLabel() {
            Label control = new Label();
            control.RectPosition = new Vector2(100, 100);
            AddChild(control);
            await this.AwaitIdleFrame();
            return control;
        }

        [Test(Description = "Property Rotate")]
        public async Task TweenPropertyRotate() {
            const float from = 1f;
            const float to = 3f;

            var node2D = await CreateNode2D();
            node2D.RotationDegrees = 0f;
            await CreateTweenPropertyVariants(node2D, Property.RotateCenter, from, to);
            Assert.That(node2D.RotationDegrees, Is.EqualTo(to));

            var control = await CreateLabel();
            control.RectRotation = 0f;
            await CreateTweenPropertyVariants(control, Property.RotateCenter, from, to);
            Assert.That(control.RectRotation, Is.EqualTo(to));
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
        }

        [Test(Description = "Property ScaleX, ScaleY")]
        public async Task TweenPropertyScaleX_Y() {
            const float from = 0.9f;
            const float to = 1.2f;

            var node2D = await CreateNode2D();
            node2D.Scale = Vector2.Zero;
            await CreateTweenPropertyVariants(node2D, Property.ScaleX, from, to);
            Assert.That(node2D.Scale.x, Is.EqualTo(to));

            node2D.Scale = Vector2.Zero;
            await CreateTweenPropertyVariants(node2D, Property.ScaleY, from, to);
            Assert.That(node2D.Scale.y, Is.EqualTo(to));

            var control = await CreateLabel();
            control.RectScale = Vector2.Zero;
            await CreateTweenPropertyVariants(control, Property.ScaleX, from, to);
            Assert.That(control.RectScale.x, Is.EqualTo(to));

            control.RectScale = Vector2.Zero;
            await CreateTweenPropertyVariants(control, Property.ScaleY, from, to);
            Assert.That(control.RectScale.y, Is.EqualTo(to));
        }

        [Test(Description = "Property SkewX, SkewY")]
        public async Task TweenPropertySkewX_Y() {
            const float from = 0.9f;
            const float to = 1.2f;

            var node2D = await CreateNode2D();
            node2D.SetIndexed("transform:y:x", 0);
            await CreateTweenPropertyVariants(node2D, Property.SkewX, from, to);
            Assert.That(node2D.GetIndexed("transform:y:x"), Is.EqualTo(to));

            node2D.SetIndexed("transform:x:y", 0);
            await CreateTweenPropertyVariants(node2D, Property.SkewY, from, to);
            Assert.That(node2D.GetIndexed("transform:x:y"), Is.EqualTo(to));
        }

        [Test(Description = "Property Position2D")]
        public async Task TweenPropertyPosition2d() {
            var to = new Vector2(23f, -12f);

            var node2D = await CreateNode2D();
            node2D.Position = Vector2.Zero;
            await CreateTweenPropertyVariants(node2D, Property.Position2D, Vector2.Zero, to);
            Assert.That(node2D.Position, Is.EqualTo(to));

            var control = await CreateLabel();
            control.RectPosition = Vector2.Zero;
            await CreateTweenPropertyVariants(control, Property.Position2D, Vector2.Zero, to);
            Assert.That(control.RectPosition, Is.EqualTo(to));
        }


        [Test(Description = "Property Scale2D")]
        public async Task TweenPropertyScale2d() {
            var to = new Vector2(23f, -12f);

            var node2D = await CreateNode2D();
            node2D.Scale = Vector2.Zero;
            await CreateTweenPropertyVariants(node2D, Property.Scale2D, Vector2.Zero, to);
            Assert.That(node2D.Scale, Is.EqualTo(to));

            var control = await CreateLabel();
            control.RectScale = Vector2.Zero;
            await CreateTweenPropertyVariants(control, Property.Scale2D, Vector2.Zero, to);
            Assert.That(control.RectScale, Is.EqualTo(to));
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
                await CreateTweenPropertyVariants(node, Property.ModulateR, 0, 1);
                Assert.That(node.Modulate, Is.EqualTo(fromR));

                node.Modulate = from;
                await CreateTweenPropertyVariants(node, Property.ModulateG, 0, 1);
                Assert.That(node.Modulate, Is.EqualTo(fromG));

                node.Modulate = from;
                await CreateTweenPropertyVariants(node, Property.ModulateB, 0, 1);
                Assert.That(node.Modulate, Is.EqualTo(fromB));

                node.Modulate = from;
                await CreateTweenPropertyVariants(node, Property.Opacity, 0, 1);
                Assert.That(node.Modulate, Is.EqualTo(fromA));
            }
        }


        public Vector2 follow;

        [Test(Description = "BasicProperty")]
        public async Task TweenPropertyBasicPropertyString() {
            var prop = (Property<Vector2>)"follow";
            await CreateTweenPropertyVariants(this, prop, Vector2.Zero, Vector2.Up);
            Assert.That(follow, Is.EqualTo(Vector2.Up));
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

        [Test]
        public void PivotControlRestoreTests() {
            var control = new Control();
            var original = new Vector2(2f, 3f);
            control.RectPivotOffset = original;
            GD.Print(control.RectPivotOffset);

            var pivotCenterBottom = control.SetPivotCenterBottom();
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
            var gradientTexture = new GradientTexture();
            sprite.Texture = gradientTexture;
            gradientTexture.Width = 300;
            var gradient = new Gradient();
            gradient.AddPoint(0, Colors.Aqua);
            gradient.AddPoint(20, Colors.Red);
            gradientTexture.Gradient = gradient;
            GD.Print(sprite.Offset);
            GD.Print(sprite.GlobalPosition);
            await this.AwaitIdleFrame();

            var pivotCenterBottom = sprite.SetPivotCenterBottom();
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