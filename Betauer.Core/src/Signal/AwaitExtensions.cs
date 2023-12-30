using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Range = Godot.Range;

namespace Betauer.Core.Signal;

/**
 * Godot version: 4.2-stable (official)
 * Date: 2023-12-30 13:13:02
 */
public static partial class AwaitExtensions {
  
    public static SignalAwaiter AwaitCanceled(this AcceptDialog target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AcceptDialog.SignalName.Canceled);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitConfirmed(this AcceptDialog target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AcceptDialog.SignalName.Confirmed);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCustomAction(this AcceptDialog target, Action<StringName>? onComplete = null) {
        var awaiter = target.ToSignal(target, AcceptDialog.SignalName.CustomAction);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationChanged(this AnimatedSprite2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimatedSprite2D.SignalName.AnimationChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationFinished(this AnimatedSprite2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimatedSprite2D.SignalName.AnimationFinished);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationLooped(this AnimatedSprite2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimatedSprite2D.SignalName.AnimationLooped);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFrameChanged(this AnimatedSprite2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimatedSprite2D.SignalName.FrameChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSpriteFramesChanged(this AnimatedSprite2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimatedSprite2D.SignalName.SpriteFramesChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationChanged(this AnimatedSprite3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimatedSprite3D.SignalName.AnimationChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationFinished(this AnimatedSprite3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimatedSprite3D.SignalName.AnimationFinished);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationLooped(this AnimatedSprite3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimatedSprite3D.SignalName.AnimationLooped);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFrameChanged(this AnimatedSprite3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimatedSprite3D.SignalName.FrameChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSpriteFramesChanged(this AnimatedSprite3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimatedSprite3D.SignalName.SpriteFramesChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationAdded(this AnimationLibrary target, Action<StringName>? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationLibrary.SignalName.AnimationAdded);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationChanged(this AnimationLibrary target, Action<StringName>? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationLibrary.SignalName.AnimationChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationRemoved(this AnimationLibrary target, Action<StringName>? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationLibrary.SignalName.AnimationRemoved);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationRenamed(this AnimationLibrary target, Action<StringName, StringName>? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationLibrary.SignalName.AnimationRenamed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>(), awaiter.GetResult()[1].As<StringName>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationFinished(this AnimationMixer target, Action<StringName>? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationMixer.SignalName.AnimationFinished);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationLibrariesUpdated(this AnimationMixer target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationMixer.SignalName.AnimationLibrariesUpdated);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationListChanged(this AnimationMixer target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationMixer.SignalName.AnimationListChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationStarted(this AnimationMixer target, Action<StringName>? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationMixer.SignalName.AnimationStarted);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCachesCleared(this AnimationMixer target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationMixer.SignalName.CachesCleared);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMixerUpdated(this AnimationMixer target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationMixer.SignalName.MixerUpdated);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationNodeRemoved(this AnimationNode target, Action<long, string>? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationNode.SignalName.AnimationNodeRemoved);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>(), awaiter.GetResult()[1].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationNodeRenamed(this AnimationNode target, Action<long, string, string>? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationNode.SignalName.AnimationNodeRenamed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>(), awaiter.GetResult()[1].As<string>(), awaiter.GetResult()[2].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTreeChanged(this AnimationNode target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationNode.SignalName.TreeChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTrianglesUpdated(this AnimationNodeBlendSpace2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationNodeBlendSpace2D.SignalName.TrianglesUpdated);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNodeChanged(this AnimationNodeBlendTree target, Action<StringName>? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationNodeBlendTree.SignalName.NodeChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAdvanceConditionChanged(this AnimationNodeStateMachineTransition target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationNodeStateMachineTransition.SignalName.AdvanceConditionChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationChanged(this AnimationPlayer target, Action<StringName, StringName>? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationPlayer.SignalName.AnimationChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>(), awaiter.GetResult()[1].As<StringName>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCurrentAnimationChanged(this AnimationPlayer target, Action<string>? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationPlayer.SignalName.CurrentAnimationChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAnimationPlayerChanged(this AnimationTree target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AnimationTree.SignalName.AnimationPlayerChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAreaEntered(this Area2D target, Action<Area2D>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area2D.SignalName.AreaEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Area2D>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAreaExited(this Area2D target, Action<Area2D>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area2D.SignalName.AreaExited);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Area2D>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAreaShapeEntered(this Area2D target, Action<Rid, Area2D, long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area2D.SignalName.AreaShapeEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Rid>(), awaiter.GetResult()[1].As<Area2D>(), awaiter.GetResult()[2].As<long>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAreaShapeExited(this Area2D target, Action<Rid, Area2D, long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area2D.SignalName.AreaShapeExited);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Rid>(), awaiter.GetResult()[1].As<Area2D>(), awaiter.GetResult()[2].As<long>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyEntered(this Area2D target, Action<Node2D>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area2D.SignalName.BodyEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node2D>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyExited(this Area2D target, Action<Node2D>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area2D.SignalName.BodyExited);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node2D>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyShapeEntered(this Area2D target, Action<Rid, Node2D, long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area2D.SignalName.BodyShapeEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Rid>(), awaiter.GetResult()[1].As<Node2D>(), awaiter.GetResult()[2].As<long>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyShapeExited(this Area2D target, Action<Rid, Node2D, long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area2D.SignalName.BodyShapeExited);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Rid>(), awaiter.GetResult()[1].As<Node2D>(), awaiter.GetResult()[2].As<long>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAreaEntered(this Area3D target, Action<Area3D>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area3D.SignalName.AreaEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Area3D>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAreaExited(this Area3D target, Action<Area3D>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area3D.SignalName.AreaExited);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Area3D>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAreaShapeEntered(this Area3D target, Action<Rid, Area3D, long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area3D.SignalName.AreaShapeEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Rid>(), awaiter.GetResult()[1].As<Area3D>(), awaiter.GetResult()[2].As<long>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAreaShapeExited(this Area3D target, Action<Rid, Area3D, long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area3D.SignalName.AreaShapeExited);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Rid>(), awaiter.GetResult()[1].As<Area3D>(), awaiter.GetResult()[2].As<long>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyEntered(this Area3D target, Action<Node3D>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area3D.SignalName.BodyEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node3D>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyExited(this Area3D target, Action<Node3D>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area3D.SignalName.BodyExited);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node3D>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyShapeEntered(this Area3D target, Action<Rid, Node3D, long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area3D.SignalName.BodyShapeEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Rid>(), awaiter.GetResult()[1].As<Node3D>(), awaiter.GetResult()[2].As<long>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyShapeExited(this Area3D target, Action<Rid, Node3D, long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Area3D.SignalName.BodyShapeExited);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Rid>(), awaiter.GetResult()[1].As<Node3D>(), awaiter.GetResult()[2].As<long>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAudioServerBusLayoutChanged(Action? onComplete = null) {
        var awaiter = AudioServer.Singleton.ToSignal(AudioServer.Singleton, AudioServer.SignalName.BusLayoutChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAudioServerBusRenamed(Action<long, StringName, StringName>? onComplete = null) {
        var awaiter = AudioServer.Singleton.ToSignal(AudioServer.Singleton, AudioServer.SignalName.BusRenamed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>(), awaiter.GetResult()[1].As<StringName>(), awaiter.GetResult()[2].As<StringName>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFinished(this AudioStreamPlayer target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AudioStreamPlayer.SignalName.Finished);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFinished(this AudioStreamPlayer2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AudioStreamPlayer2D.SignalName.Finished);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFinished(this AudioStreamPlayer3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, AudioStreamPlayer3D.SignalName.Finished);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitButtonDown(this BaseButton target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, BaseButton.SignalName.ButtonDown);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitButtonUp(this BaseButton target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, BaseButton.SignalName.ButtonUp);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPressed(this BaseButton target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, BaseButton.SignalName.Pressed);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitToggled(this BaseButton target, Action<bool>? onComplete = null) {
        var awaiter = target.ToSignal(target, BaseButton.SignalName.Toggled);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<bool>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBoneMapUpdated(this BoneMap target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, BoneMap.SignalName.BoneMapUpdated);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitProfileUpdated(this BoneMap target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, BoneMap.SignalName.ProfileUpdated);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPressed(this ButtonGroup target, Action<BaseButton>? onComplete = null) {
        var awaiter = target.ToSignal(target, ButtonGroup.SignalName.Pressed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<BaseButton>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCameraServerCameraFeedAdded(Action<long>? onComplete = null) {
        var awaiter = CameraServer.Singleton.ToSignal(CameraServer.Singleton, CameraServer.SignalName.CameraFeedAdded);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCameraServerCameraFeedRemoved(Action<long>? onComplete = null) {
        var awaiter = CameraServer.Singleton.ToSignal(CameraServer.Singleton, CameraServer.SignalName.CameraFeedRemoved);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitDraw(this CanvasItem target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, CanvasItem.SignalName.Draw);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitHidden(this CanvasItem target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, CanvasItem.SignalName.Hidden);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitItemRectChanged(this CanvasItem target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, CanvasItem.SignalName.ItemRectChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitVisibilityChanged(this CanvasItem target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, CanvasItem.SignalName.VisibilityChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitVisibilityChanged(this CanvasLayer target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, CanvasLayer.SignalName.VisibilityChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBreakpointToggled(this CodeEdit target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, CodeEdit.SignalName.BreakpointToggled);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCodeCompletionRequested(this CodeEdit target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, CodeEdit.SignalName.CodeCompletionRequested);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSymbolLookup(this CodeEdit target, Action<string, long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, CodeEdit.SignalName.SymbolLookup);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>(), awaiter.GetResult()[1].As<long>(), awaiter.GetResult()[2].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSymbolValidate(this CodeEdit target, Action<string>? onComplete = null) {
        var awaiter = target.ToSignal(target, CodeEdit.SignalName.SymbolValidate);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitInputEvent(this CollisionObject2D target, Action<Node, InputEvent, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, CollisionObject2D.SignalName.InputEvent);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>(), awaiter.GetResult()[1].As<InputEvent>(), awaiter.GetResult()[2].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMouseEntered(this CollisionObject2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, CollisionObject2D.SignalName.MouseEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMouseExited(this CollisionObject2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, CollisionObject2D.SignalName.MouseExited);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMouseShapeEntered(this CollisionObject2D target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, CollisionObject2D.SignalName.MouseShapeEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMouseShapeExited(this CollisionObject2D target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, CollisionObject2D.SignalName.MouseShapeExited);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitInputEvent(this CollisionObject3D target, Action<Node, InputEvent, Vector3, Vector3, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, CollisionObject3D.SignalName.InputEvent);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>(), awaiter.GetResult()[1].As<InputEvent>(), awaiter.GetResult()[2].As<Vector3>(), awaiter.GetResult()[3].As<Vector3>(), awaiter.GetResult()[4].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMouseEntered(this CollisionObject3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, CollisionObject3D.SignalName.MouseEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMouseExited(this CollisionObject3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, CollisionObject3D.SignalName.MouseExited);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitColorChanged(this ColorPicker target, Action<Color>? onComplete = null) {
        var awaiter = target.ToSignal(target, ColorPicker.SignalName.ColorChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Color>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPresetAdded(this ColorPicker target, Action<Color>? onComplete = null) {
        var awaiter = target.ToSignal(target, ColorPicker.SignalName.PresetAdded);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Color>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPresetRemoved(this ColorPicker target, Action<Color>? onComplete = null) {
        var awaiter = target.ToSignal(target, ColorPicker.SignalName.PresetRemoved);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Color>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitColorChanged(this ColorPickerButton target, Action<Color>? onComplete = null) {
        var awaiter = target.ToSignal(target, ColorPickerButton.SignalName.ColorChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Color>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPickerCreated(this ColorPickerButton target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, ColorPickerButton.SignalName.PickerCreated);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPopupClosed(this ColorPickerButton target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, ColorPickerButton.SignalName.PopupClosed);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPreSortChildren(this Container target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Container.SignalName.PreSortChildren);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSortChildren(this Container target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Container.SignalName.SortChildren);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFocusEntered(this Control target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Control.SignalName.FocusEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFocusExited(this Control target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Control.SignalName.FocusExited);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitGuiInput(this Control target, Action<InputEvent>? onComplete = null) {
        var awaiter = target.ToSignal(target, Control.SignalName.GuiInput);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<InputEvent>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMinimumSizeChanged(this Control target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Control.SignalName.MinimumSizeChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMouseEntered(this Control target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Control.SignalName.MouseEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMouseExited(this Control target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Control.SignalName.MouseExited);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitResized(this Control target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Control.SignalName.Resized);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSizeFlagsChanged(this Control target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Control.SignalName.SizeFlagsChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitThemeChanged(this Control target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Control.SignalName.ThemeChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitRangeChanged(this Curve target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Curve.SignalName.RangeChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitDirSelected(this FileDialog target, Action<string>? onComplete = null) {
        var awaiter = target.ToSignal(target, FileDialog.SignalName.DirSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFileSelected(this FileDialog target, Action<string>? onComplete = null) {
        var awaiter = target.ToSignal(target, FileDialog.SignalName.FileSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFilesSelected(this FileDialog target, Action<string[]>? onComplete = null) {
        var awaiter = target.ToSignal(target, FileDialog.SignalName.FilesSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string[]>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitGDExtensionManagerExtensionsReloaded(Action? onComplete = null) {
        var awaiter = GDExtensionManager.Singleton.ToSignal(GDExtensionManager.Singleton, GDExtensionManager.SignalName.ExtensionsReloaded);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBeginNodeMove(this GraphEdit target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.BeginNodeMove);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitConnectionDragEnded(this GraphEdit target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.ConnectionDragEnded);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitConnectionDragStarted(this GraphEdit target, Action<StringName, long, bool>? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.ConnectionDragStarted);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>(), awaiter.GetResult()[1].As<long>(), awaiter.GetResult()[2].As<bool>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitConnectionFromEmpty(this GraphEdit target, Action<StringName, long, Vector2>? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.ConnectionFromEmpty);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>(), awaiter.GetResult()[1].As<long>(), awaiter.GetResult()[2].As<Vector2>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitConnectionRequest(this GraphEdit target, Action<StringName, long, StringName, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.ConnectionRequest);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>(), awaiter.GetResult()[1].As<long>(), awaiter.GetResult()[2].As<StringName>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitConnectionToEmpty(this GraphEdit target, Action<StringName, long, Vector2>? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.ConnectionToEmpty);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>(), awaiter.GetResult()[1].As<long>(), awaiter.GetResult()[2].As<Vector2>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCopyNodesRequest(this GraphEdit target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.CopyNodesRequest);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitDeleteNodesRequest(this GraphEdit target, Action<Array>? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.DeleteNodesRequest);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Array>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitDisconnectionRequest(this GraphEdit target, Action<StringName, long, StringName, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.DisconnectionRequest);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>(), awaiter.GetResult()[1].As<long>(), awaiter.GetResult()[2].As<StringName>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitDuplicateNodesRequest(this GraphEdit target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.DuplicateNodesRequest);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitEndNodeMove(this GraphEdit target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.EndNodeMove);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNodeDeselected(this GraphEdit target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.NodeDeselected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNodeSelected(this GraphEdit target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.NodeSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPasteNodesRequest(this GraphEdit target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.PasteNodesRequest);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPopupRequest(this GraphEdit target, Action<Vector2>? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.PopupRequest);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Vector2>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitScrollOffsetChanged(this GraphEdit target, Action<Vector2>? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphEdit.SignalName.ScrollOffsetChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Vector2>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitDeleteRequest(this GraphElement target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphElement.SignalName.DeleteRequest);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitDragged(this GraphElement target, Action<Vector2, Vector2>? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphElement.SignalName.Dragged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Vector2>(), awaiter.GetResult()[1].As<Vector2>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNodeDeselected(this GraphElement target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphElement.SignalName.NodeDeselected);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNodeSelected(this GraphElement target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphElement.SignalName.NodeSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPositionOffsetChanged(this GraphElement target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphElement.SignalName.PositionOffsetChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitRaiseRequest(this GraphElement target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphElement.SignalName.RaiseRequest);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitResizeRequest(this GraphElement target, Action<Vector2>? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphElement.SignalName.ResizeRequest);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Vector2>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSlotUpdated(this GraphNode target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, GraphNode.SignalName.SlotUpdated);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCellSizeChanged(this GridMap target, Action<Vector3>? onComplete = null) {
        var awaiter = target.ToSignal(target, GridMap.SignalName.CellSizeChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Vector3>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitChanged(this GridMap target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, GridMap.SignalName.Changed);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitInputJoyConnectionChanged(Action<long, bool>? onComplete = null) {
        var awaiter = Input.Singleton.ToSignal(Input.Singleton, Input.SignalName.JoyConnectionChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>(), awaiter.GetResult()[1].As<bool>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitEmptyClicked(this ItemList target, Action<Vector2, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, ItemList.SignalName.EmptyClicked);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Vector2>(), awaiter.GetResult()[1].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitItemActivated(this ItemList target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, ItemList.SignalName.ItemActivated);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitItemClicked(this ItemList target, Action<long, Vector2, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, ItemList.SignalName.ItemClicked);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>(), awaiter.GetResult()[1].As<Vector2>(), awaiter.GetResult()[2].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitItemSelected(this ItemList target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, ItemList.SignalName.ItemSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMultiSelected(this ItemList target, Action<long, bool>? onComplete = null) {
        var awaiter = target.ToSignal(target, ItemList.SignalName.MultiSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>(), awaiter.GetResult()[1].As<bool>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitJavaScriptBridgePwaUpdateAvailable(Action? onComplete = null) {
        var awaiter = JavaScriptBridge.Singleton.ToSignal(JavaScriptBridge.Singleton, JavaScriptBridge.SignalName.PwaUpdateAvailable);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTextChanged(this LineEdit target, Action<string>? onComplete = null) {
        var awaiter = target.ToSignal(target, LineEdit.SignalName.TextChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTextChangeRejected(this LineEdit target, Action<string>? onComplete = null) {
        var awaiter = target.ToSignal(target, LineEdit.SignalName.TextChangeRejected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTextSubmitted(this LineEdit target, Action<string>? onComplete = null) {
        var awaiter = target.ToSignal(target, LineEdit.SignalName.TextSubmitted);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitOnRequestPermissionsResult(this MainLoop target, Action<string, bool>? onComplete = null) {
        var awaiter = target.ToSignal(target, MainLoop.SignalName.OnRequestPermissionsResult);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>(), awaiter.GetResult()[1].As<bool>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAboutToPopup(this MenuButton target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, MenuButton.SignalName.AboutToPopup);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTextureChanged(this MeshInstance2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, MeshInstance2D.SignalName.TextureChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTextureChanged(this MultiMeshInstance2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, MultiMeshInstance2D.SignalName.TextureChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPeerConnected(this MultiplayerPeer target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, MultiplayerPeer.SignalName.PeerConnected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPeerDisconnected(this MultiplayerPeer target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, MultiplayerPeer.SignalName.PeerDisconnected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitDespawned(this MultiplayerSpawner target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, MultiplayerSpawner.SignalName.Despawned);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSpawned(this MultiplayerSpawner target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, MultiplayerSpawner.SignalName.Spawned);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitDeltaSynchronized(this MultiplayerSynchronizer target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, MultiplayerSynchronizer.SignalName.DeltaSynchronized);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSynchronized(this MultiplayerSynchronizer target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, MultiplayerSynchronizer.SignalName.Synchronized);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitVisibilityChanged(this MultiplayerSynchronizer target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, MultiplayerSynchronizer.SignalName.VisibilityChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitLinkReached(this NavigationAgent2D target, Action<Dictionary>? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationAgent2D.SignalName.LinkReached);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Dictionary>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNavigationFinished(this NavigationAgent2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationAgent2D.SignalName.NavigationFinished);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPathChanged(this NavigationAgent2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationAgent2D.SignalName.PathChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTargetReached(this NavigationAgent2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationAgent2D.SignalName.TargetReached);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitVelocityComputed(this NavigationAgent2D target, Action<Vector2>? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationAgent2D.SignalName.VelocityComputed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Vector2>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitWaypointReached(this NavigationAgent2D target, Action<Dictionary>? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationAgent2D.SignalName.WaypointReached);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Dictionary>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitLinkReached(this NavigationAgent3D target, Action<Dictionary>? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationAgent3D.SignalName.LinkReached);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Dictionary>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNavigationFinished(this NavigationAgent3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationAgent3D.SignalName.NavigationFinished);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPathChanged(this NavigationAgent3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationAgent3D.SignalName.PathChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTargetReached(this NavigationAgent3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationAgent3D.SignalName.TargetReached);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitVelocityComputed(this NavigationAgent3D target, Action<Vector3>? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationAgent3D.SignalName.VelocityComputed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Vector3>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitWaypointReached(this NavigationAgent3D target, Action<Dictionary>? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationAgent3D.SignalName.WaypointReached);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Dictionary>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBakeFinished(this NavigationRegion2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationRegion2D.SignalName.BakeFinished);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNavigationPolygonChanged(this NavigationRegion2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationRegion2D.SignalName.NavigationPolygonChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBakeFinished(this NavigationRegion3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationRegion3D.SignalName.BakeFinished);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNavigationMeshChanged(this NavigationRegion3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, NavigationRegion3D.SignalName.NavigationMeshChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNavigationServer2DMapChanged(Action<Rid>? onComplete = null) {
        var awaiter = NavigationServer2D.Singleton.ToSignal(NavigationServer2D.Singleton, NavigationServer2D.SignalName.MapChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Rid>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNavigationServer2DNavigationDebugChanged(Action? onComplete = null) {
        var awaiter = NavigationServer2D.Singleton.ToSignal(NavigationServer2D.Singleton, NavigationServer2D.SignalName.NavigationDebugChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNavigationServer3DAvoidanceDebugChanged(Action? onComplete = null) {
        var awaiter = NavigationServer3D.Singleton.ToSignal(NavigationServer3D.Singleton, NavigationServer3D.SignalName.AvoidanceDebugChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNavigationServer3DMapChanged(Action<Rid>? onComplete = null) {
        var awaiter = NavigationServer3D.Singleton.ToSignal(NavigationServer3D.Singleton, NavigationServer3D.SignalName.MapChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Rid>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNavigationServer3DNavigationDebugChanged(Action? onComplete = null) {
        var awaiter = NavigationServer3D.Singleton.ToSignal(NavigationServer3D.Singleton, NavigationServer3D.SignalName.NavigationDebugChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTextureChanged(this NinePatchRect target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, NinePatchRect.SignalName.TextureChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitChildEnteredTree(this Node target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, Node.SignalName.ChildEnteredTree);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitChildExitingTree(this Node target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, Node.SignalName.ChildExitingTree);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitChildOrderChanged(this Node target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Node.SignalName.ChildOrderChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitReady(this Node target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Node.SignalName.Ready);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitRenamed(this Node target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Node.SignalName.Renamed);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitReplacingBy(this Node target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, Node.SignalName.ReplacingBy);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTreeEntered(this Node target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Node.SignalName.TreeEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTreeExited(this Node target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Node.SignalName.TreeExited);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTreeExiting(this Node target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Node.SignalName.TreeExiting);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitVisibilityChanged(this Node3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Node3D.SignalName.VisibilityChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPoseRecentered(this OpenXRInterface target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, OpenXRInterface.SignalName.PoseRecentered);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSessionBegun(this OpenXRInterface target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, OpenXRInterface.SignalName.SessionBegun);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSessionFocussed(this OpenXRInterface target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, OpenXRInterface.SignalName.SessionFocussed);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSessionStopping(this OpenXRInterface target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, OpenXRInterface.SignalName.SessionStopping);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSessionVisible(this OpenXRInterface target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, OpenXRInterface.SignalName.SessionVisible);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitItemFocused(this OptionButton target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, OptionButton.SignalName.ItemFocused);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitItemSelected(this OptionButton target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, OptionButton.SignalName.ItemSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCurveChanged(this Path3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Path3D.SignalName.CurveChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPopupHide(this Popup target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Popup.SignalName.PopupHide);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitIdFocused(this PopupMenu target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, PopupMenu.SignalName.IdFocused);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitIdPressed(this PopupMenu target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, PopupMenu.SignalName.IdPressed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitIndexPressed(this PopupMenu target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, PopupMenu.SignalName.IndexPressed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMenuChanged(this PopupMenu target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, PopupMenu.SignalName.MenuChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitProjectSettingsSettingsChanged(Action? onComplete = null) {
        var awaiter = ProjectSettings.Singleton.ToSignal(ProjectSettings.Singleton, ProjectSettings.SignalName.SettingsChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitChanged(this Range target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Range.SignalName.Changed);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitValueChanged(this Range target, Action<double>? onComplete = null) {
        var awaiter = target.ToSignal(target, Range.SignalName.ValueChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<double>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitRenderingServerFramePostDraw(Action? onComplete = null) {
        var awaiter = RenderingServer.Singleton.ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitRenderingServerFramePreDraw(Action? onComplete = null) {
        var awaiter = RenderingServer.Singleton.ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePreDraw);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitChanged(this Resource target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Resource.SignalName.Changed);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSetupLocalToSceneRequested(this Resource target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Resource.SignalName.SetupLocalToSceneRequested);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFinished(this RichTextLabel target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, RichTextLabel.SignalName.Finished);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMetaClicked(this RichTextLabel target, Action<Variant>? onComplete = null) {
        var awaiter = target.ToSignal(target, RichTextLabel.SignalName.MetaClicked);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Variant>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMetaHoverEnded(this RichTextLabel target, Action<Variant>? onComplete = null) {
        var awaiter = target.ToSignal(target, RichTextLabel.SignalName.MetaHoverEnded);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Variant>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMetaHoverStarted(this RichTextLabel target, Action<Variant>? onComplete = null) {
        var awaiter = target.ToSignal(target, RichTextLabel.SignalName.MetaHoverStarted);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Variant>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyEntered(this RigidBody2D target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, RigidBody2D.SignalName.BodyEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyExited(this RigidBody2D target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, RigidBody2D.SignalName.BodyExited);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyShapeEntered(this RigidBody2D target, Action<Rid, Node, long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, RigidBody2D.SignalName.BodyShapeEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Rid>(), awaiter.GetResult()[1].As<Node>(), awaiter.GetResult()[2].As<long>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyShapeExited(this RigidBody2D target, Action<Rid, Node, long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, RigidBody2D.SignalName.BodyShapeExited);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Rid>(), awaiter.GetResult()[1].As<Node>(), awaiter.GetResult()[2].As<long>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSleepingStateChanged(this RigidBody2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, RigidBody2D.SignalName.SleepingStateChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyEntered(this RigidBody3D target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, RigidBody3D.SignalName.BodyEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyExited(this RigidBody3D target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, RigidBody3D.SignalName.BodyExited);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyShapeEntered(this RigidBody3D target, Action<Rid, Node, long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, RigidBody3D.SignalName.BodyShapeEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Rid>(), awaiter.GetResult()[1].As<Node>(), awaiter.GetResult()[2].As<long>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBodyShapeExited(this RigidBody3D target, Action<Rid, Node, long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, RigidBody3D.SignalName.BodyShapeExited);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Rid>(), awaiter.GetResult()[1].As<Node>(), awaiter.GetResult()[2].As<long>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSleepingStateChanged(this RigidBody3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, RigidBody3D.SignalName.SleepingStateChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPeerAuthenticating(this SceneMultiplayer target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, SceneMultiplayer.SignalName.PeerAuthenticating);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPeerAuthenticationFailed(this SceneMultiplayer target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, SceneMultiplayer.SignalName.PeerAuthenticationFailed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPeerPacket(this SceneMultiplayer target, Action<long, byte[]>? onComplete = null) {
        var awaiter = target.ToSignal(target, SceneMultiplayer.SignalName.PeerPacket);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>(), awaiter.GetResult()[1].As<byte[]>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNodeAdded(this SceneTree target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, SceneTree.SignalName.NodeAdded);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNodeConfigurationWarningChanged(this SceneTree target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, SceneTree.SignalName.NodeConfigurationWarningChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNodeRemoved(this SceneTree target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, SceneTree.SignalName.NodeRemoved);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNodeRenamed(this SceneTree target, Action<Node>? onComplete = null) {
        var awaiter = target.ToSignal(target, SceneTree.SignalName.NodeRenamed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Node>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPhysicsFrame(this SceneTree target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, SceneTree.SignalName.PhysicsFrame);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitProcessFrame(this SceneTree target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, SceneTree.SignalName.ProcessFrame);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTreeChanged(this SceneTree target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, SceneTree.SignalName.TreeChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTreeProcessModeChanged(this SceneTree target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, SceneTree.SignalName.TreeProcessModeChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTimeout(this SceneTreeTimer target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, SceneTreeTimer.SignalName.Timeout);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitScrolling(this ScrollBar target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, ScrollBar.SignalName.Scrolling);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitScrollEnded(this ScrollContainer target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, ScrollContainer.SignalName.ScrollEnded);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitScrollStarted(this ScrollContainer target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, ScrollContainer.SignalName.ScrollStarted);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBoneSetupChanged(this Skeleton2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Skeleton2D.SignalName.BoneSetupChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBoneEnabledChanged(this Skeleton3D target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Skeleton3D.SignalName.BoneEnabledChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitBonePoseChanged(this Skeleton3D target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Skeleton3D.SignalName.BonePoseChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPoseUpdated(this Skeleton3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Skeleton3D.SignalName.PoseUpdated);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitShowRestOnlyChanged(this Skeleton3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Skeleton3D.SignalName.ShowRestOnlyChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitProfileUpdated(this SkeletonProfile target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, SkeletonProfile.SignalName.ProfileUpdated);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitDragEnded(this Slider target, Action<bool>? onComplete = null) {
        var awaiter = target.ToSignal(target, Slider.SignalName.DragEnded);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<bool>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitDragStarted(this Slider target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Slider.SignalName.DragStarted);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitDragged(this SplitContainer target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, SplitContainer.SignalName.Dragged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFrameChanged(this Sprite2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Sprite2D.SignalName.FrameChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTextureChanged(this Sprite2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Sprite2D.SignalName.TextureChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFrameChanged(this Sprite3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Sprite3D.SignalName.FrameChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTextureChanged(this Sprite3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Sprite3D.SignalName.TextureChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitActiveTabRearranged(this TabBar target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TabBar.SignalName.ActiveTabRearranged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTabButtonPressed(this TabBar target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TabBar.SignalName.TabButtonPressed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTabChanged(this TabBar target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TabBar.SignalName.TabChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTabClicked(this TabBar target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TabBar.SignalName.TabClicked);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTabClosePressed(this TabBar target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TabBar.SignalName.TabClosePressed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTabHovered(this TabBar target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TabBar.SignalName.TabHovered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTabRmbClicked(this TabBar target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TabBar.SignalName.TabRmbClicked);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTabSelected(this TabBar target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TabBar.SignalName.TabSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitActiveTabRearranged(this TabContainer target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TabContainer.SignalName.ActiveTabRearranged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPrePopupPressed(this TabContainer target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, TabContainer.SignalName.PrePopupPressed);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTabButtonPressed(this TabContainer target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TabContainer.SignalName.TabButtonPressed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTabChanged(this TabContainer target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TabContainer.SignalName.TabChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTabClicked(this TabContainer target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TabContainer.SignalName.TabClicked);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTabHovered(this TabContainer target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TabContainer.SignalName.TabHovered);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTabSelected(this TabContainer target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TabContainer.SignalName.TabSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCaretChanged(this TextEdit target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, TextEdit.SignalName.CaretChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitGutterAdded(this TextEdit target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, TextEdit.SignalName.GutterAdded);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitGutterClicked(this TextEdit target, Action<long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TextEdit.SignalName.GutterClicked);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>(), awaiter.GetResult()[1].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitGutterRemoved(this TextEdit target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, TextEdit.SignalName.GutterRemoved);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitLinesEditedFrom(this TextEdit target, Action<long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, TextEdit.SignalName.LinesEditedFrom);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>(), awaiter.GetResult()[1].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTextChanged(this TextEdit target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, TextEdit.SignalName.TextChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTextSet(this TextEdit target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, TextEdit.SignalName.TextSet);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTextServerManagerInterfaceAdded(Action<StringName>? onComplete = null) {
        var awaiter = TextServerManager.Singleton.ToSignal(TextServerManager.Singleton, TextServerManager.SignalName.InterfaceAdded);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTextServerManagerInterfaceRemoved(Action<StringName>? onComplete = null) {
        var awaiter = TextServerManager.Singleton.ToSignal(TextServerManager.Singleton, TextServerManager.SignalName.InterfaceRemoved);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitThemeDBFallbackChanged(Action? onComplete = null) {
        var awaiter = ThemeDB.Singleton.ToSignal(ThemeDB.Singleton, ThemeDB.SignalName.FallbackChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitChanged(this TileData target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, TileData.SignalName.Changed);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitChanged(this TileMap target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, TileMap.SignalName.Changed);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTimeout(this Timer target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Timer.SignalName.Timeout);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPressed(this TouchScreenButton target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, TouchScreenButton.SignalName.Pressed);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitReleased(this TouchScreenButton target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, TouchScreenButton.SignalName.Released);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitButtonClicked(this Tree target, Action<TreeItem, long, long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.ButtonClicked);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<TreeItem>(), awaiter.GetResult()[1].As<long>(), awaiter.GetResult()[2].As<long>(), awaiter.GetResult()[3].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCellSelected(this Tree target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.CellSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCheckPropagatedToItem(this Tree target, Action<TreeItem, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.CheckPropagatedToItem);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<TreeItem>(), awaiter.GetResult()[1].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitColumnTitleClicked(this Tree target, Action<long, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.ColumnTitleClicked);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>(), awaiter.GetResult()[1].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCustomItemClicked(this Tree target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.CustomItemClicked);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCustomPopupEdited(this Tree target, Action<bool>? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.CustomPopupEdited);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<bool>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitEmptyClicked(this Tree target, Action<Vector2, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.EmptyClicked);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Vector2>(), awaiter.GetResult()[1].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitItemActivated(this Tree target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.ItemActivated);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitItemCollapsed(this Tree target, Action<TreeItem>? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.ItemCollapsed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<TreeItem>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitItemEdited(this Tree target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.ItemEdited);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitItemIconDoubleClicked(this Tree target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.ItemIconDoubleClicked);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitItemMouseSelected(this Tree target, Action<Vector2, long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.ItemMouseSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Vector2>(), awaiter.GetResult()[1].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitItemSelected(this Tree target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.ItemSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMultiSelected(this Tree target, Action<TreeItem, long, bool>? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.MultiSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<TreeItem>(), awaiter.GetResult()[1].As<long>(), awaiter.GetResult()[2].As<bool>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitNothingSelected(this Tree target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Tree.SignalName.NothingSelected);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFinished(this Tween target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Tween.SignalName.Finished);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitLoopFinished(this Tween target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Tween.SignalName.LoopFinished);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitStepFinished(this Tween target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, Tween.SignalName.StepFinished);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFinished(this Tweener target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Tweener.SignalName.Finished);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitVersionChanged(this UndoRedo target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, UndoRedo.SignalName.VersionChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFinished(this VideoStreamPlayer target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, VideoStreamPlayer.SignalName.Finished);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitGuiFocusChanged(this Viewport target, Action<Control>? onComplete = null) {
        var awaiter = target.ToSignal(target, Viewport.SignalName.GuiFocusChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<Control>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSizeChanged(this Viewport target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Viewport.SignalName.SizeChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitScreenEntered(this VisibleOnScreenNotifier2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, VisibleOnScreenNotifier2D.SignalName.ScreenEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitScreenExited(this VisibleOnScreenNotifier2D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, VisibleOnScreenNotifier2D.SignalName.ScreenExited);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitScreenEntered(this VisibleOnScreenNotifier3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, VisibleOnScreenNotifier3D.SignalName.ScreenEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitScreenExited(this VisibleOnScreenNotifier3D target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, VisibleOnScreenNotifier3D.SignalName.ScreenExited);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitInputTypeChanged(this VisualShaderNodeInput target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, VisualShaderNodeInput.SignalName.InputTypeChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitDisplayRefreshRateChanged(this WebXRInterface target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, WebXRInterface.SignalName.DisplayRefreshRateChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitReferenceSpaceReset(this WebXRInterface target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, WebXRInterface.SignalName.ReferenceSpaceReset);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSelect(this WebXRInterface target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, WebXRInterface.SignalName.Select);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSelectend(this WebXRInterface target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, WebXRInterface.SignalName.Selectend);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSelectstart(this WebXRInterface target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, WebXRInterface.SignalName.Selectstart);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSessionEnded(this WebXRInterface target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, WebXRInterface.SignalName.SessionEnded);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSessionFailed(this WebXRInterface target, Action<string>? onComplete = null) {
        var awaiter = target.ToSignal(target, WebXRInterface.SignalName.SessionFailed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSessionStarted(this WebXRInterface target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, WebXRInterface.SignalName.SessionStarted);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSessionSupported(this WebXRInterface target, Action<string, bool>? onComplete = null) {
        var awaiter = target.ToSignal(target, WebXRInterface.SignalName.SessionSupported);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>(), awaiter.GetResult()[1].As<bool>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSqueeze(this WebXRInterface target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, WebXRInterface.SignalName.Squeeze);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSqueezeend(this WebXRInterface target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, WebXRInterface.SignalName.Squeezeend);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitSqueezestart(this WebXRInterface target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, WebXRInterface.SignalName.Squeezestart);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitVisibilityStateChanged(this WebXRInterface target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, WebXRInterface.SignalName.VisibilityStateChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitAboutToPopup(this Window target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Window.SignalName.AboutToPopup);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitCloseRequested(this Window target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Window.SignalName.CloseRequested);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitDpiChanged(this Window target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Window.SignalName.DpiChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFilesDropped(this Window target, Action<string[]>? onComplete = null) {
        var awaiter = target.ToSignal(target, Window.SignalName.FilesDropped);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string[]>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFocusEntered(this Window target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Window.SignalName.FocusEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitFocusExited(this Window target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Window.SignalName.FocusExited);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitGoBackRequested(this Window target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Window.SignalName.GoBackRequested);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMouseEntered(this Window target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Window.SignalName.MouseEntered);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitMouseExited(this Window target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Window.SignalName.MouseExited);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitThemeChanged(this Window target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Window.SignalName.ThemeChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTitlebarChanged(this Window target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Window.SignalName.TitlebarChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitVisibilityChanged(this Window target, Action? onComplete = null) {
        var awaiter = target.ToSignal(target, Window.SignalName.VisibilityChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(onComplete);
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitWindowInput(this Window target, Action<InputEvent>? onComplete = null) {
        var awaiter = target.ToSignal(target, Window.SignalName.WindowInput);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<InputEvent>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitButtonPressed(this XRController3D target, Action<string>? onComplete = null) {
        var awaiter = target.ToSignal(target, XRController3D.SignalName.ButtonPressed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitButtonReleased(this XRController3D target, Action<string>? onComplete = null) {
        var awaiter = target.ToSignal(target, XRController3D.SignalName.ButtonReleased);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitInputFloatChanged(this XRController3D target, Action<string, double>? onComplete = null) {
        var awaiter = target.ToSignal(target, XRController3D.SignalName.InputFloatChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>(), awaiter.GetResult()[1].As<double>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitInputVector2Changed(this XRController3D target, Action<string, Vector2>? onComplete = null) {
        var awaiter = target.ToSignal(target, XRController3D.SignalName.InputVector2Changed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>(), awaiter.GetResult()[1].As<Vector2>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPlayAreaChanged(this XRInterface target, Action<long>? onComplete = null) {
        var awaiter = target.ToSignal(target, XRInterface.SignalName.PlayAreaChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitTrackingChanged(this XRNode3D target, Action<bool>? onComplete = null) {
        var awaiter = target.ToSignal(target, XRNode3D.SignalName.TrackingChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<bool>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitButtonPressed(this XRPositionalTracker target, Action<string>? onComplete = null) {
        var awaiter = target.ToSignal(target, XRPositionalTracker.SignalName.ButtonPressed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitButtonReleased(this XRPositionalTracker target, Action<string>? onComplete = null) {
        var awaiter = target.ToSignal(target, XRPositionalTracker.SignalName.ButtonReleased);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitInputFloatChanged(this XRPositionalTracker target, Action<string, double>? onComplete = null) {
        var awaiter = target.ToSignal(target, XRPositionalTracker.SignalName.InputFloatChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>(), awaiter.GetResult()[1].As<double>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitInputVector2Changed(this XRPositionalTracker target, Action<string, Vector2>? onComplete = null) {
        var awaiter = target.ToSignal(target, XRPositionalTracker.SignalName.InputVector2Changed);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>(), awaiter.GetResult()[1].As<Vector2>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPoseChanged(this XRPositionalTracker target, Action<XRPose>? onComplete = null) {
        var awaiter = target.ToSignal(target, XRPositionalTracker.SignalName.PoseChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<XRPose>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitPoseLostTracking(this XRPositionalTracker target, Action<XRPose>? onComplete = null) {
        var awaiter = target.ToSignal(target, XRPositionalTracker.SignalName.PoseLostTracking);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<XRPose>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitProfileChanged(this XRPositionalTracker target, Action<string>? onComplete = null) {
        var awaiter = target.ToSignal(target, XRPositionalTracker.SignalName.ProfileChanged);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<string>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitXRServerInterfaceAdded(Action<StringName>? onComplete = null) {
        var awaiter = XRServer.Singleton.ToSignal(XRServer.Singleton, XRServer.SignalName.InterfaceAdded);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitXRServerInterfaceRemoved(Action<StringName>? onComplete = null) {
        var awaiter = XRServer.Singleton.ToSignal(XRServer.Singleton, XRServer.SignalName.InterfaceRemoved);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitXRServerTrackerAdded(Action<StringName, long>? onComplete = null) {
        var awaiter = XRServer.Singleton.ToSignal(XRServer.Singleton, XRServer.SignalName.TrackerAdded);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>(), awaiter.GetResult()[1].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitXRServerTrackerRemoved(Action<StringName, long>? onComplete = null) {
        var awaiter = XRServer.Singleton.ToSignal(XRServer.Singleton, XRServer.SignalName.TrackerRemoved);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>(), awaiter.GetResult()[1].As<long>()));
        }
        return awaiter;
    }

    public static SignalAwaiter AwaitXRServerTrackerUpdated(Action<StringName, long>? onComplete = null) {
        var awaiter = XRServer.Singleton.ToSignal(XRServer.Singleton, XRServer.SignalName.TrackerUpdated);
        if (onComplete != null) {
            awaiter.OnCompleted(() => onComplete(awaiter.GetResult()[0].As<StringName>(), awaiter.GetResult()[1].As<long>()));
        }
        return awaiter;
    }
}