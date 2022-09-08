using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;
using Environment = Godot.Environment;

namespace Betauer.Signal {
    public static partial class AwaitExtensions {

        // SceneTree signal shortcuts for Node
        public static SignalAwaiter AwaitPhysicsFrame(this Node target) => 
            target.ToSignal(target.GetTree(), "idle_frame");

        // SceneTree signal shortcut for Node
        public static SignalAwaiter AwaitIdleFrame(this Node target) => 
            target.ToSignal(target.GetTree(), "physics_frame");
      
        public static SignalAwaiter AwaitConfirmed(this AcceptDialog target) =>
            target.ToSignal(target, "confirmed");

        public static SignalAwaiter AwaitCustomAction(this AcceptDialog target) =>
            target.ToSignal(target, "custom_action");

        public static SignalAwaiter AwaitAnimationFinished(this AnimatedSprite target) =>
            target.ToSignal(target, "animation_finished");

        public static SignalAwaiter AwaitFrameChanged(this AnimatedSprite target) =>
            target.ToSignal(target, "frame_changed");

        public static SignalAwaiter AwaitAnimationFinished(this AnimatedSprite3D target) =>
            target.ToSignal(target, "animation_finished");

        public static SignalAwaiter AwaitFrameChanged(this AnimatedSprite3D target) =>
            target.ToSignal(target, "frame_changed");

        public static SignalAwaiter AwaitTracksChanged(this Animation target) =>
            target.ToSignal(target, "tracks_changed");

        public static SignalAwaiter AwaitRemovedFromGraph(this AnimationNode target) =>
            target.ToSignal(target, "removed_from_graph");

        public static SignalAwaiter AwaitTreeChanged(this AnimationNode target) =>
            target.ToSignal(target, "tree_changed");

        public static SignalAwaiter AwaitTrianglesUpdated(this AnimationNodeBlendSpace2D target) =>
            target.ToSignal(target, "triangles_updated");

        public static SignalAwaiter AwaitAdvanceConditionChanged(this AnimationNodeStateMachineTransition target) =>
            target.ToSignal(target, "advance_condition_changed");

        public static SignalAwaiter AwaitAnimationChanged(this AnimationPlayer target) =>
            target.ToSignal(target, "animation_changed");

        public static SignalAwaiter AwaitAnimationFinished(this AnimationPlayer target) =>
            target.ToSignal(target, "animation_finished");

        public static SignalAwaiter AwaitAnimationStarted(this AnimationPlayer target) =>
            target.ToSignal(target, "animation_started");

        public static SignalAwaiter AwaitCachesCleared(this AnimationPlayer target) =>
            target.ToSignal(target, "caches_cleared");

        public static SignalAwaiter AwaitAreaEntered(this Area target) =>
            target.ToSignal(target, "area_entered");

        public static SignalAwaiter AwaitAreaExited(this Area target) =>
            target.ToSignal(target, "area_exited");

        public static SignalAwaiter AwaitAreaShapeEntered(this Area target) =>
            target.ToSignal(target, "area_shape_entered");

        public static SignalAwaiter AwaitAreaShapeExited(this Area target) =>
            target.ToSignal(target, "area_shape_exited");

        public static SignalAwaiter AwaitBodyEntered(this Area target) =>
            target.ToSignal(target, "body_entered");

        public static SignalAwaiter AwaitBodyExited(this Area target) =>
            target.ToSignal(target, "body_exited");

        public static SignalAwaiter AwaitBodyShapeEntered(this Area target) =>
            target.ToSignal(target, "body_shape_entered");

        public static SignalAwaiter AwaitBodyShapeExited(this Area target) =>
            target.ToSignal(target, "body_shape_exited");

        public static SignalAwaiter AwaitAreaEntered(this Area2D target) =>
            target.ToSignal(target, "area_entered");

        public static SignalAwaiter AwaitAreaExited(this Area2D target) =>
            target.ToSignal(target, "area_exited");

        public static SignalAwaiter AwaitAreaShapeEntered(this Area2D target) =>
            target.ToSignal(target, "area_shape_entered");

        public static SignalAwaiter AwaitAreaShapeExited(this Area2D target) =>
            target.ToSignal(target, "area_shape_exited");

        public static SignalAwaiter AwaitBodyEntered(this Area2D target) =>
            target.ToSignal(target, "body_entered");

        public static SignalAwaiter AwaitBodyExited(this Area2D target) =>
            target.ToSignal(target, "body_exited");

        public static SignalAwaiter AwaitBodyShapeEntered(this Area2D target) =>
            target.ToSignal(target, "body_shape_entered");

        public static SignalAwaiter AwaitBodyShapeExited(this Area2D target) =>
            target.ToSignal(target, "body_shape_exited");

        public static SignalAwaiter AwaitMeshUpdated(this ARVRAnchor target) =>
            target.ToSignal(target, "mesh_updated");

        public static SignalAwaiter AwaitButtonPressed(this ARVRController target) =>
            target.ToSignal(target, "button_pressed");

        public static SignalAwaiter AwaitButtonRelease(this ARVRController target) =>
            target.ToSignal(target, "button_release");

        public static SignalAwaiter AwaitMeshUpdated(this ARVRController target) =>
            target.ToSignal(target, "mesh_updated");

        public static SignalAwaiter AwaitARVRServerInterfaceAdded() =>
            ARVRServer.Singleton.ToSignal(ARVRServer.Singleton, "interface_added");

        public static SignalAwaiter AwaitARVRServerInterfaceRemoved() =>
            ARVRServer.Singleton.ToSignal(ARVRServer.Singleton, "interface_removed");

        public static SignalAwaiter AwaitARVRServerTrackerAdded() =>
            ARVRServer.Singleton.ToSignal(ARVRServer.Singleton, "tracker_added");

        public static SignalAwaiter AwaitARVRServerTrackerRemoved() =>
            ARVRServer.Singleton.ToSignal(ARVRServer.Singleton, "tracker_removed");

        public static SignalAwaiter AwaitAudioServerBusLayoutChanged() =>
            AudioServer.Singleton.ToSignal(AudioServer.Singleton, "bus_layout_changed");

        public static SignalAwaiter AwaitFinished(this AudioStreamPlayer target) =>
            target.ToSignal(target, "finished");

        public static SignalAwaiter AwaitFinished(this AudioStreamPlayer2D target) =>
            target.ToSignal(target, "finished");

        public static SignalAwaiter AwaitFinished(this AudioStreamPlayer3D target) =>
            target.ToSignal(target, "finished");

        public static SignalAwaiter AwaitButtonDown(this BaseButton target) =>
            target.ToSignal(target, "button_down");

        public static SignalAwaiter AwaitButtonUp(this BaseButton target) =>
            target.ToSignal(target, "button_up");

        public static SignalAwaiter AwaitPressed(this BaseButton target) =>
            target.ToSignal(target, "pressed");

        public static SignalAwaiter AwaitToggled(this BaseButton target) =>
            target.ToSignal(target, "toggled");

        public static SignalAwaiter AwaitPressed(this ButtonGroup target) =>
            target.ToSignal(target, "pressed");

        public static SignalAwaiter AwaitCameraServerCameraFeedAdded() =>
            CameraServer.Singleton.ToSignal(CameraServer.Singleton, "camera_feed_added");

        public static SignalAwaiter AwaitCameraServerCameraFeedRemoved() =>
            CameraServer.Singleton.ToSignal(CameraServer.Singleton, "camera_feed_removed");

        public static SignalAwaiter AwaitDraw(this CanvasItem target) =>
            target.ToSignal(target, "draw");

        public static SignalAwaiter AwaitHide(this CanvasItem target) =>
            target.ToSignal(target, "hide");

        public static SignalAwaiter AwaitItemRectChanged(this CanvasItem target) =>
            target.ToSignal(target, "item_rect_changed");

        public static SignalAwaiter AwaitVisibilityChanged(this CanvasItem target) =>
            target.ToSignal(target, "visibility_changed");

        public static SignalAwaiter AwaitVisibilityChanged(this CanvasLayer target) =>
            target.ToSignal(target, "visibility_changed");

        public static SignalAwaiter AwaitInputEvent(this CollisionObject target) =>
            target.ToSignal(target, "input_event");

        public static SignalAwaiter AwaitMouseEntered(this CollisionObject target) =>
            target.ToSignal(target, "mouse_entered");

        public static SignalAwaiter AwaitMouseExited(this CollisionObject target) =>
            target.ToSignal(target, "mouse_exited");

        public static SignalAwaiter AwaitInputEvent(this CollisionObject2D target) =>
            target.ToSignal(target, "input_event");

        public static SignalAwaiter AwaitMouseEntered(this CollisionObject2D target) =>
            target.ToSignal(target, "mouse_entered");

        public static SignalAwaiter AwaitMouseExited(this CollisionObject2D target) =>
            target.ToSignal(target, "mouse_exited");

        public static SignalAwaiter AwaitColorChanged(this ColorPicker target) =>
            target.ToSignal(target, "color_changed");

        public static SignalAwaiter AwaitPresetAdded(this ColorPicker target) =>
            target.ToSignal(target, "preset_added");

        public static SignalAwaiter AwaitPresetRemoved(this ColorPicker target) =>
            target.ToSignal(target, "preset_removed");

        public static SignalAwaiter AwaitColorChanged(this ColorPickerButton target) =>
            target.ToSignal(target, "color_changed");

        public static SignalAwaiter AwaitPickerCreated(this ColorPickerButton target) =>
            target.ToSignal(target, "picker_created");

        public static SignalAwaiter AwaitPopupClosed(this ColorPickerButton target) =>
            target.ToSignal(target, "popup_closed");

        public static SignalAwaiter AwaitSortChildren(this Container target) =>
            target.ToSignal(target, "sort_children");

        public static SignalAwaiter AwaitFocusEntered(this Control target) =>
            target.ToSignal(target, "focus_entered");

        public static SignalAwaiter AwaitFocusExited(this Control target) =>
            target.ToSignal(target, "focus_exited");

        public static SignalAwaiter AwaitGuiInput(this Control target) =>
            target.ToSignal(target, "gui_input");

        public static SignalAwaiter AwaitMinimumSizeChanged(this Control target) =>
            target.ToSignal(target, "minimum_size_changed");

        public static SignalAwaiter AwaitModalClosed(this Control target) =>
            target.ToSignal(target, "modal_closed");

        public static SignalAwaiter AwaitMouseEntered(this Control target) =>
            target.ToSignal(target, "mouse_entered");

        public static SignalAwaiter AwaitMouseExited(this Control target) =>
            target.ToSignal(target, "mouse_exited");

        public static SignalAwaiter AwaitResized(this Control target) =>
            target.ToSignal(target, "resized");

        public static SignalAwaiter AwaitSizeFlagsChanged(this Control target) =>
            target.ToSignal(target, "size_flags_changed");

        public static SignalAwaiter AwaitRangeChanged(this Curve target) =>
            target.ToSignal(target, "range_changed");

        public static SignalAwaiter AwaitDirSelected(this FileDialog target) =>
            target.ToSignal(target, "dir_selected");

        public static SignalAwaiter AwaitFileSelected(this FileDialog target) =>
            target.ToSignal(target, "file_selected");

        public static SignalAwaiter AwaitFilesSelected(this FileDialog target) =>
            target.ToSignal(target, "files_selected");

        public static SignalAwaiter AwaitCompleted(this GDScriptFunctionState target) =>
            target.ToSignal(target, "completed");

        public static SignalAwaiter AwaitBeginNodeMove(this GraphEdit target) =>
            target.ToSignal(target, "_begin_node_move");

        public static SignalAwaiter AwaitConnectionFromEmpty(this GraphEdit target) =>
            target.ToSignal(target, "connection_from_empty");

        public static SignalAwaiter AwaitConnectionRequest(this GraphEdit target) =>
            target.ToSignal(target, "connection_request");

        public static SignalAwaiter AwaitConnectionToEmpty(this GraphEdit target) =>
            target.ToSignal(target, "connection_to_empty");

        public static SignalAwaiter AwaitCopyNodesRequest(this GraphEdit target) =>
            target.ToSignal(target, "copy_nodes_request");

        public static SignalAwaiter AwaitDeleteNodesRequest(this GraphEdit target) =>
            target.ToSignal(target, "delete_nodes_request");

        public static SignalAwaiter AwaitDisconnectionRequest(this GraphEdit target) =>
            target.ToSignal(target, "disconnection_request");

        public static SignalAwaiter AwaitDuplicateNodesRequest(this GraphEdit target) =>
            target.ToSignal(target, "duplicate_nodes_request");

        public static SignalAwaiter AwaitEndNodeMove(this GraphEdit target) =>
            target.ToSignal(target, "_end_node_move");

        public static SignalAwaiter AwaitNodeSelected(this GraphEdit target) =>
            target.ToSignal(target, "node_selected");

        public static SignalAwaiter AwaitNodeUnselected(this GraphEdit target) =>
            target.ToSignal(target, "node_unselected");

        public static SignalAwaiter AwaitPasteNodesRequest(this GraphEdit target) =>
            target.ToSignal(target, "paste_nodes_request");

        public static SignalAwaiter AwaitPopupRequest(this GraphEdit target) =>
            target.ToSignal(target, "popup_request");

        public static SignalAwaiter AwaitScrollOffsetChanged(this GraphEdit target) =>
            target.ToSignal(target, "scroll_offset_changed");

        public static SignalAwaiter AwaitCloseRequest(this GraphNode target) =>
            target.ToSignal(target, "close_request");

        public static SignalAwaiter AwaitDragged(this GraphNode target) =>
            target.ToSignal(target, "dragged");

        public static SignalAwaiter AwaitOffsetChanged(this GraphNode target) =>
            target.ToSignal(target, "offset_changed");

        public static SignalAwaiter AwaitRaiseRequest(this GraphNode target) =>
            target.ToSignal(target, "raise_request");

        public static SignalAwaiter AwaitResizeRequest(this GraphNode target) =>
            target.ToSignal(target, "resize_request");

        public static SignalAwaiter AwaitSlotUpdated(this GraphNode target) =>
            target.ToSignal(target, "slot_updated");

        public static SignalAwaiter AwaitCellSizeChanged(this GridMap target) =>
            target.ToSignal(target, "cell_size_changed");

        public static SignalAwaiter AwaitRequestCompleted(this HTTPRequest target) =>
            target.ToSignal(target, "request_completed");

        public static SignalAwaiter AwaitInputJoyConnectionChanged() =>
            Input.Singleton.ToSignal(Input.Singleton, "joy_connection_changed");

        public static SignalAwaiter AwaitItemActivated(this ItemList target) =>
            target.ToSignal(target, "item_activated");

        public static SignalAwaiter AwaitItemRmbSelected(this ItemList target) =>
            target.ToSignal(target, "item_rmb_selected");

        public static SignalAwaiter AwaitItemSelected(this ItemList target) =>
            target.ToSignal(target, "item_selected");

        public static SignalAwaiter AwaitMultiSelected(this ItemList target) =>
            target.ToSignal(target, "multi_selected");

        public static SignalAwaiter AwaitNothingSelected(this ItemList target) =>
            target.ToSignal(target, "nothing_selected");

        public static SignalAwaiter AwaitRmbClicked(this ItemList target) =>
            target.ToSignal(target, "rmb_clicked");

        public static SignalAwaiter AwaitJavaScriptPwaUpdateAvailable() =>
            JavaScript.Singleton.ToSignal(JavaScript.Singleton, "pwa_update_available");

        public static SignalAwaiter AwaitTextChanged(this LineEdit target) =>
            target.ToSignal(target, "text_changed");

        public static SignalAwaiter AwaitTextChangeRejected(this LineEdit target) =>
            target.ToSignal(target, "text_change_rejected");

        public static SignalAwaiter AwaitTextEntered(this LineEdit target) =>
            target.ToSignal(target, "text_entered");

        public static SignalAwaiter AwaitRequestPermissionsResult(this MainLoop target) =>
            target.ToSignal(target, "on_request_permissions_result");

        public static SignalAwaiter AwaitAboutToShow(this MenuButton target) =>
            target.ToSignal(target, "about_to_show");

        public static SignalAwaiter AwaitTextureChanged(this MeshInstance2D target) =>
            target.ToSignal(target, "texture_changed");

        public static SignalAwaiter AwaitTextureChanged(this MultiMeshInstance2D target) =>
            target.ToSignal(target, "texture_changed");

        public static SignalAwaiter AwaitConnectedToServer(this MultiplayerAPI target) =>
            target.ToSignal(target, "connected_to_server");

        public static SignalAwaiter AwaitConnectionFailed(this MultiplayerAPI target) =>
            target.ToSignal(target, "connection_failed");

        public static SignalAwaiter AwaitNetworkPeerConnected(this MultiplayerAPI target) =>
            target.ToSignal(target, "network_peer_connected");

        public static SignalAwaiter AwaitNetworkPeerDisconnected(this MultiplayerAPI target) =>
            target.ToSignal(target, "network_peer_disconnected");

        public static SignalAwaiter AwaitNetworkPeerPacket(this MultiplayerAPI target) =>
            target.ToSignal(target, "network_peer_packet");

        public static SignalAwaiter AwaitServerDisconnected(this MultiplayerAPI target) =>
            target.ToSignal(target, "server_disconnected");

        public static SignalAwaiter AwaitMapChanged(this Navigation target) =>
            target.ToSignal(target, "map_changed");

        public static SignalAwaiter AwaitNavigation2DServerMapChanged() =>
            Navigation2DServer.Singleton.ToSignal(Navigation2DServer.Singleton, "map_changed");

        public static SignalAwaiter AwaitNavigationFinished(this NavigationAgent target) =>
            target.ToSignal(target, "navigation_finished");

        public static SignalAwaiter AwaitPathChanged(this NavigationAgent target) =>
            target.ToSignal(target, "path_changed");

        public static SignalAwaiter AwaitTargetReached(this NavigationAgent target) =>
            target.ToSignal(target, "target_reached");

        public static SignalAwaiter AwaitVelocityComputed(this NavigationAgent target) =>
            target.ToSignal(target, "velocity_computed");

        public static SignalAwaiter AwaitNavigationFinished(this NavigationAgent2D target) =>
            target.ToSignal(target, "navigation_finished");

        public static SignalAwaiter AwaitPathChanged(this NavigationAgent2D target) =>
            target.ToSignal(target, "path_changed");

        public static SignalAwaiter AwaitTargetReached(this NavigationAgent2D target) =>
            target.ToSignal(target, "target_reached");

        public static SignalAwaiter AwaitVelocityComputed(this NavigationAgent2D target) =>
            target.ToSignal(target, "velocity_computed");

        public static SignalAwaiter AwaitBakeFinished(this NavigationMeshInstance target) =>
            target.ToSignal(target, "bake_finished");

        public static SignalAwaiter AwaitNavigationMeshChanged(this NavigationMeshInstance target) =>
            target.ToSignal(target, "navigation_mesh_changed");

        public static SignalAwaiter AwaitNavigationServerMapChanged() =>
            NavigationServer.Singleton.ToSignal(NavigationServer.Singleton, "map_changed");

        public static SignalAwaiter AwaitPacketGenerated(this NetworkedMultiplayerCustom target) =>
            target.ToSignal(target, "packet_generated");

        public static SignalAwaiter AwaitConnectionFailed(this NetworkedMultiplayerPeer target) =>
            target.ToSignal(target, "connection_failed");

        public static SignalAwaiter AwaitConnectionSucceeded(this NetworkedMultiplayerPeer target) =>
            target.ToSignal(target, "connection_succeeded");

        public static SignalAwaiter AwaitPeerConnected(this NetworkedMultiplayerPeer target) =>
            target.ToSignal(target, "peer_connected");

        public static SignalAwaiter AwaitPeerDisconnected(this NetworkedMultiplayerPeer target) =>
            target.ToSignal(target, "peer_disconnected");

        public static SignalAwaiter AwaitServerDisconnected(this NetworkedMultiplayerPeer target) =>
            target.ToSignal(target, "server_disconnected");

        public static SignalAwaiter AwaitTextureChanged(this NinePatchRect target) =>
            target.ToSignal(target, "texture_changed");

        public static SignalAwaiter AwaitChildEnteredTree(this Node target) =>
            target.ToSignal(target, "child_entered_tree");

        public static SignalAwaiter AwaitChildExitingTree(this Node target) =>
            target.ToSignal(target, "child_exiting_tree");

        public static SignalAwaiter AwaitReady(this Node target) =>
            target.ToSignal(target, "ready");

        public static SignalAwaiter AwaitRenamed(this Node target) =>
            target.ToSignal(target, "renamed");

        public static SignalAwaiter AwaitTreeEntered(this Node target) =>
            target.ToSignal(target, "tree_entered");

        public static SignalAwaiter AwaitTreeExited(this Node target) =>
            target.ToSignal(target, "tree_exited");

        public static SignalAwaiter AwaitTreeExiting(this Node target) =>
            target.ToSignal(target, "tree_exiting");

        public static SignalAwaiter AwaitScriptChanged(this Object target) =>
            target.ToSignal(target, "script_changed");

        public static SignalAwaiter AwaitItemFocused(this OptionButton target) =>
            target.ToSignal(target, "item_focused");

        public static SignalAwaiter AwaitItemSelected(this OptionButton target) =>
            target.ToSignal(target, "item_selected");

        public static SignalAwaiter AwaitCurveChanged(this Path target) =>
            target.ToSignal(target, "curve_changed");

        public static SignalAwaiter AwaitAboutToShow(this Popup target) =>
            target.ToSignal(target, "about_to_show");

        public static SignalAwaiter AwaitPopupHide(this Popup target) =>
            target.ToSignal(target, "popup_hide");

        public static SignalAwaiter AwaitIdFocused(this PopupMenu target) =>
            target.ToSignal(target, "id_focused");

        public static SignalAwaiter AwaitIdPressed(this PopupMenu target) =>
            target.ToSignal(target, "id_pressed");

        public static SignalAwaiter AwaitIndexPressed(this PopupMenu target) =>
            target.ToSignal(target, "index_pressed");

        public static SignalAwaiter AwaitProjectSettingsProjectSettingsChanged() =>
            ProjectSettings.Singleton.ToSignal(ProjectSettings.Singleton, "project_settings_changed");

        public static SignalAwaiter AwaitBroadcast(this ProximityGroup target) =>
            target.ToSignal(target, "broadcast");

        public static SignalAwaiter AwaitChanged(this Range target) =>
            target.ToSignal(target, "changed");

        public static SignalAwaiter AwaitValueChanged(this Range target) =>
            target.ToSignal(target, "value_changed");

        public static SignalAwaiter AwaitChanged(this Resource target) =>
            target.ToSignal(target, "changed");

        public static SignalAwaiter AwaitMetaClicked(this RichTextLabel target) =>
            target.ToSignal(target, "meta_clicked");

        public static SignalAwaiter AwaitMetaHoverEnded(this RichTextLabel target) =>
            target.ToSignal(target, "meta_hover_ended");

        public static SignalAwaiter AwaitMetaHoverStarted(this RichTextLabel target) =>
            target.ToSignal(target, "meta_hover_started");

        public static SignalAwaiter AwaitBodyEntered(this RigidBody target) =>
            target.ToSignal(target, "body_entered");

        public static SignalAwaiter AwaitBodyExited(this RigidBody target) =>
            target.ToSignal(target, "body_exited");

        public static SignalAwaiter AwaitBodyShapeEntered(this RigidBody target) =>
            target.ToSignal(target, "body_shape_entered");

        public static SignalAwaiter AwaitBodyShapeExited(this RigidBody target) =>
            target.ToSignal(target, "body_shape_exited");

        public static SignalAwaiter AwaitSleepingStateChanged(this RigidBody target) =>
            target.ToSignal(target, "sleeping_state_changed");

        public static SignalAwaiter AwaitBodyEntered(this RigidBody2D target) =>
            target.ToSignal(target, "body_entered");

        public static SignalAwaiter AwaitBodyExited(this RigidBody2D target) =>
            target.ToSignal(target, "body_exited");

        public static SignalAwaiter AwaitBodyShapeEntered(this RigidBody2D target) =>
            target.ToSignal(target, "body_shape_entered");

        public static SignalAwaiter AwaitBodyShapeExited(this RigidBody2D target) =>
            target.ToSignal(target, "body_shape_exited");

        public static SignalAwaiter AwaitSleepingStateChanged(this RigidBody2D target) =>
            target.ToSignal(target, "sleeping_state_changed");

        public static SignalAwaiter AwaitConnectedToServer(this SceneTree target) =>
            target.ToSignal(target, "connected_to_server");

        public static SignalAwaiter AwaitConnectionFailed(this SceneTree target) =>
            target.ToSignal(target, "connection_failed");

        public static SignalAwaiter AwaitFilesDropped(this SceneTree target) =>
            target.ToSignal(target, "files_dropped");

        public static SignalAwaiter AwaitGlobalMenuAction(this SceneTree target) =>
            target.ToSignal(target, "global_menu_action");

        public static SignalAwaiter AwaitIdleFrame(this SceneTree target) =>
            target.ToSignal(target, "idle_frame");

        public static SignalAwaiter AwaitNetworkPeerConnected(this SceneTree target) =>
            target.ToSignal(target, "network_peer_connected");

        public static SignalAwaiter AwaitNetworkPeerDisconnected(this SceneTree target) =>
            target.ToSignal(target, "network_peer_disconnected");

        public static SignalAwaiter AwaitNodeAdded(this SceneTree target) =>
            target.ToSignal(target, "node_added");

        public static SignalAwaiter AwaitNodeConfigurationWarningChanged(this SceneTree target) =>
            target.ToSignal(target, "node_configuration_warning_changed");

        public static SignalAwaiter AwaitNodeRemoved(this SceneTree target) =>
            target.ToSignal(target, "node_removed");

        public static SignalAwaiter AwaitNodeRenamed(this SceneTree target) =>
            target.ToSignal(target, "node_renamed");

        public static SignalAwaiter AwaitPhysicsFrame(this SceneTree target) =>
            target.ToSignal(target, "physics_frame");

        public static SignalAwaiter AwaitScreenResized(this SceneTree target) =>
            target.ToSignal(target, "screen_resized");

        public static SignalAwaiter AwaitServerDisconnected(this SceneTree target) =>
            target.ToSignal(target, "server_disconnected");

        public static SignalAwaiter AwaitTreeChanged(this SceneTree target) =>
            target.ToSignal(target, "tree_changed");

        public static SignalAwaiter AwaitTimeout(this SceneTreeTimer target) =>
            target.ToSignal(target, "timeout");

        public static SignalAwaiter AwaitFinished(this SceneTreeTween target) =>
            target.ToSignal(target, "finished");

        public static SignalAwaiter AwaitLoopFinished(this SceneTreeTween target) =>
            target.ToSignal(target, "loop_finished");

        public static SignalAwaiter AwaitStepFinished(this SceneTreeTween target) =>
            target.ToSignal(target, "step_finished");

        public static SignalAwaiter AwaitScrolling(this ScrollBar target) =>
            target.ToSignal(target, "scrolling");

        public static SignalAwaiter AwaitScrollEnded(this ScrollContainer target) =>
            target.ToSignal(target, "scroll_ended");

        public static SignalAwaiter AwaitScrollStarted(this ScrollContainer target) =>
            target.ToSignal(target, "scroll_started");

        public static SignalAwaiter AwaitSkeletonUpdated(this Skeleton target) =>
            target.ToSignal(target, "skeleton_updated");

        public static SignalAwaiter AwaitBoneSetupChanged(this Skeleton2D target) =>
            target.ToSignal(target, "bone_setup_changed");

        public static SignalAwaiter AwaitDragEnded(this Slider target) =>
            target.ToSignal(target, "drag_ended");

        public static SignalAwaiter AwaitDragStarted(this Slider target) =>
            target.ToSignal(target, "drag_started");

        public static SignalAwaiter AwaitGameplayEntered(this Spatial target) =>
            target.ToSignal(target, "gameplay_entered");

        public static SignalAwaiter AwaitGameplayExited(this Spatial target) =>
            target.ToSignal(target, "gameplay_exited");

        public static SignalAwaiter AwaitVisibilityChanged(this Spatial target) =>
            target.ToSignal(target, "visibility_changed");

        public static SignalAwaiter AwaitDragged(this SplitContainer target) =>
            target.ToSignal(target, "dragged");

        public static SignalAwaiter AwaitFrameChanged(this Sprite target) =>
            target.ToSignal(target, "frame_changed");

        public static SignalAwaiter AwaitTextureChanged(this Sprite target) =>
            target.ToSignal(target, "texture_changed");

        public static SignalAwaiter AwaitFrameChanged(this Sprite3D target) =>
            target.ToSignal(target, "frame_changed");

        public static SignalAwaiter AwaitTextureChanged(this StyleBoxTexture target) =>
            target.ToSignal(target, "texture_changed");

        public static SignalAwaiter AwaitPrePopupPressed(this TabContainer target) =>
            target.ToSignal(target, "pre_popup_pressed");

        public static SignalAwaiter AwaitTabChanged(this TabContainer target) =>
            target.ToSignal(target, "tab_changed");

        public static SignalAwaiter AwaitTabSelected(this TabContainer target) =>
            target.ToSignal(target, "tab_selected");

        public static SignalAwaiter AwaitRepositionActiveTabRequest(this Tabs target) =>
            target.ToSignal(target, "reposition_active_tab_request");

        public static SignalAwaiter AwaitRightButtonPressed(this Tabs target) =>
            target.ToSignal(target, "right_button_pressed");

        public static SignalAwaiter AwaitTabChanged(this Tabs target) =>
            target.ToSignal(target, "tab_changed");

        public static SignalAwaiter AwaitTabClicked(this Tabs target) =>
            target.ToSignal(target, "tab_clicked");

        public static SignalAwaiter AwaitTabClose(this Tabs target) =>
            target.ToSignal(target, "tab_close");

        public static SignalAwaiter AwaitTabHover(this Tabs target) =>
            target.ToSignal(target, "tab_hover");

        public static SignalAwaiter AwaitBreakpointToggled(this TextEdit target) =>
            target.ToSignal(target, "breakpoint_toggled");

        public static SignalAwaiter AwaitCursorChanged(this TextEdit target) =>
            target.ToSignal(target, "cursor_changed");

        public static SignalAwaiter AwaitInfoClicked(this TextEdit target) =>
            target.ToSignal(target, "info_clicked");

        public static SignalAwaiter AwaitRequestCompletion(this TextEdit target) =>
            target.ToSignal(target, "request_completion");

        public static SignalAwaiter AwaitSymbolLookup(this TextEdit target) =>
            target.ToSignal(target, "symbol_lookup");

        public static SignalAwaiter AwaitTextChanged(this TextEdit target) =>
            target.ToSignal(target, "text_changed");

        public static SignalAwaiter AwaitSettingsChanged(this TileMap target) =>
            target.ToSignal(target, "settings_changed");

        public static SignalAwaiter AwaitTimeout(this Timer target) =>
            target.ToSignal(target, "timeout");

        public static SignalAwaiter AwaitPressed(this TouchScreenButton target) =>
            target.ToSignal(target, "pressed");

        public static SignalAwaiter AwaitReleased(this TouchScreenButton target) =>
            target.ToSignal(target, "released");

        public static SignalAwaiter AwaitButtonPressed(this Tree target) =>
            target.ToSignal(target, "button_pressed");

        public static SignalAwaiter AwaitCellSelected(this Tree target) =>
            target.ToSignal(target, "cell_selected");

        public static SignalAwaiter AwaitColumnTitlePressed(this Tree target) =>
            target.ToSignal(target, "column_title_pressed");

        public static SignalAwaiter AwaitCustomPopupEdited(this Tree target) =>
            target.ToSignal(target, "custom_popup_edited");

        public static SignalAwaiter AwaitEmptyRmb(this Tree target) =>
            target.ToSignal(target, "empty_rmb");

        public static SignalAwaiter AwaitEmptyTreeRmbSelected(this Tree target) =>
            target.ToSignal(target, "empty_tree_rmb_selected");

        public static SignalAwaiter AwaitItemActivated(this Tree target) =>
            target.ToSignal(target, "item_activated");

        public static SignalAwaiter AwaitItemCollapsed(this Tree target) =>
            target.ToSignal(target, "item_collapsed");

        public static SignalAwaiter AwaitItemCustomButtonPressed(this Tree target) =>
            target.ToSignal(target, "item_custom_button_pressed");

        public static SignalAwaiter AwaitItemDoubleClicked(this Tree target) =>
            target.ToSignal(target, "item_double_clicked");

        public static SignalAwaiter AwaitItemEdited(this Tree target) =>
            target.ToSignal(target, "item_edited");

        public static SignalAwaiter AwaitItemRmbEdited(this Tree target) =>
            target.ToSignal(target, "item_rmb_edited");

        public static SignalAwaiter AwaitItemRmbSelected(this Tree target) =>
            target.ToSignal(target, "item_rmb_selected");

        public static SignalAwaiter AwaitItemSelected(this Tree target) =>
            target.ToSignal(target, "item_selected");

        public static SignalAwaiter AwaitMultiSelected(this Tree target) =>
            target.ToSignal(target, "multi_selected");

        public static SignalAwaiter AwaitNothingSelected(this Tree target) =>
            target.ToSignal(target, "nothing_selected");

        public static SignalAwaiter AwaitTweenAllCompleted(this Tween target) =>
            target.ToSignal(target, "tween_all_completed");

        public static SignalAwaiter AwaitTweenCompleted(this Tween target) =>
            target.ToSignal(target, "tween_completed");

        public static SignalAwaiter AwaitTweenStarted(this Tween target) =>
            target.ToSignal(target, "tween_started");

        public static SignalAwaiter AwaitTweenStep(this Tween target) =>
            target.ToSignal(target, "tween_step");

        public static SignalAwaiter AwaitFinished(this Tweener target) =>
            target.ToSignal(target, "finished");

        public static SignalAwaiter AwaitVersionChanged(this UndoRedo target) =>
            target.ToSignal(target, "version_changed");

        public static SignalAwaiter AwaitFinished(this VideoPlayer target) =>
            target.ToSignal(target, "finished");

        public static SignalAwaiter AwaitGuiFocusChanged(this Viewport target) =>
            target.ToSignal(target, "gui_focus_changed");

        public static SignalAwaiter AwaitSizeChanged(this Viewport target) =>
            target.ToSignal(target, "size_changed");

        public static SignalAwaiter AwaitCameraEntered(this VisibilityNotifier target) =>
            target.ToSignal(target, "camera_entered");

        public static SignalAwaiter AwaitCameraExited(this VisibilityNotifier target) =>
            target.ToSignal(target, "camera_exited");

        public static SignalAwaiter AwaitScreenEntered(this VisibilityNotifier target) =>
            target.ToSignal(target, "screen_entered");

        public static SignalAwaiter AwaitScreenExited(this VisibilityNotifier target) =>
            target.ToSignal(target, "screen_exited");

        public static SignalAwaiter AwaitScreenEntered(this VisibilityNotifier2D target) =>
            target.ToSignal(target, "screen_entered");

        public static SignalAwaiter AwaitScreenExited(this VisibilityNotifier2D target) =>
            target.ToSignal(target, "screen_exited");

        public static SignalAwaiter AwaitViewportEntered(this VisibilityNotifier2D target) =>
            target.ToSignal(target, "viewport_entered");

        public static SignalAwaiter AwaitViewportExited(this VisibilityNotifier2D target) =>
            target.ToSignal(target, "viewport_exited");

        public static SignalAwaiter AwaitNodePortsChanged(this VisualScript target) =>
            target.ToSignal(target, "node_ports_changed");

        public static SignalAwaiter AwaitPortsChanged(this VisualScriptNode target) =>
            target.ToSignal(target, "ports_changed");

        public static SignalAwaiter AwaitVisualServerFramePostDraw() =>
            VisualServer.Singleton.ToSignal(VisualServer.Singleton, "frame_post_draw");

        public static SignalAwaiter AwaitVisualServerFramePreDraw() =>
            VisualServer.Singleton.ToSignal(VisualServer.Singleton, "frame_pre_draw");

        public static SignalAwaiter AwaitEditorRefreshRequest(this VisualShaderNode target) =>
            target.ToSignal(target, "editor_refresh_request");

        public static SignalAwaiter AwaitInputTypeChanged(this VisualShaderNodeInput target) =>
            target.ToSignal(target, "input_type_changed");

        public static SignalAwaiter AwaitDataChannelReceived(this WebRTCPeerConnection target) =>
            target.ToSignal(target, "data_channel_received");

        public static SignalAwaiter AwaitIceCandidateCreated(this WebRTCPeerConnection target) =>
            target.ToSignal(target, "ice_candidate_created");

        public static SignalAwaiter AwaitSessionDescriptionCreated(this WebRTCPeerConnection target) =>
            target.ToSignal(target, "session_description_created");

        public static SignalAwaiter AwaitConnectionClosed(this WebSocketClient target) =>
            target.ToSignal(target, "connection_closed");

        public static SignalAwaiter AwaitConnectionError(this WebSocketClient target) =>
            target.ToSignal(target, "connection_error");

        public static SignalAwaiter AwaitConnectionEstablished(this WebSocketClient target) =>
            target.ToSignal(target, "connection_established");

        public static SignalAwaiter AwaitDataReceived(this WebSocketClient target) =>
            target.ToSignal(target, "data_received");

        public static SignalAwaiter AwaitServerCloseRequest(this WebSocketClient target) =>
            target.ToSignal(target, "server_close_request");

        public static SignalAwaiter AwaitPeerPacket(this WebSocketMultiplayerPeer target) =>
            target.ToSignal(target, "peer_packet");

        public static SignalAwaiter AwaitClientCloseRequest(this WebSocketServer target) =>
            target.ToSignal(target, "client_close_request");

        public static SignalAwaiter AwaitClientConnected(this WebSocketServer target) =>
            target.ToSignal(target, "client_connected");

        public static SignalAwaiter AwaitClientDisconnected(this WebSocketServer target) =>
            target.ToSignal(target, "client_disconnected");

        public static SignalAwaiter AwaitDataReceived(this WebSocketServer target) =>
            target.ToSignal(target, "data_received");

        public static SignalAwaiter AwaitReferenceSpaceReset(this WebXRInterface target) =>
            target.ToSignal(target, "reference_space_reset");

        public static SignalAwaiter AwaitSelect(this WebXRInterface target) =>
            target.ToSignal(target, "select");

        public static SignalAwaiter AwaitSelectend(this WebXRInterface target) =>
            target.ToSignal(target, "selectend");

        public static SignalAwaiter AwaitSelectstart(this WebXRInterface target) =>
            target.ToSignal(target, "selectstart");

        public static SignalAwaiter AwaitSessionEnded(this WebXRInterface target) =>
            target.ToSignal(target, "session_ended");

        public static SignalAwaiter AwaitSessionFailed(this WebXRInterface target) =>
            target.ToSignal(target, "session_failed");

        public static SignalAwaiter AwaitSessionStarted(this WebXRInterface target) =>
            target.ToSignal(target, "session_started");

        public static SignalAwaiter AwaitSessionSupported(this WebXRInterface target) =>
            target.ToSignal(target, "session_supported");

        public static SignalAwaiter AwaitSqueeze(this WebXRInterface target) =>
            target.ToSignal(target, "squeeze");

        public static SignalAwaiter AwaitSqueezeend(this WebXRInterface target) =>
            target.ToSignal(target, "squeezeend");

        public static SignalAwaiter AwaitSqueezestart(this WebXRInterface target) =>
            target.ToSignal(target, "squeezestart");

        public static SignalAwaiter AwaitVisibilityStateChanged(this WebXRInterface target) =>
            target.ToSignal(target, "visibility_state_changed");
    }
}