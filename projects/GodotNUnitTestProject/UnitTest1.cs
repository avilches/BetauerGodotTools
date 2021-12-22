using System;
using ClassLibrary1;
using Godot;
using NUnit.Framework;

namespace GodotNUnitTestProject
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void Test1()
        {
            int i = 0;
            Console.WriteLine("Testsss");
            GD.Print("NUnitTest");
            var x = new Sprite();
            // var main = (Main)ResourceLoader.Load<PackedScene>("res://Main.tscn").Instance();
            // Assert.AreEqual("Main", main.Name);
            Assert.AreEqual("Hello",new Class1().State);
        }
    }
}
