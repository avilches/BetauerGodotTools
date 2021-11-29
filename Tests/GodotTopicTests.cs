using System.Collections;
using System.Collections.Generic;
using Godot;
using NUnit.Framework;
using Tools.Bus;
using Tools.Bus.Topics;

namespace Veronenger.Tests {
    [TestFixture]
    public class GodotTopicTests : Node {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public IEnumerator TestMulticast() {
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
            topic.Subscribe(new BodyOnArea2DListenerDelegate("Body1", owner1, body1, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Detected, Is.EqualTo(body1));
                body1Calls++;
            }));
            topic.Subscribe(new BodyOnArea2DListenerDelegate("Body2", owner1, body2, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Detected, Is.EqualTo(body2));
                body2Calls++;
            }));
            topic.Subscribe(new BodyOnArea2DListenerDelegate("ANY", owner3, null, delegate(BodyOnArea2D @event) {
                anyCalls++;
            }));
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
            body1.QueueFree();
            yield return null;
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
            Assert.That(anyCalls, Is.EqualTo(5));   // 3 before + 2 now

            // When owner1 is disposed
            owner1.QueueFree();
            yield return null;
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
            Assert.That(anyCalls, Is.EqualTo(7));   // 5 before + 2 now
        }

        [Test]
        public IEnumerator BodyOnArea2DTopic() {
            int enterCalls = 0;
            int exitCalls = 0;
            bool overlap = false;
            KinematicBody2D body = CreateKinematicBody2D("player", 0, 0);
            Area2D area2DMustBeIgnored = CreateArea2D("other different than filtered");
            Area2D area2D = CreateArea2D("slopeDown", 2000, 2000);

            BodyOnArea2DTopic topic = new BodyOnArea2DTopic("T");
            topic.ListenSignalsOf(area2D);
            topic.Subscribe(
                new BodyOnArea2DListenerDelegate("Body", body, body, delegate(BodyOnArea2D @event) {
                    Assert.That(@event.Detected, Is.EqualTo(body));
                    Assert.That(@event.Origin, Is.EqualTo(area2D));
                    enterCalls++;
                    overlap = true;
                }),
                new BodyOnArea2DListenerDelegate("Body", body, body, delegate(BodyOnArea2D @event) {
                    Assert.That(@event.Detected, Is.EqualTo(body));
                    Assert.That(@event.Origin, Is.EqualTo(area2D));
                    exitCalls++;
                    overlap = false;
                }));
            BodyOnArea2DStatus status = topic.StatusSubscriber("Body", body, body);
            yield return null;

            // They are not colliding
            Assert.That(overlap, Is.EqualTo(false));
            Assert.That(status.IsOverlapping, Is.EqualTo(false));
            Assert.That(enterCalls, Is.EqualTo(0));
            Assert.That(exitCalls, Is.EqualTo(0));

            // Make them collide
            body.Position = area2D.Position = Vector2.Zero;
            yield return null;

            Assert.That(overlap, Is.EqualTo(true));
            Assert.That(status.IsOverlapping, Is.EqualTo(true));
            Assert.That(enterCalls, Is.EqualTo(1));
            Assert.That(exitCalls, Is.EqualTo(0));

            // Make them not collide
            area2D.Position = new Vector2(2000, 2000);
            yield return null;

            Assert.That(overlap, Is.EqualTo(false));
            Assert.That(status.IsOverlapping, Is.EqualTo(false));
            Assert.That(enterCalls, Is.EqualTo(1));
            Assert.That(exitCalls, Is.EqualTo(1));

            body.QueueFree();
            yield return null;

            Assert.That(status.IsDisposed(), Is.EqualTo(true));
            foreach (var listener in topic.EnterTopic.EventListeners) {
                Assert.That(listener.IsDisposed(), Is.EqualTo(true));
            }
            foreach (var listener in topic.ExitTopic.EventListeners) {
                Assert.That(listener.IsDisposed(), Is.EqualTo(true));
            }
        }

        [Test]
        public IEnumerator Area2DOnArea2DTopic() {
            int enterCalls = 0;
            int exitCalls = 0;
            bool overlap = false;
            Area2D from = CreateArea2D("player", 0, 0);
            Area2D area2D = CreateArea2D("slopeDown", 2000, 2000);

            Area2DOnArea2DTopic topic = new Area2DOnArea2DTopic("T");
            topic.ListenSignalsOf(area2D);
            topic.Subscribe(
                new Area2DOnArea2DListenerDelegate("From", from, from, delegate(Area2DOnArea2D @event) {
                    Assert.That(@event.Detected, Is.EqualTo(from));
                    Assert.That(@event.Origin, Is.EqualTo(area2D));
                    enterCalls++;
                    overlap = true;
                }),
                new Area2DOnArea2DListenerDelegate("From", from, from, delegate(Area2DOnArea2D @event) {
                    Assert.That(@event.Detected, Is.EqualTo(from));
                    Assert.That(@event.Origin, Is.EqualTo(area2D));
                    exitCalls++;
                    overlap = false;
                }));
            Area2DOnArea2DStatus status = topic.StatusSubscriber("Body", from, from);
            yield return null;

            // They are not colliding
            Assert.That(overlap, Is.EqualTo(false));
            Assert.That(status.IsOverlapping, Is.EqualTo(false));
            Assert.That(enterCalls, Is.EqualTo(0));
            Assert.That(exitCalls, Is.EqualTo(0));

            // Make them collide
            from.Position = area2D.Position = Vector2.Zero;
            yield return null;

            Assert.That(overlap, Is.EqualTo(true));
            Assert.That(status.IsOverlapping, Is.EqualTo(true));
            Assert.That(enterCalls, Is.EqualTo(1));
            Assert.That(exitCalls, Is.EqualTo(0));

            // Make them not collide
            area2D.Position = new Vector2(2000, 2000);
            yield return null;

            Assert.That(overlap, Is.EqualTo(false));
            Assert.That(status.IsOverlapping, Is.EqualTo(false));
            Assert.That(enterCalls, Is.EqualTo(1));
            Assert.That(exitCalls, Is.EqualTo(1));

            from.QueueFree();
            yield return null;

            Assert.That(status.IsDisposed(), Is.EqualTo(true));
            foreach (var listener in topic.EnterTopic.EventListeners) {
                Assert.That(listener.IsDisposed(), Is.EqualTo(true));
            }
            foreach (var listener in topic.ExitTopic.EventListeners) {
                Assert.That(listener.IsDisposed(), Is.EqualTo(true));
            }
        }


        [Test]
        public IEnumerator Area2DShapeOnArea2D() {
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
                new Area2DShapeOnArea2DListenerDelegate("From", from, from, delegate(Area2DShapeOnArea2D @event) {
                    Assert.That(@event.Detected, Is.EqualTo(from));
                    Assert.That(@event.Origin, Is.EqualTo(area2D));
                    enteredOriginShapes.Add(@event.OriginShape);
                    enterCalls++;
                    overlap = true;
                }),
                new Area2DShapeOnArea2DListenerDelegate("From", from, from, delegate(Area2DShapeOnArea2D @event) {
                    Assert.That(@event.Detected, Is.EqualTo(from));
                    Assert.That(@event.Origin, Is.EqualTo(area2D));
                    exitedOriginShapes.Add(@event.OriginShape);
                    exitCalls++;
                    overlap = false;
                }));
            yield return null;

            // They are not colliding
            Assert.That(overlap, Is.EqualTo(false));
            Assert.That(enterCalls, Is.EqualTo(0));
            Assert.That(exitCalls, Is.EqualTo(0));

            // Make them collide
            from.Position = area2D.Position = Vector2.Zero;
            yield return null;

            // The area2D has two shapes
            Assert.That(overlap, Is.EqualTo(true));
            Assert.That(enterCalls, Is.EqualTo(2));
            Assert.That(exitCalls, Is.EqualTo(0));
            Assert.That(enteredOriginShapes, Contains.Item(0));
            Assert.That(enteredOriginShapes, Contains.Item(1));
            Assert.That(exitedOriginShapes.Count, Is.EqualTo(0));

            // Make them not collide
            area2D.Position = new Vector2(2000, 2000);
            yield return null;

            Assert.That(overlap, Is.EqualTo(false));
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