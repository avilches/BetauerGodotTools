using System.Collections;
using Godot;
using NUnit.Framework;
using Tools.Bus;
using Debug = Tools.Debug;

namespace Veronenger.Tests {
    [TestFixture]
    public class EventTests : Node {
        Area2D Area1 = new Area2D();
        Area2D Area2 = new Area2D();

        [SetUp]
        public void Setup() {
            Debug.TESTING = true;
            Area1.Name = "Area1";
            Area2.Name = "Area2";
        }

        [Test]
        public IEnumerator TestGodotUnicastOnlyHasOneListener() {
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();
            KinematicBody2D body3 = new KinematicBody2D();
            AddChild(body1);
            AddChild(body2);
            AddChild(body3);
            yield return null; // TesRunner will make enough delay to ensure the Godot event loop add them

            /*
             Unicast only allows one subscriber, so, Body1Calls is 0 and Body2 have two calls
             */
            GodotNodeUnicastTopic<BodyOnArea2D> topic = new GodotNodeUnicastTopic<BodyOnArea2D>("T");
            int body1Calls = 0;
            int body2Calls = 0;
            topic.Subscribe(new BodyOnArea2DEnterListenerDelegate("Body1", body1, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Body, Is.EqualTo(body1));
                body1Calls++;
            }));
            topic.Subscribe(new BodyOnArea2DEnterListenerDelegate("Body2", body2, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Body, Is.EqualTo(body2));
                body2Calls++;
            }));

            // When:
            // 2 body1 calls
            topic.Publish(new BodyOnArea2D(body1, Area1));
            topic.Publish(new BodyOnArea2D(body1, Area2));
            // 1 body2 calls
            topic.Publish(new BodyOnArea2D(body2, Area1));
            topic.Publish(new BodyOnArea2D(body2, Area2));
            // nobody is listening for body3 events, so it will be ignored
            topic.Publish(new BodyOnArea2D(body3, Area1));

            // Then:
            Assert.That(body1Calls, Is.EqualTo(0));
            Assert.That(body2Calls, Is.EqualTo(2));

            body1.QueueFree();
            body2.QueueFree();
            body3.QueueFree();
            yield return null; // TesRunner will make enough delay to ensure the Godot event loop add them

        }

        [Test]
        public IEnumerator TestGodotUnicastShudn_tFailWhenListenerNodeIsDisposed() {
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();
            AddChild(body1);
            AddChild(body2);
            body2.QueueFree();
            yield return null; // TesRunner will make enough delay to ensure the Godot event loop add them

            GodotNodeUnicastTopic<BodyOnArea2D> topic = new GodotNodeUnicastTopic<BodyOnArea2D>("T");
            int body1Calls = 0;
            topic.Subscribe(new BodyOnArea2DEnterListenerDelegate("Body1", body1, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Body, Is.EqualTo(body1));
                body1Calls++;
            }));

            // When 3 events are published
            topic.Publish(new BodyOnArea2D(body1, Area1));
            topic.Publish(new BodyOnArea2D(body1, Area2));
            topic.Publish(new BodyOnArea2D(body1, Area2));
            topic.Publish(new BodyOnArea2D(body2, Area2)); // body2 is disposed
            // Then
            Assert.That(body1Calls, Is.EqualTo(3));

            body1.QueueFree();
            yield return null; // TesRunner will make enough delay to ensure the Godot event loop add them

            // When new events are published
            Assert.That(topic.Listener, Is.Not.Null);

            topic.Publish(new BodyOnArea2D(body1, Area1));
            // Listener is deleted
            Assert.That(topic.Listener, Is.Null);

            // These will be ignored
            topic.Publish(new BodyOnArea2D(body2, Area2));
            topic.Publish(new BodyOnArea2D(body1, Area2));
            topic.Publish(new BodyOnArea2D(body1, Area2));

            // No new calls
            Assert.That(body1Calls, Is.EqualTo(3));
        }

        [Test]
        public IEnumerator TestGodotMulticast() {
            int body1Calls = 0;
            int body2Calls = 0;
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();
            AddChild(body1);
            AddChild(body2);
            GodotNodeMulticastTopic<BodyOnArea2D> topic = new GodotNodeMulticastTopic<BodyOnArea2D>("T");
            /*
             Multicast allows multiple subscribers, so, Body1 and Body2 have two calls
             */
            topic.Subscribe(new BodyOnArea2DEnterListenerDelegate("Body1", body1, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Body, Is.EqualTo(body1));
                body1Calls++;
            }));
            topic.Subscribe(new BodyOnArea2DEnterListenerDelegate("Body2", body2, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Body, Is.EqualTo(body2));
                body2Calls++;
            }));
            topic.Publish(new BodyOnArea2D(body1, Area1));
            topic.Publish(new BodyOnArea2D(body2, Area1));
            topic.Publish(new BodyOnArea2D(body2, Area2));
            Assert.That(body1Calls, Is.EqualTo(1));
            Assert.That(body2Calls, Is.EqualTo(2));

            body1.QueueFree();
            yield return null; // TesRunner will make enough delay to ensure the Godot event loop add them

            // When new events are published
            topic.Publish(new BodyOnArea2D(body1, Area1)); // body1 is disposed
            topic.Publish(new BodyOnArea2D(body1, Area2)); // body1 is disposed
            topic.Publish(new BodyOnArea2D(body2, Area1));
            topic.Publish(new BodyOnArea2D(body2, Area2));
            Assert.That(body1Calls, Is.EqualTo(1)); // 1, like before
            Assert.That(body2Calls, Is.EqualTo(4)); // 2 before + 2 now
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
            t.AddTopic("t", new GodotNodeMulticastTopic<BodyOnArea2D>("T"));

            /*
             It should work in the same way as TestGodotMulticast
             */
            t.Subscribe<GodotNodeListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2DEnterListenerDelegate("Body1", body1, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Body, Is.EqualTo(body1));
                body1Calls++;
            }));
            t.Subscribe<GodotNodeListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2DEnterListenerDelegate("Body2", body2, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Body, Is.EqualTo(body2));
                body2Calls++;
            }));
            t.Publish<GodotNodeListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2D(body1, Area1));
            t.Publish<GodotNodeListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2D(body1, Area2));
            t.Publish<GodotNodeListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2D(body2, Area1));
            t.Publish<GodotNodeListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2D(body2, Area2));
            Assert.That(body1Calls, Is.EqualTo(2));
            Assert.That(body2Calls, Is.EqualTo(2));
        }

    }
}