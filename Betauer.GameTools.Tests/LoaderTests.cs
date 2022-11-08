using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Loader;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    [TestFixture]
    public class LoaderTests : Node {
        internal class ResourcesLoaderMetadataOk : ResourceLoaderContainer {
            [Load("res://test-resources/1x1.png")]
            public StreamTexture StreamTexture; // real type
            [Load("res://test-resources/1x1.png")]
            public Texture Texture; // parent type
            [Load("res://test-resources/MyScene.tscn")]
            public PackedScene PackedScene;

            [Load("res://test-resources/1x1.png")]
            public ResourceMetadata<StreamTexture> ResourceMetadataTexture;
            [Load("res://test-resources/1x1.png")]
            public ResourceMetadata<StreamTexture> ResourceMetadataStreamTexture;
            [Load("res://test-resources/MyScene.tscn")]
            public ResourceMetadata<PackedScene> ResourceMetadataPackedScene;

            [Load("res://test-resources/1x1.png")]
            public ResourceMetadata ResourceMetadata;
            [Load("res://test-resources/MyScene.tscn")]
            public ResourceMetadata ResourceMetadataScene;
        }

        [Test(Description = "Resource load ok")]
        // [Ignore("It fails in github actions")]
        public async Task ResourcesOkTests() {
            var r = new ResourcesLoaderMetadataOk();
            await r.SetAwaiter(async () => this.AwaitIdleFrame()).Load();
            Assert.That(r.Texture, Is.EqualTo(r.StreamTexture));
            Assert.That(r.Texture, Is.EqualTo(r.ResourceMetadataTexture.Resource));
            Assert.That(r.Texture, Is.EqualTo(r.ResourceMetadataStreamTexture.Resource));
            Assert.That(r.Texture, Is.EqualTo(r.ResourceMetadata.Resource));
            Assert.That(r.PackedScene, Is.EqualTo(r.ResourceMetadataPackedScene.Resource));
            Assert.That(r.PackedScene, Is.EqualTo(r.ResourceMetadataScene.Resource));
        }
        
        internal class ResourceHolder1 {
            [Load("res://test-resources/1x1.png")]
            public Texture Texture;
        }

        internal class ResourceHolder2 {
            [Load("res://test-resources/1x1.png")]
            public ResourceMetadata<StreamTexture> ResourceMetadataTexture;
        }

        [Test(Description = "Resource load From other resources")]
        // [Ignore("It fails in github actions")]
        public async Task ResourcesLoadFromTests() {
            var c = new ResourceLoaderContainer(GetTree());
            var r1 = new ResourceHolder1();
            var r2 = new ResourceHolder2();
            await c.From(r1, r2).Load();
            Assert.That(r1.Texture, Is.EqualTo(r2.ResourceMetadataTexture.Resource));
        }

        internal class ResourcesLoaderMetadataSceneOk : ResourceLoaderContainer {
            [Load("res://test-resources/MyScene.tscn")]
            public Func<Node2D> Node2dFactory;

            [Load("res://test-resources/MyScene.tscn")]
            public Func<Node> NodeFactory;

            [Load("res://test-resources/MyScene.tscn")]
            public Node2D Node2d; // parent type

            [Load("res://test-resources/MyScene.tscn")]
            public Node Node; // real type
        }

        [Test(Description = "Test PackedScene resources")]
        public async Task PackedSceneResourceTests() {
            var r = new ResourcesLoaderMetadataSceneOk();
            await r.SetAwaiter(async () => await this.AwaitIdleFrame()).Load();
            var node2DCreated1 = r.Node2dFactory.Invoke();
            var node2DCreated2 = r.Node2dFactory.Invoke();
            Assert.That(node2DCreated1, Is.Not.Null);
            Assert.That(node2DCreated2, Is.Not.Null);
            Assert.That(r.Node2d, Is.Not.Null);
            Assert.That(r.Node2d, Is.Not.EqualTo(node2DCreated1));
            Assert.That(r.Node2d, Is.Not.EqualTo(node2DCreated2));
            Assert.That(node2DCreated1, Is.Not.EqualTo(node2DCreated2));
            
            Assert.That(r.Node, Is.TypeOf(typeof(Node2D)));
            Assert.That(r.NodeFactory.Invoke(), Is.TypeOf(typeof(Node2D)));
        }

        internal class ResourceWithNameDefinition {
            [Load("1x1-1", "res://test-resources/1x1.png")]
            public StreamTexture StreamTexture;
            
            // Different name, same resource
            [Load("1x1-2", "res://test-resources/1x1.png")]
            public Texture Texture;
            
            [Load("MyScene","res://test-resources/MyScene.tscn")]
            public Node2D Node2d;
        }

        internal class ResourceWithName {
            [Resource("1x1-1")]
            public Texture Texture1;
            
            [Resource("1x1-2")]
            public Texture Texture2;
            
            [Resource("res://test-resources/1x1.png")]
            public Texture Texture3;
            
            [Resource("MyScene")]
            public Node MyScene1;
            
            [Resource("res://test-resources/MyScene.tscn")]
            public Node MyScene2;

            [Load("x", "b")]
            public object ignored;
        }

        [Test(Description = "Test load with [Resource]")]
        public async Task ResourceWithNameDefinitionTests() {
            var c = new ResourceLoaderContainer(GetTree());
            var r = new ResourceWithNameDefinition();
            await c.From(r).Load();

            Assert.That(c.Contains("res://test-resources/1x1.png"), Is.True);
            Assert.That(c.Contains("1x1-1"), Is.True);
            Assert.That(c.Contains("1x1-2"), Is.True);
            Assert.That(c.Contains("NOPE"), Is.False);

            Assert.That(c.Resource<Texture>("1x1-1"), Is.EqualTo(r.Texture));
            Assert.That(c.Resource<Texture>("1x1-2"), Is.EqualTo(r.Texture));
            Assert.That(c.Scene<Node>("MyScene"), Is.TypeOf<Node2D>());

            Assert.Throws<InvalidCastException>(() => c.Resource<Node2D>("1x1-1"));
            Assert.Throws<InvalidCastException>(() => c.Scene<Node3D>("MyScene"));
            
            Assert.Throws<KeyNotFoundException>(() => c.Resource<Texture>("NOT FOUND"));
            Assert.Throws<KeyNotFoundException>(() => c.Scene<Node>("NOT FOUND"));

            var x1 = new ResourceWithName();
            c.Inject(x1);
            Assert.That(x1.Texture1, Is.EqualTo(r.StreamTexture));
            Assert.That(x1.Texture2, Is.EqualTo(r.StreamTexture));
            Assert.That(x1.Texture3, Is.EqualTo(r.StreamTexture));
            Assert.That(x1.ignored, Is.Null);
            
            Assert.That(x1.MyScene1, Is.TypeOf<Node2D>());
            Assert.That(x1.MyScene2, Is.TypeOf<Node2D>());
            Assert.That(r.Node2d, Is.Not.EqualTo(x1.MyScene1));
            Assert.That(r.Node2d, Is.Not.EqualTo(x1.MyScene2));
        }

        internal class ResourcesNotFound : ResourceLoaderContainer {
            [Load("res://test-resources/notfound.x")]
            public Node node;
        }
        
        internal class ResourcesLoaderMetadataSceneWrong : ResourceLoaderContainer {
            [Load("res://test-resources/MyScene.tscn")]
            public Node3D Node3D;
        }
        
        internal class ResourcesLoaderMetadataFuncTypeSceneWrong : ResourceLoaderContainer {
            [Load("res://test-resources/MyScene.tscn")]
            public Func<Node, Node> SpatialFactory;
        }
        
        internal class NoPackedSceneOrNodeType : ResourceLoaderContainer {
            [Load("res://test-resources/MyScene.tscn")]
            public Texture PackedScene;
        }

        internal class NoPackedSceneInResourceMetadataType : ResourceLoaderContainer {
            [Load("res://test-resources/MyScene.tscn")]
            public ResourceMetadata<Texture> ResourceMetadataTexture;
        }

        internal class NoNodeInPackedSceneType : ResourceLoaderContainer {
            [Load("res://test-resources/MyScene.tscn")]
            public List PackedScene;
        }

        internal class WrongResourceType : ResourceLoaderContainer {
            [Load("res://test-resources/1x1.png")]
            public Node Texture;
        }

        internal class FuncNotAllowedInResourcesType : ResourceLoaderContainer {
            [Load("res://test-resources/1x1.png")]
            public Func<Texture> Texture;
        }

        internal class DuplicateResourceName : ResourceLoaderContainer{
            [Load("a", "res://test-resources/1x1.png")]
            public Texture Texture1;

            [Load("a", "res://test-resources/1x1.png")]
            public Texture Texture2;
        }

        [Test]
        public async Task WrongResourcesDefinitionTests() {
            async Task Load(ResourceLoaderContainer o) => await o.SetAwaiter(async () => { }).Load();

            Assert.ThrowsAsync<ResourceLoaderException>(async () => await new ResourceLoaderContainer().Load());
            
            Assert.ThrowsAsync<Exception>(async () => await Load(new ResourcesNotFound()));
            Assert.ThrowsAsync<ResourceLoaderException>(async () => await Load(new ResourcesLoaderMetadataSceneWrong()));
            Assert.ThrowsAsync<ResourceLoaderException>(async () => await Load(new ResourcesLoaderMetadataFuncTypeSceneWrong()));
            Assert.ThrowsAsync<ResourceLoaderException>(async () => await Load(new NoPackedSceneOrNodeType()));
            Assert.ThrowsAsync<ResourceLoaderException>(async () => await Load(new NoPackedSceneInResourceMetadataType()));
            Assert.ThrowsAsync<ResourceLoaderException>(async () => await Load(new NoNodeInPackedSceneType()));
            Assert.ThrowsAsync<ResourceLoaderException>(async () => await Load(new WrongResourceType()));
            Assert.ThrowsAsync<ResourceLoaderException>(async () => await Load(new FuncNotAllowedInResourcesType()));
            Assert.ThrowsAsync<ResourceLoaderException>(async () => await Load(new DuplicateResourceName()));
        }

        internal class ResourcesLoaderMetadataSceneFuncWrong : ResourceLoaderContainer {
            [Load("res://test-resources/MyScene.tscn")]
            public Func<Node3D> SpatialFactory;
        }

        [Test(Description = "The PackedScene is a Node2D. The function return Func<Node3D>. This fails only at runtime")]
        public async Task WrongPackedSceneFunctionReturnType() {
            var r1 = new ResourcesLoaderMetadataSceneFuncWrong();
            await r1.SetAwaiter(async () => await this.AwaitIdleFrame()).Load();
            Assert.Throws<InvalidCastException>(() => r1.SpatialFactory());
        }

        internal class ResourceNameNotFound {
            [Resource("NOT FOUND")] public Texture Texture;
        }

        [Test(Description = "Try to inject a missing resource name: not found")]
        public async Task ResourceNameNotFoundTest() {
            var r = new ResourcesLoaderMetadataOk();
            await r.SetAwaiter(async () => this.AwaitIdleFrame()).Load();
            Assert.Throws<KeyNotFoundException>(() => r.Inject(new ResourceNameNotFound()));
        }

        [Test(Description = "No loads, registry null, methods should work properly")]
        public async Task ContainsResourceAndSceneMethodWhenNoLoadTest() {
            var c = new ResourceLoaderContainer(GetTree());
            Assert.That(c.Contains("NOPE"), Is.False);
            Assert.Throws<KeyNotFoundException>(() => c.Resource<Texture>("NOT FOUND"));
            Assert.Throws<KeyNotFoundException>(() => c.Scene<Node>("NOT FOUND"));
        }

        [Test(Description = "Inject without Load fails")]
        public async Task ResourceInjectWithoutLoad() {
            var c = new ResourceLoaderContainer(GetTree());
            Assert.Throws<KeyNotFoundException>(() => c.Inject(new ResourceWithName()));
        }
    }
}