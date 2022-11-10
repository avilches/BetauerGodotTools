using System;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Core.Signal;
using Betauer.TestRunner;
using Betauer.Tools.Logging;
using Godot;
using NUnit.Framework;
using Object = Godot.Object;

namespace Betauer.Core.Tests; 

[TestFixture]
public partial class SignalExtensionsTests : Node {
    [SetUp]
    public void Setup() {
        foreach (var child in GetChildren().OfType<Node>()) child.Free();
    }

    [Test(Description = "Signal connect and disconnect")]
    public async Task ConnectAndDisconnectLambdaTests() {
        var b1 = new CheckButton();
        AddChild(b1);
        await this.AwaitIdleFrame();
        var executed1 = 0;

        SignalHandler p1 = b1.OnPressed(() => executed1 ++);
        Assert.That(p1.IsConnected(), Is.True);

        b1.EmitSignal("pressed");
        Assert.That(executed1, Is.EqualTo(1));
            
        p1.Disconnect();
        p1.Disconnect(); // Idempotent
        Assert.That(p1.IsConnected(), Is.False);

        b1.EmitSignal("pressed");
        Assert.That(executed1, Is.EqualTo(1));

        p1.Connect();
        p1.Connect(); // Idempotent
        Assert.That(p1.IsConnected(), Is.True);

        b1.EmitSignal("pressed");
        b1.EmitSignal("pressed");
        Assert.That(executed1, Is.EqualTo(3));
            
        b1.Free();
        Assert.That(p1.IsConnected(), Is.False);
    }

    [Test(Description = "Signal oneShot should only work once then it is auto freed")]
    public async Task OneShotTest() {
        var b1 = new CheckButton();
        AddChild(b1);
        await this.AwaitIdleFrame();
        var executed1 = 0;

        SignalHandler p1 = b1.OnPressed(() => executed1 ++, true);

        b1.EmitSignal("pressed");
        b1.EmitSignal("pressed");
        b1.EmitSignal("pressed");
        Assert.That(executed1, Is.EqualTo(1));
        Assert.That(p1.IsConnected(), Is.False);
            
    }

    [Test(Description = "Signal deferred should work next frame")]
    public async Task SignalToLambdaDeferredTest() {
        var b1 = new CheckButton();
        AddChild(b1);
        await this.AwaitIdleFrame();
        var executed1 = 0;

        SignalHandler p1 = b1.OnPressed(() => executed1 ++, false, true);

        b1.EmitSignal("pressed");
        b1.EmitSignal("pressed");
        b1.EmitSignal("pressed");
        Assert.That(executed1, Is.EqualTo(0));
            
        await this.AwaitIdleFrame();
        Assert.That(executed1, Is.EqualTo(3));
    }

    [Test(Description = "Signal deferred and one shot should work next frame and no more")]
    public async Task SignalToLambdaOneShotDeferredTest() {
        var b1 = new CheckButton();
        AddChild(b1);
        await this.AwaitIdleFrame();
        var executed1 = 0;

        SignalHandler p1 = b1.OnPressed(() => executed1 ++, true, true);
        Assert.That(p1.IsConnected(), Is.True);

        b1.EmitSignal("pressed");
        Assert.That(executed1, Is.EqualTo(0));
        Assert.That(p1.IsConnected(), Is.False);
            
        await this.AwaitIdleFrame();
        Assert.That(executed1, Is.EqualTo(1));
    }

    [Test(Description = "Test signal parameters")]
    public async Task SignalParameterTest() {
        var b1 = new CheckButton();
        AddChild(b1);
        await this.AwaitIdleFrame();
            
        b1.AddUserSignal("1p");
        b1.AddUserSignal("2p");
        b1.AddUserSignal("3p");
        b1.AddUserSignal("4p");
        b1.AddUserSignal("5p");
        b1.AddUserSignal("6p");
        b1.On("1p", (int p1) => {
            Assert.That(p1, Is.EqualTo(1));
        });
            
        b1.On("2p", (int p1, int p2) => {
            Assert.That(p1, Is.EqualTo(1));
            Assert.That(p2, Is.EqualTo(2));
        });
            
        b1.On("3p", (int p1, int p2, int p3) => {
            Assert.That(p1, Is.EqualTo(1));
            Assert.That(p2, Is.EqualTo(2));
            Assert.That(p3, Is.EqualTo(3));
        });
            
        b1.On("4p", (int p1, int p2, int p3, int p4) => {
            Assert.That(p1, Is.EqualTo(1));
            Assert.That(p2, Is.EqualTo(2));
            Assert.That(p3, Is.EqualTo(3));
            Assert.That(p4, Is.EqualTo(4));
        });
            
        b1.On("5p", (int p1, int p2, int p3, int p4, int p5) => {
            Assert.That(p1, Is.EqualTo(1));
            Assert.That(p2, Is.EqualTo(2));
            Assert.That(p3, Is.EqualTo(3));
            Assert.That(p4, Is.EqualTo(4));
            Assert.That(p5, Is.EqualTo(5));
        });
            
        b1.On("6p", (int p1, int p2, int p3, int p4, int p5, int p6) => {
            Assert.That(p1, Is.EqualTo(1));
            Assert.That(p2, Is.EqualTo(2));
            Assert.That(p3, Is.EqualTo(3));
            Assert.That(p4, Is.EqualTo(4));
            Assert.That(p5, Is.EqualTo(5));
            Assert.That(p6, Is.EqualTo(6));
        });

        // Assert.Throws<AssertionException>(() => b1.EmitSignal("1p", 999));
        // Assert.Throws<AssertionException>(() => b1.EmitSignal("2p", 999, 2));
        // Assert.Throws<AssertionException>(() => b1.EmitSignal("3p", 999, 2, 3));
        // Assert.Throws<AssertionException>(() => b1.EmitSignal("4p", 999, 2, 3, 4));
        // Assert.Throws<AssertionException>(() => b1.EmitSignal("5p", 999, 2, 3, 4, 5));
        // Assert.Throws<AssertionException>(() => b1.EmitSignal("6p", 999, 2, 3, 4, 5, 6));

        b1.EmitSignal("1p", 1);
        b1.EmitSignal("2p", 1, 2);
        b1.EmitSignal("3p", 1, 2, 3);
        b1.EmitSignal("4p", 1, 2, 3, 4);
        b1.EmitSignal("5p", 1, 2, 3, 4, 5);
        b1.EmitSignal("6p", 1, 2, 3, 4, 5, 6);
    }

}