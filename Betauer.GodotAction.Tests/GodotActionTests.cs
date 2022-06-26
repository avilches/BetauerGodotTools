using System;
using System.Threading.Tasks;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GodotAction.Tests {
    [TestFixture]
    public class GodotActionTests : Node {
        [Test(Description = "Test multiple actions per signal or process")]
        public async Task ConnectAndDisconnect() {
            var l = new LabelAction();
            var entered1 = 0;
            var entered2 = 0;
            Action action1 = () => entered1++;
            Action action2 = () => entered2++;
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.False);

            // Remove one when there wasn't listeners (count is 0), it shouldn't appear an error
            l.RemoveOnTreeEntered(action2);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.False);

            // Adding 2 listeners
            l.OnTreeEntered(action1);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.True);
            l.OnTreeEntered(action2);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.True);

            // Remove 2 listeners
            l.RemoveOnTreeEntered(action1);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.True);
            l.RemoveOnTreeEntered(action2);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.False);

            // Remove one when there wasn't listeners (count is 0), it shouldn't appear an error
            l.RemoveOnTreeEntered(action2);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.False);

            // Add one again
            l.OnTreeEntered(action1);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.True);

            // Remove one that wasn't added
            l.RemoveOnTreeEntered(action2);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.True);

            // Remove one
            l.RemoveOnTreeEntered(action1);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.False);

        }
        
        [Test]
        public async Task MultipleActionsPerSignal() {
            var l = new LabelAction();
            var entered1 = 0;
            var entered2 = 0;

            l.OnTreeEntered(() => entered1++);
            l.OnTreeEntered(() => entered2++);
            
            AddChild(l);
            Assert.That(entered1, Is.EqualTo(1));
            Assert.That(entered2, Is.EqualTo(1));
        }
        
        [Test]
        public async Task MultipleActionsPerProcess() {
            var l = new LabelAction();
            var process1 = 0;
            var process2 = 0;
            void Action1(float delta) => process1++;
            void Action2(float delta) => process2++;

            l.OnProcess(Action1);
            l.OnProcess(Action2);
            
            AddChild(l);
            Assert.That(process1, Is.EqualTo(0));
            Assert.That(process2, Is.EqualTo(0));

            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(1));
            Assert.That(process2, Is.EqualTo(1));

            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(2));
            Assert.That(process2, Is.EqualTo(2));
            
            // Remove one, the other is still working
            l.RemoveOnProcess(Action2);
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(2));

            // Remove the other, no one works (Process is disabled)
            l.RemoveOnProcess(Action1);
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(2));
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(2));

            // Add one again, it should work (Process should be enabled again)
            l.OnProcess(Action2);
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(3));

        }
    }
}