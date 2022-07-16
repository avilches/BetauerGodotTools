using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Betauer.Memory;
using Betauer.Signal;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Object = Godot.Object;

namespace Betauer.Tests {
    [TestFixture]
    public class SignalExtensionsTests : Node {
        [SetUp]
        public void Setup() {
            DefaultObjectWatcher.Instance.Dispose();
            LoggerFactory.SetTraceLevel(typeof(ObjectWatcher), TraceLevel.All);
        }

        private int _executed1 = 0;
        public void Pressed1() {
            _executed1++;
        }

        [Test(Description = "Signal connect and disconnect (with method)")]
        public async Task SignalToMethodTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();

            SignalHandler p1 = b1.OnPressed(Pressed1);
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p1.Target, Is.EqualTo(this));

            b1.EmitSignal("pressed");
            Assert.That(_executed1, Is.EqualTo(1));
            
            p1.Disconnect();
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p1.IsConnected(), Is.False);

            b1.EmitSignal("pressed");
            Assert.That(_executed1, Is.EqualTo(1));

            p1.Connect();
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p1.IsConnected(), Is.True);

            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            Assert.That(_executed1, Is.EqualTo(3));
            
            b1.Free();
            Assert.That(p1.IsValid(), Is.False);
            Assert.That(p1.IsConnected(), Is.False);
        }

        [Test(Description = "Signal with object method is not watched")]
        public async Task SignalToMethodIsNotWatchedTests() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            
            Assert.That(DefaultObjectWatcher.Instance.WatchingCount, Is.EqualTo(0));
            
            SignalHandler p1 = b1.OnPressed(Pressed1);
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p1.Target, Is.EqualTo(this));
            Assert.That(DefaultObjectWatcher.Instance.WatchingCount, Is.EqualTo(0));
            
            p1.Unwatch();
            Assert.That(DefaultObjectWatcher.Instance.WatchingCount, Is.EqualTo(0));
        }

        [Test(Description = "Signal connect and disconnect (with lambda)")]
        public async Task SignalToLambdaTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var executed1 = 0;

            SignalHandler p1 = b1.OnPressed(() => executed1 ++);
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p1.Target, Is.InstanceOf<IObjectWatched>());

            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));
            
            p1.Disconnect();
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p1.IsConnected(), Is.False);

            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));

            p1.Connect();
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p1.IsConnected(), Is.True);

            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(3));
            
            b1.Free();
            Assert.That(p1.IsValid(), Is.False);
            Assert.That(p1.IsConnected(), Is.False);
        }

        [Test(Description = "Signal lambda oneShot should only work one and auto freed")]
        public async Task SignalToLambdaOneShotTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var executed1 = 0;

            SignalHandler p1 = b1.OnPressed(() => executed1 ++, true);
            Assert.That(p1.Target, Is.InstanceOf<IObjectWatched>());

            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));
            
            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            // More executions are ignored
            Assert.That(executed1, Is.EqualTo(1));

            // It's not marked as freed because the origin is still alive
            Assert.That(p1.Target is IObjectWatched ww1 && ww1.MustBeFreed(), Is.False);
            Assert.That(p1.IsConnected(), Is.False);
            // It's still valid because the free is called with deferred
            Assert.That(IsInstanceValid(p1.Target), Is.True);

            // freed in the next frame
            await this.AwaitIdleFrame();
            Assert.That(p1.Target is IObjectWatched ww2 && ww2.MustBeFreed(), Is.False);
            Assert.That(IsInstanceValid(p1.Target), Is.False);
        }

        [Test(Description = "Signal lambda deferred should work next frame")]
        public async Task SignalToLambdaDeferredTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var executed1 = 0;

            SignalHandler p1 = b1.OnPressed(() => executed1 ++, false, true);
            Assert.That(p1.Target, Is.InstanceOf<IObjectWatched>());

            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(0));
            
            await this.AwaitIdleFrame();
            Assert.That(executed1, Is.EqualTo(3));
        }

        [Test(Description = "Signal lambda deferred and one shot should work next frame and no more")]
        public async Task SignalToLambdaOneShotDeferredTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var executed1 = 0;

            SignalHandler p1 = b1.OnPressed(() => executed1 ++, true, true);
            Assert.That(p1.Target, Is.InstanceOf<IObjectWatched>());

            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(0));
            
            await this.AwaitIdleFrame();
            Assert.That(executed1, Is.EqualTo(1));
            // Use CallDeferred() in an idle frame is called immediately, not in the next one
            Assert.That(IsInstanceValid(p1.Target), Is.False);

        }

        [Test(Description = "Signal lambda is marked as mustBeFreed = true if origin is free or disposed")]
        public async Task SignalToLambdaMarkedAsMustBeFreedTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();

            SignalHandler p1 = b1.OnPressed(() => { }, true);
            Assert.That(p1.Target is IObjectWatched www2 && www2.MustBeFreed(), Is.False);
            
            b1.Dispose();
            Assert.That(p1.Target is IObjectWatched www3 && www3.MustBeFreed(), Is.True);
        }

        [Test(Description = "Signal lambda is watched and unwatched")]
        public async Task SignalToLambdaIsWatchedAndUnwatched() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();

            Assert.That(DefaultObjectWatcher.Instance.WatchingCount, Is.EqualTo(0));
            SignalHandler p1 = b1.OnPressed(() => { }, true);
            Assert.That(DefaultObjectWatcher.Instance.WatchingCount, Is.EqualTo(1));
            
            p1.Unwatch();
            Assert.That(DefaultObjectWatcher.Instance.WatchingCount, Is.EqualTo(0));
        }

        [Test(Description = "Signal with 1 parameter")]
        public async Task Signal1ParameterTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var toggled1 = new List<bool>();

            b1.OnToggled((tog) => toggled1.Add(tog));
            
            b1.EmitSignal("pressed");
            Assert.That(toggled1, Is.EqualTo(new bool[]{}));

            b1.Pressed = true;
            Assert.That(toggled1.ToArray(), Is.EqualTo(new []{true}));
            b1.Pressed = false;
            Assert.That(toggled1.ToArray(), Is.EqualTo(new []{true, false}));
        }
    }
}