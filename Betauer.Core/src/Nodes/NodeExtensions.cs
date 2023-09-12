using System;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Core.Nodes; 

public static partial class NodeExtensions {

    public static T GetChildOrCreate<T>(this Node node, string path, Func<T> create) where T : Node {
        if (path.Contains('.') || path.Contains('/')) throw new Exception("No paths are allowed, only node names");
        T child = node.GetNodeOrNull<T>(path);
        if (child == null) {
            child = create();
            node.AddChild(child);
        }
        return child;
    }

    public static void RemoveFromParent(this Node node) {
        node.GetParent()?.RemoveChild(node);
    }

    public static void DisableAllShapes(this CollisionObject2D parent) {
        parent.EnableAllShapes(false);
    }

    public static void EnableAllShapes(this CollisionObject2D parent, bool enable = true) {
        parent.GetChildren().ForEach(node => {
            if (node is CollisionPolygon2D collisionPolygon2D) collisionPolygon2D.Disabled = !enable;
            else if (node is CollisionShape2D collisionShape2D) collisionShape2D.Disabled = !enable; 
            else if (node is CollisionPolygon3D collisionPolygon3D) collisionPolygon3D.Disabled = !enable;
            else if (node is CollisionShape3D collisionShape3D) collisionShape3D.Disabled = !enable; 
        });
    }

    public static T FirstNode<T>(this Node parent, Func<T, bool>? predicate = null) where T : Node {
        return predicate != null ? 
            parent.GetChildren().OfType<T>().First(predicate) : 
            parent.GetChildren().OfType<T>().First();
    }

    public static T? FirstNodeOrNull<T>(this Node parent, Func<T, bool>? predicate = null) where T : Node {
        return predicate != null ? 
            parent.GetChildren().OfType<T>().FirstOrDefault(predicate) : 
            parent.GetChildren().OfType<T>().FirstOrDefault();
    }

    public static Node AddChild(this Node parent, Node child, Action? onReady) {
        if (onReady != null) {
            child.RequestReady();
            child.OnReady(onReady, true);
        }
        parent.AddChild(child);
        return parent;
    }

    public static Node AddChildDeferred(this Node parent, Node child, Action? onReady = null) {
        if (onReady != null) {
            child.RequestReady();
            child.OnReady(onReady, true);
        }
        parent.CallDeferred(Node.MethodName.AddChild, child);
        return parent;
    }

    public static Task AddChildReady(this Node parent, Node child) {
        TaskCompletionSource promise = new();
        child.RequestReady();
        child.OnReady(promise.SetResult, true);
        parent.AddChild(child);
        return promise.Task;
    }

    public static Task AddChildReadyDeferred(this Node parent, Node child) {
        TaskCompletionSource promise = new();
        child.RequestReady();
        child.OnReady(promise.SetResult, true);
        parent.CallDeferred(Node.MethodName.AddChild, child);
        return promise.Task;
    }

        
    public static Node RemoveChildDeferred(this Node parent, Node child) {
        parent.CallDeferred(Node.MethodName.RemoveChild, child);
        return parent;
    }

    public static void DisableAllNotifications(this Node node) {
        SetAllNotifications(node, false);
    }

    public static void EnableAllNotifications(this Node node) {
        SetAllNotifications(node, true);
    }

    public static void SetAllNotifications(this Node node, bool enable) {
        node.SetProcess(enable);
        node.SetProcessInput(enable);
        node.SetProcessUnhandledInput(enable);
        node.SetProcessUnhandledKeyInput(enable);
        node.SetPhysicsProcess(enable);
    }

    public static void FocusEnable(this BaseButton control) {
        SetFocusDisabled(control, false);
    }

    public static void FocusDisable(this BaseButton control) {
        SetFocusDisabled(control, true);
    }

    public static void SetFocusDisabled(this BaseButton control, bool isDisabled) {
        control.FocusMode = isDisabled ? Control.FocusModeEnum.None : Control.FocusModeEnum.All;
        control.Disabled = isDisabled;
    }

    public static async Task<Godot.Image> Screenshot(this Node node, Vector2I size) {
        var parent = node.GetParent();
        parent.RemoveChild(node);

        var subViewportContainer = new SubViewportContainer() {
            Name = "TemporalSubViewportContainer",
        };
        subViewportContainer.Position = new Vector2(10000, 10000);
        var viewport = new SubViewport {
            Name = "SubViewport",
            RenderTargetUpdateMode = SubViewport.UpdateMode.Once,
            HandleInputLocally = false,
            Size = size,
        };
        subViewportContainer.AddChild(viewport);
        parent.AddChild(subViewportContainer);
        viewport.AddChild(node);
        await node.AwaitProcessFrame();
        var image = viewport.GetTexture().GetImage();
        viewport.RemoveChild(node);
        parent.AddChild(node);
        subViewportContainer.Free();
        return image;
    }
}