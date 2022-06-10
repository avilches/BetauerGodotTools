using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using NUnit.Framework;
using Betauer.Animation;
using Betauer.TestRunner;
using Vector2 = Godot.Vector2;

namespace Betauer.Tests.Animation {
    [TestFixture]
    public class PropertyTests : NodeTest {
        [SetUp]
        public void SetUp() {
            Engine.TimeScale = 10;
            LoggerFactory.OverrideTraceLevel(TraceLevel.All);
        }

        [TearDown]
        public void TearDown() {
            Engine.TimeScale = 1;
        }

        [Test]
        public async Task InternalTest_CreateSpriteWithTextureSize() {
            var sprite = await CreateSprite(300);
            Assert.That(sprite.GetSpriteSize().x, Is.EqualTo(300));
        }

        /*
         * Test builder methods with lambdas (Sequence and template)
         */

        [Test(Description = "Lambda with value only property")]
        public async Task LambdaValueProperty() {
            var spritePlayer = await CreateSprite();
            var spriteAnimation = await CreateSprite();
            List<float> values1s = new List<float>();
            List<float> values1sb = new List<float>();
            List<float> values1rs = new List<float>();
            List<float> values1k = new List<float>();
            List<float> values1kb = new List<float>();
            List<float> values1rk = new List<float>();

            var actionTween = await CreateTween();
            await SequenceBuilder.Create()
                // values1 animation
                .AnimateSteps((float x) => values1s.Add(x))
                .From(8).To(400, 0.01f).EndAnimate()
                .AnimateStepsBy((float x) => values1sb.Add(x))
                .From(9).Offset(400, 0.01f).EndAnimate()
                .AnimateRelativeSteps((float x) => values1rs.Add(x))
                .From(10).Offset(400, 0.01f).EndAnimate()
                .AnimateKeys((float x) => values1k.Add(x)).Duration(0.01f)
                .From(11).KeyframeTo(1, 400).EndAnimate()
                .AnimateKeysBy((float x) => values1kb.Add(x)).Duration(0.01f)
                .From(12).KeyframeOffset(1, 400).EndAnimate()
                .AnimateRelativeKeys((float x) => values1rk.Add(x)).Duration(0.01f)
                .From(13).KeyframeOffset(1, 400).EndAnimate()
                .Play(actionTween, spritePlayer)
                .Await();

            Assert.That(values1s[0], Is.EqualTo(8));
            Assert.That(values1sb[0], Is.EqualTo(9));
            Assert.That(values1rs[0], Is.EqualTo(10));
            Assert.That(values1k[0], Is.EqualTo(11));
            Assert.That(values1kb[0], Is.EqualTo(12));
            Assert.That(values1rk[0], Is.EqualTo(13));

            Assert.That(values1s.Last(), Is.EqualTo(400));
            Assert.That(values1sb.Last(), Is.EqualTo(409));
            Assert.That(values1rs.Last(), Is.EqualTo(410));
            Assert.That(values1k.Last(), Is.EqualTo(400));
            Assert.That(values1kb.Last(), Is.EqualTo(412));
            Assert.That(values1rk.Last(), Is.EqualTo(413));
            await Task.Delay((int)(ActionTween.ExtraDelayToFinish * 2 * 1000));
            Assert.That(actionTween.GetPendingObjects().Count, Is.EqualTo(0));
        }

        [Test(Description = "Lambda with value only property (Template)")]
        public async Task TemplateLambdaValueProperty() {
            var spritePlayer = await CreateSprite();
            var spriteAnimation = await CreateSprite();
            List<float> values1s = new List<float>();
            List<float> values1sb = new List<float>();
            List<float> values1rs = new List<float>();
            List<float> values1k = new List<float>();
            List<float> values1kb = new List<float>();
            List<float> values1rk = new List<float>();

            await TemplateBuilder.Create()
                // values1 animation
                .AnimateSteps((float x) => values1s.Add(x))
                .From(8).To(400, 0.01f).EndAnimate()
                .AnimateStepsBy((float x) => values1sb.Add(x))
                .From(9).Offset(400, 0.01f).EndAnimate()
                .AnimateRelativeSteps((float x) => values1rs.Add(x))
                .From(10).Offset(400, 0.01f).EndAnimate()
                .AnimateKeys((float x) => values1k.Add(x)).Duration(0.01f)
                .From(11).KeyframeTo(1, 400).EndAnimate()
                .AnimateKeysBy((float x) => values1kb.Add(x)).Duration(0.01f)
                .From(12).KeyframeOffset(1, 400).EndAnimate()
                .AnimateRelativeKeys((float x) => values1rk.Add(x)).Duration(0.01f)
                .From(13).KeyframeOffset(1, 400).EndAnimate()
                .BuildTemplate()
                .Play(await CreateTween(), spritePlayer)
                .Await();

            Assert.That(values1s[0], Is.EqualTo(8));
            Assert.That(values1sb[0], Is.EqualTo(9));
            Assert.That(values1rs[0], Is.EqualTo(10));
            Assert.That(values1k[0], Is.EqualTo(11));
            Assert.That(values1kb[0], Is.EqualTo(12));
            Assert.That(values1rk[0], Is.EqualTo(13));

            Assert.That(values1s.Last(), Is.EqualTo(400));
            Assert.That(values1sb.Last(), Is.EqualTo(409));
            Assert.That(values1rs.Last(), Is.EqualTo(410));
            Assert.That(values1k.Last(), Is.EqualTo(400));
            Assert.That(values1kb.Last(), Is.EqualTo(412));
            Assert.That(values1rk.Last(), Is.EqualTo(413));
        }

        [Test(Description = "Lambda with node and value property")]
        public async Task LambdaNodeValueProperty() {
            var spritePlayer = await CreateSprite();
            var spriteAnimation = await CreateSprite();
            List<float> values1s = new List<float>();
            List<float> values1sb = new List<float>();
            List<float> values1rs = new List<float>();
            List<float> values1k = new List<float>();
            List<float> values1kb = new List<float>();
            List<float> values1rk = new List<float>();

            await SequenceBuilder.Create()
                // values1 animation
                .AnimateSteps(spriteAnimation, (Node node, float x) => {
                    Assert.That(node, Is.EqualTo(spriteAnimation));
                    values1s.Add(x);
                })
                .From(8).To(400, 0.01f).EndAnimate()
                .AnimateStepsBy(spriteAnimation, (Node node, float x) => {
                    Assert.That(node, Is.EqualTo(spriteAnimation));
                    values1sb.Add(x);
                })
                .From(9).Offset(400, 0.01f).EndAnimate()
                .AnimateRelativeSteps(spriteAnimation, (Node node, float x) => {
                    Assert.That(node, Is.EqualTo(spriteAnimation));
                    values1rs.Add(x);
                })
                .From(10).Offset(400, 0.01f).EndAnimate()
                .AnimateKeys(spriteAnimation, (Node node, float x) => {
                    Assert.That(node, Is.EqualTo(spriteAnimation));
                    values1k.Add(x);
                }).Duration(0.01f)
                .From(11).KeyframeTo(1, 400).EndAnimate()
                .AnimateKeysBy(spriteAnimation, (Node node, float x) => {
                    Assert.That(node, Is.EqualTo(spriteAnimation));
                    values1kb.Add(x);
                }).Duration(0.01f)
                .From(12).KeyframeOffset(1, 400).EndAnimate()
                .AnimateRelativeKeys(spriteAnimation, (Node node, float x) => {
                    Assert.That(node, Is.EqualTo(spriteAnimation));
                    values1rk.Add(x);
                }).Duration(0.01f)
                .From(13).KeyframeOffset(1, 400).EndAnimate()
                .Play(await CreateTween(), spritePlayer)
                .Await();

            Assert.That(values1s[0], Is.EqualTo(8));
            Assert.That(values1sb[0], Is.EqualTo(9));
            Assert.That(values1rs[0], Is.EqualTo(10));
            Assert.That(values1k[0], Is.EqualTo(11));
            Assert.That(values1kb[0], Is.EqualTo(12));
            Assert.That(values1rk[0], Is.EqualTo(13));

            Assert.That(values1s.Last(), Is.EqualTo(400));
            Assert.That(values1sb.Last(), Is.EqualTo(409));
            Assert.That(values1rs.Last(), Is.EqualTo(410));
            Assert.That(values1k.Last(), Is.EqualTo(400));
            Assert.That(values1kb.Last(), Is.EqualTo(412));
            Assert.That(values1rk.Last(), Is.EqualTo(413));
        }

        [Test(Description = "Lambda with node and value property (Template)")]
        public async Task TemplateLambdaNodeValueProperty() {
            var spritePlayer = await CreateSprite();
            List<float> values1s = new List<float>();
            List<float> values1sb = new List<float>();
            List<float> values1rs = new List<float>();
            List<float> values1k = new List<float>();
            List<float> values1kb = new List<float>();
            List<float> values1rk = new List<float>();

            await TemplateBuilder.Create()
                // values1 animation
                .AnimateSteps((Node node, float x) => {
                    Assert.That(node, Is.EqualTo(spritePlayer));
                    values1s.Add(x);
                })
                .From(8).To(400, 0.01f).EndAnimate()
                .AnimateStepsBy((Node node, float x) => {
                    Assert.That(node, Is.EqualTo(spritePlayer));
                    values1sb.Add(x);
                })
                .From(9).Offset(400, 0.01f).EndAnimate()
                .AnimateRelativeSteps((Node node, float x) => {
                    Assert.That(node, Is.EqualTo(spritePlayer));
                    values1rs.Add(x);
                })
                .From(10).Offset(400, 0.01f).EndAnimate()
                .AnimateKeys((Node node, float x) => {
                    Assert.That(node, Is.EqualTo(spritePlayer));
                    values1k.Add(x);
                }).Duration(0.01f)
                .From(11).KeyframeTo(1, 400).EndAnimate()
                .AnimateKeysBy((Node node, float x) => {
                    Assert.That(node, Is.EqualTo(spritePlayer));
                    values1kb.Add(x);
                }).Duration(0.01f)
                .From(12).KeyframeOffset(1, 400).EndAnimate()
                .AnimateRelativeKeys((Node node, float x) => {
                    Assert.That(node, Is.EqualTo(spritePlayer));
                    values1rk.Add(x);
                }).Duration(0.01f)
                .From(13).KeyframeOffset(1, 400).EndAnimate()
                .BuildTemplate()
                .Play(await CreateTween(), spritePlayer)
                .Await();

            Assert.That(values1s[0], Is.EqualTo(8));
            Assert.That(values1sb[0], Is.EqualTo(9));
            Assert.That(values1rs[0], Is.EqualTo(10));
            Assert.That(values1k[0], Is.EqualTo(11));
            Assert.That(values1kb[0], Is.EqualTo(12));
            Assert.That(values1rk[0], Is.EqualTo(13));

            Assert.That(values1s.Last(), Is.EqualTo(400));
            Assert.That(values1sb.Last(), Is.EqualTo(409));
            Assert.That(values1rs.Last(), Is.EqualTo(410));
            Assert.That(values1k.Last(), Is.EqualTo(400));
            Assert.That(values1kb.Last(), Is.EqualTo(412));
            Assert.That(values1rk.Last(), Is.EqualTo(413));
        }

        private float _stringProperty = 0;

        [Test(Description = "classic string property tween")]
        public async Task StringProperty() {
            var spritePlayer = await CreateSprite();
            await SequenceBuilder.Create()
                .AnimateSteps<float>(this, nameof(_stringProperty))
                .From(8).To(400, 0.01f).EndAnimate()
                .Play(await CreateTween(), spritePlayer)
                .Await();
            Assert.That(_stringProperty, Is.EqualTo(400));

            await SequenceBuilder.Create()
                .AnimateStepsBy<float>(this, nameof(_stringProperty))
                .From(9).Offset(400, 0.01f).EndAnimate()
                .Play(await CreateTween(), spritePlayer)
                .Await();
            Assert.That(_stringProperty, Is.EqualTo(409));

            await SequenceBuilder.Create()
                .AnimateRelativeSteps<float>(this, nameof(_stringProperty))
                .From(10).Offset(400, 0.01f).EndAnimate()
                .Play(await CreateTween(), spritePlayer)
                .Await();
            Assert.That(_stringProperty, Is.EqualTo(410));

            await SequenceBuilder.Create()
                .AnimateKeys<float>(this, nameof(_stringProperty)).Duration(0.01f)
                .From(11).KeyframeTo(1, 400).EndAnimate()
                .Play(await CreateTween(), spritePlayer)
                .Await();
            Assert.That(_stringProperty, Is.EqualTo(400));

            await SequenceBuilder.Create()
                .AnimateKeysBy<float>(this, nameof(_stringProperty)).Duration(0.01f)
                .From(12).KeyframeOffset(1, 400).EndAnimate()
                .Play(await CreateTween(), spritePlayer)
                .Await();
            Assert.That(_stringProperty, Is.EqualTo(412));

            await SequenceBuilder.Create()
                .AnimateRelativeKeys<float>(this, nameof(_stringProperty)).Duration(0.01f)
                .From(13).KeyframeOffset(1, 400).EndAnimate()
                .Play(await CreateTween(), spritePlayer)
                .Await();
            Assert.That(_stringProperty, Is.EqualTo(413));
        }

        [Test(Description = "classic string property tween (Template)")]
        public async Task TemplateStringProperty() {
            var spritePlayer = await CreateSprite();
            await TemplateBuilder.Create()
                .AnimateSteps<float>( nameof(_stringProperty))
                .From(8).To(400, 0.01f).EndAnimate()
                .BuildTemplate()
                .Play(await CreateTween(), this)
                .Await();
            Assert.That(_stringProperty, Is.EqualTo(400));

            await TemplateBuilder.Create()
                .AnimateStepsBy<float>( nameof(_stringProperty))
                .From(9).Offset(400, 0.01f).EndAnimate()
                .BuildTemplate()
                .Play(await CreateTween(), this)
                .Await();
            Assert.That(_stringProperty, Is.EqualTo(409));

            await TemplateBuilder.Create()
                .AnimateRelativeSteps<float>( nameof(_stringProperty))
                .From(10).Offset(400, 0.01f).EndAnimate()
                .BuildTemplate()
                .Play(await CreateTween(), this)
                .Await();
            Assert.That(_stringProperty, Is.EqualTo(410));

            await TemplateBuilder.Create()
                .AnimateKeys<float>( nameof(_stringProperty)).Duration(0.01f)
                .From(11).KeyframeTo(1, 400).EndAnimate()
                .BuildTemplate()
                .Play(await CreateTween(), this)
                .Await();
            Assert.That(_stringProperty, Is.EqualTo(400));

            await TemplateBuilder.Create()
                .AnimateKeysBy<float>( nameof(_stringProperty)).Duration(0.01f)
                .From(12).KeyframeOffset(1, 400).EndAnimate()
                .BuildTemplate()
                .Play(await CreateTween(), this)
                .Await();
            Assert.That(_stringProperty, Is.EqualTo(412));

            await TemplateBuilder.Create()
                .AnimateRelativeKeys<float>( nameof(_stringProperty)).Duration(0.01f)
                .From(13).KeyframeOffset(1, 400).EndAnimate()
                .BuildTemplate()
                .Play(await CreateTween(), this)
                .Await();
            Assert.That(_stringProperty, Is.EqualTo(413));
        }

        /*
         * Property
         */

        [Test(Description = "Property Rotate")]
        public async Task TweenPropertyRotate() {
            foreach (var property in new[] { Property.Rotate2D, Property.Rotate2DByCallback }) {
                const float from = 1f;
                const float to = 3f;
                var node2D = await CreateNode2D();

                await CreateTweenPropertyVariants(node2D, property, from, to);

                var control = await CreateLabel();
                await CreateTweenPropertyVariants(control, property, from, to);

                var node = await CreateNode();
                await CreateEmptyTweenPropertyVariants(node, property, from, to);
            }
        }

        [Test(Description = "Property PositionX, PositionY")]
        public async Task TweenPropertyPositionX_Y() {
            const float from = 90f;
            const float to = 120f;

            var node2D = await CreateNode2D();

            await CreateTweenPropertyVariants(node2D, Property.PositionX, from, to);
            await CreateTweenPropertyVariants(node2D, Property.PositionY, from, to);

            var control = await CreateLabel();
            await CreateTweenPropertyVariants(control, Property.PositionX, from, to);
            await CreateTweenPropertyVariants(control, Property.PositionY, from, to);

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
            await SequenceBuilder.Create()
                .AnimateSteps(spriteX, Property.PositionBySizeX)
                .From(percentFrom)
                .To(percentTo, 0.1f)
                .To(percentTo * 2, 0.1f)
                .EndAnimate()
                .Play(await CreateTween(), spriteX)
                .Await();
            Assert.That(spriteX.Position.x, Is.EqualTo(initialPosition + width * percentTo * 2));

            var spriteY = await CreateSprite(width);
            spriteY.Position = new Vector2(0, initialPosition);
            await SequenceBuilder.Create()
                .AnimateSteps(spriteY, Property.PositionBySizeY)
                .From(percentFrom)
                .To(percentTo, 0.1f)
                .To(percentTo * 2, 0.1f)
                .EndAnimate()
                .Play(await CreateTween(), spriteY)
                .Await();
            Assert.That(spriteY.Position.y, Is.EqualTo(initialPosition + width * percentTo * 2));

            var sprite2D = await CreateSprite(width);
            sprite2D.Position = new Vector2(initialPosition, initialPosition);
            await SequenceBuilder.Create()
                .AnimateSteps(sprite2D, Property.PositionBySize2D)
                .From(new Vector2(percentFrom, percentFrom))
                .To(new Vector2(percentTo, percentTo), 0.1f)
                .To(new Vector2(percentTo * 2, percentTo * 2), 0.1f)
                .EndAnimate()
                .Play(await CreateTween(), sprite2D)
                .Await();
            Assert.That(sprite2D.Position,
                Is.EqualTo(
                    new Vector2(initialPosition + width * percentTo * 2, initialPosition + width * percentTo * 2)));

            var controlX = await CreateLabel(width);
            controlX.RectPosition = new Vector2(initialPosition, 0);
            await SequenceBuilder.Create()
                .AnimateSteps(controlX, Property.PositionBySizeX)
                .From(percentFrom)
                .To(percentTo, 0.1f)
                .To(percentTo * 2, 0.1f)
                .EndAnimate()
                .Play(await CreateTween(), controlX)
                .Await();
            Assert.That(controlX.RectPosition.x, Is.EqualTo(initialPosition + width * percentTo * 2));

            var controlY = await CreateLabel(width);
            controlY.RectPosition = new Vector2(0, initialPosition);
            await SequenceBuilder.Create()
                .AnimateSteps(controlY, Property.PositionBySizeY)
                .From(percentFrom)
                .To(percentTo, 0.1f)
                .To(percentTo * 2, 0.1f)
                .EndAnimate()
                .Play(await CreateTween(), controlY)
                .Await();
            Assert.That(controlY.RectPosition.y, Is.EqualTo(initialPosition + width * percentTo * 2));

            var control2D = await CreateLabel(width);
            control2D.RectPosition = new Vector2(initialPosition, initialPosition);
            await SequenceBuilder.Create()
                .AnimateSteps(control2D, Property.PositionBySize2D)
                .From(new Vector2(percentFrom, percentFrom))
                .To(new Vector2(percentTo, percentTo), 0.1f)
                .To(new Vector2(percentTo * 2, percentTo * 2), 0.1f)
                .EndAnimate()
                .Play(await CreateTween(), control2D)
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
                await CreateTweenPropertyVariants(node2D, property, from, to);

                var control = await CreateLabel();
                await CreateTweenPropertyVariants(control, property, from, to);

                var node = await CreateNode();
                await CreateEmptyTweenPropertyVariants(node, property, from, to);
            }

            foreach (var property in new[] { Property.Scale2DY, Property.Scale2DYByCallback }) {
                var node2D = await CreateNode2D();
                await CreateTweenPropertyVariants(node2D, property, from, to);

                var control = await CreateLabel();
                await CreateTweenPropertyVariants(control, property, from, to);

                var node = await CreateNode();
                await CreateEmptyTweenPropertyVariants(node, property, from, to);
            }
        }

        [Test(Description = "Property SkewX, SkewY")]
        public async Task TweenPropertySkewX_Y() {
            const float from = 0.9f;
            const float to = 1.2f;

            var node2D = await CreateNode2D();
            await CreateTweenPropertyVariants(node2D, Property.Skew2DX, from, to);
            await CreateTweenPropertyVariants(node2D, Property.Skew2DY, from, to);

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
            await CreateTweenPropertyVariants(node2D, Property.Position2D, from, to);

            var control = await CreateLabel();
            await CreateTweenPropertyVariants(control, Property.Position2D, from, to);

            var node = await CreateNode();
            await CreateEmptyTweenPropertyVariants(node, Property.Position2D, from, to);
        }


        [Test(Description = "Property Scale2D")]
        public async Task TweenPropertyScale2d() {
            foreach (var property in new[] { Property.Scale2D, Property.Scale2DByCallback }) {
                var from = Vector2.One;
                var to = new Vector2(23f, -12f);

                var node2D = await CreateNode2D();
                await CreateTweenPropertyVariants(node2D, property, from, to);

                var control = await CreateLabel();
                await CreateTweenPropertyVariants(control, property, from, to);

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
                await CreateTweenPropertyVariants(node, Property.Modulate, from, to);
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
            follow = Vector2.Zero;
            await CreateTweenPropertyVariants(this, (IndexedProperty<Vector2>)"follow", Vector2.Zero, Vector2.Up);
            Assert.That(follow, Is.EqualTo(Vector2.Up));

            follow = Vector2.Zero;
            await CreateTweenPropertyVariants(this, new IndexedProperty<Vector2>(nameof(follow)), Vector2.Zero,
                Vector2.Up);
            Assert.That(follow, Is.EqualTo(Vector2.Up));
        }

        /**
         * Callbacks
         */
        [Test(Description = "non IndexedProperty using the SetVale as a callback tween")]
        public async Task SequenceStepsToWithCallbackProperty() {
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

            public void SetValue(AnimationContext<float> context) {
                Calls++;
                context.Target.SetIndexed("position:x", context.Value);
            }
        }

        private async Task CreateTweenPropertyVariants<T>(Node node, IProperty<T> property, T from, T to) {
            property.SetValue(new AnimationContext<T>(node, from, -1, from));
            Assert.That(property.GetValue(node), Is.EqualTo(from));
            List<DebugStep<T>> steps = new List<DebugStep<T>>();
            var sequence = SequenceBuilder.Create()
                .AnimateSteps(node, property)
                .SetDebugSteps(steps)
                .From(from)
                .To(to, 0.1f, Easing.BackIn)
                .EndAnimate();

            // With Play()
            await sequence.Play(await CreateTween(), node).Await();
            Assert.That(property.GetValue(node), Is.EqualTo(to));

            // With MultipleSequence
            property.SetValue(new AnimationContext<T>(node, from, -1, from));
            Assert.That(property.GetValue(node), Is.EqualTo(from));
            await new MultipleSequencePlayer()
                .WithParent(node, true)
                .CreateSequence()
                .AnimateSteps(node, property)
                .SetDebugSteps(steps)
                .From(from)
                .To(to, 0.1f, Easing.BackIn)
                .EndAnimate()
                .EndSequence()
                .Play()
                .Await();
            Assert.That(property.GetValue(node), Is.EqualTo(to));

            // With SingleSequence
            property.SetValue(new AnimationContext<T>(node, from, -1, from));
            Assert.That(property.GetValue(node), Is.EqualTo(from));
            await new SingleSequencePlayer()
                .WithParent(node, true)
                .CreateSequence()
                .AnimateSteps(node, property)
                .SetDebugSteps(steps)
                .From(from)
                .To(to, 0.1f, Easing.BackIn)
                .EndAnimate()
                .EndSequence()
                .Play()
                .Await();
            Assert.That(property.GetValue(node), Is.EqualTo(to));

            // With Launcher
            property.SetValue(new AnimationContext<T>(node, from, -1, from));
            Assert.That(property.GetValue(node), Is.EqualTo(from));
            var status = await new Launcher()
                .WithParent(node)
                .Play(sequence)
                .Await();

            Assert.That(steps.Count, Is.EqualTo(4));
            AssertStep(steps[0], from, to, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[1], from, to, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[2], from, to, 0f, 0.1f, Easing.BackIn);
            AssertStep(steps[3], from, to, 0f, 0.1f, Easing.BackIn);
            Assert.That(property.GetValue(node), Is.EqualTo(to));
        }

        private async Task CreateEmptyTweenPropertyVariants<T>(Node node, IProperty<T> property, T from, T to) {
            List<DebugStep<T>> steps = new List<DebugStep<T>>();
            await SequenceBuilder.Create()
                .AnimateSteps(node, property)
                .SetDebugSteps(steps)
                .To(to, 0.1f, Easing.BackIn)
                .EndAnimate()
                .Play(await CreateTween(), node)
                .Await();

            Assert.That(steps.Count, Is.EqualTo(0));
        }

        [Test]
        public void PivotControlRestoreTests() {
            var control = new Control();
            var original = new Vector2(2f, 3f);

            control.RectPivotOffset = original;
            control.Modulate = new Color(0f,1f,0f);
            control.SelfModulate = new Color(0f,1f,0f);
            control.RectScale = original;
            control.RectPosition = original;
            control.RectRotation = 3f;

            var status = control.CreateRestorer().Save();
            control.SetRotateOriginToBottomCenter();
            control.Modulate = new Color(0.1f,0.2f,0.3f);
            control.SelfModulate = new Color(0.1f,0.2f,0.3f);
            control.RectScale *= 0.2f;
            control.RectPosition += Vector2.One;
            control.RectRotation *= 3f;

            status.Restore();

            Assert.That(control.RectPivotOffset, Is.EqualTo(original));
            Assert.That(control.Modulate, Is.EqualTo(new Color(0f,1f,0f)));
            Assert.That(control.SelfModulate, Is.EqualTo(new Color(0f,1f,0f)));
            Assert.That(control.RectScale, Is.EqualTo(original));
            Assert.That(control.RectPosition, Is.EqualTo(original));
            Assert.That(control.RectRotation, Is.EqualTo(3f));
        }

        [Test]
        public async Task PivotSpriteRestoreTests() {
            var sprite = await CreateSprite();
            var original = new Vector2(2f, 3f);

            sprite.Offset = original;
            sprite.GlobalPosition = original;

            sprite.Modulate = new Color(0f,1f,0f);
            sprite.SelfModulate = new Color(0f,1f,0f);
            sprite.Scale = Vector2.One;
            sprite.Position = original;
            sprite.Rotation = 3f;

            var status = sprite.CreateRestorer().Save();
            sprite.SetRotateOriginToBottomCenter();
            sprite.Modulate = new Color(0.1f,0.2f,0.3f);
            sprite.SelfModulate = new Color(0.1f,0.2f,0.3f);
            sprite.Scale *= 0.2f;
            sprite.Position += Vector2.One;
            sprite.Rotation *= 3f;

            status.Restore();

            Assert.That(sprite.Offset, Is.EqualTo(original));
            Assert.That(sprite.GlobalPosition, Is.EqualTo(original));
            Assert.That(sprite.Modulate, Is.EqualTo(new Color(0f,1f,0f)));
            Assert.That(sprite.SelfModulate, Is.EqualTo(new Color(0f,1f,0f)));
            Assert.That(sprite.Scale, Is.EqualTo(Vector2.One));
            Assert.That(sprite.Position, Is.EqualTo(original));
            Assert.That(sprite.Rotation, Is.EqualTo(3f));
        }

        [Test]
        public async Task PivotSpriteRestoreTests2() {
            var sprite = await CreateSprite();
            var original = new Vector2(2f, 3f);
            sprite.GlobalPosition = original;
            sprite.Offset = original;
            sprite.Scale = original;
            // GD.Print(sprite.Offset);
            // GD.Print(sprite.GlobalPosition);
            await this.AwaitIdleFrame();

            var pivotCenterBottom = sprite.SetRotateOriginToBottomCenter();
            // GD.Print(sprite.Offset);
            // GD.Print(sprite.GlobalPosition);
            Assert.That(sprite.Offset, Is.Not.EqualTo(original));
            Assert.That(sprite.GlobalPosition, Is.Not.EqualTo(original));

            pivotCenterBottom.Restore();
            // GD.Print(sprite.Offset);
            // GD.Print(sprite.GlobalPosition);
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