using System.Linq;
using Godot;
using NUnit.Framework;
using Betauer.Animation;

namespace Betauer.Tests.Animation {
    [TestFixture]
    public class TemplateTests {
        [SetUp]
        public void SetUp() {
            Engine.TimeScale = 10;
        }

        [TearDown]
        public void TearDown() {
            Engine.TimeScale = 1;
        }

        [Test]
        public void TemplateNamesCheck() {
            Assert.That(Template.Bounce, Is.EqualTo(Template.Get("bOunce")));
            Assert.That(Template.Flash, Is.EqualTo(Template.Get("fLash")));
            Assert.That(Template.HeadShake, Is.EqualTo(Template.Get("hEadshake")));
            Assert.That(Template.HeartBeat, Is.EqualTo(Template.Get("hEartbeat")));
            Assert.That(Template.Jello, Is.EqualTo(Template.Get("jEllo")));
            Assert.That(Template.Pulse, Is.EqualTo(Template.Get("pUlse")));
            Assert.That(Template.RubberBand, Is.EqualTo(Template.Get("rUbberband")));
            Assert.That(Template.Shake, Is.EqualTo(Template.Get("sHake")));
            Assert.That(Template.ShakeX, Is.EqualTo(Template.Get("sHakex")));
            Assert.That(Template.ShakeY, Is.EqualTo(Template.Get("sHakey")));
            Assert.That(Template.Swing, Is.EqualTo(Template.Get("sWing")));
            Assert.That(Template.Tada, Is.EqualTo(Template.Get("tAda")));
            Assert.That(Template.Wobble, Is.EqualTo(Template.Get("wObble")));
            Assert.That(Template.BackInDown, Is.EqualTo(Template.Get("bAckINdowN")));
            Assert.That(Template.BackInUp, Is.EqualTo(Template.Get("bAckInp")));
            Assert.That(Template.BackInLeft, Is.EqualTo(Template.Get("bAckinLeft")));
            Assert.That(Template.BackInRight, Is.EqualTo(Template.Get("bAckinRight")));
        }

        public TweenSequenceTemplate CreateTemplate() {
            return TweenSequenceBuilder.Create()
                .SetProcessMode(Tween.TweenProcessMode.Idle)
                .SetDuration(0.5f)
                .AnimateKeysBy(property: Property.PositionY)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1f, +4f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.ScaleY)
                .KeyframeTo(0.20f, 1)
                .KeyframeTo(0.50f, 1)
                .EndAnimate()
                .BuildTemplate();
        }

        [Test]
        public void TweenPlayerApplyPattern() {
            Node2D node = new Sprite();
            TweenSequenceTemplate tem = CreateTemplate();
            var player = TweenPlayer.With(node, tem);

            Assert.That(player.TweenSequences.Count, Is.EqualTo(1));

            ITweenSequence imported = player.TweenSequences[0];

            Assert.That(imported.DefaultTarget, Is.EqualTo(node));

            // the node contains a Tween child
            var tween = node.GetChild<Tween>(0);
            Assert.That(tween, Is.Not.Null);
            Assert.That(player.Tween, Is.EqualTo(tween));

            Assert.That(imported, Is.Not.EqualTo(tem));

            // the imported sequence has all the data from template
            Assert.That(imported.Duration, Is.EqualTo(tem.Duration));
            Assert.That(imported.Loops, Is.EqualTo(tem.Loops));
            Assert.That(imported.Speed, Is.EqualTo(tem.Speed));
            Assert.That(imported.DefaultProperty, Is.EqualTo(tem.DefaultProperty));
            Assert.That(imported.ProcessMode, Is.EqualTo(tem.ProcessMode));
            Assert.That(imported.TweenList, Is.EqualTo(tem.TweenList));
        }

        [Test]
        public void TweenPlayerApplyPatternWithDuration() {
            Node2D node = new Sprite();
            TweenSequenceTemplate tem = CreateTemplate();
            var player = TweenPlayer.With(node, tem, 100);

            Assert.That(player.TweenSequences.Count, Is.EqualTo(1));

            ITweenSequence imported = player.TweenSequences[0];

            Assert.That(imported.DefaultTarget, Is.EqualTo(node));
            Assert.That(imported.Duration, Is.EqualTo(100));
        }

        [Test]
        public void WhenImportATemplateAndDataIsChanged_theTemplateDataIsNotChanged() {
            TweenSequenceTemplate tem = CreateTemplate();
            TweenPlayer player = new TweenPlayer()
                .ImportTemplate(tem)
                .SetDuration(tem.Duration + 2)
                .SetLoops(tem.Loops + 2)
                .SetSpeed(tem.Speed + 2)
                .SetProcessMode(tem.ProcessMode == Tween.TweenProcessMode.Physics
                    ? Tween.TweenProcessMode.Idle
                    : Tween.TweenProcessMode.Physics)
                .EndSequence()
                .SetAutoKill(true);

            ITweenSequence imported = player.TweenSequences[0];

            Assert.That(imported.Loops, Is.Not.EqualTo(tem.Loops));
            Assert.That(imported.Duration, Is.Not.EqualTo(tem.Duration));
            Assert.That(imported.Speed, Is.Not.EqualTo(tem.Speed));
            Assert.That(imported.ProcessMode, Is.Not.EqualTo(tem.ProcessMode));

            TweenSequenceTemplate tem2 = CreateTemplate();

            Assert.That(tem.Loops, Is.EqualTo(tem2.Loops));
            Assert.That(tem.Duration, Is.EqualTo(tem2.Duration));
            Assert.That(tem.Speed, Is.EqualTo(tem2.Speed));
            Assert.That(tem.ProcessMode, Is.EqualTo(tem2.ProcessMode));
        }

        [Test]
        public void WhenImportATemplateAndAddANewTween_theTweenListIsClonedToAvoidCorruptTheOriginalTemplate() {
            TweenSequenceTemplate tem = CreateTemplate();
            TweenPlayer player = new TweenPlayer()
                .ImportTemplate(tem)
                .AnimateKeys(null, Property.Modulate)
                .KeyframeTo(1, Colors.Aqua)
                .EndAnimate()
                .EndSequence()
                .SetAutoKill(true);

            Assert.That(player.TweenSequences.Count, Is.EqualTo(1));
            ITweenSequence imported = player.TweenSequences[0];

            // when an imported template TweenList is updated, a new cloned TweenList is created
            Assert.That(imported.TweenList, Is.Not.EqualTo(tem.TweenList));
            Assert.That(tem.TweenList.Count, Is.EqualTo(1));
            Assert.That(imported.TweenList.Count, Is.EqualTo(2));

            Assert.That(imported.TweenList.First(), Is.EqualTo(tem.TweenList.First()));
        }

        [Test]
        public void WhenImportATemplateAndAddAParallelTween_theTweenListIsClonedToAvoidCorruptTheOriginalTemplate() {
            TweenSequenceTemplate tem = CreateTemplate();
            TweenPlayer player = new TweenPlayer()
                .ImportTemplate(tem)
                // The parallel makes the difference because it adds the tween to the last group instead
                .Parallel()
                .AnimateKeys(null, Property.Modulate)
                .KeyframeTo(1, Colors.Aqua)
                .EndAnimate()
                .EndSequence()
                .SetAutoKill(true);

            Assert.That(player.TweenSequences.Count, Is.EqualTo(1));
            ITweenSequence imported = player.TweenSequences[0];

            // when an imported template TweenList is updated, a new cloned TweenList is created
            Assert.That(imported.TweenList, Is.Not.EqualTo(tem.TweenList));
            Assert.That(tem.TweenList.Count, Is.EqualTo(1));
            Assert.That(imported.TweenList.Count, Is.EqualTo(1));

            Assert.That(tem.TweenList.First().Count, Is.EqualTo(2));
            Assert.That(imported.TweenList.First().Count, Is.EqualTo(3));

            Assert.That(imported.TweenList.First().First(), Is.EqualTo(tem.TweenList.First().First()));
        }
    }
}