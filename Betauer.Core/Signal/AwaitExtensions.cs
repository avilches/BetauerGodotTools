using System;
using Godot;
using Animation = Godot.Animation;
using Environment = Godot.Environment;
using Range = Godot.Range;

namespace Betauer.Core.Signal;

public static partial class AwaitExtensions {
  
    public static SignalAwaiter AwaitCanceled(this AcceptDialog target) =>
        target.ToSignal(target, "canceled");

    public static SignalAwaiter AwaitConfirmed(this AcceptDialog target) =>
        target.ToSignal(target, "confirmed");

    public static SignalAwaiter AwaitCustomAction(this AcceptDialog target) =>
        target.ToSignal(target, "custom_action");

    public static SignalAwaiter AwaitAnimationChanged(this AnimatedSprite2D target) =>
        target.ToSignal(target, "animation_changed");

    public static SignalAwaiter AwaitAnimationFinished(this AnimatedSprite2D target) =>
        target.ToSignal(target, "animation_finished");

    public static SignalAwaiter AwaitAnimationLooped(this AnimatedSprite2D target) =>
        target.ToSignal(target, "animation_looped");

    public static SignalAwaiter AwaitFrameChanged(this AnimatedSprite2D target) =>
        target.ToSignal(target, "frame_changed");

    public static SignalAwaiter AwaitSpriteFramesChanged(this AnimatedSprite2D target) =>
        target.ToSignal(target, "sprite_frames_changed");

    public static SignalAwaiter AwaitAnimationChanged(this AnimatedSprite3D target) =>
        target.ToSignal(target, "animation_changed");

    public static SignalAwaiter AwaitAnimationFinished(this AnimatedSprite3D target) =>
        target.ToSignal(target, "animation_finished");

    public static SignalAwaiter AwaitAnimationLooped(this AnimatedSprite3D target) =>
        target.ToSignal(target, "animation_looped");

    public static SignalAwaiter AwaitFrameChanged(this AnimatedSprite3D target) =>
        target.ToSignal(target, "frame_changed");

    public static SignalAwaiter AwaitSpriteFramesChanged(this AnimatedSprite3D target) =>
        target.ToSignal(target, "sprite_frames_changed");

    public static SignalAwaiter AwaitAnimationAdded(this AnimationLibrary target) =>
        target.ToSignal(target, "animation_added");

    public static SignalAwaiter AwaitAnimationChanged(this AnimationLibrary target) =>
        target.ToSignal(target, "animation_changed");

    public static SignalAwaiter AwaitAnimationRemoved(this AnimationLibrary target) =>
        target.ToSignal(target, "animation_removed");

    public static SignalAwaiter AwaitAnimationRenamed(this AnimationLibrary target) =>
        target.ToSignal(target, "animation_renamed");

    public static SignalAwaiter AwaitAnimationNodeRemoved(this AnimationNode target) =>
        target.ToSignal(target, "animation_node_removed");

    public static SignalAwaiter AwaitAnimationNodeRenamed(this AnimationNode target) =>
        target.ToSignal(target, "animation_node_renamed");

    public static SignalAwaiter AwaitTreeChanged(this AnimationNode target) =>
        target.ToSignal(target, "tree_changed");

    public static SignalAwaiter AwaitTrianglesUpdated(this AnimationNodeBlendSpace2D target) =>
        target.ToSignal(target, "triangles_updated");

    public static SignalAwaiter AwaitNodeChanged(this AnimationNodeBlendTree target) =>
        target.ToSignal(target, "node_changed");

    public static SignalAwaiter AwaitAdvanceConditionChanged(this AnimationNodeStateMachineTransition target) =>
        target.ToSignal(target, "advance_condition_changed");

    public static SignalAwaiter AwaitAnimationChanged(this AnimationPlayer target) =>
        target.ToSignal(target, "animation_changed");

    public static SignalAwaiter AwaitAnimationFinished(this AnimationPlayer target) =>
        target.ToSignal(target, "animation_finished");

    public static SignalAwaiter AwaitAnimationLibrariesUpdated(this AnimationPlayer target) =>
        target.ToSignal(target, "animation_libraries_updated");

    public static SignalAwaiter AwaitAnimationListChanged(this AnimationPlayer target) =>
        target.ToSignal(target, "animation_list_changed");

    public static SignalAwaiter AwaitAnimationStarted(this AnimationPlayer target) =>
        target.ToSignal(target, "animation_started");

    public static SignalAwaiter AwaitCachesCleared(this AnimationPlayer target) =>
        target.ToSignal(target, "caches_cleared");

    public static SignalAwaiter AwaitAnimationFinished(this AnimationTree target) =>
        target.ToSignal(target, "animation_finished");

    public static SignalAwaiter AwaitAnimationPlayerChanged(this AnimationTree target) =>
        target.ToSignal(target, "animation_player_changed");

    public static SignalAwaiter AwaitAnimationStarted(this AnimationTree target) =>
        target.ToSignal(target, "animation_started");

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

    public static SignalAwaiter AwaitAreaEntered(this Area3D target) =>
        target.ToSignal(target, "area_entered");

    public static SignalAwaiter AwaitAreaExited(this Area3D target) =>
        target.ToSignal(target, "area_exited");

    public static SignalAwaiter AwaitAreaShapeEntered(this Area3D target) =>
        target.ToSignal(target, "area_shape_entered");

    public static SignalAwaiter AwaitAreaShapeExited(this Area3D target) =>
        target.ToSignal(target, "area_shape_exited");

    public static SignalAwaiter AwaitBodyEntered(this Area3D target) =>
        target.ToSignal(target, "body_entered");

    public static SignalAwaiter AwaitBodyExited(this Area3D target) =>
        target.ToSignal(target, "body_exited");

    public static SignalAwaiter AwaitBodyShapeEntered(this Area3D target) =>
        target.ToSignal(target, "body_shape_entered");

    public static SignalAwaiter AwaitBodyShapeExited(this Area3D target) =>
        target.ToSignal(target, "body_shape_exited");

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

    public static SignalAwaiter AwaitBoneMapUpdated(this BoneMap target) =>
        target.ToSignal(target, "bone_map_updated");

    public static SignalAwaiter AwaitProfileUpdated(this BoneMap target) =>
        target.ToSignal(target, "profile_updated");

    public static SignalAwaiter AwaitPressed(this ButtonGroup target) =>
        target.ToSignal(target, "pressed");

    public static SignalAwaiter AwaitCameraServerCameraFeedAdded() =>
        CameraServer.Singleton.ToSignal(CameraServer.Singleton, "camera_feed_added");

    public static SignalAwaiter AwaitCameraServerCameraFeedRemoved() =>
        CameraServer.Singleton.ToSignal(CameraServer.Singleton, "camera_feed_removed");

    public static SignalAwaiter AwaitDraw(this CanvasItem target) =>
        target.ToSignal(target, "draw");

    public static SignalAwaiter AwaitHidden(this CanvasItem target) =>
        target.ToSignal(target, "hidden");

    public static SignalAwaiter AwaitItemRectChanged(this CanvasItem target) =>
        target.ToSignal(target, "item_rect_changed");

    public static SignalAwaiter AwaitVisibilityChanged(this CanvasItem target) =>
        target.ToSignal(target, "visibility_changed");

    public static SignalAwaiter AwaitVisibilityChanged(this CanvasLayer target) =>
        target.ToSignal(target, "visibility_changed");

    public static SignalAwaiter AwaitBreakpointToggled(this CodeEdit target) =>
        target.ToSignal(target, "breakpoint_toggled");

    public static SignalAwaiter AwaitCodeCompletionRequested(this CodeEdit target) =>
        target.ToSignal(target, "code_completion_requested");

    public static SignalAwaiter AwaitSymbolLookup(this CodeEdit target) =>
        target.ToSignal(target, "symbol_lookup");

    public static SignalAwaiter AwaitSymbolValidate(this CodeEdit target) =>
        target.ToSignal(target, "symbol_validate");

    public static SignalAwaiter AwaitInputEvent(this CollisionObject2D target) =>
        target.ToSignal(target, "input_event");

    public static SignalAwaiter AwaitMouseEntered(this CollisionObject2D target) =>
        target.ToSignal(target, "mouse_entered");

    public static SignalAwaiter AwaitMouseExited(this CollisionObject2D target) =>
        target.ToSignal(target, "mouse_exited");

    public static SignalAwaiter AwaitMouseShapeEntered(this CollisionObject2D target) =>
        target.ToSignal(target, "mouse_shape_entered");

    public static SignalAwaiter AwaitMouseShapeExited(this CollisionObject2D target) =>
        target.ToSignal(target, "mouse_shape_exited");

    public static SignalAwaiter AwaitInputEvent(this CollisionObject3D target) =>
        target.ToSignal(target, "input_event");

    public static SignalAwaiter AwaitMouseEntered(this CollisionObject3D target) =>
        target.ToSignal(target, "mouse_entered");

    public static SignalAwaiter AwaitMouseExited(this CollisionObject3D target) =>
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

    public static SignalAwaiter AwaitPreSortChildren(this Container target) =>
        target.ToSignal(target, "pre_sort_children");

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

    public static SignalAwaiter AwaitMouseEntered(this Control target) =>
        target.ToSignal(target, "mouse_entered");

    public static SignalAwaiter AwaitMouseExited(this Control target) =>
        target.ToSignal(target, "mouse_exited");

    public static SignalAwaiter AwaitResized(this Control target) =>
        target.ToSignal(target, "resized");

    public static SignalAwaiter AwaitSizeFlagsChanged(this Control target) =>
        target.ToSignal(target, "size_flags_changed");

    public static SignalAwaiter AwaitThemeChanged(this Control target) =>
        target.ToSignal(target, "theme_changed");

    public static SignalAwaiter AwaitRangeChanged(this Curve target) =>
        target.ToSignal(target, "range_changed");

    public static SignalAwaiter AwaitDirSelected(this FileDialog target) =>
        target.ToSignal(target, "dir_selected");

    public static SignalAwaiter AwaitFileSelected(this FileDialog target) =>
        target.ToSignal(target, "file_selected");

    public static SignalAwaiter AwaitFilesSelected(this FileDialog target) =>
        target.ToSignal(target, "files_selected");

    public static SignalAwaiter AwaitBeginNodeMove(this GraphEdit target) =>
        target.ToSignal(target, "begin_node_move");

    public static SignalAwaiter AwaitConnectionDragEnded(this GraphEdit target) =>
        target.ToSignal(target, "connection_drag_ended");

    public static SignalAwaiter AwaitConnectionDragStarted(this GraphEdit target) =>
        target.ToSignal(target, "connection_drag_started");

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
        target.ToSignal(target, "end_node_move");

    public static SignalAwaiter AwaitNodeDeselected(this GraphEdit target) =>
        target.ToSignal(target, "node_deselected");

    public static SignalAwaiter AwaitNodeSelected(this GraphEdit target) =>
        target.ToSignal(target, "node_selected");

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

    public static SignalAwaiter AwaitNodeDeselected(this GraphNode target) =>
        target.ToSignal(target, "node_deselected");

    public static SignalAwaiter AwaitNodeSelected(this GraphNode target) =>
        target.ToSignal(target, "node_selected");

    public static SignalAwaiter AwaitPositionOffsetChanged(this GraphNode target) =>
        target.ToSignal(target, "position_offset_changed");

    public static SignalAwaiter AwaitRaiseRequest(this GraphNode target) =>
        target.ToSignal(target, "raise_request");

    public static SignalAwaiter AwaitResizeRequest(this GraphNode target) =>
        target.ToSignal(target, "resize_request");

    public static SignalAwaiter AwaitSlotUpdated(this GraphNode target) =>
        target.ToSignal(target, "slot_updated");

    public static SignalAwaiter AwaitCellSizeChanged(this GridMap target) =>
        target.ToSignal(target, "cell_size_changed");

    public static SignalAwaiter AwaitInputJoyConnectionChanged() =>
        Input.Singleton.ToSignal(Input.Singleton, "joy_connection_changed");

    public static SignalAwaiter AwaitEmptyClicked(this ItemList target) =>
        target.ToSignal(target, "empty_clicked");

    public static SignalAwaiter AwaitItemActivated(this ItemList target) =>
        target.ToSignal(target, "item_activated");

    public static SignalAwaiter AwaitItemClicked(this ItemList target) =>
        target.ToSignal(target, "item_clicked");

    public static SignalAwaiter AwaitItemSelected(this ItemList target) =>
        target.ToSignal(target, "item_selected");

    public static SignalAwaiter AwaitMultiSelected(this ItemList target) =>
        target.ToSignal(target, "multi_selected");

    public static SignalAwaiter AwaitJavaScriptBridgePwaUpdateAvailable() =>
        JavaScriptBridge.Singleton.ToSignal(JavaScriptBridge.Singleton, "pwa_update_available");

    public static SignalAwaiter AwaitTextChanged(this LineEdit target) =>
        target.ToSignal(target, "text_changed");

    public static SignalAwaiter AwaitTextChangeRejected(this LineEdit target) =>
        target.ToSignal(target, "text_change_rejected");

    public static SignalAwaiter AwaitTextSubmitted(this LineEdit target) =>
        target.ToSignal(target, "text_submitted");

    public static SignalAwaiter AwaitOnRequestPermissionsResult(this MainLoop target) =>
        target.ToSignal(target, "on_request_permissions_result");

    public static SignalAwaiter AwaitAboutToPopup(this MenuButton target) =>
        target.ToSignal(target, "about_to_popup");

    public static SignalAwaiter AwaitTextureChanged(this MeshInstance2D target) =>
        target.ToSignal(target, "texture_changed");

    public static SignalAwaiter AwaitTextureChanged(this MultiMeshInstance2D target) =>
        target.ToSignal(target, "texture_changed");

    public static SignalAwaiter AwaitPeerConnected(this MultiplayerPeer target) =>
        target.ToSignal(target, "peer_connected");

    public static SignalAwaiter AwaitPeerDisconnected(this MultiplayerPeer target) =>
        target.ToSignal(target, "peer_disconnected");

    public static SignalAwaiter AwaitDespawned(this MultiplayerSpawner target) =>
        target.ToSignal(target, "despawned");

    public static SignalAwaiter AwaitSpawned(this MultiplayerSpawner target) =>
        target.ToSignal(target, "spawned");

    public static SignalAwaiter AwaitSynchronized(this MultiplayerSynchronizer target) =>
        target.ToSignal(target, "synchronized");

    public static SignalAwaiter AwaitVisibilityChanged(this MultiplayerSynchronizer target) =>
        target.ToSignal(target, "visibility_changed");

    public static SignalAwaiter AwaitLinkReached(this NavigationAgent2D target) =>
        target.ToSignal(target, "link_reached");

    public static SignalAwaiter AwaitNavigationFinished(this NavigationAgent2D target) =>
        target.ToSignal(target, "navigation_finished");

    public static SignalAwaiter AwaitPathChanged(this NavigationAgent2D target) =>
        target.ToSignal(target, "path_changed");

    public static SignalAwaiter AwaitTargetReached(this NavigationAgent2D target) =>
        target.ToSignal(target, "target_reached");

    public static SignalAwaiter AwaitVelocityComputed(this NavigationAgent2D target) =>
        target.ToSignal(target, "velocity_computed");

    public static SignalAwaiter AwaitWaypointReached(this NavigationAgent2D target) =>
        target.ToSignal(target, "waypoint_reached");

    public static SignalAwaiter AwaitLinkReached(this NavigationAgent3D target) =>
        target.ToSignal(target, "link_reached");

    public static SignalAwaiter AwaitNavigationFinished(this NavigationAgent3D target) =>
        target.ToSignal(target, "navigation_finished");

    public static SignalAwaiter AwaitPathChanged(this NavigationAgent3D target) =>
        target.ToSignal(target, "path_changed");

    public static SignalAwaiter AwaitTargetReached(this NavigationAgent3D target) =>
        target.ToSignal(target, "target_reached");

    public static SignalAwaiter AwaitVelocityComputed(this NavigationAgent3D target) =>
        target.ToSignal(target, "velocity_computed");

    public static SignalAwaiter AwaitWaypointReached(this NavigationAgent3D target) =>
        target.ToSignal(target, "waypoint_reached");

    public static SignalAwaiter AwaitBakeFinished(this NavigationRegion3D target) =>
        target.ToSignal(target, "bake_finished");

    public static SignalAwaiter AwaitNavigationMeshChanged(this NavigationRegion3D target) =>
        target.ToSignal(target, "navigation_mesh_changed");

    public static SignalAwaiter AwaitNavigationServer2DMapChanged() =>
        NavigationServer2D.Singleton.ToSignal(NavigationServer2D.Singleton, "map_changed");

    public static SignalAwaiter AwaitNavigationServer2DNavigationDebugChanged() =>
        NavigationServer2D.Singleton.ToSignal(NavigationServer2D.Singleton, "navigation_debug_changed");

    public static SignalAwaiter AwaitNavigationServer3DMapChanged() =>
        NavigationServer3D.Singleton.ToSignal(NavigationServer3D.Singleton, "map_changed");

    public static SignalAwaiter AwaitNavigationServer3DNavigationDebugChanged() =>
        NavigationServer3D.Singleton.ToSignal(NavigationServer3D.Singleton, "navigation_debug_changed");

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

    public static SignalAwaiter AwaitVisibilityChanged(this Node3D target) =>
        target.ToSignal(target, "visibility_changed");

    public static SignalAwaiter AwaitItemFocused(this OptionButton target) =>
        target.ToSignal(target, "item_focused");

    public static SignalAwaiter AwaitItemSelected(this OptionButton target) =>
        target.ToSignal(target, "item_selected");

    public static SignalAwaiter AwaitCurveChanged(this Path3D target) =>
        target.ToSignal(target, "curve_changed");

    public static SignalAwaiter AwaitPopupHide(this Popup target) =>
        target.ToSignal(target, "popup_hide");

    public static SignalAwaiter AwaitIdFocused(this PopupMenu target) =>
        target.ToSignal(target, "id_focused");

    public static SignalAwaiter AwaitIdPressed(this PopupMenu target) =>
        target.ToSignal(target, "id_pressed");

    public static SignalAwaiter AwaitIndexPressed(this PopupMenu target) =>
        target.ToSignal(target, "index_pressed");

    public static SignalAwaiter AwaitMenuChanged(this PopupMenu target) =>
        target.ToSignal(target, "menu_changed");

    public static SignalAwaiter AwaitChanged(this Range target) =>
        target.ToSignal(target, "changed");

    public static SignalAwaiter AwaitValueChanged(this Range target) =>
        target.ToSignal(target, "value_changed");

    public static SignalAwaiter AwaitRenderingServerFramePostDraw() =>
        RenderingServer.Singleton.ToSignal(RenderingServer.Singleton, "frame_post_draw");

    public static SignalAwaiter AwaitRenderingServerFramePreDraw() =>
        RenderingServer.Singleton.ToSignal(RenderingServer.Singleton, "frame_pre_draw");

    public static SignalAwaiter AwaitChanged(this Resource target) =>
        target.ToSignal(target, "changed");

    public static SignalAwaiter AwaitSetupLocalToSceneRequested(this Resource target) =>
        target.ToSignal(target, "setup_local_to_scene_requested");

    public static SignalAwaiter AwaitFinished(this RichTextLabel target) =>
        target.ToSignal(target, "finished");

    public static SignalAwaiter AwaitMetaClicked(this RichTextLabel target) =>
        target.ToSignal(target, "meta_clicked");

    public static SignalAwaiter AwaitMetaHoverEnded(this RichTextLabel target) =>
        target.ToSignal(target, "meta_hover_ended");

    public static SignalAwaiter AwaitMetaHoverStarted(this RichTextLabel target) =>
        target.ToSignal(target, "meta_hover_started");

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

    public static SignalAwaiter AwaitBodyEntered(this RigidBody3D target) =>
        target.ToSignal(target, "body_entered");

    public static SignalAwaiter AwaitBodyExited(this RigidBody3D target) =>
        target.ToSignal(target, "body_exited");

    public static SignalAwaiter AwaitBodyShapeEntered(this RigidBody3D target) =>
        target.ToSignal(target, "body_shape_entered");

    public static SignalAwaiter AwaitBodyShapeExited(this RigidBody3D target) =>
        target.ToSignal(target, "body_shape_exited");

    public static SignalAwaiter AwaitSleepingStateChanged(this RigidBody3D target) =>
        target.ToSignal(target, "sleeping_state_changed");

    public static SignalAwaiter AwaitPeerAuthenticating(this SceneMultiplayer target) =>
        target.ToSignal(target, "peer_authenticating");

    public static SignalAwaiter AwaitPeerAuthenticationFailed(this SceneMultiplayer target) =>
        target.ToSignal(target, "peer_authentication_failed");

    public static SignalAwaiter AwaitPeerPacket(this SceneMultiplayer target) =>
        target.ToSignal(target, "peer_packet");

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

    public static SignalAwaiter AwaitProcessFrame(this SceneTree target) =>
        target.ToSignal(target, "process_frame");

    public static SignalAwaiter AwaitTreeChanged(this SceneTree target) =>
        target.ToSignal(target, "tree_changed");

    public static SignalAwaiter AwaitTreeProcessModeChanged(this SceneTree target) =>
        target.ToSignal(target, "tree_process_mode_changed");

    public static SignalAwaiter AwaitTimeout(this SceneTreeTimer target) =>
        target.ToSignal(target, "timeout");

    public static SignalAwaiter AwaitScrolling(this ScrollBar target) =>
        target.ToSignal(target, "scrolling");

    public static SignalAwaiter AwaitScrollEnded(this ScrollContainer target) =>
        target.ToSignal(target, "scroll_ended");

    public static SignalAwaiter AwaitScrollStarted(this ScrollContainer target) =>
        target.ToSignal(target, "scroll_started");

    public static SignalAwaiter AwaitBoneSetupChanged(this Skeleton2D target) =>
        target.ToSignal(target, "bone_setup_changed");

    public static SignalAwaiter AwaitBoneEnabledChanged(this Skeleton3D target) =>
        target.ToSignal(target, "bone_enabled_changed");

    public static SignalAwaiter AwaitBonePoseChanged(this Skeleton3D target) =>
        target.ToSignal(target, "bone_pose_changed");

    public static SignalAwaiter AwaitPoseUpdated(this Skeleton3D target) =>
        target.ToSignal(target, "pose_updated");

    public static SignalAwaiter AwaitShowRestOnlyChanged(this Skeleton3D target) =>
        target.ToSignal(target, "show_rest_only_changed");

    public static SignalAwaiter AwaitProfileUpdated(this SkeletonProfile target) =>
        target.ToSignal(target, "profile_updated");

    public static SignalAwaiter AwaitDragEnded(this Slider target) =>
        target.ToSignal(target, "drag_ended");

    public static SignalAwaiter AwaitDragStarted(this Slider target) =>
        target.ToSignal(target, "drag_started");

    public static SignalAwaiter AwaitDragged(this SplitContainer target) =>
        target.ToSignal(target, "dragged");

    public static SignalAwaiter AwaitFrameChanged(this Sprite2D target) =>
        target.ToSignal(target, "frame_changed");

    public static SignalAwaiter AwaitTextureChanged(this Sprite2D target) =>
        target.ToSignal(target, "texture_changed");

    public static SignalAwaiter AwaitFrameChanged(this Sprite3D target) =>
        target.ToSignal(target, "frame_changed");

    public static SignalAwaiter AwaitTextureChanged(this Sprite3D target) =>
        target.ToSignal(target, "texture_changed");

    public static SignalAwaiter AwaitActiveTabRearranged(this TabBar target) =>
        target.ToSignal(target, "active_tab_rearranged");

    public static SignalAwaiter AwaitTabButtonPressed(this TabBar target) =>
        target.ToSignal(target, "tab_button_pressed");

    public static SignalAwaiter AwaitTabChanged(this TabBar target) =>
        target.ToSignal(target, "tab_changed");

    public static SignalAwaiter AwaitTabClicked(this TabBar target) =>
        target.ToSignal(target, "tab_clicked");

    public static SignalAwaiter AwaitTabClosePressed(this TabBar target) =>
        target.ToSignal(target, "tab_close_pressed");

    public static SignalAwaiter AwaitTabHovered(this TabBar target) =>
        target.ToSignal(target, "tab_hovered");

    public static SignalAwaiter AwaitTabRmbClicked(this TabBar target) =>
        target.ToSignal(target, "tab_rmb_clicked");

    public static SignalAwaiter AwaitTabSelected(this TabBar target) =>
        target.ToSignal(target, "tab_selected");

    public static SignalAwaiter AwaitPrePopupPressed(this TabContainer target) =>
        target.ToSignal(target, "pre_popup_pressed");

    public static SignalAwaiter AwaitTabButtonPressed(this TabContainer target) =>
        target.ToSignal(target, "tab_button_pressed");

    public static SignalAwaiter AwaitTabChanged(this TabContainer target) =>
        target.ToSignal(target, "tab_changed");

    public static SignalAwaiter AwaitTabSelected(this TabContainer target) =>
        target.ToSignal(target, "tab_selected");

    public static SignalAwaiter AwaitCaretChanged(this TextEdit target) =>
        target.ToSignal(target, "caret_changed");

    public static SignalAwaiter AwaitGutterAdded(this TextEdit target) =>
        target.ToSignal(target, "gutter_added");

    public static SignalAwaiter AwaitGutterClicked(this TextEdit target) =>
        target.ToSignal(target, "gutter_clicked");

    public static SignalAwaiter AwaitGutterRemoved(this TextEdit target) =>
        target.ToSignal(target, "gutter_removed");

    public static SignalAwaiter AwaitLinesEditedFrom(this TextEdit target) =>
        target.ToSignal(target, "lines_edited_from");

    public static SignalAwaiter AwaitTextChanged(this TextEdit target) =>
        target.ToSignal(target, "text_changed");

    public static SignalAwaiter AwaitTextSet(this TextEdit target) =>
        target.ToSignal(target, "text_set");

    public static SignalAwaiter AwaitTextServerManagerInterfaceAdded() =>
        TextServerManager.Singleton.ToSignal(TextServerManager.Singleton, "interface_added");

    public static SignalAwaiter AwaitTextServerManagerInterfaceRemoved() =>
        TextServerManager.Singleton.ToSignal(TextServerManager.Singleton, "interface_removed");

    public static SignalAwaiter AwaitThemeDBFallbackChanged() =>
        ThemeDB.Singleton.ToSignal(ThemeDB.Singleton, "fallback_changed");

    public static SignalAwaiter AwaitChanged(this TileData target) =>
        target.ToSignal(target, "changed");

    public static SignalAwaiter AwaitChanged(this TileMap target) =>
        target.ToSignal(target, "changed");

    public static SignalAwaiter AwaitTimeout(this Timer target) =>
        target.ToSignal(target, "timeout");

    public static SignalAwaiter AwaitPressed(this TouchScreenButton target) =>
        target.ToSignal(target, "pressed");

    public static SignalAwaiter AwaitReleased(this TouchScreenButton target) =>
        target.ToSignal(target, "released");

    public static SignalAwaiter AwaitButtonClicked(this Tree target) =>
        target.ToSignal(target, "button_clicked");

    public static SignalAwaiter AwaitCellSelected(this Tree target) =>
        target.ToSignal(target, "cell_selected");

    public static SignalAwaiter AwaitCheckPropagatedToItem(this Tree target) =>
        target.ToSignal(target, "check_propagated_to_item");

    public static SignalAwaiter AwaitColumnTitleClicked(this Tree target) =>
        target.ToSignal(target, "column_title_clicked");

    public static SignalAwaiter AwaitCustomItemClicked(this Tree target) =>
        target.ToSignal(target, "custom_item_clicked");

    public static SignalAwaiter AwaitCustomPopupEdited(this Tree target) =>
        target.ToSignal(target, "custom_popup_edited");

    public static SignalAwaiter AwaitEmptyClicked(this Tree target) =>
        target.ToSignal(target, "empty_clicked");

    public static SignalAwaiter AwaitItemActivated(this Tree target) =>
        target.ToSignal(target, "item_activated");

    public static SignalAwaiter AwaitItemCollapsed(this Tree target) =>
        target.ToSignal(target, "item_collapsed");

    public static SignalAwaiter AwaitItemEdited(this Tree target) =>
        target.ToSignal(target, "item_edited");

    public static SignalAwaiter AwaitItemIconDoubleClicked(this Tree target) =>
        target.ToSignal(target, "item_icon_double_clicked");

    public static SignalAwaiter AwaitItemMouseSelected(this Tree target) =>
        target.ToSignal(target, "item_mouse_selected");

    public static SignalAwaiter AwaitItemSelected(this Tree target) =>
        target.ToSignal(target, "item_selected");

    public static SignalAwaiter AwaitMultiSelected(this Tree target) =>
        target.ToSignal(target, "multi_selected");

    public static SignalAwaiter AwaitNothingSelected(this Tree target) =>
        target.ToSignal(target, "nothing_selected");

    public static SignalAwaiter AwaitFinished(this Tween target) =>
        target.ToSignal(target, "finished");

    public static SignalAwaiter AwaitLoopFinished(this Tween target) =>
        target.ToSignal(target, "loop_finished");

    public static SignalAwaiter AwaitStepFinished(this Tween target) =>
        target.ToSignal(target, "step_finished");

    public static SignalAwaiter AwaitFinished(this Tweener target) =>
        target.ToSignal(target, "finished");

    public static SignalAwaiter AwaitVersionChanged(this UndoRedo target) =>
        target.ToSignal(target, "version_changed");

    public static SignalAwaiter AwaitFinished(this VideoStreamPlayer target) =>
        target.ToSignal(target, "finished");

    public static SignalAwaiter AwaitGuiFocusChanged(this Viewport target) =>
        target.ToSignal(target, "gui_focus_changed");

    public static SignalAwaiter AwaitSizeChanged(this Viewport target) =>
        target.ToSignal(target, "size_changed");

    public static SignalAwaiter AwaitScreenEntered(this VisibleOnScreenNotifier2D target) =>
        target.ToSignal(target, "screen_entered");

    public static SignalAwaiter AwaitScreenExited(this VisibleOnScreenNotifier2D target) =>
        target.ToSignal(target, "screen_exited");

    public static SignalAwaiter AwaitScreenEntered(this VisibleOnScreenNotifier3D target) =>
        target.ToSignal(target, "screen_entered");

    public static SignalAwaiter AwaitScreenExited(this VisibleOnScreenNotifier3D target) =>
        target.ToSignal(target, "screen_exited");

    public static SignalAwaiter AwaitInputTypeChanged(this VisualShaderNodeInput target) =>
        target.ToSignal(target, "input_type_changed");

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

    public static SignalAwaiter AwaitAboutToPopup(this Window target) =>
        target.ToSignal(target, "about_to_popup");

    public static SignalAwaiter AwaitCloseRequested(this Window target) =>
        target.ToSignal(target, "close_requested");

    public static SignalAwaiter AwaitDpiChanged(this Window target) =>
        target.ToSignal(target, "dpi_changed");

    public static SignalAwaiter AwaitFilesDropped(this Window target) =>
        target.ToSignal(target, "files_dropped");

    public static SignalAwaiter AwaitFocusEntered(this Window target) =>
        target.ToSignal(target, "focus_entered");

    public static SignalAwaiter AwaitFocusExited(this Window target) =>
        target.ToSignal(target, "focus_exited");

    public static SignalAwaiter AwaitGoBackRequested(this Window target) =>
        target.ToSignal(target, "go_back_requested");

    public static SignalAwaiter AwaitMouseEntered(this Window target) =>
        target.ToSignal(target, "mouse_entered");

    public static SignalAwaiter AwaitMouseExited(this Window target) =>
        target.ToSignal(target, "mouse_exited");

    public static SignalAwaiter AwaitThemeChanged(this Window target) =>
        target.ToSignal(target, "theme_changed");

    public static SignalAwaiter AwaitTitlebarChanged(this Window target) =>
        target.ToSignal(target, "titlebar_changed");

    public static SignalAwaiter AwaitVisibilityChanged(this Window target) =>
        target.ToSignal(target, "visibility_changed");

    public static SignalAwaiter AwaitWindowInput(this Window target) =>
        target.ToSignal(target, "window_input");

    public static SignalAwaiter AwaitButtonPressed(this XRController3D target) =>
        target.ToSignal(target, "button_pressed");

    public static SignalAwaiter AwaitButtonReleased(this XRController3D target) =>
        target.ToSignal(target, "button_released");

    public static SignalAwaiter AwaitInputFloatChanged(this XRController3D target) =>
        target.ToSignal(target, "input_float_changed");

    public static SignalAwaiter AwaitInputVector2Changed(this XRController3D target) =>
        target.ToSignal(target, "input_vector2_changed");

    public static SignalAwaiter AwaitPlayAreaChanged(this XRInterface target) =>
        target.ToSignal(target, "play_area_changed");

    public static SignalAwaiter AwaitButtonPressed(this XRPositionalTracker target) =>
        target.ToSignal(target, "button_pressed");

    public static SignalAwaiter AwaitButtonReleased(this XRPositionalTracker target) =>
        target.ToSignal(target, "button_released");

    public static SignalAwaiter AwaitInputFloatChanged(this XRPositionalTracker target) =>
        target.ToSignal(target, "input_float_changed");

    public static SignalAwaiter AwaitInputVector2Changed(this XRPositionalTracker target) =>
        target.ToSignal(target, "input_vector2_changed");

    public static SignalAwaiter AwaitPoseChanged(this XRPositionalTracker target) =>
        target.ToSignal(target, "pose_changed");

    public static SignalAwaiter AwaitProfileChanged(this XRPositionalTracker target) =>
        target.ToSignal(target, "profile_changed");

    public static SignalAwaiter AwaitXRServerInterfaceAdded() =>
        XRServer.Singleton.ToSignal(XRServer.Singleton, "interface_added");

    public static SignalAwaiter AwaitXRServerInterfaceRemoved() =>
        XRServer.Singleton.ToSignal(XRServer.Singleton, "interface_removed");

    public static SignalAwaiter AwaitXRServerTrackerAdded() =>
        XRServer.Singleton.ToSignal(XRServer.Singleton, "tracker_added");

    public static SignalAwaiter AwaitXRServerTrackerRemoved() =>
        XRServer.Singleton.ToSignal(XRServer.Singleton, "tracker_removed");

    public static SignalAwaiter AwaitXRServerTrackerUpdated() =>
        XRServer.Singleton.ToSignal(XRServer.Singleton, "tracker_updated");
}