using System;
using Godot;
using Animation = Godot.Animation;
using Environment = Godot.Environment;
using Range = Godot.Range;

namespace Betauer.Core.Signal;

public static partial class AwaitExtensions {
  
    public static SignalAwaiter AwaitCanceled(this AcceptDialog target) =>
        target.ToSignal(target, AcceptDialog.SignalName.Canceled);

    public static SignalAwaiter AwaitConfirmed(this AcceptDialog target) =>
        target.ToSignal(target, AcceptDialog.SignalName.Confirmed);

    public static SignalAwaiter AwaitCustomAction(this AcceptDialog target) =>
        target.ToSignal(target, AcceptDialog.SignalName.CustomAction);

    public static SignalAwaiter AwaitAnimationChanged(this AnimatedSprite2D target) =>
        target.ToSignal(target, AnimatedSprite2D.SignalName.AnimationChanged);

    public static SignalAwaiter AwaitAnimationFinished(this AnimatedSprite2D target) =>
        target.ToSignal(target, AnimatedSprite2D.SignalName.AnimationFinished);

    public static SignalAwaiter AwaitAnimationLooped(this AnimatedSprite2D target) =>
        target.ToSignal(target, AnimatedSprite2D.SignalName.AnimationLooped);

    public static SignalAwaiter AwaitFrameChanged(this AnimatedSprite2D target) =>
        target.ToSignal(target, AnimatedSprite2D.SignalName.FrameChanged);

    public static SignalAwaiter AwaitSpriteFramesChanged(this AnimatedSprite2D target) =>
        target.ToSignal(target, AnimatedSprite2D.SignalName.SpriteFramesChanged);

    public static SignalAwaiter AwaitAnimationChanged(this AnimatedSprite3D target) =>
        target.ToSignal(target, AnimatedSprite3D.SignalName.AnimationChanged);

    public static SignalAwaiter AwaitAnimationFinished(this AnimatedSprite3D target) =>
        target.ToSignal(target, AnimatedSprite3D.SignalName.AnimationFinished);

    public static SignalAwaiter AwaitAnimationLooped(this AnimatedSprite3D target) =>
        target.ToSignal(target, AnimatedSprite3D.SignalName.AnimationLooped);

    public static SignalAwaiter AwaitFrameChanged(this AnimatedSprite3D target) =>
        target.ToSignal(target, AnimatedSprite3D.SignalName.FrameChanged);

    public static SignalAwaiter AwaitSpriteFramesChanged(this AnimatedSprite3D target) =>
        target.ToSignal(target, AnimatedSprite3D.SignalName.SpriteFramesChanged);

    public static SignalAwaiter AwaitAnimationAdded(this AnimationLibrary target) =>
        target.ToSignal(target, AnimationLibrary.SignalName.AnimationAdded);

    public static SignalAwaiter AwaitAnimationChanged(this AnimationLibrary target) =>
        target.ToSignal(target, AnimationLibrary.SignalName.AnimationChanged);

    public static SignalAwaiter AwaitAnimationRemoved(this AnimationLibrary target) =>
        target.ToSignal(target, AnimationLibrary.SignalName.AnimationRemoved);

    public static SignalAwaiter AwaitAnimationRenamed(this AnimationLibrary target) =>
        target.ToSignal(target, AnimationLibrary.SignalName.AnimationRenamed);

    public static SignalAwaiter AwaitAnimationNodeRemoved(this AnimationNode target) =>
        target.ToSignal(target, AnimationNode.SignalName.AnimationNodeRemoved);

    public static SignalAwaiter AwaitAnimationNodeRenamed(this AnimationNode target) =>
        target.ToSignal(target, AnimationNode.SignalName.AnimationNodeRenamed);

    public static SignalAwaiter AwaitTreeChanged(this AnimationNode target) =>
        target.ToSignal(target, AnimationNode.SignalName.TreeChanged);

    public static SignalAwaiter AwaitTrianglesUpdated(this AnimationNodeBlendSpace2D target) =>
        target.ToSignal(target, AnimationNodeBlendSpace2D.SignalName.TrianglesUpdated);

    public static SignalAwaiter AwaitNodeChanged(this AnimationNodeBlendTree target) =>
        target.ToSignal(target, AnimationNodeBlendTree.SignalName.NodeChanged);

    public static SignalAwaiter AwaitAdvanceConditionChanged(this AnimationNodeStateMachineTransition target) =>
        target.ToSignal(target, AnimationNodeStateMachineTransition.SignalName.AdvanceConditionChanged);

    public static SignalAwaiter AwaitAnimationChanged(this AnimationPlayer target) =>
        target.ToSignal(target, AnimationPlayer.SignalName.AnimationChanged);

    public static SignalAwaiter AwaitAnimationFinished(this AnimationPlayer target) =>
        target.ToSignal(target, AnimationPlayer.SignalName.AnimationFinished);

    public static SignalAwaiter AwaitAnimationLibrariesUpdated(this AnimationPlayer target) =>
        target.ToSignal(target, AnimationPlayer.SignalName.AnimationLibrariesUpdated);

    public static SignalAwaiter AwaitAnimationListChanged(this AnimationPlayer target) =>
        target.ToSignal(target, AnimationPlayer.SignalName.AnimationListChanged);

    public static SignalAwaiter AwaitAnimationStarted(this AnimationPlayer target) =>
        target.ToSignal(target, AnimationPlayer.SignalName.AnimationStarted);

    public static SignalAwaiter AwaitCachesCleared(this AnimationPlayer target) =>
        target.ToSignal(target, AnimationPlayer.SignalName.CachesCleared);

    public static SignalAwaiter AwaitAnimationFinished(this AnimationTree target) =>
        target.ToSignal(target, AnimationTree.SignalName.AnimationFinished);

    public static SignalAwaiter AwaitAnimationPlayerChanged(this AnimationTree target) =>
        target.ToSignal(target, AnimationTree.SignalName.AnimationPlayerChanged);

    public static SignalAwaiter AwaitAnimationStarted(this AnimationTree target) =>
        target.ToSignal(target, AnimationTree.SignalName.AnimationStarted);

    public static SignalAwaiter AwaitAreaEntered(this Area2D target) =>
        target.ToSignal(target, Area2D.SignalName.AreaEntered);

    public static SignalAwaiter AwaitAreaExited(this Area2D target) =>
        target.ToSignal(target, Area2D.SignalName.AreaExited);

    public static SignalAwaiter AwaitAreaShapeEntered(this Area2D target) =>
        target.ToSignal(target, Area2D.SignalName.AreaShapeEntered);

    public static SignalAwaiter AwaitAreaShapeExited(this Area2D target) =>
        target.ToSignal(target, Area2D.SignalName.AreaShapeExited);

    public static SignalAwaiter AwaitBodyEntered(this Area2D target) =>
        target.ToSignal(target, Area2D.SignalName.BodyEntered);

    public static SignalAwaiter AwaitBodyExited(this Area2D target) =>
        target.ToSignal(target, Area2D.SignalName.BodyExited);

    public static SignalAwaiter AwaitBodyShapeEntered(this Area2D target) =>
        target.ToSignal(target, Area2D.SignalName.BodyShapeEntered);

    public static SignalAwaiter AwaitBodyShapeExited(this Area2D target) =>
        target.ToSignal(target, Area2D.SignalName.BodyShapeExited);

    public static SignalAwaiter AwaitAreaEntered(this Area3D target) =>
        target.ToSignal(target, Area3D.SignalName.AreaEntered);

    public static SignalAwaiter AwaitAreaExited(this Area3D target) =>
        target.ToSignal(target, Area3D.SignalName.AreaExited);

    public static SignalAwaiter AwaitAreaShapeEntered(this Area3D target) =>
        target.ToSignal(target, Area3D.SignalName.AreaShapeEntered);

    public static SignalAwaiter AwaitAreaShapeExited(this Area3D target) =>
        target.ToSignal(target, Area3D.SignalName.AreaShapeExited);

    public static SignalAwaiter AwaitBodyEntered(this Area3D target) =>
        target.ToSignal(target, Area3D.SignalName.BodyEntered);

    public static SignalAwaiter AwaitBodyExited(this Area3D target) =>
        target.ToSignal(target, Area3D.SignalName.BodyExited);

    public static SignalAwaiter AwaitBodyShapeEntered(this Area3D target) =>
        target.ToSignal(target, Area3D.SignalName.BodyShapeEntered);

    public static SignalAwaiter AwaitBodyShapeExited(this Area3D target) =>
        target.ToSignal(target, Area3D.SignalName.BodyShapeExited);

    public static SignalAwaiter AwaitAudioServerBusLayoutChanged() =>
        AudioServer.Singleton.ToSignal(AudioServer.Singleton, AudioServer.SignalName.BusLayoutChanged);

    public static SignalAwaiter AwaitFinished(this AudioStreamPlayer target) =>
        target.ToSignal(target, AudioStreamPlayer.SignalName.Finished);

    public static SignalAwaiter AwaitFinished(this AudioStreamPlayer2D target) =>
        target.ToSignal(target, AudioStreamPlayer2D.SignalName.Finished);

    public static SignalAwaiter AwaitFinished(this AudioStreamPlayer3D target) =>
        target.ToSignal(target, AudioStreamPlayer3D.SignalName.Finished);

    public static SignalAwaiter AwaitButtonDown(this BaseButton target) =>
        target.ToSignal(target, BaseButton.SignalName.ButtonDown);

    public static SignalAwaiter AwaitButtonUp(this BaseButton target) =>
        target.ToSignal(target, BaseButton.SignalName.ButtonUp);

    public static SignalAwaiter AwaitPressed(this BaseButton target) =>
        target.ToSignal(target, BaseButton.SignalName.Pressed);

    public static SignalAwaiter AwaitToggled(this BaseButton target) =>
        target.ToSignal(target, BaseButton.SignalName.Toggled);

    public static SignalAwaiter AwaitBoneMapUpdated(this BoneMap target) =>
        target.ToSignal(target, BoneMap.SignalName.BoneMapUpdated);

    public static SignalAwaiter AwaitProfileUpdated(this BoneMap target) =>
        target.ToSignal(target, BoneMap.SignalName.ProfileUpdated);

    public static SignalAwaiter AwaitPressed(this ButtonGroup target) =>
        target.ToSignal(target, ButtonGroup.SignalName.Pressed);

    public static SignalAwaiter AwaitCameraServerCameraFeedAdded() =>
        CameraServer.Singleton.ToSignal(CameraServer.Singleton, CameraServer.SignalName.CameraFeedAdded);

    public static SignalAwaiter AwaitCameraServerCameraFeedRemoved() =>
        CameraServer.Singleton.ToSignal(CameraServer.Singleton, CameraServer.SignalName.CameraFeedRemoved);

    public static SignalAwaiter AwaitDraw(this CanvasItem target) =>
        target.ToSignal(target, CanvasItem.SignalName.Draw);

    public static SignalAwaiter AwaitHidden(this CanvasItem target) =>
        target.ToSignal(target, CanvasItem.SignalName.Hidden);

    public static SignalAwaiter AwaitItemRectChanged(this CanvasItem target) =>
        target.ToSignal(target, CanvasItem.SignalName.ItemRectChanged);

    public static SignalAwaiter AwaitVisibilityChanged(this CanvasItem target) =>
        target.ToSignal(target, CanvasItem.SignalName.VisibilityChanged);

    public static SignalAwaiter AwaitVisibilityChanged(this CanvasLayer target) =>
        target.ToSignal(target, CanvasLayer.SignalName.VisibilityChanged);

    public static SignalAwaiter AwaitBreakpointToggled(this CodeEdit target) =>
        target.ToSignal(target, CodeEdit.SignalName.BreakpointToggled);

    public static SignalAwaiter AwaitCodeCompletionRequested(this CodeEdit target) =>
        target.ToSignal(target, CodeEdit.SignalName.CodeCompletionRequested);

    public static SignalAwaiter AwaitSymbolLookup(this CodeEdit target) =>
        target.ToSignal(target, CodeEdit.SignalName.SymbolLookup);

    public static SignalAwaiter AwaitSymbolValidate(this CodeEdit target) =>
        target.ToSignal(target, CodeEdit.SignalName.SymbolValidate);

    public static SignalAwaiter AwaitInputEvent(this CollisionObject2D target) =>
        target.ToSignal(target, CollisionObject2D.SignalName.InputEvent);

    public static SignalAwaiter AwaitMouseEntered(this CollisionObject2D target) =>
        target.ToSignal(target, CollisionObject2D.SignalName.MouseEntered);

    public static SignalAwaiter AwaitMouseExited(this CollisionObject2D target) =>
        target.ToSignal(target, CollisionObject2D.SignalName.MouseExited);

    public static SignalAwaiter AwaitMouseShapeEntered(this CollisionObject2D target) =>
        target.ToSignal(target, CollisionObject2D.SignalName.MouseShapeEntered);

    public static SignalAwaiter AwaitMouseShapeExited(this CollisionObject2D target) =>
        target.ToSignal(target, CollisionObject2D.SignalName.MouseShapeExited);

    public static SignalAwaiter AwaitInputEvent(this CollisionObject3D target) =>
        target.ToSignal(target, CollisionObject3D.SignalName.InputEvent);

    public static SignalAwaiter AwaitMouseEntered(this CollisionObject3D target) =>
        target.ToSignal(target, CollisionObject3D.SignalName.MouseEntered);

    public static SignalAwaiter AwaitMouseExited(this CollisionObject3D target) =>
        target.ToSignal(target, CollisionObject3D.SignalName.MouseExited);

    public static SignalAwaiter AwaitColorChanged(this ColorPicker target) =>
        target.ToSignal(target, ColorPicker.SignalName.ColorChanged);

    public static SignalAwaiter AwaitPresetAdded(this ColorPicker target) =>
        target.ToSignal(target, ColorPicker.SignalName.PresetAdded);

    public static SignalAwaiter AwaitPresetRemoved(this ColorPicker target) =>
        target.ToSignal(target, ColorPicker.SignalName.PresetRemoved);

    public static SignalAwaiter AwaitColorChanged(this ColorPickerButton target) =>
        target.ToSignal(target, ColorPickerButton.SignalName.ColorChanged);

    public static SignalAwaiter AwaitPickerCreated(this ColorPickerButton target) =>
        target.ToSignal(target, ColorPickerButton.SignalName.PickerCreated);

    public static SignalAwaiter AwaitPopupClosed(this ColorPickerButton target) =>
        target.ToSignal(target, ColorPickerButton.SignalName.PopupClosed);

    public static SignalAwaiter AwaitPreSortChildren(this Container target) =>
        target.ToSignal(target, Container.SignalName.PreSortChildren);

    public static SignalAwaiter AwaitSortChildren(this Container target) =>
        target.ToSignal(target, Container.SignalName.SortChildren);

    public static SignalAwaiter AwaitFocusEntered(this Control target) =>
        target.ToSignal(target, Control.SignalName.FocusEntered);

    public static SignalAwaiter AwaitFocusExited(this Control target) =>
        target.ToSignal(target, Control.SignalName.FocusExited);

    public static SignalAwaiter AwaitGuiInput(this Control target) =>
        target.ToSignal(target, Control.SignalName.GuiInput);

    public static SignalAwaiter AwaitMinimumSizeChanged(this Control target) =>
        target.ToSignal(target, Control.SignalName.MinimumSizeChanged);

    public static SignalAwaiter AwaitMouseEntered(this Control target) =>
        target.ToSignal(target, Control.SignalName.MouseEntered);

    public static SignalAwaiter AwaitMouseExited(this Control target) =>
        target.ToSignal(target, Control.SignalName.MouseExited);

    public static SignalAwaiter AwaitResized(this Control target) =>
        target.ToSignal(target, Control.SignalName.Resized);

    public static SignalAwaiter AwaitSizeFlagsChanged(this Control target) =>
        target.ToSignal(target, Control.SignalName.SizeFlagsChanged);

    public static SignalAwaiter AwaitThemeChanged(this Control target) =>
        target.ToSignal(target, Control.SignalName.ThemeChanged);

    public static SignalAwaiter AwaitRangeChanged(this Curve target) =>
        target.ToSignal(target, Curve.SignalName.RangeChanged);

    public static SignalAwaiter AwaitDirSelected(this FileDialog target) =>
        target.ToSignal(target, FileDialog.SignalName.DirSelected);

    public static SignalAwaiter AwaitFileSelected(this FileDialog target) =>
        target.ToSignal(target, FileDialog.SignalName.FileSelected);

    public static SignalAwaiter AwaitFilesSelected(this FileDialog target) =>
        target.ToSignal(target, FileDialog.SignalName.FilesSelected);

    public static SignalAwaiter AwaitBeginNodeMove(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.BeginNodeMove);

    public static SignalAwaiter AwaitConnectionDragEnded(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.ConnectionDragEnded);

    public static SignalAwaiter AwaitConnectionDragStarted(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.ConnectionDragStarted);

    public static SignalAwaiter AwaitConnectionFromEmpty(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.ConnectionFromEmpty);

    public static SignalAwaiter AwaitConnectionRequest(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.ConnectionRequest);

    public static SignalAwaiter AwaitConnectionToEmpty(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.ConnectionToEmpty);

    public static SignalAwaiter AwaitCopyNodesRequest(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.CopyNodesRequest);

    public static SignalAwaiter AwaitDeleteNodesRequest(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.DeleteNodesRequest);

    public static SignalAwaiter AwaitDisconnectionRequest(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.DisconnectionRequest);

    public static SignalAwaiter AwaitDuplicateNodesRequest(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.DuplicateNodesRequest);

    public static SignalAwaiter AwaitEndNodeMove(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.EndNodeMove);

    public static SignalAwaiter AwaitNodeDeselected(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.NodeDeselected);

    public static SignalAwaiter AwaitNodeSelected(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.NodeSelected);

    public static SignalAwaiter AwaitPasteNodesRequest(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.PasteNodesRequest);

    public static SignalAwaiter AwaitPopupRequest(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.PopupRequest);

    public static SignalAwaiter AwaitScrollOffsetChanged(this GraphEdit target) =>
        target.ToSignal(target, GraphEdit.SignalName.ScrollOffsetChanged);

    public static SignalAwaiter AwaitCloseRequest(this GraphNode target) =>
        target.ToSignal(target, GraphNode.SignalName.CloseRequest);

    public static SignalAwaiter AwaitDragged(this GraphNode target) =>
        target.ToSignal(target, GraphNode.SignalName.Dragged);

    public static SignalAwaiter AwaitNodeDeselected(this GraphNode target) =>
        target.ToSignal(target, GraphNode.SignalName.NodeDeselected);

    public static SignalAwaiter AwaitNodeSelected(this GraphNode target) =>
        target.ToSignal(target, GraphNode.SignalName.NodeSelected);

    public static SignalAwaiter AwaitPositionOffsetChanged(this GraphNode target) =>
        target.ToSignal(target, GraphNode.SignalName.PositionOffsetChanged);

    public static SignalAwaiter AwaitRaiseRequest(this GraphNode target) =>
        target.ToSignal(target, GraphNode.SignalName.RaiseRequest);

    public static SignalAwaiter AwaitResizeRequest(this GraphNode target) =>
        target.ToSignal(target, GraphNode.SignalName.ResizeRequest);

    public static SignalAwaiter AwaitSlotUpdated(this GraphNode target) =>
        target.ToSignal(target, GraphNode.SignalName.SlotUpdated);

    public static SignalAwaiter AwaitCellSizeChanged(this GridMap target) =>
        target.ToSignal(target, GridMap.SignalName.CellSizeChanged);

    public static SignalAwaiter AwaitInputJoyConnectionChanged() =>
        Input.Singleton.ToSignal(Input.Singleton, Input.SignalName.JoyConnectionChanged);

    public static SignalAwaiter AwaitEmptyClicked(this ItemList target) =>
        target.ToSignal(target, ItemList.SignalName.EmptyClicked);

    public static SignalAwaiter AwaitItemActivated(this ItemList target) =>
        target.ToSignal(target, ItemList.SignalName.ItemActivated);

    public static SignalAwaiter AwaitItemClicked(this ItemList target) =>
        target.ToSignal(target, ItemList.SignalName.ItemClicked);

    public static SignalAwaiter AwaitItemSelected(this ItemList target) =>
        target.ToSignal(target, ItemList.SignalName.ItemSelected);

    public static SignalAwaiter AwaitMultiSelected(this ItemList target) =>
        target.ToSignal(target, ItemList.SignalName.MultiSelected);

    public static SignalAwaiter AwaitJavaScriptBridgePwaUpdateAvailable() =>
        JavaScriptBridge.Singleton.ToSignal(JavaScriptBridge.Singleton, JavaScriptBridge.SignalName.PwaUpdateAvailable);

    public static SignalAwaiter AwaitTextChanged(this LineEdit target) =>
        target.ToSignal(target, LineEdit.SignalName.TextChanged);

    public static SignalAwaiter AwaitTextChangeRejected(this LineEdit target) =>
        target.ToSignal(target, LineEdit.SignalName.TextChangeRejected);

    public static SignalAwaiter AwaitTextSubmitted(this LineEdit target) =>
        target.ToSignal(target, LineEdit.SignalName.TextSubmitted);

    public static SignalAwaiter AwaitOnRequestPermissionsResult(this MainLoop target) =>
        target.ToSignal(target, MainLoop.SignalName.OnRequestPermissionsResult);

    public static SignalAwaiter AwaitAboutToPopup(this MenuButton target) =>
        target.ToSignal(target, MenuButton.SignalName.AboutToPopup);

    public static SignalAwaiter AwaitTextureChanged(this MeshInstance2D target) =>
        target.ToSignal(target, MeshInstance2D.SignalName.TextureChanged);

    public static SignalAwaiter AwaitTextureChanged(this MultiMeshInstance2D target) =>
        target.ToSignal(target, MultiMeshInstance2D.SignalName.TextureChanged);

    public static SignalAwaiter AwaitPeerConnected(this MultiplayerPeer target) =>
        target.ToSignal(target, MultiplayerPeer.SignalName.PeerConnected);

    public static SignalAwaiter AwaitPeerDisconnected(this MultiplayerPeer target) =>
        target.ToSignal(target, MultiplayerPeer.SignalName.PeerDisconnected);

    public static SignalAwaiter AwaitDespawned(this MultiplayerSpawner target) =>
        target.ToSignal(target, MultiplayerSpawner.SignalName.Despawned);

    public static SignalAwaiter AwaitSpawned(this MultiplayerSpawner target) =>
        target.ToSignal(target, MultiplayerSpawner.SignalName.Spawned);

    public static SignalAwaiter AwaitSynchronized(this MultiplayerSynchronizer target) =>
        target.ToSignal(target, MultiplayerSynchronizer.SignalName.Synchronized);

    public static SignalAwaiter AwaitVisibilityChanged(this MultiplayerSynchronizer target) =>
        target.ToSignal(target, MultiplayerSynchronizer.SignalName.VisibilityChanged);

    public static SignalAwaiter AwaitLinkReached(this NavigationAgent2D target) =>
        target.ToSignal(target, NavigationAgent2D.SignalName.LinkReached);

    public static SignalAwaiter AwaitNavigationFinished(this NavigationAgent2D target) =>
        target.ToSignal(target, NavigationAgent2D.SignalName.NavigationFinished);

    public static SignalAwaiter AwaitPathChanged(this NavigationAgent2D target) =>
        target.ToSignal(target, NavigationAgent2D.SignalName.PathChanged);

    public static SignalAwaiter AwaitTargetReached(this NavigationAgent2D target) =>
        target.ToSignal(target, NavigationAgent2D.SignalName.TargetReached);

    public static SignalAwaiter AwaitVelocityComputed(this NavigationAgent2D target) =>
        target.ToSignal(target, NavigationAgent2D.SignalName.VelocityComputed);

    public static SignalAwaiter AwaitWaypointReached(this NavigationAgent2D target) =>
        target.ToSignal(target, NavigationAgent2D.SignalName.WaypointReached);

    public static SignalAwaiter AwaitLinkReached(this NavigationAgent3D target) =>
        target.ToSignal(target, NavigationAgent3D.SignalName.LinkReached);

    public static SignalAwaiter AwaitNavigationFinished(this NavigationAgent3D target) =>
        target.ToSignal(target, NavigationAgent3D.SignalName.NavigationFinished);

    public static SignalAwaiter AwaitPathChanged(this NavigationAgent3D target) =>
        target.ToSignal(target, NavigationAgent3D.SignalName.PathChanged);

    public static SignalAwaiter AwaitTargetReached(this NavigationAgent3D target) =>
        target.ToSignal(target, NavigationAgent3D.SignalName.TargetReached);

    public static SignalAwaiter AwaitVelocityComputed(this NavigationAgent3D target) =>
        target.ToSignal(target, NavigationAgent3D.SignalName.VelocityComputed);

    public static SignalAwaiter AwaitWaypointReached(this NavigationAgent3D target) =>
        target.ToSignal(target, NavigationAgent3D.SignalName.WaypointReached);

    public static SignalAwaiter AwaitBakeFinished(this NavigationRegion3D target) =>
        target.ToSignal(target, NavigationRegion3D.SignalName.BakeFinished);

    public static SignalAwaiter AwaitNavigationMeshChanged(this NavigationRegion3D target) =>
        target.ToSignal(target, NavigationRegion3D.SignalName.NavigationMeshChanged);

    public static SignalAwaiter AwaitNavigationServer2DMapChanged() =>
        NavigationServer2D.Singleton.ToSignal(NavigationServer2D.Singleton, NavigationServer2D.SignalName.MapChanged);

    public static SignalAwaiter AwaitNavigationServer2DNavigationDebugChanged() =>
        NavigationServer2D.Singleton.ToSignal(NavigationServer2D.Singleton, NavigationServer2D.SignalName.NavigationDebugChanged);

    public static SignalAwaiter AwaitNavigationServer3DMapChanged() =>
        NavigationServer3D.Singleton.ToSignal(NavigationServer3D.Singleton, NavigationServer3D.SignalName.MapChanged);

    public static SignalAwaiter AwaitNavigationServer3DNavigationDebugChanged() =>
        NavigationServer3D.Singleton.ToSignal(NavigationServer3D.Singleton, NavigationServer3D.SignalName.NavigationDebugChanged);

    public static SignalAwaiter AwaitTextureChanged(this NinePatchRect target) =>
        target.ToSignal(target, NinePatchRect.SignalName.TextureChanged);

    public static SignalAwaiter AwaitChildEnteredTree(this Node target) =>
        target.ToSignal(target, Node.SignalName.ChildEnteredTree);

    public static SignalAwaiter AwaitChildExitingTree(this Node target) =>
        target.ToSignal(target, Node.SignalName.ChildExitingTree);

    public static SignalAwaiter AwaitReady(this Node target) =>
        target.ToSignal(target, Node.SignalName.Ready);

    public static SignalAwaiter AwaitRenamed(this Node target) =>
        target.ToSignal(target, Node.SignalName.Renamed);

    public static SignalAwaiter AwaitTreeEntered(this Node target) =>
        target.ToSignal(target, Node.SignalName.TreeEntered);

    public static SignalAwaiter AwaitTreeExited(this Node target) =>
        target.ToSignal(target, Node.SignalName.TreeExited);

    public static SignalAwaiter AwaitTreeExiting(this Node target) =>
        target.ToSignal(target, Node.SignalName.TreeExiting);

    public static SignalAwaiter AwaitVisibilityChanged(this Node3D target) =>
        target.ToSignal(target, Node3D.SignalName.VisibilityChanged);

    public static SignalAwaiter AwaitItemFocused(this OptionButton target) =>
        target.ToSignal(target, OptionButton.SignalName.ItemFocused);

    public static SignalAwaiter AwaitItemSelected(this OptionButton target) =>
        target.ToSignal(target, OptionButton.SignalName.ItemSelected);

    public static SignalAwaiter AwaitCurveChanged(this Path3D target) =>
        target.ToSignal(target, Path3D.SignalName.CurveChanged);

    public static SignalAwaiter AwaitPopupHide(this Popup target) =>
        target.ToSignal(target, Popup.SignalName.PopupHide);

    public static SignalAwaiter AwaitIdFocused(this PopupMenu target) =>
        target.ToSignal(target, PopupMenu.SignalName.IdFocused);

    public static SignalAwaiter AwaitIdPressed(this PopupMenu target) =>
        target.ToSignal(target, PopupMenu.SignalName.IdPressed);

    public static SignalAwaiter AwaitIndexPressed(this PopupMenu target) =>
        target.ToSignal(target, PopupMenu.SignalName.IndexPressed);

    public static SignalAwaiter AwaitMenuChanged(this PopupMenu target) =>
        target.ToSignal(target, PopupMenu.SignalName.MenuChanged);

    public static SignalAwaiter AwaitChanged(this Range target) =>
        target.ToSignal(target, Range.SignalName.Changed);

    public static SignalAwaiter AwaitValueChanged(this Range target) =>
        target.ToSignal(target, Range.SignalName.ValueChanged);

    public static SignalAwaiter AwaitRenderingServerFramePostDraw() =>
        RenderingServer.Singleton.ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);

    public static SignalAwaiter AwaitRenderingServerFramePreDraw() =>
        RenderingServer.Singleton.ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePreDraw);

    public static SignalAwaiter AwaitChanged(this Resource target) =>
        target.ToSignal(target, Resource.SignalName.Changed);

    public static SignalAwaiter AwaitSetupLocalToSceneRequested(this Resource target) =>
        target.ToSignal(target, Resource.SignalName.SetupLocalToSceneRequested);

    public static SignalAwaiter AwaitFinished(this RichTextLabel target) =>
        target.ToSignal(target, RichTextLabel.SignalName.Finished);

    public static SignalAwaiter AwaitMetaClicked(this RichTextLabel target) =>
        target.ToSignal(target, RichTextLabel.SignalName.MetaClicked);

    public static SignalAwaiter AwaitMetaHoverEnded(this RichTextLabel target) =>
        target.ToSignal(target, RichTextLabel.SignalName.MetaHoverEnded);

    public static SignalAwaiter AwaitMetaHoverStarted(this RichTextLabel target) =>
        target.ToSignal(target, RichTextLabel.SignalName.MetaHoverStarted);

    public static SignalAwaiter AwaitBodyEntered(this RigidBody2D target) =>
        target.ToSignal(target, RigidBody2D.SignalName.BodyEntered);

    public static SignalAwaiter AwaitBodyExited(this RigidBody2D target) =>
        target.ToSignal(target, RigidBody2D.SignalName.BodyExited);

    public static SignalAwaiter AwaitBodyShapeEntered(this RigidBody2D target) =>
        target.ToSignal(target, RigidBody2D.SignalName.BodyShapeEntered);

    public static SignalAwaiter AwaitBodyShapeExited(this RigidBody2D target) =>
        target.ToSignal(target, RigidBody2D.SignalName.BodyShapeExited);

    public static SignalAwaiter AwaitSleepingStateChanged(this RigidBody2D target) =>
        target.ToSignal(target, RigidBody2D.SignalName.SleepingStateChanged);

    public static SignalAwaiter AwaitBodyEntered(this RigidBody3D target) =>
        target.ToSignal(target, RigidBody3D.SignalName.BodyEntered);

    public static SignalAwaiter AwaitBodyExited(this RigidBody3D target) =>
        target.ToSignal(target, RigidBody3D.SignalName.BodyExited);

    public static SignalAwaiter AwaitBodyShapeEntered(this RigidBody3D target) =>
        target.ToSignal(target, RigidBody3D.SignalName.BodyShapeEntered);

    public static SignalAwaiter AwaitBodyShapeExited(this RigidBody3D target) =>
        target.ToSignal(target, RigidBody3D.SignalName.BodyShapeExited);

    public static SignalAwaiter AwaitSleepingStateChanged(this RigidBody3D target) =>
        target.ToSignal(target, RigidBody3D.SignalName.SleepingStateChanged);

    public static SignalAwaiter AwaitPeerAuthenticating(this SceneMultiplayer target) =>
        target.ToSignal(target, SceneMultiplayer.SignalName.PeerAuthenticating);

    public static SignalAwaiter AwaitPeerAuthenticationFailed(this SceneMultiplayer target) =>
        target.ToSignal(target, SceneMultiplayer.SignalName.PeerAuthenticationFailed);

    public static SignalAwaiter AwaitPeerPacket(this SceneMultiplayer target) =>
        target.ToSignal(target, SceneMultiplayer.SignalName.PeerPacket);

    public static SignalAwaiter AwaitNodeAdded(this SceneTree target) =>
        target.ToSignal(target, SceneTree.SignalName.NodeAdded);

    public static SignalAwaiter AwaitNodeConfigurationWarningChanged(this SceneTree target) =>
        target.ToSignal(target, SceneTree.SignalName.NodeConfigurationWarningChanged);

    public static SignalAwaiter AwaitNodeRemoved(this SceneTree target) =>
        target.ToSignal(target, SceneTree.SignalName.NodeRemoved);

    public static SignalAwaiter AwaitNodeRenamed(this SceneTree target) =>
        target.ToSignal(target, SceneTree.SignalName.NodeRenamed);

    public static SignalAwaiter AwaitPhysicsFrame(this SceneTree target) =>
        target.ToSignal(target, SceneTree.SignalName.PhysicsFrame);

    public static SignalAwaiter AwaitProcessFrame(this SceneTree target) =>
        target.ToSignal(target, SceneTree.SignalName.ProcessFrame);

    public static SignalAwaiter AwaitTreeChanged(this SceneTree target) =>
        target.ToSignal(target, SceneTree.SignalName.TreeChanged);

    public static SignalAwaiter AwaitTreeProcessModeChanged(this SceneTree target) =>
        target.ToSignal(target, SceneTree.SignalName.TreeProcessModeChanged);

    public static SignalAwaiter AwaitTimeout(this SceneTreeTimer target) =>
        target.ToSignal(target, SceneTreeTimer.SignalName.Timeout);

    public static SignalAwaiter AwaitScrolling(this ScrollBar target) =>
        target.ToSignal(target, ScrollBar.SignalName.Scrolling);

    public static SignalAwaiter AwaitScrollEnded(this ScrollContainer target) =>
        target.ToSignal(target, ScrollContainer.SignalName.ScrollEnded);

    public static SignalAwaiter AwaitScrollStarted(this ScrollContainer target) =>
        target.ToSignal(target, ScrollContainer.SignalName.ScrollStarted);

    public static SignalAwaiter AwaitBoneSetupChanged(this Skeleton2D target) =>
        target.ToSignal(target, Skeleton2D.SignalName.BoneSetupChanged);

    public static SignalAwaiter AwaitBoneEnabledChanged(this Skeleton3D target) =>
        target.ToSignal(target, Skeleton3D.SignalName.BoneEnabledChanged);

    public static SignalAwaiter AwaitBonePoseChanged(this Skeleton3D target) =>
        target.ToSignal(target, Skeleton3D.SignalName.BonePoseChanged);

    public static SignalAwaiter AwaitPoseUpdated(this Skeleton3D target) =>
        target.ToSignal(target, Skeleton3D.SignalName.PoseUpdated);

    public static SignalAwaiter AwaitShowRestOnlyChanged(this Skeleton3D target) =>
        target.ToSignal(target, Skeleton3D.SignalName.ShowRestOnlyChanged);

    public static SignalAwaiter AwaitProfileUpdated(this SkeletonProfile target) =>
        target.ToSignal(target, SkeletonProfile.SignalName.ProfileUpdated);

    public static SignalAwaiter AwaitDragEnded(this Slider target) =>
        target.ToSignal(target, Slider.SignalName.DragEnded);

    public static SignalAwaiter AwaitDragStarted(this Slider target) =>
        target.ToSignal(target, Slider.SignalName.DragStarted);

    public static SignalAwaiter AwaitDragged(this SplitContainer target) =>
        target.ToSignal(target, SplitContainer.SignalName.Dragged);

    public static SignalAwaiter AwaitFrameChanged(this Sprite2D target) =>
        target.ToSignal(target, Sprite2D.SignalName.FrameChanged);

    public static SignalAwaiter AwaitTextureChanged(this Sprite2D target) =>
        target.ToSignal(target, Sprite2D.SignalName.TextureChanged);

    public static SignalAwaiter AwaitFrameChanged(this Sprite3D target) =>
        target.ToSignal(target, Sprite3D.SignalName.FrameChanged);

    public static SignalAwaiter AwaitTextureChanged(this Sprite3D target) =>
        target.ToSignal(target, Sprite3D.SignalName.TextureChanged);

    public static SignalAwaiter AwaitActiveTabRearranged(this TabBar target) =>
        target.ToSignal(target, TabBar.SignalName.ActiveTabRearranged);

    public static SignalAwaiter AwaitTabButtonPressed(this TabBar target) =>
        target.ToSignal(target, TabBar.SignalName.TabButtonPressed);

    public static SignalAwaiter AwaitTabChanged(this TabBar target) =>
        target.ToSignal(target, TabBar.SignalName.TabChanged);

    public static SignalAwaiter AwaitTabClicked(this TabBar target) =>
        target.ToSignal(target, TabBar.SignalName.TabClicked);

    public static SignalAwaiter AwaitTabClosePressed(this TabBar target) =>
        target.ToSignal(target, TabBar.SignalName.TabClosePressed);

    public static SignalAwaiter AwaitTabHovered(this TabBar target) =>
        target.ToSignal(target, TabBar.SignalName.TabHovered);

    public static SignalAwaiter AwaitTabRmbClicked(this TabBar target) =>
        target.ToSignal(target, TabBar.SignalName.TabRmbClicked);

    public static SignalAwaiter AwaitTabSelected(this TabBar target) =>
        target.ToSignal(target, TabBar.SignalName.TabSelected);

    public static SignalAwaiter AwaitPrePopupPressed(this TabContainer target) =>
        target.ToSignal(target, TabContainer.SignalName.PrePopupPressed);

    public static SignalAwaiter AwaitTabButtonPressed(this TabContainer target) =>
        target.ToSignal(target, TabContainer.SignalName.TabButtonPressed);

    public static SignalAwaiter AwaitTabChanged(this TabContainer target) =>
        target.ToSignal(target, TabContainer.SignalName.TabChanged);

    public static SignalAwaiter AwaitTabSelected(this TabContainer target) =>
        target.ToSignal(target, TabContainer.SignalName.TabSelected);

    public static SignalAwaiter AwaitCaretChanged(this TextEdit target) =>
        target.ToSignal(target, TextEdit.SignalName.CaretChanged);

    public static SignalAwaiter AwaitGutterAdded(this TextEdit target) =>
        target.ToSignal(target, TextEdit.SignalName.GutterAdded);

    public static SignalAwaiter AwaitGutterClicked(this TextEdit target) =>
        target.ToSignal(target, TextEdit.SignalName.GutterClicked);

    public static SignalAwaiter AwaitGutterRemoved(this TextEdit target) =>
        target.ToSignal(target, TextEdit.SignalName.GutterRemoved);

    public static SignalAwaiter AwaitLinesEditedFrom(this TextEdit target) =>
        target.ToSignal(target, TextEdit.SignalName.LinesEditedFrom);

    public static SignalAwaiter AwaitTextChanged(this TextEdit target) =>
        target.ToSignal(target, TextEdit.SignalName.TextChanged);

    public static SignalAwaiter AwaitTextSet(this TextEdit target) =>
        target.ToSignal(target, TextEdit.SignalName.TextSet);

    public static SignalAwaiter AwaitTextServerManagerInterfaceAdded() =>
        TextServerManager.Singleton.ToSignal(TextServerManager.Singleton, TextServerManager.SignalName.InterfaceAdded);

    public static SignalAwaiter AwaitTextServerManagerInterfaceRemoved() =>
        TextServerManager.Singleton.ToSignal(TextServerManager.Singleton, TextServerManager.SignalName.InterfaceRemoved);

    public static SignalAwaiter AwaitThemeDBFallbackChanged() =>
        ThemeDB.Singleton.ToSignal(ThemeDB.Singleton, ThemeDB.SignalName.FallbackChanged);

    public static SignalAwaiter AwaitChanged(this TileData target) =>
        target.ToSignal(target, TileData.SignalName.Changed);

    public static SignalAwaiter AwaitChanged(this TileMap target) =>
        target.ToSignal(target, TileMap.SignalName.Changed);

    public static SignalAwaiter AwaitTimeout(this Timer target) =>
        target.ToSignal(target, Timer.SignalName.Timeout);

    public static SignalAwaiter AwaitPressed(this TouchScreenButton target) =>
        target.ToSignal(target, TouchScreenButton.SignalName.Pressed);

    public static SignalAwaiter AwaitReleased(this TouchScreenButton target) =>
        target.ToSignal(target, TouchScreenButton.SignalName.Released);

    public static SignalAwaiter AwaitButtonClicked(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.ButtonClicked);

    public static SignalAwaiter AwaitCellSelected(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.CellSelected);

    public static SignalAwaiter AwaitCheckPropagatedToItem(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.CheckPropagatedToItem);

    public static SignalAwaiter AwaitColumnTitleClicked(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.ColumnTitleClicked);

    public static SignalAwaiter AwaitCustomItemClicked(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.CustomItemClicked);

    public static SignalAwaiter AwaitCustomPopupEdited(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.CustomPopupEdited);

    public static SignalAwaiter AwaitEmptyClicked(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.EmptyClicked);

    public static SignalAwaiter AwaitItemActivated(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.ItemActivated);

    public static SignalAwaiter AwaitItemCollapsed(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.ItemCollapsed);

    public static SignalAwaiter AwaitItemEdited(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.ItemEdited);

    public static SignalAwaiter AwaitItemIconDoubleClicked(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.ItemIconDoubleClicked);

    public static SignalAwaiter AwaitItemMouseSelected(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.ItemMouseSelected);

    public static SignalAwaiter AwaitItemSelected(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.ItemSelected);

    public static SignalAwaiter AwaitMultiSelected(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.MultiSelected);

    public static SignalAwaiter AwaitNothingSelected(this Tree target) =>
        target.ToSignal(target, Tree.SignalName.NothingSelected);

    public static SignalAwaiter AwaitFinished(this Tween target) =>
        target.ToSignal(target, Tween.SignalName.Finished);

    public static SignalAwaiter AwaitLoopFinished(this Tween target) =>
        target.ToSignal(target, Tween.SignalName.LoopFinished);

    public static SignalAwaiter AwaitStepFinished(this Tween target) =>
        target.ToSignal(target, Tween.SignalName.StepFinished);

    public static SignalAwaiter AwaitFinished(this Tweener target) =>
        target.ToSignal(target, Tweener.SignalName.Finished);

    public static SignalAwaiter AwaitVersionChanged(this UndoRedo target) =>
        target.ToSignal(target, UndoRedo.SignalName.VersionChanged);

    public static SignalAwaiter AwaitFinished(this VideoStreamPlayer target) =>
        target.ToSignal(target, VideoStreamPlayer.SignalName.Finished);

    public static SignalAwaiter AwaitGuiFocusChanged(this Viewport target) =>
        target.ToSignal(target, Viewport.SignalName.GuiFocusChanged);

    public static SignalAwaiter AwaitSizeChanged(this Viewport target) =>
        target.ToSignal(target, Viewport.SignalName.SizeChanged);

    public static SignalAwaiter AwaitScreenEntered(this VisibleOnScreenNotifier2D target) =>
        target.ToSignal(target, VisibleOnScreenNotifier2D.SignalName.ScreenEntered);

    public static SignalAwaiter AwaitScreenExited(this VisibleOnScreenNotifier2D target) =>
        target.ToSignal(target, VisibleOnScreenNotifier2D.SignalName.ScreenExited);

    public static SignalAwaiter AwaitScreenEntered(this VisibleOnScreenNotifier3D target) =>
        target.ToSignal(target, VisibleOnScreenNotifier3D.SignalName.ScreenEntered);

    public static SignalAwaiter AwaitScreenExited(this VisibleOnScreenNotifier3D target) =>
        target.ToSignal(target, VisibleOnScreenNotifier3D.SignalName.ScreenExited);

    public static SignalAwaiter AwaitInputTypeChanged(this VisualShaderNodeInput target) =>
        target.ToSignal(target, VisualShaderNodeInput.SignalName.InputTypeChanged);

    public static SignalAwaiter AwaitReferenceSpaceReset(this WebXRInterface target) =>
        target.ToSignal(target, WebXRInterface.SignalName.ReferenceSpaceReset);

    public static SignalAwaiter AwaitSelect(this WebXRInterface target) =>
        target.ToSignal(target, WebXRInterface.SignalName.Select);

    public static SignalAwaiter AwaitSelectend(this WebXRInterface target) =>
        target.ToSignal(target, WebXRInterface.SignalName.Selectend);

    public static SignalAwaiter AwaitSelectstart(this WebXRInterface target) =>
        target.ToSignal(target, WebXRInterface.SignalName.Selectstart);

    public static SignalAwaiter AwaitSessionEnded(this WebXRInterface target) =>
        target.ToSignal(target, WebXRInterface.SignalName.SessionEnded);

    public static SignalAwaiter AwaitSessionFailed(this WebXRInterface target) =>
        target.ToSignal(target, WebXRInterface.SignalName.SessionFailed);

    public static SignalAwaiter AwaitSessionStarted(this WebXRInterface target) =>
        target.ToSignal(target, WebXRInterface.SignalName.SessionStarted);

    public static SignalAwaiter AwaitSessionSupported(this WebXRInterface target) =>
        target.ToSignal(target, WebXRInterface.SignalName.SessionSupported);

    public static SignalAwaiter AwaitSqueeze(this WebXRInterface target) =>
        target.ToSignal(target, WebXRInterface.SignalName.Squeeze);

    public static SignalAwaiter AwaitSqueezeend(this WebXRInterface target) =>
        target.ToSignal(target, WebXRInterface.SignalName.Squeezeend);

    public static SignalAwaiter AwaitSqueezestart(this WebXRInterface target) =>
        target.ToSignal(target, WebXRInterface.SignalName.Squeezestart);

    public static SignalAwaiter AwaitVisibilityStateChanged(this WebXRInterface target) =>
        target.ToSignal(target, WebXRInterface.SignalName.VisibilityStateChanged);

    public static SignalAwaiter AwaitAboutToPopup(this Window target) =>
        target.ToSignal(target, Window.SignalName.AboutToPopup);

    public static SignalAwaiter AwaitCloseRequested(this Window target) =>
        target.ToSignal(target, Window.SignalName.CloseRequested);

    public static SignalAwaiter AwaitDpiChanged(this Window target) =>
        target.ToSignal(target, Window.SignalName.DpiChanged);

    public static SignalAwaiter AwaitFilesDropped(this Window target) =>
        target.ToSignal(target, Window.SignalName.FilesDropped);

    public static SignalAwaiter AwaitFocusEntered(this Window target) =>
        target.ToSignal(target, Window.SignalName.FocusEntered);

    public static SignalAwaiter AwaitFocusExited(this Window target) =>
        target.ToSignal(target, Window.SignalName.FocusExited);

    public static SignalAwaiter AwaitGoBackRequested(this Window target) =>
        target.ToSignal(target, Window.SignalName.GoBackRequested);

    public static SignalAwaiter AwaitMouseEntered(this Window target) =>
        target.ToSignal(target, Window.SignalName.MouseEntered);

    public static SignalAwaiter AwaitMouseExited(this Window target) =>
        target.ToSignal(target, Window.SignalName.MouseExited);

    public static SignalAwaiter AwaitThemeChanged(this Window target) =>
        target.ToSignal(target, Window.SignalName.ThemeChanged);

    public static SignalAwaiter AwaitTitlebarChanged(this Window target) =>
        target.ToSignal(target, Window.SignalName.TitlebarChanged);

    public static SignalAwaiter AwaitVisibilityChanged(this Window target) =>
        target.ToSignal(target, Window.SignalName.VisibilityChanged);

    public static SignalAwaiter AwaitWindowInput(this Window target) =>
        target.ToSignal(target, Window.SignalName.WindowInput);

    public static SignalAwaiter AwaitButtonPressed(this XRController3D target) =>
        target.ToSignal(target, XRController3D.SignalName.ButtonPressed);

    public static SignalAwaiter AwaitButtonReleased(this XRController3D target) =>
        target.ToSignal(target, XRController3D.SignalName.ButtonReleased);

    public static SignalAwaiter AwaitInputFloatChanged(this XRController3D target) =>
        target.ToSignal(target, XRController3D.SignalName.InputFloatChanged);

    public static SignalAwaiter AwaitInputVector2Changed(this XRController3D target) =>
        target.ToSignal(target, XRController3D.SignalName.InputVector2Changed);

    public static SignalAwaiter AwaitPlayAreaChanged(this XRInterface target) =>
        target.ToSignal(target, XRInterface.SignalName.PlayAreaChanged);

    public static SignalAwaiter AwaitButtonPressed(this XRPositionalTracker target) =>
        target.ToSignal(target, XRPositionalTracker.SignalName.ButtonPressed);

    public static SignalAwaiter AwaitButtonReleased(this XRPositionalTracker target) =>
        target.ToSignal(target, XRPositionalTracker.SignalName.ButtonReleased);

    public static SignalAwaiter AwaitInputFloatChanged(this XRPositionalTracker target) =>
        target.ToSignal(target, XRPositionalTracker.SignalName.InputFloatChanged);

    public static SignalAwaiter AwaitInputVector2Changed(this XRPositionalTracker target) =>
        target.ToSignal(target, XRPositionalTracker.SignalName.InputVector2Changed);

    public static SignalAwaiter AwaitPoseChanged(this XRPositionalTracker target) =>
        target.ToSignal(target, XRPositionalTracker.SignalName.PoseChanged);

    public static SignalAwaiter AwaitProfileChanged(this XRPositionalTracker target) =>
        target.ToSignal(target, XRPositionalTracker.SignalName.ProfileChanged);

    public static SignalAwaiter AwaitXRServerInterfaceAdded() =>
        XRServer.Singleton.ToSignal(XRServer.Singleton, XRServer.SignalName.InterfaceAdded);

    public static SignalAwaiter AwaitXRServerInterfaceRemoved() =>
        XRServer.Singleton.ToSignal(XRServer.Singleton, XRServer.SignalName.InterfaceRemoved);

    public static SignalAwaiter AwaitXRServerTrackerAdded() =>
        XRServer.Singleton.ToSignal(XRServer.Singleton, XRServer.SignalName.TrackerAdded);

    public static SignalAwaiter AwaitXRServerTrackerRemoved() =>
        XRServer.Singleton.ToSignal(XRServer.Singleton, XRServer.SignalName.TrackerRemoved);

    public static SignalAwaiter AwaitXRServerTrackerUpdated() =>
        XRServer.Singleton.ToSignal(XRServer.Singleton, XRServer.SignalName.TrackerUpdated);
}