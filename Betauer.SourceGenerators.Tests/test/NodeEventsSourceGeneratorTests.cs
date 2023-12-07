using System;
using System.Threading.Tasks;
using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Events;
using Betauer.Core.Signal;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.SourceGenerators.Tests;

[Notifications(Process = false, PhysicsProcess = false)]
public partial class NotificationClass : Node {
    public override partial void _Notification(int what);
}

public partial class Outer {
    [Process, PhysicsProcess]
    public partial class ProcessClass : Button {
        public override partial void _Process(double delta);
        public override partial void _PhysicsProcess(double delta);
    }
}

public partial class PloterOuter {
    public partial class MoreOuter {
        [Notifications]
        public partial class NotificationProcessClass : Button {
            public override partial void _Process(double delta);
            public override partial void _PhysicsProcess(double delta);

            public override partial void _Notification(int what);
        }
    }
}


[InputEvents]
public partial class InputClass : Node {
    public override partial void _Input(InputEvent inputEvent);
    public override partial void _UnhandledInput(InputEvent @event);
    public override partial void _UnhandledKeyInput(InputEvent inputEvent);
    public override partial void _ShortcutInput(InputEvent e);
}

[InputEvents]
[Notifications]
public partial class InputNotificationProcessClass : Node {
    public override partial void _Input(InputEvent inputEvent);
    public override partial void _UnhandledInput(InputEvent @event);
    public override partial void _UnhandledKeyInput(InputEvent inputEvent);
    public override partial void _ShortcutInput(InputEvent e);

    public override partial void _Process(double delta);
    public override partial void _PhysicsProcess(double delta);

    public override partial void _Notification(int what);
}

[TestRunner.Test]
[Only]
public partial class NodeEventsSourceGeneratorTests : Node {
    [TestRunner.Test]
    public async Task ProcessOnNotificationOnlyTest() {
        var nodes = new Node[] { new NotificationClass(), new PloterOuter.MoreOuter.NotificationProcessClass(), new Outer.ProcessClass(), new InputNotificationProcessClass() };

        foreach (var node in nodes) {
            var calls = 0;
            AddChild(node);

            await this.AwaitProcessFrame();
            Assert.That(calls, Is.EqualTo(0));

            ((IProcessEvent)node).OnProcess += (d) => calls++;

            await this.AwaitProcessFrame();
            Assert.That(calls, Is.EqualTo(1));

            node.SetProcess(false);
            await this.AwaitProcessFrame();
            await this.AwaitProcessFrame();
            Assert.That(calls, Is.EqualTo(1));

            node.SetProcess(true);
            await this.AwaitProcessFrame();
            Assert.That(calls, Is.EqualTo(2));
        }
    }

    [TestRunner.Test]
    public async Task PhysicsProcessOnNotificationOnlyTest() {
        var nodes = new Node[] { new NotificationClass(), new PloterOuter.MoreOuter.NotificationProcessClass(), new Outer.ProcessClass(), new InputNotificationProcessClass() };

        foreach (var node in nodes) {
            var calls = 0;
            AddChild(node);

            await this.AwaitPhysicsFrame();
            Assert.That(calls, Is.EqualTo(0));

            ((IPhysicsProcessEvent)node).OnPhysicsProcess += (d) => calls++;

            await this.AwaitPhysicsFrame();
            Assert.That(calls, Is.EqualTo(1));

            node.SetPhysicsProcess(false);
            await this.AwaitPhysicsFrame();
            await this.AwaitPhysicsFrame();
            Assert.That(calls, Is.EqualTo(1));

            node.SetPhysicsProcess(true);
            await this.AwaitPhysicsFrame();
            Assert.That(calls, Is.EqualTo(2));
        }
    }

    [TestRunner.Test]
    public async Task IsProcessingEnabledTest() {
        var nodes = new Node[] { new NotificationClass(), new PloterOuter.MoreOuter.NotificationProcessClass(), new Outer.ProcessClass() };

        foreach (var node in nodes) {
            AddChild(node);
            await this.AwaitProcessFrame();
            Assert.That(node.IsProcessing(), Is.EqualTo(false));

            Action<double> action = (d) => { };
            ((IProcessEvent)node).OnProcess += action;
            Assert.That(node.IsProcessing(), Is.EqualTo(true));

            ((IProcessEvent)node).OnProcess -= action;
            Assert.That(node.IsProcessing(), Is.EqualTo(false));
        }
    }

    [TestRunner.Test]
    public async Task IsPhysicsProcessingEnabledTest() {
        var nodes = new Node[] { new NotificationClass(), new PloterOuter.MoreOuter.NotificationProcessClass(), new Outer.ProcessClass() };

        foreach (var node in nodes) {
            AddChild(node);
            await this.AwaitPhysicsFrame();
            Assert.That(node.IsPhysicsProcessing(), Is.EqualTo(false));

            Action<double> action = (d) => { };
            ((IPhysicsProcessEvent)node).OnPhysicsProcess += action;
            Assert.That(node.IsPhysicsProcessing(), Is.EqualTo(true));

            ((IPhysicsProcessEvent)node).OnPhysicsProcess -= action;
            Assert.That(node.IsPhysicsProcessing(), Is.EqualTo(false));
        }
    }

    [TestRunner.Test]
    public async Task NotificationTest() {
        var node = new NotificationClass();
        var parentedCalls = 0;

        node.OnParented += () => parentedCalls++;
        Assert.That(parentedCalls, Is.EqualTo(0));

        await this.AwaitPhysicsFrame();

        AddChild(node);
        await this.AwaitPhysicsFrame();

        Assert.That(parentedCalls, Is.EqualTo(1));

        node.RemoveFromParent();
        await this.AwaitPhysicsFrame();
        AddChild(node);

        Assert.That(parentedCalls, Is.EqualTo(2));
    }

    [TestRunner.Test]
    [TestRunner.Ignore("ParseInputEvent is not working in Godot 4.1")]
    public async Task InputNotificationProcessClass() {
        var node = new InputNotificationProcessClass();
        AddChild(node);
        var parentedCalls = 0;
        var inputEventKey = new InputEventKey() {
            Keycode = Key.A,
            Pressed = true,
            Echo = false,
        };

        node.OnInput += (e) => {
            Assert.That(e, Is.EqualTo(inputEventKey));
            parentedCalls++;
        };

        node.OnUnhandledInput += (e) => {
            Assert.That(e, Is.EqualTo(inputEventKey));
            parentedCalls++;
        };

        await this.AwaitPhysicsFrame();
        Input.ParseInputEvent(inputEventKey);
        await this.AwaitPhysicsFrame();

        Assert.That(parentedCalls, Is.EqualTo(1));
    }
}




