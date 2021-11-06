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
        public IEnumerator TestGodotMulticast() {
            int body1Calls = 0;
            int body2Calls = 0;
            int anyCalls = 0;
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();
            AddChild(body1);
            AddChild(body2);
            GodotMulticastTopic<BodyOnArea2D> topic = new GodotMulticastTopic<BodyOnArea2D>("T");
            /*
             Multicast allows multiple subscribers, so, Body1 and Body2 have two calls
             */
            topic.Subscribe(new BodyOnArea2DEnterListenerDelegate("Body1", body1, delegate(BodyOnArea2D @event) {
                Assert.That(@event.From, Is.EqualTo(body1));
                body1Calls++;
            }));
            topic.Subscribe(new BodyOnArea2DEnterListenerDelegate("Body2", body2, delegate(BodyOnArea2D @event) {
                Assert.That(@event.From, Is.EqualTo(body2));
                body2Calls++;
            }));
            topic.Subscribe(new BodyOnArea2DEnterListenerDelegate("ANY", null, delegate(BodyOnArea2D @event) {
                anyCalls++;
            }));

            Assert.That(topic.EventListeners.Count, Is.EqualTo(3));
            topic.Publish(new BodyOnArea2D(body1, Area1));
            topic.Publish(new BodyOnArea2D(body2, Area1));
            topic.Publish(new BodyOnArea2D(body2, Area2));

            Assert.That(topic.EventListeners.Count, Is.EqualTo(3));
            Assert.That(body1Calls, Is.EqualTo(1));
            Assert.That(body2Calls, Is.EqualTo(2));
            Assert.That(anyCalls, Is.EqualTo(3));

            body1.QueueFree();
            yield return null; // TesRunner will make enough delay to ensure the Godot event loop add them

            // When new events are published
            Assert.That(topic.EventListeners.Count, Is.EqualTo(3));
            topic.Publish(new BodyOnArea2D(body2, Area1));
            topic.Publish(new BodyOnArea2D(body2, Area2));

            Assert.That(topic.EventListeners.Count, Is.EqualTo(2));
            Assert.That(body1Calls, Is.EqualTo(1)); // 1, like before
            Assert.That(body2Calls, Is.EqualTo(4)); // 2 before + 2 now

            // Pay attention: the body has been disposed, and this will destroy the listener, but
            Assert.That(anyCalls, Is.EqualTo(5));   // 3 before + 4 now
        }

        [Test]
        public void TestTopicMap() {
            int body1Calls = 0;
            int body2Calls = 0;
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();
            AddChild(body1);
            AddChild(body2);
            var t = TopicMap.Instance;
            t.AddTopic("t", new GodotMulticastTopic<BodyOnArea2D>("T"));

            /*
             It should work in the same way as TestGodotMulticast
             */
            t.Subscribe<GodotListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2DEnterListenerDelegate("Body1", body1, delegate(BodyOnArea2D @event) {
                Assert.That(@event.From, Is.EqualTo(body1));
                body1Calls++;
            }));
            t.Subscribe<GodotListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2DEnterListenerDelegate("Body2", body2, delegate(BodyOnArea2D @event) {
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