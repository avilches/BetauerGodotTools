using System.Threading.Tasks;
using Betauer.Application;
using Betauer.DI.Attributes;
using Betauer.TestRunner;
using Container = Betauer.DI.Container;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests; 

[TestRunner.Test]
public partial class GodotContainerTests : Node {

    [Configuration]
    public class GodotContainerConfiguration {
        [Singleton] public Node NoAdded => new() { Name = "NoAdded" };
        [Singleton(Flags = "AddToTree")] public Node Added => new() { Name = "Added" };
    }

    [Transient]
    public class Injected {
        [Inject] public Node NoAdded { get; set; }
        [Inject] public Node Added { get; set; }
    }
    
    [TestRunner.Test]
    public async Task BasicTests() {
        var container = new Container()
            .Build(builder => {
                builder.InjectOnEnterTree(GetTree());
                builder.Scan<GodotContainerConfiguration>();
                builder.Scan<Injected>();
            });

        await this.AwaitProcessFrame();
        var injected = container.Resolve<Injected>();
        Assert.NotNull(injected.Added.GetParent());
        Assert.Null(injected.NoAdded.GetParent());


    }

}