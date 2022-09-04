using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Memory;
using Betauer.Signal;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    [Only]
    public class SignalExtensionsTests : Node {
        [SetUp]
        public void Setup() {
            DefaultObjectWatcherRunner.Instance.Dispose();
            LoggerFactory.SetTraceLevel(typeof(Consumer), TraceLevel.All);
        }

        [Test(Description = "Unwatch/watch")]
        public async Task UnwatchWatch() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));

            SignalHandler p1 = b1.OnPressed(() => { });
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));

            // Another identical Watch is added...
            p1.Watch();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));
            
            p1.Unwatch();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));

            // Another Unwatch is ignored
            p1.Unwatch();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));

            p1.Watch();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));
        }

        public class Dummy : Object {
            public int Executed1 = 0;
            public void Pressed1() {
                Executed1++;
            }
            
        }
        [Test(Description = "Signal method works + watch owner")]
        public async Task SignalToMethodTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();

            var o = new Dummy();
            b1.OnPressed(o.Pressed1);
            b1.EmitSignal("pressed");
            Assert.That(o.Executed1, Is.EqualTo(1));
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));


        }

        [Test(Description = "Signal lambda works + watch signal origin")]
        public async Task SignalLambdaWorksAndWatchTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var executed1 = 0;

            SignalHandler p1 = b1.OnPressed(() => executed1++);
            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));

            // Watch object still there
            DefaultObjectWatcherRunner.Instance.Consume();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));
            
            // When signal origin is freed, watch object is disposed
            b1.Free();
            DefaultObjectWatcherRunner.Instance.Consume();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));
        }


        [Test(Description = "Signal connect and disconnect from origin (using lambda)")]
        public async Task ConnectAndDisconnectLambdaTests() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var executed1 = 0;

            SignalHandler p1 = b1.OnPressed(() => executed1 ++);
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p1.IsConnected(), Is.True);

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
            Assert.That(p1.IsConnected(), Is.False);
        }

        [Test(Description = "Signal connect and disconnect from origin (method)")]
        public async Task ConnectAndDisconnectMethodTests() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();

            var o = new Dummy();
            SignalHandler p1 = b1.OnPressed(o.Pressed1);
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p1.IsConnected(), Is.True);

            b1.EmitSignal("pressed");
            Assert.That(o.Executed1, Is.EqualTo(1));

            p1.Disconnect();
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p1.IsConnected(), Is.False);

            b1.EmitSignal("pressed");
            Assert.That(o.Executed1, Is.EqualTo(1));

            p1.Connect();
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p1.IsConnected(), Is.True);

            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            Assert.That(o.Executed1, Is.EqualTo(3));

            b1.Free();
            Assert.That(p1.IsConnected(), Is.False);

        }
        [Test(Description = "Signal connect and disconnect when owner method is freed")]
        public async Task ConnectAndDisconnectMethodFreeTargetTests() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();

            var o = new Dummy();
            SignalHandler p1 = b1.OnPressed(o.Pressed1);
            Assert.That(p1.Target, Is.EqualTo(o));
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p1.IsConnected(), Is.True);

            b1.EmitSignal("pressed");
            Assert.That(o.Executed1, Is.EqualTo(1));
            
            o.Free();
            Assert.That(p1.IsConnected(), Is.False);
            b1.EmitSignal("pressed");
            Assert.That(o.Executed1, Is.EqualTo(1));
        }

        [Test(Description = "Signal lambda oneShot should only work one and auto freed")]
        public async Task SignalToLambdaOneShotTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var executed1 = 0;

            SignalHandler p1 = b1.OnPressed(() => executed1 ++, true);

            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));
            
            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            // More executions are ignored
            Assert.That(executed1, Is.EqualTo(1));

            // It's not marked as freed because the origin is still alive
            Assert.That(p1.IsConnected(), Is.False);
            // It's still valid because the free is called with deferred
            Assert.That(IsInstanceValid(p1.Target), Is.True);

            // freed in the next frame
            await this.AwaitIdleFrame();
            Assert.That(IsInstanceValid(p1.Target), Is.False);
        }

        [Test(Description = "Signal lambda deferred should work next frame")]
        public async Task SignalToLambdaDeferredTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var executed1 = 0;

            SignalHandler p1 = b1.OnPressed(() => executed1 ++, false, true);

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

            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(0));
            
            await this.AwaitIdleFrame();
            Assert.That(executed1, Is.EqualTo(1));
            // Use CallDeferred() in an idle frame is called immediately, not in the next one
            Assert.That(IsInstanceValid(p1.Target), Is.False);

        }

        [Test(Description = "Signal lambda is watched and unwatched")]
        public async Task SignalToLambdaIsWatchedAndUnwatched() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();

            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));
            SignalHandler p1 = b1.OnPressed(() => { }, true);
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));
            
            p1.Unwatch();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));
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