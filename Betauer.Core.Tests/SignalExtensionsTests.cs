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
            ObjectLifeCycleManager.Singleton.Reset();
            LoggerFactory.SetTraceLevel(typeof(ObjectLifeCycleManager), TraceLevel.All);
        }

        [Test(Description = "Multiple signals with 0p and 1p")]
        public async Task BasicTest() {
            var b1 = new CheckButton();
            var b2 = new CheckButton();
            AddChild(b1);
            AddChild(b2);
            await this.AwaitIdleFrame();
            var executed1 = 0;
            var executed2 = 0;
            var toggled1 = new List<bool>();
            var toggled2 = new List<bool>();

            SignalHandler p1 = b1.OnPressed(() => {
                executed1++;
            });
            SignalHandler p2 = b2.OnPressed(() => {
                executed2++;
            });

            SignalHandler t1 = b1.OnToggled((tog) => {
                toggled1.Add(tog);
            });
            SignalHandler t2 = b2.OnToggled((tog) => {
                toggled2.Add(tog);
            });
            
            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(executed2, Is.EqualTo(0));
            Assert.That(toggled1, Is.EqualTo(new bool[]{}));
            Assert.That(toggled2, Is.EqualTo(new bool[]{}));

            b2.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(executed2, Is.EqualTo(1));
            Assert.That(toggled1, Is.EqualTo(new bool[]{}));
            Assert.That(toggled2, Is.EqualTo(new bool[]{}));

            b1.Pressed = true;
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(executed2, Is.EqualTo(1));
            Assert.That(toggled1.ToArray(), Is.EqualTo(new []{true}));
            Assert.That(toggled2.ToArray(), Is.EqualTo(new bool[]{}));
            
            b2.Pressed = true;
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(executed2, Is.EqualTo(1));
            Assert.That(toggled1.ToArray(), Is.EqualTo(new []{true}));
            Assert.That(toggled2.ToArray(), Is.EqualTo(new []{true}));

            b1.Pressed = false;
            b2.Pressed = false;
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(executed2, Is.EqualTo(1));
            Assert.That(toggled1.ToArray(), Is.EqualTo(new []{true, false}));
            Assert.That(toggled2.ToArray(), Is.EqualTo(new []{true, false}));
            
        }

        [Test]
        public async Task AllTypes() {
            var regular = new CheckButton();
            var oneShot = new CheckButton();
            var targetDisposed = new CheckButton();
            var bounded = new CheckButton();
            var disconnected = new CheckButton();
            var deferred = new CheckButton();
            var signalDisposed = new CheckButton();
            
            var boundedTarget = new Object();
            AddChild(regular);
            AddChild(oneShot);
            AddChild(targetDisposed);
            AddChild(bounded);
            AddChild(disconnected);
            AddChild(deferred);
            AddChild(signalDisposed);
            AddChild(new ObjectLifeCycleManagerNode(1));
            await this.AwaitIdleFrame();
            
            var executedNormal = 0;
            var executedOneShot = 0;
            var executedTargetDisposed = 0;
            var executedBounded = 0;
            var executedDisconnected = 0;
            var executedDeferred = 0;
            var executedSignalDisposed = 0;
            SignalHandler p1 = regular.OnPressed(() => { executedNormal++; });
            SignalHandler p2 = oneShot.OnPressed(() => { executedOneShot++; }, true);
            SignalHandler p3 = targetDisposed.OnPressed(() => { executedTargetDisposed++; });
            SignalHandler p4 = bounded.OnPressed(() => { executedBounded++; }).Bind(boundedTarget);
            SignalHandler p5 = disconnected.OnPressed(() => { executedDisconnected++; });
            SignalHandler p6 = deferred.OnPressed(() => { executedDeferred++; }, false, true);
            SignalHandler p7 = signalDisposed.OnPressed(() => { executedSignalDisposed++; });
            Assert.That(executedNormal, Is.EqualTo(0));
            Assert.That(executedOneShot, Is.EqualTo(0));
            Assert.That(executedTargetDisposed, Is.EqualTo(0));
            Assert.That(executedBounded, Is.EqualTo(0));
            Assert.That(executedDisconnected, Is.EqualTo(0));
            Assert.That(executedDeferred, Is.EqualTo(0));
            Assert.That(executedSignalDisposed, Is.EqualTo(0));
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p2.IsConnected(), Is.True);
            Assert.That(p3.IsConnected(), Is.True);
            Assert.That(p4.IsConnected(), Is.True);
            Assert.That(p5.IsConnected(), Is.True);
            Assert.That(p6.IsConnected(), Is.True);
            Assert.That(p7.IsConnected(), Is.True);
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p2.IsValid(), Is.True);
            Assert.That(p3.IsValid(), Is.True);
            Assert.That(p4.IsValid(), Is.True);
            Assert.That(p5.IsValid(), Is.True);
            Assert.That(p6.IsValid(), Is.True);
            Assert.That(p7.IsValid(), Is.True);
            Assert.That(p1.MustBeDisposed(), Is.False);
            Assert.That(p2.MustBeDisposed(), Is.False);
            Assert.That(p3.MustBeDisposed(), Is.False);
            Assert.That(p4.MustBeDisposed(), Is.False);
            Assert.That(p5.MustBeDisposed(), Is.False);
            Assert.That(p6.MustBeDisposed(), Is.False);
            Assert.That(p7.MustBeDisposed(), Is.False);

            regular.EmitSignal("pressed");
            oneShot.EmitSignal("pressed");
            targetDisposed.EmitSignal("pressed");
            bounded.EmitSignal("pressed");
            disconnected.EmitSignal("pressed");
            deferred.EmitSignal("pressed");
            signalDisposed.EmitSignal("pressed");
            
            Assert.That(executedNormal, Is.EqualTo(1));
            Assert.That(executedOneShot, Is.EqualTo(1));
            Assert.That(executedTargetDisposed, Is.EqualTo(1));
            Assert.That(executedBounded, Is.EqualTo(1));
            Assert.That(executedDisconnected, Is.EqualTo(1));
            Assert.That(executedDeferred, Is.EqualTo(0));
            Assert.That(executedSignalDisposed, Is.EqualTo(1));
            Assert.That(p1.MustBeDisposed(), Is.False);
            Assert.That(p2.MustBeDisposed(), Is.True);
            Assert.That(p3.MustBeDisposed(), Is.False);
            Assert.That(p4.MustBeDisposed(), Is.False);
            Assert.That(p5.MustBeDisposed(), Is.False);
            Assert.That(p6.MustBeDisposed(), Is.False);
            Assert.That(p7.MustBeDisposed(), Is.False);
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p2.IsConnected(), Is.False);
            Assert.That(p3.IsConnected(), Is.True);
            Assert.That(p4.IsConnected(), Is.True);
            Assert.That(p5.IsConnected(), Is.True);
            Assert.That(p6.IsConnected(), Is.True);
            Assert.That(p7.IsConnected(), Is.True);
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p2.IsValid(), Is.True);
            Assert.That(p3.IsValid(), Is.True);
            Assert.That(p4.IsValid(), Is.True);
            Assert.That(p5.IsValid(), Is.True);
            Assert.That(p6.IsValid(), Is.True);
            Assert.That(p7.IsValid(), Is.True);

            targetDisposed.Free();
            boundedTarget.Free();
            p5.Disconnect();
            p7.Dispose();
            
            regular.EmitSignal("pressed");
            oneShot.EmitSignal("pressed");
            // targetDisposed.EmitSignal("pressed");
            bounded.EmitSignal("pressed");
            disconnected.EmitSignal("pressed");
            signalDisposed.EmitSignal("pressed");

            Assert.That(executedNormal, Is.EqualTo(2));
            Assert.That(executedOneShot, Is.EqualTo(1));
            Assert.That(executedTargetDisposed, Is.EqualTo(1));
            Assert.That(executedBounded, Is.EqualTo(2));
            Assert.That(executedDisconnected, Is.EqualTo(1));
            Assert.That(executedDeferred, Is.EqualTo(0));
            Assert.That(executedSignalDisposed, Is.EqualTo(1));
            Assert.That(p1.MustBeDisposed(), Is.False);
            Assert.That(p2.MustBeDisposed(), Is.True);
            Assert.That(p3.MustBeDisposed(), Is.True);
            Assert.That(p4.MustBeDisposed(), Is.True);
            Assert.That(p5.MustBeDisposed(), Is.False);
            Assert.That(p6.MustBeDisposed(), Is.False);
            Assert.That(p7.MustBeDisposed(), Is.True);
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p2.IsConnected(), Is.False);
            Assert.That(p3.IsConnected(), Is.False);
            Assert.That(p4.IsConnected(), Is.True);
            Assert.That(p5.IsConnected(), Is.False);
            Assert.That(p6.IsConnected(), Is.True);
            Assert.That(p7.IsConnected(), Is.False);
            Assert.That(p1.IsValid(), Is.True);
            Assert.That(p2.IsValid(), Is.True);
            Assert.That(p3.IsValid(), Is.False);
            Assert.That(p4.IsValid(), Is.True);
            Assert.That(p5.IsValid(), Is.True);
            Assert.That(p6.IsValid(), Is.True);
            Assert.That(p7.IsValid(), Is.False);

            Assert.That(ObjectLifeCycleManager.Singleton.GetWatching().Count, Is.EqualTo(7));
            Assert.That(ObjectLifeCycleManager.Singleton.GetQueue().Count, Is.EqualTo(0));

            await this.AwaitIdleFrame();
            Assert.That(executedDeferred, Is.EqualTo(1));
            
            Assert.That(ObjectLifeCycleManager.Singleton.GetWatching().Count, Is.EqualTo(3));
            Assert.That(ObjectLifeCycleManager.Singleton.GetDisposablesNextFrame().Count, Is.EqualTo(4));
            Assert.That(IsInstanceValid(p1), Is.True);
            Assert.That(IsInstanceValid(p2), Is.True);
            Assert.That(IsInstanceValid(p3), Is.True);
            Assert.That(IsInstanceValid(p4), Is.True);
            Assert.That(IsInstanceValid(p5), Is.True);
            Assert.That(IsInstanceValid(p6), Is.True);
            Assert.That(IsInstanceValid(p7), Is.False);

            await this.AwaitIdleFrame();
            Assert.That(ObjectLifeCycleManager.Singleton.GetWatching().Count, Is.EqualTo(3));
            Assert.That(ObjectLifeCycleManager.Singleton.GetDisposablesNextFrame().Count, Is.EqualTo(0));
            Assert.That(IsInstanceValid(p1), Is.True);  // regular
            Assert.That(IsInstanceValid(p2), Is.False); // one shot
            Assert.That(IsInstanceValid(p3), Is.False); // disposed target 
            Assert.That(IsInstanceValid(p4), Is.False); // bound to a disposed object 
            Assert.That(IsInstanceValid(p5), Is.True);  // disconnected
            Assert.That(IsInstanceValid(p6), Is.True);  // deferred
            Assert.That(IsInstanceValid(p7), Is.False); // disposed
        }
    }
}