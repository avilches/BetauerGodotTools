using System;
using System.Collections.Generic;
using Betauer.Tools.Logging;
using Betauer.OnReady;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    [TestFixture]
    public partial class ScannerOnReadyTests : Node {
        [SetUp]
        public void Setup() {
            LoggerFactory.OverrideTraceLevel(TraceLevel.All);
        }

        internal partial class MyArea2D : Area2D {
            [OnReady("myControl/mySprite")] internal Node2D prop { set; get; }

            [OnReady("myControl/mySprite")] internal Node2D node2d;

            [OnReady("myControl/mySprite")] internal Sprite2D sprite;

            [OnReady("myControl/mySprite", Nullable = true)]
            internal IDisposable disposable;

            [OnReady("xxxxxxxxxxxxxx", Nullable = true)]
            internal Sprite2D allowNulls;
        }

        [Test(Description = "Fail if not found")]
        public void FailNotFound() {
            var myArea2D = new MyArea2D();
            OnReadyFieldException? e =
                Assert.Throws<OnReadyFieldException>(() => OnReadyScanner.ScanAndInject(myArea2D));
            Assert.That(e!.Message.Contains("null value"));
        }

        [Test(Description = "OnReady working")]
        public void Working() {
            var myArea2D = new MyArea2D();
            AddChild(myArea2D);
            
            var control = new Control();
            control.Name = "myControl";
            myArea2D.AddChild(control);

            var sprite = new Sprite2D();
            sprite.Name = "mySprite";
            control.AddChild(sprite);

            OnReadyScanner.ScanAndInject(myArea2D);

            Assert.That(myArea2D.prop, Is.EqualTo(sprite));
            Assert.That(myArea2D.node2d, Is.EqualTo(sprite));
            Assert.That(myArea2D.sprite, Is.EqualTo(sprite));
            Assert.That(myArea2D.disposable, Is.EqualTo(sprite));
            Assert.That(myArea2D.allowNulls, Is.Null);
        }

        [Test(Description = "OnReady fail on wrong type")]
        public void OnReadyFailWrongType() {
            var myArea2D = new MyArea2D();
            AddChild(myArea2D);

            var control = new Control();
            control.Name = "myControl";
            myArea2D.AddChild(control);

            var sprite = new Node2D();
            sprite.Name = "mySprite";
            control.AddChild(sprite);

            OnReadyFieldException? e = Assert.Throws<OnReadyFieldException>(() => OnReadyScanner.ScanAndInject(myArea2D));
            Assert.That(myArea2D.node2d, Is.EqualTo(sprite));
            Assert.That(myArea2D.sprite, Is.Null);
            Assert.That(myArea2D.disposable, Is.Null);  // Is null because the error happened before to 
            Assert.That(e!.Message.Contains("incompatible type"));
        }

        internal partial class NodeWithChildren : Node {
            [OnReady("Children")] internal List<Sprite2D> listSprites;
            [OnReady("Children")] internal List<Node> listNodes;

            [OnReady("Children")] internal Sprite2D[] arraySprites;
            [OnReady("Children")] internal Node[] arrayNodes;

            [OnReady("Children")] internal Dictionary<string, Sprite2D> dictSprites;
            [OnReady("Children")] internal Dictionary<string, Node> dictNodes;
        }

        [Test(Description = "OnReady children")]
        public void ChildrenTests() {
            var nodeWithChildren = new NodeWithChildren();
            AddChild(nodeWithChildren);
            
            nodeWithChildren.AddChild(CreateNodeWithChildren("Children"));
            OnReadyScanner.ScanAndInject(nodeWithChildren);

            Assert.That(nodeWithChildren.listSprites.Count, Is.EqualTo(2));
            Assert.That(nodeWithChildren.listSprites[0].Name.ToString(), Is.EqualTo("Sprite1"));
            Assert.That(nodeWithChildren.listSprites[1].Name.ToString(), Is.EqualTo("Sprite2"));
            
            Assert.That(nodeWithChildren.listNodes.Count, Is.EqualTo(3));
            Assert.That(nodeWithChildren.listNodes[0].Name.ToString(), Is.EqualTo("Sprite1"));
            Assert.That(nodeWithChildren.listNodes[1].Name.ToString(), Is.EqualTo("K"));
            Assert.That(nodeWithChildren.listNodes[2].Name.ToString(), Is.EqualTo("Sprite2"));
            
            Assert.That(nodeWithChildren.arraySprites.Length, Is.EqualTo(2));
            Assert.That(nodeWithChildren.arraySprites[0].Name.ToString(), Is.EqualTo("Sprite1"));
            Assert.That(nodeWithChildren.arraySprites[1].Name.ToString(), Is.EqualTo("Sprite2"));

            Assert.That(nodeWithChildren.arrayNodes.Length, Is.EqualTo(3));
            Assert.That(nodeWithChildren.arrayNodes[0].Name.ToString(), Is.EqualTo("Sprite1"));
            Assert.That(nodeWithChildren.arrayNodes[1].Name.ToString(), Is.EqualTo("K"));
            Assert.That(nodeWithChildren.arrayNodes[2].Name.ToString(), Is.EqualTo("Sprite2"));


            Assert.That(nodeWithChildren.dictSprites.Count, Is.EqualTo(2));
            Assert.That(nodeWithChildren.dictSprites.ContainsKey("Sprite1"));
            Assert.That(nodeWithChildren.dictSprites.ContainsKey("Sprite2"));

            Assert.That(nodeWithChildren.dictNodes.Count, Is.EqualTo(3));
            Assert.That(nodeWithChildren.dictNodes.ContainsKey("Sprite1"));
            Assert.That(nodeWithChildren.dictNodes.ContainsKey("K"));
            Assert.That(nodeWithChildren.dictNodes.ContainsKey("Sprite2"));
        }

        private Node CreateNodeWithChildren(string name) {
            var node = new Node();
            node.Name = name;

            node.AddChild(new Sprite2D {
                Name = "Sprite1"
            });
            node.AddChild(new CharacterBody2D {
                Name = "K"
            });
            node.AddChild(new Sprite2D {
                Name = "Sprite2"
            });
            return node;
        }
    }
}