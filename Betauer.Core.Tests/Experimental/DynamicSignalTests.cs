using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests.Experimental {
    
    internal static class DynamicSignalExtensions {
        public static DynamicSignalManager Manager = new DynamicSignalManager(); 
        public static DynamicSignal OnPressed(Button button, Action action, bool oneShot = false) =>
            Manager.ConnectSignalToAction(button, "pressed", action, oneShot);
        public static DynamicSignal OnToggled(Button button, Action<bool> action, bool oneShot = false) =>
            Manager.ConnectSignalToAction(button, "toggled", action, oneShot);
    }

    [TestFixture]
    public class DynamicSignalTests : Node {
        [Test(Description = "0p and 1p signals")]
        public async Task BasicTest() {
            DynamicSignalExtensions.Manager = new DynamicSignalManager();
            var b1 = new CheckButton();
            var b2 = new CheckButton();
            AddChild(b1);
            AddChild(b2);
            await this.AwaitIdleFrame();
            var executed1 = 0;
            var executed2 = 0;
            var toggled1 = new List<bool>();
            var toggled2 = new List<bool>();

            DynamicSignal d1 = DynamicSignalExtensions.OnPressed(b1,() => {
                executed1++;
            });
            DynamicSignalExtensions.OnPressed(b2,() => {
                executed2++;
            });

            DynamicSignalExtensions.OnToggled(b1,(tog) => {
                toggled1.Add(tog);
            });
            DynamicSignalExtensions.OnToggled(b2,(tog) => {
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
            
            Assert.That(DynamicSignalExtensions.Manager.Signals0Ps.Count, Is.EqualTo(2));            
            Assert.That(DynamicSignalExtensions.Manager.Signals1Ps.Count, Is.EqualTo(2));            
            Assert.That(DynamicSignalExtensions.Manager.SignalsList.Count, Is.EqualTo(4));

            DynamicSignalExtensions.Manager.Clean();
            
            Assert.That(DynamicSignalExtensions.Manager.Signals0Ps.Count, Is.EqualTo(2));            
            Assert.That(DynamicSignalExtensions.Manager.Signals1Ps.Count, Is.EqualTo(2));            
            Assert.That(DynamicSignalExtensions.Manager.SignalsList.Count, Is.EqualTo(4));

            DynamicSignalExtensions.Manager.Remove(d1);
            Assert.That(DynamicSignalExtensions.Manager.Signals0Ps.Count, Is.EqualTo(1));            
            Assert.That(DynamicSignalExtensions.Manager.Signals1Ps.Count, Is.EqualTo(2));            
            Assert.That(DynamicSignalExtensions.Manager.SignalsList.Count, Is.EqualTo(3));

        }

        [Test]
        public async Task OneShot() {
            DynamicSignalExtensions.Manager = new DynamicSignalManager();
            var b1 = new CheckButton();
            var b2 = new CheckButton();
            AddChild(b1);
            AddChild(b2);
            await this.AwaitIdleFrame();
            var executedNormal = 0;
            var executedOneShot = 0;

            DynamicSignalExtensions.OnPressed(b1,() => { executedNormal++; });
            DynamicSignalExtensions.OnPressed(b2, () => { executedOneShot++; }, true);
            b1.EmitSignal("pressed");
            b2.EmitSignal("pressed");
            Assert.That(executedNormal, Is.EqualTo(1));
            Assert.That(executedOneShot, Is.EqualTo(1));

            b1.EmitSignal("pressed");
            b2.EmitSignal("pressed");
            Assert.That(executedNormal, Is.EqualTo(2));
            Assert.That(executedOneShot, Is.EqualTo(1));
            
            Assert.That(DynamicSignalExtensions.Manager.Signals0Ps.Count, Is.EqualTo(2));            
            Assert.That(DynamicSignalExtensions.Manager.Signals1Ps.Count, Is.EqualTo(0));            
            Assert.That(DynamicSignalExtensions.Manager.SignalsList.Count, Is.EqualTo(2));

            DynamicSignalExtensions.Manager.Clean();
            
            Assert.That(DynamicSignalExtensions.Manager.Signals0Ps.Count, Is.EqualTo(1));            
            Assert.That(DynamicSignalExtensions.Manager.Signals1Ps.Count, Is.EqualTo(0));            
            Assert.That(DynamicSignalExtensions.Manager.SignalsList.Count, Is.EqualTo(1));


        }
    }
}