using Godot;
using NUnit.Framework;

namespace GodotTests.NodeTests {
    [TestFixture]
    [Ignore("")]
    public class NodeTests : Node {
        [Test]
        public void SpawnAndFreeManyObjectsInLessThanFiveSeconds() {
            const int seconds = 5;
            uint startTime = OS.GetTicksMsec();

            for (var i = 0; i < 100000; i++) {
                new Node().Free();
            }

            Assert.That((OS.GetTicksMsec() - startTime) / 1000.0f, Is.LessThan(seconds));
        }
    }
}