using Godot;
using NUnit.Framework;
using Tools.Bus;
using Tools.Bus.Topics;

namespace Veronenger.Tests {
    [TestFixture]
    public class TopicMapTests : Node {
        Area2D Area1 = new Area2D();
        Area2D Area2 = new Area2D();

        [SetUp]
        public void Setup() {
            Area1.Name = "Area1";
            Area2.Name = "Area2";
        }

        [Test]
        public void TestTopicMap() {
            int body1Calls = 0;
            int body2Calls = 0;
            KinematicBody2D body1 = new KinematicBody2D();
            KinematicBody2D body2 = new KinematicBody2D();
            var t = Tools.Bus.TopicMap.Instance;
            t.AddTopic("t", new GodotTopic<BodyOnArea2D>("T"));

            /*
             It should work in the same way as TestGodotMulticast
             */
            t.Subscribe<GodotListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2DListenerDelegate("Body1", body1, body1, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Detected, Is.EqualTo(body1));
                body1Calls++;
            }));
            t.Subscribe<GodotListener<BodyOnArea2D>, BodyOnArea2D>("t", new BodyOnArea2DListenerDelegate("Body2", body2, body2, delegate(BodyOnArea2D @event) {
                Assert.That(@event.Detected, Is.EqualTo(body2));
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