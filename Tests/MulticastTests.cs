using System.Collections;
using Godot;
using NUnit.Framework;
using Tools.Bus;
using Tools.Bus.Topics;
using Debug = Tools.Debug;

namespace Veronenger.Tests {
    [TestFixture]
    public class MulticastTests : Node {
        Area2D Area1 = new Area2D();
        Area2D Area2 = new Area2D();

        [SetUp]
        public void Setup() {
            Debug.TESTING = true;
            Area1.Name = "Area1";
            Area2.Name = "Area2";
        }

        [Test]
        public IEnumerator TestMulticast() {
            int body1Calls = 0;
            int body2Calls = 0;
            int anyCalls = 0;
            KinematicBody2D owner1 = new KinematicBody2D();
            KinematicBody2D owner3 = new KinematicBody2D();
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();
            GodotMulticastTopic<BodyOnArea2D> topic = new GodotMulticastTopic<BodyOnArea2D>("T");
            topic.Subscribe(new BodyOnArea2DListenerDelegate("Body1", owner1, body1, delegate(BodyOnArea2D @event) {
                Assert.That(@event.From, Is.EqualTo(body1));
                body1Calls++;
            }));
            topic.Subscribe(new BodyOnArea2DListenerDelegate("Body2", owner1, body2, delegate(BodyOnArea2D @event) {
                Assert.That(@event.From, Is.EqualTo(body2));
                body2Calls++;
            }));
            topic.Subscribe(new BodyOnArea2DListenerDelegate("ANY", owner3, null, delegate(BodyOnArea2D @event) {
                anyCalls++;
            }));
            Assert.That(topic.EventListeners.Count, Is.EqualTo(3));

            // When events are published
            topic.Publish(new BodyOnArea2D(body1, Area1));
            topic.Publish(new BodyOnArea2D(body2, Area1));
            topic.Publish(new BodyOnArea2D(body2, Area2));

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
            topic.Publish(new BodyOnArea2D(body2, Area1));
            topic.Publish(new BodyOnArea2D(body2, Area2));
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
            topic.Publish(new BodyOnArea2D(body2, Area1));
            topic.Publish(new BodyOnArea2D(body2, Area2));
            // Then the listener with the disposed filter disappear
            Assert.That(topic.EventListeners.Count, Is.EqualTo(1));
            // And data is ok
            Assert.That(body1Calls, Is.EqualTo(1)); // 1 as before
            Assert.That(body2Calls, Is.EqualTo(4)); // 4 as before
            Assert.That(anyCalls, Is.EqualTo(7));   // 5 before + 2 now
        }

        [Test]
        public void TestTopicMap() {
            int body1Calls = 0;
            int body2Calls = 0;
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();
            var t = TopicMap.Instance;
            t.AddTopic("t", new GodotMulticastTopic<BodyOnArea2D>("T"));

            /*
             It should work in the same way as TestGodotMulticast
             */
            t.Subscribe<GodotListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2DListenerDelegate("Body1", body1, body1, delegate(BodyOnArea2D @event) {
                Assert.That(@event.From, Is.EqualTo(body1));
                body1Calls++;
            }));
            t.Subscribe<GodotListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2DListenerDelegate("Body2", body2, body2, delegate(BodyOnArea2D @event) {
                Assert.That(@event.From, Is.EqualTo(body2));
                body2Calls++;
            }));
            t.Publish<GodotListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2D(body1, Area1));
            t.Publish<GodotListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2D(body1, Area2));
            t.Publish<GodotListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2D(body2, Area1));
            t.Publish<GodotListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2D(body2, Area2));
            Assert.That(body1Calls, Is.EqualTo(2));
            Assert.That(body2Calls, Is.EqualTo(2));
        }

    }
}