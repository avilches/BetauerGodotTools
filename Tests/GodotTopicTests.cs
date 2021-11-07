using System.Collections;
using System.Collections.Generic;
using Godot;
using NUnit.Framework;
using Tools.Bus.Topics;
using Debug = Tools.Debug;

namespace Veronenger.Tests {
    [TestFixture]
    public class PlatformManagerTests : Node {
        [SetUp]
        public void Setup() {
            Debug.TESTING = true;
        }

        [Test]
        public IEnumerator BodyOnArea2DTopic() {
            int enterCalls = 0;
            int exitCalls = 0;
            bool overlap = false;
            KinematicBody2D body = CreateKinematicBody2D("player", 0, 0);
            Area2D area2D = CreateArea2D("slopeDown", 2000, 2000);

            BodyOnArea2DTopic topic = new BodyOnArea2DTopic("T");
            topic.AddArea2D(area2D);
            topic.Subscribe(
                new BodyOnArea2DListenerDelegate("Body1", body, body, delegate(BodyOnArea2D @event) {
                    Assert.That(@event.Detected, Is.EqualTo(body));
                    Assert.That(@event.Origin, Is.EqualTo(area2D));
                    enterCalls++;
                    overlap = true;
                }),
                new BodyOnArea2DListenerDelegate("Body1", body, body, delegate(BodyOnArea2D @event) {
                    Assert.That(@event.Detected, Is.EqualTo(body));
                    Assert.That(@event.Origin, Is.EqualTo(area2D));
                    exitCalls++;
                    overlap = false;
                }));
            yield return null;

            // They are not colliding
            Assert.That(overlap, Is.EqualTo(false));
            Assert.That(enterCalls, Is.EqualTo(0));
            Assert.That(exitCalls, Is.EqualTo(0));

            // Make them collide
            body.Position = area2D.Position = Vector2.Zero;
            yield return null;

            Assert.That(overlap, Is.EqualTo(true));
            Assert.That(enterCalls, Is.EqualTo(1));
            Assert.That(exitCalls, Is.EqualTo(0));

            // Make them not collide
            area2D.Position = new Vector2(2000, 2000);
            yield return null;

            Assert.That(overlap, Is.EqualTo(false));
            Assert.That(enterCalls, Is.EqualTo(1));
            Assert.That(exitCalls, Is.EqualTo(1));
        }

        [Test]
        public IEnumerator Area2DOnArea2D() {
            int enterCalls = 0;
            int exitCalls = 0;
            bool overlap = false;
            Area2D from = CreateArea2D("player", 0, 0);
            Area2D area2D = CreateArea2D("slopeDown", 2000, 2000);

            Area2DOnArea2DTopic topic = new Area2DOnArea2DTopic("T");
            topic.AddArea2D(area2D);
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
            yield return null;

            // They are not colliding
            Assert.That(overlap, Is.EqualTo(false));
            Assert.That(enterCalls, Is.EqualTo(0));
            Assert.That(exitCalls, Is.EqualTo(0));

            // Make them collide
            from.Position = area2D.Position = Vector2.Zero;
            yield return null;

            Assert.That(overlap, Is.EqualTo(true));
            Assert.That(enterCalls, Is.EqualTo(1));
            Assert.That(exitCalls, Is.EqualTo(0));

            // Make them not collide
            area2D.Position = new Vector2(2000, 2000);
            yield return null;

            Assert.That(overlap, Is.EqualTo(false));
            Assert.That(enterCalls, Is.EqualTo(1));
            Assert.That(exitCalls, Is.EqualTo(1));
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
            topic.AddArea2D(area2D);
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