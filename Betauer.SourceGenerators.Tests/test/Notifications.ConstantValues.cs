using Godot;
using Betauer.Core.Nodes.Events;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.SourceGenerators.Tests; 

/**
 * Godot version: 4.2-stable (official)
 * Date: 2024-03-09 17:41:50
 */

[TestFixture]
public class NotificationTest {

    [Test]
    public void NotificationTests() {
        var instanceCanvasItem = new CanvasItemNotificationTest();
        var instanceContainer = new ContainerNotificationTest();
        var instanceControl = new ControlNotificationTest();
        var instanceNode3D = new Node3DNotificationTest();
        var instanceSkeleton3D = new Skeleton3DNotificationTest();
        var instanceWindow = new WindowNotificationTest();
    }
}

[Notifications(Process = false, PhysicsProcess = false)]
public partial class CanvasItemNotificationTest : Node2D /* CanvasItem can not be inherit from */ {
    public override partial void _Notification(int what);
    public override void _Ready() {
        OnTransformChanged += () => { };
        OnLocalTransformChanged += () => { };
        OnDraw += () => { };
        OnVisibilityChanged += () => { };
        OnEnterCanvas += () => { };
        OnExitCanvas += () => { };
        OnWorld2DChanged += () => { };
    }
}

[Notifications(Process = false, PhysicsProcess = false)]
public partial class ContainerNotificationTest : Container {
    public override partial void _Notification(int what);
    public override void _Ready() {
        OnPreSortChildren += () => { };
        OnSortChildren += () => { };
    }
}

[Notifications(Process = false, PhysicsProcess = false)]
public partial class ControlNotificationTest : Control {
    public override partial void _Notification(int what);
    public override void _Ready() {
        OnResized += () => { };
        OnMouseEnter += () => { };
        OnMouseExit += () => { };
        OnMouseEnterSelf += () => { };
        OnMouseExitSelf += () => { };
        OnFocusEnter += () => { };
        OnFocusExit += () => { };
        OnThemeChanged += () => { };
        OnScrollBegin += () => { };
        OnScrollEnd += () => { };
        OnLayoutDirectionChanged += () => { };
    }
}

[Notifications(Process = false, PhysicsProcess = false)]
public partial class Node3DNotificationTest : Node3D {
    public override partial void _Notification(int what);
    public override void _Ready() {
        OnTransformChanged += () => { };
        OnEnterWorld += () => { };
        OnExitWorld += () => { };
        OnVisibilityChanged += () => { };
        OnLocalTransformChanged += () => { };
    }
}

[Notifications(Process = false, PhysicsProcess = false)]
public partial class Skeleton3DNotificationTest : Skeleton3D {
    public override partial void _Notification(int what);
    public override void _Ready() {
        OnUpdateSkeleton += () => { };
    }
}

[Notifications(Process = false, PhysicsProcess = false)]
public partial class WindowNotificationTest : Window {
    public override partial void _Notification(int what);
    public override void _Ready() {
        OnVisibilityChanged += () => { };
        OnThemeChanged += () => { };
    }
}
