using System.Collections;
using Godot;
using NUnit.Framework;
using Tools.Bus.Topics;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers.Autoload;
using Debug = Tools.Debug;

namespace Veronenger.Tests {
    [TestFixture]
    public class PlatformManagerTests : Node {
        private int Body1Calls = 0;
        private int Body2Calls = 0;

        [SetUp]
        public void Setup() {
            Debug.TESTING = true;
            Body1Calls = 0;
            Body2Calls = 0;
        }

        [Test]
        public IEnumerator PlatformMap() {
            KinematicBody2D body = CreateKinematicBody2D("player", 0, 0);
            KinematicBody2D body2 = CreateKinematicBody2D("Enemy", 0, 0);
            Area2D area2D = CreateArea2D("slopeDown", 2000, 2000);

            GameManager.Instance.SlopeStairsManager.ConfigureSlopeStairsDown(area2D);
            GameManager.Instance.SlopeStairsManager.SubscribeSlopeStairsDown(new BodyOnArea2DEnterListenerDelegate("Body1", body, _OnEventBody1));
            yield return null;

            // They are not colliding
            Assert.That(Body1Calls, Is.EqualTo(0));
            Assert.That(Body2Calls, Is.EqualTo(0));

            // Make them collide
            body.Position = area2D.Position = Vector2.Zero;
            yield return null;

            Assert.That(Body1Calls, Is.EqualTo(1));
            Assert.That(Body2Calls, Is.EqualTo(0));
        }

        private Area2D CreateArea2D(string name, int x = 0, int y = 0) {
            var area2D = new Area2D();
            area2D.Position = new Vector2(x, y);
            area2D.Name = name;
            AddCollider(area2D);
            AddChild(area2D);
            return area2D;
        }

        private KinematicBody2D CreateKinematicBody2D(string name, int x = 0, int y = 0) {
            var body2D = new KinematicBody2D();
            body2D.Position = new Vector2(x, y);
            body2D.Name = name;
            AddCollider(body2D);
            AddChild(body2D);
            return body2D;
        }

        private PlayerController CreatePlayerController(string name, int x = 0, int y = 0) {
            var body2D = new PlayerController();
            body2D.Position = new Vector2(x, y);
            body2D.Name = name;
            AddCollider(body2D);
            AddChild(body2D);
            return body2D;
        }

        private Node2D AddCollider(Node2D node) {
            var rectangleShape2D = new RectangleShape2D();
            rectangleShape2D.Extents = new Vector2(4, 4);
            var collisionShape2D = new CollisionShape2D();
            collisionShape2D.Shape = rectangleShape2D;
            node.AddChild(collisionShape2D);
            return node;
        }

        public void _OnEventBody1(BodyOnArea2D evt) {
            Body1Calls++;
        }

        public void _OnEventBody2(BodyOnArea2D evt) {
            Body2Calls++;
        }
    }
}