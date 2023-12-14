using System;
using System.Threading.Tasks;
using Betauer.Application.Lifecycle;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.GameTools.Tests;

[TestRunner.Test]
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
    public class MainResources2 {
    }

    [TestRunner.Test]
    public async Task BasicTests() {
        var c = new Container();
        c.Build(di => {
            di.Scan<MainResources>();
            di.Scan<MainResources2>();
        });

        var resource = c.Resolve<ResourceHolder<Texture2D>>("1x1.png");
        var transient = c.Resolve<ITransient<Node2D>>("Factory:SceneTransient");
        var single = c.Resolve<ILazy<Node2D>>("Factory:SceneSingleton");

        Assert.That(resource.Resource, Is.Null);
        Assert.Throws<Exception>(() => transient.Create());
        Assert.Throws<Exception>(() => single.Get());

        var loader = c.Resolve<ResourceLoaderContainer>("GameLoader");

        // load default resources only
        await loader.LoadResources();

        Assert.That(resource.Resource, Is.Not.Null);
        Assert.That(transient.Create(), Is.Not.EqualTo(transient.Create()));

        // singleton belongs to main2 tag, so it's not loaded and it fails
        Assert.Throws<Exception>(() => single.Get());

        await loader.LoadResources("main2");
        Assert.That(single.Get(), Is.EqualTo(single.Get()));

        loader.UnloadResources();

        Assert.That(resource.Resource, Is.Null);
        Assert.Throws<Exception>(() => transient.Create());

        // singleton instances work forever no matter if the resource is loaded or unloaded
        Assert.That(single.Get(), Is.EqualTo(single.Get()));
    }
}