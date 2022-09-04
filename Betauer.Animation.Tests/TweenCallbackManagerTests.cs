using System;
using System.Threading.Tasks;
using Betauer.Animation.Tween;
using Betauer.Memory;
using Betauer.Signal;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Animation.Tests {
    [TestFixture]
    public class TweenCallbackManagerTests : NodeTest {
        [SetUp]
        public void Setup() {
            DefaultObjectWatcherRunner.Instance.Dispose();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));
            DefaultObjectWatcherRunner.Instance = new ObjectWatcherRunner();
            Engine.TimeScale = 1;
        }
        
        [Test]
        public async Task TweenCallbackActionTests() {
            // All empty
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));
            Assert.That(DefaultTweenCallbackManager.Instance.ActionsPerSceneTree.Count, Is.EqualTo(0));

            // Add 3 tweens with 2 actions
            var tween = CreateTween();
            var x = 0;
            Action Recycled = () => x++;
            tween.TweenCallbackAction(Recycled);
            tween.TweenCallbackAction(Recycled).SetDelay(0.01f);
            tween.TweenCallbackAction(() => { x++; }).SetDelay(0.02f);
            tween.SetLoops(2);
            // There are 3 method tween, but only 2 different action
            Assert.That(DefaultTweenCallbackManager.Instance.ActionsPerSceneTree.Count, Is.EqualTo(1));
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));

            // tween is not executed yet, so it's still valid and ...
            Assert.That(x, Is.EqualTo(0));
            Assert.That(tween.IsValid(), Is.True);
            // ... nothing happens if Consume() the watching
            DefaultObjectWatcherRunner.Instance.Consume();
            Assert.That(DefaultTweenCallbackManager.Instance.ActionsPerSceneTree.Count, Is.EqualTo(1));
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));

            // execute tween
            await tween.AwaitFinished();
            Assert.That(x, Is.EqualTo(6));
            
            // wait until tween is not valid
            while (tween.IsValid()) {
                await tween.AwaitIdleFrame();
            }
            
            // Actions and watching are still there...
            Assert.That(DefaultTweenCallbackManager.Instance.ActionsPerSceneTree.Count, Is.EqualTo(1));
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));

            // ...until Consume() clean them
            DefaultObjectWatcherRunner.Instance.Consume();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task TweenCallbackActionKillBeforeFinishTest() {
            // All empty
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));
            Assert.That(DefaultTweenCallbackManager.Instance.ActionsPerSceneTree.Count, Is.EqualTo(0));

            // Add 2 tweens in an infinite loop
            var tween = CreateTween();
            var x = 0;
            var y = 0;
            tween.TweenCallbackAction(() => x++);
            tween.TweenCallbackAction(() => y++);
            tween.TweenInterval(0.1f);
            tween.SetLoops();
            Assert.That(DefaultTweenCallbackManager.Instance.ActionsPerSceneTree.Count, Is.EqualTo(1));
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));

            // tween working as expected
            await Task.Delay(TimeSpan.FromSeconds(0.2f));
            Assert.That(x, Is.LessThanOrEqualTo(5));
            Assert.That(y, Is.LessThanOrEqualTo(5));

            // Consume() doesn't do anything because tween is working 
            DefaultObjectWatcherRunner.Instance.Consume();
            Assert.That(DefaultTweenCallbackManager.Instance.ActionsPerSceneTree.Count, Is.EqualTo(1));
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));
            
            // kill
            tween.Kill();
            while (tween.IsValid()) {
                await tween.AwaitIdleFrame();
            }

            // Actions and watching are still there...
            Assert.That(DefaultTweenCallbackManager.Instance.ActionsPerSceneTree.Count, Is.EqualTo(1));
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));

            // ...until Consume() clean them
            DefaultObjectWatcherRunner.Instance.Consume();
            Assert.That(DefaultTweenCallbackManager.Instance.ActionsPerSceneTree.Count, Is.EqualTo(0));
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));

        }

        [Test]
        public async Task InterpolateActionTest() {
            // All empty
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));
            Assert.That(DefaultTweenCallbackManager.Instance.ActionsPerSceneTree.Count, Is.EqualTo(0));

            // Add 2 tweens in an infinite loop
            var tween = CreateTween();
            var x = 0;
            var y = 0;
            tween.TweenInterpolateAction(0f, 1f, 0.1f, value => x++);
            tween.TweenInterpolateAction(0f, 1f, 0.1f, value => y++);
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(2));
            
            // ... nothing happens if Consume() the watching
            DefaultObjectWatcherRunner.Instance.Consume();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(2));

            // execute tween
            await tween.AwaitFinished();
            Assert.That(x, Is.GreaterThan(0));
            Assert.That(y, Is.GreaterThan(0));
            
            // wait until tween is not valid
            while (tween.IsValid()) {
                await tween.AwaitIdleFrame();
            }
            DefaultObjectWatcherRunner.Instance.Consume();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));
        }
    }
}