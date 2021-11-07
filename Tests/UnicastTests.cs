using System.Collections;
using Godot;
using NUnit.Framework;
using Tools.Bus;
using Tools.Bus.Topics;
using Debug = Tools.Debug;

namespace Veronenger.Tests {
    [TestFixture]
    public class UnicastTests : Node {
        Area2D Area1 = new Area2D();
        Area2D Area2 = new Area2D();

        [SetUp]
        public void Setup() {
            Debug.TESTING = true;
            Area1.Name = "Area1";
            Area2.Name = "Area2";
        }

        [Test]
        public void TestWithNoFilter() {
            KinematicBody2D owner = new KinematicBody2D();
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();
            KinematicBody2D body3 = new KinematicBody2D();

            /*
             Unicast only allows one subscriber, so, Body1Calls is 0 and Body2 have two calls
             */
            GodotUnicastTopic<BodyOnArea2D> topic = new GodotUnicastTopic<BodyOnArea2D>("T");
            int body1Calls = 0;
            var listener = new BodyOnArea2DListenerDelegate("Body1", owner, null, delegate(BodyOnArea2D @event) {
                body1Calls++;
            });
            topic.Subscribe(listener);
            Assert.That(topic.Listener, Is.EqualTo(listener));

            // When 3 calls with any body
            topic.Publish(new BodyOnArea2D(body1, Area1));
            topic.Publish(new BodyOnArea2D(body2, Area2));
            topic.Publish(new BodyOnArea2D(body3, Area2));

            // Then there are 3 calls
            Assert.That(topic.Listener, Is.EqualTo(listener));
            Assert.That(body1Calls, Is.EqualTo(3));

            // When no listener
            topic.Subscribe(null);
            // And more calls
            topic.Publish(new BodyOnArea2D(body3, Area2));

            // Same amount of calls and no listener
            Assert.That(topic.Listener, Is.Null);
            Assert.That(body1Calls, Is.EqualTo(3));
        }

        [Test]
        public void TestOnlyHasOneListener() {
            KinematicBody2D owner = new KinematicBody2D();
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();
            KinematicBody2D body3 = new KinematicBody2D();

            /*
             Unicast only allows one subscriber, so, Body1Calls is 0 and Body2 have two calls
             */
            GodotUnicastTopic<BodyOnArea2D> topic = new GodotUnicastTopic<BodyOnArea2D>("T");
            int body1Calls = 0;
            int body2Calls = 0;
            var listener1 = new BodyOnArea2DListenerDelegate("Body1", owner,body1, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Detected, Is.EqualTo(body1));
                body1Calls++;
            });
            var listener2 = new BodyOnArea2DListenerDelegate("Body2", owner,body2, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Detected, Is.EqualTo(body2));
                body2Calls++;
            });
            topic.Subscribe(listener1);
            Assert.That(topic.Listener, Is.EqualTo(listener1));
            topic.Subscribe(listener2);
            Assert.That(topic.Listener, Is.EqualTo(listener2));

            // When: 2 body1 calls
            topic.Publish(new BodyOnArea2D(body1, Area1));
            topic.Publish(new BodyOnArea2D(body1, Area2));
            // And: 1 body2 calls
            topic.Publish(new BodyOnArea2D(body2, Area1));
            topic.Publish(new BodyOnArea2D(body2, Area2));
            // nobody is listening for body3 events, so it will be ignored
            topic.Publish(new BodyOnArea2D(body3, Area1));

            // Then:
            Assert.That(body1Calls, Is.EqualTo(0));
            Assert.That(body2Calls, Is.EqualTo(2));
        }

        [Test]
        public IEnumerator TestFilterIsDisposed() {
            KinematicBody2D owner = new KinematicBody2D();
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();

            GodotUnicastTopic<BodyOnArea2D> topic = new GodotUnicastTopic<BodyOnArea2D>("T");
            int body1Calls = 0;
            var listener = new BodyOnArea2DListenerDelegate("Body1", owner, body1, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Detected, Is.EqualTo(body1));
                body1Calls++;
            });
            topic.Subscribe(listener);
            Assert.That(topic.Listener, Is.EqualTo(listener));

            // When events are published
            topic.Publish(new BodyOnArea2D(body1, Area1));
            topic.Publish(new BodyOnArea2D(body1, Area2));
            topic.Publish(new BodyOnArea2D(body1, Area2));
            topic.Publish(new BodyOnArea2D(body2, Area2));
            // Then
            Assert.That(body1Calls, Is.EqualTo(3));
            Assert.That(topic.Listener, Is.EqualTo(listener));

            // When filter is disposed
            body1.QueueFree();
            yield return null;
            // Then listener is still ok
            Assert.That(topic.Listener, Is.EqualTo(listener));

            // When new events are published
            topic.Publish(new BodyOnArea2D(body1, Area1));
            // Listener is deleted
            Assert.That(topic.Listener, Is.Null);

            // When new events are published
            topic.Publish(new BodyOnArea2D(body2, Area2));
            topic.Publish(new BodyOnArea2D(body1, Area2));
            topic.Publish(new BodyOnArea2D(body1, Area2));

            // No new calls
            Assert.That(body1Calls, Is.EqualTo(3));
        }

        [Test]
        public IEnumerator TestOwnerIsDisposed() {
            KinematicBody2D owner = new KinematicBody2D();
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();

            GodotUnicastTopic<BodyOnArea2D> topic = new GodotUnicastTopic<BodyOnArea2D>("T");
            int body1Calls = 0;
            var listener = new BodyOnArea2DListenerDelegate("Body1", owner, body1, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Detected, Is.EqualTo(body1));
                body1Calls++;
            });
            topic.Subscribe(listener);
            Assert.That(topic.Listener, Is.EqualTo(listener));

            // When events are published
            topic.Publish(new BodyOnArea2D(body1, Area1));
            topic.Publish(new BodyOnArea2D(body1, Area2));
            topic.Publish(new BodyOnArea2D(body1, Area2));
            topic.Publish(new BodyOnArea2D(body2, Area2));
            // Then
            Assert.That(body1Calls, Is.EqualTo(3));
            Assert.That(topic.Listener, Is.EqualTo(listener));

            // When owner is disposed
            owner.QueueFree();
            yield return null;
            // Then listener is still ok
            Assert.That(topic.Listener, Is.EqualTo(listener));

            // When new events are published
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

    }
}