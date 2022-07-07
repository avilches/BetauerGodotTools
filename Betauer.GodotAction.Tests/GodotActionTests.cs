using System;
using System.Threading.Tasks;
using Betauer.Signal;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GodotAction.Tests {
    [TestFixture]
    // [Only]
    public class GodotActionTests : Node {
        
        [Test]
        public void ErrorIfNoParent() {
            var la = new LabelAction();
            Assert.Throws<InvalidOperationException>(() => la.OnTreeEntered(() => Console.WriteLine("a")));
        }

        [Test]
        [Ignore("TODO")]
        public async Task MultipleActionsPerSignal() {
            var l = new Label();
            var entered1 = 0;
            var entered2 = 0;

            l.GetProxy()
                .OnTreeEntered(() => entered1++)
                .OnTreeEntered(() => entered2++);

            AddChild(l);
            Assert.That(entered1, Is.EqualTo(1));
            Assert.That(entered2, Is.EqualTo(1));
        }

        [Test(Description = "Test IsConnected() when multiple actions per signal are added and removed")]
        [Ignore("TODO")]
        public async Task MultipleConnectAndDisconnect() {
            var l = new Label();
            var entered1 = 0;
            var entered2 = 0;
            Action action1 = () => entered1++;
            Action action2 = () => entered2++;
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.False);

            // Remove one when there wasn't listeners (count is 0), it shouldn't appear an error
            l.GetProxy().RemoveOnTreeEntered(action2);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.False);

            // Adding 2 listeners
            l.OnTreeEntered(action1);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.True);
            l.OnTreeEntered(action2);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.True);

            // Remove 2 listeners
            l.GetProxy().RemoveOnTreeEntered(action1);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.True);
            l.GetProxy().RemoveOnTreeEntered(action2);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.False);

            // Remove one when there wasn't listeners (count is 0), it shouldn't appear an error
            l.GetProxy().RemoveOnTreeEntered(action2);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.False);

            // Add one again
            l.OnTreeEntered(action1);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.True);

            // Remove one that wasn't added
            l.GetProxy().RemoveOnTreeEntered(action2);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.True);

            // Remove one
            l.GetProxy().RemoveOnTreeEntered(action1);
            Assert.That(l.IsConnected("tree_entered", l, "_GodotSignalTreeEntered"), Is.False);

        }

        [Test(Description = "Test when add and remove multiple action to _Process, the process is disabled or enabled")]
        public async Task MultipleActionsPerProcessAddAndRemove() {
            var l = new Label();
            var process1 = 0;
            var process2 = 0;
            void Action1(float delta) => process1++;
            void Action2(float delta) => process2++;

            // 2 actions added to _Process
            l.GetProxy().OnProcess(Action1);
            l.GetProxy().OnProcess(Action2);
            
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
            l.GetProxy().RemoveOnProcess(Action2);
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(2));

            // Remove the other, no one works (_Process should be disabled)
            l.GetProxy().RemoveOnProcess(Action1);
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(2));
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(2));

            // Add one again, it should work (_Process should be enabled again)
            l.GetProxy().OnProcess(Action2);
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(3));

        }
    }
}