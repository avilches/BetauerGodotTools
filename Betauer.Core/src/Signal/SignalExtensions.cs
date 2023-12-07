using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Range = Godot.Range;
using static Betauer.Core.Signal.SignalTools; 

namespace Betauer.Core.Signal;

/**
 * Godot version: 4.2-stable (official)
 * Date: 2023-12-07 09:47:30
 *
 * Regular signal C# events don't allow flags as deferred or one shot. This class allows it.
 */
public static partial class SignalExtensions {
  
    public static Action OnCanceled(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AcceptDialog.SignalName.Canceled, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AcceptDialog.SignalName.Canceled, callable);
    }

    public static Action OnConfirmed(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AcceptDialog.SignalName.Confirmed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AcceptDialog.SignalName.Confirmed, callable);
    }

    public static Action OnCustomAction(this AcceptDialog target, Action<StringName> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AcceptDialog.SignalName.CustomAction, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AcceptDialog.SignalName.CustomAction, callable);
    }

    public static Action OnAnimationChanged(this AnimatedSprite2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimatedSprite2D.SignalName.AnimationChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimatedSprite2D.SignalName.AnimationChanged, callable);
    }

    public static Action OnAnimationFinished(this AnimatedSprite2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimatedSprite2D.SignalName.AnimationFinished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimatedSprite2D.SignalName.AnimationFinished, callable);
    }

    public static Action OnAnimationLooped(this AnimatedSprite2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimatedSprite2D.SignalName.AnimationLooped, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimatedSprite2D.SignalName.AnimationLooped, callable);
    }

    public static Action OnFrameChanged(this AnimatedSprite2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimatedSprite2D.SignalName.FrameChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimatedSprite2D.SignalName.FrameChanged, callable);
    }

    public static Action OnSpriteFramesChanged(this AnimatedSprite2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimatedSprite2D.SignalName.SpriteFramesChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimatedSprite2D.SignalName.SpriteFramesChanged, callable);
    }

    public static Action OnAnimationChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimatedSprite3D.SignalName.AnimationChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimatedSprite3D.SignalName.AnimationChanged, callable);
    }

    public static Action OnAnimationFinished(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimatedSprite3D.SignalName.AnimationFinished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimatedSprite3D.SignalName.AnimationFinished, callable);
    }

    public static Action OnAnimationLooped(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimatedSprite3D.SignalName.AnimationLooped, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimatedSprite3D.SignalName.AnimationLooped, callable);
    }

    public static Action OnFrameChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimatedSprite3D.SignalName.FrameChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimatedSprite3D.SignalName.FrameChanged, callable);
    }

    public static Action OnSpriteFramesChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimatedSprite3D.SignalName.SpriteFramesChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimatedSprite3D.SignalName.SpriteFramesChanged, callable);
    }

    public static Action OnAnimationAdded(this AnimationLibrary target, Action<StringName> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationLibrary.SignalName.AnimationAdded, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationLibrary.SignalName.AnimationAdded, callable);
    }

    public static Action OnAnimationChanged(this AnimationLibrary target, Action<StringName> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationLibrary.SignalName.AnimationChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationLibrary.SignalName.AnimationChanged, callable);
    }

    public static Action OnAnimationRemoved(this AnimationLibrary target, Action<StringName> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationLibrary.SignalName.AnimationRemoved, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationLibrary.SignalName.AnimationRemoved, callable);
    }

    public static Action OnAnimationRenamed(this AnimationLibrary target, Action<StringName, StringName> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationLibrary.SignalName.AnimationRenamed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationLibrary.SignalName.AnimationRenamed, callable);
    }

    public static Action OnAnimationFinished(this AnimationMixer target, Action<StringName> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationMixer.SignalName.AnimationFinished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationMixer.SignalName.AnimationFinished, callable);
    }

    public static Action OnAnimationLibrariesUpdated(this AnimationMixer target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationMixer.SignalName.AnimationLibrariesUpdated, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationMixer.SignalName.AnimationLibrariesUpdated, callable);
    }

    public static Action OnAnimationListChanged(this AnimationMixer target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationMixer.SignalName.AnimationListChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationMixer.SignalName.AnimationListChanged, callable);
    }

    public static Action OnAnimationStarted(this AnimationMixer target, Action<StringName> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationMixer.SignalName.AnimationStarted, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationMixer.SignalName.AnimationStarted, callable);
    }

    public static Action OnCachesCleared(this AnimationMixer target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationMixer.SignalName.CachesCleared, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationMixer.SignalName.CachesCleared, callable);
    }

    public static Action OnMixerUpdated(this AnimationMixer target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationMixer.SignalName.MixerUpdated, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationMixer.SignalName.MixerUpdated, callable);
    }

    public static Action OnAnimationNodeRemoved(this AnimationNode target, Action<long, string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationNode.SignalName.AnimationNodeRemoved, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationNode.SignalName.AnimationNodeRemoved, callable);
    }

    public static Action OnAnimationNodeRenamed(this AnimationNode target, Action<long, string, string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationNode.SignalName.AnimationNodeRenamed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationNode.SignalName.AnimationNodeRenamed, callable);
    }

    public static Action OnTreeChanged(this AnimationNode target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationNode.SignalName.TreeChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationNode.SignalName.TreeChanged, callable);
    }

    public static Action OnTrianglesUpdated(this AnimationNodeBlendSpace2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationNodeBlendSpace2D.SignalName.TrianglesUpdated, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationNodeBlendSpace2D.SignalName.TrianglesUpdated, callable);
    }

    public static Action OnNodeChanged(this AnimationNodeBlendTree target, Action<StringName> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationNodeBlendTree.SignalName.NodeChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationNodeBlendTree.SignalName.NodeChanged, callable);
    }

    public static Action OnAdvanceConditionChanged(this AnimationNodeStateMachineTransition target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationNodeStateMachineTransition.SignalName.AdvanceConditionChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationNodeStateMachineTransition.SignalName.AdvanceConditionChanged, callable);
    }

    public static Action OnAnimationChanged(this AnimationPlayer target, Action<StringName, StringName> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationPlayer.SignalName.AnimationChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationPlayer.SignalName.AnimationChanged, callable);
    }

    public static Action OnCurrentAnimationChanged(this AnimationPlayer target, Action<string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationPlayer.SignalName.CurrentAnimationChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationPlayer.SignalName.CurrentAnimationChanged, callable);
    }

    public static Action OnAnimationPlayerChanged(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AnimationTree.SignalName.AnimationPlayerChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AnimationTree.SignalName.AnimationPlayerChanged, callable);
    }

    public static Action OnAreaEntered(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area2D.SignalName.AreaEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area2D.SignalName.AreaEntered, callable);
    }

    public static Action OnAreaExited(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area2D.SignalName.AreaExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area2D.SignalName.AreaExited, callable);
    }

    public static Action OnAreaShapeEntered(this Area2D target, Action<Rid, Area2D, long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area2D.SignalName.AreaShapeEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area2D.SignalName.AreaShapeEntered, callable);
    }

    public static Action OnAreaShapeExited(this Area2D target, Action<Rid, Area2D, long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area2D.SignalName.AreaShapeExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area2D.SignalName.AreaShapeExited, callable);
    }

    public static Action OnBodyEntered(this Area2D target, Action<Node2D> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area2D.SignalName.BodyEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area2D.SignalName.BodyEntered, callable);
    }

    public static Action OnBodyExited(this Area2D target, Action<Node2D> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area2D.SignalName.BodyExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area2D.SignalName.BodyExited, callable);
    }

    public static Action OnBodyShapeEntered(this Area2D target, Action<Rid, Node2D, long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area2D.SignalName.BodyShapeEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area2D.SignalName.BodyShapeEntered, callable);
    }

    public static Action OnBodyShapeExited(this Area2D target, Action<Rid, Node2D, long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area2D.SignalName.BodyShapeExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area2D.SignalName.BodyShapeExited, callable);
    }

    public static Action OnAreaEntered(this Area3D target, Action<Area3D> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area3D.SignalName.AreaEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area3D.SignalName.AreaEntered, callable);
    }

    public static Action OnAreaExited(this Area3D target, Action<Area3D> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area3D.SignalName.AreaExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area3D.SignalName.AreaExited, callable);
    }

    public static Action OnAreaShapeEntered(this Area3D target, Action<Rid, Area3D, long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area3D.SignalName.AreaShapeEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area3D.SignalName.AreaShapeEntered, callable);
    }

    public static Action OnAreaShapeExited(this Area3D target, Action<Rid, Area3D, long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area3D.SignalName.AreaShapeExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area3D.SignalName.AreaShapeExited, callable);
    }

    public static Action OnBodyEntered(this Area3D target, Action<Node3D> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area3D.SignalName.BodyEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area3D.SignalName.BodyEntered, callable);
    }

    public static Action OnBodyExited(this Area3D target, Action<Node3D> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area3D.SignalName.BodyExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area3D.SignalName.BodyExited, callable);
    }

    public static Action OnBodyShapeEntered(this Area3D target, Action<Rid, Node3D, long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area3D.SignalName.BodyShapeEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area3D.SignalName.BodyShapeEntered, callable);
    }

    public static Action OnBodyShapeExited(this Area3D target, Action<Rid, Node3D, long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Area3D.SignalName.BodyShapeExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Area3D.SignalName.BodyShapeExited, callable);
    }

    public static Action OnAudioServerBusLayoutChanged(Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        AudioServer.Singleton.Connect(AudioServer.SignalName.BusLayoutChanged, callable, SignalFlags(oneShot, deferred));
        return () => AudioServer.Singleton.Disconnect(AudioServer.SignalName.BusLayoutChanged, callable);
    }

    public static Action OnAudioServerBusRenamed(Action<long, StringName, StringName> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        AudioServer.Singleton.Connect(AudioServer.SignalName.BusRenamed, callable, SignalFlags(oneShot, deferred));
        return () => AudioServer.Singleton.Disconnect(AudioServer.SignalName.BusRenamed, callable);
    }

    public static Action OnFinished(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AudioStreamPlayer.SignalName.Finished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AudioStreamPlayer.SignalName.Finished, callable);
    }

    public static Action OnFinished(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AudioStreamPlayer2D.SignalName.Finished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AudioStreamPlayer2D.SignalName.Finished, callable);
    }

    public static Action OnFinished(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(AudioStreamPlayer3D.SignalName.Finished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(AudioStreamPlayer3D.SignalName.Finished, callable);
    }

    public static Action OnButtonDown(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(BaseButton.SignalName.ButtonDown, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(BaseButton.SignalName.ButtonDown, callable);
    }

    public static Action OnButtonUp(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(BaseButton.SignalName.ButtonUp, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(BaseButton.SignalName.ButtonUp, callable);
    }

    public static Action OnPressed(this BaseButton target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(BaseButton.SignalName.Pressed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(BaseButton.SignalName.Pressed, callable);
    }

    public static Action OnToggled(this BaseButton target, Action<bool> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(BaseButton.SignalName.Toggled, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(BaseButton.SignalName.Toggled, callable);
    }

    public static Action OnBoneMapUpdated(this BoneMap target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(BoneMap.SignalName.BoneMapUpdated, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(BoneMap.SignalName.BoneMapUpdated, callable);
    }

    public static Action OnProfileUpdated(this BoneMap target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(BoneMap.SignalName.ProfileUpdated, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(BoneMap.SignalName.ProfileUpdated, callable);
    }

    public static Action OnPressed(this ButtonGroup target, Action<BaseButton> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ButtonGroup.SignalName.Pressed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ButtonGroup.SignalName.Pressed, callable);
    }

    public static Action OnCameraServerCameraFeedAdded(Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        CameraServer.Singleton.Connect(CameraServer.SignalName.CameraFeedAdded, callable, SignalFlags(oneShot, deferred));
        return () => CameraServer.Singleton.Disconnect(CameraServer.SignalName.CameraFeedAdded, callable);
    }

    public static Action OnCameraServerCameraFeedRemoved(Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        CameraServer.Singleton.Connect(CameraServer.SignalName.CameraFeedRemoved, callable, SignalFlags(oneShot, deferred));
        return () => CameraServer.Singleton.Disconnect(CameraServer.SignalName.CameraFeedRemoved, callable);
    }

    public static Action OnDraw(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CanvasItem.SignalName.Draw, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CanvasItem.SignalName.Draw, callable);
    }

    public static Action OnHidden(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CanvasItem.SignalName.Hidden, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CanvasItem.SignalName.Hidden, callable);
    }

    public static Action OnItemRectChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CanvasItem.SignalName.ItemRectChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CanvasItem.SignalName.ItemRectChanged, callable);
    }

    public static Action OnVisibilityChanged(this CanvasItem target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CanvasItem.SignalName.VisibilityChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CanvasItem.SignalName.VisibilityChanged, callable);
    }

    public static Action OnVisibilityChanged(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CanvasLayer.SignalName.VisibilityChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CanvasLayer.SignalName.VisibilityChanged, callable);
    }

    public static Action OnBreakpointToggled(this CodeEdit target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CodeEdit.SignalName.BreakpointToggled, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CodeEdit.SignalName.BreakpointToggled, callable);
    }

    public static Action OnCodeCompletionRequested(this CodeEdit target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CodeEdit.SignalName.CodeCompletionRequested, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CodeEdit.SignalName.CodeCompletionRequested, callable);
    }

    public static Action OnSymbolLookup(this CodeEdit target, Action<string, long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CodeEdit.SignalName.SymbolLookup, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CodeEdit.SignalName.SymbolLookup, callable);
    }

    public static Action OnSymbolValidate(this CodeEdit target, Action<string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CodeEdit.SignalName.SymbolValidate, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CodeEdit.SignalName.SymbolValidate, callable);
    }

    public static Action OnInputEvent(this CollisionObject2D target, Action<Node, InputEvent, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CollisionObject2D.SignalName.InputEvent, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CollisionObject2D.SignalName.InputEvent, callable);
    }

    public static Action OnMouseEntered(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CollisionObject2D.SignalName.MouseEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CollisionObject2D.SignalName.MouseEntered, callable);
    }

    public static Action OnMouseExited(this CollisionObject2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CollisionObject2D.SignalName.MouseExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CollisionObject2D.SignalName.MouseExited, callable);
    }

    public static Action OnMouseShapeEntered(this CollisionObject2D target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CollisionObject2D.SignalName.MouseShapeEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CollisionObject2D.SignalName.MouseShapeEntered, callable);
    }

    public static Action OnMouseShapeExited(this CollisionObject2D target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CollisionObject2D.SignalName.MouseShapeExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CollisionObject2D.SignalName.MouseShapeExited, callable);
    }

    public static Action OnInputEvent(this CollisionObject3D target, Action<Node, InputEvent, Vector3, Vector3, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CollisionObject3D.SignalName.InputEvent, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CollisionObject3D.SignalName.InputEvent, callable);
    }

    public static Action OnMouseEntered(this CollisionObject3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CollisionObject3D.SignalName.MouseEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CollisionObject3D.SignalName.MouseEntered, callable);
    }

    public static Action OnMouseExited(this CollisionObject3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(CollisionObject3D.SignalName.MouseExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(CollisionObject3D.SignalName.MouseExited, callable);
    }

    public static Action OnColorChanged(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ColorPicker.SignalName.ColorChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ColorPicker.SignalName.ColorChanged, callable);
    }

    public static Action OnPresetAdded(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ColorPicker.SignalName.PresetAdded, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ColorPicker.SignalName.PresetAdded, callable);
    }

    public static Action OnPresetRemoved(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ColorPicker.SignalName.PresetRemoved, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ColorPicker.SignalName.PresetRemoved, callable);
    }

    public static Action OnColorChanged(this ColorPickerButton target, Action<Color> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ColorPickerButton.SignalName.ColorChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ColorPickerButton.SignalName.ColorChanged, callable);
    }

    public static Action OnPickerCreated(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ColorPickerButton.SignalName.PickerCreated, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ColorPickerButton.SignalName.PickerCreated, callable);
    }

    public static Action OnPopupClosed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ColorPickerButton.SignalName.PopupClosed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ColorPickerButton.SignalName.PopupClosed, callable);
    }

    public static Action OnPreSortChildren(this Container target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Container.SignalName.PreSortChildren, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Container.SignalName.PreSortChildren, callable);
    }

    public static Action OnSortChildren(this Container target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Container.SignalName.SortChildren, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Container.SignalName.SortChildren, callable);
    }

    public static Action OnFocusEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Control.SignalName.FocusEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Control.SignalName.FocusEntered, callable);
    }

    public static Action OnFocusExited(this Control target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Control.SignalName.FocusExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Control.SignalName.FocusExited, callable);
    }

    public static Action OnGuiInput(this Control target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Control.SignalName.GuiInput, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Control.SignalName.GuiInput, callable);
    }

    public static Action OnMinimumSizeChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Control.SignalName.MinimumSizeChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Control.SignalName.MinimumSizeChanged, callable);
    }

    public static Action OnMouseEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Control.SignalName.MouseEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Control.SignalName.MouseEntered, callable);
    }

    public static Action OnMouseExited(this Control target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Control.SignalName.MouseExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Control.SignalName.MouseExited, callable);
    }

    public static Action OnResized(this Control target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Control.SignalName.Resized, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Control.SignalName.Resized, callable);
    }

    public static Action OnSizeFlagsChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Control.SignalName.SizeFlagsChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Control.SignalName.SizeFlagsChanged, callable);
    }

    public static Action OnThemeChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Control.SignalName.ThemeChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Control.SignalName.ThemeChanged, callable);
    }

    public static Action OnRangeChanged(this Curve target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Curve.SignalName.RangeChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Curve.SignalName.RangeChanged, callable);
    }

    public static Action OnDirSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(FileDialog.SignalName.DirSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(FileDialog.SignalName.DirSelected, callable);
    }

    public static Action OnFileSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(FileDialog.SignalName.FileSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(FileDialog.SignalName.FileSelected, callable);
    }

    public static Action OnFilesSelected(this FileDialog target, Action<string[]> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(FileDialog.SignalName.FilesSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(FileDialog.SignalName.FilesSelected, callable);
    }

    public static Action OnGDExtensionManagerExtensionsReloaded(Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        GDExtensionManager.Singleton.Connect(GDExtensionManager.SignalName.ExtensionsReloaded, callable, SignalFlags(oneShot, deferred));
        return () => GDExtensionManager.Singleton.Disconnect(GDExtensionManager.SignalName.ExtensionsReloaded, callable);
    }

    public static Action OnBeginNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.BeginNodeMove, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.BeginNodeMove, callable);
    }

    public static Action OnConnectionDragEnded(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.ConnectionDragEnded, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.ConnectionDragEnded, callable);
    }

    public static Action OnConnectionDragStarted(this GraphEdit target, Action<StringName, long, bool> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.ConnectionDragStarted, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.ConnectionDragStarted, callable);
    }

    public static Action OnConnectionFromEmpty(this GraphEdit target, Action<StringName, long, Vector2> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.ConnectionFromEmpty, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.ConnectionFromEmpty, callable);
    }

    public static Action OnConnectionRequest(this GraphEdit target, Action<StringName, long, StringName, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.ConnectionRequest, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.ConnectionRequest, callable);
    }

    public static Action OnConnectionToEmpty(this GraphEdit target, Action<StringName, long, Vector2> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.ConnectionToEmpty, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.ConnectionToEmpty, callable);
    }

    public static Action OnCopyNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.CopyNodesRequest, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.CopyNodesRequest, callable);
    }

    public static Action OnDeleteNodesRequest(this GraphEdit target, Action<Array> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.DeleteNodesRequest, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.DeleteNodesRequest, callable);
    }

    public static Action OnDisconnectionRequest(this GraphEdit target, Action<StringName, long, StringName, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.DisconnectionRequest, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.DisconnectionRequest, callable);
    }

    public static Action OnDuplicateNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.DuplicateNodesRequest, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.DuplicateNodesRequest, callable);
    }

    public static Action OnEndNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.EndNodeMove, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.EndNodeMove, callable);
    }

    public static Action OnNodeDeselected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.NodeDeselected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.NodeDeselected, callable);
    }

    public static Action OnNodeSelected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.NodeSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.NodeSelected, callable);
    }

    public static Action OnPasteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.PasteNodesRequest, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.PasteNodesRequest, callable);
    }

    public static Action OnPopupRequest(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.PopupRequest, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.PopupRequest, callable);
    }

    public static Action OnScrollOffsetChanged(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphEdit.SignalName.ScrollOffsetChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphEdit.SignalName.ScrollOffsetChanged, callable);
    }

    public static Action OnDeleteRequest(this GraphElement target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphElement.SignalName.DeleteRequest, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphElement.SignalName.DeleteRequest, callable);
    }

    public static Action OnDragged(this GraphElement target, Action<Vector2, Vector2> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphElement.SignalName.Dragged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphElement.SignalName.Dragged, callable);
    }

    public static Action OnNodeDeselected(this GraphElement target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphElement.SignalName.NodeDeselected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphElement.SignalName.NodeDeselected, callable);
    }

    public static Action OnNodeSelected(this GraphElement target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphElement.SignalName.NodeSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphElement.SignalName.NodeSelected, callable);
    }

    public static Action OnPositionOffsetChanged(this GraphElement target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphElement.SignalName.PositionOffsetChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphElement.SignalName.PositionOffsetChanged, callable);
    }

    public static Action OnRaiseRequest(this GraphElement target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphElement.SignalName.RaiseRequest, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphElement.SignalName.RaiseRequest, callable);
    }

    public static Action OnResizeRequest(this GraphElement target, Action<Vector2> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphElement.SignalName.ResizeRequest, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphElement.SignalName.ResizeRequest, callable);
    }

    public static Action OnSlotUpdated(this GraphNode target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GraphNode.SignalName.SlotUpdated, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GraphNode.SignalName.SlotUpdated, callable);
    }

    public static Action OnCellSizeChanged(this GridMap target, Action<Vector3> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GridMap.SignalName.CellSizeChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GridMap.SignalName.CellSizeChanged, callable);
    }

    public static Action OnChanged(this GridMap target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(GridMap.SignalName.Changed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(GridMap.SignalName.Changed, callable);
    }

    public static Action OnInputJoyConnectionChanged(Action<long, bool> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        Input.Singleton.Connect(Input.SignalName.JoyConnectionChanged, callable, SignalFlags(oneShot, deferred));
        return () => Input.Singleton.Disconnect(Input.SignalName.JoyConnectionChanged, callable);
    }

    public static Action OnEmptyClicked(this ItemList target, Action<Vector2, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ItemList.SignalName.EmptyClicked, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ItemList.SignalName.EmptyClicked, callable);
    }

    public static Action OnItemActivated(this ItemList target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ItemList.SignalName.ItemActivated, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ItemList.SignalName.ItemActivated, callable);
    }

    public static Action OnItemClicked(this ItemList target, Action<long, Vector2, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ItemList.SignalName.ItemClicked, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ItemList.SignalName.ItemClicked, callable);
    }

    public static Action OnItemSelected(this ItemList target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ItemList.SignalName.ItemSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ItemList.SignalName.ItemSelected, callable);
    }

    public static Action OnMultiSelected(this ItemList target, Action<long, bool> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ItemList.SignalName.MultiSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ItemList.SignalName.MultiSelected, callable);
    }

    public static Action OnJavaScriptBridgePwaUpdateAvailable(Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        JavaScriptBridge.Singleton.Connect(JavaScriptBridge.SignalName.PwaUpdateAvailable, callable, SignalFlags(oneShot, deferred));
        return () => JavaScriptBridge.Singleton.Disconnect(JavaScriptBridge.SignalName.PwaUpdateAvailable, callable);
    }

    public static Action OnTextChanged(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(LineEdit.SignalName.TextChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(LineEdit.SignalName.TextChanged, callable);
    }

    public static Action OnTextChangeRejected(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(LineEdit.SignalName.TextChangeRejected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(LineEdit.SignalName.TextChangeRejected, callable);
    }

    public static Action OnTextSubmitted(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(LineEdit.SignalName.TextSubmitted, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(LineEdit.SignalName.TextSubmitted, callable);
    }

    public static Action OnOnRequestPermissionsResult(this MainLoop target, Action<string, bool> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(MainLoop.SignalName.OnRequestPermissionsResult, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(MainLoop.SignalName.OnRequestPermissionsResult, callable);
    }

    public static Action OnAboutToPopup(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(MenuButton.SignalName.AboutToPopup, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(MenuButton.SignalName.AboutToPopup, callable);
    }

    public static Action OnTextureChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(MeshInstance2D.SignalName.TextureChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(MeshInstance2D.SignalName.TextureChanged, callable);
    }

    public static Action OnTextureChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(MultiMeshInstance2D.SignalName.TextureChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(MultiMeshInstance2D.SignalName.TextureChanged, callable);
    }

    public static Action OnPeerConnected(this MultiplayerPeer target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(MultiplayerPeer.SignalName.PeerConnected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(MultiplayerPeer.SignalName.PeerConnected, callable);
    }

    public static Action OnPeerDisconnected(this MultiplayerPeer target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(MultiplayerPeer.SignalName.PeerDisconnected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(MultiplayerPeer.SignalName.PeerDisconnected, callable);
    }

    public static Action OnDespawned(this MultiplayerSpawner target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(MultiplayerSpawner.SignalName.Despawned, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(MultiplayerSpawner.SignalName.Despawned, callable);
    }

    public static Action OnSpawned(this MultiplayerSpawner target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(MultiplayerSpawner.SignalName.Spawned, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(MultiplayerSpawner.SignalName.Spawned, callable);
    }

    public static Action OnDeltaSynchronized(this MultiplayerSynchronizer target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(MultiplayerSynchronizer.SignalName.DeltaSynchronized, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(MultiplayerSynchronizer.SignalName.DeltaSynchronized, callable);
    }

    public static Action OnSynchronized(this MultiplayerSynchronizer target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(MultiplayerSynchronizer.SignalName.Synchronized, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(MultiplayerSynchronizer.SignalName.Synchronized, callable);
    }

    public static Action OnVisibilityChanged(this MultiplayerSynchronizer target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(MultiplayerSynchronizer.SignalName.VisibilityChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(MultiplayerSynchronizer.SignalName.VisibilityChanged, callable);
    }

    public static Action OnLinkReached(this NavigationAgent2D target, Action<Dictionary> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationAgent2D.SignalName.LinkReached, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationAgent2D.SignalName.LinkReached, callable);
    }

    public static Action OnNavigationFinished(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationAgent2D.SignalName.NavigationFinished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationAgent2D.SignalName.NavigationFinished, callable);
    }

    public static Action OnPathChanged(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationAgent2D.SignalName.PathChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationAgent2D.SignalName.PathChanged, callable);
    }

    public static Action OnTargetReached(this NavigationAgent2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationAgent2D.SignalName.TargetReached, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationAgent2D.SignalName.TargetReached, callable);
    }

    public static Action OnVelocityComputed(this NavigationAgent2D target, Action<Vector2> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationAgent2D.SignalName.VelocityComputed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationAgent2D.SignalName.VelocityComputed, callable);
    }

    public static Action OnWaypointReached(this NavigationAgent2D target, Action<Dictionary> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationAgent2D.SignalName.WaypointReached, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationAgent2D.SignalName.WaypointReached, callable);
    }

    public static Action OnLinkReached(this NavigationAgent3D target, Action<Dictionary> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationAgent3D.SignalName.LinkReached, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationAgent3D.SignalName.LinkReached, callable);
    }

    public static Action OnNavigationFinished(this NavigationAgent3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationAgent3D.SignalName.NavigationFinished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationAgent3D.SignalName.NavigationFinished, callable);
    }

    public static Action OnPathChanged(this NavigationAgent3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationAgent3D.SignalName.PathChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationAgent3D.SignalName.PathChanged, callable);
    }

    public static Action OnTargetReached(this NavigationAgent3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationAgent3D.SignalName.TargetReached, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationAgent3D.SignalName.TargetReached, callable);
    }

    public static Action OnVelocityComputed(this NavigationAgent3D target, Action<Vector3> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationAgent3D.SignalName.VelocityComputed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationAgent3D.SignalName.VelocityComputed, callable);
    }

    public static Action OnWaypointReached(this NavigationAgent3D target, Action<Dictionary> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationAgent3D.SignalName.WaypointReached, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationAgent3D.SignalName.WaypointReached, callable);
    }

    public static Action OnBakeFinished(this NavigationRegion2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationRegion2D.SignalName.BakeFinished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationRegion2D.SignalName.BakeFinished, callable);
    }

    public static Action OnNavigationPolygonChanged(this NavigationRegion2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationRegion2D.SignalName.NavigationPolygonChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationRegion2D.SignalName.NavigationPolygonChanged, callable);
    }

    public static Action OnBakeFinished(this NavigationRegion3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationRegion3D.SignalName.BakeFinished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationRegion3D.SignalName.BakeFinished, callable);
    }

    public static Action OnNavigationMeshChanged(this NavigationRegion3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NavigationRegion3D.SignalName.NavigationMeshChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NavigationRegion3D.SignalName.NavigationMeshChanged, callable);
    }

    public static Action OnNavigationServer2DMapChanged(Action<Rid> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        NavigationServer2D.Singleton.Connect(NavigationServer2D.SignalName.MapChanged, callable, SignalFlags(oneShot, deferred));
        return () => NavigationServer2D.Singleton.Disconnect(NavigationServer2D.SignalName.MapChanged, callable);
    }

    public static Action OnNavigationServer2DNavigationDebugChanged(Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        NavigationServer2D.Singleton.Connect(NavigationServer2D.SignalName.NavigationDebugChanged, callable, SignalFlags(oneShot, deferred));
        return () => NavigationServer2D.Singleton.Disconnect(NavigationServer2D.SignalName.NavigationDebugChanged, callable);
    }

    public static Action OnNavigationServer3DAvoidanceDebugChanged(Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        NavigationServer3D.Singleton.Connect(NavigationServer3D.SignalName.AvoidanceDebugChanged, callable, SignalFlags(oneShot, deferred));
        return () => NavigationServer3D.Singleton.Disconnect(NavigationServer3D.SignalName.AvoidanceDebugChanged, callable);
    }

    public static Action OnNavigationServer3DMapChanged(Action<Rid> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        NavigationServer3D.Singleton.Connect(NavigationServer3D.SignalName.MapChanged, callable, SignalFlags(oneShot, deferred));
        return () => NavigationServer3D.Singleton.Disconnect(NavigationServer3D.SignalName.MapChanged, callable);
    }

    public static Action OnNavigationServer3DNavigationDebugChanged(Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        NavigationServer3D.Singleton.Connect(NavigationServer3D.SignalName.NavigationDebugChanged, callable, SignalFlags(oneShot, deferred));
        return () => NavigationServer3D.Singleton.Disconnect(NavigationServer3D.SignalName.NavigationDebugChanged, callable);
    }

    public static Action OnTextureChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(NinePatchRect.SignalName.TextureChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(NinePatchRect.SignalName.TextureChanged, callable);
    }

    public static Action OnChildEnteredTree(this Node target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Node.SignalName.ChildEnteredTree, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Node.SignalName.ChildEnteredTree, callable);
    }

    public static Action OnChildExitingTree(this Node target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Node.SignalName.ChildExitingTree, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Node.SignalName.ChildExitingTree, callable);
    }

    public static Action OnChildOrderChanged(this Node target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Node.SignalName.ChildOrderChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Node.SignalName.ChildOrderChanged, callable);
    }

    public static Action OnReady(this Node target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Node.SignalName.Ready, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Node.SignalName.Ready, callable);
    }

    public static Action OnRenamed(this Node target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Node.SignalName.Renamed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Node.SignalName.Renamed, callable);
    }

    public static Action OnReplacingBy(this Node target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Node.SignalName.ReplacingBy, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Node.SignalName.ReplacingBy, callable);
    }

    public static Action OnTreeEntered(this Node target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Node.SignalName.TreeEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Node.SignalName.TreeEntered, callable);
    }

    public static Action OnTreeExited(this Node target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Node.SignalName.TreeExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Node.SignalName.TreeExited, callable);
    }

    public static Action OnTreeExiting(this Node target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Node.SignalName.TreeExiting, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Node.SignalName.TreeExiting, callable);
    }

    public static Action OnVisibilityChanged(this Node3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Node3D.SignalName.VisibilityChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Node3D.SignalName.VisibilityChanged, callable);
    }

    public static Action OnPoseRecentered(this OpenXRInterface target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(OpenXRInterface.SignalName.PoseRecentered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(OpenXRInterface.SignalName.PoseRecentered, callable);
    }

    public static Action OnSessionBegun(this OpenXRInterface target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(OpenXRInterface.SignalName.SessionBegun, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(OpenXRInterface.SignalName.SessionBegun, callable);
    }

    public static Action OnSessionFocussed(this OpenXRInterface target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(OpenXRInterface.SignalName.SessionFocussed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(OpenXRInterface.SignalName.SessionFocussed, callable);
    }

    public static Action OnSessionStopping(this OpenXRInterface target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(OpenXRInterface.SignalName.SessionStopping, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(OpenXRInterface.SignalName.SessionStopping, callable);
    }

    public static Action OnSessionVisible(this OpenXRInterface target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(OpenXRInterface.SignalName.SessionVisible, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(OpenXRInterface.SignalName.SessionVisible, callable);
    }

    public static Action OnItemFocused(this OptionButton target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(OptionButton.SignalName.ItemFocused, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(OptionButton.SignalName.ItemFocused, callable);
    }

    public static Action OnItemSelected(this OptionButton target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(OptionButton.SignalName.ItemSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(OptionButton.SignalName.ItemSelected, callable);
    }

    public static Action OnCurveChanged(this Path3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Path3D.SignalName.CurveChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Path3D.SignalName.CurveChanged, callable);
    }

    public static Action OnPopupHide(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Popup.SignalName.PopupHide, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Popup.SignalName.PopupHide, callable);
    }

    public static Action OnIdFocused(this PopupMenu target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(PopupMenu.SignalName.IdFocused, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(PopupMenu.SignalName.IdFocused, callable);
    }

    public static Action OnIdPressed(this PopupMenu target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(PopupMenu.SignalName.IdPressed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(PopupMenu.SignalName.IdPressed, callable);
    }

    public static Action OnIndexPressed(this PopupMenu target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(PopupMenu.SignalName.IndexPressed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(PopupMenu.SignalName.IndexPressed, callable);
    }

    public static Action OnMenuChanged(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(PopupMenu.SignalName.MenuChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(PopupMenu.SignalName.MenuChanged, callable);
    }

    public static Action OnProjectSettingsSettingsChanged(Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        ProjectSettings.Singleton.Connect(ProjectSettings.SignalName.SettingsChanged, callable, SignalFlags(oneShot, deferred));
        return () => ProjectSettings.Singleton.Disconnect(ProjectSettings.SignalName.SettingsChanged, callable);
    }

    public static Action OnChanged(this Range target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Range.SignalName.Changed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Range.SignalName.Changed, callable);
    }

    public static Action OnValueChanged(this Range target, Action<double> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Range.SignalName.ValueChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Range.SignalName.ValueChanged, callable);
    }

    public static Action OnRenderingServerFramePostDraw(Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        RenderingServer.Singleton.Connect(RenderingServer.SignalName.FramePostDraw, callable, SignalFlags(oneShot, deferred));
        return () => RenderingServer.Singleton.Disconnect(RenderingServer.SignalName.FramePostDraw, callable);
    }

    public static Action OnRenderingServerFramePreDraw(Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        RenderingServer.Singleton.Connect(RenderingServer.SignalName.FramePreDraw, callable, SignalFlags(oneShot, deferred));
        return () => RenderingServer.Singleton.Disconnect(RenderingServer.SignalName.FramePreDraw, callable);
    }

    public static Action OnChanged(this Resource target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Resource.SignalName.Changed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Resource.SignalName.Changed, callable);
    }

    public static Action OnSetupLocalToSceneRequested(this Resource target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Resource.SignalName.SetupLocalToSceneRequested, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Resource.SignalName.SetupLocalToSceneRequested, callable);
    }

    public static Action OnFinished(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(RichTextLabel.SignalName.Finished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(RichTextLabel.SignalName.Finished, callable);
    }

    public static Action OnMetaClicked(this RichTextLabel target, Action<Variant> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(RichTextLabel.SignalName.MetaClicked, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(RichTextLabel.SignalName.MetaClicked, callable);
    }

    public static Action OnMetaHoverEnded(this RichTextLabel target, Action<Variant> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(RichTextLabel.SignalName.MetaHoverEnded, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(RichTextLabel.SignalName.MetaHoverEnded, callable);
    }

    public static Action OnMetaHoverStarted(this RichTextLabel target, Action<Variant> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(RichTextLabel.SignalName.MetaHoverStarted, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(RichTextLabel.SignalName.MetaHoverStarted, callable);
    }

    public static Action OnBodyEntered(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(RigidBody2D.SignalName.BodyEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(RigidBody2D.SignalName.BodyEntered, callable);
    }

    public static Action OnBodyExited(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(RigidBody2D.SignalName.BodyExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(RigidBody2D.SignalName.BodyExited, callable);
    }

    public static Action OnBodyShapeEntered(this RigidBody2D target, Action<Rid, Node, long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(RigidBody2D.SignalName.BodyShapeEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(RigidBody2D.SignalName.BodyShapeEntered, callable);
    }

    public static Action OnBodyShapeExited(this RigidBody2D target, Action<Rid, Node, long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(RigidBody2D.SignalName.BodyShapeExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(RigidBody2D.SignalName.BodyShapeExited, callable);
    }

    public static Action OnSleepingStateChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(RigidBody2D.SignalName.SleepingStateChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(RigidBody2D.SignalName.SleepingStateChanged, callable);
    }

    public static Action OnBodyEntered(this RigidBody3D target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(RigidBody3D.SignalName.BodyEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(RigidBody3D.SignalName.BodyEntered, callable);
    }

    public static Action OnBodyExited(this RigidBody3D target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(RigidBody3D.SignalName.BodyExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(RigidBody3D.SignalName.BodyExited, callable);
    }

    public static Action OnBodyShapeEntered(this RigidBody3D target, Action<Rid, Node, long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(RigidBody3D.SignalName.BodyShapeEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(RigidBody3D.SignalName.BodyShapeEntered, callable);
    }

    public static Action OnBodyShapeExited(this RigidBody3D target, Action<Rid, Node, long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(RigidBody3D.SignalName.BodyShapeExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(RigidBody3D.SignalName.BodyShapeExited, callable);
    }

    public static Action OnSleepingStateChanged(this RigidBody3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(RigidBody3D.SignalName.SleepingStateChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(RigidBody3D.SignalName.SleepingStateChanged, callable);
    }

    public static Action OnPeerAuthenticating(this SceneMultiplayer target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(SceneMultiplayer.SignalName.PeerAuthenticating, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(SceneMultiplayer.SignalName.PeerAuthenticating, callable);
    }

    public static Action OnPeerAuthenticationFailed(this SceneMultiplayer target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(SceneMultiplayer.SignalName.PeerAuthenticationFailed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(SceneMultiplayer.SignalName.PeerAuthenticationFailed, callable);
    }

    public static Action OnPeerPacket(this SceneMultiplayer target, Action<long, byte[]> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(SceneMultiplayer.SignalName.PeerPacket, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(SceneMultiplayer.SignalName.PeerPacket, callable);
    }

    public static Action OnNodeAdded(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(SceneTree.SignalName.NodeAdded, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(SceneTree.SignalName.NodeAdded, callable);
    }

    public static Action OnNodeConfigurationWarningChanged(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(SceneTree.SignalName.NodeConfigurationWarningChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(SceneTree.SignalName.NodeConfigurationWarningChanged, callable);
    }

    public static Action OnNodeRemoved(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(SceneTree.SignalName.NodeRemoved, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(SceneTree.SignalName.NodeRemoved, callable);
    }

    public static Action OnNodeRenamed(this SceneTree target, Action<Node> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(SceneTree.SignalName.NodeRenamed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(SceneTree.SignalName.NodeRenamed, callable);
    }

    public static Action OnPhysicsFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(SceneTree.SignalName.PhysicsFrame, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(SceneTree.SignalName.PhysicsFrame, callable);
    }

    public static Action OnProcessFrame(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(SceneTree.SignalName.ProcessFrame, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(SceneTree.SignalName.ProcessFrame, callable);
    }

    public static Action OnTreeChanged(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(SceneTree.SignalName.TreeChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(SceneTree.SignalName.TreeChanged, callable);
    }

    public static Action OnTreeProcessModeChanged(this SceneTree target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(SceneTree.SignalName.TreeProcessModeChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(SceneTree.SignalName.TreeProcessModeChanged, callable);
    }

    public static Action OnTimeout(this SceneTreeTimer target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(SceneTreeTimer.SignalName.Timeout, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(SceneTreeTimer.SignalName.Timeout, callable);
    }

    public static Action OnScrolling(this ScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ScrollBar.SignalName.Scrolling, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ScrollBar.SignalName.Scrolling, callable);
    }

    public static Action OnScrollEnded(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ScrollContainer.SignalName.ScrollEnded, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ScrollContainer.SignalName.ScrollEnded, callable);
    }

    public static Action OnScrollStarted(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(ScrollContainer.SignalName.ScrollStarted, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(ScrollContainer.SignalName.ScrollStarted, callable);
    }

    public static Action OnBoneSetupChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Skeleton2D.SignalName.BoneSetupChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Skeleton2D.SignalName.BoneSetupChanged, callable);
    }

    public static Action OnBoneEnabledChanged(this Skeleton3D target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Skeleton3D.SignalName.BoneEnabledChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Skeleton3D.SignalName.BoneEnabledChanged, callable);
    }

    public static Action OnBonePoseChanged(this Skeleton3D target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Skeleton3D.SignalName.BonePoseChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Skeleton3D.SignalName.BonePoseChanged, callable);
    }

    public static Action OnPoseUpdated(this Skeleton3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Skeleton3D.SignalName.PoseUpdated, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Skeleton3D.SignalName.PoseUpdated, callable);
    }

    public static Action OnShowRestOnlyChanged(this Skeleton3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Skeleton3D.SignalName.ShowRestOnlyChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Skeleton3D.SignalName.ShowRestOnlyChanged, callable);
    }

    public static Action OnProfileUpdated(this SkeletonProfile target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(SkeletonProfile.SignalName.ProfileUpdated, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(SkeletonProfile.SignalName.ProfileUpdated, callable);
    }

    public static Action OnDragEnded(this Slider target, Action<bool> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Slider.SignalName.DragEnded, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Slider.SignalName.DragEnded, callable);
    }

    public static Action OnDragStarted(this Slider target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Slider.SignalName.DragStarted, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Slider.SignalName.DragStarted, callable);
    }

    public static Action OnDragged(this SplitContainer target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(SplitContainer.SignalName.Dragged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(SplitContainer.SignalName.Dragged, callable);
    }

    public static Action OnFrameChanged(this Sprite2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Sprite2D.SignalName.FrameChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Sprite2D.SignalName.FrameChanged, callable);
    }

    public static Action OnTextureChanged(this Sprite2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Sprite2D.SignalName.TextureChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Sprite2D.SignalName.TextureChanged, callable);
    }

    public static Action OnFrameChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Sprite3D.SignalName.FrameChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Sprite3D.SignalName.FrameChanged, callable);
    }

    public static Action OnTextureChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Sprite3D.SignalName.TextureChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Sprite3D.SignalName.TextureChanged, callable);
    }

    public static Action OnActiveTabRearranged(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabBar.SignalName.ActiveTabRearranged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabBar.SignalName.ActiveTabRearranged, callable);
    }

    public static Action OnTabButtonPressed(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabBar.SignalName.TabButtonPressed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabBar.SignalName.TabButtonPressed, callable);
    }

    public static Action OnTabChanged(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabBar.SignalName.TabChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabBar.SignalName.TabChanged, callable);
    }

    public static Action OnTabClicked(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabBar.SignalName.TabClicked, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabBar.SignalName.TabClicked, callable);
    }

    public static Action OnTabClosePressed(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabBar.SignalName.TabClosePressed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabBar.SignalName.TabClosePressed, callable);
    }

    public static Action OnTabHovered(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabBar.SignalName.TabHovered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabBar.SignalName.TabHovered, callable);
    }

    public static Action OnTabRmbClicked(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabBar.SignalName.TabRmbClicked, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabBar.SignalName.TabRmbClicked, callable);
    }

    public static Action OnTabSelected(this TabBar target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabBar.SignalName.TabSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabBar.SignalName.TabSelected, callable);
    }

    public static Action OnActiveTabRearranged(this TabContainer target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabContainer.SignalName.ActiveTabRearranged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabContainer.SignalName.ActiveTabRearranged, callable);
    }

    public static Action OnPrePopupPressed(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabContainer.SignalName.PrePopupPressed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabContainer.SignalName.PrePopupPressed, callable);
    }

    public static Action OnTabButtonPressed(this TabContainer target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabContainer.SignalName.TabButtonPressed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabContainer.SignalName.TabButtonPressed, callable);
    }

    public static Action OnTabChanged(this TabContainer target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabContainer.SignalName.TabChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabContainer.SignalName.TabChanged, callable);
    }

    public static Action OnTabClicked(this TabContainer target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabContainer.SignalName.TabClicked, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabContainer.SignalName.TabClicked, callable);
    }

    public static Action OnTabHovered(this TabContainer target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabContainer.SignalName.TabHovered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabContainer.SignalName.TabHovered, callable);
    }

    public static Action OnTabSelected(this TabContainer target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TabContainer.SignalName.TabSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TabContainer.SignalName.TabSelected, callable);
    }

    public static Action OnCaretChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TextEdit.SignalName.CaretChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TextEdit.SignalName.CaretChanged, callable);
    }

    public static Action OnGutterAdded(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TextEdit.SignalName.GutterAdded, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TextEdit.SignalName.GutterAdded, callable);
    }

    public static Action OnGutterClicked(this TextEdit target, Action<long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TextEdit.SignalName.GutterClicked, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TextEdit.SignalName.GutterClicked, callable);
    }

    public static Action OnGutterRemoved(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TextEdit.SignalName.GutterRemoved, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TextEdit.SignalName.GutterRemoved, callable);
    }

    public static Action OnLinesEditedFrom(this TextEdit target, Action<long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TextEdit.SignalName.LinesEditedFrom, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TextEdit.SignalName.LinesEditedFrom, callable);
    }

    public static Action OnTextChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TextEdit.SignalName.TextChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TextEdit.SignalName.TextChanged, callable);
    }

    public static Action OnTextSet(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TextEdit.SignalName.TextSet, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TextEdit.SignalName.TextSet, callable);
    }

    public static Action OnTextServerManagerInterfaceAdded(Action<StringName> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        TextServerManager.Singleton.Connect(TextServerManager.SignalName.InterfaceAdded, callable, SignalFlags(oneShot, deferred));
        return () => TextServerManager.Singleton.Disconnect(TextServerManager.SignalName.InterfaceAdded, callable);
    }

    public static Action OnTextServerManagerInterfaceRemoved(Action<StringName> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        TextServerManager.Singleton.Connect(TextServerManager.SignalName.InterfaceRemoved, callable, SignalFlags(oneShot, deferred));
        return () => TextServerManager.Singleton.Disconnect(TextServerManager.SignalName.InterfaceRemoved, callable);
    }

    public static Action OnThemeDBFallbackChanged(Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        ThemeDB.Singleton.Connect(ThemeDB.SignalName.FallbackChanged, callable, SignalFlags(oneShot, deferred));
        return () => ThemeDB.Singleton.Disconnect(ThemeDB.SignalName.FallbackChanged, callable);
    }

    public static Action OnChanged(this TileData target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TileData.SignalName.Changed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TileData.SignalName.Changed, callable);
    }

    public static Action OnChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TileMap.SignalName.Changed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TileMap.SignalName.Changed, callable);
    }

    public static Action OnTimeout(this Timer target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Timer.SignalName.Timeout, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Timer.SignalName.Timeout, callable);
    }

    public static Action OnPressed(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TouchScreenButton.SignalName.Pressed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TouchScreenButton.SignalName.Pressed, callable);
    }

    public static Action OnReleased(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(TouchScreenButton.SignalName.Released, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(TouchScreenButton.SignalName.Released, callable);
    }

    public static Action OnButtonClicked(this Tree target, Action<TreeItem, long, long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.ButtonClicked, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.ButtonClicked, callable);
    }

    public static Action OnCellSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.CellSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.CellSelected, callable);
    }

    public static Action OnCheckPropagatedToItem(this Tree target, Action<TreeItem, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.CheckPropagatedToItem, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.CheckPropagatedToItem, callable);
    }

    public static Action OnColumnTitleClicked(this Tree target, Action<long, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.ColumnTitleClicked, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.ColumnTitleClicked, callable);
    }

    public static Action OnCustomItemClicked(this Tree target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.CustomItemClicked, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.CustomItemClicked, callable);
    }

    public static Action OnCustomPopupEdited(this Tree target, Action<bool> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.CustomPopupEdited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.CustomPopupEdited, callable);
    }

    public static Action OnEmptyClicked(this Tree target, Action<Vector2, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.EmptyClicked, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.EmptyClicked, callable);
    }

    public static Action OnItemActivated(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.ItemActivated, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.ItemActivated, callable);
    }

    public static Action OnItemCollapsed(this Tree target, Action<TreeItem> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.ItemCollapsed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.ItemCollapsed, callable);
    }

    public static Action OnItemEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.ItemEdited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.ItemEdited, callable);
    }

    public static Action OnItemIconDoubleClicked(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.ItemIconDoubleClicked, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.ItemIconDoubleClicked, callable);
    }

    public static Action OnItemMouseSelected(this Tree target, Action<Vector2, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.ItemMouseSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.ItemMouseSelected, callable);
    }

    public static Action OnItemSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.ItemSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.ItemSelected, callable);
    }

    public static Action OnMultiSelected(this Tree target, Action<TreeItem, long, bool> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.MultiSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.MultiSelected, callable);
    }

    public static Action OnNothingSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tree.SignalName.NothingSelected, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tree.SignalName.NothingSelected, callable);
    }

    public static Action OnFinished(this Tween target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tween.SignalName.Finished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tween.SignalName.Finished, callable);
    }

    public static Action OnLoopFinished(this Tween target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tween.SignalName.LoopFinished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tween.SignalName.LoopFinished, callable);
    }

    public static Action OnStepFinished(this Tween target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tween.SignalName.StepFinished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tween.SignalName.StepFinished, callable);
    }

    public static Action OnFinished(this Tweener target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Tweener.SignalName.Finished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Tweener.SignalName.Finished, callable);
    }

    public static Action OnVersionChanged(this UndoRedo target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(UndoRedo.SignalName.VersionChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(UndoRedo.SignalName.VersionChanged, callable);
    }

    public static Action OnFinished(this VideoStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(VideoStreamPlayer.SignalName.Finished, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(VideoStreamPlayer.SignalName.Finished, callable);
    }

    public static Action OnGuiFocusChanged(this Viewport target, Action<Control> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Viewport.SignalName.GuiFocusChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Viewport.SignalName.GuiFocusChanged, callable);
    }

    public static Action OnSizeChanged(this Viewport target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Viewport.SignalName.SizeChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Viewport.SignalName.SizeChanged, callable);
    }

    public static Action OnScreenEntered(this VisibleOnScreenNotifier2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(VisibleOnScreenNotifier2D.SignalName.ScreenEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(VisibleOnScreenNotifier2D.SignalName.ScreenEntered, callable);
    }

    public static Action OnScreenExited(this VisibleOnScreenNotifier2D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(VisibleOnScreenNotifier2D.SignalName.ScreenExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(VisibleOnScreenNotifier2D.SignalName.ScreenExited, callable);
    }

    public static Action OnScreenEntered(this VisibleOnScreenNotifier3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(VisibleOnScreenNotifier3D.SignalName.ScreenEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(VisibleOnScreenNotifier3D.SignalName.ScreenEntered, callable);
    }

    public static Action OnScreenExited(this VisibleOnScreenNotifier3D target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(VisibleOnScreenNotifier3D.SignalName.ScreenExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(VisibleOnScreenNotifier3D.SignalName.ScreenExited, callable);
    }

    public static Action OnInputTypeChanged(this VisualShaderNodeInput target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(VisualShaderNodeInput.SignalName.InputTypeChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(VisualShaderNodeInput.SignalName.InputTypeChanged, callable);
    }

    public static Action OnDisplayRefreshRateChanged(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(WebXRInterface.SignalName.DisplayRefreshRateChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(WebXRInterface.SignalName.DisplayRefreshRateChanged, callable);
    }

    public static Action OnReferenceSpaceReset(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(WebXRInterface.SignalName.ReferenceSpaceReset, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(WebXRInterface.SignalName.ReferenceSpaceReset, callable);
    }

    public static Action OnSelect(this WebXRInterface target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(WebXRInterface.SignalName.Select, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(WebXRInterface.SignalName.Select, callable);
    }

    public static Action OnSelectend(this WebXRInterface target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(WebXRInterface.SignalName.Selectend, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(WebXRInterface.SignalName.Selectend, callable);
    }

    public static Action OnSelectstart(this WebXRInterface target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(WebXRInterface.SignalName.Selectstart, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(WebXRInterface.SignalName.Selectstart, callable);
    }

    public static Action OnSessionEnded(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(WebXRInterface.SignalName.SessionEnded, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(WebXRInterface.SignalName.SessionEnded, callable);
    }

    public static Action OnSessionFailed(this WebXRInterface target, Action<string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(WebXRInterface.SignalName.SessionFailed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(WebXRInterface.SignalName.SessionFailed, callable);
    }

    public static Action OnSessionStarted(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(WebXRInterface.SignalName.SessionStarted, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(WebXRInterface.SignalName.SessionStarted, callable);
    }

    public static Action OnSessionSupported(this WebXRInterface target, Action<string, bool> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(WebXRInterface.SignalName.SessionSupported, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(WebXRInterface.SignalName.SessionSupported, callable);
    }

    public static Action OnSqueeze(this WebXRInterface target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(WebXRInterface.SignalName.Squeeze, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(WebXRInterface.SignalName.Squeeze, callable);
    }

    public static Action OnSqueezeend(this WebXRInterface target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(WebXRInterface.SignalName.Squeezeend, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(WebXRInterface.SignalName.Squeezeend, callable);
    }

    public static Action OnSqueezestart(this WebXRInterface target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(WebXRInterface.SignalName.Squeezestart, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(WebXRInterface.SignalName.Squeezestart, callable);
    }

    public static Action OnVisibilityStateChanged(this WebXRInterface target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(WebXRInterface.SignalName.VisibilityStateChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(WebXRInterface.SignalName.VisibilityStateChanged, callable);
    }

    public static Action OnAboutToPopup(this Window target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Window.SignalName.AboutToPopup, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Window.SignalName.AboutToPopup, callable);
    }

    public static Action OnCloseRequested(this Window target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Window.SignalName.CloseRequested, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Window.SignalName.CloseRequested, callable);
    }

    public static Action OnDpiChanged(this Window target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Window.SignalName.DpiChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Window.SignalName.DpiChanged, callable);
    }

    public static Action OnFilesDropped(this Window target, Action<string[]> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Window.SignalName.FilesDropped, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Window.SignalName.FilesDropped, callable);
    }

    public static Action OnFocusEntered(this Window target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Window.SignalName.FocusEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Window.SignalName.FocusEntered, callable);
    }

    public static Action OnFocusExited(this Window target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Window.SignalName.FocusExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Window.SignalName.FocusExited, callable);
    }

    public static Action OnGoBackRequested(this Window target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Window.SignalName.GoBackRequested, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Window.SignalName.GoBackRequested, callable);
    }

    public static Action OnMouseEntered(this Window target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Window.SignalName.MouseEntered, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Window.SignalName.MouseEntered, callable);
    }

    public static Action OnMouseExited(this Window target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Window.SignalName.MouseExited, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Window.SignalName.MouseExited, callable);
    }

    public static Action OnThemeChanged(this Window target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Window.SignalName.ThemeChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Window.SignalName.ThemeChanged, callable);
    }

    public static Action OnTitlebarChanged(this Window target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Window.SignalName.TitlebarChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Window.SignalName.TitlebarChanged, callable);
    }

    public static Action OnVisibilityChanged(this Window target, Action action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Window.SignalName.VisibilityChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Window.SignalName.VisibilityChanged, callable);
    }

    public static Action OnWindowInput(this Window target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(Window.SignalName.WindowInput, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(Window.SignalName.WindowInput, callable);
    }

    public static Action OnButtonPressed(this XRController3D target, Action<string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(XRController3D.SignalName.ButtonPressed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(XRController3D.SignalName.ButtonPressed, callable);
    }

    public static Action OnButtonReleased(this XRController3D target, Action<string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(XRController3D.SignalName.ButtonReleased, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(XRController3D.SignalName.ButtonReleased, callable);
    }

    public static Action OnInputFloatChanged(this XRController3D target, Action<string, double> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(XRController3D.SignalName.InputFloatChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(XRController3D.SignalName.InputFloatChanged, callable);
    }

    public static Action OnInputVector2Changed(this XRController3D target, Action<string, Vector2> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(XRController3D.SignalName.InputVector2Changed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(XRController3D.SignalName.InputVector2Changed, callable);
    }

    public static Action OnPlayAreaChanged(this XRInterface target, Action<long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(XRInterface.SignalName.PlayAreaChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(XRInterface.SignalName.PlayAreaChanged, callable);
    }

    public static Action OnTrackingChanged(this XRNode3D target, Action<bool> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(XRNode3D.SignalName.TrackingChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(XRNode3D.SignalName.TrackingChanged, callable);
    }

    public static Action OnButtonPressed(this XRPositionalTracker target, Action<string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(XRPositionalTracker.SignalName.ButtonPressed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(XRPositionalTracker.SignalName.ButtonPressed, callable);
    }

    public static Action OnButtonReleased(this XRPositionalTracker target, Action<string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(XRPositionalTracker.SignalName.ButtonReleased, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(XRPositionalTracker.SignalName.ButtonReleased, callable);
    }

    public static Action OnInputFloatChanged(this XRPositionalTracker target, Action<string, double> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(XRPositionalTracker.SignalName.InputFloatChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(XRPositionalTracker.SignalName.InputFloatChanged, callable);
    }

    public static Action OnInputVector2Changed(this XRPositionalTracker target, Action<string, Vector2> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(XRPositionalTracker.SignalName.InputVector2Changed, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(XRPositionalTracker.SignalName.InputVector2Changed, callable);
    }

    public static Action OnPoseChanged(this XRPositionalTracker target, Action<XRPose> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(XRPositionalTracker.SignalName.PoseChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(XRPositionalTracker.SignalName.PoseChanged, callable);
    }

    public static Action OnPoseLostTracking(this XRPositionalTracker target, Action<XRPose> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(XRPositionalTracker.SignalName.PoseLostTracking, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(XRPositionalTracker.SignalName.PoseLostTracking, callable);
    }

    public static Action OnProfileChanged(this XRPositionalTracker target, Action<string> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        target.Connect(XRPositionalTracker.SignalName.ProfileChanged, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect(XRPositionalTracker.SignalName.ProfileChanged, callable);
    }

    public static Action OnXRServerInterfaceAdded(Action<StringName> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        XRServer.Singleton.Connect(XRServer.SignalName.InterfaceAdded, callable, SignalFlags(oneShot, deferred));
        return () => XRServer.Singleton.Disconnect(XRServer.SignalName.InterfaceAdded, callable);
    }

    public static Action OnXRServerInterfaceRemoved(Action<StringName> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        XRServer.Singleton.Connect(XRServer.SignalName.InterfaceRemoved, callable, SignalFlags(oneShot, deferred));
        return () => XRServer.Singleton.Disconnect(XRServer.SignalName.InterfaceRemoved, callable);
    }

    public static Action OnXRServerTrackerAdded(Action<StringName, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        XRServer.Singleton.Connect(XRServer.SignalName.TrackerAdded, callable, SignalFlags(oneShot, deferred));
        return () => XRServer.Singleton.Disconnect(XRServer.SignalName.TrackerAdded, callable);
    }

    public static Action OnXRServerTrackerRemoved(Action<StringName, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        XRServer.Singleton.Connect(XRServer.SignalName.TrackerRemoved, callable, SignalFlags(oneShot, deferred));
        return () => XRServer.Singleton.Disconnect(XRServer.SignalName.TrackerRemoved, callable);
    }

    public static Action OnXRServerTrackerUpdated(Action<StringName, long> action, bool oneShot = false, bool deferred = false) {
        var callable = Callable.From(action);
        XRServer.Singleton.Connect(XRServer.SignalName.TrackerUpdated, callable, SignalFlags(oneShot, deferred));
        return () => XRServer.Singleton.Disconnect(XRServer.SignalName.TrackerUpdated, callable);
    }
}