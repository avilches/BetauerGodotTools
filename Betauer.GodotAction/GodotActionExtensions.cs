using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;
using Environment = Godot.Environment;
using Betauer.GodotAction;

namespace Betauer {
    public static partial class GodotActionExtensions {

        public static AcceptDialog OnAboutToShow(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnAboutToShow(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnConfirmed(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnConfirmed(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnCustomAction(this AcceptDialog target, Action<string> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnCustomAction(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnDraw(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnFocusEntered(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnFocusExited(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnGuiInput(this AcceptDialog target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnHide(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnItemRectChanged(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnMinimumSizeChanged(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnModalClosed(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnMouseEntered(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnMouseExited(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnPopupHide(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnPopupHide(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnReady(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnRenamed(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnResized(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnScriptChanged(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnSizeFlagsChanged(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnTreeEntered(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnTreeExited(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnTreeExiting(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static AcceptDialog OnVisibilityChanged(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AcceptDialogAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite OnAnimationFinished(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnAnimationFinished(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite OnDraw(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite OnFrameChanged(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnFrameChanged(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite OnHide(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite OnItemRectChanged(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite OnReady(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite OnRenamed(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite OnScriptChanged(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite OnTreeEntered(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite OnTreeExited(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite OnTreeExiting(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite OnVisibilityChanged(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite3D OnAnimationFinished(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnAnimationFinished(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite3D OnFrameChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnFrameChanged(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite3D OnGameplayEntered(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite3D OnGameplayExited(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite3D OnReady(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite3D OnRenamed(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite3D OnScriptChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite3D OnTreeEntered(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite3D OnTreeExited(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite3D OnTreeExiting(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static AnimatedSprite3D OnVisibilityChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static AnimationPlayer OnAnimationChanged(this AnimationPlayer target, Action<string, string> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnAnimationChanged(action, oneShot, deferred);
            return target;
        }

        public static AnimationPlayer OnAnimationFinished(this AnimationPlayer target, Action<string> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnAnimationFinished(action, oneShot, deferred);
            return target;
        }

        public static AnimationPlayer OnAnimationStarted(this AnimationPlayer target, Action<string> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnAnimationStarted(action, oneShot, deferred);
            return target;
        }

        public static AnimationPlayer OnCachesCleared(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnCachesCleared(action, oneShot, deferred);
            return target;
        }

        public static AnimationPlayer OnReady(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static AnimationPlayer OnRenamed(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static AnimationPlayer OnScriptChanged(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static AnimationPlayer OnTreeEntered(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static AnimationPlayer OnTreeExited(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static AnimationPlayer OnTreeExiting(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static AnimationTree OnReady(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationTreeAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static AnimationTree OnRenamed(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationTreeAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static AnimationTree OnScriptChanged(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationTreeAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static AnimationTree OnTreeEntered(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationTreeAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static AnimationTree OnTreeExited(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationTreeAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static AnimationTree OnTreeExiting(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationTreeAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static AnimationTreePlayer OnReady(this AnimationTreePlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationTreePlayerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static AnimationTreePlayer OnRenamed(this AnimationTreePlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationTreePlayerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static AnimationTreePlayer OnScriptChanged(this AnimationTreePlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationTreePlayerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static AnimationTreePlayer OnTreeEntered(this AnimationTreePlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationTreePlayerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static AnimationTreePlayer OnTreeExited(this AnimationTreePlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationTreePlayerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static AnimationTreePlayer OnTreeExiting(this AnimationTreePlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AnimationTreePlayerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Area OnAreaEntered(this Area target, Action<Area> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnAreaEntered(action, oneShot, deferred);
            return target;
        }

        public static Area OnAreaExited(this Area target, Action<Area> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnAreaExited(action, oneShot, deferred);
            return target;
        }

        public static Area OnAreaShapeEntered(this Area target, Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnAreaShapeEntered(action, oneShot, deferred);
            return target;
        }

        public static Area OnAreaShapeExited(this Area target, Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnAreaShapeExited(action, oneShot, deferred);
            return target;
        }

        public static Area OnBodyEntered(this Area target, Action<Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnBodyEntered(action, oneShot, deferred);
            return target;
        }

        public static Area OnBodyExited(this Area target, Action<Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnBodyExited(action, oneShot, deferred);
            return target;
        }

        public static Area OnBodyShapeEntered(this Area target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnBodyShapeEntered(action, oneShot, deferred);
            return target;
        }

        public static Area OnBodyShapeExited(this Area target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnBodyShapeExited(action, oneShot, deferred);
            return target;
        }

        public static Area OnGameplayEntered(this Area target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static Area OnGameplayExited(this Area target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static Area OnInputEvent(this Area target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnInputEvent(action, oneShot, deferred);
            return target;
        }

        public static Area OnMouseEntered(this Area target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static Area OnMouseExited(this Area target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static Area OnReady(this Area target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Area OnRenamed(this Area target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Area OnScriptChanged(this Area target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Area OnTreeEntered(this Area target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Area OnTreeExited(this Area target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Area OnTreeExiting(this Area target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Area OnVisibilityChanged(this Area target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AreaAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnAreaEntered(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnAreaEntered(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnAreaExited(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnAreaExited(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnAreaShapeEntered(this Area2D target, Action<Area2D, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnAreaShapeEntered(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnAreaShapeExited(this Area2D target, Action<Area2D, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnAreaShapeExited(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnBodyEntered(this Area2D target, Action<Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnBodyEntered(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnBodyExited(this Area2D target, Action<Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnBodyExited(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnBodyShapeEntered(this Area2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnBodyShapeEntered(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnBodyShapeExited(this Area2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnBodyShapeExited(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnDraw(this Area2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnHide(this Area2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnInputEvent(this Area2D target, Action<InputEvent, int, Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnInputEvent(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnItemRectChanged(this Area2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnMouseEntered(this Area2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnMouseExited(this Area2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnReady(this Area2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnRenamed(this Area2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnScriptChanged(this Area2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnTreeEntered(this Area2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnTreeExited(this Area2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnTreeExiting(this Area2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Area2D OnVisibilityChanged(this Area2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Area2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ARVRAnchor OnGameplayEntered(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static ARVRAnchor OnGameplayExited(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static ARVRAnchor OnMeshUpdated(this ARVRAnchor target, Action<Mesh> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnMeshUpdated(action, oneShot, deferred);
            return target;
        }

        public static ARVRAnchor OnReady(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ARVRAnchor OnRenamed(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ARVRAnchor OnScriptChanged(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ARVRAnchor OnTreeEntered(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ARVRAnchor OnTreeExited(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ARVRAnchor OnTreeExiting(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ARVRAnchor OnVisibilityChanged(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ARVRCamera OnGameplayEntered(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRCameraAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static ARVRCamera OnGameplayExited(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRCameraAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static ARVRCamera OnReady(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRCameraAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ARVRCamera OnRenamed(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRCameraAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ARVRCamera OnScriptChanged(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRCameraAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ARVRCamera OnTreeEntered(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRCameraAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ARVRCamera OnTreeExited(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRCameraAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ARVRCamera OnTreeExiting(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRCameraAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ARVRCamera OnVisibilityChanged(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRCameraAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ARVRController OnButtonPressed(this ARVRController target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRControllerAction>(target).OnButtonPressed(action, oneShot, deferred);
            return target;
        }

        public static ARVRController OnButtonRelease(this ARVRController target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRControllerAction>(target).OnButtonRelease(action, oneShot, deferred);
            return target;
        }

        public static ARVRController OnGameplayEntered(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRControllerAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static ARVRController OnGameplayExited(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRControllerAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static ARVRController OnMeshUpdated(this ARVRController target, Action<Mesh> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRControllerAction>(target).OnMeshUpdated(action, oneShot, deferred);
            return target;
        }

        public static ARVRController OnReady(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRControllerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ARVRController OnRenamed(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRControllerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ARVRController OnScriptChanged(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRControllerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ARVRController OnTreeEntered(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRControllerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ARVRController OnTreeExited(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRControllerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ARVRController OnTreeExiting(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRControllerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ARVRController OnVisibilityChanged(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVRControllerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ARVROrigin OnGameplayEntered(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVROriginAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static ARVROrigin OnGameplayExited(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVROriginAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static ARVROrigin OnReady(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVROriginAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ARVROrigin OnRenamed(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVROriginAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ARVROrigin OnScriptChanged(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVROriginAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ARVROrigin OnTreeEntered(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVROriginAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ARVROrigin OnTreeExited(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVROriginAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ARVROrigin OnTreeExiting(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVROriginAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ARVROrigin OnVisibilityChanged(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ARVROriginAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnDraw(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnFocusEntered(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnFocusExited(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnGuiInput(this AspectRatioContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnHide(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnItemRectChanged(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnMinimumSizeChanged(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnModalClosed(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnMouseEntered(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnMouseExited(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnReady(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnRenamed(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnResized(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnScriptChanged(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnSizeFlagsChanged(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnSortChildren(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnTreeEntered(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnTreeExited(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnTreeExiting(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static AspectRatioContainer OnVisibilityChanged(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer OnFinished(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayerAction>(target).OnFinished(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer OnReady(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer OnRenamed(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer OnScriptChanged(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer OnTreeEntered(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer OnTreeExited(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer OnTreeExiting(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer2D OnDraw(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer2D OnFinished(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnFinished(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer2D OnHide(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer2D OnItemRectChanged(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer2D OnReady(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer2D OnRenamed(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer2D OnScriptChanged(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer2D OnTreeEntered(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer2D OnTreeExited(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer2D OnTreeExiting(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer2D OnVisibilityChanged(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer3D OnFinished(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnFinished(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer3D OnGameplayEntered(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer3D OnGameplayExited(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer3D OnReady(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer3D OnRenamed(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer3D OnScriptChanged(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer3D OnTreeEntered(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer3D OnTreeExited(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer3D OnTreeExiting(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static AudioStreamPlayer3D OnVisibilityChanged(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static BackBufferCopy OnDraw(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static BackBufferCopy OnHide(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static BackBufferCopy OnItemRectChanged(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static BackBufferCopy OnReady(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static BackBufferCopy OnRenamed(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static BackBufferCopy OnScriptChanged(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static BackBufferCopy OnTreeEntered(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static BackBufferCopy OnTreeExited(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static BackBufferCopy OnTreeExiting(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static BackBufferCopy OnVisibilityChanged(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static BakedLightmap OnGameplayEntered(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BakedLightmapAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static BakedLightmap OnGameplayExited(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BakedLightmapAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static BakedLightmap OnReady(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BakedLightmapAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static BakedLightmap OnRenamed(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BakedLightmapAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static BakedLightmap OnScriptChanged(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BakedLightmapAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static BakedLightmap OnTreeEntered(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BakedLightmapAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static BakedLightmap OnTreeExited(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BakedLightmapAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static BakedLightmap OnTreeExiting(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BakedLightmapAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static BakedLightmap OnVisibilityChanged(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BakedLightmapAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Bone2D OnDraw(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Bone2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Bone2D OnHide(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Bone2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Bone2D OnItemRectChanged(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Bone2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Bone2D OnReady(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Bone2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Bone2D OnRenamed(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Bone2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Bone2D OnScriptChanged(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Bone2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Bone2D OnTreeEntered(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Bone2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Bone2D OnTreeExited(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Bone2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Bone2D OnTreeExiting(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Bone2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Bone2D OnVisibilityChanged(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Bone2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static BoneAttachment OnGameplayEntered(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static BoneAttachment OnGameplayExited(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static BoneAttachment OnReady(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static BoneAttachment OnRenamed(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static BoneAttachment OnScriptChanged(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static BoneAttachment OnTreeEntered(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static BoneAttachment OnTreeExited(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static BoneAttachment OnTreeExiting(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static BoneAttachment OnVisibilityChanged(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Button OnButtonDown(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnButtonDown(action, oneShot, deferred);
            return target;
        }

        public static Button OnButtonUp(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnButtonUp(action, oneShot, deferred);
            return target;
        }

        public static Button OnDraw(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Button OnFocusEntered(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static Button OnFocusExited(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static Button OnGuiInput(this Button target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static Button OnHide(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Button OnItemRectChanged(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Button OnMinimumSizeChanged(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static Button OnModalClosed(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static Button OnMouseEntered(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static Button OnMouseExited(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static Button OnPressed(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnPressed(action, oneShot, deferred);
            return target;
        }

        public static Button OnReady(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Button OnRenamed(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Button OnResized(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static Button OnScriptChanged(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Button OnSizeFlagsChanged(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static Button OnToggled(this Button target, Action<bool> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnToggled(action, oneShot, deferred);
            return target;
        }

        public static Button OnTreeEntered(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Button OnTreeExited(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Button OnTreeExiting(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Button OnVisibilityChanged(this Button target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Camera OnGameplayEntered(this Camera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CameraAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static Camera OnGameplayExited(this Camera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CameraAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static Camera OnReady(this Camera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CameraAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Camera OnRenamed(this Camera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CameraAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Camera OnScriptChanged(this Camera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CameraAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Camera OnTreeEntered(this Camera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CameraAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Camera OnTreeExited(this Camera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CameraAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Camera OnTreeExiting(this Camera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CameraAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Camera OnVisibilityChanged(this Camera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CameraAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Camera2D OnDraw(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Camera2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Camera2D OnHide(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Camera2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Camera2D OnItemRectChanged(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Camera2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Camera2D OnReady(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Camera2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Camera2D OnRenamed(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Camera2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Camera2D OnScriptChanged(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Camera2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Camera2D OnTreeEntered(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Camera2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Camera2D OnTreeExited(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Camera2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Camera2D OnTreeExiting(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Camera2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Camera2D OnVisibilityChanged(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Camera2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CanvasLayer OnReady(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasLayerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CanvasLayer OnRenamed(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasLayerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CanvasLayer OnScriptChanged(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasLayerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CanvasLayer OnTreeEntered(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasLayerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CanvasLayer OnTreeExited(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasLayerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CanvasLayer OnTreeExiting(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasLayerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CanvasModulate OnDraw(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasModulateAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static CanvasModulate OnHide(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasModulateAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static CanvasModulate OnItemRectChanged(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasModulateAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static CanvasModulate OnReady(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasModulateAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CanvasModulate OnRenamed(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasModulateAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CanvasModulate OnScriptChanged(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasModulateAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CanvasModulate OnTreeEntered(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasModulateAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CanvasModulate OnTreeExited(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasModulateAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CanvasModulate OnTreeExiting(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasModulateAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CanvasModulate OnVisibilityChanged(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CanvasModulateAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnDraw(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnFocusEntered(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnFocusExited(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnGuiInput(this CenterContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnHide(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnItemRectChanged(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnMinimumSizeChanged(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnModalClosed(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnMouseEntered(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnMouseExited(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnReady(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnRenamed(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnResized(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnScriptChanged(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnSizeFlagsChanged(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnSortChildren(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnTreeEntered(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnTreeExited(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnTreeExiting(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CenterContainer OnVisibilityChanged(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CenterContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnButtonDown(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnButtonDown(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnButtonUp(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnButtonUp(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnDraw(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnFocusEntered(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnFocusExited(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnGuiInput(this CheckBox target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnHide(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnItemRectChanged(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnMinimumSizeChanged(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnModalClosed(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnMouseEntered(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnMouseExited(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnPressed(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnPressed(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnReady(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnRenamed(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnResized(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnScriptChanged(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnSizeFlagsChanged(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnToggled(this CheckBox target, Action<bool> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnToggled(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnTreeEntered(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnTreeExited(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnTreeExiting(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CheckBox OnVisibilityChanged(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckBoxAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnButtonDown(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnButtonDown(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnButtonUp(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnButtonUp(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnDraw(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnFocusEntered(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnFocusExited(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnGuiInput(this CheckButton target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnHide(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnItemRectChanged(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnMinimumSizeChanged(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnModalClosed(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnMouseEntered(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnMouseExited(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnPressed(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnPressed(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnReady(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnRenamed(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnResized(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnScriptChanged(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnSizeFlagsChanged(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnToggled(this CheckButton target, Action<bool> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnToggled(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnTreeEntered(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnTreeExited(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnTreeExiting(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CheckButton OnVisibilityChanged(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CheckButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ClippedCamera OnGameplayEntered(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ClippedCameraAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static ClippedCamera OnGameplayExited(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ClippedCameraAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static ClippedCamera OnReady(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ClippedCameraAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ClippedCamera OnRenamed(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ClippedCameraAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ClippedCamera OnScriptChanged(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ClippedCameraAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ClippedCamera OnTreeEntered(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ClippedCameraAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ClippedCamera OnTreeExited(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ClippedCameraAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ClippedCamera OnTreeExiting(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ClippedCameraAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ClippedCamera OnVisibilityChanged(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ClippedCameraAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon OnGameplayEntered(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon OnGameplayExited(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon OnReady(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon OnRenamed(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon OnScriptChanged(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon OnTreeEntered(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon OnTreeExited(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon OnTreeExiting(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon OnVisibilityChanged(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon2D OnDraw(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon2D OnHide(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon2D OnItemRectChanged(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon2D OnReady(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon2D OnRenamed(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon2D OnScriptChanged(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon2D OnTreeEntered(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon2D OnTreeExited(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon2D OnTreeExiting(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CollisionPolygon2D OnVisibilityChanged(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape OnGameplayEntered(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShapeAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape OnGameplayExited(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShapeAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape OnReady(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShapeAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape OnRenamed(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShapeAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape OnScriptChanged(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShapeAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape OnTreeEntered(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShapeAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape OnTreeExited(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShapeAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape OnTreeExiting(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShapeAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape OnVisibilityChanged(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShapeAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape2D OnDraw(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape2D OnHide(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape2D OnItemRectChanged(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape2D OnReady(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape2D OnRenamed(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape2D OnScriptChanged(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape2D OnTreeEntered(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape2D OnTreeExited(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape2D OnTreeExiting(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CollisionShape2D OnVisibilityChanged(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnColorChanged(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnColorChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnDraw(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnFocusEntered(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnFocusExited(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnGuiInput(this ColorPicker target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnHide(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnItemRectChanged(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnMinimumSizeChanged(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnModalClosed(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnMouseEntered(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnMouseExited(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnPresetAdded(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnPresetAdded(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnPresetRemoved(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnPresetRemoved(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnReady(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnRenamed(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnResized(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnScriptChanged(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnSizeFlagsChanged(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnSortChildren(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnTreeEntered(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnTreeExited(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnTreeExiting(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ColorPicker OnVisibilityChanged(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnButtonDown(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnButtonDown(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnButtonUp(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnButtonUp(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnColorChanged(this ColorPickerButton target, Action<Color> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnColorChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnDraw(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnFocusEntered(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnFocusExited(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnGuiInput(this ColorPickerButton target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnHide(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnItemRectChanged(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnMinimumSizeChanged(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnModalClosed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnMouseEntered(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnMouseExited(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnPickerCreated(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnPickerCreated(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnPopupClosed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnPopupClosed(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnPressed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnPressed(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnReady(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnRenamed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnResized(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnScriptChanged(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnSizeFlagsChanged(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnToggled(this ColorPickerButton target, Action<bool> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnToggled(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnTreeEntered(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnTreeExited(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnTreeExiting(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ColorPickerButton OnVisibilityChanged(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnDraw(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnFocusEntered(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnFocusExited(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnGuiInput(this ColorRect target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnHide(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnItemRectChanged(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnMinimumSizeChanged(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnModalClosed(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnMouseEntered(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnMouseExited(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnReady(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnRenamed(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnResized(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnScriptChanged(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnSizeFlagsChanged(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnTreeEntered(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnTreeExited(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnTreeExiting(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ColorRect OnVisibilityChanged(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ColorRectAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ConeTwistJoint OnGameplayEntered(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static ConeTwistJoint OnGameplayExited(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static ConeTwistJoint OnReady(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ConeTwistJoint OnRenamed(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ConeTwistJoint OnScriptChanged(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ConeTwistJoint OnTreeEntered(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ConeTwistJoint OnTreeExited(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ConeTwistJoint OnTreeExiting(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ConeTwistJoint OnVisibilityChanged(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnAboutToShow(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnAboutToShow(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnConfirmed(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnConfirmed(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnCustomAction(this ConfirmationDialog target, Action<string> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnCustomAction(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnDraw(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnFocusEntered(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnFocusExited(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnGuiInput(this ConfirmationDialog target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnHide(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnItemRectChanged(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnMinimumSizeChanged(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnModalClosed(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnMouseEntered(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnMouseExited(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnPopupHide(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnPopupHide(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnReady(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnRenamed(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnResized(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnScriptChanged(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnSizeFlagsChanged(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnTreeEntered(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnTreeExited(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnTreeExiting(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ConfirmationDialog OnVisibilityChanged(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Container OnDraw(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Container OnFocusEntered(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static Container OnFocusExited(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static Container OnGuiInput(this Container target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static Container OnHide(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Container OnItemRectChanged(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Container OnMinimumSizeChanged(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static Container OnModalClosed(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static Container OnMouseEntered(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static Container OnMouseExited(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static Container OnReady(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Container OnRenamed(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Container OnResized(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static Container OnScriptChanged(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Container OnSizeFlagsChanged(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static Container OnSortChildren(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static Container OnTreeEntered(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Container OnTreeExited(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Container OnTreeExiting(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Container OnVisibilityChanged(this Container target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Control OnDraw(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Control OnFocusEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static Control OnFocusExited(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static Control OnGuiInput(this Control target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static Control OnHide(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Control OnItemRectChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Control OnMinimumSizeChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static Control OnModalClosed(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static Control OnMouseEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static Control OnMouseExited(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static Control OnReady(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Control OnRenamed(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Control OnResized(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static Control OnScriptChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Control OnSizeFlagsChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static Control OnTreeEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Control OnTreeExited(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Control OnTreeExiting(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Control OnVisibilityChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ControlAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles OnGameplayEntered(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticlesAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles OnGameplayExited(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticlesAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles OnReady(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticlesAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles OnRenamed(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticlesAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles OnScriptChanged(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticlesAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles OnTreeEntered(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticlesAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles OnTreeExited(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticlesAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles OnTreeExiting(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticlesAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles OnVisibilityChanged(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticlesAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles2D OnDraw(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles2D OnHide(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles2D OnItemRectChanged(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles2D OnReady(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles2D OnRenamed(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles2D OnScriptChanged(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles2D OnTreeEntered(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles2D OnTreeExited(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles2D OnTreeExiting(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CPUParticles2D OnVisibilityChanged(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CSGBox OnGameplayEntered(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGBoxAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static CSGBox OnGameplayExited(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGBoxAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static CSGBox OnReady(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGBoxAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CSGBox OnRenamed(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGBoxAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CSGBox OnScriptChanged(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGBoxAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CSGBox OnTreeEntered(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGBoxAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CSGBox OnTreeExited(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGBoxAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CSGBox OnTreeExiting(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGBoxAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CSGBox OnVisibilityChanged(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGBoxAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CSGCombiner OnGameplayEntered(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCombinerAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static CSGCombiner OnGameplayExited(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCombinerAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static CSGCombiner OnReady(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCombinerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CSGCombiner OnRenamed(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCombinerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CSGCombiner OnScriptChanged(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCombinerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CSGCombiner OnTreeEntered(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCombinerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CSGCombiner OnTreeExited(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCombinerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CSGCombiner OnTreeExiting(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCombinerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CSGCombiner OnVisibilityChanged(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCombinerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CSGCylinder OnGameplayEntered(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCylinderAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static CSGCylinder OnGameplayExited(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCylinderAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static CSGCylinder OnReady(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCylinderAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CSGCylinder OnRenamed(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCylinderAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CSGCylinder OnScriptChanged(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCylinderAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CSGCylinder OnTreeEntered(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCylinderAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CSGCylinder OnTreeExited(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCylinderAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CSGCylinder OnTreeExiting(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCylinderAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CSGCylinder OnVisibilityChanged(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGCylinderAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CSGMesh OnGameplayEntered(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGMeshAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static CSGMesh OnGameplayExited(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGMeshAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static CSGMesh OnReady(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGMeshAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CSGMesh OnRenamed(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGMeshAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CSGMesh OnScriptChanged(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGMeshAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CSGMesh OnTreeEntered(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGMeshAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CSGMesh OnTreeExited(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGMeshAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CSGMesh OnTreeExiting(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGMeshAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CSGMesh OnVisibilityChanged(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGMeshAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CSGPolygon OnGameplayEntered(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGPolygonAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static CSGPolygon OnGameplayExited(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGPolygonAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static CSGPolygon OnReady(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGPolygonAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CSGPolygon OnRenamed(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGPolygonAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CSGPolygon OnScriptChanged(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGPolygonAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CSGPolygon OnTreeEntered(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGPolygonAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CSGPolygon OnTreeExited(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGPolygonAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CSGPolygon OnTreeExiting(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGPolygonAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CSGPolygon OnVisibilityChanged(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGPolygonAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CSGSphere OnGameplayEntered(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGSphereAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static CSGSphere OnGameplayExited(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGSphereAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static CSGSphere OnReady(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGSphereAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CSGSphere OnRenamed(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGSphereAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CSGSphere OnScriptChanged(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGSphereAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CSGSphere OnTreeEntered(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGSphereAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CSGSphere OnTreeExited(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGSphereAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CSGSphere OnTreeExiting(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGSphereAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CSGSphere OnVisibilityChanged(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGSphereAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static CSGTorus OnGameplayEntered(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGTorusAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static CSGTorus OnGameplayExited(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGTorusAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static CSGTorus OnReady(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGTorusAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static CSGTorus OnRenamed(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGTorusAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static CSGTorus OnScriptChanged(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGTorusAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static CSGTorus OnTreeEntered(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGTorusAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static CSGTorus OnTreeExited(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGTorusAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static CSGTorus OnTreeExiting(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGTorusAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static CSGTorus OnVisibilityChanged(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<CSGTorusAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static DampedSpringJoint2D OnDraw(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static DampedSpringJoint2D OnHide(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static DampedSpringJoint2D OnItemRectChanged(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static DampedSpringJoint2D OnReady(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static DampedSpringJoint2D OnRenamed(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static DampedSpringJoint2D OnScriptChanged(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static DampedSpringJoint2D OnTreeEntered(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static DampedSpringJoint2D OnTreeExited(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static DampedSpringJoint2D OnTreeExiting(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static DampedSpringJoint2D OnVisibilityChanged(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static DirectionalLight OnGameplayEntered(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DirectionalLightAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static DirectionalLight OnGameplayExited(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DirectionalLightAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static DirectionalLight OnReady(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DirectionalLightAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static DirectionalLight OnRenamed(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DirectionalLightAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static DirectionalLight OnScriptChanged(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DirectionalLightAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static DirectionalLight OnTreeEntered(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DirectionalLightAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static DirectionalLight OnTreeExited(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DirectionalLightAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static DirectionalLight OnTreeExiting(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DirectionalLightAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static DirectionalLight OnVisibilityChanged(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<DirectionalLightAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnAboutToShow(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnAboutToShow(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnConfirmed(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnConfirmed(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnCustomAction(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnCustomAction(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnDirSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnDirSelected(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnDraw(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnFileSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnFileSelected(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnFilesSelected(this FileDialog target, Action<string[]> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnFilesSelected(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnFocusEntered(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnFocusExited(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnGuiInput(this FileDialog target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnHide(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnItemRectChanged(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnMinimumSizeChanged(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnModalClosed(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnMouseEntered(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnMouseExited(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnPopupHide(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnPopupHide(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnReady(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnRenamed(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnResized(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnScriptChanged(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnSizeFlagsChanged(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnTreeEntered(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnTreeExited(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnTreeExiting(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static FileDialog OnVisibilityChanged(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<FileDialogAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Generic6DOFJoint OnGameplayEntered(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static Generic6DOFJoint OnGameplayExited(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static Generic6DOFJoint OnReady(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Generic6DOFJoint OnRenamed(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Generic6DOFJoint OnScriptChanged(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Generic6DOFJoint OnTreeEntered(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Generic6DOFJoint OnTreeExited(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Generic6DOFJoint OnTreeExiting(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Generic6DOFJoint OnVisibilityChanged(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static GIProbe OnGameplayEntered(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GIProbeAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static GIProbe OnGameplayExited(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GIProbeAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static GIProbe OnReady(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GIProbeAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static GIProbe OnRenamed(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GIProbeAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static GIProbe OnScriptChanged(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GIProbeAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static GIProbe OnTreeEntered(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GIProbeAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static GIProbe OnTreeExited(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GIProbeAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static GIProbe OnTreeExiting(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GIProbeAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static GIProbe OnVisibilityChanged(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GIProbeAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnBeginNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnBeginNodeMove(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnConnectionFromEmpty(this GraphEdit target, Action<Vector2, string, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnConnectionFromEmpty(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnConnectionRequest(this GraphEdit target, Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnConnectionRequest(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnConnectionToEmpty(this GraphEdit target, Action<string, int, Vector2> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnConnectionToEmpty(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnCopyNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnCopyNodesRequest(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnDeleteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnDeleteNodesRequest(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnDisconnectionRequest(this GraphEdit target, Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnDisconnectionRequest(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnDraw(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnDuplicateNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnDuplicateNodesRequest(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnEndNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnEndNodeMove(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnFocusEntered(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnFocusExited(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnGuiInput(this GraphEdit target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnHide(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnItemRectChanged(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnMinimumSizeChanged(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnModalClosed(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnMouseEntered(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnMouseExited(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnNodeSelected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnNodeSelected(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnNodeUnselected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnNodeUnselected(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnPasteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnPasteNodesRequest(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnPopupRequest(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnPopupRequest(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnReady(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnRenamed(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnResized(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnScriptChanged(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnScrollOffsetChanged(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnScrollOffsetChanged(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnSizeFlagsChanged(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnTreeEntered(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnTreeExited(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnTreeExiting(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static GraphEdit OnVisibilityChanged(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphEditAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnCloseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnCloseRequest(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnDragged(this GraphNode target, Action<Vector2, Vector2> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnDragged(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnDraw(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnFocusEntered(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnFocusExited(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnGuiInput(this GraphNode target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnHide(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnItemRectChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnMinimumSizeChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnModalClosed(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnMouseEntered(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnMouseExited(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnOffsetChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnOffsetChanged(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnRaiseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnRaiseRequest(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnReady(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnRenamed(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnResized(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnResizeRequest(this GraphNode target, Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnResizeRequest(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnScriptChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnSizeFlagsChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnSlotUpdated(this GraphNode target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnSlotUpdated(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnSortChildren(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnTreeEntered(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnTreeExited(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnTreeExiting(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static GraphNode OnVisibilityChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GraphNodeAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnDraw(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnFocusEntered(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnFocusExited(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnGuiInput(this GridContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnHide(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnItemRectChanged(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnMinimumSizeChanged(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnModalClosed(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnMouseEntered(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnMouseExited(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnReady(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnRenamed(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnResized(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnScriptChanged(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnSizeFlagsChanged(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnSortChildren(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnTreeEntered(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnTreeExited(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnTreeExiting(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static GridContainer OnVisibilityChanged(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static GridMap OnCellSizeChanged(this GridMap target, Action<Vector3> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridMapAction>(target).OnCellSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static GridMap OnGameplayEntered(this GridMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridMapAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static GridMap OnGameplayExited(this GridMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridMapAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static GridMap OnReady(this GridMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridMapAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static GridMap OnRenamed(this GridMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridMapAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static GridMap OnScriptChanged(this GridMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridMapAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static GridMap OnTreeEntered(this GridMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridMapAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static GridMap OnTreeExited(this GridMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridMapAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static GridMap OnTreeExiting(this GridMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridMapAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static GridMap OnVisibilityChanged(this GridMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GridMapAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static GrooveJoint2D OnDraw(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static GrooveJoint2D OnHide(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static GrooveJoint2D OnItemRectChanged(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static GrooveJoint2D OnReady(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static GrooveJoint2D OnRenamed(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static GrooveJoint2D OnScriptChanged(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static GrooveJoint2D OnTreeEntered(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static GrooveJoint2D OnTreeExited(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static GrooveJoint2D OnTreeExiting(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static GrooveJoint2D OnVisibilityChanged(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnDraw(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnFocusEntered(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnFocusExited(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnGuiInput(this HBoxContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnHide(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnItemRectChanged(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnMinimumSizeChanged(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnModalClosed(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnMouseEntered(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnMouseExited(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnReady(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnRenamed(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnResized(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnScriptChanged(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnSizeFlagsChanged(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnSortChildren(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnTreeEntered(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnTreeExited(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnTreeExiting(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static HBoxContainer OnVisibilityChanged(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HBoxContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static HingeJoint OnGameplayEntered(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HingeJointAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static HingeJoint OnGameplayExited(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HingeJointAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static HingeJoint OnReady(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HingeJointAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static HingeJoint OnRenamed(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HingeJointAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static HingeJoint OnScriptChanged(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HingeJointAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static HingeJoint OnTreeEntered(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HingeJointAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static HingeJoint OnTreeExited(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HingeJointAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static HingeJoint OnTreeExiting(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HingeJointAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static HingeJoint OnVisibilityChanged(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HingeJointAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnChanged(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnChanged(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnDraw(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnFocusEntered(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnFocusExited(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnGuiInput(this HScrollBar target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnHide(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnItemRectChanged(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnMinimumSizeChanged(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnModalClosed(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnMouseEntered(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnMouseExited(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnReady(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnRenamed(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnResized(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnScriptChanged(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnScrolling(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnScrolling(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnSizeFlagsChanged(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnTreeEntered(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnTreeExited(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnTreeExiting(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnValueChanged(this HScrollBar target, Action<float> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnValueChanged(action, oneShot, deferred);
            return target;
        }

        public static HScrollBar OnVisibilityChanged(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HScrollBarAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnDraw(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnFocusEntered(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnFocusExited(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnGuiInput(this HSeparator target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnHide(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnItemRectChanged(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnMinimumSizeChanged(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnModalClosed(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnMouseEntered(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnMouseExited(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnReady(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnRenamed(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnResized(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnScriptChanged(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnSizeFlagsChanged(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnTreeEntered(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnTreeExited(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnTreeExiting(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static HSeparator OnVisibilityChanged(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSeparatorAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnChanged(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnChanged(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnDraw(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnFocusEntered(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnFocusExited(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnGuiInput(this HSlider target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnHide(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnItemRectChanged(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnMinimumSizeChanged(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnModalClosed(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnMouseEntered(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnMouseExited(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnReady(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnRenamed(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnResized(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnScriptChanged(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnSizeFlagsChanged(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnTreeEntered(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnTreeExited(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnTreeExiting(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnValueChanged(this HSlider target, Action<float> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnValueChanged(action, oneShot, deferred);
            return target;
        }

        public static HSlider OnVisibilityChanged(this HSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSliderAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnDragged(this HSplitContainer target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnDragged(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnDraw(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnFocusEntered(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnFocusExited(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnGuiInput(this HSplitContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnHide(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnItemRectChanged(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnMinimumSizeChanged(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnModalClosed(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnMouseEntered(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnMouseExited(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnReady(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnRenamed(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnResized(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnScriptChanged(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnSizeFlagsChanged(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnSortChildren(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnTreeEntered(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnTreeExited(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnTreeExiting(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static HSplitContainer OnVisibilityChanged(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HSplitContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static HTTPRequest OnReady(this HTTPRequest target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HTTPRequestAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static HTTPRequest OnRenamed(this HTTPRequest target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HTTPRequestAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static HTTPRequest OnRequestCompleted(this HTTPRequest target, Action<byte[], string[], int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HTTPRequestAction>(target).OnRequestCompleted(action, oneShot, deferred);
            return target;
        }

        public static HTTPRequest OnScriptChanged(this HTTPRequest target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HTTPRequestAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static HTTPRequest OnTreeEntered(this HTTPRequest target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HTTPRequestAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static HTTPRequest OnTreeExited(this HTTPRequest target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HTTPRequestAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static HTTPRequest OnTreeExiting(this HTTPRequest target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<HTTPRequestAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ImmediateGeometry OnGameplayEntered(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static ImmediateGeometry OnGameplayExited(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static ImmediateGeometry OnReady(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ImmediateGeometry OnRenamed(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ImmediateGeometry OnScriptChanged(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ImmediateGeometry OnTreeEntered(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ImmediateGeometry OnTreeExited(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ImmediateGeometry OnTreeExiting(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ImmediateGeometry OnVisibilityChanged(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static InterpolatedCamera OnGameplayEntered(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static InterpolatedCamera OnGameplayExited(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static InterpolatedCamera OnReady(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static InterpolatedCamera OnRenamed(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static InterpolatedCamera OnScriptChanged(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static InterpolatedCamera OnTreeEntered(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static InterpolatedCamera OnTreeExited(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static InterpolatedCamera OnTreeExiting(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static InterpolatedCamera OnVisibilityChanged(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnDraw(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnFocusEntered(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnFocusExited(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnGuiInput(this ItemList target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnHide(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnItemActivated(this ItemList target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnItemActivated(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnItemRectChanged(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnItemRmbSelected(this ItemList target, Action<Vector2, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnItemRmbSelected(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnItemSelected(this ItemList target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnItemSelected(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnMinimumSizeChanged(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnModalClosed(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnMouseEntered(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnMouseExited(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnMultiSelected(this ItemList target, Action<int, bool> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnMultiSelected(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnNothingSelected(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnNothingSelected(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnReady(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnRenamed(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnResized(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnRmbClicked(this ItemList target, Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnRmbClicked(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnScriptChanged(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnSizeFlagsChanged(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnTreeEntered(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnTreeExited(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnTreeExiting(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ItemList OnVisibilityChanged(this ItemList target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ItemListAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody OnGameplayEntered(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBodyAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody OnGameplayExited(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBodyAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody OnInputEvent(this KinematicBody target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBodyAction>(target).OnInputEvent(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody OnMouseEntered(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBodyAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody OnMouseExited(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBodyAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody OnReady(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBodyAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody OnRenamed(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBodyAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody OnScriptChanged(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBodyAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody OnTreeEntered(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBodyAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody OnTreeExited(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBodyAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody OnTreeExiting(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBodyAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody OnVisibilityChanged(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBodyAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody2D OnDraw(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody2D OnHide(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody2D OnInputEvent(this KinematicBody2D target, Action<InputEvent, int, Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnInputEvent(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody2D OnItemRectChanged(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody2D OnMouseEntered(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody2D OnMouseExited(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody2D OnReady(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody2D OnRenamed(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody2D OnScriptChanged(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody2D OnTreeEntered(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody2D OnTreeExited(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody2D OnTreeExiting(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static KinematicBody2D OnVisibilityChanged(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Label OnDraw(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Label OnFocusEntered(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static Label OnFocusExited(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static Label OnGuiInput(this Label target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static Label OnHide(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Label OnItemRectChanged(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Label OnMinimumSizeChanged(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static Label OnModalClosed(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static Label OnMouseEntered(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static Label OnMouseExited(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static Label OnReady(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Label OnRenamed(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Label OnResized(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static Label OnScriptChanged(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Label OnSizeFlagsChanged(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static Label OnTreeEntered(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Label OnTreeExited(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Label OnTreeExiting(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Label OnVisibilityChanged(this Label target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LabelAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Light2D OnDraw(this Light2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Light2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Light2D OnHide(this Light2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Light2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Light2D OnItemRectChanged(this Light2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Light2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Light2D OnReady(this Light2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Light2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Light2D OnRenamed(this Light2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Light2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Light2D OnScriptChanged(this Light2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Light2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Light2D OnTreeEntered(this Light2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Light2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Light2D OnTreeExited(this Light2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Light2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Light2D OnTreeExiting(this Light2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Light2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Light2D OnVisibilityChanged(this Light2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Light2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static LightOccluder2D OnDraw(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static LightOccluder2D OnHide(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static LightOccluder2D OnItemRectChanged(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static LightOccluder2D OnReady(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static LightOccluder2D OnRenamed(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static LightOccluder2D OnScriptChanged(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static LightOccluder2D OnTreeEntered(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static LightOccluder2D OnTreeExited(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static LightOccluder2D OnTreeExiting(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static LightOccluder2D OnVisibilityChanged(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Line2D OnDraw(this Line2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Line2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Line2D OnHide(this Line2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Line2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Line2D OnItemRectChanged(this Line2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Line2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Line2D OnReady(this Line2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Line2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Line2D OnRenamed(this Line2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Line2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Line2D OnScriptChanged(this Line2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Line2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Line2D OnTreeEntered(this Line2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Line2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Line2D OnTreeExited(this Line2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Line2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Line2D OnTreeExiting(this Line2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Line2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Line2D OnVisibilityChanged(this Line2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Line2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnDraw(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnFocusEntered(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnFocusExited(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnGuiInput(this LineEdit target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnHide(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnItemRectChanged(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnMinimumSizeChanged(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnModalClosed(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnMouseEntered(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnMouseExited(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnReady(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnRenamed(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnResized(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnScriptChanged(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnSizeFlagsChanged(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnTextChanged(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnTextChanged(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnTextChangeRejected(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnTextChangeRejected(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnTextEntered(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnTextEntered(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnTreeEntered(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnTreeExited(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnTreeExiting(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static LineEdit OnVisibilityChanged(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LineEditAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnButtonDown(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnButtonDown(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnButtonUp(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnButtonUp(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnDraw(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnFocusEntered(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnFocusExited(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnGuiInput(this LinkButton target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnHide(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnItemRectChanged(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnMinimumSizeChanged(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnModalClosed(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnMouseEntered(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnMouseExited(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnPressed(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnPressed(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnReady(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnRenamed(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnResized(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnScriptChanged(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnSizeFlagsChanged(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnToggled(this LinkButton target, Action<bool> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnToggled(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnTreeEntered(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnTreeExited(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnTreeExiting(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static LinkButton OnVisibilityChanged(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<LinkButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Listener OnGameplayEntered(this Listener target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ListenerAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static Listener OnGameplayExited(this Listener target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ListenerAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static Listener OnReady(this Listener target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ListenerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Listener OnRenamed(this Listener target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ListenerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Listener OnScriptChanged(this Listener target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ListenerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Listener OnTreeEntered(this Listener target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ListenerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Listener OnTreeExited(this Listener target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ListenerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Listener OnTreeExiting(this Listener target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ListenerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Listener OnVisibilityChanged(this Listener target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ListenerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Listener2D OnDraw(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Listener2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Listener2D OnHide(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Listener2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Listener2D OnItemRectChanged(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Listener2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Listener2D OnReady(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Listener2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Listener2D OnRenamed(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Listener2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Listener2D OnScriptChanged(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Listener2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Listener2D OnTreeEntered(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Listener2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Listener2D OnTreeExited(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Listener2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Listener2D OnTreeExiting(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Listener2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Listener2D OnVisibilityChanged(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Listener2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnDraw(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnFocusEntered(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnFocusExited(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnGuiInput(this MarginContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnHide(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnItemRectChanged(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnMinimumSizeChanged(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnModalClosed(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnMouseEntered(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnMouseExited(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnReady(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnRenamed(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnResized(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnScriptChanged(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnSizeFlagsChanged(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnSortChildren(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnTreeEntered(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnTreeExited(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnTreeExiting(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static MarginContainer OnVisibilityChanged(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MarginContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnAboutToShow(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnAboutToShow(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnButtonDown(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnButtonDown(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnButtonUp(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnButtonUp(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnDraw(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnFocusEntered(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnFocusExited(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnGuiInput(this MenuButton target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnHide(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnItemRectChanged(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnMinimumSizeChanged(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnModalClosed(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnMouseEntered(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnMouseExited(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnPressed(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnPressed(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnReady(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnRenamed(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnResized(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnScriptChanged(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnSizeFlagsChanged(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnToggled(this MenuButton target, Action<bool> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnToggled(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnTreeEntered(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnTreeExited(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnTreeExiting(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static MenuButton OnVisibilityChanged(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MenuButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance OnGameplayEntered(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstanceAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance OnGameplayExited(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstanceAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance OnReady(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstanceAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance OnRenamed(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstanceAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance OnScriptChanged(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstanceAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance OnTreeEntered(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstanceAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance OnTreeExited(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstanceAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance OnTreeExiting(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstanceAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance OnVisibilityChanged(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstanceAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance2D OnDraw(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance2D OnHide(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance2D OnItemRectChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance2D OnReady(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance2D OnRenamed(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance2D OnScriptChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance2D OnTextureChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnTextureChanged(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance2D OnTreeEntered(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance2D OnTreeExited(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance2D OnTreeExiting(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static MeshInstance2D OnVisibilityChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance OnGameplayEntered(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance OnGameplayExited(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance OnReady(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance OnRenamed(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance OnScriptChanged(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance OnTreeEntered(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance OnTreeExited(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance OnTreeExiting(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance OnVisibilityChanged(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance2D OnDraw(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance2D OnHide(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance2D OnItemRectChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance2D OnReady(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance2D OnRenamed(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance2D OnScriptChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance2D OnTextureChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnTextureChanged(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance2D OnTreeEntered(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance2D OnTreeExited(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance2D OnTreeExiting(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static MultiMeshInstance2D OnVisibilityChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Navigation OnGameplayEntered(this Navigation target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static Navigation OnGameplayExited(this Navigation target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static Navigation OnReady(this Navigation target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Navigation OnRenamed(this Navigation target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Navigation OnScriptChanged(this Navigation target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Navigation OnTreeEntered(this Navigation target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Navigation OnTreeExited(this Navigation target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Navigation OnTreeExiting(this Navigation target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Navigation OnVisibilityChanged(this Navigation target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Navigation2D OnDraw(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Navigation2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Navigation2D OnHide(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Navigation2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Navigation2D OnItemRectChanged(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Navigation2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Navigation2D OnReady(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Navigation2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Navigation2D OnRenamed(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Navigation2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Navigation2D OnScriptChanged(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Navigation2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Navigation2D OnTreeEntered(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Navigation2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Navigation2D OnTreeExited(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Navigation2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Navigation2D OnTreeExiting(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Navigation2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Navigation2D OnVisibilityChanged(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Navigation2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static NavigationMeshInstance OnGameplayEntered(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static NavigationMeshInstance OnGameplayExited(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static NavigationMeshInstance OnReady(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static NavigationMeshInstance OnRenamed(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static NavigationMeshInstance OnScriptChanged(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static NavigationMeshInstance OnTreeEntered(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static NavigationMeshInstance OnTreeExited(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static NavigationMeshInstance OnTreeExiting(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static NavigationMeshInstance OnVisibilityChanged(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static NavigationPolygonInstance OnDraw(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static NavigationPolygonInstance OnHide(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static NavigationPolygonInstance OnItemRectChanged(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static NavigationPolygonInstance OnReady(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static NavigationPolygonInstance OnRenamed(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static NavigationPolygonInstance OnScriptChanged(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static NavigationPolygonInstance OnTreeEntered(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static NavigationPolygonInstance OnTreeExited(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static NavigationPolygonInstance OnTreeExiting(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static NavigationPolygonInstance OnVisibilityChanged(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnDraw(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnFocusEntered(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnFocusExited(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnGuiInput(this NinePatchRect target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnHide(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnItemRectChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnMinimumSizeChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnModalClosed(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnMouseEntered(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnMouseExited(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnReady(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnRenamed(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnResized(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnScriptChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnSizeFlagsChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnTextureChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnTextureChanged(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnTreeEntered(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnTreeExited(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnTreeExiting(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static NinePatchRect OnVisibilityChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<NinePatchRectAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Node2D OnDraw(this Node2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Node2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Node2D OnHide(this Node2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Node2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Node2D OnItemRectChanged(this Node2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Node2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Node2D OnReady(this Node2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Node2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Node2D OnRenamed(this Node2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Node2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Node2D OnScriptChanged(this Node2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Node2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Node2D OnTreeEntered(this Node2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Node2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Node2D OnTreeExited(this Node2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Node2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Node2D OnTreeExiting(this Node2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Node2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Node2D OnVisibilityChanged(this Node2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Node2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Occluder OnGameplayEntered(this Occluder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OccluderAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static Occluder OnGameplayExited(this Occluder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OccluderAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static Occluder OnReady(this Occluder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OccluderAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Occluder OnRenamed(this Occluder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OccluderAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Occluder OnScriptChanged(this Occluder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OccluderAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Occluder OnTreeEntered(this Occluder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OccluderAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Occluder OnTreeExited(this Occluder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OccluderAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Occluder OnTreeExiting(this Occluder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OccluderAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Occluder OnVisibilityChanged(this Occluder target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OccluderAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static OmniLight OnGameplayEntered(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OmniLightAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static OmniLight OnGameplayExited(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OmniLightAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static OmniLight OnReady(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OmniLightAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static OmniLight OnRenamed(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OmniLightAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static OmniLight OnScriptChanged(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OmniLightAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static OmniLight OnTreeEntered(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OmniLightAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static OmniLight OnTreeExited(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OmniLightAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static OmniLight OnTreeExiting(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OmniLightAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static OmniLight OnVisibilityChanged(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OmniLightAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnButtonDown(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnButtonDown(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnButtonUp(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnButtonUp(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnDraw(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnFocusEntered(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnFocusExited(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnGuiInput(this OptionButton target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnHide(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnItemFocused(this OptionButton target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnItemFocused(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnItemRectChanged(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnItemSelected(this OptionButton target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnItemSelected(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnMinimumSizeChanged(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnModalClosed(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnMouseEntered(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnMouseExited(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnPressed(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnPressed(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnReady(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnRenamed(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnResized(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnScriptChanged(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnSizeFlagsChanged(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnToggled(this OptionButton target, Action<bool> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnToggled(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnTreeEntered(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnTreeExited(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnTreeExiting(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static OptionButton OnVisibilityChanged(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<OptionButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Panel OnDraw(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Panel OnFocusEntered(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static Panel OnFocusExited(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static Panel OnGuiInput(this Panel target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static Panel OnHide(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Panel OnItemRectChanged(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Panel OnMinimumSizeChanged(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static Panel OnModalClosed(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static Panel OnMouseEntered(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static Panel OnMouseExited(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static Panel OnReady(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Panel OnRenamed(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Panel OnResized(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static Panel OnScriptChanged(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Panel OnSizeFlagsChanged(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static Panel OnTreeEntered(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Panel OnTreeExited(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Panel OnTreeExiting(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Panel OnVisibilityChanged(this Panel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnDraw(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnFocusEntered(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnFocusExited(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnGuiInput(this PanelContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnHide(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnItemRectChanged(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnMinimumSizeChanged(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnModalClosed(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnMouseEntered(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnMouseExited(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnReady(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnRenamed(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnResized(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnScriptChanged(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnSizeFlagsChanged(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnSortChildren(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnTreeEntered(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnTreeExited(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnTreeExiting(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static PanelContainer OnVisibilityChanged(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PanelContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ParallaxBackground OnReady(this ParallaxBackground target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxBackgroundAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ParallaxBackground OnRenamed(this ParallaxBackground target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxBackgroundAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ParallaxBackground OnScriptChanged(this ParallaxBackground target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxBackgroundAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ParallaxBackground OnTreeEntered(this ParallaxBackground target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxBackgroundAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ParallaxBackground OnTreeExited(this ParallaxBackground target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxBackgroundAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ParallaxBackground OnTreeExiting(this ParallaxBackground target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxBackgroundAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ParallaxLayer OnDraw(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static ParallaxLayer OnHide(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static ParallaxLayer OnItemRectChanged(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static ParallaxLayer OnReady(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ParallaxLayer OnRenamed(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ParallaxLayer OnScriptChanged(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ParallaxLayer OnTreeEntered(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ParallaxLayer OnTreeExited(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ParallaxLayer OnTreeExiting(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ParallaxLayer OnVisibilityChanged(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Particles OnGameplayEntered(this Particles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParticlesAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static Particles OnGameplayExited(this Particles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParticlesAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static Particles OnReady(this Particles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParticlesAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Particles OnRenamed(this Particles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParticlesAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Particles OnScriptChanged(this Particles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParticlesAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Particles OnTreeEntered(this Particles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParticlesAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Particles OnTreeExited(this Particles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParticlesAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Particles OnTreeExiting(this Particles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParticlesAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Particles OnVisibilityChanged(this Particles target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ParticlesAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Particles2D OnDraw(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Particles2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Particles2D OnHide(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Particles2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Particles2D OnItemRectChanged(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Particles2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Particles2D OnReady(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Particles2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Particles2D OnRenamed(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Particles2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Particles2D OnScriptChanged(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Particles2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Particles2D OnTreeEntered(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Particles2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Particles2D OnTreeExited(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Particles2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Particles2D OnTreeExiting(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Particles2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Particles2D OnVisibilityChanged(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Particles2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Path OnCurveChanged(this Path target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathAction>(target).OnCurveChanged(action, oneShot, deferred);
            return target;
        }

        public static Path OnGameplayEntered(this Path target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static Path OnGameplayExited(this Path target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static Path OnReady(this Path target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Path OnRenamed(this Path target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Path OnScriptChanged(this Path target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Path OnTreeEntered(this Path target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Path OnTreeExited(this Path target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Path OnTreeExiting(this Path target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Path OnVisibilityChanged(this Path target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Path2D OnDraw(this Path2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Path2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Path2D OnHide(this Path2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Path2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Path2D OnItemRectChanged(this Path2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Path2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Path2D OnReady(this Path2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Path2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Path2D OnRenamed(this Path2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Path2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Path2D OnScriptChanged(this Path2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Path2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Path2D OnTreeEntered(this Path2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Path2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Path2D OnTreeExited(this Path2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Path2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Path2D OnTreeExiting(this Path2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Path2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Path2D OnVisibilityChanged(this Path2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Path2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static PathFollow OnGameplayEntered(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollowAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static PathFollow OnGameplayExited(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollowAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static PathFollow OnReady(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollowAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static PathFollow OnRenamed(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollowAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static PathFollow OnScriptChanged(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollowAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static PathFollow OnTreeEntered(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollowAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static PathFollow OnTreeExited(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollowAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static PathFollow OnTreeExiting(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollowAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static PathFollow OnVisibilityChanged(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollowAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static PathFollow2D OnDraw(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollow2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static PathFollow2D OnHide(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollow2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static PathFollow2D OnItemRectChanged(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollow2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static PathFollow2D OnReady(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollow2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static PathFollow2D OnRenamed(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollow2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static PathFollow2D OnScriptChanged(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollow2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static PathFollow2D OnTreeEntered(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollow2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static PathFollow2D OnTreeExited(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollow2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static PathFollow2D OnTreeExiting(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollow2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static PathFollow2D OnVisibilityChanged(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PathFollow2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static PhysicalBone OnGameplayEntered(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static PhysicalBone OnGameplayExited(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static PhysicalBone OnInputEvent(this PhysicalBone target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnInputEvent(action, oneShot, deferred);
            return target;
        }

        public static PhysicalBone OnMouseEntered(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static PhysicalBone OnMouseExited(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static PhysicalBone OnReady(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static PhysicalBone OnRenamed(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static PhysicalBone OnScriptChanged(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static PhysicalBone OnTreeEntered(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static PhysicalBone OnTreeExited(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static PhysicalBone OnTreeExiting(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static PhysicalBone OnVisibilityChanged(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static PinJoint OnGameplayEntered(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJointAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static PinJoint OnGameplayExited(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJointAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static PinJoint OnReady(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJointAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static PinJoint OnRenamed(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJointAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static PinJoint OnScriptChanged(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJointAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static PinJoint OnTreeEntered(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJointAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static PinJoint OnTreeExited(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJointAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static PinJoint OnTreeExiting(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJointAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static PinJoint OnVisibilityChanged(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJointAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static PinJoint2D OnDraw(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJoint2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static PinJoint2D OnHide(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJoint2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static PinJoint2D OnItemRectChanged(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJoint2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static PinJoint2D OnReady(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJoint2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static PinJoint2D OnRenamed(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJoint2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static PinJoint2D OnScriptChanged(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJoint2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static PinJoint2D OnTreeEntered(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJoint2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static PinJoint2D OnTreeExited(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJoint2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static PinJoint2D OnTreeExiting(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJoint2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static PinJoint2D OnVisibilityChanged(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PinJoint2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Polygon2D OnDraw(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Polygon2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Polygon2D OnHide(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Polygon2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Polygon2D OnItemRectChanged(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Polygon2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Polygon2D OnReady(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Polygon2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Polygon2D OnRenamed(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Polygon2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Polygon2D OnScriptChanged(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Polygon2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Polygon2D OnTreeEntered(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Polygon2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Polygon2D OnTreeExited(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Polygon2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Polygon2D OnTreeExiting(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Polygon2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Polygon2D OnVisibilityChanged(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Polygon2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Popup OnAboutToShow(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnAboutToShow(action, oneShot, deferred);
            return target;
        }

        public static Popup OnDraw(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Popup OnFocusEntered(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static Popup OnFocusExited(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static Popup OnGuiInput(this Popup target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static Popup OnHide(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Popup OnItemRectChanged(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Popup OnMinimumSizeChanged(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static Popup OnModalClosed(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static Popup OnMouseEntered(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static Popup OnMouseExited(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static Popup OnPopupHide(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnPopupHide(action, oneShot, deferred);
            return target;
        }

        public static Popup OnReady(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Popup OnRenamed(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Popup OnResized(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static Popup OnScriptChanged(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Popup OnSizeFlagsChanged(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static Popup OnTreeEntered(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Popup OnTreeExited(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Popup OnTreeExiting(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Popup OnVisibilityChanged(this Popup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnAboutToShow(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnAboutToShow(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnDraw(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnFocusEntered(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnFocusExited(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnGuiInput(this PopupDialog target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnHide(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnItemRectChanged(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnMinimumSizeChanged(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnModalClosed(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnMouseEntered(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnMouseExited(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnPopupHide(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnPopupHide(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnReady(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnRenamed(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnResized(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnScriptChanged(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnSizeFlagsChanged(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnTreeEntered(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnTreeExited(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnTreeExiting(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static PopupDialog OnVisibilityChanged(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupDialogAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnAboutToShow(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnAboutToShow(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnDraw(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnFocusEntered(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnFocusExited(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnGuiInput(this PopupMenu target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnHide(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnIdFocused(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnIdFocused(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnIdPressed(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnIdPressed(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnIndexPressed(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnIndexPressed(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnItemRectChanged(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnMinimumSizeChanged(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnModalClosed(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnMouseEntered(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnMouseExited(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnPopupHide(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnPopupHide(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnReady(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnRenamed(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnResized(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnScriptChanged(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnSizeFlagsChanged(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnTreeEntered(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnTreeExited(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnTreeExiting(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static PopupMenu OnVisibilityChanged(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupMenuAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnAboutToShow(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnAboutToShow(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnDraw(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnFocusEntered(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnFocusExited(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnGuiInput(this PopupPanel target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnHide(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnItemRectChanged(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnMinimumSizeChanged(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnModalClosed(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnMouseEntered(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnMouseExited(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnPopupHide(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnPopupHide(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnReady(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnRenamed(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnResized(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnScriptChanged(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnSizeFlagsChanged(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnTreeEntered(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnTreeExited(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnTreeExiting(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static PopupPanel OnVisibilityChanged(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PopupPanelAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Portal OnGameplayEntered(this Portal target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PortalAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static Portal OnGameplayExited(this Portal target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PortalAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static Portal OnReady(this Portal target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PortalAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Portal OnRenamed(this Portal target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PortalAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Portal OnScriptChanged(this Portal target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PortalAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Portal OnTreeEntered(this Portal target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PortalAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Portal OnTreeExited(this Portal target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PortalAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Portal OnTreeExiting(this Portal target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PortalAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Portal OnVisibilityChanged(this Portal target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<PortalAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Position2D OnDraw(this Position2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Position2D OnHide(this Position2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Position2D OnItemRectChanged(this Position2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Position2D OnReady(this Position2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Position2D OnRenamed(this Position2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Position2D OnScriptChanged(this Position2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Position2D OnTreeEntered(this Position2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Position2D OnTreeExited(this Position2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Position2D OnTreeExiting(this Position2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Position2D OnVisibilityChanged(this Position2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Position3D OnGameplayEntered(this Position3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position3DAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static Position3D OnGameplayExited(this Position3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position3DAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static Position3D OnReady(this Position3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position3DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Position3D OnRenamed(this Position3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position3DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Position3D OnScriptChanged(this Position3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position3DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Position3D OnTreeEntered(this Position3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position3DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Position3D OnTreeExited(this Position3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position3DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Position3D OnTreeExiting(this Position3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position3DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Position3D OnVisibilityChanged(this Position3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Position3DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnChanged(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnChanged(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnDraw(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnFocusEntered(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnFocusExited(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnGuiInput(this ProgressBar target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnHide(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnItemRectChanged(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnMinimumSizeChanged(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnModalClosed(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnMouseEntered(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnMouseExited(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnReady(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnRenamed(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnResized(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnScriptChanged(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnSizeFlagsChanged(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnTreeEntered(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnTreeExited(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnTreeExiting(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnValueChanged(this ProgressBar target, Action<float> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnValueChanged(action, oneShot, deferred);
            return target;
        }

        public static ProgressBar OnVisibilityChanged(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProgressBarAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ProximityGroup OnBroadcast(this ProximityGroup target, Action<string, Godot.Collections.Array> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProximityGroupAction>(target).OnBroadcast(action, oneShot, deferred);
            return target;
        }

        public static ProximityGroup OnGameplayEntered(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProximityGroupAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static ProximityGroup OnGameplayExited(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProximityGroupAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static ProximityGroup OnReady(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProximityGroupAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ProximityGroup OnRenamed(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProximityGroupAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ProximityGroup OnScriptChanged(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProximityGroupAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ProximityGroup OnTreeEntered(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProximityGroupAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ProximityGroup OnTreeExited(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProximityGroupAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ProximityGroup OnTreeExiting(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProximityGroupAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ProximityGroup OnVisibilityChanged(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ProximityGroupAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static RayCast OnGameplayEntered(this RayCast target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCastAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static RayCast OnGameplayExited(this RayCast target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCastAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static RayCast OnReady(this RayCast target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCastAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static RayCast OnRenamed(this RayCast target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCastAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static RayCast OnScriptChanged(this RayCast target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCastAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static RayCast OnTreeEntered(this RayCast target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCastAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static RayCast OnTreeExited(this RayCast target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCastAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static RayCast OnTreeExiting(this RayCast target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCastAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static RayCast OnVisibilityChanged(this RayCast target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCastAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static RayCast2D OnDraw(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCast2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static RayCast2D OnHide(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCast2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static RayCast2D OnItemRectChanged(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCast2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static RayCast2D OnReady(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCast2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static RayCast2D OnRenamed(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCast2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static RayCast2D OnScriptChanged(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCast2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static RayCast2D OnTreeEntered(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCast2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static RayCast2D OnTreeExited(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCast2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static RayCast2D OnTreeExiting(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCast2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static RayCast2D OnVisibilityChanged(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RayCast2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnDraw(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnFocusEntered(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnFocusExited(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnGuiInput(this ReferenceRect target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnHide(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnItemRectChanged(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnMinimumSizeChanged(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnModalClosed(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnMouseEntered(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnMouseExited(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnReady(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnRenamed(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnResized(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnScriptChanged(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnSizeFlagsChanged(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnTreeEntered(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnTreeExited(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnTreeExiting(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ReferenceRect OnVisibilityChanged(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReferenceRectAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ReflectionProbe OnGameplayEntered(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static ReflectionProbe OnGameplayExited(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static ReflectionProbe OnReady(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ReflectionProbe OnRenamed(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ReflectionProbe OnScriptChanged(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ReflectionProbe OnTreeEntered(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ReflectionProbe OnTreeExited(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ReflectionProbe OnTreeExiting(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ReflectionProbe OnVisibilityChanged(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform OnGameplayEntered(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransformAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform OnGameplayExited(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransformAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform OnReady(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransformAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform OnRenamed(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransformAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform OnScriptChanged(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransformAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform OnTreeEntered(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransformAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform OnTreeExited(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransformAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform OnTreeExiting(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransformAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform OnVisibilityChanged(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransformAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform2D OnDraw(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform2D OnHide(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform2D OnItemRectChanged(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform2D OnReady(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform2D OnRenamed(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform2D OnScriptChanged(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform2D OnTreeEntered(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform2D OnTreeExited(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform2D OnTreeExiting(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static RemoteTransform2D OnVisibilityChanged(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ResourcePreloader OnReady(this ResourcePreloader target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ResourcePreloaderAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ResourcePreloader OnRenamed(this ResourcePreloader target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ResourcePreloaderAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ResourcePreloader OnScriptChanged(this ResourcePreloader target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ResourcePreloaderAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ResourcePreloader OnTreeEntered(this ResourcePreloader target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ResourcePreloaderAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ResourcePreloader OnTreeExited(this ResourcePreloader target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ResourcePreloaderAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ResourcePreloader OnTreeExiting(this ResourcePreloader target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ResourcePreloaderAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnDraw(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnFocusEntered(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnFocusExited(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnGuiInput(this RichTextLabel target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnHide(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnItemRectChanged(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnMetaClicked(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnMetaClicked(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnMetaHoverEnded(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnMetaHoverEnded(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnMetaHoverStarted(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnMetaHoverStarted(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnMinimumSizeChanged(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnModalClosed(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnMouseEntered(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnMouseExited(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnReady(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnRenamed(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnResized(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnScriptChanged(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnSizeFlagsChanged(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnTreeEntered(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnTreeExited(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnTreeExiting(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static RichTextLabel OnVisibilityChanged(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RichTextLabelAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnBodyEntered(this RigidBody target, Action<Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnBodyEntered(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnBodyExited(this RigidBody target, Action<Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnBodyExited(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnBodyShapeEntered(this RigidBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnBodyShapeEntered(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnBodyShapeExited(this RigidBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnBodyShapeExited(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnGameplayEntered(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnGameplayExited(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnInputEvent(this RigidBody target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnInputEvent(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnMouseEntered(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnMouseExited(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnReady(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnRenamed(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnScriptChanged(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnSleepingStateChanged(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnSleepingStateChanged(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnTreeEntered(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnTreeExited(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnTreeExiting(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static RigidBody OnVisibilityChanged(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBodyAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnBodyEntered(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnBodyEntered(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnBodyExited(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnBodyExited(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnBodyShapeEntered(this RigidBody2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnBodyShapeEntered(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnBodyShapeExited(this RigidBody2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnBodyShapeExited(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnDraw(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnHide(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnInputEvent(this RigidBody2D target, Action<InputEvent, int, Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnInputEvent(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnItemRectChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnMouseEntered(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnMouseExited(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnReady(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnRenamed(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnScriptChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnSleepingStateChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnSleepingStateChanged(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnTreeEntered(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnTreeExited(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnTreeExiting(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static RigidBody2D OnVisibilityChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RigidBody2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Room OnGameplayEntered(this Room target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static Room OnGameplayExited(this Room target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static Room OnReady(this Room target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Room OnRenamed(this Room target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Room OnScriptChanged(this Room target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Room OnTreeEntered(this Room target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Room OnTreeExited(this Room target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Room OnTreeExiting(this Room target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Room OnVisibilityChanged(this Room target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static RoomGroup OnGameplayEntered(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomGroupAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static RoomGroup OnGameplayExited(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomGroupAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static RoomGroup OnReady(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomGroupAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static RoomGroup OnRenamed(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomGroupAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static RoomGroup OnScriptChanged(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomGroupAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static RoomGroup OnTreeEntered(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomGroupAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static RoomGroup OnTreeExited(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomGroupAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static RoomGroup OnTreeExiting(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomGroupAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static RoomGroup OnVisibilityChanged(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomGroupAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static RoomManager OnGameplayEntered(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomManagerAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static RoomManager OnGameplayExited(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomManagerAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static RoomManager OnReady(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomManagerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static RoomManager OnRenamed(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomManagerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static RoomManager OnScriptChanged(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomManagerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static RoomManager OnTreeEntered(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomManagerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static RoomManager OnTreeExited(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomManagerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static RoomManager OnTreeExiting(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomManagerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static RoomManager OnVisibilityChanged(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<RoomManagerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnDraw(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnFocusEntered(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnFocusExited(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnGuiInput(this ScrollContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnHide(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnItemRectChanged(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnMinimumSizeChanged(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnModalClosed(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnMouseEntered(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnMouseExited(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnReady(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnRenamed(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnResized(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnScriptChanged(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnScrollEnded(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnScrollEnded(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnScrollStarted(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnScrollStarted(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnSizeFlagsChanged(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnSortChildren(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnTreeEntered(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnTreeExited(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnTreeExiting(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ScrollContainer OnVisibilityChanged(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ScrollContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Skeleton OnGameplayEntered(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static Skeleton OnGameplayExited(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static Skeleton OnReady(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Skeleton OnRenamed(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Skeleton OnScriptChanged(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Skeleton OnSkeletonUpdated(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonAction>(target).OnSkeletonUpdated(action, oneShot, deferred);
            return target;
        }

        public static Skeleton OnTreeEntered(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Skeleton OnTreeExited(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Skeleton OnTreeExiting(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Skeleton OnVisibilityChanged(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Skeleton2D OnBoneSetupChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Skeleton2DAction>(target).OnBoneSetupChanged(action, oneShot, deferred);
            return target;
        }

        public static Skeleton2D OnDraw(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Skeleton2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Skeleton2D OnHide(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Skeleton2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Skeleton2D OnItemRectChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Skeleton2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Skeleton2D OnReady(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Skeleton2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Skeleton2D OnRenamed(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Skeleton2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Skeleton2D OnScriptChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Skeleton2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Skeleton2D OnTreeEntered(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Skeleton2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Skeleton2D OnTreeExited(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Skeleton2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Skeleton2D OnTreeExiting(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Skeleton2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Skeleton2D OnVisibilityChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Skeleton2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static SkeletonIK OnReady(this SkeletonIK target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonIKAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static SkeletonIK OnRenamed(this SkeletonIK target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonIKAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static SkeletonIK OnScriptChanged(this SkeletonIK target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonIKAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static SkeletonIK OnTreeEntered(this SkeletonIK target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonIKAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static SkeletonIK OnTreeExited(this SkeletonIK target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonIKAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static SkeletonIK OnTreeExiting(this SkeletonIK target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SkeletonIKAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static SliderJoint OnGameplayEntered(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SliderJointAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static SliderJoint OnGameplayExited(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SliderJointAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static SliderJoint OnReady(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SliderJointAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static SliderJoint OnRenamed(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SliderJointAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static SliderJoint OnScriptChanged(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SliderJointAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static SliderJoint OnTreeEntered(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SliderJointAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static SliderJoint OnTreeExited(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SliderJointAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static SliderJoint OnTreeExiting(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SliderJointAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static SliderJoint OnVisibilityChanged(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SliderJointAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static SoftBody OnGameplayEntered(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SoftBodyAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static SoftBody OnGameplayExited(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SoftBodyAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static SoftBody OnReady(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SoftBodyAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static SoftBody OnRenamed(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SoftBodyAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static SoftBody OnScriptChanged(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SoftBodyAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static SoftBody OnTreeEntered(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SoftBodyAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static SoftBody OnTreeExited(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SoftBodyAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static SoftBody OnTreeExiting(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SoftBodyAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static SoftBody OnVisibilityChanged(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SoftBodyAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Spatial OnGameplayEntered(this Spatial target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpatialAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static Spatial OnGameplayExited(this Spatial target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpatialAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static Spatial OnReady(this Spatial target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpatialAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Spatial OnRenamed(this Spatial target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpatialAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Spatial OnScriptChanged(this Spatial target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpatialAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Spatial OnTreeEntered(this Spatial target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpatialAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Spatial OnTreeExited(this Spatial target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpatialAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Spatial OnTreeExiting(this Spatial target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpatialAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Spatial OnVisibilityChanged(this Spatial target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpatialAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnChanged(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnChanged(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnDraw(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnFocusEntered(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnFocusExited(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnGuiInput(this SpinBox target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnHide(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnItemRectChanged(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnMinimumSizeChanged(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnModalClosed(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnMouseEntered(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnMouseExited(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnReady(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnRenamed(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnResized(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnScriptChanged(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnSizeFlagsChanged(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnTreeEntered(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnTreeExited(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnTreeExiting(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnValueChanged(this SpinBox target, Action<float> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnValueChanged(action, oneShot, deferred);
            return target;
        }

        public static SpinBox OnVisibilityChanged(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpinBoxAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static SpotLight OnGameplayEntered(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpotLightAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static SpotLight OnGameplayExited(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpotLightAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static SpotLight OnReady(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpotLightAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static SpotLight OnRenamed(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpotLightAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static SpotLight OnScriptChanged(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpotLightAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static SpotLight OnTreeEntered(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpotLightAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static SpotLight OnTreeExited(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpotLightAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static SpotLight OnTreeExiting(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpotLightAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static SpotLight OnVisibilityChanged(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpotLightAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static SpringArm OnGameplayEntered(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpringArmAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static SpringArm OnGameplayExited(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpringArmAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static SpringArm OnReady(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpringArmAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static SpringArm OnRenamed(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpringArmAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static SpringArm OnScriptChanged(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpringArmAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static SpringArm OnTreeEntered(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpringArmAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static SpringArm OnTreeExited(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpringArmAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static SpringArm OnTreeExiting(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpringArmAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static SpringArm OnVisibilityChanged(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpringArmAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Sprite OnDraw(this Sprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpriteAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Sprite OnFrameChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpriteAction>(target).OnFrameChanged(action, oneShot, deferred);
            return target;
        }

        public static Sprite OnHide(this Sprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpriteAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Sprite OnItemRectChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpriteAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Sprite OnReady(this Sprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpriteAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Sprite OnRenamed(this Sprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpriteAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Sprite OnScriptChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpriteAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Sprite OnTextureChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpriteAction>(target).OnTextureChanged(action, oneShot, deferred);
            return target;
        }

        public static Sprite OnTreeEntered(this Sprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpriteAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Sprite OnTreeExited(this Sprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpriteAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Sprite OnTreeExiting(this Sprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpriteAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Sprite OnVisibilityChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<SpriteAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Sprite3D OnFrameChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Sprite3DAction>(target).OnFrameChanged(action, oneShot, deferred);
            return target;
        }

        public static Sprite3D OnGameplayEntered(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Sprite3DAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static Sprite3D OnGameplayExited(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Sprite3DAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static Sprite3D OnReady(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Sprite3DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Sprite3D OnRenamed(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Sprite3DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Sprite3D OnScriptChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Sprite3DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Sprite3D OnTreeEntered(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Sprite3DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Sprite3D OnTreeExited(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Sprite3DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Sprite3D OnTreeExiting(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Sprite3DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Sprite3D OnVisibilityChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<Sprite3DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static StaticBody OnGameplayEntered(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBodyAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static StaticBody OnGameplayExited(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBodyAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static StaticBody OnInputEvent(this StaticBody target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBodyAction>(target).OnInputEvent(action, oneShot, deferred);
            return target;
        }

        public static StaticBody OnMouseEntered(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBodyAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static StaticBody OnMouseExited(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBodyAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static StaticBody OnReady(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBodyAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static StaticBody OnRenamed(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBodyAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static StaticBody OnScriptChanged(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBodyAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static StaticBody OnTreeEntered(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBodyAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static StaticBody OnTreeExited(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBodyAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static StaticBody OnTreeExiting(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBodyAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static StaticBody OnVisibilityChanged(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBodyAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static StaticBody2D OnDraw(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBody2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static StaticBody2D OnHide(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBody2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static StaticBody2D OnInputEvent(this StaticBody2D target, Action<InputEvent, int, Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBody2DAction>(target).OnInputEvent(action, oneShot, deferred);
            return target;
        }

        public static StaticBody2D OnItemRectChanged(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBody2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static StaticBody2D OnMouseEntered(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBody2DAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static StaticBody2D OnMouseExited(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBody2DAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static StaticBody2D OnReady(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBody2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static StaticBody2D OnRenamed(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBody2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static StaticBody2D OnScriptChanged(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBody2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static StaticBody2D OnTreeEntered(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBody2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static StaticBody2D OnTreeExited(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBody2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static StaticBody2D OnTreeExiting(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBody2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static StaticBody2D OnVisibilityChanged(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<StaticBody2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnDraw(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnFocusEntered(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnFocusExited(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnGuiInput(this TabContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnHide(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnItemRectChanged(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnMinimumSizeChanged(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnModalClosed(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnMouseEntered(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnMouseExited(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnPrePopupPressed(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnPrePopupPressed(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnReady(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnRenamed(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnResized(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnScriptChanged(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnSizeFlagsChanged(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnSortChildren(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnTabChanged(this TabContainer target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnTabChanged(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnTabSelected(this TabContainer target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnTabSelected(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnTreeEntered(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnTreeExited(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnTreeExiting(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static TabContainer OnVisibilityChanged(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnDraw(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnFocusEntered(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnFocusExited(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnGuiInput(this Tabs target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnHide(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnItemRectChanged(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnMinimumSizeChanged(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnModalClosed(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnMouseEntered(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnMouseExited(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnReady(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnRenamed(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnRepositionActiveTabRequest(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnRepositionActiveTabRequest(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnResized(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnRightButtonPressed(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnRightButtonPressed(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnScriptChanged(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnSizeFlagsChanged(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnTabChanged(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnTabChanged(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnTabClicked(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnTabClicked(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnTabClose(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnTabClose(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnTabHover(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnTabHover(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnTreeEntered(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnTreeExited(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnTreeExiting(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Tabs OnVisibilityChanged(this Tabs target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TabsAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnBreakpointToggled(this TextEdit target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnBreakpointToggled(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnCursorChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnCursorChanged(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnDraw(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnFocusEntered(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnFocusExited(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnGuiInput(this TextEdit target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnHide(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnInfoClicked(this TextEdit target, Action<string, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnInfoClicked(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnItemRectChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnMinimumSizeChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnModalClosed(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnMouseEntered(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnMouseExited(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnReady(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnRenamed(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnRequestCompletion(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnRequestCompletion(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnResized(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnScriptChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnSizeFlagsChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnSymbolLookup(this TextEdit target, Action<int, int, string> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnSymbolLookup(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnTextChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnTextChanged(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnTreeEntered(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnTreeExited(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnTreeExiting(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static TextEdit OnVisibilityChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextEditAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnButtonDown(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnButtonDown(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnButtonUp(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnButtonUp(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnDraw(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnFocusEntered(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnFocusExited(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnGuiInput(this TextureButton target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnHide(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnItemRectChanged(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnMinimumSizeChanged(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnModalClosed(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnMouseEntered(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnMouseExited(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnPressed(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnPressed(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnReady(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnRenamed(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnResized(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnScriptChanged(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnSizeFlagsChanged(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnToggled(this TextureButton target, Action<bool> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnToggled(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnTreeEntered(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnTreeExited(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnTreeExiting(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static TextureButton OnVisibilityChanged(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnChanged(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnDraw(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnFocusEntered(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnFocusExited(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnGuiInput(this TextureProgress target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnHide(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnItemRectChanged(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnMinimumSizeChanged(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnModalClosed(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnMouseEntered(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnMouseExited(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnReady(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnRenamed(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnResized(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnScriptChanged(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnSizeFlagsChanged(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnTreeEntered(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnTreeExited(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnTreeExiting(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnValueChanged(this TextureProgress target, Action<float> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnValueChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureProgress OnVisibilityChanged(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureProgressAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnDraw(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnFocusEntered(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnFocusExited(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnGuiInput(this TextureRect target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnHide(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnItemRectChanged(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnMinimumSizeChanged(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnModalClosed(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnMouseEntered(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnMouseExited(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnReady(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnRenamed(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnResized(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnScriptChanged(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnSizeFlagsChanged(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnTreeEntered(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnTreeExited(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnTreeExiting(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static TextureRect OnVisibilityChanged(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TextureRectAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static TileMap OnDraw(this TileMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TileMapAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static TileMap OnHide(this TileMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TileMapAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static TileMap OnItemRectChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TileMapAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static TileMap OnReady(this TileMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TileMapAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static TileMap OnRenamed(this TileMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TileMapAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static TileMap OnScriptChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TileMapAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static TileMap OnSettingsChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TileMapAction>(target).OnSettingsChanged(action, oneShot, deferred);
            return target;
        }

        public static TileMap OnTreeEntered(this TileMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TileMapAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static TileMap OnTreeExited(this TileMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TileMapAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static TileMap OnTreeExiting(this TileMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TileMapAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static TileMap OnVisibilityChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TileMapAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Timer OnReady(this Timer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TimerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Timer OnRenamed(this Timer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TimerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Timer OnScriptChanged(this Timer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TimerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Timer OnTimeout(this Timer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TimerAction>(target).OnTimeout(action, oneShot, deferred);
            return target;
        }

        public static Timer OnTreeEntered(this Timer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TimerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Timer OnTreeExited(this Timer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TimerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Timer OnTreeExiting(this Timer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TimerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnButtonDown(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnButtonDown(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnButtonUp(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnButtonUp(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnDraw(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnFocusEntered(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnFocusExited(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnGuiInput(this ToolButton target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnHide(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnItemRectChanged(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnMinimumSizeChanged(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnModalClosed(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnMouseEntered(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnMouseExited(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnPressed(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnPressed(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnReady(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnRenamed(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnResized(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnScriptChanged(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnSizeFlagsChanged(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnToggled(this ToolButton target, Action<bool> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnToggled(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnTreeEntered(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnTreeExited(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnTreeExiting(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ToolButton OnVisibilityChanged(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ToolButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static TouchScreenButton OnDraw(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static TouchScreenButton OnHide(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static TouchScreenButton OnItemRectChanged(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static TouchScreenButton OnPressed(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnPressed(action, oneShot, deferred);
            return target;
        }

        public static TouchScreenButton OnReady(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static TouchScreenButton OnReleased(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnReleased(action, oneShot, deferred);
            return target;
        }

        public static TouchScreenButton OnRenamed(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static TouchScreenButton OnScriptChanged(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static TouchScreenButton OnTreeEntered(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static TouchScreenButton OnTreeExited(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static TouchScreenButton OnTreeExiting(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static TouchScreenButton OnVisibilityChanged(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Tree OnButtonPressed(this Tree target, Action<int, int, TreeItem> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnButtonPressed(action, oneShot, deferred);
            return target;
        }

        public static Tree OnCellSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnCellSelected(action, oneShot, deferred);
            return target;
        }

        public static Tree OnColumnTitlePressed(this Tree target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnColumnTitlePressed(action, oneShot, deferred);
            return target;
        }

        public static Tree OnCustomPopupEdited(this Tree target, Action<bool> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnCustomPopupEdited(action, oneShot, deferred);
            return target;
        }

        public static Tree OnDraw(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static Tree OnEmptyRmb(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnEmptyRmb(action, oneShot, deferred);
            return target;
        }

        public static Tree OnEmptyTreeRmbSelected(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnEmptyTreeRmbSelected(action, oneShot, deferred);
            return target;
        }

        public static Tree OnFocusEntered(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static Tree OnFocusExited(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static Tree OnGuiInput(this Tree target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static Tree OnHide(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static Tree OnItemActivated(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnItemActivated(action, oneShot, deferred);
            return target;
        }

        public static Tree OnItemCollapsed(this Tree target, Action<TreeItem> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnItemCollapsed(action, oneShot, deferred);
            return target;
        }

        public static Tree OnItemCustomButtonPressed(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnItemCustomButtonPressed(action, oneShot, deferred);
            return target;
        }

        public static Tree OnItemDoubleClicked(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnItemDoubleClicked(action, oneShot, deferred);
            return target;
        }

        public static Tree OnItemEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnItemEdited(action, oneShot, deferred);
            return target;
        }

        public static Tree OnItemRectChanged(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static Tree OnItemRmbEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnItemRmbEdited(action, oneShot, deferred);
            return target;
        }

        public static Tree OnItemRmbSelected(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnItemRmbSelected(action, oneShot, deferred);
            return target;
        }

        public static Tree OnItemSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnItemSelected(action, oneShot, deferred);
            return target;
        }

        public static Tree OnMinimumSizeChanged(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static Tree OnModalClosed(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static Tree OnMouseEntered(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static Tree OnMouseExited(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static Tree OnMultiSelected(this Tree target, Action<int, TreeItem, bool> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnMultiSelected(action, oneShot, deferred);
            return target;
        }

        public static Tree OnNothingSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnNothingSelected(action, oneShot, deferred);
            return target;
        }

        public static Tree OnReady(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Tree OnRenamed(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Tree OnResized(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static Tree OnScriptChanged(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Tree OnSizeFlagsChanged(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static Tree OnTreeEntered(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Tree OnTreeExited(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Tree OnTreeExiting(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Tree OnVisibilityChanged(this Tree target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TreeAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Tween OnReady(this Tween target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TweenAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Tween OnRenamed(this Tween target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TweenAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Tween OnScriptChanged(this Tween target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TweenAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Tween OnTreeEntered(this Tween target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TweenAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Tween OnTreeExited(this Tween target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TweenAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Tween OnTreeExiting(this Tween target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TweenAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static Tween OnTweenAllCompleted(this Tween target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TweenAction>(target).OnTweenAllCompleted(action, oneShot, deferred);
            return target;
        }

        public static Tween OnTweenCompleted(this Tween target, Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TweenAction>(target).OnTweenCompleted(action, oneShot, deferred);
            return target;
        }

        public static Tween OnTweenStarted(this Tween target, Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TweenAction>(target).OnTweenStarted(action, oneShot, deferred);
            return target;
        }

        public static Tween OnTweenStep(this Tween target, Action<Object, float, NodePath, Object> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<TweenAction>(target).OnTweenStep(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnDraw(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnFocusEntered(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnFocusExited(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnGuiInput(this VBoxContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnHide(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnItemRectChanged(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnMinimumSizeChanged(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnModalClosed(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnMouseEntered(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnMouseExited(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnReady(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnRenamed(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnResized(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnScriptChanged(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnSizeFlagsChanged(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnSortChildren(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnTreeEntered(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnTreeExited(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnTreeExiting(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static VBoxContainer OnVisibilityChanged(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VBoxContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnBodyEntered(this VehicleBody target, Action<Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnBodyEntered(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnBodyExited(this VehicleBody target, Action<Node> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnBodyExited(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnBodyShapeEntered(this VehicleBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnBodyShapeEntered(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnBodyShapeExited(this VehicleBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnBodyShapeExited(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnGameplayEntered(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnGameplayExited(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnInputEvent(this VehicleBody target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnInputEvent(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnMouseEntered(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnMouseExited(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnReady(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnRenamed(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnScriptChanged(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnSleepingStateChanged(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnSleepingStateChanged(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnTreeEntered(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnTreeExited(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnTreeExiting(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static VehicleBody OnVisibilityChanged(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleBodyAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static VehicleWheel OnGameplayEntered(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleWheelAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static VehicleWheel OnGameplayExited(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleWheelAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static VehicleWheel OnReady(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleWheelAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static VehicleWheel OnRenamed(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleWheelAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static VehicleWheel OnScriptChanged(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleWheelAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static VehicleWheel OnTreeEntered(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleWheelAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static VehicleWheel OnTreeExited(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleWheelAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static VehicleWheel OnTreeExiting(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleWheelAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static VehicleWheel OnVisibilityChanged(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VehicleWheelAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnDraw(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnFinished(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnFinished(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnFocusEntered(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnFocusExited(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnGuiInput(this VideoPlayer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnHide(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnItemRectChanged(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnMinimumSizeChanged(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnModalClosed(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnMouseEntered(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnMouseExited(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnReady(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnRenamed(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnResized(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnScriptChanged(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnSizeFlagsChanged(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnTreeEntered(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnTreeExited(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnTreeExiting(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static VideoPlayer OnVisibilityChanged(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VideoPlayerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static Viewport OnGuiFocusChanged(this Viewport target, Action<Control> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportAction>(target).OnGuiFocusChanged(action, oneShot, deferred);
            return target;
        }

        public static Viewport OnReady(this Viewport target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static Viewport OnRenamed(this Viewport target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static Viewport OnScriptChanged(this Viewport target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static Viewport OnSizeChanged(this Viewport target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportAction>(target).OnSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static Viewport OnTreeEntered(this Viewport target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static Viewport OnTreeExited(this Viewport target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static Viewport OnTreeExiting(this Viewport target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnDraw(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnFocusEntered(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnFocusExited(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnGuiInput(this ViewportContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnHide(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnItemRectChanged(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnMinimumSizeChanged(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnModalClosed(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnMouseEntered(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnMouseExited(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnReady(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnRenamed(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnResized(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnScriptChanged(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnSizeFlagsChanged(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnSortChildren(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnTreeEntered(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnTreeExited(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnTreeExiting(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static ViewportContainer OnVisibilityChanged(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<ViewportContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler OnCameraEntered(this VisibilityEnabler target, Action<Camera> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnCameraEntered(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler OnCameraExited(this VisibilityEnabler target, Action<Camera> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnCameraExited(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler OnGameplayEntered(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler OnGameplayExited(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler OnReady(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler OnRenamed(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler OnScreenEntered(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnScreenEntered(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler OnScreenExited(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnScreenExited(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler OnScriptChanged(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler OnTreeEntered(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler OnTreeExited(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler OnTreeExiting(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler OnVisibilityChanged(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler2D OnDraw(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler2D OnHide(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler2D OnItemRectChanged(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler2D OnReady(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler2D OnRenamed(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler2D OnScreenEntered(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnScreenEntered(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler2D OnScreenExited(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnScreenExited(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler2D OnScriptChanged(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler2D OnTreeEntered(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler2D OnTreeExited(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler2D OnTreeExiting(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler2D OnViewportEntered(this VisibilityEnabler2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnViewportEntered(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler2D OnViewportExited(this VisibilityEnabler2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnViewportExited(action, oneShot, deferred);
            return target;
        }

        public static VisibilityEnabler2D OnVisibilityChanged(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier OnCameraEntered(this VisibilityNotifier target, Action<Camera> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnCameraEntered(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier OnCameraExited(this VisibilityNotifier target, Action<Camera> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnCameraExited(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier OnGameplayEntered(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnGameplayEntered(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier OnGameplayExited(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnGameplayExited(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier OnReady(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier OnRenamed(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier OnScreenEntered(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnScreenEntered(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier OnScreenExited(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnScreenExited(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier OnScriptChanged(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier OnTreeEntered(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier OnTreeExited(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier OnTreeExiting(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier OnVisibilityChanged(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier2D OnDraw(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier2D OnHide(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier2D OnItemRectChanged(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier2D OnReady(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier2D OnRenamed(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier2D OnScreenEntered(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnScreenEntered(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier2D OnScreenExited(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnScreenExited(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier2D OnScriptChanged(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier2D OnTreeEntered(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier2D OnTreeExited(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier2D OnTreeExiting(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier2D OnViewportEntered(this VisibilityNotifier2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnViewportEntered(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier2D OnViewportExited(this VisibilityNotifier2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnViewportExited(action, oneShot, deferred);
            return target;
        }

        public static VisibilityNotifier2D OnVisibilityChanged(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnChanged(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnChanged(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnDraw(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnFocusEntered(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnFocusExited(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnGuiInput(this VScrollBar target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnHide(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnItemRectChanged(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnMinimumSizeChanged(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnModalClosed(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnMouseEntered(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnMouseExited(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnReady(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnRenamed(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnResized(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnScriptChanged(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnScrolling(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnScrolling(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnSizeFlagsChanged(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnTreeEntered(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnTreeExited(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnTreeExiting(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnValueChanged(this VScrollBar target, Action<float> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnValueChanged(action, oneShot, deferred);
            return target;
        }

        public static VScrollBar OnVisibilityChanged(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VScrollBarAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnDraw(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnFocusEntered(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnFocusExited(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnGuiInput(this VSeparator target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnHide(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnItemRectChanged(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnMinimumSizeChanged(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnModalClosed(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnMouseEntered(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnMouseExited(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnReady(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnRenamed(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnResized(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnScriptChanged(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnSizeFlagsChanged(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnTreeEntered(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnTreeExited(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnTreeExiting(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static VSeparator OnVisibilityChanged(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSeparatorAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnChanged(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnChanged(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnDraw(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnFocusEntered(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnFocusExited(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnGuiInput(this VSlider target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnHide(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnItemRectChanged(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnMinimumSizeChanged(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnModalClosed(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnMouseEntered(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnMouseExited(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnReady(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnRenamed(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnResized(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnScriptChanged(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnSizeFlagsChanged(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnTreeEntered(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnTreeExited(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnTreeExiting(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnValueChanged(this VSlider target, Action<float> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnValueChanged(action, oneShot, deferred);
            return target;
        }

        public static VSlider OnVisibilityChanged(this VSlider target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSliderAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnDragged(this VSplitContainer target, Action<int> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnDragged(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnDraw(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnFocusEntered(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnFocusExited(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnGuiInput(this VSplitContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnHide(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnItemRectChanged(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnMinimumSizeChanged(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnModalClosed(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnMouseEntered(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnMouseExited(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnReady(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnRenamed(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnResized(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnScriptChanged(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnSizeFlagsChanged(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnSortChildren(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnSortChildren(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnTreeEntered(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnTreeExited(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnTreeExiting(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static VSplitContainer OnVisibilityChanged(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<VSplitContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnAboutToShow(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnAboutToShow(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnDraw(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnFocusEntered(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnFocusEntered(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnFocusExited(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnFocusExited(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnGuiInput(this WindowDialog target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnGuiInput(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnHide(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnItemRectChanged(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnMinimumSizeChanged(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnModalClosed(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnModalClosed(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnMouseEntered(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnMouseEntered(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnMouseExited(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnMouseExited(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnPopupHide(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnPopupHide(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnReady(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnRenamed(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnResized(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnResized(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnScriptChanged(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnSizeFlagsChanged(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnTreeEntered(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnTreeExited(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnTreeExiting(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static WindowDialog OnVisibilityChanged(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WindowDialogAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }

        public static WorldEnvironment OnReady(this WorldEnvironment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WorldEnvironmentAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static WorldEnvironment OnRenamed(this WorldEnvironment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WorldEnvironmentAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static WorldEnvironment OnScriptChanged(this WorldEnvironment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WorldEnvironmentAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static WorldEnvironment OnTreeEntered(this WorldEnvironment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WorldEnvironmentAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static WorldEnvironment OnTreeExited(this WorldEnvironment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WorldEnvironmentAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static WorldEnvironment OnTreeExiting(this WorldEnvironment target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<WorldEnvironmentAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static YSort OnDraw(this YSort target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<YSortAction>(target).OnDraw(action, oneShot, deferred);
            return target;
        }

        public static YSort OnHide(this YSort target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<YSortAction>(target).OnHide(action, oneShot, deferred);
            return target;
        }

        public static YSort OnItemRectChanged(this YSort target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<YSortAction>(target).OnItemRectChanged(action, oneShot, deferred);
            return target;
        }

        public static YSort OnReady(this YSort target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<YSortAction>(target).OnReady(action, oneShot, deferred);
            return target;
        }

        public static YSort OnRenamed(this YSort target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<YSortAction>(target).OnRenamed(action, oneShot, deferred);
            return target;
        }

        public static YSort OnScriptChanged(this YSort target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<YSortAction>(target).OnScriptChanged(action, oneShot, deferred);
            return target;
        }

        public static YSort OnTreeEntered(this YSort target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<YSortAction>(target).OnTreeEntered(action, oneShot, deferred);
            return target;
        }

        public static YSort OnTreeExited(this YSort target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<YSortAction>(target).OnTreeExited(action, oneShot, deferred);
            return target;
        }

        public static YSort OnTreeExiting(this YSort target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<YSortAction>(target).OnTreeExiting(action, oneShot, deferred);
            return target;
        }

        public static YSort OnVisibilityChanged(this YSort target, Action action, bool oneShot = false, bool deferred = false) {
            NodeAction.GetProxy<YSortAction>(target).OnVisibilityChanged(action, oneShot, deferred);
            return target;
        }
    }
}