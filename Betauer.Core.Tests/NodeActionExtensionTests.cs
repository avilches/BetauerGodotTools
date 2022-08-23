using System.Threading.Tasks;
using Betauer.Nodes;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests {
    
    // TODO: test if a input event is processed, stop process it
    // TODO: rename GodotAction to ProxyNode
    // TODO: move to a Betauer.Node namespace
    
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
            var a1 = l.OnProcess(Action1);
            var a2 = l.OnProcess(Action2);

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
            
            // Disable one, the other is still working
            a2.Disable();
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(2));

            // Destroy the other, no one works
            a1.Destroy();
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(2));
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(2));

            // Enable all of them again, it only works the non destroyed
            a2.Enable();
            a1.Enable(); // this is ignored because it was destroyed
            await this.AwaitIdleFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(3));
        }
    }
}