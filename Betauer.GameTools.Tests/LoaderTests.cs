using System;
using System.Threading.Tasks;
using Betauer.Loader;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    [TestFixture]
    public class LoaderTests : Node {
        internal class ResourcesLoaderMetadataOk : ResourceLoaderContainer {
            [Resource("res://test-resources/1x1.png")]
            public Texture Texture; // parent type
            [Resource("res://test-resources/1x1.png")]
            public StreamTexture StreamTexture; // real type
            [Resource("res://test-resources/MyScene.tscn")]
            public PackedScene PackedScene;

            [Resource("res://test-resources/1x1.png")]
            public ResourceMetadata<StreamTexture> ResourceMetadataTexture;
            [Resource("res://test-resources/1x1.png")]
            public ResourceMetadata<StreamTexture> ResourceMetadataStreamTexture;
            [Resource("res://test-resources/MyScene.tscn")]
            public ResourceMetadata<PackedScene> ResourceMetadataPackedScene;

            [Resource("res://test-resources/1x1.png")]
            public ResourceMetadata ResourceMetadata;
            [Resource("res://test-resources/MyScene.tscn")]
            public ResourceMetadata ResourceMetadataScene;
        }

        internal class AdditionalResource {
            [Resource("res://test-resources/1x1.png")]
            public ResourceMetadata ResourceMetadata;
        }

        [Test(Description = "Resource load ok")]
        public async Task ResourcesOkTests() {
            var r1 = new ResourcesLoaderMetadataOk();
            var x1 = new AdditionalResource();
            var x2 = new AdditionalResource();
            await r1.SetAwaiter(async () => this.AwaitIdleFrame()).Bind(x1).Load(x2);
            Assert.That(r1.Texture, Is.EqualTo(r1.StreamTexture));
            Assert.That(r1.Texture, Is.EqualTo(r1.ResourceMetadataTexture.Resource));
            Assert.That(r1.Texture, Is.EqualTo(r1.ResourceMetadataStreamTexture.Resource));
            Assert.That(r1.Texture, Is.EqualTo(r1.ResourceMetadata.Resource));
            Assert.That(r1.PackedScene, Is.EqualTo(r1.ResourceMetadataPackedScene.Resource));
            Assert.That(r1.PackedScene, Is.EqualTo(r1.ResourceMetadataScene.Resource));
            
            Assert.That(x1.ResourceMetadata.Resource, Is.EqualTo(r1.ResourceMetadata.Resource));
        }

        internal class WrongType1 : ResourceLoaderContainer {
            [Resource("res://test-resources/MyScene.tscn")]
            public Texture PackedScene;
        }

        internal class WrongMetadataType : ResourceLoaderContainer {
            [Resource("res://test-resources/MyScene.tscn")]
            public ResourceMetadata<Texture> ResourceMetadataTexture;
        }

        internal class NotGodotType : ResourceLoaderContainer {
            [Resource("res://test-resources/MyScene.tscn")]
            public List Resource;
        }

        [Test(Description = "Resource load error")]
        public async Task WrongTypeTests() {
            var e1 = Assert.ThrowsAsync<ResourceLoaderException>(async () =>
                await new WrongType1().SetAwaiter(async () => { }).Load());
            var e2 = Assert.ThrowsAsync<ResourceLoaderException>(async () =>
                await new WrongMetadataType().SetAwaiter(async () => { }).Load());
            var e3 = Assert.ThrowsAsync<ResourceLoaderException>(async () =>
                await new NotGodotType().SetAwaiter(async () => { }).Load());
        }

        internal class ResourcesLoaderMetadataSceneOk : ResourceLoaderContainer {
            [Scene("res://test-resources/MyScene.tscn")]
            public Func<Node2D> Node2dFactory;

            [Scene("res://test-resources/MyScene.tscn")]
            public Func<Node> NodeFactory;

            [Scene("res://test-resources/MyScene.tscn")]
            public Node2D Node2d;

            [Scene("res://test-resources/MyScene.tscn")]
            public Node Node;
        }

        internal class AdditionalScene {
            [Scene("res://test-resources/MyScene.tscn")]
            public Node Node;
        }
        
        [Test(Description = "PackedScene to Scene ok")]
        public async Task ResourcesSceneOkTests() {
            var r1 = new ResourcesLoaderMetadataSceneOk();
            var x1 = new AdditionalScene();
            var x2 = new AdditionalScene();
            await r1.SetAwaiter(async () => await this.AwaitIdleFrame()).Bind(x1).Load(x2);
            var node2DCreated1 = r1.Node2dFactory.Invoke();
            var node2DCreated2 = r1.Node2dFactory.Invoke();
            Assert.That(node2DCreated1, Is.Not.Null);
            Assert.That(node2DCreated2, Is.Not.Null);
            Assert.That(r1.Node2d, Is.Not.Null);
            Assert.That(r1.Node2d, Is.Not.EqualTo(node2DCreated1));
            Assert.That(r1.Node2d, Is.Not.EqualTo(node2DCreated2));
            Assert.That(node2DCreated1, Is.Not.EqualTo(node2DCreated2));
            
            Assert.That(r1.Node, Is.TypeOf(typeof(Node2D)));
            Assert.That(r1.NodeFactory.Invoke(), Is.TypeOf(typeof(Node2D)));
            
            Assert.That(x1.Node, Is.TypeOf(typeof(Node2D)));
            Assert.That(x2.Node, Is.TypeOf(typeof(Node2D)));
            Assert.That(node2DCreated1, Is.Not.EqualTo(node2DCreated2));
            Assert.That(node2DCreated1, Is.Not.EqualTo(x1));
            Assert.That(node2DCreated1, Is.Not.EqualTo(x2));


        }
        internal class ResourcesLoaderMetadataSceneWrong : ResourceLoaderContainer {
            [Scene("res://test-resources/MyScene.tscn")]
            public Spatial Spatial;
        }
        
        internal class ResourcesLoaderMetadataFuncTypeSceneWrong : ResourceLoaderContainer {
            [Scene("res://test-resources/MyScene.tscn")]
            public Func<Node, Node> SpatialFactory;
        }
        
        internal class ResourcesLoaderMetadataSceneFuncWrong : ResourceLoaderContainer {
            [Scene("res://test-resources/MyScene.tscn")]
            public Func<Spatial> SpatialFactory;
        }
        
        [Test]
        public async Task ResourcesSceneWrongTests() {
            var e1 = Assert.ThrowsAsync<ResourceLoaderException>(async () =>
                await new ResourcesLoaderMetadataSceneWrong().SetAwaiter(async () => { }).Load());
            var e2 = Assert.ThrowsAsync<ResourceLoaderException>(async () =>
                await new ResourcesLoaderMetadataFuncTypeSceneWrong().SetAwaiter(async () => { }).Load());
            
            var r1 = new ResourcesLoaderMetadataSceneFuncWrong();
            await r1.SetAwaiter(async () => await this.AwaitIdleFrame()).Load();
            Assert.Throws<InvalidCastException>(() => r1.SpatialFactory());
        }
    }
}