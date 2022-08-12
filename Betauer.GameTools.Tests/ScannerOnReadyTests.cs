using System;
using Betauer.OnReady;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    [TestFixture]
    public class ScannerOnReadyTests : Node {
        [SetUp]
        public void Setup() {
            LoggerFactory.OverrideTraceLevel(TraceLevel.All);
        }

        internal class MyArea2D : Area2D {
            [OnReady("myControl/mySprite")] internal Node2D prop { set; get; }

            [OnReady("myControl/mySprite")]
            internal Node2D node2d;

            [OnReady("myControl/mySprite")]
            internal Sprite sprite;

            [OnReady("myControl/mySprite", Nullable = true)]
            internal IDisposable disposable;

            [OnReady("xxxxxxxxxxxxxx", Nullable = true)]
            internal Sprite allowNulls;

            internal int x = 0;

            public override void _Ready() {
                x++;
            }

        }

        [Test(Description = "Fail if not found")]
        public void FailNotFound() {
            var myArea2D = new MyArea2D();
            OnReadyFieldException? e = Assert.Throws<OnReadyFieldException>(() => OnReadyScanner.ScanAndInject(myArea2D));
            Assert.That(e!.Message.Contains("null value"));
        }

        [Test(Description = "OnReady working")]
        public void Working() {
            var myArea2D = new MyArea2D();
            var control = new Control();
            control.Name = "myControl";
            myArea2D.AddChild(control);

            var sprite = new Sprite();
            sprite.Name = "mySprite";
            control.AddChild(sprite);

            AddChild(myArea2D);
            OnReadyScanner.ScanAndInject(myArea2D);
            OnReadyScanner.ScanAndInject(control);
            OnReadyScanner.ScanAndInject(sprite);

            Assert.That(myArea2D.prop, Is.EqualTo(sprite));
            Assert.That(myArea2D.node2d, Is.EqualTo(sprite));
            Assert.That(myArea2D.sprite, Is.EqualTo(sprite));
            Assert.That(myArea2D.disposable, Is.EqualTo(sprite));
            Assert.That(myArea2D.x, Is.EqualTo(1));
            Assert.That(myArea2D.allowNulls, Is.Null);
        }

        [Test(Description = "OnReady fail on wrong type")]
        public void OnReadyFailWrongType() {
            var myArea2D = new MyArea2D();
            var control = new Control();
            control.Name = "myControl";
            myArea2D.AddChild(control);

            var sprite = new Node2D();
            sprite.Name = "mySprite";
            control.AddChild(sprite);

            OnReadyScanner.ScanAndInject(control);
            OnReadyScanner.ScanAndInject(sprite);

            OnReadyFieldException? e = Assert.Throws<OnReadyFieldException>(() => OnReadyScanner.ScanAndInject(myArea2D));
            Assert.That(myArea2D.x, Is.EqualTo(0)); // Ready() can't be executed because the error
            Assert.That(myArea2D.node2d, Is.EqualTo(sprite));
            Assert.That(myArea2D.sprite, Is.Null);
            Assert.That(myArea2D.disposable, Is.Null);  // Is null because the error happened before to 
            Assert.That(e!.Message.Contains("incompatible type"));
        }
    }
}