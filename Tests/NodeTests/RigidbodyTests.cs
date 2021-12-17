using System.Collections;
using Godot;
using NUnit.Framework;

namespace GodotTests.NodeTests {
    [TestFixture]
    [Ignore("")]
    public class RigidbodyTests : RigidBody {
        [Test]
        public IEnumerator RigidbodyShouldFallOverOneSecond() {
            uint finishTime = OS.GetTicksMsec() + 1000;

            while (OS.GetTicksMsec() < finishTime) {
                yield return null;
            }

            Assert.That(Translation.y, Is.LessThan(-1));
        }


        [Test]
        public IEnumerator KinematicRigidbodyShouldNotFall() {
            uint finishTime = OS.GetTicksMsec() + 1000;

            Mode = ModeEnum.Kinematic;

            while (OS.GetTicksMsec() < finishTime) {
                yield return null;
            }

            Assert.That(Translation.y, Is.EqualTo(0).Within(0.01f));
        }
    }
}