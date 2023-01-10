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
            On(target, AcceptDialog.SignalName.Cancelled, action, oneShot, deferred);

        public static SignalHandler OnConfirmed(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AcceptDialog.SignalName.Confirmed, action, oneShot, deferred);

        public static SignalHandler OnCustomAction(this AcceptDialog target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, AcceptDialog.SignalName.CustomAction, action, oneShot, deferred);

        public static SignalHandler OnAnimationFinished(this AnimatedSprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimatedSprite2D.SignalName.AnimationFinished, action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this AnimatedSprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimatedSprite2D.SignalName.FrameChanged, action, oneShot, deferred);

        public static SignalHandler OnAnimationFinished(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimatedSprite3D.SignalName.AnimationFinished, action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimatedSprite3D.SignalName.FrameChanged, action, oneShot, deferred);

        public static SignalHandler OnAnimationAdded(this AnimationLibrary target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationLibrary.SignalName.AnimationAdded, action, oneShot, deferred);

        public static SignalHandler OnAnimationChanged(this AnimationLibrary target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationLibrary.SignalName.AnimationChanged, action, oneShot, deferred);

        public static SignalHandler OnAnimationRemoved(this AnimationLibrary target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationLibrary.SignalName.AnimationRemoved, action, oneShot, deferred);

        public static SignalHandler OnAnimationRenamed(this AnimationLibrary target, Action<Godot.StringName, Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationLibrary.SignalName.AnimationRenamed, action, oneShot, deferred);

        public static SignalHandler OnTreeChanged(this AnimationNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationNode.SignalName.TreeChanged, action, oneShot, deferred);

        public static SignalHandler OnTrianglesUpdated(this AnimationNodeBlendSpace2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationNodeBlendSpace2D.SignalName.TrianglesUpdated, action, oneShot, deferred);

        public static SignalHandler OnNodeChanged(this AnimationNodeBlendTree target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationNodeBlendTree.SignalName.NodeChanged, action, oneShot, deferred);

        public static SignalHandler OnAdvanceConditionChanged(this AnimationNodeStateMachineTransition target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationNodeStateMachineTransition.SignalName.AdvanceConditionChanged, action, oneShot, deferred);

        public static SignalHandler OnAnimationChanged(this AnimationPlayer target, Action<Godot.StringName, Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationPlayer.SignalName.AnimationChanged, action, oneShot, deferred);

        public static SignalHandler OnAnimationFinished(this AnimationPlayer target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationPlayer.SignalName.AnimationFinished, action, oneShot, deferred);

        public static SignalHandler OnAnimationLibrariesUpdated(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationPlayer.SignalName.AnimationLibrariesUpdated, action, oneShot, deferred);

        public static SignalHandler OnAnimationListChanged(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationPlayer.SignalName.AnimationListChanged, action, oneShot, deferred);

        public static SignalHandler OnAnimationStarted(this AnimationPlayer target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationPlayer.SignalName.AnimationStarted, action, oneShot, deferred);

        public static SignalHandler OnCachesCleared(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationPlayer.SignalName.CachesCleared, action, oneShot, deferred);

        public static SignalHandler OnAnimationFinished(this AnimationTree target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationTree.SignalName.AnimationFinished, action, oneShot, deferred);

        public static SignalHandler OnAnimationPlayerChanged(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationTree.SignalName.AnimationPlayerChanged, action, oneShot, deferred);

        public static SignalHandler OnAnimationStarted(this AnimationTree target, Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(target, AnimationTree.SignalName.AnimationStarted, action, oneShot, deferred);

        public static SignalHandler OnAreaEntered(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area2D.SignalName.AreaEntered, action, oneShot, deferred);

        public static SignalHandler OnAreaExited(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area2D.SignalName.AreaExited, action, oneShot, deferred);

        public static SignalHandler OnAreaShapeEntered(this Area2D target, Action<Godot.RID, Area2D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area2D.SignalName.AreaShapeEntered, action, oneShot, deferred);

        public static SignalHandler OnAreaShapeExited(this Area2D target, Action<Godot.RID, Area2D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area2D.SignalName.AreaShapeExited, action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this Area2D target, Action<Node2D> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area2D.SignalName.BodyEntered, action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this Area2D target, Action<Node2D> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area2D.SignalName.BodyExited, action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this Area2D target, Action<Godot.RID, Node2D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area2D.SignalName.BodyShapeEntered, action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this Area2D target, Action<Godot.RID, Node2D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area2D.SignalName.BodyShapeExited, action, oneShot, deferred);

        public static SignalHandler OnAreaEntered(this Area3D target, Action<Area3D> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area3D.SignalName.AreaEntered, action, oneShot, deferred);

        public static SignalHandler OnAreaExited(this Area3D target, Action<Area3D> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area3D.SignalName.AreaExited, action, oneShot, deferred);

        public static SignalHandler OnAreaShapeEntered(this Area3D target, Action<Godot.RID, Area3D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area3D.SignalName.AreaShapeEntered, action, oneShot, deferred);

        public static SignalHandler OnAreaShapeExited(this Area3D target, Action<Godot.RID, Area3D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area3D.SignalName.AreaShapeExited, action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this Area3D target, Action<Node3D> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area3D.SignalName.BodyEntered, action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this Area3D target, Action<Node3D> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area3D.SignalName.BodyExited, action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this Area3D target, Action<Godot.RID, Node3D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area3D.SignalName.BodyShapeEntered, action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this Area3D target, Action<Godot.RID, Node3D, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Area3D.SignalName.BodyShapeExited, action, oneShot, deferred);

        public static SignalHandler OnAudioServerBusLayoutChanged(Action action, bool oneShot = false, bool deferred = false) =>
            On(AudioServer.Singleton, AudioServer.SignalName.BusLayoutChanged, action, oneShot, deferred);

        public static SignalHandler OnFinished(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AudioStreamPlayer.SignalName.Finished, action, oneShot, deferred);

        public static SignalHandler OnFinished(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AudioStreamPlayer2D.SignalName.Finished, action, oneShot, deferred);

        public static SignalHandler OnFinished(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, AudioStreamPlayer3D.SignalName.Finished, action, oneShot, deferred);

        public static SignalHandler OnButtonDown(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, BaseButton.SignalName.ButtonDown, action, oneShot, deferred);

        public static SignalHandler OnButtonUp(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, BaseButton.SignalName.ButtonUp, action, oneShot, deferred);

        public static SignalHandler OnPressed(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, BaseButton.SignalName.Pressed, action, oneShot, deferred);

        public static SignalHandler OnToggled(this BaseButton target, Action<System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, BaseButton.SignalName.Toggled, action, oneShot, deferred);

        public static SignalHandler OnBoneMapUpdated(this BoneMap target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, BoneMap.SignalName.BoneMapUpdated, action, oneShot, deferred);

        public static SignalHandler OnProfileUpdated(this BoneMap target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, BoneMap.SignalName.ProfileUpdated, action, oneShot, deferred);

        public static SignalHandler OnPressed(this ButtonGroup target, Action<BaseButton> action, bool oneShot = false, bool deferred = false) =>
            On(target, ButtonGroup.SignalName.Pressed, action, oneShot, deferred);

        public static SignalHandler OnCameraServerCameraFeedAdded(Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(CameraServer.Singleton, CameraServer.SignalName.CameraFeedAdded, action, oneShot, deferred);

        public static SignalHandler OnCameraServerCameraFeedRemoved(Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(CameraServer.Singleton, CameraServer.SignalName.CameraFeedRemoved, action, oneShot, deferred);

        public static SignalHandler OnDraw(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, CanvasItem.SignalName.Draw, action, oneShot, deferred);

        public static SignalHandler OnHidden(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, CanvasItem.SignalName.Hidden, action, oneShot, deferred);

        public static SignalHandler OnItemRectChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, CanvasItem.SignalName.ItemRectChanged, action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, CanvasItem.SignalName.VisibilityChanged, action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, CanvasLayer.SignalName.VisibilityChanged, action, oneShot, deferred);

        public static SignalHandler OnBreakpointToggled(this CodeEdit target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, CodeEdit.SignalName.BreakpointToggled, action, oneShot, deferred);

        public static SignalHandler OnCodeCompletionRequested(this CodeEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, CodeEdit.SignalName.CodeCompletionRequested, action, oneShot, deferred);

        public static SignalHandler OnSymbolLookup(this CodeEdit target, Action<System.String, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, CodeEdit.SignalName.SymbolLookup, action, oneShot, deferred);

        public static SignalHandler OnSymbolValidate(this CodeEdit target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, CodeEdit.SignalName.SymbolValidate, action, oneShot, deferred);

        public static SignalHandler OnInputEvent(this CollisionObject2D target, Action<Node, InputEvent, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, CollisionObject2D.SignalName.InputEvent, action, oneShot, deferred);

        public static SignalHandler OnMouseEntered(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, CollisionObject2D.SignalName.MouseEntered, action, oneShot, deferred);

        public static SignalHandler OnMouseExited(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, CollisionObject2D.SignalName.MouseExited, action, oneShot, deferred);

        public static SignalHandler OnMouseShapeEntered(this CollisionObject2D target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, CollisionObject2D.SignalName.MouseShapeEntered, action, oneShot, deferred);

        public static SignalHandler OnMouseShapeExited(this CollisionObject2D target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, CollisionObject2D.SignalName.MouseShapeExited, action, oneShot, deferred);

        public static SignalHandler OnInputEvent(this CollisionObject3D target, Action<Node, InputEvent, Godot.Vector3, Godot.Vector3, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, CollisionObject3D.SignalName.InputEvent, action, oneShot, deferred);

        public static SignalHandler OnMouseEntered(this CollisionObject3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, CollisionObject3D.SignalName.MouseEntered, action, oneShot, deferred);

        public static SignalHandler OnMouseExited(this CollisionObject3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, CollisionObject3D.SignalName.MouseExited, action, oneShot, deferred);

        public static SignalHandler OnColorChanged(this ColorPicker target, Action<Godot.Color> action, bool oneShot = false, bool deferred = false) =>
            On(target, ColorPicker.SignalName.ColorChanged, action, oneShot, deferred);

        public static SignalHandler OnPresetAdded(this ColorPicker target, Action<Godot.Color> action, bool oneShot = false, bool deferred = false) =>
            On(target, ColorPicker.SignalName.PresetAdded, action, oneShot, deferred);

        public static SignalHandler OnPresetRemoved(this ColorPicker target, Action<Godot.Color> action, bool oneShot = false, bool deferred = false) =>
            On(target, ColorPicker.SignalName.PresetRemoved, action, oneShot, deferred);

        public static SignalHandler OnColorChanged(this ColorPickerButton target, Action<Godot.Color> action, bool oneShot = false, bool deferred = false) =>
            On(target, ColorPickerButton.SignalName.ColorChanged, action, oneShot, deferred);

        public static SignalHandler OnPickerCreated(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, ColorPickerButton.SignalName.PickerCreated, action, oneShot, deferred);

        public static SignalHandler OnPopupClosed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, ColorPickerButton.SignalName.PopupClosed, action, oneShot, deferred);

        public static SignalHandler OnPreSortChildren(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Container.SignalName.PreSortChildren, action, oneShot, deferred);

        public static SignalHandler OnSortChildren(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Container.SignalName.SortChildren, action, oneShot, deferred);

        public static SignalHandler OnFocusEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Control.SignalName.FocusEntered, action, oneShot, deferred);

        public static SignalHandler OnFocusExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Control.SignalName.FocusExited, action, oneShot, deferred);

        public static SignalHandler OnGuiInput(this Control target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            On(target, Control.SignalName.GuiInput, action, oneShot, deferred);

        public static SignalHandler OnMinimumSizeChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Control.SignalName.MinimumSizeChanged, action, oneShot, deferred);

        public static SignalHandler OnMouseEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Control.SignalName.MouseEntered, action, oneShot, deferred);

        public static SignalHandler OnMouseExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Control.SignalName.MouseExited, action, oneShot, deferred);

        public static SignalHandler OnResized(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Control.SignalName.Resized, action, oneShot, deferred);

        public static SignalHandler OnSizeFlagsChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Control.SignalName.SizeFlagsChanged, action, oneShot, deferred);

        public static SignalHandler OnThemeChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Control.SignalName.ThemeChanged, action, oneShot, deferred);

        public static SignalHandler OnRangeChanged(this Curve target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Curve.SignalName.RangeChanged, action, oneShot, deferred);

        public static SignalHandler OnDirSelected(this FileDialog target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, FileDialog.SignalName.DirSelected, action, oneShot, deferred);

        public static SignalHandler OnFileSelected(this FileDialog target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, FileDialog.SignalName.FileSelected, action, oneShot, deferred);

        public static SignalHandler OnFilesSelected(this FileDialog target, Action<System.String[]> action, bool oneShot = false, bool deferred = false) =>
            On(target, FileDialog.SignalName.FilesSelected, action, oneShot, deferred);

        public static SignalHandler OnBeginNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.BeginNodeMove, action, oneShot, deferred);

        public static SignalHandler OnConnectionDragEnded(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.ConnectionDragEnded, action, oneShot, deferred);

        public static SignalHandler OnConnectionDragStarted(this GraphEdit target, Action<Godot.StringName, System.Int32, System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.ConnectionDragStarted, action, oneShot, deferred);

        public static SignalHandler OnConnectionFromEmpty(this GraphEdit target, Action<Godot.StringName, System.Int32, Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.ConnectionFromEmpty, action, oneShot, deferred);

        public static SignalHandler OnConnectionRequest(this GraphEdit target, Action<Godot.StringName, System.Int32, Godot.StringName, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.ConnectionRequest, action, oneShot, deferred);

        public static SignalHandler OnConnectionToEmpty(this GraphEdit target, Action<Godot.StringName, System.Int32, Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.ConnectionToEmpty, action, oneShot, deferred);

        public static SignalHandler OnCopyNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.CopyNodesRequest, action, oneShot, deferred);

        public static SignalHandler OnDeleteNodesRequest(this GraphEdit target, Action<Godot.Collections.Array> action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.DeleteNodesRequest, action, oneShot, deferred);

        public static SignalHandler OnDisconnectionRequest(this GraphEdit target, Action<Godot.StringName, System.Int32, Godot.StringName, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.DisconnectionRequest, action, oneShot, deferred);

        public static SignalHandler OnDuplicateNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.DuplicateNodesRequest, action, oneShot, deferred);

        public static SignalHandler OnEndNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.EndNodeMove, action, oneShot, deferred);

        public static SignalHandler OnNodeDeselected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.NodeDeselected, action, oneShot, deferred);

        public static SignalHandler OnNodeSelected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.NodeSelected, action, oneShot, deferred);

        public static SignalHandler OnPasteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.PasteNodesRequest, action, oneShot, deferred);

        public static SignalHandler OnPopupRequest(this GraphEdit target, Action<Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.PopupRequest, action, oneShot, deferred);

        public static SignalHandler OnScrollOffsetChanged(this GraphEdit target, Action<Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphEdit.SignalName.ScrollOffsetChanged, action, oneShot, deferred);

        public static SignalHandler OnCloseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphNode.SignalName.CloseRequest, action, oneShot, deferred);

        public static SignalHandler OnDeselected(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphNode.SignalName.Deselected, action, oneShot, deferred);

        public static SignalHandler OnDragged(this GraphNode target, Action<Godot.Vector2, Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphNode.SignalName.Dragged, action, oneShot, deferred);

        public static SignalHandler OnPositionOffsetChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphNode.SignalName.PositionOffsetChanged, action, oneShot, deferred);

        public static SignalHandler OnRaiseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphNode.SignalName.RaiseRequest, action, oneShot, deferred);

        public static SignalHandler OnResizeRequest(this GraphNode target, Action<Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphNode.SignalName.ResizeRequest, action, oneShot, deferred);

        public static SignalHandler OnSelectedSignal(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphNode.SignalName.SelectedSignal, action, oneShot, deferred);

        public static SignalHandler OnSlotUpdated(this GraphNode target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, GraphNode.SignalName.SlotUpdated, action, oneShot, deferred);

        public static SignalHandler OnCellSizeChanged(this GridMap target, Action<Godot.Vector3> action, bool oneShot = false, bool deferred = false) =>
            On(target, GridMap.SignalName.CellSizeChanged, action, oneShot, deferred);

        public static SignalHandler OnRequestCompleted(this HTTPRequest target, Action<System.Int32, System.Int32, System.String[], System.Byte[]> action, bool oneShot = false, bool deferred = false) =>
            On(target, HTTPRequest.SignalName.RequestCompleted, action, oneShot, deferred);

        public static SignalHandler OnInputJoyConnectionChanged(Action<System.Int32, System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(Input.Singleton, Input.SignalName.JoyConnectionChanged, action, oneShot, deferred);

        public static SignalHandler OnEmptyClicked(this ItemList target, Action<Godot.Vector2, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, ItemList.SignalName.EmptyClicked, action, oneShot, deferred);

        public static SignalHandler OnItemActivated(this ItemList target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, ItemList.SignalName.ItemActivated, action, oneShot, deferred);

        public static SignalHandler OnItemClicked(this ItemList target, Action<System.Int32, Godot.Vector2, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, ItemList.SignalName.ItemClicked, action, oneShot, deferred);

        public static SignalHandler OnItemSelected(this ItemList target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, ItemList.SignalName.ItemSelected, action, oneShot, deferred);

        public static SignalHandler OnMultiSelected(this ItemList target, Action<System.Int32, System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, ItemList.SignalName.MultiSelected, action, oneShot, deferred);

        public static SignalHandler OnJavaScriptBridgePwaUpdateAvailable(Action action, bool oneShot = false, bool deferred = false) =>
            On(JavaScriptBridge.Singleton, JavaScriptBridge.SignalName.PwaUpdateAvailable, action, oneShot, deferred);

        public static SignalHandler OnTextChanged(this LineEdit target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, LineEdit.SignalName.TextChanged, action, oneShot, deferred);

        public static SignalHandler OnTextChangeRejected(this LineEdit target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, LineEdit.SignalName.TextChangeRejected, action, oneShot, deferred);

        public static SignalHandler OnTextSubmitted(this LineEdit target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, LineEdit.SignalName.TextSubmitted, action, oneShot, deferred);

        public static SignalHandler OnOnRequestPermissionsResult(this MainLoop target, Action<System.String, System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, MainLoop.SignalName.OnRequestPermissionsResult, action, oneShot, deferred);

        public static SignalHandler OnAboutToPopup(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, MenuButton.SignalName.AboutToPopup, action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, MeshInstance2D.SignalName.TextureChanged, action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, MultiMeshInstance2D.SignalName.TextureChanged, action, oneShot, deferred);

        public static SignalHandler OnConnectedToServer(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, MultiplayerAPI.SignalName.ConnectedToServer, action, oneShot, deferred);

        public static SignalHandler OnConnectionFailed(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, MultiplayerAPI.SignalName.ConnectionFailed, action, oneShot, deferred);

        public static SignalHandler OnPeerConnected(this MultiplayerAPI target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, MultiplayerAPI.SignalName.PeerConnected, action, oneShot, deferred);

        public static SignalHandler OnPeerDisconnected(this MultiplayerAPI target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, MultiplayerAPI.SignalName.PeerDisconnected, action, oneShot, deferred);

        public static SignalHandler OnServerDisconnected(this MultiplayerAPI target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, MultiplayerAPI.SignalName.ServerDisconnected, action, oneShot, deferred);

        public static SignalHandler OnPeerConnected(this MultiplayerPeer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, MultiplayerPeer.SignalName.PeerConnected, action, oneShot, deferred);

        public static SignalHandler OnPeerDisconnected(this MultiplayerPeer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, MultiplayerPeer.SignalName.PeerDisconnected, action, oneShot, deferred);

        public static SignalHandler OnDespawned(this MultiplayerSpawner target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, MultiplayerSpawner.SignalName.Despawned, action, oneShot, deferred);

        public static SignalHandler OnSpawned(this MultiplayerSpawner target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, MultiplayerSpawner.SignalName.Spawned, action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this MultiplayerSynchronizer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, MultiplayerSynchronizer.SignalName.VisibilityChanged, action, oneShot, deferred);

        public static SignalHandler OnLinkReached(this NavigationAgent2D target, Action<Godot.Collections.Dictionary> action, bool oneShot = false, bool deferred = false) =>
            On(target, NavigationAgent2D.SignalName.LinkReached, action, oneShot, deferred);

        public static SignalHandler OnNavigationFinished(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, NavigationAgent2D.SignalName.NavigationFinished, action, oneShot, deferred);

        public static SignalHandler OnPathChanged(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, NavigationAgent2D.SignalName.PathChanged, action, oneShot, deferred);

        public static SignalHandler OnTargetReached(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, NavigationAgent2D.SignalName.TargetReached, action, oneShot, deferred);

        public static SignalHandler OnVelocityComputed(this NavigationAgent2D target, Action<Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, NavigationAgent2D.SignalName.VelocityComputed, action, oneShot, deferred);

        public static SignalHandler OnWaypointReached(this NavigationAgent2D target, Action<Godot.Collections.Dictionary> action, bool oneShot = false, bool deferred = false) =>
            On(target, NavigationAgent2D.SignalName.WaypointReached, action, oneShot, deferred);

        public static SignalHandler OnLinkReached(this NavigationAgent3D target, Action<Godot.Collections.Dictionary> action, bool oneShot = false, bool deferred = false) =>
            On(target, NavigationAgent3D.SignalName.LinkReached, action, oneShot, deferred);

        public static SignalHandler OnNavigationFinished(this NavigationAgent3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, NavigationAgent3D.SignalName.NavigationFinished, action, oneShot, deferred);

        public static SignalHandler OnPathChanged(this NavigationAgent3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, NavigationAgent3D.SignalName.PathChanged, action, oneShot, deferred);

        public static SignalHandler OnTargetReached(this NavigationAgent3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, NavigationAgent3D.SignalName.TargetReached, action, oneShot, deferred);

        public static SignalHandler OnVelocityComputed(this NavigationAgent3D target, Action<Godot.Vector3> action, bool oneShot = false, bool deferred = false) =>
            On(target, NavigationAgent3D.SignalName.VelocityComputed, action, oneShot, deferred);

        public static SignalHandler OnWaypointReached(this NavigationAgent3D target, Action<Godot.Collections.Dictionary> action, bool oneShot = false, bool deferred = false) =>
            On(target, NavigationAgent3D.SignalName.WaypointReached, action, oneShot, deferred);

        public static SignalHandler OnBakeFinished(this NavigationRegion3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, NavigationRegion3D.SignalName.BakeFinished, action, oneShot, deferred);

        public static SignalHandler OnNavigationMeshChanged(this NavigationRegion3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, NavigationRegion3D.SignalName.NavigationMeshChanged, action, oneShot, deferred);

        public static SignalHandler OnNavigationServer2DMapChanged(Action<Godot.RID> action, bool oneShot = false, bool deferred = false) =>
            On(NavigationServer2D.Singleton, NavigationServer2D.SignalName.MapChanged, action, oneShot, deferred);

        public static SignalHandler OnNavigationServer3DMapChanged(Action<Godot.RID> action, bool oneShot = false, bool deferred = false) =>
            On(NavigationServer3D.Singleton, NavigationServer3D.SignalName.MapChanged, action, oneShot, deferred);

        public static SignalHandler OnNavigationServer3DNavigationDebugChanged(Action action, bool oneShot = false, bool deferred = false) =>
            On(NavigationServer3D.Singleton, NavigationServer3D.SignalName.NavigationDebugChanged, action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, NinePatchRect.SignalName.TextureChanged, action, oneShot, deferred);

        public static SignalHandler OnChildEnteredTree(this Node target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, Node.SignalName.ChildEnteredTree, action, oneShot, deferred);

        public static SignalHandler OnChildExitingTree(this Node target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, Node.SignalName.ChildExitingTree, action, oneShot, deferred);

        public static SignalHandler OnReady(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Node.SignalName.Ready, action, oneShot, deferred);

        public static SignalHandler OnRenamed(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Node.SignalName.Renamed, action, oneShot, deferred);

        public static SignalHandler OnTreeEntered(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Node.SignalName.TreeEntered, action, oneShot, deferred);

        public static SignalHandler OnTreeExited(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Node.SignalName.TreeExited, action, oneShot, deferred);

        public static SignalHandler OnTreeExiting(this Node target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Node.SignalName.TreeExiting, action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this Node3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Node3D.SignalName.VisibilityChanged, action, oneShot, deferred);

        public static SignalHandler OnPropertyListChanged(this Object target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Object.SignalName.PropertyListChanged, action, oneShot, deferred);

        public static SignalHandler OnScriptChanged(this Object target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Object.SignalName.ScriptChanged, action, oneShot, deferred);

        public static SignalHandler OnItemFocused(this OptionButton target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, OptionButton.SignalName.ItemFocused, action, oneShot, deferred);

        public static SignalHandler OnItemSelected(this OptionButton target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, OptionButton.SignalName.ItemSelected, action, oneShot, deferred);

        public static SignalHandler OnCurveChanged(this Path3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Path3D.SignalName.CurveChanged, action, oneShot, deferred);

        public static SignalHandler OnPopupHide(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Popup.SignalName.PopupHide, action, oneShot, deferred);

        public static SignalHandler OnIdFocused(this PopupMenu target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, PopupMenu.SignalName.IdFocused, action, oneShot, deferred);

        public static SignalHandler OnIdPressed(this PopupMenu target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, PopupMenu.SignalName.IdPressed, action, oneShot, deferred);

        public static SignalHandler OnIndexPressed(this PopupMenu target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, PopupMenu.SignalName.IndexPressed, action, oneShot, deferred);

        public static SignalHandler OnMenuChanged(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, PopupMenu.SignalName.MenuChanged, action, oneShot, deferred);

        public static SignalHandler OnChanged(this Range target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Range.SignalName.Changed, action, oneShot, deferred);

        public static SignalHandler OnValueChanged(this Range target, Action<System.Single> action, bool oneShot = false, bool deferred = false) =>
            On(target, Range.SignalName.ValueChanged, action, oneShot, deferred);

        public static SignalHandler OnRenderingServerFramePostDraw(Action action, bool oneShot = false, bool deferred = false) =>
            On(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw, action, oneShot, deferred);

        public static SignalHandler OnRenderingServerFramePreDraw(Action action, bool oneShot = false, bool deferred = false) =>
            On(RenderingServer.Singleton, RenderingServer.SignalName.FramePreDraw, action, oneShot, deferred);

        public static SignalHandler OnChanged(this Resource target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Resource.SignalName.Changed, action, oneShot, deferred);

        public static SignalHandler OnSetupLocalToSceneRequested(this Resource target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Resource.SignalName.SetupLocalToSceneRequested, action, oneShot, deferred);

        public static SignalHandler OnFinished(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, RichTextLabel.SignalName.Finished, action, oneShot, deferred);

        public static SignalHandler OnMetaClicked(this RichTextLabel target, Action<Godot.Variant> action, bool oneShot = false, bool deferred = false) =>
            On(target, RichTextLabel.SignalName.MetaClicked, action, oneShot, deferred);

        public static SignalHandler OnMetaHoverEnded(this RichTextLabel target, Action<Godot.Variant> action, bool oneShot = false, bool deferred = false) =>
            On(target, RichTextLabel.SignalName.MetaHoverEnded, action, oneShot, deferred);

        public static SignalHandler OnMetaHoverStarted(this RichTextLabel target, Action<Godot.Variant> action, bool oneShot = false, bool deferred = false) =>
            On(target, RichTextLabel.SignalName.MetaHoverStarted, action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, RigidBody2D.SignalName.BodyEntered, action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, RigidBody2D.SignalName.BodyExited, action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this RigidBody2D target, Action<Godot.RID, Node, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, RigidBody2D.SignalName.BodyShapeEntered, action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this RigidBody2D target, Action<Godot.RID, Node, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, RigidBody2D.SignalName.BodyShapeExited, action, oneShot, deferred);

        public static SignalHandler OnSleepingStateChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, RigidBody2D.SignalName.SleepingStateChanged, action, oneShot, deferred);

        public static SignalHandler OnBodyEntered(this RigidBody3D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, RigidBody3D.SignalName.BodyEntered, action, oneShot, deferred);

        public static SignalHandler OnBodyExited(this RigidBody3D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, RigidBody3D.SignalName.BodyExited, action, oneShot, deferred);

        public static SignalHandler OnBodyShapeEntered(this RigidBody3D target, Action<Godot.RID, Node, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, RigidBody3D.SignalName.BodyShapeEntered, action, oneShot, deferred);

        public static SignalHandler OnBodyShapeExited(this RigidBody3D target, Action<Godot.RID, Node, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, RigidBody3D.SignalName.BodyShapeExited, action, oneShot, deferred);

        public static SignalHandler OnSleepingStateChanged(this RigidBody3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, RigidBody3D.SignalName.SleepingStateChanged, action, oneShot, deferred);

        public static SignalHandler OnPeerAuthenticating(this SceneMultiplayer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, SceneMultiplayer.SignalName.PeerAuthenticating, action, oneShot, deferred);

        public static SignalHandler OnPeerAuthenticationFailed(this SceneMultiplayer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, SceneMultiplayer.SignalName.PeerAuthenticationFailed, action, oneShot, deferred);

        public static SignalHandler OnPeerPacket(this SceneMultiplayer target, Action<System.Int32, System.Byte[]> action, bool oneShot = false, bool deferred = false) =>
            On(target, SceneMultiplayer.SignalName.PeerPacket, action, oneShot, deferred);

        public static SignalHandler OnNodeAdded(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, SceneTree.SignalName.NodeAdded, action, oneShot, deferred);

        public static SignalHandler OnNodeConfigurationWarningChanged(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, SceneTree.SignalName.NodeConfigurationWarningChanged, action, oneShot, deferred);

        public static SignalHandler OnNodeRemoved(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, SceneTree.SignalName.NodeRemoved, action, oneShot, deferred);

        public static SignalHandler OnNodeRenamed(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            On(target, SceneTree.SignalName.NodeRenamed, action, oneShot, deferred);

        public static SignalHandler OnPhysicsFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, SceneTree.SignalName.PhysicsFrame, action, oneShot, deferred);

        public static SignalHandler OnProcessFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, SceneTree.SignalName.ProcessFrame, action, oneShot, deferred);

        public static SignalHandler OnTreeChanged(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, SceneTree.SignalName.TreeChanged, action, oneShot, deferred);

        public static SignalHandler OnTreeProcessModeChanged(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, SceneTree.SignalName.TreeProcessModeChanged, action, oneShot, deferred);

        public static SignalHandler OnTimeout(this SceneTreeTimer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, SceneTreeTimer.SignalName.Timeout, action, oneShot, deferred);

        public static SignalHandler OnScrolling(this ScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, ScrollBar.SignalName.Scrolling, action, oneShot, deferred);

        public static SignalHandler OnScrollEnded(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, ScrollContainer.SignalName.ScrollEnded, action, oneShot, deferred);

        public static SignalHandler OnScrollStarted(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, ScrollContainer.SignalName.ScrollStarted, action, oneShot, deferred);

        public static SignalHandler OnBoneSetupChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Skeleton2D.SignalName.BoneSetupChanged, action, oneShot, deferred);

        public static SignalHandler OnBoneEnabledChanged(this Skeleton3D target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Skeleton3D.SignalName.BoneEnabledChanged, action, oneShot, deferred);

        public static SignalHandler OnBonePoseChanged(this Skeleton3D target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Skeleton3D.SignalName.BonePoseChanged, action, oneShot, deferred);

        public static SignalHandler OnPoseUpdated(this Skeleton3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Skeleton3D.SignalName.PoseUpdated, action, oneShot, deferred);

        public static SignalHandler OnShowRestOnlyChanged(this Skeleton3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Skeleton3D.SignalName.ShowRestOnlyChanged, action, oneShot, deferred);

        public static SignalHandler OnProfileUpdated(this SkeletonProfile target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, SkeletonProfile.SignalName.ProfileUpdated, action, oneShot, deferred);

        public static SignalHandler OnDragEnded(this Slider target, Action<System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, Slider.SignalName.DragEnded, action, oneShot, deferred);

        public static SignalHandler OnDragStarted(this Slider target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Slider.SignalName.DragStarted, action, oneShot, deferred);

        public static SignalHandler OnDragged(this SplitContainer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, SplitContainer.SignalName.Dragged, action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this Sprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Sprite2D.SignalName.FrameChanged, action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this Sprite2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Sprite2D.SignalName.TextureChanged, action, oneShot, deferred);

        public static SignalHandler OnFrameChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Sprite3D.SignalName.FrameChanged, action, oneShot, deferred);

        public static SignalHandler OnTextureChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Sprite3D.SignalName.TextureChanged, action, oneShot, deferred);

        public static SignalHandler OnActiveTabRearranged(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, TabBar.SignalName.ActiveTabRearranged, action, oneShot, deferred);

        public static SignalHandler OnTabButtonPressed(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, TabBar.SignalName.TabButtonPressed, action, oneShot, deferred);

        public static SignalHandler OnTabChanged(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, TabBar.SignalName.TabChanged, action, oneShot, deferred);

        public static SignalHandler OnTabClicked(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, TabBar.SignalName.TabClicked, action, oneShot, deferred);

        public static SignalHandler OnTabClosePressed(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, TabBar.SignalName.TabClosePressed, action, oneShot, deferred);

        public static SignalHandler OnTabHovered(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, TabBar.SignalName.TabHovered, action, oneShot, deferred);

        public static SignalHandler OnTabRmbClicked(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, TabBar.SignalName.TabRmbClicked, action, oneShot, deferred);

        public static SignalHandler OnTabSelected(this TabBar target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, TabBar.SignalName.TabSelected, action, oneShot, deferred);

        public static SignalHandler OnPrePopupPressed(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, TabContainer.SignalName.PrePopupPressed, action, oneShot, deferred);

        public static SignalHandler OnTabButtonPressed(this TabContainer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, TabContainer.SignalName.TabButtonPressed, action, oneShot, deferred);

        public static SignalHandler OnTabChanged(this TabContainer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, TabContainer.SignalName.TabChanged, action, oneShot, deferred);

        public static SignalHandler OnTabSelected(this TabContainer target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, TabContainer.SignalName.TabSelected, action, oneShot, deferred);

        public static SignalHandler OnCaretChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, TextEdit.SignalName.CaretChanged, action, oneShot, deferred);

        public static SignalHandler OnGutterAdded(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, TextEdit.SignalName.GutterAdded, action, oneShot, deferred);

        public static SignalHandler OnGutterClicked(this TextEdit target, Action<System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, TextEdit.SignalName.GutterClicked, action, oneShot, deferred);

        public static SignalHandler OnGutterRemoved(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, TextEdit.SignalName.GutterRemoved, action, oneShot, deferred);

        public static SignalHandler OnLinesEditedFrom(this TextEdit target, Action<System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, TextEdit.SignalName.LinesEditedFrom, action, oneShot, deferred);

        public static SignalHandler OnTextChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, TextEdit.SignalName.TextChanged, action, oneShot, deferred);

        public static SignalHandler OnTextSet(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, TextEdit.SignalName.TextSet, action, oneShot, deferred);

        public static SignalHandler OnTextServerManagerInterfaceAdded(Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(TextServerManager.Singleton, TextServerManager.SignalName.InterfaceAdded, action, oneShot, deferred);

        public static SignalHandler OnTextServerManagerInterfaceRemoved(Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(TextServerManager.Singleton, TextServerManager.SignalName.InterfaceRemoved, action, oneShot, deferred);

        public static SignalHandler OnThemeDBFallbackChanged(Action action, bool oneShot = false, bool deferred = false) =>
            On(ThemeDB.Singleton, ThemeDB.SignalName.FallbackChanged, action, oneShot, deferred);

        public static SignalHandler OnChanged(this TileData target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, TileData.SignalName.Changed, action, oneShot, deferred);

        public static SignalHandler OnChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, TileMap.SignalName.Changed, action, oneShot, deferred);

        public static SignalHandler OnTimeout(this Timer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Timer.SignalName.Timeout, action, oneShot, deferred);

        public static SignalHandler OnPressed(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, TouchScreenButton.SignalName.Pressed, action, oneShot, deferred);

        public static SignalHandler OnReleased(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, TouchScreenButton.SignalName.Released, action, oneShot, deferred);

        public static SignalHandler OnButtonClicked(this Tree target, Action<TreeItem, System.Int32, System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.ButtonClicked, action, oneShot, deferred);

        public static SignalHandler OnCellSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.CellSelected, action, oneShot, deferred);

        public static SignalHandler OnCheckPropagatedToItem(this Tree target, Action<TreeItem, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.CheckPropagatedToItem, action, oneShot, deferred);

        public static SignalHandler OnColumnTitleClicked(this Tree target, Action<System.Int32, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.ColumnTitleClicked, action, oneShot, deferred);

        public static SignalHandler OnCustomItemClicked(this Tree target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.CustomItemClicked, action, oneShot, deferred);

        public static SignalHandler OnCustomPopupEdited(this Tree target, Action<System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.CustomPopupEdited, action, oneShot, deferred);

        public static SignalHandler OnEmptyClicked(this Tree target, Action<Godot.Vector2, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.EmptyClicked, action, oneShot, deferred);

        public static SignalHandler OnItemActivated(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.ItemActivated, action, oneShot, deferred);

        public static SignalHandler OnItemCollapsed(this Tree target, Action<TreeItem> action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.ItemCollapsed, action, oneShot, deferred);

        public static SignalHandler OnItemCustomButtonPressed(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.ItemCustomButtonPressed, action, oneShot, deferred);

        public static SignalHandler OnItemDoubleClicked(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.ItemDoubleClicked, action, oneShot, deferred);

        public static SignalHandler OnItemEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.ItemEdited, action, oneShot, deferred);

        public static SignalHandler OnItemMouseSelected(this Tree target, Action<Godot.Vector2, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.ItemMouseSelected, action, oneShot, deferred);

        public static SignalHandler OnItemSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.ItemSelected, action, oneShot, deferred);

        public static SignalHandler OnMultiSelected(this Tree target, Action<TreeItem, System.Int32, System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.MultiSelected, action, oneShot, deferred);

        public static SignalHandler OnNothingSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Tree.SignalName.NothingSelected, action, oneShot, deferred);

        public static SignalHandler OnFinished(this Tween target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Tween.SignalName.Finished, action, oneShot, deferred);

        public static SignalHandler OnLoopFinished(this Tween target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Tween.SignalName.LoopFinished, action, oneShot, deferred);

        public static SignalHandler OnStepFinished(this Tween target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, Tween.SignalName.StepFinished, action, oneShot, deferred);

        public static SignalHandler OnFinished(this Tweener target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Tweener.SignalName.Finished, action, oneShot, deferred);

        public static SignalHandler OnVersionChanged(this UndoRedo target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, UndoRedo.SignalName.VersionChanged, action, oneShot, deferred);

        public static SignalHandler OnFinished(this VideoStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, VideoStreamPlayer.SignalName.Finished, action, oneShot, deferred);

        public static SignalHandler OnGuiFocusChanged(this Viewport target, Action<Control> action, bool oneShot = false, bool deferred = false) =>
            On(target, Viewport.SignalName.GuiFocusChanged, action, oneShot, deferred);

        public static SignalHandler OnSizeChanged(this Viewport target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Viewport.SignalName.SizeChanged, action, oneShot, deferred);

        public static SignalHandler OnScreenEntered(this VisibleOnScreenNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, VisibleOnScreenNotifier2D.SignalName.ScreenEntered, action, oneShot, deferred);

        public static SignalHandler OnScreenExited(this VisibleOnScreenNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, VisibleOnScreenNotifier2D.SignalName.ScreenExited, action, oneShot, deferred);

        public static SignalHandler OnScreenEntered(this VisibleOnScreenNotifier3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, VisibleOnScreenNotifier3D.SignalName.ScreenEntered, action, oneShot, deferred);

        public static SignalHandler OnScreenExited(this VisibleOnScreenNotifier3D target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, VisibleOnScreenNotifier3D.SignalName.ScreenExited, action, oneShot, deferred);

        public static SignalHandler OnEditorRefreshRequest(this VisualShaderNode target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, VisualShaderNode.SignalName.EditorRefreshRequest, action, oneShot, deferred);

        public static SignalHandler OnInputTypeChanged(this VisualShaderNodeInput target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, VisualShaderNodeInput.SignalName.InputTypeChanged, action, oneShot, deferred);

        public static SignalHandler OnDataChannelReceived(this WebRTCPeerConnection target, Action<WebRTCDataChannel> action, bool oneShot = false, bool deferred = false) =>
            On(target, WebRTCPeerConnection.SignalName.DataChannelReceived, action, oneShot, deferred);

        public static SignalHandler OnIceCandidateCreated(this WebRTCPeerConnection target, Action<System.String, System.Int32, System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, WebRTCPeerConnection.SignalName.IceCandidateCreated, action, oneShot, deferred);

        public static SignalHandler OnSessionDescriptionCreated(this WebRTCPeerConnection target, Action<System.String, System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, WebRTCPeerConnection.SignalName.SessionDescriptionCreated, action, oneShot, deferred);

        public static SignalHandler OnReferenceSpaceReset(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, WebXRInterface.SignalName.ReferenceSpaceReset, action, oneShot, deferred);

        public static SignalHandler OnSelect(this WebXRInterface target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, WebXRInterface.SignalName.Select, action, oneShot, deferred);

        public static SignalHandler OnSelectend(this WebXRInterface target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, WebXRInterface.SignalName.Selectend, action, oneShot, deferred);

        public static SignalHandler OnSelectstart(this WebXRInterface target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, WebXRInterface.SignalName.Selectstart, action, oneShot, deferred);

        public static SignalHandler OnSessionEnded(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, WebXRInterface.SignalName.SessionEnded, action, oneShot, deferred);

        public static SignalHandler OnSessionFailed(this WebXRInterface target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, WebXRInterface.SignalName.SessionFailed, action, oneShot, deferred);

        public static SignalHandler OnSessionStarted(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, WebXRInterface.SignalName.SessionStarted, action, oneShot, deferred);

        public static SignalHandler OnSessionSupported(this WebXRInterface target, Action<System.String, System.Boolean> action, bool oneShot = false, bool deferred = false) =>
            On(target, WebXRInterface.SignalName.SessionSupported, action, oneShot, deferred);

        public static SignalHandler OnSqueeze(this WebXRInterface target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, WebXRInterface.SignalName.Squeeze, action, oneShot, deferred);

        public static SignalHandler OnSqueezeend(this WebXRInterface target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, WebXRInterface.SignalName.Squeezeend, action, oneShot, deferred);

        public static SignalHandler OnSqueezestart(this WebXRInterface target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, WebXRInterface.SignalName.Squeezestart, action, oneShot, deferred);

        public static SignalHandler OnVisibilityStateChanged(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, WebXRInterface.SignalName.VisibilityStateChanged, action, oneShot, deferred);

        public static SignalHandler OnAboutToPopup(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Window.SignalName.AboutToPopup, action, oneShot, deferred);

        public static SignalHandler OnCloseRequested(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Window.SignalName.CloseRequested, action, oneShot, deferred);

        public static SignalHandler OnFilesDropped(this Window target, Action<System.String[]> action, bool oneShot = false, bool deferred = false) =>
            On(target, Window.SignalName.FilesDropped, action, oneShot, deferred);

        public static SignalHandler OnFocusEntered(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Window.SignalName.FocusEntered, action, oneShot, deferred);

        public static SignalHandler OnFocusExited(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Window.SignalName.FocusExited, action, oneShot, deferred);

        public static SignalHandler OnGoBackRequested(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Window.SignalName.GoBackRequested, action, oneShot, deferred);

        public static SignalHandler OnMouseEntered(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Window.SignalName.MouseEntered, action, oneShot, deferred);

        public static SignalHandler OnMouseExited(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Window.SignalName.MouseExited, action, oneShot, deferred);

        public static SignalHandler OnThemeChanged(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Window.SignalName.ThemeChanged, action, oneShot, deferred);

        public static SignalHandler OnTitlebarChanged(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Window.SignalName.TitlebarChanged, action, oneShot, deferred);

        public static SignalHandler OnVisibilityChanged(this Window target, Action action, bool oneShot = false, bool deferred = false) =>
            On(target, Window.SignalName.VisibilityChanged, action, oneShot, deferred);

        public static SignalHandler OnWindowInput(this Window target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            On(target, Window.SignalName.WindowInput, action, oneShot, deferred);

        public static SignalHandler OnButtonPressed(this XRController3D target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, XRController3D.SignalName.ButtonPressed, action, oneShot, deferred);

        public static SignalHandler OnButtonReleased(this XRController3D target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, XRController3D.SignalName.ButtonReleased, action, oneShot, deferred);

        public static SignalHandler OnInputAxisChanged(this XRController3D target, Action<System.String, Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, XRController3D.SignalName.InputAxisChanged, action, oneShot, deferred);

        public static SignalHandler OnInputValueChanged(this XRController3D target, Action<System.String, System.Single> action, bool oneShot = false, bool deferred = false) =>
            On(target, XRController3D.SignalName.InputValueChanged, action, oneShot, deferred);

        public static SignalHandler OnPlayAreaChanged(this XRInterface target, Action<System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(target, XRInterface.SignalName.PlayAreaChanged, action, oneShot, deferred);

        public static SignalHandler OnButtonPressed(this XRPositionalTracker target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, XRPositionalTracker.SignalName.ButtonPressed, action, oneShot, deferred);

        public static SignalHandler OnButtonReleased(this XRPositionalTracker target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, XRPositionalTracker.SignalName.ButtonReleased, action, oneShot, deferred);

        public static SignalHandler OnInputAxisChanged(this XRPositionalTracker target, Action<System.String, Godot.Vector2> action, bool oneShot = false, bool deferred = false) =>
            On(target, XRPositionalTracker.SignalName.InputAxisChanged, action, oneShot, deferred);

        public static SignalHandler OnInputValueChanged(this XRPositionalTracker target, Action<System.String, System.Single> action, bool oneShot = false, bool deferred = false) =>
            On(target, XRPositionalTracker.SignalName.InputValueChanged, action, oneShot, deferred);

        public static SignalHandler OnPoseChanged(this XRPositionalTracker target, Action<XRPose> action, bool oneShot = false, bool deferred = false) =>
            On(target, XRPositionalTracker.SignalName.PoseChanged, action, oneShot, deferred);

        public static SignalHandler OnProfileChanged(this XRPositionalTracker target, Action<System.String> action, bool oneShot = false, bool deferred = false) =>
            On(target, XRPositionalTracker.SignalName.ProfileChanged, action, oneShot, deferred);

        public static SignalHandler OnXRServerInterfaceAdded(Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(XRServer.Singleton, XRServer.SignalName.InterfaceAdded, action, oneShot, deferred);

        public static SignalHandler OnXRServerInterfaceRemoved(Action<Godot.StringName> action, bool oneShot = false, bool deferred = false) =>
            On(XRServer.Singleton, XRServer.SignalName.InterfaceRemoved, action, oneShot, deferred);

        public static SignalHandler OnXRServerTrackerAdded(Action<Godot.StringName, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(XRServer.Singleton, XRServer.SignalName.TrackerAdded, action, oneShot, deferred);

        public static SignalHandler OnXRServerTrackerRemoved(Action<Godot.StringName, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(XRServer.Singleton, XRServer.SignalName.TrackerRemoved, action, oneShot, deferred);

        public static SignalHandler OnXRServerTrackerUpdated(Action<Godot.StringName, System.Int32> action, bool oneShot = false, bool deferred = false) =>
            On(XRServer.Singleton, XRServer.SignalName.TrackerUpdated, action, oneShot, deferred);
    }
}