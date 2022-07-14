using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;
using Environment = Godot.Environment;

namespace Betauer.Signal {
    public static partial class SignalExtensions {
        public static Object EnsureObject(object o) =>
            !(o is Object obj) ? throw new Exception("Only Godot.Object instances are allowed") : obj;

        public static SignalHandler Connect<T>(Object origin, string signal, Action<T> method, bool oneShot, bool deferred) =>
            new SignalHandler(origin, signal, EnsureObject(method.Target), method.Method.Name, oneShot, deferred);

        public static SignalHandler Connect(Object origin, string signal, Action method, bool oneShot, bool deferred) =>
            new SignalHandler(origin, signal, EnsureObject(method.Target), method.Method.Name, oneShot, deferred);

        public static SignalHandler ConnectConfirmed(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "confirmed", action, oneShot, deferred);

        public static SignalHandlerAction OnConfirmed(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "confirmed", action, oneShot, deferred);

        public static SignalHandler ConnectCustomAction(this AcceptDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "custom_action", action, oneShot, deferred);

        public static SignalHandlerAction<string> OnCustomAction(this AcceptDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string>(target, "custom_action", action, oneShot, deferred);

        public static SignalHandler ConnectAnimationFinished(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandlerAction OnAnimationFinished(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandler ConnectFrameChanged(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnFrameChanged(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler ConnectAnimationFinished(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandlerAction OnAnimationFinished(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandler ConnectFrameChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnFrameChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler ConnectTracksChanged(this Animation target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tracks_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnTracksChanged(this Animation target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "tracks_changed", action, oneShot, deferred);

        public static SignalHandler ConnectRemovedFromGraph(this AnimationNode target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "removed_from_graph", action, oneShot, deferred);

        public static SignalHandlerAction OnRemovedFromGraph(this AnimationNode target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "removed_from_graph", action, oneShot, deferred);

        public static SignalHandler ConnectTreeChanged(this AnimationNode target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tree_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnTreeChanged(this AnimationNode target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "tree_changed", action, oneShot, deferred);

        public static SignalHandler ConnectTrianglesUpdated(this AnimationNodeBlendSpace2D target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "triangles_updated", action, oneShot, deferred);

        public static SignalHandlerAction OnTrianglesUpdated(this AnimationNodeBlendSpace2D target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "triangles_updated", action, oneShot, deferred);

        public static SignalHandler ConnectAdvanceConditionChanged(this AnimationNodeStateMachineTransition target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "advance_condition_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnAdvanceConditionChanged(this AnimationNodeStateMachineTransition target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "advance_condition_changed", action, oneShot, deferred);

        public static SignalHandler ConnectAnimationChanged(this AnimationPlayer target, Action<string, string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "animation_changed", action, oneShot, deferred);

        public static SignalHandlerAction<string, string> OnAnimationChanged(this AnimationPlayer target, Action<string, string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string, string>(target, "animation_changed", action, oneShot, deferred);

        public static SignalHandler ConnectAnimationFinished(this AnimationPlayer target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandlerAction<string> OnAnimationFinished(this AnimationPlayer target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string>(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandler ConnectAnimationStarted(this AnimationPlayer target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "animation_started", action, oneShot, deferred);

        public static SignalHandlerAction<string> OnAnimationStarted(this AnimationPlayer target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string>(target, "animation_started", action, oneShot, deferred);

        public static SignalHandler ConnectCachesCleared(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "caches_cleared", action, oneShot, deferred);

        public static SignalHandlerAction OnCachesCleared(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "caches_cleared", action, oneShot, deferred);

        public static SignalHandler ConnectAreaEntered(this Area target, Action<Area> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "area_entered", action, oneShot, deferred);

        public static SignalHandlerAction<Area> OnAreaEntered(this Area target, Action<Area> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Area>(target, "area_entered", action, oneShot, deferred);

        public static SignalHandler ConnectAreaExited(this Area target, Action<Area> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "area_exited", action, oneShot, deferred);

        public static SignalHandlerAction<Area> OnAreaExited(this Area target, Action<Area> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Area>(target, "area_exited", action, oneShot, deferred);

        public static SignalHandler ConnectAreaShapeEntered(this Area target, Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "area_shape_entered", action, oneShot, deferred);

        public static SignalHandlerAction<Area, RID, int, int> OnAreaShapeEntered(this Area target, Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Area, RID, int, int>(target, "area_shape_entered", action, oneShot, deferred);

        public static SignalHandler ConnectAreaShapeExited(this Area target, Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "area_shape_exited", action, oneShot, deferred);

        public static SignalHandlerAction<Area, RID, int, int> OnAreaShapeExited(this Area target, Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Area, RID, int, int>(target, "area_shape_exited", action, oneShot, deferred);

        public static SignalHandler ConnectBodyEntered(this Area target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_entered", action, oneShot, deferred);

        public static SignalHandlerAction<Node> OnBodyEntered(this Area target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node>(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler ConnectBodyExited(this Area target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_exited", action, oneShot, deferred);

        public static SignalHandlerAction<Node> OnBodyExited(this Area target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node>(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler ConnectBodyShapeEntered(this Area target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandlerAction<Node, RID, int, int> OnBodyShapeEntered(this Area target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node, RID, int, int>(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler ConnectBodyShapeExited(this Area target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandlerAction<Node, RID, int, int> OnBodyShapeExited(this Area target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node, RID, int, int>(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler ConnectAreaEntered(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "area_entered", action, oneShot, deferred);

        public static SignalHandlerAction<Area2D> OnAreaEntered(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Area2D>(target, "area_entered", action, oneShot, deferred);

        public static SignalHandler ConnectAreaExited(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "area_exited", action, oneShot, deferred);

        public static SignalHandlerAction<Area2D> OnAreaExited(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Area2D>(target, "area_exited", action, oneShot, deferred);

        public static SignalHandler ConnectAreaShapeEntered(this Area2D target, Action<Area2D, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "area_shape_entered", action, oneShot, deferred);

        public static SignalHandlerAction<Area2D, RID, int, int> OnAreaShapeEntered(this Area2D target, Action<Area2D, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Area2D, RID, int, int>(target, "area_shape_entered", action, oneShot, deferred);

        public static SignalHandler ConnectAreaShapeExited(this Area2D target, Action<Area2D, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "area_shape_exited", action, oneShot, deferred);

        public static SignalHandlerAction<Area2D, RID, int, int> OnAreaShapeExited(this Area2D target, Action<Area2D, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Area2D, RID, int, int>(target, "area_shape_exited", action, oneShot, deferred);

        public static SignalHandler ConnectBodyEntered(this Area2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_entered", action, oneShot, deferred);

        public static SignalHandlerAction<Node> OnBodyEntered(this Area2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node>(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler ConnectBodyExited(this Area2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_exited", action, oneShot, deferred);

        public static SignalHandlerAction<Node> OnBodyExited(this Area2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node>(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler ConnectBodyShapeEntered(this Area2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandlerAction<Node, RID, int, int> OnBodyShapeEntered(this Area2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node, RID, int, int>(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler ConnectBodyShapeExited(this Area2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandlerAction<Node, RID, int, int> OnBodyShapeExited(this Area2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node, RID, int, int>(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler ConnectMeshUpdated(this ARVRAnchor target, Action<Mesh> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "mesh_updated", action, oneShot, deferred);

        public static SignalHandlerAction<Mesh> OnMeshUpdated(this ARVRAnchor target, Action<Mesh> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Mesh>(target, "mesh_updated", action, oneShot, deferred);

        public static SignalHandler ConnectButtonPressed(this ARVRController target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "button_pressed", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnButtonPressed(this ARVRController target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "button_pressed", action, oneShot, deferred);

        public static SignalHandler ConnectButtonRelease(this ARVRController target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "button_release", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnButtonRelease(this ARVRController target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "button_release", action, oneShot, deferred);

        public static SignalHandler ConnectMeshUpdated(this ARVRController target, Action<Mesh> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "mesh_updated", action, oneShot, deferred);

        public static SignalHandlerAction<Mesh> OnMeshUpdated(this ARVRController target, Action<Mesh> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Mesh>(target, "mesh_updated", action, oneShot, deferred);

        public static SignalHandler ConnectARVRServerInterfaceAdded(Action<string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "interface_added", action, oneShot, deferred);

        public static SignalHandlerAction<string> OnARVRServerInterfaceAdded(Action<string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string>(ARVRServer.Singleton, "interface_added", action, oneShot, deferred);

        public static SignalHandler ConnectARVRServerInterfaceRemoved(Action<string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "interface_removed", action, oneShot, deferred);

        public static SignalHandlerAction<string> OnARVRServerInterfaceRemoved(Action<string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string>(ARVRServer.Singleton, "interface_removed", action, oneShot, deferred);

        public static SignalHandler ConnectARVRServerTrackerAdded(Action<int, string, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tracker_added", action, oneShot, deferred);

        public static SignalHandlerAction<int, string, int> OnARVRServerTrackerAdded(Action<int, string, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int, string, int>(ARVRServer.Singleton, "tracker_added", action, oneShot, deferred);

        public static SignalHandler ConnectARVRServerTrackerRemoved(Action<int, string, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tracker_removed", action, oneShot, deferred);

        public static SignalHandlerAction<int, string, int> OnARVRServerTrackerRemoved(Action<int, string, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int, string, int>(ARVRServer.Singleton, "tracker_removed", action, oneShot, deferred);

        public static SignalHandler ConnectAudioServerBusLayoutChanged(Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "bus_layout_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnAudioServerBusLayoutChanged(Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(AudioServer.Singleton, "bus_layout_changed", action, oneShot, deferred);

        public static SignalHandler ConnectFinished(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "finished", action, oneShot, deferred);

        public static SignalHandlerAction OnFinished(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "finished", action, oneShot, deferred);

        public static SignalHandler ConnectFinished(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "finished", action, oneShot, deferred);

        public static SignalHandlerAction OnFinished(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "finished", action, oneShot, deferred);

        public static SignalHandler ConnectFinished(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "finished", action, oneShot, deferred);

        public static SignalHandlerAction OnFinished(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "finished", action, oneShot, deferred);

        public static SignalHandler ConnectButtonDown(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "button_down", action, oneShot, deferred);

        public static SignalHandlerAction OnButtonDown(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "button_down", action, oneShot, deferred);

        public static SignalHandler ConnectButtonUp(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "button_up", action, oneShot, deferred);

        public static SignalHandlerAction OnButtonUp(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "button_up", action, oneShot, deferred);

        public static SignalHandler ConnectPressed(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "pressed", action, oneShot, deferred);

        public static SignalHandlerAction OnPressed(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "pressed", action, oneShot, deferred);

        public static SignalHandler ConnectToggled(this BaseButton target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "toggled", action, oneShot, deferred);

        public static SignalHandlerAction<bool> OnToggled(this BaseButton target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<bool>(target, "toggled", action, oneShot, deferred);

        public static SignalHandler ConnectPressed(this ButtonGroup target, Action<Object> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "pressed", action, oneShot, deferred);

        public static SignalHandlerAction<Object> OnPressed(this ButtonGroup target, Action<Object> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Object>(target, "pressed", action, oneShot, deferred);

        public static SignalHandler ConnectCameraServerCameraFeedAdded(Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "camera_feed_added", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnCameraServerCameraFeedAdded(Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(CameraServer.Singleton, "camera_feed_added", action, oneShot, deferred);

        public static SignalHandler ConnectCameraServerCameraFeedRemoved(Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "camera_feed_removed", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnCameraServerCameraFeedRemoved(Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(CameraServer.Singleton, "camera_feed_removed", action, oneShot, deferred);

        public static SignalHandler ConnectDraw(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "draw", action, oneShot, deferred);

        public static SignalHandlerAction OnDraw(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "draw", action, oneShot, deferred);

        public static SignalHandler ConnectHide(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "hide", action, oneShot, deferred);

        public static SignalHandlerAction OnHide(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "hide", action, oneShot, deferred);

        public static SignalHandler ConnectItemRectChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "item_rect_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnItemRectChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "item_rect_changed", action, oneShot, deferred);

        public static SignalHandler ConnectVisibilityChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "visibility_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnVisibilityChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "visibility_changed", action, oneShot, deferred);

        public static SignalHandler ConnectInputEvent(this CollisionObject target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "input_event", action, oneShot, deferred);

        public static SignalHandlerAction<InputEvent, Node, Vector3, Vector3, int> OnInputEvent(this CollisionObject target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<InputEvent, Node, Vector3, Vector3, int>(target, "input_event", action, oneShot, deferred);

        public static SignalHandler ConnectMouseEntered(this CollisionObject target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandlerAction OnMouseEntered(this CollisionObject target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandler ConnectMouseExited(this CollisionObject target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandlerAction OnMouseExited(this CollisionObject target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandler ConnectInputEvent(this CollisionObject2D target, Action<InputEvent, int, Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "input_event", action, oneShot, deferred);

        public static SignalHandlerAction<InputEvent, int, Node> OnInputEvent(this CollisionObject2D target, Action<InputEvent, int, Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<InputEvent, int, Node>(target, "input_event", action, oneShot, deferred);

        public static SignalHandler ConnectMouseEntered(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandlerAction OnMouseEntered(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandler ConnectMouseExited(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandlerAction OnMouseExited(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandler ConnectColorChanged(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "color_changed", action, oneShot, deferred);

        public static SignalHandlerAction<Color> OnColorChanged(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Color>(target, "color_changed", action, oneShot, deferred);

        public static SignalHandler ConnectPresetAdded(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "preset_added", action, oneShot, deferred);

        public static SignalHandlerAction<Color> OnPresetAdded(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Color>(target, "preset_added", action, oneShot, deferred);

        public static SignalHandler ConnectPresetRemoved(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "preset_removed", action, oneShot, deferred);

        public static SignalHandlerAction<Color> OnPresetRemoved(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Color>(target, "preset_removed", action, oneShot, deferred);

        public static SignalHandler ConnectColorChanged(this ColorPickerButton target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "color_changed", action, oneShot, deferred);

        public static SignalHandlerAction<Color> OnColorChanged(this ColorPickerButton target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Color>(target, "color_changed", action, oneShot, deferred);

        public static SignalHandler ConnectPickerCreated(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "picker_created", action, oneShot, deferred);

        public static SignalHandlerAction OnPickerCreated(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "picker_created", action, oneShot, deferred);

        public static SignalHandler ConnectPopupClosed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "popup_closed", action, oneShot, deferred);

        public static SignalHandlerAction OnPopupClosed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "popup_closed", action, oneShot, deferred);

        public static SignalHandler ConnectSortChildren(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "sort_children", action, oneShot, deferred);

        public static SignalHandlerAction OnSortChildren(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "sort_children", action, oneShot, deferred);

        public static SignalHandler ConnectFocusEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "focus_entered", action, oneShot, deferred);

        public static SignalHandlerAction OnFocusEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "focus_entered", action, oneShot, deferred);

        public static SignalHandler ConnectFocusExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "focus_exited", action, oneShot, deferred);

        public static SignalHandlerAction OnFocusExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "focus_exited", action, oneShot, deferred);

        public static SignalHandler ConnectGuiInput(this Control target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "gui_input", action, oneShot, deferred);

        public static SignalHandlerAction<InputEvent> OnGuiInput(this Control target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<InputEvent>(target, "gui_input", action, oneShot, deferred);

        public static SignalHandler ConnectMinimumSizeChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "minimum_size_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnMinimumSizeChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "minimum_size_changed", action, oneShot, deferred);

        public static SignalHandler ConnectModalClosed(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "modal_closed", action, oneShot, deferred);

        public static SignalHandlerAction OnModalClosed(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "modal_closed", action, oneShot, deferred);

        public static SignalHandler ConnectMouseEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandlerAction OnMouseEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandler ConnectMouseExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandlerAction OnMouseExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandler ConnectResized(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "resized", action, oneShot, deferred);

        public static SignalHandlerAction OnResized(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "resized", action, oneShot, deferred);

        public static SignalHandler ConnectSizeFlagsChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "size_flags_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnSizeFlagsChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "size_flags_changed", action, oneShot, deferred);

        public static SignalHandler ConnectRangeChanged(this Curve target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "range_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnRangeChanged(this Curve target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "range_changed", action, oneShot, deferred);

        public static SignalHandler ConnectDirSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "dir_selected", action, oneShot, deferred);

        public static SignalHandlerAction<string> OnDirSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string>(target, "dir_selected", action, oneShot, deferred);

        public static SignalHandler ConnectFileSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "file_selected", action, oneShot, deferred);

        public static SignalHandlerAction<string> OnFileSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string>(target, "file_selected", action, oneShot, deferred);

        public static SignalHandler ConnectFilesSelected(this FileDialog target, Action<string[]> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "files_selected", action, oneShot, deferred);

        public static SignalHandlerAction<string[]> OnFilesSelected(this FileDialog target, Action<string[]> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string[]>(target, "files_selected", action, oneShot, deferred);

        public static SignalHandler ConnectCompleted(this GDScriptFunctionState target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "completed", action, oneShot, deferred);

        public static SignalHandlerAction<object> OnCompleted(this GDScriptFunctionState target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<object>(target, "completed", action, oneShot, deferred);

        public static SignalHandler ConnectBeginNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "_begin_node_move", action, oneShot, deferred);

        public static SignalHandlerAction OnBeginNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "_begin_node_move", action, oneShot, deferred);

        public static SignalHandler ConnectConnectionFromEmpty(this GraphEdit target, Action<Vector2, string, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "connection_from_empty", action, oneShot, deferred);

        public static SignalHandlerAction<Vector2, string, int> OnConnectionFromEmpty(this GraphEdit target, Action<Vector2, string, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Vector2, string, int>(target, "connection_from_empty", action, oneShot, deferred);

        public static SignalHandler ConnectConnectionRequest(this GraphEdit target, Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "connection_request", action, oneShot, deferred);

        public static SignalHandlerAction<string, int, string, int> OnConnectionRequest(this GraphEdit target, Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string, int, string, int>(target, "connection_request", action, oneShot, deferred);

        public static SignalHandler ConnectConnectionToEmpty(this GraphEdit target, Action<string, int, Vector2> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "connection_to_empty", action, oneShot, deferred);

        public static SignalHandlerAction<string, int, Vector2> OnConnectionToEmpty(this GraphEdit target, Action<string, int, Vector2> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string, int, Vector2>(target, "connection_to_empty", action, oneShot, deferred);

        public static SignalHandler ConnectCopyNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "copy_nodes_request", action, oneShot, deferred);

        public static SignalHandlerAction OnCopyNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "copy_nodes_request", action, oneShot, deferred);

        public static SignalHandler ConnectDeleteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "delete_nodes_request", action, oneShot, deferred);

        public static SignalHandlerAction OnDeleteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "delete_nodes_request", action, oneShot, deferred);

        public static SignalHandler ConnectDisconnectionRequest(this GraphEdit target, Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "disconnection_request", action, oneShot, deferred);

        public static SignalHandlerAction<string, int, string, int> OnDisconnectionRequest(this GraphEdit target, Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string, int, string, int>(target, "disconnection_request", action, oneShot, deferred);

        public static SignalHandler ConnectDuplicateNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "duplicate_nodes_request", action, oneShot, deferred);

        public static SignalHandlerAction OnDuplicateNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "duplicate_nodes_request", action, oneShot, deferred);

        public static SignalHandler ConnectEndNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "_end_node_move", action, oneShot, deferred);

        public static SignalHandlerAction OnEndNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "_end_node_move", action, oneShot, deferred);

        public static SignalHandler ConnectNodeSelected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "node_selected", action, oneShot, deferred);

        public static SignalHandlerAction<Node> OnNodeSelected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node>(target, "node_selected", action, oneShot, deferred);

        public static SignalHandler ConnectNodeUnselected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "node_unselected", action, oneShot, deferred);

        public static SignalHandlerAction<Node> OnNodeUnselected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node>(target, "node_unselected", action, oneShot, deferred);

        public static SignalHandler ConnectPasteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "paste_nodes_request", action, oneShot, deferred);

        public static SignalHandlerAction OnPasteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "paste_nodes_request", action, oneShot, deferred);

        public static SignalHandler ConnectPopupRequest(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "popup_request", action, oneShot, deferred);

        public static SignalHandlerAction<Vector2> OnPopupRequest(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Vector2>(target, "popup_request", action, oneShot, deferred);

        public static SignalHandler ConnectScrollOffsetChanged(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "scroll_offset_changed", action, oneShot, deferred);

        public static SignalHandlerAction<Vector2> OnScrollOffsetChanged(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Vector2>(target, "scroll_offset_changed", action, oneShot, deferred);

        public static SignalHandler ConnectCloseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "close_request", action, oneShot, deferred);

        public static SignalHandlerAction OnCloseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "close_request", action, oneShot, deferred);

        public static SignalHandler ConnectDragged(this GraphNode target, Action<Vector2, Vector2> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "dragged", action, oneShot, deferred);

        public static SignalHandlerAction<Vector2, Vector2> OnDragged(this GraphNode target, Action<Vector2, Vector2> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Vector2, Vector2>(target, "dragged", action, oneShot, deferred);

        public static SignalHandler ConnectOffsetChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "offset_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnOffsetChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "offset_changed", action, oneShot, deferred);

        public static SignalHandler ConnectRaiseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "raise_request", action, oneShot, deferred);

        public static SignalHandlerAction OnRaiseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "raise_request", action, oneShot, deferred);

        public static SignalHandler ConnectResizeRequest(this GraphNode target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "resize_request", action, oneShot, deferred);

        public static SignalHandlerAction<Vector2> OnResizeRequest(this GraphNode target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Vector2>(target, "resize_request", action, oneShot, deferred);

        public static SignalHandler ConnectSlotUpdated(this GraphNode target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "slot_updated", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnSlotUpdated(this GraphNode target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "slot_updated", action, oneShot, deferred);

        public static SignalHandler ConnectCellSizeChanged(this GridMap target, Action<Vector3> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "cell_size_changed", action, oneShot, deferred);

        public static SignalHandlerAction<Vector3> OnCellSizeChanged(this GridMap target, Action<Vector3> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Vector3>(target, "cell_size_changed", action, oneShot, deferred);

        public static SignalHandler ConnectRequestCompleted(this HTTPRequest target, Action<byte[], string[], int, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "request_completed", action, oneShot, deferred);

        public static SignalHandlerAction<byte[], string[], int, int> OnRequestCompleted(this HTTPRequest target, Action<byte[], string[], int, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<byte[], string[], int, int>(target, "request_completed", action, oneShot, deferred);

        public static SignalHandler ConnectInputJoyConnectionChanged(Action<bool, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "joy_connection_changed", action, oneShot, deferred);

        public static SignalHandlerAction<bool, int> OnInputJoyConnectionChanged(Action<bool, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<bool, int>(Input.Singleton, "joy_connection_changed", action, oneShot, deferred);

        public static SignalHandler ConnectItemActivated(this ItemList target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "item_activated", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnItemActivated(this ItemList target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "item_activated", action, oneShot, deferred);

        public static SignalHandler ConnectItemRmbSelected(this ItemList target, Action<Vector2, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "item_rmb_selected", action, oneShot, deferred);

        public static SignalHandlerAction<Vector2, int> OnItemRmbSelected(this ItemList target, Action<Vector2, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Vector2, int>(target, "item_rmb_selected", action, oneShot, deferred);

        public static SignalHandler ConnectItemSelected(this ItemList target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "item_selected", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnItemSelected(this ItemList target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "item_selected", action, oneShot, deferred);

        public static SignalHandler ConnectMultiSelected(this ItemList target, Action<int, bool> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "multi_selected", action, oneShot, deferred);

        public static SignalHandlerAction<int, bool> OnMultiSelected(this ItemList target, Action<int, bool> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int, bool>(target, "multi_selected", action, oneShot, deferred);

        public static SignalHandler ConnectNothingSelected(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "nothing_selected", action, oneShot, deferred);

        public static SignalHandlerAction OnNothingSelected(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "nothing_selected", action, oneShot, deferred);

        public static SignalHandler ConnectRmbClicked(this ItemList target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "rmb_clicked", action, oneShot, deferred);

        public static SignalHandlerAction<Vector2> OnRmbClicked(this ItemList target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Vector2>(target, "rmb_clicked", action, oneShot, deferred);

        public static SignalHandler ConnectTextChanged(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "text_changed", action, oneShot, deferred);

        public static SignalHandlerAction<string> OnTextChanged(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string>(target, "text_changed", action, oneShot, deferred);

        public static SignalHandler ConnectTextChangeRejected(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "text_change_rejected", action, oneShot, deferred);

        public static SignalHandlerAction<string> OnTextChangeRejected(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string>(target, "text_change_rejected", action, oneShot, deferred);

        public static SignalHandler ConnectTextEntered(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "text_entered", action, oneShot, deferred);

        public static SignalHandlerAction<string> OnTextEntered(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string>(target, "text_entered", action, oneShot, deferred);

        public static SignalHandler ConnectRequestPermissionsResult(this MainLoop target, Action<bool, string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "on_request_permissions_result", action, oneShot, deferred);

        public static SignalHandlerAction<bool, string> OnRequestPermissionsResult(this MainLoop target, Action<bool, string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<bool, string>(target, "on_request_permissions_result", action, oneShot, deferred);

        public static SignalHandler ConnectAboutToShow(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "about_to_show", action, oneShot, deferred);

        public static SignalHandlerAction OnAboutToShow(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "about_to_show", action, oneShot, deferred);

        public static SignalHandler ConnectTextureChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnTextureChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler ConnectTextureChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnTextureChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler ConnectConnectedToServer(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "connected_to_server", action, oneShot, deferred);

        public static SignalHandlerAction OnConnectedToServer(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "connected_to_server", action, oneShot, deferred);

        public static SignalHandler ConnectConnectionFailed(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "connection_failed", action, oneShot, deferred);

        public static SignalHandlerAction OnConnectionFailed(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "connection_failed", action, oneShot, deferred);

        public static SignalHandler ConnectNetworkPeerConnected(this MultiplayerAPI target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "network_peer_connected", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnNetworkPeerConnected(this MultiplayerAPI target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "network_peer_connected", action, oneShot, deferred);

        public static SignalHandler ConnectNetworkPeerDisconnected(this MultiplayerAPI target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "network_peer_disconnected", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnNetworkPeerDisconnected(this MultiplayerAPI target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "network_peer_disconnected", action, oneShot, deferred);

        public static SignalHandler ConnectNetworkPeerPacket(this MultiplayerAPI target, Action<int, byte[]> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "network_peer_packet", action, oneShot, deferred);

        public static SignalHandlerAction<int, byte[]> OnNetworkPeerPacket(this MultiplayerAPI target, Action<int, byte[]> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int, byte[]>(target, "network_peer_packet", action, oneShot, deferred);

        public static SignalHandler ConnectServerDisconnected(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "server_disconnected", action, oneShot, deferred);

        public static SignalHandlerAction OnServerDisconnected(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "server_disconnected", action, oneShot, deferred);

        public static SignalHandler ConnectConnectionFailed(this NetworkedMultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "connection_failed", action, oneShot, deferred);

        public static SignalHandlerAction OnConnectionFailed(this NetworkedMultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "connection_failed", action, oneShot, deferred);

        public static SignalHandler ConnectConnectionSucceeded(this NetworkedMultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "connection_succeeded", action, oneShot, deferred);

        public static SignalHandlerAction OnConnectionSucceeded(this NetworkedMultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "connection_succeeded", action, oneShot, deferred);

        public static SignalHandler ConnectPeerConnected(this NetworkedMultiplayerPeer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "peer_connected", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnPeerConnected(this NetworkedMultiplayerPeer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "peer_connected", action, oneShot, deferred);

        public static SignalHandler ConnectPeerDisconnected(this NetworkedMultiplayerPeer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "peer_disconnected", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnPeerDisconnected(this NetworkedMultiplayerPeer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "peer_disconnected", action, oneShot, deferred);

        public static SignalHandler ConnectServerDisconnected(this NetworkedMultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "server_disconnected", action, oneShot, deferred);

        public static SignalHandlerAction OnServerDisconnected(this NetworkedMultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "server_disconnected", action, oneShot, deferred);

        public static SignalHandler ConnectTextureChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnTextureChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler ConnectReady(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "ready", action, oneShot, deferred);

        public static SignalHandlerAction OnReady(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "ready", action, oneShot, deferred);

        public static SignalHandler ConnectRenamed(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "renamed", action, oneShot, deferred);

        public static SignalHandlerAction OnRenamed(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "renamed", action, oneShot, deferred);

        public static SignalHandler ConnectTreeEntered(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tree_entered", action, oneShot, deferred);

        public static SignalHandlerAction OnTreeEntered(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "tree_entered", action, oneShot, deferred);

        public static SignalHandler ConnectTreeExited(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tree_exited", action, oneShot, deferred);

        public static SignalHandlerAction OnTreeExited(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "tree_exited", action, oneShot, deferred);

        public static SignalHandler ConnectTreeExiting(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tree_exiting", action, oneShot, deferred);

        public static SignalHandlerAction OnTreeExiting(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "tree_exiting", action, oneShot, deferred);

        public static SignalHandler ConnectScriptChanged(this Object target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "script_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnScriptChanged(this Object target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "script_changed", action, oneShot, deferred);

        public static SignalHandler ConnectItemFocused(this OptionButton target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "item_focused", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnItemFocused(this OptionButton target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "item_focused", action, oneShot, deferred);

        public static SignalHandler ConnectItemSelected(this OptionButton target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "item_selected", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnItemSelected(this OptionButton target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "item_selected", action, oneShot, deferred);

        public static SignalHandler ConnectCurveChanged(this Path target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "curve_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnCurveChanged(this Path target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "curve_changed", action, oneShot, deferred);

        public static SignalHandler ConnectAboutToShow(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "about_to_show", action, oneShot, deferred);

        public static SignalHandlerAction OnAboutToShow(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "about_to_show", action, oneShot, deferred);

        public static SignalHandler ConnectPopupHide(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "popup_hide", action, oneShot, deferred);

        public static SignalHandlerAction OnPopupHide(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "popup_hide", action, oneShot, deferred);

        public static SignalHandler ConnectIdFocused(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "id_focused", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnIdFocused(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "id_focused", action, oneShot, deferred);

        public static SignalHandler ConnectIdPressed(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "id_pressed", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnIdPressed(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "id_pressed", action, oneShot, deferred);

        public static SignalHandler ConnectIndexPressed(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "index_pressed", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnIndexPressed(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "index_pressed", action, oneShot, deferred);

        public static SignalHandler ConnectBroadcast(this ProximityGroup target, Action<string, Godot.Collections.Array> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "broadcast", action, oneShot, deferred);

        public static SignalHandlerAction<string, Godot.Collections.Array> OnBroadcast(this ProximityGroup target, Action<string, Godot.Collections.Array> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string, Godot.Collections.Array>(target, "broadcast", action, oneShot, deferred);

        public static SignalHandler ConnectChanged(this Range target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "changed", action, oneShot, deferred);

        public static SignalHandlerAction OnChanged(this Range target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "changed", action, oneShot, deferred);

        public static SignalHandler ConnectValueChanged(this Range target, Action<float> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "value_changed", action, oneShot, deferred);

        public static SignalHandlerAction<float> OnValueChanged(this Range target, Action<float> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<float>(target, "value_changed", action, oneShot, deferred);

        public static SignalHandler ConnectChanged(this Resource target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "changed", action, oneShot, deferred);

        public static SignalHandlerAction OnChanged(this Resource target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "changed", action, oneShot, deferred);

        public static SignalHandler ConnectMetaClicked(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "meta_clicked", action, oneShot, deferred);

        public static SignalHandlerAction<object> OnMetaClicked(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<object>(target, "meta_clicked", action, oneShot, deferred);

        public static SignalHandler ConnectMetaHoverEnded(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "meta_hover_ended", action, oneShot, deferred);

        public static SignalHandlerAction<object> OnMetaHoverEnded(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<object>(target, "meta_hover_ended", action, oneShot, deferred);

        public static SignalHandler ConnectMetaHoverStarted(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "meta_hover_started", action, oneShot, deferred);

        public static SignalHandlerAction<object> OnMetaHoverStarted(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<object>(target, "meta_hover_started", action, oneShot, deferred);

        public static SignalHandler ConnectBodyEntered(this RigidBody target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_entered", action, oneShot, deferred);

        public static SignalHandlerAction<Node> OnBodyEntered(this RigidBody target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node>(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler ConnectBodyExited(this RigidBody target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_exited", action, oneShot, deferred);

        public static SignalHandlerAction<Node> OnBodyExited(this RigidBody target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node>(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler ConnectBodyShapeEntered(this RigidBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandlerAction<Node, RID, int, int> OnBodyShapeEntered(this RigidBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node, RID, int, int>(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler ConnectBodyShapeExited(this RigidBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandlerAction<Node, RID, int, int> OnBodyShapeExited(this RigidBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node, RID, int, int>(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler ConnectSleepingStateChanged(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "sleeping_state_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnSleepingStateChanged(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "sleeping_state_changed", action, oneShot, deferred);

        public static SignalHandler ConnectBodyEntered(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_entered", action, oneShot, deferred);

        public static SignalHandlerAction<Node> OnBodyEntered(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node>(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler ConnectBodyExited(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_exited", action, oneShot, deferred);

        public static SignalHandlerAction<Node> OnBodyExited(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node>(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler ConnectBodyShapeEntered(this RigidBody2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandlerAction<Node, RID, int, int> OnBodyShapeEntered(this RigidBody2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node, RID, int, int>(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler ConnectBodyShapeExited(this RigidBody2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandlerAction<Node, RID, int, int> OnBodyShapeExited(this RigidBody2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node, RID, int, int>(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler ConnectSleepingStateChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "sleeping_state_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnSleepingStateChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "sleeping_state_changed", action, oneShot, deferred);

        public static SignalHandler ConnectConnectedToServer(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "connected_to_server", action, oneShot, deferred);

        public static SignalHandlerAction OnConnectedToServer(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "connected_to_server", action, oneShot, deferred);

        public static SignalHandler ConnectConnectionFailed(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "connection_failed", action, oneShot, deferred);

        public static SignalHandlerAction OnConnectionFailed(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "connection_failed", action, oneShot, deferred);

        public static SignalHandler ConnectFilesDropped(this SceneTree target, Action<string[], int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "files_dropped", action, oneShot, deferred);

        public static SignalHandlerAction<string[], int> OnFilesDropped(this SceneTree target, Action<string[], int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string[], int>(target, "files_dropped", action, oneShot, deferred);

        public static SignalHandler ConnectGlobalMenuAction(this SceneTree target, Action<object, object> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "global_menu_action", action, oneShot, deferred);

        public static SignalHandlerAction<object, object> OnGlobalMenuAction(this SceneTree target, Action<object, object> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<object, object>(target, "global_menu_action", action, oneShot, deferred);

        public static SignalHandler ConnectIdleFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "idle_frame", action, oneShot, deferred);

        public static SignalHandlerAction OnIdleFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "idle_frame", action, oneShot, deferred);

        public static SignalHandler ConnectNetworkPeerConnected(this SceneTree target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "network_peer_connected", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnNetworkPeerConnected(this SceneTree target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "network_peer_connected", action, oneShot, deferred);

        public static SignalHandler ConnectNetworkPeerDisconnected(this SceneTree target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "network_peer_disconnected", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnNetworkPeerDisconnected(this SceneTree target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "network_peer_disconnected", action, oneShot, deferred);

        public static SignalHandler ConnectNodeAdded(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "node_added", action, oneShot, deferred);

        public static SignalHandlerAction<Node> OnNodeAdded(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node>(target, "node_added", action, oneShot, deferred);

        public static SignalHandler ConnectNodeConfigurationWarningChanged(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "node_configuration_warning_changed", action, oneShot, deferred);

        public static SignalHandlerAction<Node> OnNodeConfigurationWarningChanged(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node>(target, "node_configuration_warning_changed", action, oneShot, deferred);

        public static SignalHandler ConnectNodeRemoved(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "node_removed", action, oneShot, deferred);

        public static SignalHandlerAction<Node> OnNodeRemoved(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node>(target, "node_removed", action, oneShot, deferred);

        public static SignalHandler ConnectNodeRenamed(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "node_renamed", action, oneShot, deferred);

        public static SignalHandlerAction<Node> OnNodeRenamed(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Node>(target, "node_renamed", action, oneShot, deferred);

        public static SignalHandler ConnectPhysicsFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "physics_frame", action, oneShot, deferred);

        public static SignalHandlerAction OnPhysicsFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "physics_frame", action, oneShot, deferred);

        public static SignalHandler ConnectScreenResized(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "screen_resized", action, oneShot, deferred);

        public static SignalHandlerAction OnScreenResized(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "screen_resized", action, oneShot, deferred);

        public static SignalHandler ConnectServerDisconnected(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "server_disconnected", action, oneShot, deferred);

        public static SignalHandlerAction OnServerDisconnected(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "server_disconnected", action, oneShot, deferred);

        public static SignalHandler ConnectTreeChanged(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tree_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnTreeChanged(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "tree_changed", action, oneShot, deferred);

        public static SignalHandler ConnectTimeout(this SceneTreeTimer target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "timeout", action, oneShot, deferred);

        public static SignalHandlerAction OnTimeout(this SceneTreeTimer target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "timeout", action, oneShot, deferred);

        public static SignalHandler ConnectScrolling(this ScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "scrolling", action, oneShot, deferred);

        public static SignalHandlerAction OnScrolling(this ScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "scrolling", action, oneShot, deferred);

        public static SignalHandler ConnectScrollEnded(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "scroll_ended", action, oneShot, deferred);

        public static SignalHandlerAction OnScrollEnded(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "scroll_ended", action, oneShot, deferred);

        public static SignalHandler ConnectScrollStarted(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "scroll_started", action, oneShot, deferred);

        public static SignalHandlerAction OnScrollStarted(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "scroll_started", action, oneShot, deferred);

        public static SignalHandler ConnectSkeletonUpdated(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "skeleton_updated", action, oneShot, deferred);

        public static SignalHandlerAction OnSkeletonUpdated(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "skeleton_updated", action, oneShot, deferred);

        public static SignalHandler ConnectBoneSetupChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "bone_setup_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnBoneSetupChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "bone_setup_changed", action, oneShot, deferred);

        public static SignalHandler ConnectGameplayEntered(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "gameplay_entered", action, oneShot, deferred);

        public static SignalHandlerAction OnGameplayEntered(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "gameplay_entered", action, oneShot, deferred);

        public static SignalHandler ConnectGameplayExited(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "gameplay_exited", action, oneShot, deferred);

        public static SignalHandlerAction OnGameplayExited(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "gameplay_exited", action, oneShot, deferred);

        public static SignalHandler ConnectVisibilityChanged(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "visibility_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnVisibilityChanged(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "visibility_changed", action, oneShot, deferred);

        public static SignalHandler ConnectDragged(this SplitContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "dragged", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnDragged(this SplitContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "dragged", action, oneShot, deferred);

        public static SignalHandler ConnectFrameChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnFrameChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler ConnectTextureChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnTextureChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler ConnectFrameChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnFrameChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler ConnectTextureChanged(this StyleBoxTexture target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnTextureChanged(this StyleBoxTexture target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler ConnectPrePopupPressed(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "pre_popup_pressed", action, oneShot, deferred);

        public static SignalHandlerAction OnPrePopupPressed(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "pre_popup_pressed", action, oneShot, deferred);

        public static SignalHandler ConnectTabChanged(this TabContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tab_changed", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnTabChanged(this TabContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "tab_changed", action, oneShot, deferred);

        public static SignalHandler ConnectTabSelected(this TabContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tab_selected", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnTabSelected(this TabContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "tab_selected", action, oneShot, deferred);

        public static SignalHandler ConnectRepositionActiveTabRequest(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "reposition_active_tab_request", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnRepositionActiveTabRequest(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "reposition_active_tab_request", action, oneShot, deferred);

        public static SignalHandler ConnectRightButtonPressed(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "right_button_pressed", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnRightButtonPressed(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "right_button_pressed", action, oneShot, deferred);

        public static SignalHandler ConnectTabChanged(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tab_changed", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnTabChanged(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "tab_changed", action, oneShot, deferred);

        public static SignalHandler ConnectTabClicked(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tab_clicked", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnTabClicked(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "tab_clicked", action, oneShot, deferred);

        public static SignalHandler ConnectTabClose(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tab_close", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnTabClose(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "tab_close", action, oneShot, deferred);

        public static SignalHandler ConnectTabHover(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tab_hover", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnTabHover(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "tab_hover", action, oneShot, deferred);

        public static SignalHandler ConnectBreakpointToggled(this TextEdit target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "breakpoint_toggled", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnBreakpointToggled(this TextEdit target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "breakpoint_toggled", action, oneShot, deferred);

        public static SignalHandler ConnectCursorChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "cursor_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnCursorChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "cursor_changed", action, oneShot, deferred);

        public static SignalHandler ConnectInfoClicked(this TextEdit target, Action<string, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "info_clicked", action, oneShot, deferred);

        public static SignalHandlerAction<string, int> OnInfoClicked(this TextEdit target, Action<string, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string, int>(target, "info_clicked", action, oneShot, deferred);

        public static SignalHandler ConnectRequestCompletion(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "request_completion", action, oneShot, deferred);

        public static SignalHandlerAction OnRequestCompletion(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "request_completion", action, oneShot, deferred);

        public static SignalHandler ConnectSymbolLookup(this TextEdit target, Action<int, int, string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "symbol_lookup", action, oneShot, deferred);

        public static SignalHandlerAction<int, int, string> OnSymbolLookup(this TextEdit target, Action<int, int, string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int, int, string>(target, "symbol_lookup", action, oneShot, deferred);

        public static SignalHandler ConnectTextChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "text_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnTextChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "text_changed", action, oneShot, deferred);

        public static SignalHandler ConnectSettingsChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "settings_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnSettingsChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "settings_changed", action, oneShot, deferred);

        public static SignalHandler ConnectTimeout(this Timer target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "timeout", action, oneShot, deferred);

        public static SignalHandlerAction OnTimeout(this Timer target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "timeout", action, oneShot, deferred);

        public static SignalHandler ConnectPressed(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "pressed", action, oneShot, deferred);

        public static SignalHandlerAction OnPressed(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "pressed", action, oneShot, deferred);

        public static SignalHandler ConnectReleased(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "released", action, oneShot, deferred);

        public static SignalHandlerAction OnReleased(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "released", action, oneShot, deferred);

        public static SignalHandler ConnectButtonPressed(this Tree target, Action<int, int, TreeItem> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "button_pressed", action, oneShot, deferred);

        public static SignalHandlerAction<int, int, TreeItem> OnButtonPressed(this Tree target, Action<int, int, TreeItem> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int, int, TreeItem>(target, "button_pressed", action, oneShot, deferred);

        public static SignalHandler ConnectCellSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "cell_selected", action, oneShot, deferred);

        public static SignalHandlerAction OnCellSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "cell_selected", action, oneShot, deferred);

        public static SignalHandler ConnectColumnTitlePressed(this Tree target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "column_title_pressed", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnColumnTitlePressed(this Tree target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "column_title_pressed", action, oneShot, deferred);

        public static SignalHandler ConnectCustomPopupEdited(this Tree target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "custom_popup_edited", action, oneShot, deferred);

        public static SignalHandlerAction<bool> OnCustomPopupEdited(this Tree target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<bool>(target, "custom_popup_edited", action, oneShot, deferred);

        public static SignalHandler ConnectEmptyRmb(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "empty_rmb", action, oneShot, deferred);

        public static SignalHandlerAction<Vector2> OnEmptyRmb(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Vector2>(target, "empty_rmb", action, oneShot, deferred);

        public static SignalHandler ConnectEmptyTreeRmbSelected(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "empty_tree_rmb_selected", action, oneShot, deferred);

        public static SignalHandlerAction<Vector2> OnEmptyTreeRmbSelected(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Vector2>(target, "empty_tree_rmb_selected", action, oneShot, deferred);

        public static SignalHandler ConnectItemActivated(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "item_activated", action, oneShot, deferred);

        public static SignalHandlerAction OnItemActivated(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "item_activated", action, oneShot, deferred);

        public static SignalHandler ConnectItemCollapsed(this Tree target, Action<TreeItem> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "item_collapsed", action, oneShot, deferred);

        public static SignalHandlerAction<TreeItem> OnItemCollapsed(this Tree target, Action<TreeItem> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<TreeItem>(target, "item_collapsed", action, oneShot, deferred);

        public static SignalHandler ConnectItemCustomButtonPressed(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "item_custom_button_pressed", action, oneShot, deferred);

        public static SignalHandlerAction OnItemCustomButtonPressed(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "item_custom_button_pressed", action, oneShot, deferred);

        public static SignalHandler ConnectItemDoubleClicked(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "item_double_clicked", action, oneShot, deferred);

        public static SignalHandlerAction OnItemDoubleClicked(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "item_double_clicked", action, oneShot, deferred);

        public static SignalHandler ConnectItemEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "item_edited", action, oneShot, deferred);

        public static SignalHandlerAction OnItemEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "item_edited", action, oneShot, deferred);

        public static SignalHandler ConnectItemRmbEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "item_rmb_edited", action, oneShot, deferred);

        public static SignalHandlerAction OnItemRmbEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "item_rmb_edited", action, oneShot, deferred);

        public static SignalHandler ConnectItemRmbSelected(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "item_rmb_selected", action, oneShot, deferred);

        public static SignalHandlerAction<Vector2> OnItemRmbSelected(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Vector2>(target, "item_rmb_selected", action, oneShot, deferred);

        public static SignalHandler ConnectItemSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "item_selected", action, oneShot, deferred);

        public static SignalHandlerAction OnItemSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "item_selected", action, oneShot, deferred);

        public static SignalHandler ConnectMultiSelected(this Tree target, Action<int, TreeItem, bool> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "multi_selected", action, oneShot, deferred);

        public static SignalHandlerAction<int, TreeItem, bool> OnMultiSelected(this Tree target, Action<int, TreeItem, bool> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int, TreeItem, bool>(target, "multi_selected", action, oneShot, deferred);

        public static SignalHandler ConnectNothingSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "nothing_selected", action, oneShot, deferred);

        public static SignalHandlerAction OnNothingSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "nothing_selected", action, oneShot, deferred);

        public static SignalHandler ConnectTweenAllCompleted(this Tween target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tween_all_completed", action, oneShot, deferred);

        public static SignalHandlerAction OnTweenAllCompleted(this Tween target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "tween_all_completed", action, oneShot, deferred);

        public static SignalHandler ConnectTweenCompleted(this Tween target, Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tween_completed", action, oneShot, deferred);

        public static SignalHandlerAction<Object, NodePath> OnTweenCompleted(this Tween target, Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Object, NodePath>(target, "tween_completed", action, oneShot, deferred);

        public static SignalHandler ConnectTweenStarted(this Tween target, Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tween_started", action, oneShot, deferred);

        public static SignalHandlerAction<Object, NodePath> OnTweenStarted(this Tween target, Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Object, NodePath>(target, "tween_started", action, oneShot, deferred);

        public static SignalHandler ConnectTweenStep(this Tween target, Action<Object, float, NodePath, Object> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "tween_step", action, oneShot, deferred);

        public static SignalHandlerAction<Object, float, NodePath, Object> OnTweenStep(this Tween target, Action<Object, float, NodePath, Object> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Object, float, NodePath, Object>(target, "tween_step", action, oneShot, deferred);

        public static SignalHandler ConnectVersionChanged(this UndoRedo target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "version_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnVersionChanged(this UndoRedo target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "version_changed", action, oneShot, deferred);

        public static SignalHandler ConnectFinished(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "finished", action, oneShot, deferred);

        public static SignalHandlerAction OnFinished(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "finished", action, oneShot, deferred);

        public static SignalHandler ConnectGuiFocusChanged(this Viewport target, Action<Control> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "gui_focus_changed", action, oneShot, deferred);

        public static SignalHandlerAction<Control> OnGuiFocusChanged(this Viewport target, Action<Control> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Control>(target, "gui_focus_changed", action, oneShot, deferred);

        public static SignalHandler ConnectSizeChanged(this Viewport target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "size_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnSizeChanged(this Viewport target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "size_changed", action, oneShot, deferred);

        public static SignalHandler ConnectCameraEntered(this VisibilityNotifier target, Action<Camera> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "camera_entered", action, oneShot, deferred);

        public static SignalHandlerAction<Camera> OnCameraEntered(this VisibilityNotifier target, Action<Camera> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Camera>(target, "camera_entered", action, oneShot, deferred);

        public static SignalHandler ConnectCameraExited(this VisibilityNotifier target, Action<Camera> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "camera_exited", action, oneShot, deferred);

        public static SignalHandlerAction<Camera> OnCameraExited(this VisibilityNotifier target, Action<Camera> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Camera>(target, "camera_exited", action, oneShot, deferred);

        public static SignalHandler ConnectScreenEntered(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "screen_entered", action, oneShot, deferred);

        public static SignalHandlerAction OnScreenEntered(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "screen_entered", action, oneShot, deferred);

        public static SignalHandler ConnectScreenExited(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "screen_exited", action, oneShot, deferred);

        public static SignalHandlerAction OnScreenExited(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "screen_exited", action, oneShot, deferred);

        public static SignalHandler ConnectScreenEntered(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "screen_entered", action, oneShot, deferred);

        public static SignalHandlerAction OnScreenEntered(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "screen_entered", action, oneShot, deferred);

        public static SignalHandler ConnectScreenExited(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "screen_exited", action, oneShot, deferred);

        public static SignalHandlerAction OnScreenExited(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "screen_exited", action, oneShot, deferred);

        public static SignalHandler ConnectViewportEntered(this VisibilityNotifier2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "viewport_entered", action, oneShot, deferred);

        public static SignalHandlerAction<Viewport> OnViewportEntered(this VisibilityNotifier2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Viewport>(target, "viewport_entered", action, oneShot, deferred);

        public static SignalHandler ConnectViewportExited(this VisibilityNotifier2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "viewport_exited", action, oneShot, deferred);

        public static SignalHandlerAction<Viewport> OnViewportExited(this VisibilityNotifier2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Viewport>(target, "viewport_exited", action, oneShot, deferred);

        public static SignalHandler ConnectNodePortsChanged(this VisualScript target, Action<string, int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "node_ports_changed", action, oneShot, deferred);

        public static SignalHandlerAction<string, int> OnNodePortsChanged(this VisualScript target, Action<string, int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string, int>(target, "node_ports_changed", action, oneShot, deferred);

        public static SignalHandler ConnectPortsChanged(this VisualScriptNode target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "ports_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnPortsChanged(this VisualScriptNode target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "ports_changed", action, oneShot, deferred);

        public static SignalHandler ConnectVisualServerFramePostDraw(Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "frame_post_draw", action, oneShot, deferred);

        public static SignalHandlerAction OnVisualServerFramePostDraw(Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(VisualServer.Singleton, "frame_post_draw", action, oneShot, deferred);

        public static SignalHandler ConnectVisualServerFramePreDraw(Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "frame_pre_draw", action, oneShot, deferred);

        public static SignalHandlerAction OnVisualServerFramePreDraw(Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(VisualServer.Singleton, "frame_pre_draw", action, oneShot, deferred);

        public static SignalHandler ConnectEditorRefreshRequest(this VisualShaderNode target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "editor_refresh_request", action, oneShot, deferred);

        public static SignalHandlerAction OnEditorRefreshRequest(this VisualShaderNode target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "editor_refresh_request", action, oneShot, deferred);

        public static SignalHandler ConnectInputTypeChanged(this VisualShaderNodeInput target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "input_type_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnInputTypeChanged(this VisualShaderNodeInput target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "input_type_changed", action, oneShot, deferred);

        public static SignalHandler ConnectDataChannelReceived(this WebRTCPeerConnection target, Action<Object> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "data_channel_received", action, oneShot, deferred);

        public static SignalHandlerAction<Object> OnDataChannelReceived(this WebRTCPeerConnection target, Action<Object> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<Object>(target, "data_channel_received", action, oneShot, deferred);

        public static SignalHandler ConnectIceCandidateCreated(this WebRTCPeerConnection target, Action<int, string, string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "ice_candidate_created", action, oneShot, deferred);

        public static SignalHandlerAction<int, string, string> OnIceCandidateCreated(this WebRTCPeerConnection target, Action<int, string, string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int, string, string>(target, "ice_candidate_created", action, oneShot, deferred);

        public static SignalHandler ConnectSessionDescriptionCreated(this WebRTCPeerConnection target, Action<string, string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "session_description_created", action, oneShot, deferred);

        public static SignalHandlerAction<string, string> OnSessionDescriptionCreated(this WebRTCPeerConnection target, Action<string, string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string, string>(target, "session_description_created", action, oneShot, deferred);

        public static SignalHandler ConnectConnectionClosed(this WebSocketClient target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "connection_closed", action, oneShot, deferred);

        public static SignalHandlerAction<bool> OnConnectionClosed(this WebSocketClient target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<bool>(target, "connection_closed", action, oneShot, deferred);

        public static SignalHandler ConnectConnectionError(this WebSocketClient target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "connection_error", action, oneShot, deferred);

        public static SignalHandlerAction OnConnectionError(this WebSocketClient target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "connection_error", action, oneShot, deferred);

        public static SignalHandler ConnectConnectionEstablished(this WebSocketClient target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "connection_established", action, oneShot, deferred);

        public static SignalHandlerAction<string> OnConnectionEstablished(this WebSocketClient target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string>(target, "connection_established", action, oneShot, deferred);

        public static SignalHandler ConnectDataReceived(this WebSocketClient target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "data_received", action, oneShot, deferred);

        public static SignalHandlerAction OnDataReceived(this WebSocketClient target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "data_received", action, oneShot, deferred);

        public static SignalHandler ConnectServerCloseRequest(this WebSocketClient target, Action<int, string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "server_close_request", action, oneShot, deferred);

        public static SignalHandlerAction<int, string> OnServerCloseRequest(this WebSocketClient target, Action<int, string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int, string>(target, "server_close_request", action, oneShot, deferred);

        public static SignalHandler ConnectPeerPacket(this WebSocketMultiplayerPeer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "peer_packet", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnPeerPacket(this WebSocketMultiplayerPeer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "peer_packet", action, oneShot, deferred);

        public static SignalHandler ConnectClientCloseRequest(this WebSocketServer target, Action<int, int, string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "client_close_request", action, oneShot, deferred);

        public static SignalHandlerAction<int, int, string> OnClientCloseRequest(this WebSocketServer target, Action<int, int, string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int, int, string>(target, "client_close_request", action, oneShot, deferred);

        public static SignalHandler ConnectClientConnected(this WebSocketServer target, Action<int, string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "client_connected", action, oneShot, deferred);

        public static SignalHandlerAction<int, string> OnClientConnected(this WebSocketServer target, Action<int, string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int, string>(target, "client_connected", action, oneShot, deferred);

        public static SignalHandler ConnectClientDisconnected(this WebSocketServer target, Action<int, bool> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "client_disconnected", action, oneShot, deferred);

        public static SignalHandlerAction<int, bool> OnClientDisconnected(this WebSocketServer target, Action<int, bool> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int, bool>(target, "client_disconnected", action, oneShot, deferred);

        public static SignalHandler ConnectDataReceived(this WebSocketServer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "data_received", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnDataReceived(this WebSocketServer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "data_received", action, oneShot, deferred);

        public static SignalHandler ConnectReferenceSpaceReset(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "reference_space_reset", action, oneShot, deferred);

        public static SignalHandlerAction OnReferenceSpaceReset(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "reference_space_reset", action, oneShot, deferred);

        public static SignalHandler ConnectSelect(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "select", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnSelect(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "select", action, oneShot, deferred);

        public static SignalHandler ConnectSelectend(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "selectend", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnSelectend(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "selectend", action, oneShot, deferred);

        public static SignalHandler ConnectSelectstart(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "selectstart", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnSelectstart(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "selectstart", action, oneShot, deferred);

        public static SignalHandler ConnectSessionEnded(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "session_ended", action, oneShot, deferred);

        public static SignalHandlerAction OnSessionEnded(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "session_ended", action, oneShot, deferred);

        public static SignalHandler ConnectSessionFailed(this WebXRInterface target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "session_failed", action, oneShot, deferred);

        public static SignalHandlerAction<string> OnSessionFailed(this WebXRInterface target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string>(target, "session_failed", action, oneShot, deferred);

        public static SignalHandler ConnectSessionStarted(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "session_started", action, oneShot, deferred);

        public static SignalHandlerAction OnSessionStarted(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "session_started", action, oneShot, deferred);

        public static SignalHandler ConnectSessionSupported(this WebXRInterface target, Action<string, bool> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "session_supported", action, oneShot, deferred);

        public static SignalHandlerAction<string, bool> OnSessionSupported(this WebXRInterface target, Action<string, bool> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<string, bool>(target, "session_supported", action, oneShot, deferred);

        public static SignalHandler ConnectSqueeze(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "squeeze", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnSqueeze(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "squeeze", action, oneShot, deferred);

        public static SignalHandler ConnectSqueezeend(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "squeezeend", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnSqueezeend(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "squeezeend", action, oneShot, deferred);

        public static SignalHandler ConnectSqueezestart(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "squeezestart", action, oneShot, deferred);

        public static SignalHandlerAction<int> OnSqueezestart(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction<int>(target, "squeezestart", action, oneShot, deferred);

        public static SignalHandler ConnectVisibilityStateChanged(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            Connect(target, "visibility_state_changed", action, oneShot, deferred);

        public static SignalHandlerAction OnVisibilityStateChanged(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction(target, "visibility_state_changed", action, oneShot, deferred);
    }
}