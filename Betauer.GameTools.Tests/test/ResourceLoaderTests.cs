using System;
using System.Threading.Tasks;
using Betauer.Application.Lifecycle;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.GameTools.Tests;

[TestFixture]
public class ResourceLoaderTests {
    [Configuration]
    [Loader("GameLoader")]
    [Resource<Texture2D>("1x1.png", "res://test-resources/1x1.png")]
    [Scene.Transient<Node2D>(Name = "SceneTransient", Path = "res://test-resources/MyScene.tscn")]
    public class MainResources {
        [Singleton] public ResourceLoaderContainer GameLoader => new();
    }

    [Configuration]
    [Loader("GameLoader", Tag = "main2")]
    [Scene.Singleton<Node2D>(Name = "SceneSingleton", Path = "res://test-resources/MyScene.tscn")]
    [Scene.Singleton<Node2D>(Name = "SceneSingletonLazy", Lazy = true, Path = "res://test-resources/MyScene.tscn")]
    public class MainResources2 {
    }

    [Test]
    public async Task BasicTests() {
        var c = new Container();
        c.Build(di => {
            di.Scan<MainResources>();
            di.Scan<MainResources2>();
        });

        var resource = c.Resolve<ResourceHolder<Texture2D>>("1x1.png");
        var transient = c.Resolve<ITransient<Node2D>>("SceneTransient");
        var single = c.Resolve<ILazy<Node2D>>("SceneSingleton");
        var singleLazy = c.Resolve<ILazy<Node2D>>("SceneSingletonLazy");

        Assert.That(resource.Resource, Is.Null);
        Assert.Throws<Exception>(() => transient.Create());
        Assert.Throws<Exception>(() => single.Get());
        Assert.Throws<Exception>(() => singleLazy.Get());
        Assert.That(single.HasValue(), Is.False);
        Assert.That(singleLazy.HasValue(), Is.False);

        // load default resources only
        var loader = c.Resolve<ResourceLoaderContainer>("GameLoader");
        await loader.LoadResources();

        Assert.That(resource.Resource, Is.Not.Null);
        Assert.That(transient.Create(), Is.Not.EqualTo(transient.Create()));

        // both singleton belongs to main2 tag, so it's not loaded and it fails
        Assert.Throws<Exception>(() => single.Get());
        Assert.Throws<Exception>(() => singleLazy.Get());
        Assert.That(single.HasValue(), Is.False);
        Assert.That(singleLazy.HasValue(), Is.False);

        await loader.LoadResources("main2");
        
        Assert.That(single.HasValue(), Is.True);
        Assert.That(singleLazy.HasValue(), Is.False);
        Assert.That(single.Get(), Is.EqualTo(single.Get()));
        Assert.That(singleLazy.Get(), Is.EqualTo(singleLazy.Get()));

        Assert.That(singleLazy.HasValue(), Is.True);

        loader.UnloadResources();

        Assert.That(resource.Resource, Is.Null);
        Assert.Throws<Exception>(() => transient.Create());

        // singleton instances work forever no matter if the resource is loaded or unloaded
        Assert.That(single.Get(), Is.EqualTo(single.Get()));
    }
}