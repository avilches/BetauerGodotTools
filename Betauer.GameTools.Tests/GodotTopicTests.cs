using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Bus;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    [TestFixture]
    public class GodotTopicTests : Node {
        [SetUp]
        public void Setup() {
            LoggerFactory.OverrideTraceLevel(TraceLevel.All);
            LoggerFactory.SetConsoleOutput(ConsoleOutput.ConsoleWriteLine);
        }

        [Test]
        public async Task TestMulticast() {
            int body1Calls = 0;
            int body2Calls = 0;
            int anyCalls = 0;
            Area2D area1 = new Area2D();
            Area2D area2 = new Area2D();
            KinematicBody2D owner1 = new KinematicBody2D();
            KinematicBody2D owner3 = new KinematicBody2D();
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();
            GodotTopic<BodyOnArea2D> topic = new GodotTopic<BodyOnArea2D>("T");
            topic.Subscribe(new BodyOnArea2DListenerAction("Body1", owner1, body1, (BodyOnArea2D @event) => {
                Assert.That(@event.Detected, Is.EqualTo(body1));
                body1Calls++;
            }));
            topic.Subscribe(new BodyOnArea2DListenerAction("Body2", owner1, body2, (BodyOnArea2D @event) => {
                Assert.That(@event.Detected, Is.EqualTo(body2));
                body2Calls++;
            }));
            topic.Subscribe(new BodyOnArea2DListenerAction("ANY", owner3, null,
                (BodyOnArea2D @event) => anyCalls++));
            Assert.That(topic.EventListeners.Count, Is.EqualTo(3));

            // When events are published
            topic.Publish(new BodyOnArea2D(body1, area1));
            topic.Publish(new BodyOnArea2D(body2, area1));
            topic.Publish(new BodyOnArea2D(body2, area2));

            // Then
            Assert.That(topic.EventListeners.Count, Is.EqualTo(3));
            Assert.That(body1Calls, Is.EqualTo(1));
            Assert.That(body2Calls, Is.EqualTo(2));
            Assert.That(anyCalls, Is.EqualTo(3));

            // When body is disposed
            body1.Dispose();
            await this.AwaitPhysicsFrame();
            // Then listeners are still the same
            Assert.That(topic.EventListeners.Count, Is.EqualTo(3));

            // When new events are published
            topic.Publish(new BodyOnArea2D(body2, area1));
            topic.Publish(new BodyOnArea2D(body2, area2));
            // Then the "Body1" listener with the disposed filter disappear
            Assert.That(topic.EventListeners.Count, Is.EqualTo(2));
            // And data is ok
            Assert.That(body1Calls, Is.EqualTo(1)); // 1 as before
            Assert.That(body2Calls, Is.EqualTo(4)); // 2 before + 2 now
            Assert.That(anyCalls, Is.EqualTo(5)); // 3 before + 2 now

            // When owner1 is disposed
            owner1.Dispose();
            await this.AwaitPhysicsFrame();
            // Then listeners are still the same
            Assert.That(topic.EventListeners.Count, Is.EqualTo(2));

            // When new events are published
            topic.Publish(new BodyOnArea2D(body2, area1));
            topic.Publish(new BodyOnArea2D(body2, area2));
            // Then the listener with the disposed filter disappear
            Assert.That(topic.EventListeners.Count, Is.EqualTo(1));
            // And data is ok
            Assert.That(body1Calls, Is.EqualTo(1)); // 1 as before
            Assert.That(body2Calls, Is.EqualTo(4)); // 4 as before
            Assert.That(anyCalls, Is.EqualTo(7)); // 5 before + 2 now
        }

        [Test]
        public async Task BodyOnArea2DTopic() {
            int enterCalls = 0;
            int exitCalls = 0;
            bool overlap = false;
            KinematicBody2D body = CreateKinematicBody2D("player", 0, 0);
            Area2D area2DMustBeIgnored = CreateArea2D("other different than filtered");
            Area2D area2D = CreateArea2D("slopeDown", 2000, 2000);

            BodyOnArea2DTopic topic = new BodyOnArea2DTopic("T");
            topic.ListenSignalsOf(area2D);
            topic.Subscribe(
                new BodyOnArea2DListenerAction("Body", body, body, (BodyOnArea2D @event) => {
                    Assert.That(@event.Detected, Is.EqualTo(body));
                    Assert.That(@event.Origin, Is.EqualTo(area2D));
                    enterCalls++;
                    overlap = true;
                }),
                new BodyOnArea2DListenerAction("Body", body, body, (BodyOnArea2D @event) => {
                    Assert.That(@event.Detected, Is.EqualTo(body));
                    Assert.That(@event.Origin, Is.EqualTo(area2D));
                    exitCalls++;
                    overlap = false;
                }));
            BodyOnArea2DStatus status = topic.StatusSubscriber("Body", body, body);
            await this.AwaitPhysicsFrame();

            // They are not colliding
            Assert.That(overlap, Is.False);
            Assert.That(status.IsOverlapping, Is.False);
            Assert.That(enterCalls, Is.EqualTo(0));
            Assert.That(exitCalls, Is.EqualTo(0));

            // Make them collide
            body.Position = area2D.Position = Vector2.Zero;
            await this.AwaitPhysicsFrame();

            Assert.That(overlap, Is.True);
            Assert.That(status.IsOverlapping, Is.True);
            Assert.That(enterCalls, Is.EqualTo(1));
            Assert.That(exitCalls, Is.EqualTo(0));

            // Make them not collide
            area2D.Position = new Vector2(2000, 2000);
            await this.AwaitPhysicsFrame();

            Assert.That(overlap, Is.False);
            Assert.That(status.IsOverlapping, Is.False);
            Assert.That(enterCalls, Is.EqualTo(1));
            Assert.That(exitCalls, Is.EqualTo(1));

            body.Dispose();
            await this.AwaitPhysicsFrame();

            Assert.That(status.IsDisposed(), Is.True);
            foreach (var listener in topic.EnterTopic.EventListeners) {
                Assert.That(listener.IsDisposed(), Is.True);
            }
            foreach (var listener in topic.ExitTopic.EventListeners) {
                Assert.That(listener.IsDisposed(), Is.True);
            }
        }

        [Test]
        public async Task Area2DOnArea2DTopic() {
            int enterCalls = 0;
            int exitCalls = 0;
            bool overlap = false;
            Area2D from = CreateArea2D("player", 0, 0);
            Area2D area2D = CreateArea2D("slopeDown", 2000, 2000);

            Area2DOnArea2DTopic topic = new Area2DOnArea2DTopic("T");
            topic.ListenSignalsOf(area2D);
            topic.Subscribe(
                new Area2DOnArea2DListenerAction("From", from, from, (Area2DOnArea2D @event) => {
                    Assert.That(@event.Detected, Is.EqualTo(from));
                    Assert.That(@event.Origin, Is.EqualTo(area2D));
                    enterCalls++;
                    overlap = true;
                }),
                new Area2DOnArea2DListenerAction("From", from, from, (Area2DOnArea2D @event) => {
                    Assert.That(@event.Detected, Is.EqualTo(from));
                    Assert.That(@event.Origin, Is.EqualTo(area2D));
                    exitCalls++;
                    overlap = false;
                }));
            Area2DOnArea2DStatus status = topic.StatusSubscriber("Body", from, from);
            await this.AwaitPhysicsFrame();

            // They are not colliding
            Assert.That(overlap, Is.False);
            Assert.That(status.IsOverlapping, Is.False);
            Assert.That(enterCalls, Is.EqualTo(0));
            Assert.That(exitCalls, Is.EqualTo(0));

            // Make them collide
            from.Position = area2D.Position = Vector2.Zero;
            await this.AwaitPhysicsFrame();

            Assert.That(overlap, Is.True);
            Assert.That(status.IsOverlapping, Is.True);
            Assert.That(enterCalls, Is.EqualTo(1));
            Assert.That(exitCalls, Is.EqualTo(0));

            // Make them not collide
            area2D.Position = new Vector2(2000, 2000);
            await this.AwaitPhysicsFrame();

            Assert.That(overlap, Is.False);
            Assert.That(status.IsOverlapping, Is.False);
            Assert.That(enterCalls, Is.EqualTo(1));
            Assert.That(exitCalls, Is.EqualTo(1));

            from.Dispose();
            await this.AwaitPhysicsFrame();

            Assert.That(status.IsDisposed(), Is.True);
            foreach (var listener in topic.EnterTopic.EventListeners) {
                Assert.That(listener.IsDisposed(), Is.True);
            }
            foreach (var listener in topic.ExitTopic.EventListeners) {
                Assert.That(listener.IsDisposed(), Is.True);
            }
        }


        [Test]
        public async Task Area2DShapeOnArea2D() {
            int enterCalls = 0;
            int exitCalls = 0;
            bool overlap = false;
            Area2D from = CreateArea2D("player", 0, 0);
            Area2D area2D = CreateArea2D("stage", 2000, 2000);
            AddCollisionShape(area2D);

            Area2DShapeOnArea2DTopic topic = new Area2DShapeOnArea2DTopic("T");
            topic.ListenSignalsOf(area2D);
            List<int> enteredOriginShapes = new List<int>();
            List<int> exitedOriginShapes = new List<int>();
            topic.Subscribe(
                new Area2DShapeOnArea2DListenerAction("From", from, from, (Area2DShapeOnArea2D @event) => {
                    Assert.That(@event.Detected, Is.EqualTo(from));
                    Assert.That(@event.Origin, Is.EqualTo(area2D));
                    enteredOriginShapes.Add(@event.OriginShape);
                    enterCalls++;
                    overlap = true;
                }),
                new Area2DShapeOnArea2DListenerAction("From", from, from, (Area2DShapeOnArea2D @event) => {
                    Assert.That(@event.Detected, Is.EqualTo(from));
                    Assert.That(@event.Origin, Is.EqualTo(area2D));
                    exitedOriginShapes.Add(@event.OriginShape);
                    exitCalls++;
                    overlap = false;
                }));
            await this.AwaitPhysicsFrame();

            // They are not colliding
            Assert.That(overlap, Is.False);
            Assert.That(enterCalls, Is.EqualTo(0));
            Assert.That(exitCalls, Is.EqualTo(0));

            // Make them collide
            from.Position = area2D.Position = Vector2.Zero;
            await this.AwaitPhysicsFrame();

            // The area2D has two shapes
            Assert.That(overlap, Is.True);
            Assert.That(enterCalls, Is.EqualTo(2));
            Assert.That(exitCalls, Is.EqualTo(0));
            Assert.That(enteredOriginShapes, Contains.Item(0));
            Assert.That(enteredOriginShapes, Contains.Item(1));
            Assert.That(exitedOriginShapes.Count, Is.EqualTo(0));

            // Make them not collide
            area2D.Position = new Vector2(2000, 2000);
            await this.AwaitPhysicsFrame();

            Assert.That(overlap, Is.False);
            Assert.That(enterCalls, Is.EqualTo(2));
            Assert.That(exitCalls, Is.EqualTo(2));
            Assert.That(exitedOriginShapes, Contains.Item(0));
            Assert.That(exitedOriginShapes, Contains.Item(1));
        }

        private Area2D CreateArea2D(string name, int x = 0, int y = 0) {
            var area2D = new Area2D();
            area2D.Position = new Vector2(x, y);
            area2D.Name = name;
            AddCollisionShape(area2D);
            AddChild(area2D);
            return area2D;
        }

        private KinematicBody2D CreateKinematicBody2D(string name, int x = 0, int y = 0) {
            var body2D = new KinematicBody2D();
            body2D.Position = new Vector2(x, y);
            body2D.Name = name;
            AddCollisionShape(body2D);
            AddChild(body2D);
            return body2D;
        }

        private void AddCollisionShape(Node2D node) {
            var rectangleShape2D = new RectangleShape2D();
            rectangleShape2D.Extents = new Vector2(4, 4);
            var collisionShape2D = new CollisionShape2D();
            collisionShape2D.Shape = rectangleShape2D;
            node.AddChild(collisionShape2D);
        }
    }
}