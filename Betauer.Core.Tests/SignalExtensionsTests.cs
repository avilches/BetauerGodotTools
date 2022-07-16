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
    // [Only]
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

        [Test(Description = "Signal with method")]
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

        [Test]
        public async Task AllTypes() {
            var regular = new CheckButton();
            var oneShot = new CheckButton();
            var originFreed = new CheckButton();
            var disconnected = new CheckButton();
            var deferred = new CheckButton();
            var targetDisposed = new CheckButton();
            // TODO: test oneShot + deferred
            
            AddChild(regular);
            AddChild(oneShot);
            AddChild(originFreed);
            AddChild(disconnected);
            AddChild(deferred);
            AddChild(targetDisposed);
            AddChild(new ObjectWatcherNode(1));
            await this.AwaitIdleFrame();
            
            var executedNormal = 0;
            var executedOneShot = 0;
            var executedOriginFreed = 0;
            var executedDisconnected = 0;
            var executedDeferred = 0;
            var executedTargetDisposed = 0;
            SignalHandler p1 = regular.OnPressed(() => { executedNormal++; });
            SignalHandler p2 = oneShot.OnPressed(() => { executedOneShot++; }, true);
            SignalHandler p3 = originFreed.OnPressed(() => { executedOriginFreed++; });
            SignalHandler p5 = disconnected.OnPressed(() => { executedDisconnected++; });
            SignalHandler p6 = deferred.OnPressed(() => { executedDeferred++; }, false, true);
            SignalHandler p7 = targetDisposed.OnPressed(() => { executedTargetDisposed++; });
            Assert.That(executedNormal, Is.EqualTo(0));
            Assert.That(executedOneShot, Is.EqualTo(0));
            Assert.That(executedOriginFreed, Is.EqualTo(0));
            Assert.That(executedDisconnected, Is.EqualTo(0));
            Assert.That(executedDeferred, Is.EqualTo(0));
            Assert.That(executedTargetDisposed, Is.EqualTo(0));
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p2.IsConnected(), Is.True);
            Assert.That(p3.IsConnected(), Is.True);
            Assert.That(p5.IsConnected(), Is.True);
            Assert.That(p6.IsConnected(), Is.True);
            Assert.That(p7.IsConnected(), Is.True);
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p2.IsValid(), Is.True);
            Assert.That(p3.IsValid(), Is.True);
            Assert.That(p5.IsValid(), Is.True);
            Assert.That(p6.IsValid(), Is.True);
            Assert.That(p7.IsValid(), Is.True);
            Assert.That(p1.Target is IObjectWatched w1 && w1.MustBeFreed(), Is.False);
            Assert.That(p2.Target is IObjectWatched w2 && w2.MustBeFreed(), Is.False);
            Assert.That(p3.Target is IObjectWatched w3 && w3.MustBeFreed(), Is.False);
            Assert.That(p5.Target is IObjectWatched w5 && w5.MustBeFreed(), Is.False);
            Assert.That(p6.Target is IObjectWatched w6 && w6.MustBeFreed(), Is.False);
            Assert.That(p7.Target is IObjectWatched w7 && w7.MustBeFreed(), Is.False);

            regular.EmitSignal("pressed");
            oneShot.EmitSignal("pressed");
            originFreed.EmitSignal("pressed");
            disconnected.EmitSignal("pressed");
            deferred.EmitSignal("pressed");
            targetDisposed.EmitSignal("pressed");
            
            Assert.That(executedNormal, Is.EqualTo(1));
            Assert.That(executedOneShot, Is.EqualTo(1));
            Assert.That(executedOriginFreed, Is.EqualTo(1));
            Assert.That(executedDisconnected, Is.EqualTo(1));
            Assert.That(executedDeferred, Is.EqualTo(0));
            Assert.That(executedTargetDisposed, Is.EqualTo(1));
            Assert.That(p1.Target is IObjectWatched ww1 && ww1.MustBeFreed(), Is.False);
            Assert.That(p2.Target is IObjectWatched ww2 && ww2.MustBeFreed(), Is.False); // not yet, one shot is queue freed
            Assert.That(p3.Target is IObjectWatched ww3 && ww3.MustBeFreed(), Is.False);
            Assert.That(p5.Target is IObjectWatched ww5 && ww5.MustBeFreed(), Is.False);
            Assert.That(p6.Target is IObjectWatched ww6 && ww6.MustBeFreed(), Is.False);
            Assert.That(p7.Target is IObjectWatched ww7 && ww7.MustBeFreed(), Is.False);
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p2.IsConnected(), Is.False);
            Assert.That(p3.IsConnected(), Is.True);
            Assert.That(p5.IsConnected(), Is.True);
            Assert.That(p6.IsConnected(), Is.True);
            Assert.That(p7.IsConnected(), Is.True);
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p2.IsValid(), Is.True);
            Assert.That(p3.IsValid(), Is.True);
            Assert.That(p5.IsValid(), Is.True);
            Assert.That(p6.IsValid(), Is.True);
            Assert.That(p7.IsValid(), Is.True);

            originFreed.Free();
            p5.Disconnect();
            p7.Target.Dispose();
            
            regular.EmitSignal("pressed");
            oneShot.EmitSignal("pressed");
            Console.WriteLine("+-------------------------------------------------------");
            Console.WriteLine("| Next line will show: ERROR: Parameter \"ptr\" is null.");
            originFreed.EmitSignal("pressed"); // This doesn't emit any signal because it's disposed 
            Console.WriteLine("+-------------------------------------------------------");
            disconnected.EmitSignal("pressed");
            Console.WriteLine("+-------------------------------------------------------");
            Console.WriteLine("Next line will show an error");
            targetDisposed.EmitSignal("pressed");
            Console.WriteLine("+-------------------------------------------------------");

            Assert.That(executedNormal, Is.EqualTo(2));
            Assert.That(executedOneShot, Is.EqualTo(1));
            Assert.That(executedOriginFreed, Is.EqualTo(1));
            Assert.That(executedDisconnected, Is.EqualTo(1));
            Assert.That(executedDeferred, Is.EqualTo(0));
            Assert.That(executedTargetDisposed, Is.EqualTo(1));
            Assert.That(p1.Target is IObjectWatched www1 && www1.MustBeFreed(), Is.False);
            Assert.That(p2.Target is IObjectWatched www2 && www2.MustBeFreed(), Is.False); // not yet, one shot is queue freed
            Assert.That(p3.Target is IObjectWatched www3 && www3.MustBeFreed(), Is.True);
            Assert.That(p5.Target is IObjectWatched www5 && www5.MustBeFreed(), Is.False);
            Assert.That(p6.Target is IObjectWatched www6 && www6.MustBeFreed(), Is.False);
            Assert.That(p7.Target is IObjectWatched www7 && www7.MustBeFreed(), Is.False);
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p2.IsConnected(), Is.False);
            Assert.That(p3.IsConnected(), Is.False);
            Assert.That(p5.IsConnected(), Is.False);
            Assert.That(p6.IsConnected(), Is.True);
            Assert.That(p7.IsConnected(), Is.False);
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p2.IsValid(), Is.True);
            Assert.That(p3.IsValid(), Is.False);
            Assert.That(p5.IsValid(), Is.True);
            Assert.That(p6.IsValid(), Is.True);
            Assert.That(p7.IsValid(), Is.False);

            DefaultObjectWatcher.Instance.Process();
            await this.AwaitIdleFrame();

            Assert.That(executedDeferred, Is.EqualTo(1));
            
            DefaultObjectWatcher.Instance.Process();
            Assert.That(IsInstanceValid(p1.Target), Is.True);  // regular
            Assert.That(IsInstanceValid(p2.Target), Is.False); // one shot
            Assert.That(IsInstanceValid(p3.Target), Is.False); // disposed target 
            Assert.That(IsInstanceValid(p5.Target), Is.True);  // disconnected
            Assert.That(IsInstanceValid(p6.Target), Is.True);  // deferred
            Assert.That(IsInstanceValid(p7.Target), Is.False); // disposed

        }
    }
}