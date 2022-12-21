using Godot;
using NUnit.Framework;

namespace Veronenger.Tests.NodeTests {
    [TestFixture]
    // [Ignore("")]
    public partial class NodeTests : Node {
        [Test]
        public void SpawnAndFreeManyObjectsInLessThanFiveSeconds() {
            const int seconds = 5;
            var startTime = Time.GetTicksMsec();

            for (var i = 0; i < 100000; i++) {
                new Node().Free();
            }

            Assert.That((Time.GetTicksMsec() - startTime) / 1000.0f, Is.LessThan(seconds));
        }
    }
}