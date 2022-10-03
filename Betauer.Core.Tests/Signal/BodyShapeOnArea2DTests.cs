using System.Threading.Tasks;
using Betauer.Signal;
using Betauer.Bus.Signal;
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
            LoggerFactory.SetConsoleOutput(ConsoleOutput.ConsoleWriteLine);
        }

        [Test]
        public void BodyShapeOnArea2DFilterMulticastTest() {
            var body1Calls = 0;
            var body2Calls = 0;
            var noFilter = 0;
            var noFilterNoOwner = 0;
            Area2D area1 = new Area2D();
            Area2D area2 = new Area2D();
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();
            BodyShapeOnArea2DEntered.Multicast topic = new BodyShapeOnArea2DEntered.Multicast("T");
            var o = new Object();
            topic.OnEvent((_, tuple) => {
                Assert.That(tuple.Item2, Is.EqualTo(body1));
                body1Calls++;
            }).WithFilter(body1);
            topic.OnEvent((_,_) => body2Calls++).WithFilter(body2);
            topic.OnEvent((_,_) => noFilterNoOwner++);
            topic.OnEvent((_,_) => noFilter++).RemoveIfInvalid(o);
            Assert.That(topic.Handlers.Count, Is.EqualTo(4));

            // When events are published, the origin doesn't matter
            topic.Publish(area1, (new RID(new Object()), body1, 1, 1));
            topic.Publish(area2, (new RID(new Object()), body2, 1, 1));
            topic.Publish(area1, (new RID(new Object()), body2, 1, 1));

            // Then
            Assert.That(body1Calls, Is.EqualTo(1));
            Assert.That(body2Calls, Is.EqualTo(2));
            Assert.That(noFilter, Is.EqualTo(3));
            Assert.That(noFilterNoOwner, Is.EqualTo(3));

            // When body is disposed
            body1.Dispose();
            // When new events are published
            topic.Publish(area1, (new RID(new Object()), body2, 1, 1));
            // Then the "Body1" listener with the disposed filter disappear
            Assert.That(topic.Handlers.Count, Is.EqualTo(3));
            // And data is ok
            Assert.That(body1Calls, Is.EqualTo(1)); // 1 as before
            Assert.That(body2Calls, Is.EqualTo(3)); // 2 before + 1 now
            Assert.That(noFilter, Is.EqualTo(4)); // 3 before + 1 now
            Assert.That(noFilterNoOwner, Is.EqualTo(4)); // 3 before + 1 now
            
            o.Free();
            topic.Publish(area1, (new RID(new Object()), body1, 1, 1));

            Assert.That(topic.Handlers.Count, Is.EqualTo(2));
            // And data is ok
            Assert.That(body1Calls, Is.EqualTo(1)); // 1 as before
            Assert.That(body2Calls, Is.EqualTo(3)); // 3 before
            Assert.That(noFilter, Is.EqualTo(4)); // 4 as before
            Assert.That(noFilterNoOwner, Is.EqualTo(5)); // 4 as before + 1
        }

        [Test]
        public void BodyShapeOnArea2DFilterUnicastTest() {
            var calls = 0;
            Area2D area1 = new Area2D();
            Area2D area2 = new Area2D();
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();
            BodyShapeOnArea2DEntered.Unicast topic = new BodyShapeOnArea2DEntered.Unicast("T");
            
            topic.OnEvent((_, tuple) => {
                Assert.That(tuple.Item2, Is.EqualTo(body1));
                calls++;
            }).WithFilter(body1);
            topic.Publish(area1, (new RID(new Object()), body1, 1, 1));
            topic.Publish(area2, (new RID(new Object()), body1, 1, 1));
            topic.Publish(area2, (new RID(new Object()), body2, 1, 1));
            Assert.That(calls, Is.EqualTo(2));

            calls = 0;
            topic.OnEvent((_,_) => calls++).WithFilter(body1);
            topic.Publish(area1, (new RID(new Object()), body1, 1, 1));
            topic.Publish(area2, (new RID(new Object()), body1, 1, 1));
            topic.Publish(area2, (new RID(new Object()), body2, 1, 1));
            Assert.That(calls, Is.EqualTo(2));

            calls = 0;
            topic.OnEvent((_,_) => calls++);
            topic.Publish(area1, (new RID(new Object()), body1, 1, 1));
            topic.Publish(area2, (new RID(new Object()), body1, 1, 1));
            topic.Publish(area2, (new RID(new Object()), body2, 1, 1));
            Assert.That(calls, Is.EqualTo(3));
        }

        [Test]
        public async Task BodyShapeOnArea2DCollisionTests() {
            var multiEnterCalls = 0;
            var multiExitCalls = 0;
            var uniEnterCalls = 0;
            var uniExitCalls = 0;
            var body = CreateKinematicBody2D("player body", 0, 0);
            var area2D = CreateArea2D("area", 2000, 2000);
            
            await this.AwaitPhysicsFrame();

            BodyShapeOnArea2DEntered.Multicast multiEnter = new BodyShapeOnArea2DEntered.Multicast("T");
            BodyShapeOnArea2DExited.Multicast multiExit = new BodyShapeOnArea2DExited.Multicast("T");
            BodyShapeOnArea2DEntered.Unicast uniEnter = new BodyShapeOnArea2DEntered.Unicast("T");
            BodyShapeOnArea2DExited.Unicast uniExit = new BodyShapeOnArea2DExited.Unicast("T");
            BodyShapeOnArea2D.Status status = new BodyShapeOnArea2D.Status();
            BodyShapeOnArea2D.Collection collection = new BodyShapeOnArea2D.Collection();
            uniEnter.Connect(area2D);
            uniExit.Connect(area2D);
            multiEnter.Connect(area2D);
            multiExit.Connect(area2D);
            status.Connect(area2D);
            collection.Connect(area2D);

            uniEnter.OnEvent((_,_) => uniEnterCalls++).WithFilter(body);
            uniExit.OnEvent((_,_) => uniExitCalls++).WithFilter(body);
            multiEnter.OnEvent((_,_) => multiEnterCalls++).WithFilter(body);
            multiExit.OnEvent((_,_) => multiExitCalls++).WithFilter(body);
            await this.AwaitPhysicsFrame();

            // They are not colliding
            Assert.That(uniEnterCalls, Is.EqualTo(0));
            Assert.That(uniExitCalls, Is.EqualTo(0));
            Assert.That(multiEnterCalls, Is.EqualTo(0));
            Assert.That(multiExitCalls, Is.EqualTo(0));
            Assert.That(status.Status, Is.False);
            Assert.That(collection.Size(), Is.EqualTo(0));

            await ForceCollision(area2D, body);

            Assert.That(uniEnterCalls, Is.EqualTo(1));
            Assert.That(uniExitCalls, Is.EqualTo(0));
            Assert.That(multiEnterCalls, Is.EqualTo(1));
            Assert.That(multiExitCalls, Is.EqualTo(0));
            Assert.That(status.Status, Is.True);
            Assert.That(collection.Size(), Is.EqualTo(1));
            Assert.That(collection.Contains(body), Is.True);

            await ForceNotCollision(area2D, body);
            
            Assert.That(uniEnterCalls, Is.EqualTo(1));
            Assert.That(uniExitCalls, Is.EqualTo(1));
            Assert.That(multiEnterCalls, Is.EqualTo(1));
            Assert.That(multiExitCalls, Is.EqualTo(1));
            Assert.That(status.Status, Is.False);
            Assert.That(collection.Size(), Is.EqualTo(0));
        }
    }
}