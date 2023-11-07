using System;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Core.Nodes; 

public static partial class NodeExtensions {
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

    public static void RemoveFromParent(this Node node) {
        node.GetParent()?.RemoveChild(node);
    }

    public static void RemoveFromParentDeferred(this Node node) {
        node.GetParent()?.RemoveChildDeferred(node);
    }

    /// <summary>
    /// Find the first node with T type in the children of the node parent.
    /// If not found, go up to next parent and repeat until found or until the are no more parents.
    /// </summary>
    /// <param name="node"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? FirstChildInParentOrNull<T>(this Node node) where T : Node {
        var parent = node.GetParent();
        var found = parent?.GetChildren().OfType<T>().FirstOrDefault();
        while (found == null && parent != null) {
            parent = parent.GetParent();
            found = parent?.GetChildren().OfType<T>().FirstOrDefault();
        }
        return found;
    }

    public static T? FindParent<T>(this Node node) where T : Node {
        Node parent = node.GetParent();
        return parent switch {
            null => null,
            T parentT => parentT,
            _ => parent.FindParent<T>()
        };
    }

    /// <summary>
    /// Returns the first node among the children with the T type and the predicate, or returns null.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="predicate"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? FirstNode<T>(this Node parent, Func<T, bool>? predicate = null) where T : Node {
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

        
    public static Node MoveChildDeferred(this Node parent, Node child, int pos) {
        parent.CallDeferred(Node.MethodName.MoveChild, child, pos);
        return parent;
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