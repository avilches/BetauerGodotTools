using System.Threading.Tasks;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests.Animation {
    [TestFixture]
    public class ActionTweenTests : NodeTest {
        [SetUp]
        public void SetUp() {
            Engine.TimeScale = 1;
        }

        [TearDown]
        public void TearDown() {
            Engine.TimeScale = 1;
        }

        [Test]
        public async Task ScheduleCallback() {
            var tween = await CreateTween();
            int x = 0;
            tween.ScheduleCallback(0.00f, () => x++);
            tween.ScheduleCallback(0.01f, () => x++);
            tween.ScheduleCallback(0.02f, () => x++);
            Assert.That(tween.GetPendingActions().Count, Is.EqualTo(3));
            await Task.Delay(50);
            Assert.That(x, Is.EqualTo(3));
            Assert.That(tween.GetPendingActions().Count, Is.EqualTo(0));
        }

        [Test]
        public async Task InterpolateAction() {
            var tween = await CreateTween();
            int x = 0;
            tween.InterpolateAction<int>(0, 1, 0.01f, Tween.TransitionType.Linear, Tween.EaseType.InOut, 0f,
                (p) => x = x + p);
            Assert.That(tween.GetPendingObjects().Count, Is.EqualTo(1));
            await Task.Delay(50);
            Assert.That(x, Is.EqualTo(2));
            Assert.That(tween.GetPendingObjects().Count, Is.EqualTo(0));
        }
    }
}