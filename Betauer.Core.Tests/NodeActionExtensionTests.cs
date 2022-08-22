using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests {
    
    [TestFixture]
    public class NodeActionExtensionsTests : Node {
        [Test(Description = "Test when add and remove multiple action to _Process, the process is disabled or enabled")]
        public async Task MultipleActionsPerProcessAddAndRemove() {
            var l = new Label();
            AddChild(DefaultNodeHandler.Instance);

            var process1 = 0;
            var process2 = 0;
            void Action1(float delta) => process1++;
            void Action2(float delta) => process2++;
            // 2 actions added to _Process
            l.OnProcess(Action1);
            l.OnProcess(Action2);

            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            // Nothing happens because node is not in the tree
            Assert.That(process1, Is.EqualTo(0));
            Assert.That(process2, Is.EqualTo(0));

            AddChild(l);

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

            // Remove the other, no one works (_Process should be disabled)
            l.RemoveOnProcess(Action1);
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(2));
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(2));

            // Add one again, it should work (_Process should be enabled again)
            l.OnProcess(Action2);
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(3));
        }
    }
}