using Game.Tools.Events;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Godot;

namespace Tests {
    public class Tests {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public void Test1() {
            var a = TopicMap.Instance;
            var godotUnicastTopic1 = new GodotUnicastTopic<BodyOnArea2D>();
            var gototUnicastTopic2 = new GodotUnicastTopic<BodyOnArea2D>();
            a.AddTopic("topic1", godotUnicastTopic1);
            a.AddTopic("topic2", gototUnicastTopic2);
            Assert.Pass();
        }
    }
}