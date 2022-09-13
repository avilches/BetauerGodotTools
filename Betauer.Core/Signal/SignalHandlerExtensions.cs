using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;
using Environment = Godot.Environment;

namespace Betauer.Signal {
    public static partial class SignalExtensions {

        public static SignalManager SignalFactory => DefaultSignalManager.Instance;
        
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
      
        public static SignalHandler OnConfirmed(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "confirmed", action, oneShot, deferred);

        public static SignalHandler OnCustomAction(this AcceptDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "custom_action", action, oneShot, deferred);

        public static SignalHandler OnAnimationFinished(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler OnAnimationFinished(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler OnTracksChanged(this Animation target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "tracks_changed", action, oneShot, deferred);

        public static SignalHandler OnRemovedFromGraph(this AnimationNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "removed_from_graph", action, oneShot, deferred);

        public static SignalHandler OnTreeChanged(this AnimationNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "tree_changed", action, oneShot, deferred);

        public static SignalHandler OnTrianglesUpdated(this AnimationNodeBlendSpace2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "triangles_updated", action, oneShot, deferred);

        public static SignalHandler OnAdvanceConditionChanged(this AnimationNodeStateMachineTransition target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "advance_condition_changed", action, oneShot, deferred);

        public static SignalHandler OnAnimationChanged(this AnimationPlayer target, Action<string, string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_changed", action, oneShot, deferred);

        public static SignalHandler OnAnimationFinished(this AnimationPlayer target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandler OnAnimationStarted(this AnimationPlayer target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "animation_started", action, oneShot, deferred);

        public static SignalHandler OnCachesCleared(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "caches_cleared", action, oneShot, deferred);

        public static SignalHandler OnAreaEntered(this Area target, Action<Area> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_entered", action, oneShot, deferred);

        public static SignalHandler OnAreaExited(this Area target, Action<Area> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_exited", action, oneShot, deferred);

        public static SignalHandler OnAreaShapeEntered(this Area target, Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnAreaShapeExited(this Area target, Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this Area target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this Area target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this Area target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this Area target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnAreaEntered(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_entered", action, oneShot, deferred);

        public static SignalHandler OnAreaExited(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_exited", action, oneShot, deferred);

        public static SignalHandler OnAreaShapeEntered(this Area2D target, Action<Area2D, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnAreaShapeExited(this Area2D target, Action<Area2D, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "area_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this Area2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this Area2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this Area2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this Area2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnMeshUpdated(this ARVRAnchor target, Action<Mesh> action, bool oneShot = false, bool deferred = false) =>
            On(target, "mesh_updated", action, oneShot, deferred);

        public static SignalHandler OnButtonPressed(this ARVRController target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "button_pressed", action, oneShot, deferred);

        public static SignalHandler OnButtonRelease(this ARVRController target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "button_release", action, oneShot, deferred);

        public static SignalHandler OnMeshUpdated(this ARVRController target, Action<Mesh> action, bool oneShot = false, bool deferred = false) =>
            On(target, "mesh_updated", action, oneShot, deferred);

        public static SignalHandler OnARVRServerInterfaceAdded(Action<string> action, bool oneShot = false, bool deferred = false) =>
            On(ARVRServer.Singleton, "interface_added", action, oneShot, deferred);

        public static SignalHandler OnARVRServerInterfaceRemoved(Action<string> action, bool oneShot = false, bool deferred = false) =>
            On(ARVRServer.Singleton, "interface_removed", action, oneShot, deferred);

        public static SignalHandler OnARVRServerTrackerAdded(Action<int, string, int> action, bool oneShot = false, bool deferred = false) =>
            On(ARVRServer.Singleton, "tracker_added", action, oneShot, deferred);

        public static SignalHandler OnARVRServerTrackerRemoved(Action<int, string, int> action, bool oneShot = false, bool deferred = false) =>
            On(ARVRServer.Singleton, "tracker_removed", action, oneShot, deferred);

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

        public static SignalHandler OnToggled(this BaseButton target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            On(target, "toggled", action, oneShot, deferred);

        public static SignalHandler OnPressed(this ButtonGroup target, Action<Object> action, bool oneShot = false, bool deferred = false) =>
            On(target, "pressed", action, oneShot, deferred);

        public static SignalHandler OnCameraServerCameraFeedAdded(Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(CameraServer.Singleton, "camera_feed_added", action, oneShot, deferred);

        public static SignalHandler OnCameraServerCameraFeedRemoved(Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(CameraServer.Singleton, "camera_feed_removed", action, oneShot, deferred);

        public static SignalHandler OnDraw(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "draw", action, oneShot, deferred);

        public static SignalHandler OnHide(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "hide", action, oneShot, deferred);

        public static SignalHandler OnItemRectChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_rect_changed", action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "visibility_changed", action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "visibility_changed", action, oneShot, deferred);

        public static SignalHandler OnInputEvent(this CollisionObject target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "input_event", action, oneShot, deferred);

        public static SignalHandler OnMouseEntered(this CollisionObject target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandler OnMouseExited(this CollisionObject target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandler OnInputEvent(this CollisionObject2D target, Action<InputEvent, int, Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "input_event", action, oneShot, deferred);

        public static SignalHandler OnMouseEntered(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandler OnMouseExited(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandler OnColorChanged(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            On(target, "color_changed", action, oneShot, deferred);

        public static SignalHandler OnPresetAdded(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            On(target, "preset_added", action, oneShot, deferred);

        public static SignalHandler OnPresetRemoved(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            On(target, "preset_removed", action, oneShot, deferred);

        public static SignalHandler OnColorChanged(this ColorPickerButton target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            On(target, "color_changed", action, oneShot, deferred);

        public static SignalHandler OnPickerCreated(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "picker_created", action, oneShot, deferred);

        public static SignalHandler OnPopupClosed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "popup_closed", action, oneShot, deferred);

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

        public static SignalHandler OnModalClosed(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "modal_closed", action, oneShot, deferred);

        public static SignalHandler OnMouseEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandler OnMouseExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandler OnResized(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "resized", action, oneShot, deferred);

        public static SignalHandler OnSizeFlagsChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "size_flags_changed", action, oneShot, deferred);

        public static SignalHandler OnRangeChanged(this Curve target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "range_changed", action, oneShot, deferred);

        public static SignalHandler OnDirSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "dir_selected", action, oneShot, deferred);

        public static SignalHandler OnFileSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "file_selected", action, oneShot, deferred);

        public static SignalHandler OnFilesSelected(this FileDialog target, Action<string[]> action, bool oneShot = false, bool deferred = false) =>
            On(target, "files_selected", action, oneShot, deferred);

        public static SignalHandler OnCompleted(this GDScriptFunctionState target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            On(target, "completed", action, oneShot, deferred);

        public static SignalHandler OnBeginNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "_begin_node_move", action, oneShot, deferred);

        public static SignalHandler OnConnectionFromEmpty(this GraphEdit target, Action<Vector2, string, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_from_empty", action, oneShot, deferred);

        public static SignalHandler OnConnectionRequest(this GraphEdit target, Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_request", action, oneShot, deferred);

        public static SignalHandler OnConnectionToEmpty(this GraphEdit target, Action<string, int, Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_to_empty", action, oneShot, deferred);

        public static SignalHandler OnCopyNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "copy_nodes_request", action, oneShot, deferred);

        public static SignalHandler OnDeleteNodesRequest(this GraphEdit target, Action<Godot.Collections.Array> action, bool oneShot = false, bool deferred = false) =>
            On(target, "delete_nodes_request", action, oneShot, deferred);

        public static SignalHandler OnDisconnectionRequest(this GraphEdit target, Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "disconnection_request", action, oneShot, deferred);

        public static SignalHandler OnDuplicateNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "duplicate_nodes_request", action, oneShot, deferred);

        public static SignalHandler OnEndNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "_end_node_move", action, oneShot, deferred);

        public static SignalHandler OnNodeSelected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "node_selected", action, oneShot, deferred);

        public static SignalHandler OnNodeUnselected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "node_unselected", action, oneShot, deferred);

        public static SignalHandler OnPasteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "paste_nodes_request", action, oneShot, deferred);

        public static SignalHandler OnPopupRequest(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "popup_request", action, oneShot, deferred);

        public static SignalHandler OnScrollOffsetChanged(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "scroll_offset_changed", action, oneShot, deferred);

        public static SignalHandler OnCloseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "close_request", action, oneShot, deferred);

        public static SignalHandler OnDragged(this GraphNode target, Action<Vector2, Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "dragged", action, oneShot, deferred);

        public static SignalHandler OnOffsetChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "offset_changed", action, oneShot, deferred);

        public static SignalHandler OnRaiseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "raise_request", action, oneShot, deferred);

        public static SignalHandler OnResizeRequest(this GraphNode target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "resize_request", action, oneShot, deferred);

        public static SignalHandler OnSlotUpdated(this GraphNode target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "slot_updated", action, oneShot, deferred);

        public static SignalHandler OnCellSizeChanged(this GridMap target, Action<Vector3> action, bool oneShot = false, bool deferred = false) =>
            On(target, "cell_size_changed", action, oneShot, deferred);

        public static SignalHandler OnRequestCompleted(this HTTPRequest target, Action<byte[], string[], int, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "request_completed", action, oneShot, deferred);

        public static SignalHandler OnInputJoyConnectionChanged(Action<bool, int> action, bool oneShot = false, bool deferred = false) =>
            On(Input.Singleton, "joy_connection_changed", action, oneShot, deferred);

        public static SignalHandler OnItemActivated(this ItemList target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_activated", action, oneShot, deferred);

        public static SignalHandler OnItemRmbSelected(this ItemList target, Action<Vector2, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_rmb_selected", action, oneShot, deferred);

        public static SignalHandler OnItemSelected(this ItemList target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_selected", action, oneShot, deferred);

        public static SignalHandler OnMultiSelected(this ItemList target, Action<int, bool> action, bool oneShot = false, bool deferred = false) =>
            On(target, "multi_selected", action, oneShot, deferred);

        public static SignalHandler OnNothingSelected(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "nothing_selected", action, oneShot, deferred);

        public static SignalHandler OnRmbClicked(this ItemList target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "rmb_clicked", action, oneShot, deferred);

        public static SignalHandler OnJavaScriptPwaUpdateAvailable(Action action, bool oneShot = false, bool deferred = false) =>
            On(JavaScript.Singleton, "pwa_update_available", action, oneShot, deferred);

        public static SignalHandler OnTextChanged(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "text_changed", action, oneShot, deferred);

        public static SignalHandler OnTextChangeRejected(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "text_change_rejected", action, oneShot, deferred);

        public static SignalHandler OnTextEntered(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "text_entered", action, oneShot, deferred);

        public static SignalHandler OnRequestPermissionsResult(this MainLoop target, Action<bool, string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "on_request_permissions_result", action, oneShot, deferred);

        public static SignalHandler OnAboutToShow(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "about_to_show", action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler OnConnectedToServer(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "connected_to_server", action, oneShot, deferred);

        public static SignalHandler OnConnectionFailed(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_failed", action, oneShot, deferred);

        public static SignalHandler OnNetworkPeerConnected(this MultiplayerAPI target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "network_peer_connected", action, oneShot, deferred);

        public static SignalHandler OnNetworkPeerDisconnected(this MultiplayerAPI target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "network_peer_disconnected", action, oneShot, deferred);

        public static SignalHandler OnNetworkPeerPacket(this MultiplayerAPI target, Action<int, byte[]> action, bool oneShot = false, bool deferred = false) =>
            On(target, "network_peer_packet", action, oneShot, deferred);

        public static SignalHandler OnServerDisconnected(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "server_disconnected", action, oneShot, deferred);

        public static SignalHandler OnMapChanged(this Navigation target, Action<RID> action, bool oneShot = false, bool deferred = false) =>
            On(target, "map_changed", action, oneShot, deferred);

        public static SignalHandler OnNavigation2DServerMapChanged(Action<RID> action, bool oneShot = false, bool deferred = false) =>
            On(Navigation2DServer.Singleton, "map_changed", action, oneShot, deferred);

        public static SignalHandler OnNavigationFinished(this NavigationAgent target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "navigation_finished", action, oneShot, deferred);

        public static SignalHandler OnPathChanged(this NavigationAgent target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "path_changed", action, oneShot, deferred);

        public static SignalHandler OnTargetReached(this NavigationAgent target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "target_reached", action, oneShot, deferred);

        public static SignalHandler OnVelocityComputed(this NavigationAgent target, Action<Vector3> action, bool oneShot = false, bool deferred = false) =>
            On(target, "velocity_computed", action, oneShot, deferred);

        public static SignalHandler OnNavigationFinished(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "navigation_finished", action, oneShot, deferred);

        public static SignalHandler OnPathChanged(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "path_changed", action, oneShot, deferred);

        public static SignalHandler OnTargetReached(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "target_reached", action, oneShot, deferred);

        public static SignalHandler OnVelocityComputed(this NavigationAgent2D target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "velocity_computed", action, oneShot, deferred);

        public static SignalHandler OnBakeFinished(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "bake_finished", action, oneShot, deferred);

        public static SignalHandler OnNavigationMeshChanged(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "navigation_mesh_changed", action, oneShot, deferred);

        public static SignalHandler OnNavigationServerMapChanged(Action<RID> action, bool oneShot = false, bool deferred = false) =>
            On(NavigationServer.Singleton, "map_changed", action, oneShot, deferred);

        public static SignalHandler OnPacketGenerated(this NetworkedMultiplayerCustom target, Action<byte[], int, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "packet_generated", action, oneShot, deferred);

        public static SignalHandler OnConnectionFailed(this NetworkedMultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_failed", action, oneShot, deferred);

        public static SignalHandler OnConnectionSucceeded(this NetworkedMultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_succeeded", action, oneShot, deferred);

        public static SignalHandler OnPeerConnected(this NetworkedMultiplayerPeer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "peer_connected", action, oneShot, deferred);

        public static SignalHandler OnPeerDisconnected(this NetworkedMultiplayerPeer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "peer_disconnected", action, oneShot, deferred);

        public static SignalHandler OnServerDisconnected(this NetworkedMultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "server_disconnected", action, oneShot, deferred);

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

        public static SignalHandler OnScriptChanged(this Object target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "script_changed", action, oneShot, deferred);

        public static SignalHandler OnItemFocused(this OptionButton target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_focused", action, oneShot, deferred);

        public static SignalHandler OnItemSelected(this OptionButton target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_selected", action, oneShot, deferred);

        public static SignalHandler OnCurveChanged(this Path target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "curve_changed", action, oneShot, deferred);

        public static SignalHandler OnAboutToShow(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "about_to_show", action, oneShot, deferred);

        public static SignalHandler OnPopupHide(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "popup_hide", action, oneShot, deferred);

        public static SignalHandler OnIdFocused(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "id_focused", action, oneShot, deferred);

        public static SignalHandler OnIdPressed(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "id_pressed", action, oneShot, deferred);

        public static SignalHandler OnIndexPressed(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "index_pressed", action, oneShot, deferred);

        public static SignalHandler OnProjectSettingsProjectSettingsChanged(Action action, bool oneShot = false, bool deferred = false) =>
            On(ProjectSettings.Singleton, "project_settings_changed", action, oneShot, deferred);

        public static SignalHandler OnBroadcast(this ProximityGroup target, Action<string, Godot.Collections.Array> action, bool oneShot = false, bool deferred = false) =>
            On(target, "broadcast", action, oneShot, deferred);

        public static SignalHandler OnChanged(this Range target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "changed", action, oneShot, deferred);

        public static SignalHandler OnValueChanged(this Range target, Action<float> action, bool oneShot = false, bool deferred = false) =>
            On(target, "value_changed", action, oneShot, deferred);

        public static SignalHandler OnChanged(this Resource target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "changed", action, oneShot, deferred);

        public static SignalHandler OnMetaClicked(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            On(target, "meta_clicked", action, oneShot, deferred);

        public static SignalHandler OnMetaHoverEnded(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            On(target, "meta_hover_ended", action, oneShot, deferred);

        public static SignalHandler OnMetaHoverStarted(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            On(target, "meta_hover_started", action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this RigidBody target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this RigidBody target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this RigidBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this RigidBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnSleepingStateChanged(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "sleeping_state_changed", action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this RigidBody2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this RigidBody2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnSleepingStateChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "sleeping_state_changed", action, oneShot, deferred);

        public static SignalHandler OnConnectedToServer(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "connected_to_server", action, oneShot, deferred);

        public static SignalHandler OnConnectionFailed(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_failed", action, oneShot, deferred);

        public static SignalHandler OnFilesDropped(this SceneTree target, Action<string[], int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "files_dropped", action, oneShot, deferred);

        public static SignalHandler OnGlobalMenuAction(this SceneTree target, Action<object, object> action, bool oneShot = false, bool deferred = false) =>
            On(target, "global_menu_action", action, oneShot, deferred);

        public static SignalHandler OnIdleFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "idle_frame", action, oneShot, deferred);

        public static SignalHandler OnNetworkPeerConnected(this SceneTree target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "network_peer_connected", action, oneShot, deferred);

        public static SignalHandler OnNetworkPeerDisconnected(this SceneTree target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "network_peer_disconnected", action, oneShot, deferred);

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

        public static SignalHandler OnScreenResized(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "screen_resized", action, oneShot, deferred);

        public static SignalHandler OnServerDisconnected(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "server_disconnected", action, oneShot, deferred);

        public static SignalHandler OnTreeChanged(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "tree_changed", action, oneShot, deferred);

        public static SignalHandler OnTimeout(this SceneTreeTimer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "timeout", action, oneShot, deferred);

        public static SignalHandler OnFinished(this SceneTreeTween target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "finished", action, oneShot, deferred);

        public static SignalHandler OnLoopFinished(this SceneTreeTween target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "loop_finished", action, oneShot, deferred);

        public static SignalHandler OnStepFinished(this SceneTreeTween target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "step_finished", action, oneShot, deferred);

        public static SignalHandler OnScrolling(this ScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "scrolling", action, oneShot, deferred);

        public static SignalHandler OnScrollEnded(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "scroll_ended", action, oneShot, deferred);

        public static SignalHandler OnScrollStarted(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "scroll_started", action, oneShot, deferred);

        public static SignalHandler OnSkeletonUpdated(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "skeleton_updated", action, oneShot, deferred);

        public static SignalHandler OnBoneSetupChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "bone_setup_changed", action, oneShot, deferred);

        public static SignalHandler OnDragEnded(this Slider target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            On(target, "drag_ended", action, oneShot, deferred);

        public static SignalHandler OnDragStarted(this Slider target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "drag_started", action, oneShot, deferred);

        public static SignalHandler OnGameplayEntered(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "gameplay_entered", action, oneShot, deferred);

        public static SignalHandler OnGameplayExited(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "gameplay_exited", action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "visibility_changed", action, oneShot, deferred);

        public static SignalHandler OnDragged(this SplitContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "dragged", action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this StyleBoxTexture target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler OnPrePopupPressed(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "pre_popup_pressed", action, oneShot, deferred);

        public static SignalHandler OnTabChanged(this TabContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_changed", action, oneShot, deferred);

        public static SignalHandler OnTabSelected(this TabContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_selected", action, oneShot, deferred);

        public static SignalHandler OnRepositionActiveTabRequest(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "reposition_active_tab_request", action, oneShot, deferred);

        public static SignalHandler OnRightButtonPressed(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "right_button_pressed", action, oneShot, deferred);

        public static SignalHandler OnTabChanged(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_changed", action, oneShot, deferred);

        public static SignalHandler OnTabClicked(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_clicked", action, oneShot, deferred);

        public static SignalHandler OnTabClose(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_close", action, oneShot, deferred);

        public static SignalHandler OnTabHover(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tab_hover", action, oneShot, deferred);

        public static SignalHandler OnBreakpointToggled(this TextEdit target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "breakpoint_toggled", action, oneShot, deferred);

        public static SignalHandler OnCursorChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "cursor_changed", action, oneShot, deferred);

        public static SignalHandler OnInfoClicked(this TextEdit target, Action<string, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "info_clicked", action, oneShot, deferred);

        public static SignalHandler OnRequestCompletion(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "request_completion", action, oneShot, deferred);

        public static SignalHandler OnSymbolLookup(this TextEdit target, Action<int, int, string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "symbol_lookup", action, oneShot, deferred);

        public static SignalHandler OnTextChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "text_changed", action, oneShot, deferred);

        public static SignalHandler OnSettingsChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "settings_changed", action, oneShot, deferred);

        public static SignalHandler OnTimeout(this Timer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "timeout", action, oneShot, deferred);

        public static SignalHandler OnPressed(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "pressed", action, oneShot, deferred);

        public static SignalHandler OnReleased(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "released", action, oneShot, deferred);

        public static SignalHandler OnButtonPressed(this Tree target, Action<int, int, TreeItem> action, bool oneShot = false, bool deferred = false) =>
            On(target, "button_pressed", action, oneShot, deferred);

        public static SignalHandler OnCellSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "cell_selected", action, oneShot, deferred);

        public static SignalHandler OnColumnTitlePressed(this Tree target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "column_title_pressed", action, oneShot, deferred);

        public static SignalHandler OnCustomPopupEdited(this Tree target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            On(target, "custom_popup_edited", action, oneShot, deferred);

        public static SignalHandler OnEmptyRmb(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "empty_rmb", action, oneShot, deferred);

        public static SignalHandler OnEmptyTreeRmbSelected(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "empty_tree_rmb_selected", action, oneShot, deferred);

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

        public static SignalHandler OnItemRmbEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_rmb_edited", action, oneShot, deferred);

        public static SignalHandler OnItemRmbSelected(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_rmb_selected", action, oneShot, deferred);

        public static SignalHandler OnItemSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "item_selected", action, oneShot, deferred);

        public static SignalHandler OnMultiSelected(this Tree target, Action<int, TreeItem, bool> action, bool oneShot = false, bool deferred = false) =>
            On(target, "multi_selected", action, oneShot, deferred);

        public static SignalHandler OnNothingSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "nothing_selected", action, oneShot, deferred);

        public static SignalHandler OnTweenAllCompleted(this Tween target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "tween_all_completed", action, oneShot, deferred);

        public static SignalHandler OnTweenCompleted(this Tween target, Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tween_completed", action, oneShot, deferred);

        public static SignalHandler OnTweenStarted(this Tween target, Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tween_started", action, oneShot, deferred);

        public static SignalHandler OnTweenStep(this Tween target, Action<Object, float, NodePath, Object> action, bool oneShot = false, bool deferred = false) =>
            On(target, "tween_step", action, oneShot, deferred);

        public static SignalHandler OnFinished(this Tweener target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "finished", action, oneShot, deferred);

        public static SignalHandler OnVersionChanged(this UndoRedo target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "version_changed", action, oneShot, deferred);

        public static SignalHandler OnFinished(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "finished", action, oneShot, deferred);

        public static SignalHandler OnGuiFocusChanged(this Viewport target, Action<Control> action, bool oneShot = false, bool deferred = false) =>
            On(target, "gui_focus_changed", action, oneShot, deferred);

        public static SignalHandler OnSizeChanged(this Viewport target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "size_changed", action, oneShot, deferred);

        public static SignalHandler OnCameraEntered(this VisibilityNotifier target, Action<Camera> action, bool oneShot = false, bool deferred = false) =>
            On(target, "camera_entered", action, oneShot, deferred);

        public static SignalHandler OnCameraExited(this VisibilityNotifier target, Action<Camera> action, bool oneShot = false, bool deferred = false) =>
            On(target, "camera_exited", action, oneShot, deferred);

        public static SignalHandler OnScreenEntered(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "screen_entered", action, oneShot, deferred);

        public static SignalHandler OnScreenExited(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "screen_exited", action, oneShot, deferred);

        public static SignalHandler OnScreenEntered(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "screen_entered", action, oneShot, deferred);

        public static SignalHandler OnScreenExited(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "screen_exited", action, oneShot, deferred);

        public static SignalHandler OnViewportEntered(this VisibilityNotifier2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) =>
            On(target, "viewport_entered", action, oneShot, deferred);

        public static SignalHandler OnViewportExited(this VisibilityNotifier2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) =>
            On(target, "viewport_exited", action, oneShot, deferred);

        public static SignalHandler OnNodePortsChanged(this VisualScript target, Action<string, int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "node_ports_changed", action, oneShot, deferred);

        public static SignalHandler OnPortsChanged(this VisualScriptNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "ports_changed", action, oneShot, deferred);

        public static SignalHandler OnVisualServerFramePostDraw(Action action, bool oneShot = false, bool deferred = false) =>
            On(VisualServer.Singleton, "frame_post_draw", action, oneShot, deferred);

        public static SignalHandler OnVisualServerFramePreDraw(Action action, bool oneShot = false, bool deferred = false) =>
            On(VisualServer.Singleton, "frame_pre_draw", action, oneShot, deferred);

        public static SignalHandler OnEditorRefreshRequest(this VisualShaderNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "editor_refresh_request", action, oneShot, deferred);

        public static SignalHandler OnInputTypeChanged(this VisualShaderNodeInput target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "input_type_changed", action, oneShot, deferred);

        public static SignalHandler OnDataChannelReceived(this WebRTCPeerConnection target, Action<Object> action, bool oneShot = false, bool deferred = false) =>
            On(target, "data_channel_received", action, oneShot, deferred);

        public static SignalHandler OnIceCandidateCreated(this WebRTCPeerConnection target, Action<int, string, string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "ice_candidate_created", action, oneShot, deferred);

        public static SignalHandler OnSessionDescriptionCreated(this WebRTCPeerConnection target, Action<string, string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "session_description_created", action, oneShot, deferred);

        public static SignalHandler OnConnectionClosed(this WebSocketClient target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_closed", action, oneShot, deferred);

        public static SignalHandler OnConnectionError(this WebSocketClient target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_error", action, oneShot, deferred);

        public static SignalHandler OnConnectionEstablished(this WebSocketClient target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "connection_established", action, oneShot, deferred);

        public static SignalHandler OnDataReceived(this WebSocketClient target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "data_received", action, oneShot, deferred);

        public static SignalHandler OnServerCloseRequest(this WebSocketClient target, Action<int, string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "server_close_request", action, oneShot, deferred);

        public static SignalHandler OnPeerPacket(this WebSocketMultiplayerPeer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "peer_packet", action, oneShot, deferred);

        public static SignalHandler OnClientCloseRequest(this WebSocketServer target, Action<int, int, string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "client_close_request", action, oneShot, deferred);

        public static SignalHandler OnClientConnected(this WebSocketServer target, Action<int, string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "client_connected", action, oneShot, deferred);

        public static SignalHandler OnClientDisconnected(this WebSocketServer target, Action<int, bool> action, bool oneShot = false, bool deferred = false) =>
            On(target, "client_disconnected", action, oneShot, deferred);

        public static SignalHandler OnDataReceived(this WebSocketServer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "data_received", action, oneShot, deferred);

        public static SignalHandler OnReferenceSpaceReset(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "reference_space_reset", action, oneShot, deferred);

        public static SignalHandler OnSelect(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "select", action, oneShot, deferred);

        public static SignalHandler OnSelectend(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "selectend", action, oneShot, deferred);

        public static SignalHandler OnSelectstart(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "selectstart", action, oneShot, deferred);

        public static SignalHandler OnSessionEnded(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "session_ended", action, oneShot, deferred);

        public static SignalHandler OnSessionFailed(this WebXRInterface target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            On(target, "session_failed", action, oneShot, deferred);

        public static SignalHandler OnSessionStarted(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "session_started", action, oneShot, deferred);

        public static SignalHandler OnSessionSupported(this WebXRInterface target, Action<string, bool> action, bool oneShot = false, bool deferred = false) =>
            On(target, "session_supported", action, oneShot, deferred);

        public static SignalHandler OnSqueeze(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "squeeze", action, oneShot, deferred);

        public static SignalHandler OnSqueezeend(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "squeezeend", action, oneShot, deferred);

        public static SignalHandler OnSqueezestart(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            On(target, "squeezestart", action, oneShot, deferred);

        public static SignalHandler OnVisibilityStateChanged(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, "visibility_state_changed", action, oneShot, deferred);
    }
}