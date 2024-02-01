using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Core.Easing;
using Betauer.Tools.Logging;
using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Property;
using Betauer.Core.Nodes.Property.Callback;
using Betauer.Core.Signal;
using Godot;
using NUnit.Framework;
using Betauer.TestRunner;
using Vector2 = Godot.Vector2;

namespace Betauer.Animation.Tests; 

[TestRunner.Test]
public partial class PropertyTests : NodeTest {
    [SetUpClass]
    public void SetUp() {
        Engine.TimeScale = 10;
        LoggerFactory.OverrideTraceLevel(TraceLevel.All);
    }

    [TestRunner.TearDownClass]
    public void TearDown() {
        Engine.TimeScale = 1;
    }

    [TestRunner.Test]
    public async Task InternalTest_CreateSpriteWithTextureSize() {
        var sprite = await CreateSprite(300);
        Assert.That(sprite.GetSpriteSize().X, Is.EqualTo(300));
    }

    /*
     * Test builder methods with lambdas (Sequence and template)
     */

    [TestRunner.Test(Description = "Lambda with value only property")]
    public async Task LambdaValueProperty() {
        Engine.TimeScale = 1;
        var spritePlayer = await CreateSprite();
        List<int> values1s = new List<int>();
        List<int> values1sb = new List<int>();
        List<int> values1rs = new List<int>();
        List<int> values1k = new List<int>();
        List<int> values1kb = new List<int>();
        List<int> values1rk = new List<int>();

        await SequenceAnimation.Create(spritePlayer)
            // values1 animation
            .AnimateSteps((int x) => values1s.Add(x))
            .From(8).To(14, 0.2f).EndAnimate()
            .AnimateStepsBy((int x) => values1sb.Add(x))
            .From(9).Offset(10, 0.2f).EndAnimate()
            .AnimateRelativeSteps((int x) => values1rs.Add(x))
            .From(10).Offset(10, 0.2f).EndAnimate()
            .Play()
            .AwaitFinished();

        await KeyframeAnimation.Create(spritePlayer)
            .SetDuration(0.1f)
            .AnimateKeys((int x) => values1k.Add(x))
            .From(11).KeyframeTo(1, 15).EndAnimate()
            .AnimateKeysBy((int x) => values1kb.Add(x))
            .From(12).KeyframeOffset(1, 10).EndAnimate()
            .AnimateRelativeKeys((int x) => values1rk.Add(x))
            .From(13).KeyframeOffset(1, 10).EndAnimate()
            .Play()
            .AwaitFinished();

        Assert.That(values1s[0], Is.EqualTo(8).Within(1));
        Assert.That(values1sb[0], Is.EqualTo(9).Within(1));
        Assert.That(values1rs[0], Is.EqualTo(10).Within(1));
        Assert.That(values1k[0], Is.EqualTo(11).Within(1));
        Assert.That(values1kb[0], Is.EqualTo(12).Within(1));
        Assert.That(values1rk[0], Is.EqualTo(13).Within(1));

        Assert.That(values1s.Last(), Is.EqualTo(14));
        Assert.That(values1sb.Last(), Is.EqualTo(19));
        Assert.That(values1rs.Last(), Is.EqualTo(20));
        Assert.That(values1k.Last(), Is.EqualTo(15));
        Assert.That(values1kb.Last(), Is.EqualTo(22));
        Assert.That(values1rk.Last(), Is.EqualTo(23));
    }

    [TestRunner.Test(Description = "Lambda with node and value property")]
    public async Task LambdaNodeValueProperty() {
        Engine.TimeScale = 1;
        var spriteAnimation = await CreateSprite();
        List<int> values1s = new List<int>();
        List<int> values1sb = new List<int>();
        List<int> values1rs = new List<int>();
        List<int> values1k = new List<int>();
        List<int> values1kb = new List<int>();
        List<int> values1rk = new List<int>();

        await SequenceAnimation.Create(spriteAnimation)
            // values1 animation
            .AnimateSteps((Node node, int x) => {
                Assert.That(node, Is.EqualTo(spriteAnimation));
                values1s.Add(x);
            })
            .From(8).To(14, 0.2f).EndAnimate()
            .AnimateStepsBy((Node node, int x) => {
                Assert.That(node, Is.EqualTo(spriteAnimation));
                values1sb.Add(x);
            })
            .From(9).Offset(10, 0.2f).EndAnimate()
            .AnimateRelativeSteps((Node node, int x) => {
                Assert.That(node, Is.EqualTo(spriteAnimation));
                values1rs.Add(x);
            })
            .From(10).Offset(10, 0.2f).EndAnimate()
            .Play()
            .AwaitFinished();

        await KeyframeAnimation.Create(spriteAnimation)
            .SetDuration(0.1f)
            .AnimateKeys((Node node, int x) => {
                Assert.That(node, Is.EqualTo(spriteAnimation));
                values1k.Add(x);
            })
            .From(11).KeyframeTo(1, 15).EndAnimate()
            .AnimateKeysBy((Node node, int x) => {
                Assert.That(node, Is.EqualTo(spriteAnimation));
                values1kb.Add(x);
            })
            .From(12).KeyframeOffset(1, 10).EndAnimate()
            .AnimateRelativeKeys((Node node, int x) => {
                Assert.That(node, Is.EqualTo(spriteAnimation));
                values1rk.Add(x);
            })
            .From(13).KeyframeOffset(1, 10).EndAnimate()
            .Play()
            .AwaitFinished();

        Assert.That(values1s[0], Is.EqualTo(8).Within(1));
        Assert.That(values1sb[0], Is.EqualTo(9).Within(1));
        Assert.That(values1rs[0], Is.EqualTo(10).Within(1));
        Assert.That(values1k[0], Is.EqualTo(11).Within(1));
        Assert.That(values1kb[0], Is.EqualTo(12).Within(1));
        Assert.That(values1rk[0], Is.EqualTo(13).Within(1));

        Assert.That(values1s.Last(), Is.EqualTo(14));
        Assert.That(values1sb.Last(), Is.EqualTo(19));
        Assert.That(values1rs.Last(), Is.EqualTo(20));
        Assert.That(values1k.Last(), Is.EqualTo(15));
        Assert.That(values1kb.Last(), Is.EqualTo(22));
        Assert.That(values1rk.Last(), Is.EqualTo(23));
    }

    public float _stringProperty = 0;

    [TestRunner.Test(Description = "classic string property tween")]
    public async Task StringProperty() {
        var spritePlayer = await CreateSprite();
        await SequenceAnimation.Create(this)
            .AnimateSteps<float>(nameof(_stringProperty))
            .From(8).To(400, 0.01f).EndAnimate()
            .Play()
            .AwaitFinished();
        Assert.That(_stringProperty, Is.EqualTo(400));

        await SequenceAnimation.Create(this)
            .AnimateStepsBy<float>(nameof(_stringProperty))
            .From(9).Offset(400, 0.01f).EndAnimate()
            .Play()
            .AwaitFinished();
        Assert.That(_stringProperty, Is.EqualTo(409));

        await SequenceAnimation.Create(this)
            .AnimateRelativeSteps<float>(nameof(_stringProperty))
            .From(10).Offset(400, 0.01f).EndAnimate()
            .Play()
            .AwaitFinished();
        Assert.That(_stringProperty, Is.EqualTo(410));

        await KeyframeAnimation.Create(this)
            .SetDuration(0.1f)
            .AnimateKeys<float>(nameof(_stringProperty))
            .From(11).KeyframeTo(1, 400).EndAnimate()
            .Play()
            .AwaitFinished();
        Assert.That(_stringProperty, Is.EqualTo(400));

        await KeyframeAnimation.Create(this)
            .SetDuration(0.1f)
            .AnimateKeysBy<float>(nameof(_stringProperty))
            .From(12).KeyframeOffset(1, 400).EndAnimate()
            .Play()
            .AwaitFinished();
        Assert.That(_stringProperty, Is.EqualTo(412));

        await KeyframeAnimation.Create(this)
            .SetDuration(0.1f)
            .AnimateRelativeKeys<float>(nameof(_stringProperty))
            .From(13).KeyframeOffset(1, 400).EndAnimate()
            .Play()
            .AwaitFinished();
        Assert.That(_stringProperty, Is.EqualTo(413));
    }

    /*
     * Property
     */

    [TestRunner.Test(Description = "Property Rotate")]
    public async Task TweenPropertyRotate() {
        foreach (var property in new IProperty<float>[] { Properties.Rotate2D, CallbackProperties.Rotate2D }) {
            const float from = 1f;
            const float to = 3f;
            var node2D = await CreateNode2D();
            await CreateTweenPropertyVariants(node2D, property, from, to);

            var control = await CreateLabel();
            await CreateTweenPropertyVariants(control, property, from, to);

            var node = await CreateNode();
            await CreateIncompatibleNodeTweenPropertyVariants(node, property, from, to);
        }
    }

    [TestRunner.Test(Description = "Property PositionX, PositionY")]
    public async Task TweenPropertyPositionX_Y() {
        const float from = 90f;
        const float to = 120f;

        var node2D = await CreateNode2D();

        await CreateTweenPropertyVariants(node2D, Properties.PositionX, from, to);
        await CreateTweenPropertyVariants(node2D, Properties.PositionY, from, to);

        var control = await CreateLabel();
        await CreateTweenPropertyVariants(control, Properties.PositionX, from, to);
        await CreateTweenPropertyVariants(control, Properties.PositionY, from, to);

        var node = await CreateNode();
        await CreateIncompatibleNodeTweenPropertyVariants(node, Properties.PositionX, from, to);
        await CreateIncompatibleNodeTweenPropertyVariants(node, Properties.PositionY, from, to);
    }

    [TestRunner.Test(Description = "Property PositionBySizeX, PositionBySizeY")]
    public async Task TweenPropertyPositionPercentX_Y() {
        const float percentFrom = 0f;
        const float percentTo = 0.1f;
        const float initialPosition = 50f;
        const float endPosition = 120f;
        const int width = 300;

        var spriteX = await CreateSprite(width);
        spriteX.Position = new Vector2(initialPosition, 0);
        await SequenceAnimation.Create()
            .AnimateSteps(Properties.PositionBySizeX)
            .From(percentFrom)
            .To(percentTo, 0.1f)
            .To(percentTo * 2, 0.1f)
            .EndAnimate()
            .Play(spriteX)
            .AwaitFinished();
        Assert.That(spriteX.Position.X, Is.EqualTo(initialPosition + width * percentTo * 2));

        var spriteY = await CreateSprite(width);
        spriteY.Position = new Vector2(0, initialPosition);
        await SequenceAnimation.Create()
            .AnimateSteps(Properties.PositionBySizeY)
            .From(percentFrom)
            .To(percentTo, 0.1f)
            .To(percentTo * 2, 0.1f)
            .EndAnimate()
            .Play(spriteY)
            .AwaitFinished();
        Assert.That(spriteY.Position.Y, Is.EqualTo(initialPosition + width * percentTo * 2));

        var sprite2D = await CreateSprite(width);
        sprite2D.Position = new Vector2(initialPosition, initialPosition);
        await SequenceAnimation.Create()
            .AnimateSteps(Properties.PositionBySize2D)
            .From(new Vector2(percentFrom, percentFrom))
            .To(new Vector2(percentTo, percentTo), 0.1f)
            .To(new Vector2(percentTo * 2, percentTo * 2), 0.1f)
            .EndAnimate()
            .Play(sprite2D)
            .AwaitFinished();
        Assert.That(sprite2D.Position,
            Is.EqualTo(
                new Vector2(initialPosition + width * percentTo * 2, initialPosition + width * percentTo * 2)));

        var controlX = await CreateLabel(width);
        controlX.Position = new Vector2(initialPosition, 0);
        await SequenceAnimation.Create()
            .AnimateSteps(Properties.PositionBySizeX)
            .From(percentFrom)
            .To(percentTo, 0.1f)
            .To(percentTo * 2, 0.1f)
            .EndAnimate()
            .Play(controlX)
            .AwaitFinished();
        Assert.That(controlX.Position.X, Is.EqualTo(initialPosition + width * percentTo * 2));

        var controlY = await CreateLabel(width);
        controlY.Position = new Vector2(0, initialPosition);
        await SequenceAnimation.Create()
            .AnimateSteps(Properties.PositionBySizeY)
            .From(percentFrom)
            .To(percentTo, 0.1f)
            .To(percentTo * 2, 0.1f)
            .EndAnimate()
            .Play(controlY)
            .AwaitFinished();
        Assert.That(controlY.Position.Y, Is.EqualTo(initialPosition + width * percentTo * 2));

        var control2D = await CreateLabel(width);
        control2D.Position = new Vector2(initialPosition, initialPosition);
        await SequenceAnimation.Create()
            .AnimateSteps(Properties.PositionBySize2D)
            .From(new Vector2(percentFrom, percentFrom))
            .To(new Vector2(percentTo, percentTo), 0.1f)
            .To(new Vector2(percentTo * 2, percentTo * 2), 0.1f)
            .EndAnimate()
            .Play(control2D)
            .AwaitFinished();
        Assert.That(control2D.Position, Is.EqualTo(
            new Vector2(initialPosition + width * percentTo * 2, initialPosition + width * percentTo * 2)));

        var node = await CreateNode();
        await CreateIncompatibleNodeTweenPropertyVariants(node, Properties.PositionBySizeX, percentFrom, percentTo);
        await CreateIncompatibleNodeTweenPropertyVariants(node, Properties.PositionBySizeY, percentFrom, percentTo);
        await CreateIncompatibleNodeTweenPropertyVariants(node, Properties.PositionBySize2D, Vector2.One, Vector2.Zero);

        var node2D = await CreateNode2D();
        await CreateIncompatibleNodeTweenPropertyVariants(node2D, Properties.PositionBySizeX, percentFrom, percentTo);
        await CreateIncompatibleNodeTweenPropertyVariants(node2D, Properties.PositionBySizeY, percentFrom, percentTo);
        await CreateIncompatibleNodeTweenPropertyVariants(node2D, Properties.PositionBySize2D, Vector2.One, Vector2.Zero);
    }

    [TestRunner.Test(Description = "Property Scale2DX, Scale2DXByCallback, Scale2DY, Scale2DYByCallback")]
    public async Task TweenPropertyScaleX_Y() {
        const float from = 0.9f;
        const float to = 1.2f;
        foreach (var property in new IProperty<float>[] { Properties.Scale2Dx, CallbackProperties.Scale2Dx }) {
            var node2D = await CreateNode2D();
            await CreateTweenPropertyVariants(node2D, property, from, to);

            var control = await CreateLabel();
            await CreateTweenPropertyVariants(control, property, from, to);

            var node = await CreateNode();
            await CreateIncompatibleNodeTweenPropertyVariants(node, property, from, to);
        }

        foreach (var property in new IProperty<float>[] { Properties.Scale2Dy, CallbackProperties.Scale2Dy }) {
            var node2D = await CreateNode2D();
            await CreateTweenPropertyVariants(node2D, property, from, to);

            var control = await CreateLabel();
            await CreateTweenPropertyVariants(control, property, from, to);

            var node = await CreateNode();
            await CreateIncompatibleNodeTweenPropertyVariants(node, property, from, to);
        }
    }

    [TestRunner.Test(Description = "Property SkewX, SkewY")]
    public async Task TweenPropertySkewX_Y() {
        const float from = 0.9f;
        const float to = 1.2f;

        var node2D = await CreateNode2D();
        await CreateTweenPropertyVariants(node2D, Properties.Skew2DX, from, to);
        await CreateTweenPropertyVariants(node2D, Properties.Skew2DY, from, to);

        var label = await CreateLabel();
        await CreateIncompatibleNodeTweenPropertyVariants(label, Properties.Skew2DX, from, to);
        await CreateIncompatibleNodeTweenPropertyVariants(label, Properties.Skew2DY, from, to);

        var node = await CreateNode();
        await CreateIncompatibleNodeTweenPropertyVariants(node, Properties.Skew2DX, from, to);
        await CreateIncompatibleNodeTweenPropertyVariants(node, Properties.Skew2DY, from, to);
    }

    [TestRunner.Test(Description = "Property Position2D")]
    public async Task TweenPropertyPosition2d() {
        var from = Vector2.Zero;
        var to = new Vector2(23f, -12f);

        var node2D = await CreateNode2D();
        await CreateTweenPropertyVariants(node2D, Properties.Position2D, from, to);

        var control = await CreateLabel();
        await CreateTweenPropertyVariants(control, Properties.Position2D, from, to);

        var node = await CreateNode();
        await CreateIncompatibleNodeTweenPropertyVariants(node, Properties.Position2D, from, to);
    }


    [TestRunner.Test(Description = "Property Scale2D")]
    public async Task TweenPropertyScale2d() {
        foreach (var property in new IProperty<Vector2>[] { Properties.Scale2D, CallbackProperties.Scale2D }) {
            var from = Vector2.One;
            var to = new Vector2(23f, -12f);

            var node2D = await CreateNode2D();
            await CreateTweenPropertyVariants(node2D, property, from, to);

            var control = await CreateLabel();
            await CreateTweenPropertyVariants(control, property, from, to);

            var node = await CreateNode();
            await CreateIncompatibleNodeTweenPropertyVariants(node, property, from, to);
        }
    }

    [TestRunner.Test(Description = "Property modulate")]
    public async Task TweenPropertyColor() {
        var from = new Color(0.1f, 0.1f, 0.1f, 0.1f);
        var fromA = new Color(0.1f, 0.1f, 0.1f, 1f);
        var fromB = new Color(0.1f, 0.1f, 1f, 0.1f);
        var fromG = new Color(0.1f, 1f, 0.1f, 0.1f);
        var fromR = new Color(1f, 0.1f, 0.1f, 0.1f);
        var to = new Color(1f, 1f, 1f, 1f);

        foreach (var node in new CanvasItem[] { await CreateNode2D(), await CreateSprite(), await CreateLabel() }) {
            await CreateTweenPropertyVariants(node, Properties.Modulate, from, to);
            Assert.That(node.Modulate, Is.EqualTo(to));

            node.Modulate = from;
            await CreateTweenPropertyVariants(node, Properties.ModulateR, 0f, 1f);
            Assert.That(node.Modulate, Is.EqualTo(fromR));

            node.Modulate = from;
            await CreateTweenPropertyVariants(node, Properties.ModulateG, 0f, 1f);
            Assert.That(node.Modulate, Is.EqualTo(fromG));

            node.Modulate = from;
            await CreateTweenPropertyVariants(node, Properties.ModulateB, 0f, 1f);
            Assert.That(node.Modulate, Is.EqualTo(fromB));

            node.Modulate = from;
            await CreateTweenPropertyVariants(node, Properties.Opacity, 0f, 1f);
            Assert.That(node.Modulate, Is.EqualTo(fromA));
        }

        await CreateIncompatibleNodeTweenPropertyVariants(await CreateNode(), Properties.Modulate, from, to);
        await CreateIncompatibleNodeTweenPropertyVariants(await CreateNode(), Properties.ModulateR, 0f, 1f);
        await CreateIncompatibleNodeTweenPropertyVariants(await CreateNode(), Properties.ModulateG, 0f, 1f);
        await CreateIncompatibleNodeTweenPropertyVariants(await CreateNode(), Properties.ModulateB, 0f, 1f);
        await CreateIncompatibleNodeTweenPropertyVariants(await CreateNode(), Properties.Opacity, 0f, 1f);
    }


    [TestRunner.Test(Description = "IndexedProperty compatibleWith")]
    public async Task IndexedPropertyCompatibleWithTests() {
            
        IndexedSingleProperty.Cache.Clear();

        Assert.Throws<ArgumentException>(() => IndexedSingleProperty.Create<Vector2>("x", typeof(Array)));

        var a1 = IndexedSingleProperty.Create<Vector2>("position", typeof(Node));
        Assert.That(a1.IsCompatibleWith(new Node()));
        Assert.That(a1.IsCompatibleWith(new Node2D()));

        var c1 = IndexedSingleProperty.Create<Vector2>("x", typeof(Control));
        Assert.That(!c1.IsCompatibleWith(new Node()));
        Assert.That(!c1.IsCompatibleWith(new Node2D()));
        Assert.That(c1.IsCompatibleWith(new Control()));
        Assert.That(c1.IsCompatibleWith(new Label()));

    }

    public Vector2 follow;
        
    [TestRunner.Test(Description = "Custom IndexedProperty + test cache")]
    public async Task IndexedPropertyTests() {
            
        IndexedSingleProperty.Cache.Clear();

        var a1 = (IndexedSingleProperty<Vector2>)"position";
        var a2 = (IndexedSingleProperty<Vector2>)"position";
        Assert.That(a1.IsCompatibleWith(new Node()));
        Assert.That(a1.IsCompatibleWith(new Node2D()));

        Assert.That(a1 == a2);

        var b1 = (IndexedSingleProperty<Vector2>)"x";
        var b2 = (IndexedSingleProperty<Vector2>)"x";
        Assert.That(b1 == b2);
        Assert.That(a1 != b1);

        var c1 = IndexedSingleProperty.Create<Vector2>("x", typeof(Control));
        var c2 = IndexedSingleProperty.Create<Vector2>("x", typeof(Node2D));
        Assert.That(c1 != c2);
    }

    [TestRunner.Test(Description = "Custom IndexedProperty + test cache")]
    public async Task TweenPropertyBasicPropertyString() {
            
        IndexedSingleProperty.Cache.Clear();
            
        follow = Vector2.Zero;
        await CreateTweenPropertyVariants(this, (IndexedSingleProperty<Vector2>)"follow", Vector2.Zero, Vector2.Up);
        Assert.That(follow, Is.EqualTo(Vector2.Up));

        follow = Vector2.Zero;
        await CreateTweenPropertyVariants(this, IndexedSingleProperty.Create<Vector2>(nameof(follow), typeof(Node)), Vector2.Zero,
            Vector2.Up);
        Assert.That(follow, Is.EqualTo(Vector2.Up));
            
        Assert.That(IndexedSingleProperty.Cache.Count, Is.EqualTo(1));
    }

    /**
         * Callbacks
         */
    [TestRunner.Test(Description = "non IndexedProperty using the SetVale as a callback tween")]
    public async Task SequenceStepsToWithCallbackProperty() {
        List<DebugStep<float>> steps = new List<DebugStep<float>>();
        var sprite = await CreateSprite();
        var callbackProperty = new CallbackProperty();
        Assert.That(callbackProperty.Calls, Is.EqualTo(0));

        await CreateTweenPropertyVariants(sprite, callbackProperty, 0, -90);

        Assert.That(callbackProperty.Calls, Is.GreaterThan(0));
        Assert.That(sprite.Position.X, Is.EqualTo(-90));
    }

    private class CallbackProperty : IProperty<float> {
        public int Calls = 0;

        public float GetValue(Node node) {
            return (float)node.GetIndexed("position:x");
        }

        public bool IsCompatibleWith(Node node) {
            return true;
        }

        public void SetValue(Node target, float value) {
            Calls++;
            target.SetIndexed("position:x", value);
        }

        public string GetPropertyName(Node node) {
            return "position:x";
        }
    }

    private async Task CreateTweenPropertyVariants<[MustBeVariant] T>(Node node, IProperty<T> property, T from, T to) {
        property.SetValue(node, from);
        Assert.That(property.GetValue(node), Is.EqualTo(from));
        List<DebugStep<T>> steps = new List<DebugStep<T>>();
        var sequence = SequenceAnimation.Create()
            .AnimateSteps(property)
            .SetDebugSteps(steps)
            .From(from)
            .To(to, 0.1f, Easings.BackIn)
            .EndAnimate();

        // With Play()
        await sequence.Play(node).AwaitFinished();
        Assert.That(property.GetValue(node), Is.EqualTo(to));

        Assert.That(steps.Count, Is.EqualTo(1));
        AssertStep(steps[0], from, to, 0f, 0.1f, Easings.BackIn);
        Assert.That(property.GetValue(node), Is.EqualTo(to));
    }

    private async Task CreateKeyframePropertyVariants<[MustBeVariant] T>(Node node, IProperty<T> property, T from, T to) {
        property.SetValue(node, from);
        Assert.That(property.GetValue(node), Is.EqualTo(from));
        List<DebugStep<T>> steps = new List<DebugStep<T>>();
        var sequence = KeyframeAnimation.Create()
            .SetDuration(1f)
            .AnimateKeys(property)
            .From(from)
            .KeyframeTo(0.1f, to, Easings.BackIn)
            .SetDebugSteps(steps)
            .EndAnimate();

        // With Play()
        await sequence.Play(node).AwaitFinished();
        Assert.That(property.GetValue(node), Is.EqualTo(to));

        Assert.That(steps.Count, Is.EqualTo(1));
        AssertStep(steps[0], from, to, 0f, 0.1f, Easings.BackIn);
        Assert.That(property.GetValue(node), Is.EqualTo(to));
    }

    private async Task CreateIncompatibleNodeTweenPropertyVariants<T>(Node node, IProperty<T> property, T from, T to) {
        var seq = SequenceAnimation.Create()
            .AnimateSteps(property)
            .To(to, 0.1f, Easings.BackIn)
            .EndAnimate();
        Assert.That(seq.IsCompatibleWith(node), Is.False);
            
        var e = Assert.Throws<NodeNotCompatibleWithPropertyException>(() => seq.Play(node));

        var keyf = KeyframeAnimation.Create()
            .SetDuration(1f)
            .AnimateKeys(property)
            .From(from)
            .KeyframeTo(0.1f, to, Easings.BackIn)
            .EndAnimate();
        Assert.That(keyf.IsCompatibleWith(node), Is.False);
            
        var e2 = Assert.Throws<NodeNotCompatibleWithPropertyException>(() => keyf.Play(node));
    }

    private async Task CreateIncompatibleNodeTweenPropertyVariants<T>(Node node, Func<Node, IProperty<T>> property, T from, T to) {
        var seq = SequenceAnimation.Create()
            .AnimateSteps(property)
            .To(to, 0.1f, Easings.BackIn)
            .EndAnimate();
        Assert.That(seq.IsCompatibleWith(node), Is.False);

        var e = Assert.Throws<NodeNotCompatibleWithPropertyException>(() => seq.Play(node));
            
        var keyf = KeyframeAnimation.Create()
            .SetDuration(1f)
            .AnimateKeys(property)
            .From(from)
            .KeyframeTo(0.1f, to, Easings.BackIn)
            .EndAnimate();
        Assert.That(keyf.IsCompatibleWith(node), Is.False);
            
        var e2 = Assert.Throws<NodeNotCompatibleWithPropertyException>(() => keyf.Play(node));
    }

    [TestRunner.Test]
    public async Task PivotSpriteRestoreTests2() {
        var sprite = await CreateSprite();
        var original = new Vector2(2f, 3f);
        sprite.GlobalPosition = original;
        sprite.Offset = original;
        sprite.Scale = original;
        // GD.Print(sprite.Offset);
        // GD.Print(sprite.GlobalPosition);
        await this.AwaitProcessFrame();

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

    private static void AssertStep<[MustBeVariant] T>(DebugStep<T> step, T from, T to, float start, float duration, IEasing easing) {
        Assert.That(step.From, Is.EqualTo(from).Within(0.0000001f));
        Assert.That(step.To, Is.EqualTo(to).Within(0.0000001f));
        Assert.That(step.Start, Is.EqualTo(start));
        Assert.That(step.Duration, Is.EqualTo(duration).Within(0.0000001f));
        Assert.That(step.Easing, Is.EqualTo(easing));
    }
}