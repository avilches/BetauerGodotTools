using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Range = Godot.Range;
using static Betauer.Core.Signal.SignalTools; 

namespace Betauer.Core.Signal;

public static partial class SignalExtensions {
  
    public static void OnCanceled(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AcceptDialog.SignalName.Canceled, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnConfirmed(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AcceptDialog.SignalName.Confirmed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCustomAction(this AcceptDialog target, Action<StringName> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AcceptDialog.SignalName.CustomAction, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationChanged(this AnimatedSprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimatedSprite2D.SignalName.AnimationChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationFinished(this AnimatedSprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimatedSprite2D.SignalName.AnimationFinished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationLooped(this AnimatedSprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimatedSprite2D.SignalName.AnimationLooped, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFrameChanged(this AnimatedSprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimatedSprite2D.SignalName.FrameChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSpriteFramesChanged(this AnimatedSprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimatedSprite2D.SignalName.SpriteFramesChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimatedSprite3D.SignalName.AnimationChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationFinished(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimatedSprite3D.SignalName.AnimationFinished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationLooped(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimatedSprite3D.SignalName.AnimationLooped, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFrameChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimatedSprite3D.SignalName.FrameChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSpriteFramesChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimatedSprite3D.SignalName.SpriteFramesChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationAdded(this AnimationLibrary target, Action<StringName> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationLibrary.SignalName.AnimationAdded, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationChanged(this AnimationLibrary target, Action<StringName> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationLibrary.SignalName.AnimationChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationRemoved(this AnimationLibrary target, Action<StringName> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationLibrary.SignalName.AnimationRemoved, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationRenamed(this AnimationLibrary target, Action<StringName, StringName> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationLibrary.SignalName.AnimationRenamed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationNodeRemoved(this AnimationNode target, Action<long, string> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationNode.SignalName.AnimationNodeRemoved, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationNodeRenamed(this AnimationNode target, Action<long, string, string> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationNode.SignalName.AnimationNodeRenamed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTreeChanged(this AnimationNode target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationNode.SignalName.TreeChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTrianglesUpdated(this AnimationNodeBlendSpace2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationNodeBlendSpace2D.SignalName.TrianglesUpdated, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNodeChanged(this AnimationNodeBlendTree target, Action<StringName> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationNodeBlendTree.SignalName.NodeChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAdvanceConditionChanged(this AnimationNodeStateMachineTransition target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationNodeStateMachineTransition.SignalName.AdvanceConditionChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationChanged(this AnimationPlayer target, Action<StringName, StringName> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationPlayer.SignalName.AnimationChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationFinished(this AnimationPlayer target, Action<StringName> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationPlayer.SignalName.AnimationFinished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationLibrariesUpdated(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationPlayer.SignalName.AnimationLibrariesUpdated, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationListChanged(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationPlayer.SignalName.AnimationListChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationStarted(this AnimationPlayer target, Action<StringName> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationPlayer.SignalName.AnimationStarted, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCachesCleared(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationPlayer.SignalName.CachesCleared, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationFinished(this AnimationTree target, Action<StringName> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationTree.SignalName.AnimationFinished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationPlayerChanged(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationTree.SignalName.AnimationPlayerChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAnimationStarted(this AnimationTree target, Action<StringName> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AnimationTree.SignalName.AnimationStarted, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAreaEntered(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area2D.SignalName.AreaEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAreaExited(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area2D.SignalName.AreaExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAreaShapeEntered(this Area2D target, Action<Rid, Area2D, long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area2D.SignalName.AreaShapeEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAreaShapeExited(this Area2D target, Action<Rid, Area2D, long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area2D.SignalName.AreaShapeExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyEntered(this Area2D target, Action<Node2D> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area2D.SignalName.BodyEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyExited(this Area2D target, Action<Node2D> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area2D.SignalName.BodyExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyShapeEntered(this Area2D target, Action<Rid, Node2D, long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area2D.SignalName.BodyShapeEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyShapeExited(this Area2D target, Action<Rid, Node2D, long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area2D.SignalName.BodyShapeExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAreaEntered(this Area3D target, Action<Area3D> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area3D.SignalName.AreaEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAreaExited(this Area3D target, Action<Area3D> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area3D.SignalName.AreaExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAreaShapeEntered(this Area3D target, Action<Rid, Area3D, long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area3D.SignalName.AreaShapeEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAreaShapeExited(this Area3D target, Action<Rid, Area3D, long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area3D.SignalName.AreaShapeExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyEntered(this Area3D target, Action<Node3D> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area3D.SignalName.BodyEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyExited(this Area3D target, Action<Node3D> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area3D.SignalName.BodyExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyShapeEntered(this Area3D target, Action<Rid, Node3D, long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area3D.SignalName.BodyShapeEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyShapeExited(this Area3D target, Action<Rid, Node3D, long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Area3D.SignalName.BodyShapeExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAudioServerBusLayoutChanged(Action action, bool oneShot = false, bool deferred = false) =>
        AudioServer.Singleton.Connect(AudioServer.SignalName.BusLayoutChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFinished(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AudioStreamPlayer.SignalName.Finished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFinished(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AudioStreamPlayer2D.SignalName.Finished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFinished(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(AudioStreamPlayer3D.SignalName.Finished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnButtonDown(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(BaseButton.SignalName.ButtonDown, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnButtonUp(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(BaseButton.SignalName.ButtonUp, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPressed(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(BaseButton.SignalName.Pressed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnToggled(this BaseButton target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(BaseButton.SignalName.Toggled, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBoneMapUpdated(this BoneMap target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(BoneMap.SignalName.BoneMapUpdated, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnProfileUpdated(this BoneMap target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(BoneMap.SignalName.ProfileUpdated, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPressed(this ButtonGroup target, Action<BaseButton> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ButtonGroup.SignalName.Pressed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCameraServerCameraFeedAdded(Action<long> action, bool oneShot = false, bool deferred = false) =>
        CameraServer.Singleton.Connect(CameraServer.SignalName.CameraFeedAdded, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCameraServerCameraFeedRemoved(Action<long> action, bool oneShot = false, bool deferred = false) =>
        CameraServer.Singleton.Connect(CameraServer.SignalName.CameraFeedRemoved, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnDraw(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CanvasItem.SignalName.Draw, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnHidden(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CanvasItem.SignalName.Hidden, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnItemRectChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CanvasItem.SignalName.ItemRectChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnVisibilityChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CanvasItem.SignalName.VisibilityChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnVisibilityChanged(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CanvasLayer.SignalName.VisibilityChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBreakpointToggled(this CodeEdit target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CodeEdit.SignalName.BreakpointToggled, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCodeCompletionRequested(this CodeEdit target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CodeEdit.SignalName.CodeCompletionRequested, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSymbolLookup(this CodeEdit target, Action<string, long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CodeEdit.SignalName.SymbolLookup, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSymbolValidate(this CodeEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CodeEdit.SignalName.SymbolValidate, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnInputEvent(this CollisionObject2D target, Action<Node, InputEvent, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CollisionObject2D.SignalName.InputEvent, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMouseEntered(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CollisionObject2D.SignalName.MouseEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMouseExited(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CollisionObject2D.SignalName.MouseExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMouseShapeEntered(this CollisionObject2D target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CollisionObject2D.SignalName.MouseShapeEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMouseShapeExited(this CollisionObject2D target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CollisionObject2D.SignalName.MouseShapeExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnInputEvent(this CollisionObject3D target, Action<Node, InputEvent, Vector3, Vector3, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CollisionObject3D.SignalName.InputEvent, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMouseEntered(this CollisionObject3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CollisionObject3D.SignalName.MouseEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMouseExited(this CollisionObject3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(CollisionObject3D.SignalName.MouseExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnColorChanged(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ColorPicker.SignalName.ColorChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPresetAdded(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ColorPicker.SignalName.PresetAdded, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPresetRemoved(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ColorPicker.SignalName.PresetRemoved, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnColorChanged(this ColorPickerButton target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ColorPickerButton.SignalName.ColorChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPickerCreated(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ColorPickerButton.SignalName.PickerCreated, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPopupClosed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ColorPickerButton.SignalName.PopupClosed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPreSortChildren(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Container.SignalName.PreSortChildren, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSortChildren(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Container.SignalName.SortChildren, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFocusEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Control.SignalName.FocusEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFocusExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Control.SignalName.FocusExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnGuiInput(this Control target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Control.SignalName.GuiInput, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMinimumSizeChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Control.SignalName.MinimumSizeChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMouseEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Control.SignalName.MouseEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMouseExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Control.SignalName.MouseExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnResized(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Control.SignalName.Resized, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSizeFlagsChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Control.SignalName.SizeFlagsChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnThemeChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Control.SignalName.ThemeChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnRangeChanged(this Curve target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Curve.SignalName.RangeChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnDirSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(FileDialog.SignalName.DirSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFileSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(FileDialog.SignalName.FileSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFilesSelected(this FileDialog target, Action<string[]> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(FileDialog.SignalName.FilesSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBeginNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.BeginNodeMove, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnConnectionDragEnded(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.ConnectionDragEnded, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnConnectionDragStarted(this GraphEdit target, Action<StringName, long, bool> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.ConnectionDragStarted, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnConnectionFromEmpty(this GraphEdit target, Action<StringName, long, Vector2> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.ConnectionFromEmpty, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnConnectionRequest(this GraphEdit target, Action<StringName, long, StringName, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.ConnectionRequest, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnConnectionToEmpty(this GraphEdit target, Action<StringName, long, Vector2> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.ConnectionToEmpty, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCopyNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.CopyNodesRequest, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnDeleteNodesRequest(this GraphEdit target, Action<Array> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.DeleteNodesRequest, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnDisconnectionRequest(this GraphEdit target, Action<StringName, long, StringName, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.DisconnectionRequest, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnDuplicateNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.DuplicateNodesRequest, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnEndNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.EndNodeMove, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNodeDeselected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.NodeDeselected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNodeSelected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.NodeSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPasteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.PasteNodesRequest, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPopupRequest(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.PopupRequest, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnScrollOffsetChanged(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphEdit.SignalName.ScrollOffsetChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCloseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphNode.SignalName.CloseRequest, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnDragged(this GraphNode target, Action<Vector2, Vector2> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphNode.SignalName.Dragged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNodeDeselected(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphNode.SignalName.NodeDeselected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNodeSelected(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphNode.SignalName.NodeSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPositionOffsetChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphNode.SignalName.PositionOffsetChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnRaiseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphNode.SignalName.RaiseRequest, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnResizeRequest(this GraphNode target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphNode.SignalName.ResizeRequest, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSlotUpdated(this GraphNode target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GraphNode.SignalName.SlotUpdated, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCellSizeChanged(this GridMap target, Action<Vector3> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(GridMap.SignalName.CellSizeChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnInputJoyConnectionChanged(Action<long, bool> action, bool oneShot = false, bool deferred = false) =>
        Input.Singleton.Connect(Input.SignalName.JoyConnectionChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnEmptyClicked(this ItemList target, Action<Vector2, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ItemList.SignalName.EmptyClicked, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnItemActivated(this ItemList target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ItemList.SignalName.ItemActivated, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnItemClicked(this ItemList target, Action<long, Vector2, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ItemList.SignalName.ItemClicked, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnItemSelected(this ItemList target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ItemList.SignalName.ItemSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMultiSelected(this ItemList target, Action<long, bool> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ItemList.SignalName.MultiSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnJavaScriptBridgePwaUpdateAvailable(Action action, bool oneShot = false, bool deferred = false) =>
        JavaScriptBridge.Singleton.Connect(JavaScriptBridge.SignalName.PwaUpdateAvailable, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTextChanged(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(LineEdit.SignalName.TextChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTextChangeRejected(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(LineEdit.SignalName.TextChangeRejected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTextSubmitted(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(LineEdit.SignalName.TextSubmitted, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnOnRequestPermissionsResult(this MainLoop target, Action<string, bool> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(MainLoop.SignalName.OnRequestPermissionsResult, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAboutToPopup(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(MenuButton.SignalName.AboutToPopup, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTextureChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(MeshInstance2D.SignalName.TextureChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTextureChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(MultiMeshInstance2D.SignalName.TextureChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPeerConnected(this MultiplayerPeer target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(MultiplayerPeer.SignalName.PeerConnected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPeerDisconnected(this MultiplayerPeer target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(MultiplayerPeer.SignalName.PeerDisconnected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnDespawned(this MultiplayerSpawner target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(MultiplayerSpawner.SignalName.Despawned, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSpawned(this MultiplayerSpawner target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(MultiplayerSpawner.SignalName.Spawned, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSynchronized(this MultiplayerSynchronizer target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(MultiplayerSynchronizer.SignalName.Synchronized, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnVisibilityChanged(this MultiplayerSynchronizer target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(MultiplayerSynchronizer.SignalName.VisibilityChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnLinkReached(this NavigationAgent2D target, Action<Dictionary> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NavigationAgent2D.SignalName.LinkReached, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNavigationFinished(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NavigationAgent2D.SignalName.NavigationFinished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPathChanged(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NavigationAgent2D.SignalName.PathChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTargetReached(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NavigationAgent2D.SignalName.TargetReached, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnVelocityComputed(this NavigationAgent2D target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NavigationAgent2D.SignalName.VelocityComputed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnWaypointReached(this NavigationAgent2D target, Action<Dictionary> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NavigationAgent2D.SignalName.WaypointReached, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnLinkReached(this NavigationAgent3D target, Action<Dictionary> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NavigationAgent3D.SignalName.LinkReached, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNavigationFinished(this NavigationAgent3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NavigationAgent3D.SignalName.NavigationFinished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPathChanged(this NavigationAgent3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NavigationAgent3D.SignalName.PathChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTargetReached(this NavigationAgent3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NavigationAgent3D.SignalName.TargetReached, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnVelocityComputed(this NavigationAgent3D target, Action<Vector3> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NavigationAgent3D.SignalName.VelocityComputed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnWaypointReached(this NavigationAgent3D target, Action<Dictionary> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NavigationAgent3D.SignalName.WaypointReached, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBakeFinished(this NavigationRegion3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NavigationRegion3D.SignalName.BakeFinished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNavigationMeshChanged(this NavigationRegion3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NavigationRegion3D.SignalName.NavigationMeshChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNavigationServer2DMapChanged(Action<Rid> action, bool oneShot = false, bool deferred = false) =>
        NavigationServer2D.Singleton.Connect(NavigationServer2D.SignalName.MapChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNavigationServer2DNavigationDebugChanged(Action action, bool oneShot = false, bool deferred = false) =>
        NavigationServer2D.Singleton.Connect(NavigationServer2D.SignalName.NavigationDebugChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNavigationServer3DMapChanged(Action<Rid> action, bool oneShot = false, bool deferred = false) =>
        NavigationServer3D.Singleton.Connect(NavigationServer3D.SignalName.MapChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNavigationServer3DNavigationDebugChanged(Action action, bool oneShot = false, bool deferred = false) =>
        NavigationServer3D.Singleton.Connect(NavigationServer3D.SignalName.NavigationDebugChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTextureChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(NinePatchRect.SignalName.TextureChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnChildEnteredTree(this Node target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Node.SignalName.ChildEnteredTree, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnChildExitingTree(this Node target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Node.SignalName.ChildExitingTree, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnChildOrderChanged(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Node.SignalName.ChildOrderChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnReady(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Node.SignalName.Ready, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnRenamed(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Node.SignalName.Renamed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTreeEntered(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Node.SignalName.TreeEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTreeExited(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Node.SignalName.TreeExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTreeExiting(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Node.SignalName.TreeExiting, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnVisibilityChanged(this Node3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Node3D.SignalName.VisibilityChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnItemFocused(this OptionButton target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(OptionButton.SignalName.ItemFocused, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnItemSelected(this OptionButton target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(OptionButton.SignalName.ItemSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCurveChanged(this Path3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Path3D.SignalName.CurveChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPopupHide(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Popup.SignalName.PopupHide, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnIdFocused(this PopupMenu target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(PopupMenu.SignalName.IdFocused, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnIdPressed(this PopupMenu target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(PopupMenu.SignalName.IdPressed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnIndexPressed(this PopupMenu target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(PopupMenu.SignalName.IndexPressed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMenuChanged(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(PopupMenu.SignalName.MenuChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnChanged(this Range target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Range.SignalName.Changed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnValueChanged(this Range target, Action<double> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Range.SignalName.ValueChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnRenderingServerFramePostDraw(Action action, bool oneShot = false, bool deferred = false) =>
        RenderingServer.Singleton.Connect(RenderingServer.SignalName.FramePostDraw, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnRenderingServerFramePreDraw(Action action, bool oneShot = false, bool deferred = false) =>
        RenderingServer.Singleton.Connect(RenderingServer.SignalName.FramePreDraw, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnChanged(this Resource target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Resource.SignalName.Changed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSetupLocalToSceneRequested(this Resource target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Resource.SignalName.SetupLocalToSceneRequested, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFinished(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(RichTextLabel.SignalName.Finished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMetaClicked(this RichTextLabel target, Action<Variant> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(RichTextLabel.SignalName.MetaClicked, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMetaHoverEnded(this RichTextLabel target, Action<Variant> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(RichTextLabel.SignalName.MetaHoverEnded, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMetaHoverStarted(this RichTextLabel target, Action<Variant> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(RichTextLabel.SignalName.MetaHoverStarted, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyEntered(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(RigidBody2D.SignalName.BodyEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyExited(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(RigidBody2D.SignalName.BodyExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyShapeEntered(this RigidBody2D target, Action<Rid, Node, long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(RigidBody2D.SignalName.BodyShapeEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyShapeExited(this RigidBody2D target, Action<Rid, Node, long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(RigidBody2D.SignalName.BodyShapeExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSleepingStateChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(RigidBody2D.SignalName.SleepingStateChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyEntered(this RigidBody3D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(RigidBody3D.SignalName.BodyEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyExited(this RigidBody3D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(RigidBody3D.SignalName.BodyExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyShapeEntered(this RigidBody3D target, Action<Rid, Node, long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(RigidBody3D.SignalName.BodyShapeEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBodyShapeExited(this RigidBody3D target, Action<Rid, Node, long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(RigidBody3D.SignalName.BodyShapeExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSleepingStateChanged(this RigidBody3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(RigidBody3D.SignalName.SleepingStateChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPeerAuthenticating(this SceneMultiplayer target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(SceneMultiplayer.SignalName.PeerAuthenticating, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPeerAuthenticationFailed(this SceneMultiplayer target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(SceneMultiplayer.SignalName.PeerAuthenticationFailed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPeerPacket(this SceneMultiplayer target, Action<long, byte[]> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(SceneMultiplayer.SignalName.PeerPacket, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNodeAdded(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(SceneTree.SignalName.NodeAdded, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNodeConfigurationWarningChanged(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(SceneTree.SignalName.NodeConfigurationWarningChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNodeRemoved(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(SceneTree.SignalName.NodeRemoved, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNodeRenamed(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(SceneTree.SignalName.NodeRenamed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPhysicsFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(SceneTree.SignalName.PhysicsFrame, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnProcessFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(SceneTree.SignalName.ProcessFrame, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTreeChanged(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(SceneTree.SignalName.TreeChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTreeProcessModeChanged(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(SceneTree.SignalName.TreeProcessModeChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTimeout(this SceneTreeTimer target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(SceneTreeTimer.SignalName.Timeout, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnScrolling(this ScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ScrollBar.SignalName.Scrolling, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnScrollEnded(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ScrollContainer.SignalName.ScrollEnded, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnScrollStarted(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(ScrollContainer.SignalName.ScrollStarted, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBoneSetupChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Skeleton2D.SignalName.BoneSetupChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBoneEnabledChanged(this Skeleton3D target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Skeleton3D.SignalName.BoneEnabledChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnBonePoseChanged(this Skeleton3D target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Skeleton3D.SignalName.BonePoseChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPoseUpdated(this Skeleton3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Skeleton3D.SignalName.PoseUpdated, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnShowRestOnlyChanged(this Skeleton3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Skeleton3D.SignalName.ShowRestOnlyChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnProfileUpdated(this SkeletonProfile target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(SkeletonProfile.SignalName.ProfileUpdated, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnDragEnded(this Slider target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Slider.SignalName.DragEnded, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnDragStarted(this Slider target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Slider.SignalName.DragStarted, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnDragged(this SplitContainer target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(SplitContainer.SignalName.Dragged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFrameChanged(this Sprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Sprite2D.SignalName.FrameChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTextureChanged(this Sprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Sprite2D.SignalName.TextureChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFrameChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Sprite3D.SignalName.FrameChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTextureChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Sprite3D.SignalName.TextureChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnActiveTabRearranged(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TabBar.SignalName.ActiveTabRearranged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTabButtonPressed(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TabBar.SignalName.TabButtonPressed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTabChanged(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TabBar.SignalName.TabChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTabClicked(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TabBar.SignalName.TabClicked, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTabClosePressed(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TabBar.SignalName.TabClosePressed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTabHovered(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TabBar.SignalName.TabHovered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTabRmbClicked(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TabBar.SignalName.TabRmbClicked, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTabSelected(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TabBar.SignalName.TabSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPrePopupPressed(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TabContainer.SignalName.PrePopupPressed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTabButtonPressed(this TabContainer target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TabContainer.SignalName.TabButtonPressed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTabChanged(this TabContainer target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TabContainer.SignalName.TabChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTabSelected(this TabContainer target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TabContainer.SignalName.TabSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCaretChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TextEdit.SignalName.CaretChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnGutterAdded(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TextEdit.SignalName.GutterAdded, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnGutterClicked(this TextEdit target, Action<long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TextEdit.SignalName.GutterClicked, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnGutterRemoved(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TextEdit.SignalName.GutterRemoved, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnLinesEditedFrom(this TextEdit target, Action<long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TextEdit.SignalName.LinesEditedFrom, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTextChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TextEdit.SignalName.TextChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTextSet(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TextEdit.SignalName.TextSet, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTextServerManagerInterfaceAdded(Action<StringName> action, bool oneShot = false, bool deferred = false) =>
        TextServerManager.Singleton.Connect(TextServerManager.SignalName.InterfaceAdded, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTextServerManagerInterfaceRemoved(Action<StringName> action, bool oneShot = false, bool deferred = false) =>
        TextServerManager.Singleton.Connect(TextServerManager.SignalName.InterfaceRemoved, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnThemeDBFallbackChanged(Action action, bool oneShot = false, bool deferred = false) =>
        ThemeDB.Singleton.Connect(ThemeDB.SignalName.FallbackChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnChanged(this TileData target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TileData.SignalName.Changed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TileMap.SignalName.Changed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTimeout(this Timer target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Timer.SignalName.Timeout, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPressed(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TouchScreenButton.SignalName.Pressed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnReleased(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(TouchScreenButton.SignalName.Released, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnButtonClicked(this Tree target, Action<TreeItem, long, long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.ButtonClicked, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCellSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.CellSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCheckPropagatedToItem(this Tree target, Action<TreeItem, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.CheckPropagatedToItem, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnColumnTitleClicked(this Tree target, Action<long, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.ColumnTitleClicked, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCustomItemClicked(this Tree target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.CustomItemClicked, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCustomPopupEdited(this Tree target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.CustomPopupEdited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnEmptyClicked(this Tree target, Action<Vector2, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.EmptyClicked, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnItemActivated(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.ItemActivated, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnItemCollapsed(this Tree target, Action<TreeItem> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.ItemCollapsed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnItemEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.ItemEdited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnItemIconDoubleClicked(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.ItemIconDoubleClicked, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnItemMouseSelected(this Tree target, Action<Vector2, long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.ItemMouseSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnItemSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.ItemSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMultiSelected(this Tree target, Action<TreeItem, long, bool> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.MultiSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnNothingSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tree.SignalName.NothingSelected, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFinished(this Tween target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tween.SignalName.Finished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnLoopFinished(this Tween target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tween.SignalName.LoopFinished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnStepFinished(this Tween target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tween.SignalName.StepFinished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFinished(this Tweener target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Tweener.SignalName.Finished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnVersionChanged(this UndoRedo target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(UndoRedo.SignalName.VersionChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFinished(this VideoStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(VideoStreamPlayer.SignalName.Finished, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnGuiFocusChanged(this Viewport target, Action<Control> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Viewport.SignalName.GuiFocusChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSizeChanged(this Viewport target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Viewport.SignalName.SizeChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnScreenEntered(this VisibleOnScreenNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(VisibleOnScreenNotifier2D.SignalName.ScreenEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnScreenExited(this VisibleOnScreenNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(VisibleOnScreenNotifier2D.SignalName.ScreenExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnScreenEntered(this VisibleOnScreenNotifier3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(VisibleOnScreenNotifier3D.SignalName.ScreenEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnScreenExited(this VisibleOnScreenNotifier3D target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(VisibleOnScreenNotifier3D.SignalName.ScreenExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnInputTypeChanged(this VisualShaderNodeInput target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(VisualShaderNodeInput.SignalName.InputTypeChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnDisplayRefreshRateChanged(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(WebXRInterface.SignalName.DisplayRefreshRateChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnReferenceSpaceReset(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(WebXRInterface.SignalName.ReferenceSpaceReset, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSelect(this WebXRInterface target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(WebXRInterface.SignalName.Select, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSelectend(this WebXRInterface target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(WebXRInterface.SignalName.Selectend, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSelectstart(this WebXRInterface target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(WebXRInterface.SignalName.Selectstart, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSessionEnded(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(WebXRInterface.SignalName.SessionEnded, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSessionFailed(this WebXRInterface target, Action<string> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(WebXRInterface.SignalName.SessionFailed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSessionStarted(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(WebXRInterface.SignalName.SessionStarted, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSessionSupported(this WebXRInterface target, Action<string, bool> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(WebXRInterface.SignalName.SessionSupported, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSqueeze(this WebXRInterface target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(WebXRInterface.SignalName.Squeeze, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSqueezeend(this WebXRInterface target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(WebXRInterface.SignalName.Squeezeend, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnSqueezestart(this WebXRInterface target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(WebXRInterface.SignalName.Squeezestart, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnVisibilityStateChanged(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(WebXRInterface.SignalName.VisibilityStateChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnAboutToPopup(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Window.SignalName.AboutToPopup, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnCloseRequested(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Window.SignalName.CloseRequested, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnDpiChanged(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Window.SignalName.DpiChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFilesDropped(this Window target, Action<string[]> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Window.SignalName.FilesDropped, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFocusEntered(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Window.SignalName.FocusEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnFocusExited(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Window.SignalName.FocusExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnGoBackRequested(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Window.SignalName.GoBackRequested, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMouseEntered(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Window.SignalName.MouseEntered, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnMouseExited(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Window.SignalName.MouseExited, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnThemeChanged(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Window.SignalName.ThemeChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnTitlebarChanged(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Window.SignalName.TitlebarChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnVisibilityChanged(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Window.SignalName.VisibilityChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnWindowInput(this Window target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(Window.SignalName.WindowInput, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnButtonPressed(this XRController3D target, Action<string> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(XRController3D.SignalName.ButtonPressed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnButtonReleased(this XRController3D target, Action<string> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(XRController3D.SignalName.ButtonReleased, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnInputFloatChanged(this XRController3D target, Action<string, double> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(XRController3D.SignalName.InputFloatChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnInputVector2Changed(this XRController3D target, Action<string, Vector2> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(XRController3D.SignalName.InputVector2Changed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPlayAreaChanged(this XRInterface target, Action<long> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(XRInterface.SignalName.PlayAreaChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnButtonPressed(this XRPositionalTracker target, Action<string> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(XRPositionalTracker.SignalName.ButtonPressed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnButtonReleased(this XRPositionalTracker target, Action<string> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(XRPositionalTracker.SignalName.ButtonReleased, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnInputFloatChanged(this XRPositionalTracker target, Action<string, double> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(XRPositionalTracker.SignalName.InputFloatChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnInputVector2Changed(this XRPositionalTracker target, Action<string, Vector2> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(XRPositionalTracker.SignalName.InputVector2Changed, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnPoseChanged(this XRPositionalTracker target, Action<XRPose> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(XRPositionalTracker.SignalName.PoseChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnProfileChanged(this XRPositionalTracker target, Action<string> action, bool oneShot = false, bool deferred = false) =>
        target.Connect(XRPositionalTracker.SignalName.ProfileChanged, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnXRServerInterfaceAdded(Action<StringName> action, bool oneShot = false, bool deferred = false) =>
        XRServer.Singleton.Connect(XRServer.SignalName.InterfaceAdded, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnXRServerInterfaceRemoved(Action<StringName> action, bool oneShot = false, bool deferred = false) =>
        XRServer.Singleton.Connect(XRServer.SignalName.InterfaceRemoved, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnXRServerTrackerAdded(Action<StringName, long> action, bool oneShot = false, bool deferred = false) =>
        XRServer.Singleton.Connect(XRServer.SignalName.TrackerAdded, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnXRServerTrackerRemoved(Action<StringName, long> action, bool oneShot = false, bool deferred = false) =>
        XRServer.Singleton.Connect(XRServer.SignalName.TrackerRemoved, Callable.From(action), SignalFlags(oneShot, deferred));

    public static void OnXRServerTrackerUpdated(Action<StringName, long> action, bool oneShot = false, bool deferred = false) =>
        XRServer.Singleton.Connect(XRServer.SignalName.TrackerUpdated, Callable.From(action), SignalFlags(oneShot, deferred));
}