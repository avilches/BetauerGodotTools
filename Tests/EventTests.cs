using Godot;
using NUnit.Framework;
using Tools.Events;
using Debug = Tools.Events.Debug;

namespace Veronenger.Tests {
    [TestFixture]
    public class EventTests {
        KinematicBody2D Body1 = new KinematicBody2D();
        KinematicBody2D Body2 = new KinematicBody2D();
        KinematicBody2D Body3 = new KinematicBody2D();
        Area2D Area1 = new Area2D();
        Area2D Area2 = new Area2D();
        Area2D Area3 = new Area2D();
        private int Body1Calls = 0;
        private int Body2Calls = 0;

        [SetUp]
        public void Setup() {
            Debug.TESTING = true;
            Area1.Name = "Area1";
            Area2.Name = "Area2";
            Area3.Name = "Area3";
            Body1.Name = "Body1";
            Body2.Name = "Body2";
            Body3.Name = "Body3";
            Body1Calls = 0;
            Body2Calls = 0;
        }

        [Test]
        public void TestGodotUnicast() {
            GodotUnicastTopic<BodyOnArea2D> topic = new GodotUnicastTopic<BodyOnArea2D>();
            /*
             Unicast only allows one subscriber, so, Body1Calls is 0 and Body2 have two calls
             */
            topic.Subscribe(new BodyOnArea2DEnterListenerDelegate(Body1, _OnEventBody1));
            topic.Subscribe(new BodyOnArea2DEnterListenerDelegate(Body2, _OnEventBody2));
            topic.Publish(new BodyOnArea2D(Body1, Area1));
            topic.Publish(new BodyOnArea2D(Body1, Area2));
            topic.Publish(new BodyOnArea2D(Body2, Area1));
            topic.Publish(new BodyOnArea2D(Body2, Area2));
            Assert.That(Body1Calls, Is.EqualTo(0));
            Assert.That(Body2Calls, Is.EqualTo(2));
        }

        [Test]
        public void TestGodotMulticast() {
            GodotMulticastTopic<BodyOnArea2D> topic = new GodotMulticastTopic<BodyOnArea2D>();
            /*
             Multicast allows multiple subscribers, so, Body1 and Body2 have two calls
             */
            topic.Subscribe(new BodyOnArea2DEnterListenerDelegate(Body1, _OnEventBody1));
            topic.Subscribe(new BodyOnArea2DEnterListenerDelegate(Body2, _OnEventBody2));
            topic.Publish(new BodyOnArea2D(Body1, Area1));
            topic.Publish(new BodyOnArea2D(Body1, Area2));
            topic.Publish(new BodyOnArea2D(Body2, Area1));
            topic.Publish(new BodyOnArea2D(Body2, Area2));
            Assert.That(Body1Calls, Is.EqualTo(2));
            Assert.That(Body2Calls, Is.EqualTo(2));
        }

        [Test]
        public void TestTopicMap() {
            var t = TopicMap.Instance;
            t.AddTopic("t", new GodotMulticastTopic<BodyOnArea2D>());

            /*
             It should work in the same way as TestGodotMulticast
             */
            t.Subscribe<NodeFromListenerDelegate<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2DEnterListenerDelegate(Body1, _OnEventBody1));
            t.Subscribe<NodeFromListenerDelegate<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2DEnterListenerDelegate(Body2, _OnEventBody2));
            t.Publish<NodeFromListenerDelegate<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2D(Body1, Area1));
            t.Publish<NodeFromListenerDelegate<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2D(Body1, Area2));
            t.Publish<NodeFromListenerDelegate<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2D(Body2, Area1));
            t.Publish<NodeFromListenerDelegate<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2D(Body2, Area2));
            Assert.That(Body1Calls, Is.EqualTo(2));
            Assert.That(Body2Calls, Is.EqualTo(2));
        }

        public void _OnEventBody1(BodyOnArea2D evt) {
            Assert.That(evt.Body, Is.EqualTo(Body1));
            Body1Calls++;
        }

        public void _OnEventBody2(BodyOnArea2D evt) {
            Assert.That(evt.Body, Is.EqualTo(Body2));
            Body2Calls++;
        }
    }
}