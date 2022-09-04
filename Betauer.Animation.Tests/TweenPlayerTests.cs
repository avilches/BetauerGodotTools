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
    public class TweenPlayerTests : NodeTest {
        [SetUp]
        public void SetUp() {
            Engine.TimeScale = 10;
        }

        [TearDown]
        public void TearDown() {
            Engine.TimeScale = 1;
        }

        [Test(Description = "Test OnAction method is executed immediately")]
        public async Task OnStart() {

            var sprite = await CreateSprite();
            Property.Opacity.SetValue(sprite, 1f);
            Assert.That(sprite.Modulate.a, Is.EqualTo(1f));
            
            var t = Sequence.Create(sprite)
                .Pause(1f)
                .AnimateSteps(Property.Opacity)
                .To(12, 0.1f, Easings.BackIn)
                .To(12, 0.1f, Easings.BackIn)
                .EndAnimate()
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .Execute();
            
            Assert.That(sprite.Modulate.a, Is.EqualTo(0f));
        }

        [Test(Description = "Sequence await works, multiple executions, no disposed")]
        public async Task SequenceMultipleExecutions() {
            var l1 = 0;

            // when created, it's not running
            var t = Sequence.Create(this)
                .Callback(() => l1++);

            // When started, it's running
            SceneTreeTween sceneTreeTween = t.Execute().SetLoops(2);
            Assert.That(sceneTreeTween.IsRunning, Is.True);

            // Await, then it's not running and results are ok
            await sceneTreeTween.AwaitFinished();
            Assert.That(sceneTreeTween.IsRunning, Is.False);
            Assert.That(l1, Is.EqualTo(2));

            // If start + await again, it will work properly
            sceneTreeTween = t.Execute();
            Assert.That(sceneTreeTween.IsRunning, Is.True);
            await sceneTreeTween.AwaitFinished();
            Assert.That(sceneTreeTween.IsRunning, Is.False);
            Assert.That(l1, Is.EqualTo(3));

        }

        [Test(Description = "Test values in steps")]
        public void TweenSequenceSteps() {
            var sequence = Sequence.Create(new Sprite())
                .AnimateSteps(Property.PositionX)
                .To(120, 0.1f, Easings.BackIn)
                .To(-90, 0.2f)
                .EndAnimate();

            var tweener = GetTweener<PropertyKeyStepTweener<float>>(sequence, 0);
            var steps = tweener.CreateStepList();
            Assert.That(steps[0].Duration, Is.EqualTo(0.1f));
            Assert.That(steps[0].Easing, Is.EqualTo(Easings.BackIn));
            Assert.That(steps[1].Duration, Is.EqualTo(0.2f));
            Assert.That(steps[1].Easing, Is.Null);
        }

        [Test(Description = "Test values in keyframes")]
        public void TweenSequenceKeyframe() {
            var sequence = Sequence.Create(new Sprite())
                .AnimateKeys(Property.PositionX)
                .Duration(0.5f)
                .KeyframeTo(0.1f, 120f, Easings.BackIn)
                .KeyframeTo(0.6f, -90f)
                .EndAnimate();

            var tweener = GetTweener<PropertyKeyPercentTweener<float>>(sequence, 0);
            Assert.That(tweener.AllStepsDuration, Is.EqualTo(0.5f));

            var steps = tweener.CreateStepList();
            Assert.That(steps[0].Percent, Is.EqualTo(0.1f));
            Assert.That(steps[0].Easing, Is.EqualTo(Easings.BackIn));

            Assert.That(steps[1].Percent, Is.EqualTo(0.6f));
            Assert.That(steps[1].Easing, Is.Null);
        }

        private T GetTweener<T>(Sequence sequence, int group, int pos = 0) where T : class {
            return sequence.TweenList.ToList()[group].ToList()[pos] as T;
        }
    }
}