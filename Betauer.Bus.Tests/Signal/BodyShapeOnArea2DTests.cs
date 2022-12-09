using System.Threading.Tasks;
using Betauer.Core.Signal;
using Betauer.Bus.Signal;
using Betauer.Tools.Logging;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Object = Godot.Object;

namespace Betauer.Tests.Signal {
    [TestFixture]
    public class BodyShapeOnArea2DTests : BaseNodeTest {
        [SetUp]
        public void Setup() {
            LoggerFactory.OverrideTraceLevel(TraceLevel.All);
        }

        [Test]
        public async Task BodyShapeOnArea2DCollisionTests() {
            var body = CreateKinematicBody2D("player body", 0, 0);
            var area2D = CreateArea2D("area", 2000, 2000);
            
            await this.AwaitPhysicsFrame();

            BodyShapeOnArea2D.Status status = new BodyShapeOnArea2D.Status();
            BodyShapeOnArea2D.Collection collection = new BodyShapeOnArea2D.Collection();
            status.Connect(area2D);
            collection.Connect(area2D);

            await this.AwaitPhysicsFrame();

            // They are not colliding
            Assert.That(status.Status, Is.False);
            Assert.That(collection.Size(), Is.EqualTo(0));

            await ForceCollision(area2D, body);

            Assert.That(status.Status, Is.True);
            Assert.That(collection.Size(), Is.EqualTo(1));
            Assert.That(collection.Contains(body), Is.True);

            await ForceNotCollision(area2D, body);
            
            Assert.That(status.Status, Is.False);
            Assert.That(collection.Size(), Is.EqualTo(0));
        }
    }
}