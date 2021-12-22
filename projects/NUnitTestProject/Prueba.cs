using ClassLibrary1;
using Godot;
using NUnit.Framework;

namespace NUnitTestProject
{
    [TestFixture]
    public class NUnitTest
    {
        [Test]
        public void Test() {
            var x = new Node2D();
            Assert.AreEqual("Hello",new Class1().State);
        }
    }
}