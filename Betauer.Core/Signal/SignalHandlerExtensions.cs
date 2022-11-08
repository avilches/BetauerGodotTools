using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;
using Environment = Godot.Environment;
using Range = Godot.Range;

namespace Betauer.Core.Signal {
    public static partial class SignalExtensions {

        public static SignalHandler On(this Object target, string signal, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, signal, action, oneShot, deferred);

        public static SignalHandler On<T>(this Object target, string signal, Action<T> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, signal, action, oneShot, deferred);

        public static SignalHandler On<T1, T2>(this Object target, string signal, Action<T1, T2> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, signal, action, oneShot, deferred);

        public static SignalHandler On<T1, T2, T3>(this Object target, string signal, Action<T1, T2, T3> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, signal, action, oneShot, deferred);

        public static SignalHandler On<T1, T2, T3, T4>(this Object target, string signal, Action<T1, T2, T3, T4> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, signal, action, oneShot, deferred);

        public static SignalHandler On<T1, T2, T3, T4, T5>(this Object target, string signal, Action<T1, T2, T3, T4, T5> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, signal, action, oneShot, deferred);

        public static SignalHandler On<T1, T2, T3, T4, T5, T6>(this Object target, string signal, Action<T1, T2, T3, T4, T5, T6> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, signal, action, oneShot, deferred);
      
        public static SignalHandler OnCancelled(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "cancelled", action, oneShot, deferred);

        public static SignalHandler OnConfirmed(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "confirmed", action, oneShot, deferred);

        public static SignalHandler OnCustomAction(this AcceptDialog target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, "custom_action", action, oneShot, deferred);

        public static SignalHandler OnAnimationFinished(this AnimatedSprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this AnimatedSprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler OnAnimationFinished(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler OnTracksChanged(this Animation target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "tracks_changed", action, oneShot, deferred);

        public static SignalHandler OnAnimationAdded(this AnimationLibrary target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_added", action, oneShot, deferred);

        public static SignalHandler OnAnimationRemoved(this AnimationLibrary target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_removed", action, oneShot, deferred);

        public static SignalHandler OnAnimationRenamed(this AnimationLibrary target, Action<Godot.StringName, Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_renamed", action, oneShot, deferred);

        public static SignalHandler OnRemovedFromGraph(this AnimationNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "removed_from_graph", action, oneShot, deferred);

        public static SignalHandler OnTreeChanged(this AnimationNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "tree_changed", action, oneShot, deferred);

        public static SignalHandler OnTrianglesUpdated(this AnimationNodeBlendSpace2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "triangles_updated", action, oneShot, deferred);

        public static SignalHandler OnNodeChanged(this AnimationNodeBlendTree target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, "node_changed", action, oneShot, deferred);

        public static SignalHandler OnAdvanceConditionChanged(this AnimationNodeStateMachineTransition target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "advance_condition_changed", action, oneShot, deferred);

        public static SignalHandler OnAnimationChanged(this AnimationPlayer target, Action<Godot.StringName, Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_changed", action, oneShot, deferred);

        public static SignalHandler OnAnimationFinished(this AnimationPlayer target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandler OnAnimationLibrariesUpdated(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_libraries_updated", action, oneShot, deferred);

        public static SignalHandler OnAnimationListChanged(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_list_changed", action, oneShot, deferred);

        public static SignalHandler OnAnimationStarted(this AnimationPlayer target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_started", action, oneShot, deferred);

        public static SignalHandler OnCachesCleared(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "caches_cleared", action, oneShot, deferred);

        public static SignalHandler OnAnimationPlayerChanged(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_player_changed", action, oneShot, deferred);

        public static SignalHandler OnAreaEntered(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_entered", action, oneShot, deferred);

        public static SignalHandler OnAreaExited(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_exited", action, oneShot, deferred);

        public static SignalHandler OnAreaShapeEntered(this Area2D target, Action<Godot.RID, Area2D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnAreaShapeExited(this Area2D target, Action<Godot.RID, Area2D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this Area2D target, Action<Node2D> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this Area2D target, Action<Node2D> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this Area2D target, Action<Godot.RID, Node2D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this Area2D target, Action<Godot.RID, Node2D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnAreaEntered(this Area3D target, Action<Area3D> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_entered", action, oneShot, deferred);

        public static SignalHandler OnAreaExited(this Area3D target, Action<Area3D> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_exited", action, oneShot, deferred);

        public static SignalHandler OnAreaShapeEntered(this Area3D target, Action<Godot.RID, Area3D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnAreaShapeExited(this Area3D target, Action<Godot.RID, Area3D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this Area3D target, Action<Node3D> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this Area3D target, Action<Node3D> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this Area3D target, Action<Godot.RID, Node3D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this Area3D target, Action<Godot.RID, Node3D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnAudioServerBusLayoutChanged(Action action, bool oneShot = false, bool deferred = false) =>
            On(AudioServer.Singleton, "bus_layout_changed", action, oneShot, deferred);

        public static SignalHandler OnFinished(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "finished", action, oneShot, deferred);

        public static SignalHandler OnFinished(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "finished", action, oneShot, deferred);

        public static SignalHandler OnFinished(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "finished", action, oneShot, deferred);

        public static SignalHandler OnButtonDown(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "button_down", action, oneShot, deferred);

        public static SignalHandler OnButtonUp(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "button_up", action, oneShot, deferred);

        public static SignalHandler OnPressed(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "pressed", action, oneShot, deferred);

        public static SignalHandler OnToggled(this BaseButton target, Action<System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, "toggled", action, oneShot, deferred);

        public static SignalHandler OnBoneMapUpdated(this BoneMap target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "bone_map_updated", action, oneShot, deferred);

        public static SignalHandler OnProfileUpdated(this BoneMap target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "profile_updated", action, oneShot, deferred);

        public static SignalHandler OnPressed(this ButtonGroup target, Action<BaseButton> action, bool oneShot = false, bool deferred = false) =>
            On(target, "pressed", action, oneShot, deferred);

        public static SignalHandler OnCameraServerCameraFeedAdded(Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(CameraServer.Singleton, "camera_feed_added", action, oneShot, deferred);

        public static SignalHandler OnCameraServerCameraFeedRemoved(Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(CameraServer.Singleton, "camera_feed_removed", action, oneShot, deferred);

        public static SignalHandler OnDraw(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "draw", action, oneShot, deferred);

        public static SignalHandler OnHidden(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "hidden", action, oneShot, deferred);

        public static SignalHandler OnItemRectChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_rect_changed", action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "visibility_changed", action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "visibility_changed", action, oneShot, deferred);

        public static SignalHandler OnBreakpointToggled(this CodeEdit target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "breakpoint_toggled", action, oneShot, deferred);

        public static SignalHandler OnCodeCompletionRequested(this CodeEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "code_completion_requested", action, oneShot, deferred);

        public static SignalHandler OnSymbolLookup(this CodeEdit target, Action<System.String, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "symbol_lookup", action, oneShot, deferred);

        public static SignalHandler OnSymbolValidate(this CodeEdit target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, "symbol_validate", action, oneShot, deferred);

        public static SignalHandler OnInputEvent(this CollisionObject2D target, Action<Node, InputEvent, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "input_event", action, oneShot, deferred);

        public static SignalHandler OnMouseEntered(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandler OnMouseExited(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandler OnMouseShapeEntered(this CollisionObject2D target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnMouseShapeExited(this CollisionObject2D target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnInputEvent(this CollisionObject3D target, Action<Node, InputEvent, Godot.Vector3, Godot.Vector3, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "input_event", action, oneShot, deferred);

        public static SignalHandler OnMouseEntered(this CollisionObject3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandler OnMouseExited(this CollisionObject3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandler OnColorChanged(this ColorPicker target, Action<Godot.Color> action, bool oneShot = false, bool deferred = false) =>
            On(target, "color_changed", action, oneShot, deferred);

        public static SignalHandler OnPresetAdded(this ColorPicker target, Action<Godot.Color> action, bool oneShot = false, bool deferred = false) =>
            On(target, "preset_added", action, oneShot, deferred);

        public static SignalHandler OnPresetRemoved(this ColorPicker target, Action<Godot.Color> action, bool oneShot = false, bool deferred = false) =>
            On(target, "preset_removed", action, oneShot, deferred);

        public static SignalHandler OnColorChanged(this ColorPickerButton target, Action<Godot.Color> action, bool oneShot = false, bool deferred = false) =>
            On(target, "color_changed", action, oneShot, deferred);

        public static SignalHandler OnPickerCreated(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "picker_created", action, oneShot, deferred);

        public static SignalHandler OnPopupClosed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "popup_closed", action, oneShot, deferred);

        public static SignalHandler OnPreSortChildren(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "pre_sort_children", action, oneShot, deferred);

        public static SignalHandler OnSortChildren(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "sort_children", action, oneShot, deferred);

        public static SignalHandler OnFocusEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "focus_entered", action, oneShot, deferred);

        public static SignalHandler OnFocusExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "focus_exited", action, oneShot, deferred);

        public static SignalHandler OnGuiInput(this Control target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            On(target, "gui_input", action, oneShot, deferred);

        public static SignalHandler OnMinimumSizeChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "minimum_size_changed", action, oneShot, deferred);

        public static SignalHandler OnMouseEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandler OnMouseExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandler OnResized(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "resized", action, oneShot, deferred);

        public static SignalHandler OnSizeFlagsChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "size_flags_changed", action, oneShot, deferred);

        public static SignalHandler OnThemeChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "theme_changed", action, oneShot, deferred);

        public static SignalHandler OnRangeChanged(this Curve target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "range_changed", action, oneShot, deferred);

        public static SignalHandler OnDirSelected(this FileDialog target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, "dir_selected", action, oneShot, deferred);

        public static SignalHandler OnFileSelected(this FileDialog target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, "file_selected", action, oneShot, deferred);

        public static SignalHandler OnFilesSelected(this FileDialog target, Action<System.String[]> action, bool oneShot = false, bool deferred = false) =>
            On(target, "files_selected", action, oneShot, deferred);

        public static SignalHandler OnBeginNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "begin_node_move", action, oneShot, deferred);

        public static SignalHandler OnConnectionDragEnded(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_drag_ended", action, oneShot, deferred);

        public static SignalHandler OnConnectionDragStarted(this GraphEdit target, Action<System.String, System.Int32, System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_drag_started", action, oneShot, deferred);

        public static SignalHandler OnConnectionFromEmpty(this GraphEdit target, Action<Godot.StringName, System.Int32, Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_from_empty", action, oneShot, deferred);

        public static SignalHandler OnConnectionRequest(this GraphEdit target, Action<Godot.StringName, System.Int32, Godot.StringName, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_request", action, oneShot, deferred);

        public static SignalHandler OnConnectionToEmpty(this GraphEdit target, Action<Godot.StringName, System.Int32, Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_to_empty", action, oneShot, deferred);

        public static SignalHandler OnCopyNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "copy_nodes_request", action, oneShot, deferred);

        public static SignalHandler OnDeleteNodesRequest(this GraphEdit target, Action<Godot.Collections.Array> action, bool oneShot = false, bool deferred = false) =>
            On(target, "delete_nodes_request", action, oneShot, deferred);

        public static SignalHandler OnDisconnectionRequest(this GraphEdit target, Action<Godot.StringName, System.Int32, Godot.StringName, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "disconnection_request", action, oneShot, deferred);

        public static SignalHandler OnDuplicateNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "duplicate_nodes_request", action, oneShot, deferred);

        public static SignalHandler OnEndNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "end_node_move", action, oneShot, deferred);

        public static SignalHandler OnNodeDeselected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "node_deselected", action, oneShot, deferred);

        public static SignalHandler OnNodeSelected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "node_selected", action, oneShot, deferred);

        public static SignalHandler OnPasteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "paste_nodes_request", action, oneShot, deferred);

        public static SignalHandler OnPopupRequest(this GraphEdit target, Action<Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "popup_request", action, oneShot, deferred);

        public static SignalHandler OnScrollOffsetChanged(this GraphEdit target, Action<Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "scroll_offset_changed", action, oneShot, deferred);

        public static SignalHandler OnCloseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "close_request", action, oneShot, deferred);

        public static SignalHandler OnDeselected(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "deselected", action, oneShot, deferred);

        public static SignalHandler OnDragged(this GraphNode target, Action<Godot.Vector2, Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "dragged", action, oneShot, deferred);

        public static SignalHandler OnPositionOffsetChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "position_offset_changed", action, oneShot, deferred);

        public static SignalHandler OnRaiseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "raise_request", action, oneShot, deferred);

        public static SignalHandler OnResizeRequest(this GraphNode target, Action<Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "resize_request", action, oneShot, deferred);

        public static SignalHandler OnSelected(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "selected", action, oneShot, deferred);

        public static SignalHandler OnSlotUpdated(this GraphNode target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "slot_updated", action, oneShot, deferred);

        public static SignalHandler OnCellSizeChanged(this GridMap target, Action<Godot.Vector3> action, bool oneShot = false, bool deferred = false) =>
            On(target, "cell_size_changed", action, oneShot, deferred);

        public static SignalHandler OnRequestCompleted(this HTTPRequest target, Action<System.Int32, System.Int32, System.String[], System.Byte[]> action, bool oneShot = false, bool deferred = false) =>
            On(target, "request_completed", action, oneShot, deferred);

        public static SignalHandler OnInputJoyConnectionChanged(Action<System.Int32, System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(Input.Singleton, "joy_connection_changed", action, oneShot, deferred);

        public static SignalHandler OnEmptyClicked(this ItemList target, Action<Godot.Vector2, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "empty_clicked", action, oneShot, deferred);

        public static SignalHandler OnItemActivated(this ItemList target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_activated", action, oneShot, deferred);

        public static SignalHandler OnItemClicked(this ItemList target, Action<System.Int32, Godot.Vector2, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_clicked", action, oneShot, deferred);

        public static SignalHandler OnItemSelected(this ItemList target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_selected", action, oneShot, deferred);

        public static SignalHandler OnMultiSelected(this ItemList target, Action<System.Int32, System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, "multi_selected", action, oneShot, deferred);

        public static SignalHandler OnJavaScriptBridgePwaUpdateAvailable(Action action, bool oneShot = false, bool deferred = false) =>
            On(JavaScriptBridge.Singleton, "pwa_update_available", action, oneShot, deferred);

        public static SignalHandler OnTextChanged(this LineEdit target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, "text_changed", action, oneShot, deferred);

        public static SignalHandler OnTextChangeRejected(this LineEdit target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, "text_change_rejected", action, oneShot, deferred);

        public static SignalHandler OnTextSubmitted(this LineEdit target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, "text_submitted", action, oneShot, deferred);

        public static SignalHandler OnRequestPermissionsResult(this MainLoop target, Action<System.String, System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, "on_request_permissions_result", action, oneShot, deferred);

        public static SignalHandler OnAboutToPopup(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "about_to_popup", action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler OnConnectedToServer(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "connected_to_server", action, oneShot, deferred);

        public static SignalHandler OnConnectionFailed(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_failed", action, oneShot, deferred);

        public static SignalHandler OnPeerConnected(this MultiplayerAPI target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "peer_connected", action, oneShot, deferred);

        public static SignalHandler OnPeerDisconnected(this MultiplayerAPI target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "peer_disconnected", action, oneShot, deferred);

        public static SignalHandler OnServerDisconnected(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "server_disconnected", action, oneShot, deferred);

        public static SignalHandler OnConnectionFailed(this MultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_failed", action, oneShot, deferred);

        public static SignalHandler OnConnectionSucceeded(this MultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_succeeded", action, oneShot, deferred);

        public static SignalHandler OnPeerConnected(this MultiplayerPeer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "peer_connected", action, oneShot, deferred);

        public static SignalHandler OnPeerDisconnected(this MultiplayerPeer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "peer_disconnected", action, oneShot, deferred);

        public static SignalHandler OnServerDisconnected(this MultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "server_disconnected", action, oneShot, deferred);

        public static SignalHandler OnDespawned(this MultiplayerSpawner target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "despawned", action, oneShot, deferred);

        public static SignalHandler OnSpawned(this MultiplayerSpawner target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "spawned", action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this MultiplayerSynchronizer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "visibility_changed", action, oneShot, deferred);

        public static SignalHandler OnNavigationFinished(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "navigation_finished", action, oneShot, deferred);

        public static SignalHandler OnPathChanged(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "path_changed", action, oneShot, deferred);

        public static SignalHandler OnTargetReached(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "target_reached", action, oneShot, deferred);

        public static SignalHandler OnVelocityComputed(this NavigationAgent2D target, Action<Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "velocity_computed", action, oneShot, deferred);

        public static SignalHandler OnNavigationFinished(this NavigationAgent3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "navigation_finished", action, oneShot, deferred);

        public static SignalHandler OnPathChanged(this NavigationAgent3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "path_changed", action, oneShot, deferred);

        public static SignalHandler OnTargetReached(this NavigationAgent3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "target_reached", action, oneShot, deferred);

        public static SignalHandler OnVelocityComputed(this NavigationAgent3D target, Action<Godot.Vector3> action, bool oneShot = false, bool deferred = false) =>
            On(target, "velocity_computed", action, oneShot, deferred);

        public static SignalHandler OnBakeFinished(this NavigationRegion3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "bake_finished", action, oneShot, deferred);

        public static SignalHandler OnNavigationMeshChanged(this NavigationRegion3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "navigation_mesh_changed", action, oneShot, deferred);

        public static SignalHandler OnNavigationServer2DMapChanged(Action<Godot.RID> action, bool oneShot = false, bool deferred = false) =>
            On(NavigationServer2D.Singleton, "map_changed", action, oneShot, deferred);

        public static SignalHandler OnNavigationServer3DMapChanged(Action<Godot.RID> action, bool oneShot = false, bool deferred = false) =>
            On(NavigationServer3D.Singleton, "map_changed", action, oneShot, deferred);

        public static SignalHandler OnNavigationServer3DNavigationDebugChanged(Action action, bool oneShot = false, bool deferred = false) =>
            On(NavigationServer3D.Singleton, "navigation_debug_changed", action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler OnChildEnteredTree(this Node target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "child_entered_tree", action, oneShot, deferred);

        public static SignalHandler OnChildExitingTree(this Node target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "child_exiting_tree", action, oneShot, deferred);

        public static SignalHandler OnReady(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "ready", action, oneShot, deferred);

        public static SignalHandler OnRenamed(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "renamed", action, oneShot, deferred);

        public static SignalHandler OnTreeEntered(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "tree_entered", action, oneShot, deferred);

        public static SignalHandler OnTreeExited(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "tree_exited", action, oneShot, deferred);

        public static SignalHandler OnTreeExiting(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "tree_exiting", action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this Node3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "visibility_changed", action, oneShot, deferred);

        public static SignalHandler OnPropertyListChanged(this Object target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "property_list_changed", action, oneShot, deferred);

        public static SignalHandler OnScriptChanged(this Object target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "script_changed", action, oneShot, deferred);

        public static SignalHandler OnItemFocused(this OptionButton target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_focused", action, oneShot, deferred);

        public static SignalHandler OnItemSelected(this OptionButton target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_selected", action, oneShot, deferred);

        public static SignalHandler OnCurveChanged(this Path3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "curve_changed", action, oneShot, deferred);

        public static SignalHandler OnPopupHide(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "popup_hide", action, oneShot, deferred);

        public static SignalHandler OnIdFocused(this PopupMenu target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "id_focused", action, oneShot, deferred);

        public static SignalHandler OnIdPressed(this PopupMenu target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "id_pressed", action, oneShot, deferred);

        public static SignalHandler OnIndexPressed(this PopupMenu target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "index_pressed", action, oneShot, deferred);

        public static SignalHandler OnMenuChanged(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "menu_changed", action, oneShot, deferred);

        public static SignalHandler OnChanged(this Range target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "changed", action, oneShot, deferred);

        public static SignalHandler OnValueChanged(this Range target, Action<System.Single> action, bool oneShot = false, bool deferred = false) =>
            On(target, "value_changed", action, oneShot, deferred);

        public static SignalHandler OnRenderingServerFramePostDraw(Action action, bool oneShot = false, bool deferred = false) =>
            On(RenderingServer.Singleton, "frame_post_draw", action, oneShot, deferred);

        public static SignalHandler OnRenderingServerFramePreDraw(Action action, bool oneShot = false, bool deferred = false) =>
            On(RenderingServer.Singleton, "frame_pre_draw", action, oneShot, deferred);

        public static SignalHandler OnChanged(this Resource target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "changed", action, oneShot, deferred);

        public static SignalHandler OnSetupLocalToSceneRequested(this Resource target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "setup_local_to_scene_requested", action, oneShot, deferred);

        public static SignalHandler OnFinished(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "finished", action, oneShot, deferred);

        public static SignalHandler OnMetaClicked(this RichTextLabel target, Action<Godot.Variant> action, bool oneShot = false, bool deferred = false) =>
            On(target, "meta_clicked", action, oneShot, deferred);

        public static SignalHandler OnMetaHoverEnded(this RichTextLabel target, Action<Godot.Variant> action, bool oneShot = false, bool deferred = false) =>
            On(target, "meta_hover_ended", action, oneShot, deferred);

        public static SignalHandler OnMetaHoverStarted(this RichTextLabel target, Action<Godot.Variant> action, bool oneShot = false, bool deferred = false) =>
            On(target, "meta_hover_started", action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this RigidBody2D target, Action<Godot.RID, Node, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this RigidBody2D target, Action<Godot.RID, Node, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnSleepingStateChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "sleeping_state_changed", action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this RigidBody3D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this RigidBody3D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this RigidBody3D target, Action<Godot.RID, Node, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this RigidBody3D target, Action<Godot.RID, Node, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnSleepingStateChanged(this RigidBody3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "sleeping_state_changed", action, oneShot, deferred);

        public static SignalHandler OnPeerPacket(this SceneMultiplayer target, Action<System.Int32, System.Byte[]> action, bool oneShot = false, bool deferred = false) =>
            On(target, "peer_packet", action, oneShot, deferred);

        public static SignalHandler OnNodeAdded(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "node_added", action, oneShot, deferred);

        public static SignalHandler OnNodeConfigurationWarningChanged(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "node_configuration_warning_changed", action, oneShot, deferred);

        public static SignalHandler OnNodeRemoved(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "node_removed", action, oneShot, deferred);

        public static SignalHandler OnNodeRenamed(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "node_renamed", action, oneShot, deferred);

        public static SignalHandler OnPhysicsFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "physics_frame", action, oneShot, deferred);

        public static SignalHandler OnProcessFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "process_frame", action, oneShot, deferred);

        public static SignalHandler OnTreeChanged(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "tree_changed", action, oneShot, deferred);

        public static SignalHandler OnTreeProcessModeChanged(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "tree_process_mode_changed", action, oneShot, deferred);

        public static SignalHandler OnTimeout(this SceneTreeTimer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "timeout", action, oneShot, deferred);

        public static SignalHandler OnScrolling(this ScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "scrolling", action, oneShot, deferred);

        public static SignalHandler OnScrollEnded(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "scroll_ended", action, oneShot, deferred);

        public static SignalHandler OnScrollStarted(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "scroll_started", action, oneShot, deferred);

        public static SignalHandler OnBoneSetupChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "bone_setup_changed", action, oneShot, deferred);

        public static SignalHandler OnBoneEnabledChanged(this Skeleton3D target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "bone_enabled_changed", action, oneShot, deferred);

        public static SignalHandler OnBonePoseChanged(this Skeleton3D target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "bone_pose_changed", action, oneShot, deferred);

        public static SignalHandler OnPoseUpdated(this Skeleton3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "pose_updated", action, oneShot, deferred);

        public static SignalHandler OnShowRestOnlyChanged(this Skeleton3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "show_rest_only_changed", action, oneShot, deferred);

        public static SignalHandler OnProfileUpdated(this SkeletonProfile target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "profile_updated", action, oneShot, deferred);

        public static SignalHandler OnDragEnded(this Slider target, Action<System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, "drag_ended", action, oneShot, deferred);

        public static SignalHandler OnDragStarted(this Slider target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "drag_started", action, oneShot, deferred);

        public static SignalHandler OnDragged(this SplitContainer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "dragged", action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this Sprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this Sprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler OnActiveTabRearranged(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "active_tab_rearranged", action, oneShot, deferred);

        public static SignalHandler OnTabButtonPressed(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_button_pressed", action, oneShot, deferred);

        public static SignalHandler OnTabChanged(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_changed", action, oneShot, deferred);

        public static SignalHandler OnTabClicked(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_clicked", action, oneShot, deferred);

        public static SignalHandler OnTabClosePressed(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_close_pressed", action, oneShot, deferred);

        public static SignalHandler OnTabHovered(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_hovered", action, oneShot, deferred);

        public static SignalHandler OnTabRmbClicked(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_rmb_clicked", action, oneShot, deferred);

        public static SignalHandler OnTabSelected(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_selected", action, oneShot, deferred);

        public static SignalHandler OnPrePopupPressed(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "pre_popup_pressed", action, oneShot, deferred);

        public static SignalHandler OnTabButtonPressed(this TabContainer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_button_pressed", action, oneShot, deferred);

        public static SignalHandler OnTabChanged(this TabContainer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_changed", action, oneShot, deferred);

        public static SignalHandler OnTabSelected(this TabContainer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_selected", action, oneShot, deferred);

        public static SignalHandler OnCaretChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "caret_changed", action, oneShot, deferred);

        public static SignalHandler OnGutterAdded(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "gutter_added", action, oneShot, deferred);

        public static SignalHandler OnGutterClicked(this TextEdit target, Action<System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "gutter_clicked", action, oneShot, deferred);

        public static SignalHandler OnGutterRemoved(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "gutter_removed", action, oneShot, deferred);

        public static SignalHandler OnLinesEditedFrom(this TextEdit target, Action<System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "lines_edited_from", action, oneShot, deferred);

        public static SignalHandler OnTextChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "text_changed", action, oneShot, deferred);

        public static SignalHandler OnTextSet(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "text_set", action, oneShot, deferred);

        public static SignalHandler OnTextServerManagerInterfaceAdded(Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(TextServerManager.Singleton, "interface_added", action, oneShot, deferred);

        public static SignalHandler OnTextServerManagerInterfaceRemoved(Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(TextServerManager.Singleton, "interface_removed", action, oneShot, deferred);

        public static SignalHandler OnThemeDBFallbackChanged(Action action, bool oneShot = false, bool deferred = false) =>
            On(ThemeDB.Singleton, "fallback_changed", action, oneShot, deferred);

        public static SignalHandler OnChanged(this TileData target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "changed", action, oneShot, deferred);

        public static SignalHandler OnChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "changed", action, oneShot, deferred);

        public static SignalHandler OnTimeout(this Timer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "timeout", action, oneShot, deferred);

        public static SignalHandler OnPressed(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "pressed", action, oneShot, deferred);

        public static SignalHandler OnReleased(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "released", action, oneShot, deferred);

        public static SignalHandler OnButtonClicked(this Tree target, Action<TreeItem, System.Int32, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "button_clicked", action, oneShot, deferred);

        public static SignalHandler OnCellSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "cell_selected", action, oneShot, deferred);

        public static SignalHandler OnCheckPropagatedToItem(this Tree target, Action<TreeItem, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "check_propagated_to_item", action, oneShot, deferred);

        public static SignalHandler OnColumnTitleClicked(this Tree target, Action<System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "column_title_clicked", action, oneShot, deferred);

        public static SignalHandler OnCustomItemClicked(this Tree target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "custom_item_clicked", action, oneShot, deferred);

        public static SignalHandler OnCustomPopupEdited(this Tree target, Action<System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, "custom_popup_edited", action, oneShot, deferred);

        public static SignalHandler OnEmptyClicked(this Tree target, Action<Godot.Vector2, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "empty_clicked", action, oneShot, deferred);

        public static SignalHandler OnItemActivated(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_activated", action, oneShot, deferred);

        public static SignalHandler OnItemCollapsed(this Tree target, Action<TreeItem> action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_collapsed", action, oneShot, deferred);

        public static SignalHandler OnItemCustomButtonPressed(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_custom_button_pressed", action, oneShot, deferred);

        public static SignalHandler OnItemDoubleClicked(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_double_clicked", action, oneShot, deferred);

        public static SignalHandler OnItemEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_edited", action, oneShot, deferred);

        public static SignalHandler OnItemMouseSelected(this Tree target, Action<Godot.Vector2, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_mouse_selected", action, oneShot, deferred);

        public static SignalHandler OnItemSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_selected", action, oneShot, deferred);

        public static SignalHandler OnMultiSelected(this Tree target, Action<TreeItem, System.Int32, System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, "multi_selected", action, oneShot, deferred);

        public static SignalHandler OnNothingSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "nothing_selected", action, oneShot, deferred);

        public static SignalHandler OnFinished(this Tween target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "finished", action, oneShot, deferred);

        public static SignalHandler OnLoopFinished(this Tween target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "loop_finished", action, oneShot, deferred);

        public static SignalHandler OnStepFinished(this Tween target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "step_finished", action, oneShot, deferred);

        public static SignalHandler OnFinished(this Tweener target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "finished", action, oneShot, deferred);

        public static SignalHandler OnVersionChanged(this UndoRedo target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "version_changed", action, oneShot, deferred);

        public static SignalHandler OnFinished(this VideoStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "finished", action, oneShot, deferred);

        public static SignalHandler OnGuiFocusChanged(this Viewport target, Action<Control> action, bool oneShot = false, bool deferred = false) =>
            On(target, "gui_focus_changed", action, oneShot, deferred);

        public static SignalHandler OnSizeChanged(this Viewport target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "size_changed", action, oneShot, deferred);

        public static SignalHandler OnScreenEntered(this VisibleOnScreenNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "screen_entered", action, oneShot, deferred);

        public static SignalHandler OnScreenExited(this VisibleOnScreenNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "screen_exited", action, oneShot, deferred);

        public static SignalHandler OnScreenEntered(this VisibleOnScreenNotifier3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "screen_entered", action, oneShot, deferred);

        public static SignalHandler OnScreenExited(this VisibleOnScreenNotifier3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "screen_exited", action, oneShot, deferred);

        public static SignalHandler OnEditorRefreshRequest(this VisualShaderNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "editor_refresh_request", action, oneShot, deferred);

        public static SignalHandler OnInputTypeChanged(this VisualShaderNodeInput target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "input_type_changed", action, oneShot, deferred);

        public static SignalHandler OnDataChannelReceived(this WebRTCPeerConnection target, Action<WebRTCDataChannel> action, bool oneShot = false, bool deferred = false) =>
            On(target, "data_channel_received", action, oneShot, deferred);

        public static SignalHandler OnIceCandidateCreated(this WebRTCPeerConnection target, Action<System.String, System.Int32, System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, "ice_candidate_created", action, oneShot, deferred);

        public static SignalHandler OnSessionDescriptionCreated(this WebRTCPeerConnection target, Action<System.String, System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, "session_description_created", action, oneShot, deferred);

        public static SignalHandler OnReferenceSpaceReset(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "reference_space_reset", action, oneShot, deferred);

        public static SignalHandler OnSelect(this WebXRInterface target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "select", action, oneShot, deferred);

        public static SignalHandler OnSelectend(this WebXRInterface target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "selectend", action, oneShot, deferred);

        public static SignalHandler OnSelectstart(this WebXRInterface target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "selectstart", action, oneShot, deferred);

        public static SignalHandler OnSessionEnded(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "session_ended", action, oneShot, deferred);

        public static SignalHandler OnSessionFailed(this WebXRInterface target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, "session_failed", action, oneShot, deferred);

        public static SignalHandler OnSessionStarted(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "session_started", action, oneShot, deferred);

        public static SignalHandler OnSessionSupported(this WebXRInterface target, Action<System.String, System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, "session_supported", action, oneShot, deferred);

        public static SignalHandler OnSqueeze(this WebXRInterface target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "squeeze", action, oneShot, deferred);

        public static SignalHandler OnSqueezeend(this WebXRInterface target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "squeezeend", action, oneShot, deferred);

        public static SignalHandler OnSqueezestart(this WebXRInterface target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "squeezestart", action, oneShot, deferred);

        public static SignalHandler OnVisibilityStateChanged(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "visibility_state_changed", action, oneShot, deferred);

        public static SignalHandler OnAboutToPopup(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "about_to_popup", action, oneShot, deferred);

        public static SignalHandler OnCloseRequested(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "close_requested", action, oneShot, deferred);

        public static SignalHandler OnFilesDropped(this Window target, Action<System.String[]> action, bool oneShot = false, bool deferred = false) =>
            On(target, "files_dropped", action, oneShot, deferred);

        public static SignalHandler OnFocusEntered(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "focus_entered", action, oneShot, deferred);

        public static SignalHandler OnFocusExited(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "focus_exited", action, oneShot, deferred);

        public static SignalHandler OnGoBackRequested(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "go_back_requested", action, oneShot, deferred);

        public static SignalHandler OnMouseEntered(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandler OnMouseExited(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandler OnThemeChanged(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "theme_changed", action, oneShot, deferred);

        public static SignalHandler OnTitlebarChanged(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "titlebar_changed", action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "visibility_changed", action, oneShot, deferred);

        public static SignalHandler OnWindowInput(this Window target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            On(target, "window_input", action, oneShot, deferred);

        public static SignalHandler OnButtonPressed(this XRController3D target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, "button_pressed", action, oneShot, deferred);

        public static SignalHandler OnButtonReleased(this XRController3D target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, "button_released", action, oneShot, deferred);

        public static SignalHandler OnInputAxisChanged(this XRController3D target, Action<System.String, Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "input_axis_changed", action, oneShot, deferred);

        public static SignalHandler OnInputValueChanged(this XRController3D target, Action<System.String, System.Single> action, bool oneShot = false, bool deferred = false) =>
            On(target, "input_value_changed", action, oneShot, deferred);

        public static SignalHandler OnPlayAreaChanged(this XRInterface target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, "play_area_changed", action, oneShot, deferred);

        public static SignalHandler OnButtonPressed(this XRPositionalTracker target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, "button_pressed", action, oneShot, deferred);

        public static SignalHandler OnButtonReleased(this XRPositionalTracker target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, "button_released", action, oneShot, deferred);

        public static SignalHandler OnInputAxisChanged(this XRPositionalTracker target, Action<System.String, Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "input_axis_changed", action, oneShot, deferred);

        public static SignalHandler OnInputValueChanged(this XRPositionalTracker target, Action<System.String, System.Single> action, bool oneShot = false, bool deferred = false) =>
            On(target, "input_value_changed", action, oneShot, deferred);

        public static SignalHandler OnPoseChanged(this XRPositionalTracker target, Action<XRPose> action, bool oneShot = false, bool deferred = false) =>
            On(target, "pose_changed", action, oneShot, deferred);

        public static SignalHandler OnProfileChanged(this XRPositionalTracker target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, "profile_changed", action, oneShot, deferred);

        public static SignalHandler OnXRServerInterfaceAdded(Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(XRServer.Singleton, "interface_added", action, oneShot, deferred);

        public static SignalHandler OnXRServerInterfaceRemoved(Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(XRServer.Singleton, "interface_removed", action, oneShot, deferred);

        public static SignalHandler OnXRServerTrackerAdded(Action<Godot.StringName, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(XRServer.Singleton, "tracker_added", action, oneShot, deferred);

        public static SignalHandler OnXRServerTrackerRemoved(Action<Godot.StringName, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(XRServer.Singleton, "tracker_removed", action, oneShot, deferred);

        public static SignalHandler OnXRServerTrackerUpdated(Action<Godot.StringName, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(XRServer.Singleton, "tracker_updated", action, oneShot, deferred);
    }
}