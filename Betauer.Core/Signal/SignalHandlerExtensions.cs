using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;
using Environment = Godot.Environment;

namespace Betauer.Signal {
    public static partial class SignalExtensions {
      
        public static SignalHandler OnConfirmed(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "confirmed", action, oneShot, deferred);

        public static SignalHandler OnCustomAction(this AcceptDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "custom_action", action, oneShot, deferred);

        public static SignalHandler OnAnimationFinished(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler OnAnimationFinished(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler OnTracksChanged(this Animation target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tracks_changed", action, oneShot, deferred);

        public static SignalHandler OnRemovedFromGraph(this AnimationNode target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "removed_from_graph", action, oneShot, deferred);

        public static SignalHandler OnTreeChanged(this AnimationNode target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tree_changed", action, oneShot, deferred);

        public static SignalHandler OnTrianglesUpdated(this AnimationNodeBlendSpace2D target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "triangles_updated", action, oneShot, deferred);

        public static SignalHandler OnAdvanceConditionChanged(this AnimationNodeStateMachineTransition target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "advance_condition_changed", action, oneShot, deferred);

        public static SignalHandler OnAnimationChanged(this AnimationPlayer target, Action<string, string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "animation_changed", action, oneShot, deferred);

        public static SignalHandler OnAnimationFinished(this AnimationPlayer target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "animation_finished", action, oneShot, deferred);

        public static SignalHandler OnAnimationStarted(this AnimationPlayer target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "animation_started", action, oneShot, deferred);

        public static SignalHandler OnCachesCleared(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "caches_cleared", action, oneShot, deferred);

        public static SignalHandler OnAreaEntered(this Area target, Action<Area> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "area_entered", action, oneShot, deferred);

        public static SignalHandler OnAreaExited(this Area target, Action<Area> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "area_exited", action, oneShot, deferred);

        public static SignalHandler OnAreaShapeEntered(this Area target, Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "area_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnAreaShapeExited(this Area target, Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "area_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this Area target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this Area target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this Area target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this Area target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnAreaEntered(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "area_entered", action, oneShot, deferred);

        public static SignalHandler OnAreaExited(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "area_exited", action, oneShot, deferred);

        public static SignalHandler OnAreaShapeEntered(this Area2D target, Action<Area2D, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "area_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnAreaShapeExited(this Area2D target, Action<Area2D, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "area_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this Area2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this Area2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this Area2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this Area2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnMeshUpdated(this ARVRAnchor target, Action<Mesh> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "mesh_updated", action, oneShot, deferred);

        public static SignalHandler OnButtonPressed(this ARVRController target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "button_pressed", action, oneShot, deferred);

        public static SignalHandler OnButtonRelease(this ARVRController target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "button_release", action, oneShot, deferred);

        public static SignalHandler OnMeshUpdated(this ARVRController target, Action<Mesh> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "mesh_updated", action, oneShot, deferred);

        public static SignalHandler OnARVRServerInterfaceAdded(Action<string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(ARVRServer.Singleton, "interface_added", action, oneShot, deferred);

        public static SignalHandler OnARVRServerInterfaceRemoved(Action<string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(ARVRServer.Singleton, "interface_removed", action, oneShot, deferred);

        public static SignalHandler OnARVRServerTrackerAdded(Action<int, string, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(ARVRServer.Singleton, "tracker_added", action, oneShot, deferred);

        public static SignalHandler OnARVRServerTrackerRemoved(Action<int, string, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(ARVRServer.Singleton, "tracker_removed", action, oneShot, deferred);

        public static SignalHandler OnAudioServerBusLayoutChanged(Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(AudioServer.Singleton, "bus_layout_changed", action, oneShot, deferred);

        public static SignalHandler OnFinished(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "finished", action, oneShot, deferred);

        public static SignalHandler OnFinished(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "finished", action, oneShot, deferred);

        public static SignalHandler OnFinished(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "finished", action, oneShot, deferred);

        public static SignalHandler OnButtonDown(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "button_down", action, oneShot, deferred);

        public static SignalHandler OnButtonUp(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "button_up", action, oneShot, deferred);

        public static SignalHandler OnPressed(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "pressed", action, oneShot, deferred);

        public static SignalHandler OnToggled(this BaseButton target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "toggled", action, oneShot, deferred);

        public static SignalHandler OnPressed(this ButtonGroup target, Action<Object> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "pressed", action, oneShot, deferred);

        public static SignalHandler OnCameraServerCameraFeedAdded(Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(CameraServer.Singleton, "camera_feed_added", action, oneShot, deferred);

        public static SignalHandler OnCameraServerCameraFeedRemoved(Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(CameraServer.Singleton, "camera_feed_removed", action, oneShot, deferred);

        public static SignalHandler OnDraw(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "draw", action, oneShot, deferred);

        public static SignalHandler OnHide(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "hide", action, oneShot, deferred);

        public static SignalHandler OnItemRectChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "item_rect_changed", action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "visibility_changed", action, oneShot, deferred);

        public static SignalHandler OnInputEvent(this CollisionObject target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "input_event", action, oneShot, deferred);

        public static SignalHandler OnMouseEntered(this CollisionObject target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandler OnMouseExited(this CollisionObject target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandler OnInputEvent(this CollisionObject2D target, Action<InputEvent, int, Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "input_event", action, oneShot, deferred);

        public static SignalHandler OnMouseEntered(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandler OnMouseExited(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandler OnColorChanged(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "color_changed", action, oneShot, deferred);

        public static SignalHandler OnPresetAdded(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "preset_added", action, oneShot, deferred);

        public static SignalHandler OnPresetRemoved(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "preset_removed", action, oneShot, deferred);

        public static SignalHandler OnColorChanged(this ColorPickerButton target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "color_changed", action, oneShot, deferred);

        public static SignalHandler OnPickerCreated(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "picker_created", action, oneShot, deferred);

        public static SignalHandler OnPopupClosed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "popup_closed", action, oneShot, deferred);

        public static SignalHandler OnSortChildren(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "sort_children", action, oneShot, deferred);

        public static SignalHandler OnFocusEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "focus_entered", action, oneShot, deferred);

        public static SignalHandler OnFocusExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "focus_exited", action, oneShot, deferred);

        public static SignalHandler OnGuiInput(this Control target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "gui_input", action, oneShot, deferred);

        public static SignalHandler OnMinimumSizeChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "minimum_size_changed", action, oneShot, deferred);

        public static SignalHandler OnModalClosed(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "modal_closed", action, oneShot, deferred);

        public static SignalHandler OnMouseEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "mouse_entered", action, oneShot, deferred);

        public static SignalHandler OnMouseExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "mouse_exited", action, oneShot, deferred);

        public static SignalHandler OnResized(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "resized", action, oneShot, deferred);

        public static SignalHandler OnSizeFlagsChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "size_flags_changed", action, oneShot, deferred);

        public static SignalHandler OnRangeChanged(this Curve target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "range_changed", action, oneShot, deferred);

        public static SignalHandler OnDirSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "dir_selected", action, oneShot, deferred);

        public static SignalHandler OnFileSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "file_selected", action, oneShot, deferred);

        public static SignalHandler OnFilesSelected(this FileDialog target, Action<string[]> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "files_selected", action, oneShot, deferred);

        public static SignalHandler OnCompleted(this GDScriptFunctionState target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "completed", action, oneShot, deferred);

        public static SignalHandler OnBeginNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "_begin_node_move", action, oneShot, deferred);

        public static SignalHandler OnConnectionFromEmpty(this GraphEdit target, Action<Vector2, string, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "connection_from_empty", action, oneShot, deferred);

        public static SignalHandler OnConnectionRequest(this GraphEdit target, Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "connection_request", action, oneShot, deferred);

        public static SignalHandler OnConnectionToEmpty(this GraphEdit target, Action<string, int, Vector2> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "connection_to_empty", action, oneShot, deferred);

        public static SignalHandler OnCopyNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "copy_nodes_request", action, oneShot, deferred);

        public static SignalHandler OnDeleteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "delete_nodes_request", action, oneShot, deferred);

        public static SignalHandler OnDisconnectionRequest(this GraphEdit target, Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "disconnection_request", action, oneShot, deferred);

        public static SignalHandler OnDuplicateNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "duplicate_nodes_request", action, oneShot, deferred);

        public static SignalHandler OnEndNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "_end_node_move", action, oneShot, deferred);

        public static SignalHandler OnNodeSelected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "node_selected", action, oneShot, deferred);

        public static SignalHandler OnNodeUnselected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "node_unselected", action, oneShot, deferred);

        public static SignalHandler OnPasteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "paste_nodes_request", action, oneShot, deferred);

        public static SignalHandler OnPopupRequest(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "popup_request", action, oneShot, deferred);

        public static SignalHandler OnScrollOffsetChanged(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "scroll_offset_changed", action, oneShot, deferred);

        public static SignalHandler OnCloseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "close_request", action, oneShot, deferred);

        public static SignalHandler OnDragged(this GraphNode target, Action<Vector2, Vector2> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "dragged", action, oneShot, deferred);

        public static SignalHandler OnOffsetChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "offset_changed", action, oneShot, deferred);

        public static SignalHandler OnRaiseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "raise_request", action, oneShot, deferred);

        public static SignalHandler OnResizeRequest(this GraphNode target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "resize_request", action, oneShot, deferred);

        public static SignalHandler OnSlotUpdated(this GraphNode target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "slot_updated", action, oneShot, deferred);

        public static SignalHandler OnCellSizeChanged(this GridMap target, Action<Vector3> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "cell_size_changed", action, oneShot, deferred);

        public static SignalHandler OnRequestCompleted(this HTTPRequest target, Action<byte[], string[], int, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "request_completed", action, oneShot, deferred);

        public static SignalHandler OnInputJoyConnectionChanged(Action<bool, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(Input.Singleton, "joy_connection_changed", action, oneShot, deferred);

        public static SignalHandler OnItemActivated(this ItemList target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "item_activated", action, oneShot, deferred);

        public static SignalHandler OnItemRmbSelected(this ItemList target, Action<Vector2, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "item_rmb_selected", action, oneShot, deferred);

        public static SignalHandler OnItemSelected(this ItemList target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "item_selected", action, oneShot, deferred);

        public static SignalHandler OnMultiSelected(this ItemList target, Action<int, bool> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "multi_selected", action, oneShot, deferred);

        public static SignalHandler OnNothingSelected(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "nothing_selected", action, oneShot, deferred);

        public static SignalHandler OnRmbClicked(this ItemList target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "rmb_clicked", action, oneShot, deferred);

        public static SignalHandler OnTextChanged(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "text_changed", action, oneShot, deferred);

        public static SignalHandler OnTextChangeRejected(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "text_change_rejected", action, oneShot, deferred);

        public static SignalHandler OnTextEntered(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "text_entered", action, oneShot, deferred);

        public static SignalHandler OnRequestPermissionsResult(this MainLoop target, Action<bool, string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "on_request_permissions_result", action, oneShot, deferred);

        public static SignalHandler OnAboutToShow(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "about_to_show", action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler OnConnectedToServer(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "connected_to_server", action, oneShot, deferred);

        public static SignalHandler OnConnectionFailed(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "connection_failed", action, oneShot, deferred);

        public static SignalHandler OnNetworkPeerConnected(this MultiplayerAPI target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "network_peer_connected", action, oneShot, deferred);

        public static SignalHandler OnNetworkPeerDisconnected(this MultiplayerAPI target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "network_peer_disconnected", action, oneShot, deferred);

        public static SignalHandler OnNetworkPeerPacket(this MultiplayerAPI target, Action<int, byte[]> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "network_peer_packet", action, oneShot, deferred);

        public static SignalHandler OnServerDisconnected(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "server_disconnected", action, oneShot, deferred);

        public static SignalHandler OnConnectionFailed(this NetworkedMultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "connection_failed", action, oneShot, deferred);

        public static SignalHandler OnConnectionSucceeded(this NetworkedMultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "connection_succeeded", action, oneShot, deferred);

        public static SignalHandler OnPeerConnected(this NetworkedMultiplayerPeer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "peer_connected", action, oneShot, deferred);

        public static SignalHandler OnPeerDisconnected(this NetworkedMultiplayerPeer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "peer_disconnected", action, oneShot, deferred);

        public static SignalHandler OnServerDisconnected(this NetworkedMultiplayerPeer target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "server_disconnected", action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler OnReady(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "ready", action, oneShot, deferred);

        public static SignalHandler OnRenamed(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "renamed", action, oneShot, deferred);

        public static SignalHandler OnTreeEntered(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tree_entered", action, oneShot, deferred);

        public static SignalHandler OnTreeExited(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tree_exited", action, oneShot, deferred);

        public static SignalHandler OnTreeExiting(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tree_exiting", action, oneShot, deferred);

        public static SignalHandler OnScriptChanged(this Object target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "script_changed", action, oneShot, deferred);

        public static SignalHandler OnItemFocused(this OptionButton target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "item_focused", action, oneShot, deferred);

        public static SignalHandler OnItemSelected(this OptionButton target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "item_selected", action, oneShot, deferred);

        public static SignalHandler OnCurveChanged(this Path target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "curve_changed", action, oneShot, deferred);

        public static SignalHandler OnAboutToShow(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "about_to_show", action, oneShot, deferred);

        public static SignalHandler OnPopupHide(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "popup_hide", action, oneShot, deferred);

        public static SignalHandler OnIdFocused(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "id_focused", action, oneShot, deferred);

        public static SignalHandler OnIdPressed(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "id_pressed", action, oneShot, deferred);

        public static SignalHandler OnIndexPressed(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "index_pressed", action, oneShot, deferred);

        public static SignalHandler OnBroadcast(this ProximityGroup target, Action<string, Godot.Collections.Array> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "broadcast", action, oneShot, deferred);

        public static SignalHandler OnChanged(this Range target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "changed", action, oneShot, deferred);

        public static SignalHandler OnValueChanged(this Range target, Action<float> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "value_changed", action, oneShot, deferred);

        public static SignalHandler OnChanged(this Resource target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "changed", action, oneShot, deferred);

        public static SignalHandler OnMetaClicked(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "meta_clicked", action, oneShot, deferred);

        public static SignalHandler OnMetaHoverEnded(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "meta_hover_ended", action, oneShot, deferred);

        public static SignalHandler OnMetaHoverStarted(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "meta_hover_started", action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this RigidBody target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this RigidBody target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this RigidBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this RigidBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnSleepingStateChanged(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "sleeping_state_changed", action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_exited", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this RigidBody2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_shape_entered", action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this RigidBody2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "body_shape_exited", action, oneShot, deferred);

        public static SignalHandler OnSleepingStateChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "sleeping_state_changed", action, oneShot, deferred);

        public static SignalHandler OnConnectedToServer(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "connected_to_server", action, oneShot, deferred);

        public static SignalHandler OnConnectionFailed(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "connection_failed", action, oneShot, deferred);

        public static SignalHandler OnFilesDropped(this SceneTree target, Action<string[], int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "files_dropped", action, oneShot, deferred);

        public static SignalHandler OnGlobalMenuAction(this SceneTree target, Action<object, object> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "global_menu_action", action, oneShot, deferred);

        public static SignalHandler OnIdleFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "idle_frame", action, oneShot, deferred);

        public static SignalHandler OnNetworkPeerConnected(this SceneTree target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "network_peer_connected", action, oneShot, deferred);

        public static SignalHandler OnNetworkPeerDisconnected(this SceneTree target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "network_peer_disconnected", action, oneShot, deferred);

        public static SignalHandler OnNodeAdded(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "node_added", action, oneShot, deferred);

        public static SignalHandler OnNodeConfigurationWarningChanged(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "node_configuration_warning_changed", action, oneShot, deferred);

        public static SignalHandler OnNodeRemoved(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "node_removed", action, oneShot, deferred);

        public static SignalHandler OnNodeRenamed(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "node_renamed", action, oneShot, deferred);

        public static SignalHandler OnPhysicsFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "physics_frame", action, oneShot, deferred);

        public static SignalHandler OnScreenResized(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "screen_resized", action, oneShot, deferred);

        public static SignalHandler OnServerDisconnected(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "server_disconnected", action, oneShot, deferred);

        public static SignalHandler OnTreeChanged(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tree_changed", action, oneShot, deferred);

        public static SignalHandler OnTimeout(this SceneTreeTimer target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "timeout", action, oneShot, deferred);

        public static SignalHandler OnScrolling(this ScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "scrolling", action, oneShot, deferred);

        public static SignalHandler OnScrollEnded(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "scroll_ended", action, oneShot, deferred);

        public static SignalHandler OnScrollStarted(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "scroll_started", action, oneShot, deferred);

        public static SignalHandler OnSkeletonUpdated(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "skeleton_updated", action, oneShot, deferred);

        public static SignalHandler OnBoneSetupChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "bone_setup_changed", action, oneShot, deferred);

        public static SignalHandler OnGameplayEntered(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "gameplay_entered", action, oneShot, deferred);

        public static SignalHandler OnGameplayExited(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "gameplay_exited", action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "visibility_changed", action, oneShot, deferred);

        public static SignalHandler OnDragged(this SplitContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "dragged", action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "frame_changed", action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this StyleBoxTexture target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "texture_changed", action, oneShot, deferred);

        public static SignalHandler OnPrePopupPressed(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "pre_popup_pressed", action, oneShot, deferred);

        public static SignalHandler OnTabChanged(this TabContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tab_changed", action, oneShot, deferred);

        public static SignalHandler OnTabSelected(this TabContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tab_selected", action, oneShot, deferred);

        public static SignalHandler OnRepositionActiveTabRequest(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "reposition_active_tab_request", action, oneShot, deferred);

        public static SignalHandler OnRightButtonPressed(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "right_button_pressed", action, oneShot, deferred);

        public static SignalHandler OnTabChanged(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tab_changed", action, oneShot, deferred);

        public static SignalHandler OnTabClicked(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tab_clicked", action, oneShot, deferred);

        public static SignalHandler OnTabClose(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tab_close", action, oneShot, deferred);

        public static SignalHandler OnTabHover(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tab_hover", action, oneShot, deferred);

        public static SignalHandler OnBreakpointToggled(this TextEdit target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "breakpoint_toggled", action, oneShot, deferred);

        public static SignalHandler OnCursorChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "cursor_changed", action, oneShot, deferred);

        public static SignalHandler OnInfoClicked(this TextEdit target, Action<string, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "info_clicked", action, oneShot, deferred);

        public static SignalHandler OnRequestCompletion(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "request_completion", action, oneShot, deferred);

        public static SignalHandler OnSymbolLookup(this TextEdit target, Action<int, int, string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "symbol_lookup", action, oneShot, deferred);

        public static SignalHandler OnTextChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "text_changed", action, oneShot, deferred);

        public static SignalHandler OnSettingsChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "settings_changed", action, oneShot, deferred);

        public static SignalHandler OnTimeout(this Timer target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "timeout", action, oneShot, deferred);

        public static SignalHandler OnPressed(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "pressed", action, oneShot, deferred);

        public static SignalHandler OnReleased(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "released", action, oneShot, deferred);

        public static SignalHandler OnButtonPressed(this Tree target, Action<int, int, TreeItem> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "button_pressed", action, oneShot, deferred);

        public static SignalHandler OnCellSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "cell_selected", action, oneShot, deferred);

        public static SignalHandler OnColumnTitlePressed(this Tree target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "column_title_pressed", action, oneShot, deferred);

        public static SignalHandler OnCustomPopupEdited(this Tree target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "custom_popup_edited", action, oneShot, deferred);

        public static SignalHandler OnEmptyRmb(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "empty_rmb", action, oneShot, deferred);

        public static SignalHandler OnEmptyTreeRmbSelected(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "empty_tree_rmb_selected", action, oneShot, deferred);

        public static SignalHandler OnItemActivated(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "item_activated", action, oneShot, deferred);

        public static SignalHandler OnItemCollapsed(this Tree target, Action<TreeItem> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "item_collapsed", action, oneShot, deferred);

        public static SignalHandler OnItemCustomButtonPressed(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "item_custom_button_pressed", action, oneShot, deferred);

        public static SignalHandler OnItemDoubleClicked(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "item_double_clicked", action, oneShot, deferred);

        public static SignalHandler OnItemEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "item_edited", action, oneShot, deferred);

        public static SignalHandler OnItemRmbEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "item_rmb_edited", action, oneShot, deferred);

        public static SignalHandler OnItemRmbSelected(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "item_rmb_selected", action, oneShot, deferred);

        public static SignalHandler OnItemSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "item_selected", action, oneShot, deferred);

        public static SignalHandler OnMultiSelected(this Tree target, Action<int, TreeItem, bool> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "multi_selected", action, oneShot, deferred);

        public static SignalHandler OnNothingSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "nothing_selected", action, oneShot, deferred);

        public static SignalHandler OnTweenAllCompleted(this Tween target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tween_all_completed", action, oneShot, deferred);

        public static SignalHandler OnTweenCompleted(this Tween target, Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tween_completed", action, oneShot, deferred);

        public static SignalHandler OnTweenStarted(this Tween target, Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tween_started", action, oneShot, deferred);

        public static SignalHandler OnTweenStep(this Tween target, Action<Object, float, NodePath, Object> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "tween_step", action, oneShot, deferred);

        public static SignalHandler OnVersionChanged(this UndoRedo target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "version_changed", action, oneShot, deferred);

        public static SignalHandler OnFinished(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "finished", action, oneShot, deferred);

        public static SignalHandler OnGuiFocusChanged(this Viewport target, Action<Control> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "gui_focus_changed", action, oneShot, deferred);

        public static SignalHandler OnSizeChanged(this Viewport target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "size_changed", action, oneShot, deferred);

        public static SignalHandler OnCameraEntered(this VisibilityNotifier target, Action<Camera> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "camera_entered", action, oneShot, deferred);

        public static SignalHandler OnCameraExited(this VisibilityNotifier target, Action<Camera> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "camera_exited", action, oneShot, deferred);

        public static SignalHandler OnScreenEntered(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "screen_entered", action, oneShot, deferred);

        public static SignalHandler OnScreenExited(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "screen_exited", action, oneShot, deferred);

        public static SignalHandler OnScreenEntered(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "screen_entered", action, oneShot, deferred);

        public static SignalHandler OnScreenExited(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "screen_exited", action, oneShot, deferred);

        public static SignalHandler OnViewportEntered(this VisibilityNotifier2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "viewport_entered", action, oneShot, deferred);

        public static SignalHandler OnViewportExited(this VisibilityNotifier2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "viewport_exited", action, oneShot, deferred);

        public static SignalHandler OnNodePortsChanged(this VisualScript target, Action<string, int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "node_ports_changed", action, oneShot, deferred);

        public static SignalHandler OnPortsChanged(this VisualScriptNode target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "ports_changed", action, oneShot, deferred);

        public static SignalHandler OnVisualServerFramePostDraw(Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(VisualServer.Singleton, "frame_post_draw", action, oneShot, deferred);

        public static SignalHandler OnVisualServerFramePreDraw(Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(VisualServer.Singleton, "frame_pre_draw", action, oneShot, deferred);

        public static SignalHandler OnEditorRefreshRequest(this VisualShaderNode target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "editor_refresh_request", action, oneShot, deferred);

        public static SignalHandler OnInputTypeChanged(this VisualShaderNodeInput target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "input_type_changed", action, oneShot, deferred);

        public static SignalHandler OnDataChannelReceived(this WebRTCPeerConnection target, Action<Object> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "data_channel_received", action, oneShot, deferred);

        public static SignalHandler OnIceCandidateCreated(this WebRTCPeerConnection target, Action<int, string, string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "ice_candidate_created", action, oneShot, deferred);

        public static SignalHandler OnSessionDescriptionCreated(this WebRTCPeerConnection target, Action<string, string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "session_description_created", action, oneShot, deferred);

        public static SignalHandler OnConnectionClosed(this WebSocketClient target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "connection_closed", action, oneShot, deferred);

        public static SignalHandler OnConnectionError(this WebSocketClient target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "connection_error", action, oneShot, deferred);

        public static SignalHandler OnConnectionEstablished(this WebSocketClient target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "connection_established", action, oneShot, deferred);

        public static SignalHandler OnDataReceived(this WebSocketClient target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "data_received", action, oneShot, deferred);

        public static SignalHandler OnServerCloseRequest(this WebSocketClient target, Action<int, string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "server_close_request", action, oneShot, deferred);

        public static SignalHandler OnPeerPacket(this WebSocketMultiplayerPeer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "peer_packet", action, oneShot, deferred);

        public static SignalHandler OnClientCloseRequest(this WebSocketServer target, Action<int, int, string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "client_close_request", action, oneShot, deferred);

        public static SignalHandler OnClientConnected(this WebSocketServer target, Action<int, string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "client_connected", action, oneShot, deferred);

        public static SignalHandler OnClientDisconnected(this WebSocketServer target, Action<int, bool> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "client_disconnected", action, oneShot, deferred);

        public static SignalHandler OnDataReceived(this WebSocketServer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "data_received", action, oneShot, deferred);

        public static SignalHandler OnReferenceSpaceReset(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "reference_space_reset", action, oneShot, deferred);

        public static SignalHandler OnSelect(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "select", action, oneShot, deferred);

        public static SignalHandler OnSelectend(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "selectend", action, oneShot, deferred);

        public static SignalHandler OnSelectstart(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "selectstart", action, oneShot, deferred);

        public static SignalHandler OnSessionEnded(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "session_ended", action, oneShot, deferred);

        public static SignalHandler OnSessionFailed(this WebXRInterface target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "session_failed", action, oneShot, deferred);

        public static SignalHandler OnSessionStarted(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "session_started", action, oneShot, deferred);

        public static SignalHandler OnSessionSupported(this WebXRInterface target, Action<string, bool> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "session_supported", action, oneShot, deferred);

        public static SignalHandler OnSqueeze(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "squeeze", action, oneShot, deferred);

        public static SignalHandler OnSqueezeend(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "squeezeend", action, oneShot, deferred);

        public static SignalHandler OnSqueezestart(this WebXRInterface target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "squeezestart", action, oneShot, deferred);

        public static SignalHandler OnVisibilityStateChanged(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, "visibility_state_changed", action, oneShot, deferred);
    }
}