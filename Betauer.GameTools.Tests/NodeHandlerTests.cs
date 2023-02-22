using System.Threading.Tasks;
using Betauer.Core.Nodes;
using Betauer.Nodes;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    
    // TODO: test if a input event is processed, stop process it
    
    [TestFixture]
    public partial class NodeHandlerTests : Node {
        [Test(Description = "Test when add and remove multiple action to _Process, the process is disabled or enabled")]
        public async Task MultipleActionsPerProcessAddAndRemove() {
            var l = new Label();
            AddChild(DefaultNodeHandler.Instance);
            await this.AwaitProcessFrame();

            var process1 = 0;
            var process2 = 0;
            void Action1(double delta) => process1++;
            void Action2(double delta) => process2++;
            // 2 actions added to _Process
            var a1 = l.OnProcess(Action1);
            var a2 = l.OnProcess(Action2);

            await this.AwaitProcessFrame();
            await this.AwaitProcessFrame();
            await this.AwaitProcessFrame();
            // Nothing happens because node is not in the tree
            Assert.That(process1, Is.EqualTo(0));
            Assert.That(process2, Is.EqualTo(0));

            AddChild(l);

            await this.AwaitProcessFrame();
            Assert.That(process1, Is.EqualTo(1));
            Assert.That(process2, Is.EqualTo(1));

            await this.AwaitProcessFrame();
            Assert.That(process1, Is.EqualTo(2));
            Assert.That(process2, Is.EqualTo(2));
            
            // Disable one, the other is still working
            a2.Disable();
            await this.AwaitProcessFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(2));

            // Destroy the other, no one works
            a1.Destroy();
            await this.AwaitProcessFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(2));
            await this.AwaitProcessFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(2));

            // Enable all of them again, it only works the non destroyed
            a2.Enable();
            a1.Enable(); // this is ignored because it was destroyed
            await this.AwaitProcessFrame();
            Assert.That(process1, Is.EqualTo(3));
            Assert.That(process2, Is.EqualTo(3));
        }

        [Test]
        public void ProcessModeTest() {
            Assert.That(BaseHandler.ShouldProcess(true, ProcessModeEnum.Always), Is.True);
            Assert.That(BaseHandler.ShouldProcess(false, ProcessModeEnum.Always), Is.True);

            Assert.That(BaseHandler.ShouldProcess(true, ProcessModeEnum.Disabled), Is.False);
            Assert.That(BaseHandler.ShouldProcess(false, ProcessModeEnum.Disabled), Is.False);

            Assert.That(BaseHandler.ShouldProcess(true, ProcessModeEnum.Inherit), Is.False);
            Assert.That(BaseHandler.ShouldProcess(false, ProcessModeEnum.Inherit), Is.True);

            Assert.That(BaseHandler.ShouldProcess(true, ProcessModeEnum.Pausable), Is.False);
            Assert.That(BaseHandler.ShouldProcess(false, ProcessModeEnum.Pausable), Is.True);

            Assert.That(BaseHandler.ShouldProcess(true, ProcessModeEnum.WhenPaused), Is.True);
            Assert.That(BaseHandler.ShouldProcess(false, ProcessModeEnum.WhenPaused), Is.False);

        }
    }
}