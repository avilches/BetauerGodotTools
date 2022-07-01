using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;
using Environment = Godot.Environment;
using Betauer.GodotAction;

namespace Betauer {
    public static partial class GodotActionExtensions {

        public static AcceptDialogAction OnAboutToShow(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnAboutToShow(action, oneShot, deferred);

        public static AcceptDialogAction OnConfirmed(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnConfirmed(action, oneShot, deferred);

        public static AcceptDialogAction OnCustomAction(this AcceptDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnCustomAction(action, oneShot, deferred);

        public static AcceptDialogAction OnDraw(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnDraw(action, oneShot, deferred);

        public static AcceptDialogAction OnFocusEntered(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static AcceptDialogAction OnFocusExited(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnFocusExited(action, oneShot, deferred);

        public static AcceptDialogAction OnGuiInput(this AcceptDialog target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnGuiInput(action, oneShot, deferred);

        public static AcceptDialogAction OnHide(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnHide(action, oneShot, deferred);

        public static AcceptDialogAction OnItemRectChanged(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static AcceptDialogAction OnMinimumSizeChanged(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static AcceptDialogAction OnModalClosed(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnModalClosed(action, oneShot, deferred);

        public static AcceptDialogAction OnMouseEntered(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static AcceptDialogAction OnMouseExited(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnMouseExited(action, oneShot, deferred);

        public static AcceptDialogAction OnPopupHide(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnPopupHide(action, oneShot, deferred);

        public static AcceptDialogAction OnReady(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnReady(action, oneShot, deferred);

        public static AcceptDialogAction OnRenamed(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnRenamed(action, oneShot, deferred);

        public static AcceptDialogAction OnResized(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnResized(action, oneShot, deferred);

        public static AcceptDialogAction OnScriptChanged(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static AcceptDialogAction OnSizeFlagsChanged(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static AcceptDialogAction OnTreeEntered(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static AcceptDialogAction OnTreeExited(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnTreeExited(action, oneShot, deferred);

        public static AcceptDialogAction OnTreeExiting(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static AcceptDialogAction OnVisibilityChanged(this AcceptDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AcceptDialogAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static AnimatedSpriteAction OnAnimationFinished(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnAnimationFinished(action, oneShot, deferred);

        public static AnimatedSpriteAction OnDraw(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnDraw(action, oneShot, deferred);

        public static AnimatedSpriteAction OnFrameChanged(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnFrameChanged(action, oneShot, deferred);

        public static AnimatedSpriteAction OnHide(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnHide(action, oneShot, deferred);

        public static AnimatedSpriteAction OnItemRectChanged(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static AnimatedSpriteAction OnReady(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnReady(action, oneShot, deferred);

        public static AnimatedSpriteAction OnRenamed(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnRenamed(action, oneShot, deferred);

        public static AnimatedSpriteAction OnScriptChanged(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static AnimatedSpriteAction OnTreeEntered(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static AnimatedSpriteAction OnTreeExited(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnTreeExited(action, oneShot, deferred);

        public static AnimatedSpriteAction OnTreeExiting(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static AnimatedSpriteAction OnVisibilityChanged(this AnimatedSprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSpriteAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static AnimatedSprite3DAction OnAnimationFinished(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnAnimationFinished(action, oneShot, deferred);

        public static AnimatedSprite3DAction OnFrameChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnFrameChanged(action, oneShot, deferred);

        public static AnimatedSprite3DAction OnGameplayEntered(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static AnimatedSprite3DAction OnGameplayExited(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static AnimatedSprite3DAction OnReady(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnReady(action, oneShot, deferred);

        public static AnimatedSprite3DAction OnRenamed(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnRenamed(action, oneShot, deferred);

        public static AnimatedSprite3DAction OnScriptChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static AnimatedSprite3DAction OnTreeEntered(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static AnimatedSprite3DAction OnTreeExited(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static AnimatedSprite3DAction OnTreeExiting(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static AnimatedSprite3DAction OnVisibilityChanged(this AnimatedSprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimatedSprite3DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static AnimationPlayerAction OnAnimationChanged(this AnimationPlayer target, Action<string, string> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnAnimationChanged(action, oneShot, deferred);

        public static AnimationPlayerAction OnAnimationFinished(this AnimationPlayer target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnAnimationFinished(action, oneShot, deferred);

        public static AnimationPlayerAction OnAnimationStarted(this AnimationPlayer target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnAnimationStarted(action, oneShot, deferred);

        public static AnimationPlayerAction OnCachesCleared(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnCachesCleared(action, oneShot, deferred);

        public static AnimationPlayerAction OnReady(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnReady(action, oneShot, deferred);

        public static AnimationPlayerAction OnRenamed(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnRenamed(action, oneShot, deferred);

        public static AnimationPlayerAction OnScriptChanged(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static AnimationPlayerAction OnTreeEntered(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static AnimationPlayerAction OnTreeExited(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static AnimationPlayerAction OnTreeExiting(this AnimationPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationPlayerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static AnimationTreeAction OnReady(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationTreeAction>(target).OnReady(action, oneShot, deferred);

        public static AnimationTreeAction OnRenamed(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationTreeAction>(target).OnRenamed(action, oneShot, deferred);

        public static AnimationTreeAction OnScriptChanged(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationTreeAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static AnimationTreeAction OnTreeEntered(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationTreeAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static AnimationTreeAction OnTreeExited(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationTreeAction>(target).OnTreeExited(action, oneShot, deferred);

        public static AnimationTreeAction OnTreeExiting(this AnimationTree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationTreeAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static AnimationTreePlayerAction OnReady(this AnimationTreePlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationTreePlayerAction>(target).OnReady(action, oneShot, deferred);

        public static AnimationTreePlayerAction OnRenamed(this AnimationTreePlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationTreePlayerAction>(target).OnRenamed(action, oneShot, deferred);

        public static AnimationTreePlayerAction OnScriptChanged(this AnimationTreePlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationTreePlayerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static AnimationTreePlayerAction OnTreeEntered(this AnimationTreePlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationTreePlayerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static AnimationTreePlayerAction OnTreeExited(this AnimationTreePlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationTreePlayerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static AnimationTreePlayerAction OnTreeExiting(this AnimationTreePlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AnimationTreePlayerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static AreaAction OnAreaEntered(this Area target, Action<Area> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnAreaEntered(action, oneShot, deferred);

        public static AreaAction OnAreaExited(this Area target, Action<Area> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnAreaExited(action, oneShot, deferred);

        public static AreaAction OnAreaShapeEntered(this Area target, Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnAreaShapeEntered(action, oneShot, deferred);

        public static AreaAction OnAreaShapeExited(this Area target, Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnAreaShapeExited(action, oneShot, deferred);

        public static AreaAction OnBodyEntered(this Area target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnBodyEntered(action, oneShot, deferred);

        public static AreaAction OnBodyExited(this Area target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnBodyExited(action, oneShot, deferred);

        public static AreaAction OnBodyShapeEntered(this Area target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnBodyShapeEntered(action, oneShot, deferred);

        public static AreaAction OnBodyShapeExited(this Area target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnBodyShapeExited(action, oneShot, deferred);

        public static AreaAction OnGameplayEntered(this Area target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static AreaAction OnGameplayExited(this Area target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static AreaAction OnInputEvent(this Area target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnInputEvent(action, oneShot, deferred);

        public static AreaAction OnMouseEntered(this Area target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static AreaAction OnMouseExited(this Area target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnMouseExited(action, oneShot, deferred);

        public static AreaAction OnReady(this Area target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnReady(action, oneShot, deferred);

        public static AreaAction OnRenamed(this Area target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnRenamed(action, oneShot, deferred);

        public static AreaAction OnScriptChanged(this Area target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static AreaAction OnTreeEntered(this Area target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static AreaAction OnTreeExited(this Area target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnTreeExited(action, oneShot, deferred);

        public static AreaAction OnTreeExiting(this Area target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static AreaAction OnVisibilityChanged(this Area target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AreaAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Area2DAction OnAreaEntered(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnAreaEntered(action, oneShot, deferred);

        public static Area2DAction OnAreaExited(this Area2D target, Action<Area2D> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnAreaExited(action, oneShot, deferred);

        public static Area2DAction OnAreaShapeEntered(this Area2D target, Action<Area2D, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnAreaShapeEntered(action, oneShot, deferred);

        public static Area2DAction OnAreaShapeExited(this Area2D target, Action<Area2D, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnAreaShapeExited(action, oneShot, deferred);

        public static Area2DAction OnBodyEntered(this Area2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnBodyEntered(action, oneShot, deferred);

        public static Area2DAction OnBodyExited(this Area2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnBodyExited(action, oneShot, deferred);

        public static Area2DAction OnBodyShapeEntered(this Area2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnBodyShapeEntered(action, oneShot, deferred);

        public static Area2DAction OnBodyShapeExited(this Area2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnBodyShapeExited(action, oneShot, deferred);

        public static Area2DAction OnDraw(this Area2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnDraw(action, oneShot, deferred);

        public static Area2DAction OnHide(this Area2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnHide(action, oneShot, deferred);

        public static Area2DAction OnInputEvent(this Area2D target, Action<InputEvent, int, Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnInputEvent(action, oneShot, deferred);

        public static Area2DAction OnItemRectChanged(this Area2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static Area2DAction OnMouseEntered(this Area2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static Area2DAction OnMouseExited(this Area2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnMouseExited(action, oneShot, deferred);

        public static Area2DAction OnReady(this Area2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnReady(action, oneShot, deferred);

        public static Area2DAction OnRenamed(this Area2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Area2DAction OnScriptChanged(this Area2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Area2DAction OnTreeEntered(this Area2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Area2DAction OnTreeExited(this Area2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Area2DAction OnTreeExiting(this Area2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Area2DAction OnVisibilityChanged(this Area2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Area2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ARVRAnchorAction OnGameplayEntered(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static ARVRAnchorAction OnGameplayExited(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static ARVRAnchorAction OnMeshUpdated(this ARVRAnchor target, Action<Mesh> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnMeshUpdated(action, oneShot, deferred);

        public static ARVRAnchorAction OnReady(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnReady(action, oneShot, deferred);

        public static ARVRAnchorAction OnRenamed(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnRenamed(action, oneShot, deferred);

        public static ARVRAnchorAction OnScriptChanged(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ARVRAnchorAction OnTreeEntered(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ARVRAnchorAction OnTreeExited(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ARVRAnchorAction OnTreeExiting(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ARVRAnchorAction OnVisibilityChanged(this ARVRAnchor target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRAnchorAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ARVRCameraAction OnGameplayEntered(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRCameraAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static ARVRCameraAction OnGameplayExited(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRCameraAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static ARVRCameraAction OnReady(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRCameraAction>(target).OnReady(action, oneShot, deferred);

        public static ARVRCameraAction OnRenamed(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRCameraAction>(target).OnRenamed(action, oneShot, deferred);

        public static ARVRCameraAction OnScriptChanged(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRCameraAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ARVRCameraAction OnTreeEntered(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRCameraAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ARVRCameraAction OnTreeExited(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRCameraAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ARVRCameraAction OnTreeExiting(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRCameraAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ARVRCameraAction OnVisibilityChanged(this ARVRCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRCameraAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ARVRControllerAction OnButtonPressed(this ARVRController target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRControllerAction>(target).OnButtonPressed(action, oneShot, deferred);

        public static ARVRControllerAction OnButtonRelease(this ARVRController target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRControllerAction>(target).OnButtonRelease(action, oneShot, deferred);

        public static ARVRControllerAction OnGameplayEntered(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRControllerAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static ARVRControllerAction OnGameplayExited(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRControllerAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static ARVRControllerAction OnMeshUpdated(this ARVRController target, Action<Mesh> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRControllerAction>(target).OnMeshUpdated(action, oneShot, deferred);

        public static ARVRControllerAction OnReady(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRControllerAction>(target).OnReady(action, oneShot, deferred);

        public static ARVRControllerAction OnRenamed(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRControllerAction>(target).OnRenamed(action, oneShot, deferred);

        public static ARVRControllerAction OnScriptChanged(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRControllerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ARVRControllerAction OnTreeEntered(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRControllerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ARVRControllerAction OnTreeExited(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRControllerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ARVRControllerAction OnTreeExiting(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRControllerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ARVRControllerAction OnVisibilityChanged(this ARVRController target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVRControllerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ARVROriginAction OnGameplayEntered(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVROriginAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static ARVROriginAction OnGameplayExited(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVROriginAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static ARVROriginAction OnReady(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVROriginAction>(target).OnReady(action, oneShot, deferred);

        public static ARVROriginAction OnRenamed(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVROriginAction>(target).OnRenamed(action, oneShot, deferred);

        public static ARVROriginAction OnScriptChanged(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVROriginAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ARVROriginAction OnTreeEntered(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVROriginAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ARVROriginAction OnTreeExited(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVROriginAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ARVROriginAction OnTreeExiting(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVROriginAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ARVROriginAction OnVisibilityChanged(this ARVROrigin target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ARVROriginAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static AspectRatioContainerAction OnDraw(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnDraw(action, oneShot, deferred);

        public static AspectRatioContainerAction OnFocusEntered(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static AspectRatioContainerAction OnFocusExited(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static AspectRatioContainerAction OnGuiInput(this AspectRatioContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static AspectRatioContainerAction OnHide(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnHide(action, oneShot, deferred);

        public static AspectRatioContainerAction OnItemRectChanged(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static AspectRatioContainerAction OnMinimumSizeChanged(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static AspectRatioContainerAction OnModalClosed(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static AspectRatioContainerAction OnMouseEntered(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static AspectRatioContainerAction OnMouseExited(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static AspectRatioContainerAction OnReady(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnReady(action, oneShot, deferred);

        public static AspectRatioContainerAction OnRenamed(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnRenamed(action, oneShot, deferred);

        public static AspectRatioContainerAction OnResized(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnResized(action, oneShot, deferred);

        public static AspectRatioContainerAction OnScriptChanged(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static AspectRatioContainerAction OnSizeFlagsChanged(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static AspectRatioContainerAction OnSortChildren(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnSortChildren(action, oneShot, deferred);

        public static AspectRatioContainerAction OnTreeEntered(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static AspectRatioContainerAction OnTreeExited(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static AspectRatioContainerAction OnTreeExiting(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static AspectRatioContainerAction OnVisibilityChanged(this AspectRatioContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AspectRatioContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static AudioStreamPlayerAction OnFinished(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayerAction>(target).OnFinished(action, oneShot, deferred);

        public static AudioStreamPlayerAction OnReady(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayerAction>(target).OnReady(action, oneShot, deferred);

        public static AudioStreamPlayerAction OnRenamed(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayerAction>(target).OnRenamed(action, oneShot, deferred);

        public static AudioStreamPlayerAction OnScriptChanged(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static AudioStreamPlayerAction OnTreeEntered(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static AudioStreamPlayerAction OnTreeExited(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static AudioStreamPlayerAction OnTreeExiting(this AudioStreamPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static AudioStreamPlayer2DAction OnDraw(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnDraw(action, oneShot, deferred);

        public static AudioStreamPlayer2DAction OnFinished(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnFinished(action, oneShot, deferred);

        public static AudioStreamPlayer2DAction OnHide(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnHide(action, oneShot, deferred);

        public static AudioStreamPlayer2DAction OnItemRectChanged(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static AudioStreamPlayer2DAction OnReady(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnReady(action, oneShot, deferred);

        public static AudioStreamPlayer2DAction OnRenamed(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static AudioStreamPlayer2DAction OnScriptChanged(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static AudioStreamPlayer2DAction OnTreeEntered(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static AudioStreamPlayer2DAction OnTreeExited(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static AudioStreamPlayer2DAction OnTreeExiting(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static AudioStreamPlayer2DAction OnVisibilityChanged(this AudioStreamPlayer2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static AudioStreamPlayer3DAction OnFinished(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnFinished(action, oneShot, deferred);

        public static AudioStreamPlayer3DAction OnGameplayEntered(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static AudioStreamPlayer3DAction OnGameplayExited(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static AudioStreamPlayer3DAction OnReady(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnReady(action, oneShot, deferred);

        public static AudioStreamPlayer3DAction OnRenamed(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnRenamed(action, oneShot, deferred);

        public static AudioStreamPlayer3DAction OnScriptChanged(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static AudioStreamPlayer3DAction OnTreeEntered(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static AudioStreamPlayer3DAction OnTreeExited(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static AudioStreamPlayer3DAction OnTreeExiting(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static AudioStreamPlayer3DAction OnVisibilityChanged(this AudioStreamPlayer3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<AudioStreamPlayer3DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static BackBufferCopyAction OnDraw(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnDraw(action, oneShot, deferred);

        public static BackBufferCopyAction OnHide(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnHide(action, oneShot, deferred);

        public static BackBufferCopyAction OnItemRectChanged(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static BackBufferCopyAction OnReady(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnReady(action, oneShot, deferred);

        public static BackBufferCopyAction OnRenamed(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnRenamed(action, oneShot, deferred);

        public static BackBufferCopyAction OnScriptChanged(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static BackBufferCopyAction OnTreeEntered(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static BackBufferCopyAction OnTreeExited(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnTreeExited(action, oneShot, deferred);

        public static BackBufferCopyAction OnTreeExiting(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static BackBufferCopyAction OnVisibilityChanged(this BackBufferCopy target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BackBufferCopyAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static BakedLightmapAction OnGameplayEntered(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BakedLightmapAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static BakedLightmapAction OnGameplayExited(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BakedLightmapAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static BakedLightmapAction OnReady(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BakedLightmapAction>(target).OnReady(action, oneShot, deferred);

        public static BakedLightmapAction OnRenamed(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BakedLightmapAction>(target).OnRenamed(action, oneShot, deferred);

        public static BakedLightmapAction OnScriptChanged(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BakedLightmapAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static BakedLightmapAction OnTreeEntered(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BakedLightmapAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static BakedLightmapAction OnTreeExited(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BakedLightmapAction>(target).OnTreeExited(action, oneShot, deferred);

        public static BakedLightmapAction OnTreeExiting(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BakedLightmapAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static BakedLightmapAction OnVisibilityChanged(this BakedLightmap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BakedLightmapAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Bone2DAction OnDraw(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Bone2DAction>(target).OnDraw(action, oneShot, deferred);

        public static Bone2DAction OnHide(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Bone2DAction>(target).OnHide(action, oneShot, deferred);

        public static Bone2DAction OnItemRectChanged(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Bone2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static Bone2DAction OnReady(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Bone2DAction>(target).OnReady(action, oneShot, deferred);

        public static Bone2DAction OnRenamed(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Bone2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Bone2DAction OnScriptChanged(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Bone2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Bone2DAction OnTreeEntered(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Bone2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Bone2DAction OnTreeExited(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Bone2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Bone2DAction OnTreeExiting(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Bone2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Bone2DAction OnVisibilityChanged(this Bone2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Bone2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static BoneAttachmentAction OnGameplayEntered(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static BoneAttachmentAction OnGameplayExited(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static BoneAttachmentAction OnReady(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnReady(action, oneShot, deferred);

        public static BoneAttachmentAction OnRenamed(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnRenamed(action, oneShot, deferred);

        public static BoneAttachmentAction OnScriptChanged(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static BoneAttachmentAction OnTreeEntered(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static BoneAttachmentAction OnTreeExited(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnTreeExited(action, oneShot, deferred);

        public static BoneAttachmentAction OnTreeExiting(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static BoneAttachmentAction OnVisibilityChanged(this BoneAttachment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<BoneAttachmentAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ButtonAction OnButtonDown(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnButtonDown(action, oneShot, deferred);

        public static ButtonAction OnButtonUp(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnButtonUp(action, oneShot, deferred);

        public static ButtonAction OnDraw(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnDraw(action, oneShot, deferred);

        public static ButtonAction OnFocusEntered(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static ButtonAction OnFocusExited(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnFocusExited(action, oneShot, deferred);

        public static ButtonAction OnGuiInput(this Button target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnGuiInput(action, oneShot, deferred);

        public static ButtonAction OnHide(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnHide(action, oneShot, deferred);

        public static ButtonAction OnItemRectChanged(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static ButtonAction OnMinimumSizeChanged(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static ButtonAction OnModalClosed(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnModalClosed(action, oneShot, deferred);

        public static ButtonAction OnMouseEntered(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static ButtonAction OnMouseExited(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnMouseExited(action, oneShot, deferred);

        public static ButtonAction OnPressed(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnPressed(action, oneShot, deferred);

        public static ButtonAction OnReady(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnReady(action, oneShot, deferred);

        public static ButtonAction OnRenamed(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnRenamed(action, oneShot, deferred);

        public static ButtonAction OnResized(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnResized(action, oneShot, deferred);

        public static ButtonAction OnScriptChanged(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ButtonAction OnSizeFlagsChanged(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static ButtonAction OnToggled(this Button target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnToggled(action, oneShot, deferred);

        public static ButtonAction OnTreeEntered(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ButtonAction OnTreeExited(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ButtonAction OnTreeExiting(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ButtonAction OnVisibilityChanged(this Button target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CameraAction OnGameplayEntered(this Camera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CameraAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static CameraAction OnGameplayExited(this Camera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CameraAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static CameraAction OnReady(this Camera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CameraAction>(target).OnReady(action, oneShot, deferred);

        public static CameraAction OnRenamed(this Camera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CameraAction>(target).OnRenamed(action, oneShot, deferred);

        public static CameraAction OnScriptChanged(this Camera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CameraAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CameraAction OnTreeEntered(this Camera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CameraAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CameraAction OnTreeExited(this Camera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CameraAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CameraAction OnTreeExiting(this Camera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CameraAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CameraAction OnVisibilityChanged(this Camera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CameraAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Camera2DAction OnDraw(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Camera2DAction>(target).OnDraw(action, oneShot, deferred);

        public static Camera2DAction OnHide(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Camera2DAction>(target).OnHide(action, oneShot, deferred);

        public static Camera2DAction OnItemRectChanged(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Camera2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static Camera2DAction OnReady(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Camera2DAction>(target).OnReady(action, oneShot, deferred);

        public static Camera2DAction OnRenamed(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Camera2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Camera2DAction OnScriptChanged(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Camera2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Camera2DAction OnTreeEntered(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Camera2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Camera2DAction OnTreeExited(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Camera2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Camera2DAction OnTreeExiting(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Camera2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Camera2DAction OnVisibilityChanged(this Camera2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Camera2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CanvasLayerAction OnReady(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasLayerAction>(target).OnReady(action, oneShot, deferred);

        public static CanvasLayerAction OnRenamed(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasLayerAction>(target).OnRenamed(action, oneShot, deferred);

        public static CanvasLayerAction OnScriptChanged(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasLayerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CanvasLayerAction OnTreeEntered(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasLayerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CanvasLayerAction OnTreeExited(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasLayerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CanvasLayerAction OnTreeExiting(this CanvasLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasLayerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CanvasModulateAction OnDraw(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasModulateAction>(target).OnDraw(action, oneShot, deferred);

        public static CanvasModulateAction OnHide(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasModulateAction>(target).OnHide(action, oneShot, deferred);

        public static CanvasModulateAction OnItemRectChanged(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasModulateAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static CanvasModulateAction OnReady(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasModulateAction>(target).OnReady(action, oneShot, deferred);

        public static CanvasModulateAction OnRenamed(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasModulateAction>(target).OnRenamed(action, oneShot, deferred);

        public static CanvasModulateAction OnScriptChanged(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasModulateAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CanvasModulateAction OnTreeEntered(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasModulateAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CanvasModulateAction OnTreeExited(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasModulateAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CanvasModulateAction OnTreeExiting(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasModulateAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CanvasModulateAction OnVisibilityChanged(this CanvasModulate target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CanvasModulateAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CenterContainerAction OnDraw(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnDraw(action, oneShot, deferred);

        public static CenterContainerAction OnFocusEntered(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static CenterContainerAction OnFocusExited(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static CenterContainerAction OnGuiInput(this CenterContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static CenterContainerAction OnHide(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnHide(action, oneShot, deferred);

        public static CenterContainerAction OnItemRectChanged(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static CenterContainerAction OnMinimumSizeChanged(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static CenterContainerAction OnModalClosed(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static CenterContainerAction OnMouseEntered(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static CenterContainerAction OnMouseExited(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static CenterContainerAction OnReady(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnReady(action, oneShot, deferred);

        public static CenterContainerAction OnRenamed(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnRenamed(action, oneShot, deferred);

        public static CenterContainerAction OnResized(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnResized(action, oneShot, deferred);

        public static CenterContainerAction OnScriptChanged(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CenterContainerAction OnSizeFlagsChanged(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static CenterContainerAction OnSortChildren(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnSortChildren(action, oneShot, deferred);

        public static CenterContainerAction OnTreeEntered(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CenterContainerAction OnTreeExited(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CenterContainerAction OnTreeExiting(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CenterContainerAction OnVisibilityChanged(this CenterContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CenterContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CheckBoxAction OnButtonDown(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnButtonDown(action, oneShot, deferred);

        public static CheckBoxAction OnButtonUp(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnButtonUp(action, oneShot, deferred);

        public static CheckBoxAction OnDraw(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnDraw(action, oneShot, deferred);

        public static CheckBoxAction OnFocusEntered(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static CheckBoxAction OnFocusExited(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnFocusExited(action, oneShot, deferred);

        public static CheckBoxAction OnGuiInput(this CheckBox target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnGuiInput(action, oneShot, deferred);

        public static CheckBoxAction OnHide(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnHide(action, oneShot, deferred);

        public static CheckBoxAction OnItemRectChanged(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static CheckBoxAction OnMinimumSizeChanged(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static CheckBoxAction OnModalClosed(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnModalClosed(action, oneShot, deferred);

        public static CheckBoxAction OnMouseEntered(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static CheckBoxAction OnMouseExited(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnMouseExited(action, oneShot, deferred);

        public static CheckBoxAction OnPressed(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnPressed(action, oneShot, deferred);

        public static CheckBoxAction OnReady(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnReady(action, oneShot, deferred);

        public static CheckBoxAction OnRenamed(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnRenamed(action, oneShot, deferred);

        public static CheckBoxAction OnResized(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnResized(action, oneShot, deferred);

        public static CheckBoxAction OnScriptChanged(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CheckBoxAction OnSizeFlagsChanged(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static CheckBoxAction OnToggled(this CheckBox target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnToggled(action, oneShot, deferred);

        public static CheckBoxAction OnTreeEntered(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CheckBoxAction OnTreeExited(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CheckBoxAction OnTreeExiting(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CheckBoxAction OnVisibilityChanged(this CheckBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckBoxAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CheckButtonAction OnButtonDown(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnButtonDown(action, oneShot, deferred);

        public static CheckButtonAction OnButtonUp(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnButtonUp(action, oneShot, deferred);

        public static CheckButtonAction OnDraw(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnDraw(action, oneShot, deferred);

        public static CheckButtonAction OnFocusEntered(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static CheckButtonAction OnFocusExited(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnFocusExited(action, oneShot, deferred);

        public static CheckButtonAction OnGuiInput(this CheckButton target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnGuiInput(action, oneShot, deferred);

        public static CheckButtonAction OnHide(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnHide(action, oneShot, deferred);

        public static CheckButtonAction OnItemRectChanged(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static CheckButtonAction OnMinimumSizeChanged(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static CheckButtonAction OnModalClosed(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnModalClosed(action, oneShot, deferred);

        public static CheckButtonAction OnMouseEntered(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static CheckButtonAction OnMouseExited(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnMouseExited(action, oneShot, deferred);

        public static CheckButtonAction OnPressed(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnPressed(action, oneShot, deferred);

        public static CheckButtonAction OnReady(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnReady(action, oneShot, deferred);

        public static CheckButtonAction OnRenamed(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnRenamed(action, oneShot, deferred);

        public static CheckButtonAction OnResized(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnResized(action, oneShot, deferred);

        public static CheckButtonAction OnScriptChanged(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CheckButtonAction OnSizeFlagsChanged(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static CheckButtonAction OnToggled(this CheckButton target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnToggled(action, oneShot, deferred);

        public static CheckButtonAction OnTreeEntered(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CheckButtonAction OnTreeExited(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CheckButtonAction OnTreeExiting(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CheckButtonAction OnVisibilityChanged(this CheckButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CheckButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ClippedCameraAction OnGameplayEntered(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ClippedCameraAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static ClippedCameraAction OnGameplayExited(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ClippedCameraAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static ClippedCameraAction OnReady(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ClippedCameraAction>(target).OnReady(action, oneShot, deferred);

        public static ClippedCameraAction OnRenamed(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ClippedCameraAction>(target).OnRenamed(action, oneShot, deferred);

        public static ClippedCameraAction OnScriptChanged(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ClippedCameraAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ClippedCameraAction OnTreeEntered(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ClippedCameraAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ClippedCameraAction OnTreeExited(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ClippedCameraAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ClippedCameraAction OnTreeExiting(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ClippedCameraAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ClippedCameraAction OnVisibilityChanged(this ClippedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ClippedCameraAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CollisionPolygonAction OnGameplayEntered(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static CollisionPolygonAction OnGameplayExited(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static CollisionPolygonAction OnReady(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnReady(action, oneShot, deferred);

        public static CollisionPolygonAction OnRenamed(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnRenamed(action, oneShot, deferred);

        public static CollisionPolygonAction OnScriptChanged(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CollisionPolygonAction OnTreeEntered(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CollisionPolygonAction OnTreeExited(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CollisionPolygonAction OnTreeExiting(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CollisionPolygonAction OnVisibilityChanged(this CollisionPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygonAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CollisionPolygon2DAction OnDraw(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnDraw(action, oneShot, deferred);

        public static CollisionPolygon2DAction OnHide(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnHide(action, oneShot, deferred);

        public static CollisionPolygon2DAction OnItemRectChanged(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static CollisionPolygon2DAction OnReady(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnReady(action, oneShot, deferred);

        public static CollisionPolygon2DAction OnRenamed(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static CollisionPolygon2DAction OnScriptChanged(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CollisionPolygon2DAction OnTreeEntered(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CollisionPolygon2DAction OnTreeExited(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CollisionPolygon2DAction OnTreeExiting(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CollisionPolygon2DAction OnVisibilityChanged(this CollisionPolygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionPolygon2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CollisionShapeAction OnGameplayEntered(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShapeAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static CollisionShapeAction OnGameplayExited(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShapeAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static CollisionShapeAction OnReady(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShapeAction>(target).OnReady(action, oneShot, deferred);

        public static CollisionShapeAction OnRenamed(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShapeAction>(target).OnRenamed(action, oneShot, deferred);

        public static CollisionShapeAction OnScriptChanged(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShapeAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CollisionShapeAction OnTreeEntered(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShapeAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CollisionShapeAction OnTreeExited(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShapeAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CollisionShapeAction OnTreeExiting(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShapeAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CollisionShapeAction OnVisibilityChanged(this CollisionShape target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShapeAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CollisionShape2DAction OnDraw(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnDraw(action, oneShot, deferred);

        public static CollisionShape2DAction OnHide(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnHide(action, oneShot, deferred);

        public static CollisionShape2DAction OnItemRectChanged(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static CollisionShape2DAction OnReady(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnReady(action, oneShot, deferred);

        public static CollisionShape2DAction OnRenamed(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static CollisionShape2DAction OnScriptChanged(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CollisionShape2DAction OnTreeEntered(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CollisionShape2DAction OnTreeExited(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CollisionShape2DAction OnTreeExiting(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CollisionShape2DAction OnVisibilityChanged(this CollisionShape2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CollisionShape2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ColorPickerAction OnColorChanged(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnColorChanged(action, oneShot, deferred);

        public static ColorPickerAction OnDraw(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnDraw(action, oneShot, deferred);

        public static ColorPickerAction OnFocusEntered(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static ColorPickerAction OnFocusExited(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static ColorPickerAction OnGuiInput(this ColorPicker target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static ColorPickerAction OnHide(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnHide(action, oneShot, deferred);

        public static ColorPickerAction OnItemRectChanged(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static ColorPickerAction OnMinimumSizeChanged(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static ColorPickerAction OnModalClosed(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static ColorPickerAction OnMouseEntered(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static ColorPickerAction OnMouseExited(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static ColorPickerAction OnPresetAdded(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnPresetAdded(action, oneShot, deferred);

        public static ColorPickerAction OnPresetRemoved(this ColorPicker target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnPresetRemoved(action, oneShot, deferred);

        public static ColorPickerAction OnReady(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnReady(action, oneShot, deferred);

        public static ColorPickerAction OnRenamed(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnRenamed(action, oneShot, deferred);

        public static ColorPickerAction OnResized(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnResized(action, oneShot, deferred);

        public static ColorPickerAction OnScriptChanged(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ColorPickerAction OnSizeFlagsChanged(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static ColorPickerAction OnSortChildren(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnSortChildren(action, oneShot, deferred);

        public static ColorPickerAction OnTreeEntered(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ColorPickerAction OnTreeExited(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ColorPickerAction OnTreeExiting(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ColorPickerAction OnVisibilityChanged(this ColorPicker target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ColorPickerButtonAction OnButtonDown(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnButtonDown(action, oneShot, deferred);

        public static ColorPickerButtonAction OnButtonUp(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnButtonUp(action, oneShot, deferred);

        public static ColorPickerButtonAction OnColorChanged(this ColorPickerButton target, Action<Color> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnColorChanged(action, oneShot, deferred);

        public static ColorPickerButtonAction OnDraw(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnDraw(action, oneShot, deferred);

        public static ColorPickerButtonAction OnFocusEntered(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static ColorPickerButtonAction OnFocusExited(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnFocusExited(action, oneShot, deferred);

        public static ColorPickerButtonAction OnGuiInput(this ColorPickerButton target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnGuiInput(action, oneShot, deferred);

        public static ColorPickerButtonAction OnHide(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnHide(action, oneShot, deferred);

        public static ColorPickerButtonAction OnItemRectChanged(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static ColorPickerButtonAction OnMinimumSizeChanged(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static ColorPickerButtonAction OnModalClosed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnModalClosed(action, oneShot, deferred);

        public static ColorPickerButtonAction OnMouseEntered(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static ColorPickerButtonAction OnMouseExited(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnMouseExited(action, oneShot, deferred);

        public static ColorPickerButtonAction OnPickerCreated(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnPickerCreated(action, oneShot, deferred);

        public static ColorPickerButtonAction OnPopupClosed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnPopupClosed(action, oneShot, deferred);

        public static ColorPickerButtonAction OnPressed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnPressed(action, oneShot, deferred);

        public static ColorPickerButtonAction OnReady(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnReady(action, oneShot, deferred);

        public static ColorPickerButtonAction OnRenamed(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnRenamed(action, oneShot, deferred);

        public static ColorPickerButtonAction OnResized(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnResized(action, oneShot, deferred);

        public static ColorPickerButtonAction OnScriptChanged(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ColorPickerButtonAction OnSizeFlagsChanged(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static ColorPickerButtonAction OnToggled(this ColorPickerButton target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnToggled(action, oneShot, deferred);

        public static ColorPickerButtonAction OnTreeEntered(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ColorPickerButtonAction OnTreeExited(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ColorPickerButtonAction OnTreeExiting(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ColorPickerButtonAction OnVisibilityChanged(this ColorPickerButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorPickerButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ColorRectAction OnDraw(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnDraw(action, oneShot, deferred);

        public static ColorRectAction OnFocusEntered(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static ColorRectAction OnFocusExited(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnFocusExited(action, oneShot, deferred);

        public static ColorRectAction OnGuiInput(this ColorRect target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnGuiInput(action, oneShot, deferred);

        public static ColorRectAction OnHide(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnHide(action, oneShot, deferred);

        public static ColorRectAction OnItemRectChanged(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static ColorRectAction OnMinimumSizeChanged(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static ColorRectAction OnModalClosed(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnModalClosed(action, oneShot, deferred);

        public static ColorRectAction OnMouseEntered(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static ColorRectAction OnMouseExited(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnMouseExited(action, oneShot, deferred);

        public static ColorRectAction OnReady(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnReady(action, oneShot, deferred);

        public static ColorRectAction OnRenamed(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnRenamed(action, oneShot, deferred);

        public static ColorRectAction OnResized(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnResized(action, oneShot, deferred);

        public static ColorRectAction OnScriptChanged(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ColorRectAction OnSizeFlagsChanged(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static ColorRectAction OnTreeEntered(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ColorRectAction OnTreeExited(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ColorRectAction OnTreeExiting(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ColorRectAction OnVisibilityChanged(this ColorRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ColorRectAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ConeTwistJointAction OnGameplayEntered(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static ConeTwistJointAction OnGameplayExited(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static ConeTwistJointAction OnReady(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnReady(action, oneShot, deferred);

        public static ConeTwistJointAction OnRenamed(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnRenamed(action, oneShot, deferred);

        public static ConeTwistJointAction OnScriptChanged(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ConeTwistJointAction OnTreeEntered(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ConeTwistJointAction OnTreeExited(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ConeTwistJointAction OnTreeExiting(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ConeTwistJointAction OnVisibilityChanged(this ConeTwistJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConeTwistJointAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ConfirmationDialogAction OnAboutToShow(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnAboutToShow(action, oneShot, deferred);

        public static ConfirmationDialogAction OnConfirmed(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnConfirmed(action, oneShot, deferred);

        public static ConfirmationDialogAction OnCustomAction(this ConfirmationDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnCustomAction(action, oneShot, deferred);

        public static ConfirmationDialogAction OnDraw(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnDraw(action, oneShot, deferred);

        public static ConfirmationDialogAction OnFocusEntered(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static ConfirmationDialogAction OnFocusExited(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnFocusExited(action, oneShot, deferred);

        public static ConfirmationDialogAction OnGuiInput(this ConfirmationDialog target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnGuiInput(action, oneShot, deferred);

        public static ConfirmationDialogAction OnHide(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnHide(action, oneShot, deferred);

        public static ConfirmationDialogAction OnItemRectChanged(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static ConfirmationDialogAction OnMinimumSizeChanged(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static ConfirmationDialogAction OnModalClosed(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnModalClosed(action, oneShot, deferred);

        public static ConfirmationDialogAction OnMouseEntered(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static ConfirmationDialogAction OnMouseExited(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnMouseExited(action, oneShot, deferred);

        public static ConfirmationDialogAction OnPopupHide(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnPopupHide(action, oneShot, deferred);

        public static ConfirmationDialogAction OnReady(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnReady(action, oneShot, deferred);

        public static ConfirmationDialogAction OnRenamed(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnRenamed(action, oneShot, deferred);

        public static ConfirmationDialogAction OnResized(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnResized(action, oneShot, deferred);

        public static ConfirmationDialogAction OnScriptChanged(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ConfirmationDialogAction OnSizeFlagsChanged(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static ConfirmationDialogAction OnTreeEntered(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ConfirmationDialogAction OnTreeExited(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ConfirmationDialogAction OnTreeExiting(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ConfirmationDialogAction OnVisibilityChanged(this ConfirmationDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ConfirmationDialogAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ContainerAction OnDraw(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnDraw(action, oneShot, deferred);

        public static ContainerAction OnFocusEntered(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static ContainerAction OnFocusExited(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static ContainerAction OnGuiInput(this Container target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static ContainerAction OnHide(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnHide(action, oneShot, deferred);

        public static ContainerAction OnItemRectChanged(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static ContainerAction OnMinimumSizeChanged(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static ContainerAction OnModalClosed(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static ContainerAction OnMouseEntered(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static ContainerAction OnMouseExited(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static ContainerAction OnReady(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnReady(action, oneShot, deferred);

        public static ContainerAction OnRenamed(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnRenamed(action, oneShot, deferred);

        public static ContainerAction OnResized(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnResized(action, oneShot, deferred);

        public static ContainerAction OnScriptChanged(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ContainerAction OnSizeFlagsChanged(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static ContainerAction OnSortChildren(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnSortChildren(action, oneShot, deferred);

        public static ContainerAction OnTreeEntered(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ContainerAction OnTreeExited(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ContainerAction OnTreeExiting(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ContainerAction OnVisibilityChanged(this Container target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ControlAction OnDraw(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnDraw(action, oneShot, deferred);

        public static ControlAction OnFocusEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static ControlAction OnFocusExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnFocusExited(action, oneShot, deferred);

        public static ControlAction OnGuiInput(this Control target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnGuiInput(action, oneShot, deferred);

        public static ControlAction OnHide(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnHide(action, oneShot, deferred);

        public static ControlAction OnItemRectChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static ControlAction OnMinimumSizeChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static ControlAction OnModalClosed(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnModalClosed(action, oneShot, deferred);

        public static ControlAction OnMouseEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static ControlAction OnMouseExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnMouseExited(action, oneShot, deferred);

        public static ControlAction OnReady(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnReady(action, oneShot, deferred);

        public static ControlAction OnRenamed(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnRenamed(action, oneShot, deferred);

        public static ControlAction OnResized(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnResized(action, oneShot, deferred);

        public static ControlAction OnScriptChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ControlAction OnSizeFlagsChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static ControlAction OnTreeEntered(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ControlAction OnTreeExited(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ControlAction OnTreeExiting(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ControlAction OnVisibilityChanged(this Control target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ControlAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CPUParticlesAction OnGameplayEntered(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticlesAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static CPUParticlesAction OnGameplayExited(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticlesAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static CPUParticlesAction OnReady(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticlesAction>(target).OnReady(action, oneShot, deferred);

        public static CPUParticlesAction OnRenamed(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticlesAction>(target).OnRenamed(action, oneShot, deferred);

        public static CPUParticlesAction OnScriptChanged(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticlesAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CPUParticlesAction OnTreeEntered(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticlesAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CPUParticlesAction OnTreeExited(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticlesAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CPUParticlesAction OnTreeExiting(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticlesAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CPUParticlesAction OnVisibilityChanged(this CPUParticles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticlesAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CPUParticles2DAction OnDraw(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnDraw(action, oneShot, deferred);

        public static CPUParticles2DAction OnHide(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnHide(action, oneShot, deferred);

        public static CPUParticles2DAction OnItemRectChanged(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static CPUParticles2DAction OnReady(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnReady(action, oneShot, deferred);

        public static CPUParticles2DAction OnRenamed(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static CPUParticles2DAction OnScriptChanged(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CPUParticles2DAction OnTreeEntered(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CPUParticles2DAction OnTreeExited(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CPUParticles2DAction OnTreeExiting(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CPUParticles2DAction OnVisibilityChanged(this CPUParticles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CPUParticles2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CSGBoxAction OnGameplayEntered(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGBoxAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static CSGBoxAction OnGameplayExited(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGBoxAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static CSGBoxAction OnReady(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGBoxAction>(target).OnReady(action, oneShot, deferred);

        public static CSGBoxAction OnRenamed(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGBoxAction>(target).OnRenamed(action, oneShot, deferred);

        public static CSGBoxAction OnScriptChanged(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGBoxAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CSGBoxAction OnTreeEntered(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGBoxAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CSGBoxAction OnTreeExited(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGBoxAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CSGBoxAction OnTreeExiting(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGBoxAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CSGBoxAction OnVisibilityChanged(this CSGBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGBoxAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CSGCombinerAction OnGameplayEntered(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCombinerAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static CSGCombinerAction OnGameplayExited(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCombinerAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static CSGCombinerAction OnReady(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCombinerAction>(target).OnReady(action, oneShot, deferred);

        public static CSGCombinerAction OnRenamed(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCombinerAction>(target).OnRenamed(action, oneShot, deferred);

        public static CSGCombinerAction OnScriptChanged(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCombinerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CSGCombinerAction OnTreeEntered(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCombinerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CSGCombinerAction OnTreeExited(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCombinerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CSGCombinerAction OnTreeExiting(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCombinerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CSGCombinerAction OnVisibilityChanged(this CSGCombiner target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCombinerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CSGCylinderAction OnGameplayEntered(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCylinderAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static CSGCylinderAction OnGameplayExited(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCylinderAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static CSGCylinderAction OnReady(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCylinderAction>(target).OnReady(action, oneShot, deferred);

        public static CSGCylinderAction OnRenamed(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCylinderAction>(target).OnRenamed(action, oneShot, deferred);

        public static CSGCylinderAction OnScriptChanged(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCylinderAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CSGCylinderAction OnTreeEntered(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCylinderAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CSGCylinderAction OnTreeExited(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCylinderAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CSGCylinderAction OnTreeExiting(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCylinderAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CSGCylinderAction OnVisibilityChanged(this CSGCylinder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGCylinderAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CSGMeshAction OnGameplayEntered(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGMeshAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static CSGMeshAction OnGameplayExited(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGMeshAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static CSGMeshAction OnReady(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGMeshAction>(target).OnReady(action, oneShot, deferred);

        public static CSGMeshAction OnRenamed(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGMeshAction>(target).OnRenamed(action, oneShot, deferred);

        public static CSGMeshAction OnScriptChanged(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGMeshAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CSGMeshAction OnTreeEntered(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGMeshAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CSGMeshAction OnTreeExited(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGMeshAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CSGMeshAction OnTreeExiting(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGMeshAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CSGMeshAction OnVisibilityChanged(this CSGMesh target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGMeshAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CSGPolygonAction OnGameplayEntered(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGPolygonAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static CSGPolygonAction OnGameplayExited(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGPolygonAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static CSGPolygonAction OnReady(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGPolygonAction>(target).OnReady(action, oneShot, deferred);

        public static CSGPolygonAction OnRenamed(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGPolygonAction>(target).OnRenamed(action, oneShot, deferred);

        public static CSGPolygonAction OnScriptChanged(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGPolygonAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CSGPolygonAction OnTreeEntered(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGPolygonAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CSGPolygonAction OnTreeExited(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGPolygonAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CSGPolygonAction OnTreeExiting(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGPolygonAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CSGPolygonAction OnVisibilityChanged(this CSGPolygon target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGPolygonAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CSGSphereAction OnGameplayEntered(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGSphereAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static CSGSphereAction OnGameplayExited(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGSphereAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static CSGSphereAction OnReady(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGSphereAction>(target).OnReady(action, oneShot, deferred);

        public static CSGSphereAction OnRenamed(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGSphereAction>(target).OnRenamed(action, oneShot, deferred);

        public static CSGSphereAction OnScriptChanged(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGSphereAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CSGSphereAction OnTreeEntered(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGSphereAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CSGSphereAction OnTreeExited(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGSphereAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CSGSphereAction OnTreeExiting(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGSphereAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CSGSphereAction OnVisibilityChanged(this CSGSphere target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGSphereAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static CSGTorusAction OnGameplayEntered(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGTorusAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static CSGTorusAction OnGameplayExited(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGTorusAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static CSGTorusAction OnReady(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGTorusAction>(target).OnReady(action, oneShot, deferred);

        public static CSGTorusAction OnRenamed(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGTorusAction>(target).OnRenamed(action, oneShot, deferred);

        public static CSGTorusAction OnScriptChanged(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGTorusAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static CSGTorusAction OnTreeEntered(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGTorusAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static CSGTorusAction OnTreeExited(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGTorusAction>(target).OnTreeExited(action, oneShot, deferred);

        public static CSGTorusAction OnTreeExiting(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGTorusAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static CSGTorusAction OnVisibilityChanged(this CSGTorus target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<CSGTorusAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static DampedSpringJoint2DAction OnDraw(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnDraw(action, oneShot, deferred);

        public static DampedSpringJoint2DAction OnHide(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnHide(action, oneShot, deferred);

        public static DampedSpringJoint2DAction OnItemRectChanged(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static DampedSpringJoint2DAction OnReady(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnReady(action, oneShot, deferred);

        public static DampedSpringJoint2DAction OnRenamed(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static DampedSpringJoint2DAction OnScriptChanged(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static DampedSpringJoint2DAction OnTreeEntered(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static DampedSpringJoint2DAction OnTreeExited(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static DampedSpringJoint2DAction OnTreeExiting(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static DampedSpringJoint2DAction OnVisibilityChanged(this DampedSpringJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DampedSpringJoint2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static DirectionalLightAction OnGameplayEntered(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DirectionalLightAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static DirectionalLightAction OnGameplayExited(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DirectionalLightAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static DirectionalLightAction OnReady(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DirectionalLightAction>(target).OnReady(action, oneShot, deferred);

        public static DirectionalLightAction OnRenamed(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DirectionalLightAction>(target).OnRenamed(action, oneShot, deferred);

        public static DirectionalLightAction OnScriptChanged(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DirectionalLightAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static DirectionalLightAction OnTreeEntered(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DirectionalLightAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static DirectionalLightAction OnTreeExited(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DirectionalLightAction>(target).OnTreeExited(action, oneShot, deferred);

        public static DirectionalLightAction OnTreeExiting(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DirectionalLightAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static DirectionalLightAction OnVisibilityChanged(this DirectionalLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<DirectionalLightAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static FileDialogAction OnAboutToShow(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnAboutToShow(action, oneShot, deferred);

        public static FileDialogAction OnConfirmed(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnConfirmed(action, oneShot, deferred);

        public static FileDialogAction OnCustomAction(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnCustomAction(action, oneShot, deferred);

        public static FileDialogAction OnDirSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnDirSelected(action, oneShot, deferred);

        public static FileDialogAction OnDraw(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnDraw(action, oneShot, deferred);

        public static FileDialogAction OnFileSelected(this FileDialog target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnFileSelected(action, oneShot, deferred);

        public static FileDialogAction OnFilesSelected(this FileDialog target, Action<string[]> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnFilesSelected(action, oneShot, deferred);

        public static FileDialogAction OnFocusEntered(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static FileDialogAction OnFocusExited(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnFocusExited(action, oneShot, deferred);

        public static FileDialogAction OnGuiInput(this FileDialog target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnGuiInput(action, oneShot, deferred);

        public static FileDialogAction OnHide(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnHide(action, oneShot, deferred);

        public static FileDialogAction OnItemRectChanged(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static FileDialogAction OnMinimumSizeChanged(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static FileDialogAction OnModalClosed(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnModalClosed(action, oneShot, deferred);

        public static FileDialogAction OnMouseEntered(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static FileDialogAction OnMouseExited(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnMouseExited(action, oneShot, deferred);

        public static FileDialogAction OnPopupHide(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnPopupHide(action, oneShot, deferred);

        public static FileDialogAction OnReady(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnReady(action, oneShot, deferred);

        public static FileDialogAction OnRenamed(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnRenamed(action, oneShot, deferred);

        public static FileDialogAction OnResized(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnResized(action, oneShot, deferred);

        public static FileDialogAction OnScriptChanged(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static FileDialogAction OnSizeFlagsChanged(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static FileDialogAction OnTreeEntered(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static FileDialogAction OnTreeExited(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnTreeExited(action, oneShot, deferred);

        public static FileDialogAction OnTreeExiting(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static FileDialogAction OnVisibilityChanged(this FileDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<FileDialogAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Generic6DOFJointAction OnGameplayEntered(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static Generic6DOFJointAction OnGameplayExited(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static Generic6DOFJointAction OnReady(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnReady(action, oneShot, deferred);

        public static Generic6DOFJointAction OnRenamed(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnRenamed(action, oneShot, deferred);

        public static Generic6DOFJointAction OnScriptChanged(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Generic6DOFJointAction OnTreeEntered(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Generic6DOFJointAction OnTreeExited(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Generic6DOFJointAction OnTreeExiting(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Generic6DOFJointAction OnVisibilityChanged(this Generic6DOFJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Generic6DOFJointAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static GIProbeAction OnGameplayEntered(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GIProbeAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static GIProbeAction OnGameplayExited(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GIProbeAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static GIProbeAction OnReady(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GIProbeAction>(target).OnReady(action, oneShot, deferred);

        public static GIProbeAction OnRenamed(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GIProbeAction>(target).OnRenamed(action, oneShot, deferred);

        public static GIProbeAction OnScriptChanged(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GIProbeAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static GIProbeAction OnTreeEntered(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GIProbeAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static GIProbeAction OnTreeExited(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GIProbeAction>(target).OnTreeExited(action, oneShot, deferred);

        public static GIProbeAction OnTreeExiting(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GIProbeAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static GIProbeAction OnVisibilityChanged(this GIProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GIProbeAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static GraphEditAction OnBeginNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnBeginNodeMove(action, oneShot, deferred);

        public static GraphEditAction OnConnectionFromEmpty(this GraphEdit target, Action<Vector2, string, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnConnectionFromEmpty(action, oneShot, deferred);

        public static GraphEditAction OnConnectionRequest(this GraphEdit target, Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnConnectionRequest(action, oneShot, deferred);

        public static GraphEditAction OnConnectionToEmpty(this GraphEdit target, Action<string, int, Vector2> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnConnectionToEmpty(action, oneShot, deferred);

        public static GraphEditAction OnCopyNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnCopyNodesRequest(action, oneShot, deferred);

        public static GraphEditAction OnDeleteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnDeleteNodesRequest(action, oneShot, deferred);

        public static GraphEditAction OnDisconnectionRequest(this GraphEdit target, Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnDisconnectionRequest(action, oneShot, deferred);

        public static GraphEditAction OnDraw(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnDraw(action, oneShot, deferred);

        public static GraphEditAction OnDuplicateNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnDuplicateNodesRequest(action, oneShot, deferred);

        public static GraphEditAction OnEndNodeMove(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnEndNodeMove(action, oneShot, deferred);

        public static GraphEditAction OnFocusEntered(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static GraphEditAction OnFocusExited(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnFocusExited(action, oneShot, deferred);

        public static GraphEditAction OnGuiInput(this GraphEdit target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnGuiInput(action, oneShot, deferred);

        public static GraphEditAction OnHide(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnHide(action, oneShot, deferred);

        public static GraphEditAction OnItemRectChanged(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static GraphEditAction OnMinimumSizeChanged(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static GraphEditAction OnModalClosed(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnModalClosed(action, oneShot, deferred);

        public static GraphEditAction OnMouseEntered(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static GraphEditAction OnMouseExited(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnMouseExited(action, oneShot, deferred);

        public static GraphEditAction OnNodeSelected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnNodeSelected(action, oneShot, deferred);

        public static GraphEditAction OnNodeUnselected(this GraphEdit target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnNodeUnselected(action, oneShot, deferred);

        public static GraphEditAction OnPasteNodesRequest(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnPasteNodesRequest(action, oneShot, deferred);

        public static GraphEditAction OnPopupRequest(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnPopupRequest(action, oneShot, deferred);

        public static GraphEditAction OnReady(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnReady(action, oneShot, deferred);

        public static GraphEditAction OnRenamed(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnRenamed(action, oneShot, deferred);

        public static GraphEditAction OnResized(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnResized(action, oneShot, deferred);

        public static GraphEditAction OnScriptChanged(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static GraphEditAction OnScrollOffsetChanged(this GraphEdit target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnScrollOffsetChanged(action, oneShot, deferred);

        public static GraphEditAction OnSizeFlagsChanged(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static GraphEditAction OnTreeEntered(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static GraphEditAction OnTreeExited(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnTreeExited(action, oneShot, deferred);

        public static GraphEditAction OnTreeExiting(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static GraphEditAction OnVisibilityChanged(this GraphEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphEditAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static GraphNodeAction OnCloseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnCloseRequest(action, oneShot, deferred);

        public static GraphNodeAction OnDragged(this GraphNode target, Action<Vector2, Vector2> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnDragged(action, oneShot, deferred);

        public static GraphNodeAction OnDraw(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnDraw(action, oneShot, deferred);

        public static GraphNodeAction OnFocusEntered(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static GraphNodeAction OnFocusExited(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnFocusExited(action, oneShot, deferred);

        public static GraphNodeAction OnGuiInput(this GraphNode target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnGuiInput(action, oneShot, deferred);

        public static GraphNodeAction OnHide(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnHide(action, oneShot, deferred);

        public static GraphNodeAction OnItemRectChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static GraphNodeAction OnMinimumSizeChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static GraphNodeAction OnModalClosed(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnModalClosed(action, oneShot, deferred);

        public static GraphNodeAction OnMouseEntered(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static GraphNodeAction OnMouseExited(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnMouseExited(action, oneShot, deferred);

        public static GraphNodeAction OnOffsetChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnOffsetChanged(action, oneShot, deferred);

        public static GraphNodeAction OnRaiseRequest(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnRaiseRequest(action, oneShot, deferred);

        public static GraphNodeAction OnReady(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnReady(action, oneShot, deferred);

        public static GraphNodeAction OnRenamed(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnRenamed(action, oneShot, deferred);

        public static GraphNodeAction OnResized(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnResized(action, oneShot, deferred);

        public static GraphNodeAction OnResizeRequest(this GraphNode target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnResizeRequest(action, oneShot, deferred);

        public static GraphNodeAction OnScriptChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static GraphNodeAction OnSizeFlagsChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static GraphNodeAction OnSlotUpdated(this GraphNode target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnSlotUpdated(action, oneShot, deferred);

        public static GraphNodeAction OnSortChildren(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnSortChildren(action, oneShot, deferred);

        public static GraphNodeAction OnTreeEntered(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static GraphNodeAction OnTreeExited(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnTreeExited(action, oneShot, deferred);

        public static GraphNodeAction OnTreeExiting(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static GraphNodeAction OnVisibilityChanged(this GraphNode target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GraphNodeAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static GridContainerAction OnDraw(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnDraw(action, oneShot, deferred);

        public static GridContainerAction OnFocusEntered(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static GridContainerAction OnFocusExited(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static GridContainerAction OnGuiInput(this GridContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static GridContainerAction OnHide(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnHide(action, oneShot, deferred);

        public static GridContainerAction OnItemRectChanged(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static GridContainerAction OnMinimumSizeChanged(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static GridContainerAction OnModalClosed(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static GridContainerAction OnMouseEntered(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static GridContainerAction OnMouseExited(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static GridContainerAction OnReady(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnReady(action, oneShot, deferred);

        public static GridContainerAction OnRenamed(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnRenamed(action, oneShot, deferred);

        public static GridContainerAction OnResized(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnResized(action, oneShot, deferred);

        public static GridContainerAction OnScriptChanged(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static GridContainerAction OnSizeFlagsChanged(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static GridContainerAction OnSortChildren(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnSortChildren(action, oneShot, deferred);

        public static GridContainerAction OnTreeEntered(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static GridContainerAction OnTreeExited(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static GridContainerAction OnTreeExiting(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static GridContainerAction OnVisibilityChanged(this GridContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static GridMapAction OnCellSizeChanged(this GridMap target, Action<Vector3> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridMapAction>(target).OnCellSizeChanged(action, oneShot, deferred);

        public static GridMapAction OnGameplayEntered(this GridMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridMapAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static GridMapAction OnGameplayExited(this GridMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridMapAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static GridMapAction OnReady(this GridMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridMapAction>(target).OnReady(action, oneShot, deferred);

        public static GridMapAction OnRenamed(this GridMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridMapAction>(target).OnRenamed(action, oneShot, deferred);

        public static GridMapAction OnScriptChanged(this GridMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridMapAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static GridMapAction OnTreeEntered(this GridMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridMapAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static GridMapAction OnTreeExited(this GridMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridMapAction>(target).OnTreeExited(action, oneShot, deferred);

        public static GridMapAction OnTreeExiting(this GridMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridMapAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static GridMapAction OnVisibilityChanged(this GridMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GridMapAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static GrooveJoint2DAction OnDraw(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnDraw(action, oneShot, deferred);

        public static GrooveJoint2DAction OnHide(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnHide(action, oneShot, deferred);

        public static GrooveJoint2DAction OnItemRectChanged(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static GrooveJoint2DAction OnReady(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnReady(action, oneShot, deferred);

        public static GrooveJoint2DAction OnRenamed(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static GrooveJoint2DAction OnScriptChanged(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static GrooveJoint2DAction OnTreeEntered(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static GrooveJoint2DAction OnTreeExited(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static GrooveJoint2DAction OnTreeExiting(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static GrooveJoint2DAction OnVisibilityChanged(this GrooveJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<GrooveJoint2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static HBoxContainerAction OnDraw(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnDraw(action, oneShot, deferred);

        public static HBoxContainerAction OnFocusEntered(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static HBoxContainerAction OnFocusExited(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static HBoxContainerAction OnGuiInput(this HBoxContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static HBoxContainerAction OnHide(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnHide(action, oneShot, deferred);

        public static HBoxContainerAction OnItemRectChanged(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static HBoxContainerAction OnMinimumSizeChanged(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static HBoxContainerAction OnModalClosed(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static HBoxContainerAction OnMouseEntered(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static HBoxContainerAction OnMouseExited(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static HBoxContainerAction OnReady(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnReady(action, oneShot, deferred);

        public static HBoxContainerAction OnRenamed(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnRenamed(action, oneShot, deferred);

        public static HBoxContainerAction OnResized(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnResized(action, oneShot, deferred);

        public static HBoxContainerAction OnScriptChanged(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static HBoxContainerAction OnSizeFlagsChanged(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static HBoxContainerAction OnSortChildren(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnSortChildren(action, oneShot, deferred);

        public static HBoxContainerAction OnTreeEntered(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static HBoxContainerAction OnTreeExited(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static HBoxContainerAction OnTreeExiting(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static HBoxContainerAction OnVisibilityChanged(this HBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HBoxContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static HingeJointAction OnGameplayEntered(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HingeJointAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static HingeJointAction OnGameplayExited(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HingeJointAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static HingeJointAction OnReady(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HingeJointAction>(target).OnReady(action, oneShot, deferred);

        public static HingeJointAction OnRenamed(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HingeJointAction>(target).OnRenamed(action, oneShot, deferred);

        public static HingeJointAction OnScriptChanged(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HingeJointAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static HingeJointAction OnTreeEntered(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HingeJointAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static HingeJointAction OnTreeExited(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HingeJointAction>(target).OnTreeExited(action, oneShot, deferred);

        public static HingeJointAction OnTreeExiting(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HingeJointAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static HingeJointAction OnVisibilityChanged(this HingeJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HingeJointAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static HScrollBarAction OnChanged(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnChanged(action, oneShot, deferred);

        public static HScrollBarAction OnDraw(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnDraw(action, oneShot, deferred);

        public static HScrollBarAction OnFocusEntered(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static HScrollBarAction OnFocusExited(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnFocusExited(action, oneShot, deferred);

        public static HScrollBarAction OnGuiInput(this HScrollBar target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnGuiInput(action, oneShot, deferred);

        public static HScrollBarAction OnHide(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnHide(action, oneShot, deferred);

        public static HScrollBarAction OnItemRectChanged(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static HScrollBarAction OnMinimumSizeChanged(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static HScrollBarAction OnModalClosed(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnModalClosed(action, oneShot, deferred);

        public static HScrollBarAction OnMouseEntered(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static HScrollBarAction OnMouseExited(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnMouseExited(action, oneShot, deferred);

        public static HScrollBarAction OnReady(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnReady(action, oneShot, deferred);

        public static HScrollBarAction OnRenamed(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnRenamed(action, oneShot, deferred);

        public static HScrollBarAction OnResized(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnResized(action, oneShot, deferred);

        public static HScrollBarAction OnScriptChanged(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static HScrollBarAction OnScrolling(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnScrolling(action, oneShot, deferred);

        public static HScrollBarAction OnSizeFlagsChanged(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static HScrollBarAction OnTreeEntered(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static HScrollBarAction OnTreeExited(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnTreeExited(action, oneShot, deferred);

        public static HScrollBarAction OnTreeExiting(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static HScrollBarAction OnValueChanged(this HScrollBar target, Action<float> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnValueChanged(action, oneShot, deferred);

        public static HScrollBarAction OnVisibilityChanged(this HScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HScrollBarAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static HSeparatorAction OnDraw(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnDraw(action, oneShot, deferred);

        public static HSeparatorAction OnFocusEntered(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static HSeparatorAction OnFocusExited(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnFocusExited(action, oneShot, deferred);

        public static HSeparatorAction OnGuiInput(this HSeparator target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnGuiInput(action, oneShot, deferred);

        public static HSeparatorAction OnHide(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnHide(action, oneShot, deferred);

        public static HSeparatorAction OnItemRectChanged(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static HSeparatorAction OnMinimumSizeChanged(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static HSeparatorAction OnModalClosed(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnModalClosed(action, oneShot, deferred);

        public static HSeparatorAction OnMouseEntered(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static HSeparatorAction OnMouseExited(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnMouseExited(action, oneShot, deferred);

        public static HSeparatorAction OnReady(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnReady(action, oneShot, deferred);

        public static HSeparatorAction OnRenamed(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnRenamed(action, oneShot, deferred);

        public static HSeparatorAction OnResized(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnResized(action, oneShot, deferred);

        public static HSeparatorAction OnScriptChanged(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static HSeparatorAction OnSizeFlagsChanged(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static HSeparatorAction OnTreeEntered(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static HSeparatorAction OnTreeExited(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnTreeExited(action, oneShot, deferred);

        public static HSeparatorAction OnTreeExiting(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static HSeparatorAction OnVisibilityChanged(this HSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSeparatorAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static HSliderAction OnChanged(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnChanged(action, oneShot, deferred);

        public static HSliderAction OnDraw(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnDraw(action, oneShot, deferred);

        public static HSliderAction OnFocusEntered(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static HSliderAction OnFocusExited(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnFocusExited(action, oneShot, deferred);

        public static HSliderAction OnGuiInput(this HSlider target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnGuiInput(action, oneShot, deferred);

        public static HSliderAction OnHide(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnHide(action, oneShot, deferred);

        public static HSliderAction OnItemRectChanged(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static HSliderAction OnMinimumSizeChanged(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static HSliderAction OnModalClosed(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnModalClosed(action, oneShot, deferred);

        public static HSliderAction OnMouseEntered(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static HSliderAction OnMouseExited(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnMouseExited(action, oneShot, deferred);

        public static HSliderAction OnReady(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnReady(action, oneShot, deferred);

        public static HSliderAction OnRenamed(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnRenamed(action, oneShot, deferred);

        public static HSliderAction OnResized(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnResized(action, oneShot, deferred);

        public static HSliderAction OnScriptChanged(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static HSliderAction OnSizeFlagsChanged(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static HSliderAction OnTreeEntered(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static HSliderAction OnTreeExited(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnTreeExited(action, oneShot, deferred);

        public static HSliderAction OnTreeExiting(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static HSliderAction OnValueChanged(this HSlider target, Action<float> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnValueChanged(action, oneShot, deferred);

        public static HSliderAction OnVisibilityChanged(this HSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSliderAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static HSplitContainerAction OnDragged(this HSplitContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnDragged(action, oneShot, deferred);

        public static HSplitContainerAction OnDraw(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnDraw(action, oneShot, deferred);

        public static HSplitContainerAction OnFocusEntered(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static HSplitContainerAction OnFocusExited(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static HSplitContainerAction OnGuiInput(this HSplitContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static HSplitContainerAction OnHide(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnHide(action, oneShot, deferred);

        public static HSplitContainerAction OnItemRectChanged(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static HSplitContainerAction OnMinimumSizeChanged(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static HSplitContainerAction OnModalClosed(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static HSplitContainerAction OnMouseEntered(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static HSplitContainerAction OnMouseExited(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static HSplitContainerAction OnReady(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnReady(action, oneShot, deferred);

        public static HSplitContainerAction OnRenamed(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnRenamed(action, oneShot, deferred);

        public static HSplitContainerAction OnResized(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnResized(action, oneShot, deferred);

        public static HSplitContainerAction OnScriptChanged(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static HSplitContainerAction OnSizeFlagsChanged(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static HSplitContainerAction OnSortChildren(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnSortChildren(action, oneShot, deferred);

        public static HSplitContainerAction OnTreeEntered(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static HSplitContainerAction OnTreeExited(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static HSplitContainerAction OnTreeExiting(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static HSplitContainerAction OnVisibilityChanged(this HSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HSplitContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static HTTPRequestAction OnReady(this HTTPRequest target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HTTPRequestAction>(target).OnReady(action, oneShot, deferred);

        public static HTTPRequestAction OnRenamed(this HTTPRequest target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HTTPRequestAction>(target).OnRenamed(action, oneShot, deferred);

        public static HTTPRequestAction OnRequestCompleted(this HTTPRequest target, Action<byte[], string[], int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HTTPRequestAction>(target).OnRequestCompleted(action, oneShot, deferred);

        public static HTTPRequestAction OnScriptChanged(this HTTPRequest target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HTTPRequestAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static HTTPRequestAction OnTreeEntered(this HTTPRequest target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HTTPRequestAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static HTTPRequestAction OnTreeExited(this HTTPRequest target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HTTPRequestAction>(target).OnTreeExited(action, oneShot, deferred);

        public static HTTPRequestAction OnTreeExiting(this HTTPRequest target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<HTTPRequestAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ImmediateGeometryAction OnGameplayEntered(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static ImmediateGeometryAction OnGameplayExited(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static ImmediateGeometryAction OnReady(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnReady(action, oneShot, deferred);

        public static ImmediateGeometryAction OnRenamed(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnRenamed(action, oneShot, deferred);

        public static ImmediateGeometryAction OnScriptChanged(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ImmediateGeometryAction OnTreeEntered(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ImmediateGeometryAction OnTreeExited(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ImmediateGeometryAction OnTreeExiting(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ImmediateGeometryAction OnVisibilityChanged(this ImmediateGeometry target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ImmediateGeometryAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static InterpolatedCameraAction OnGameplayEntered(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static InterpolatedCameraAction OnGameplayExited(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static InterpolatedCameraAction OnReady(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnReady(action, oneShot, deferred);

        public static InterpolatedCameraAction OnRenamed(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnRenamed(action, oneShot, deferred);

        public static InterpolatedCameraAction OnScriptChanged(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static InterpolatedCameraAction OnTreeEntered(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static InterpolatedCameraAction OnTreeExited(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnTreeExited(action, oneShot, deferred);

        public static InterpolatedCameraAction OnTreeExiting(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static InterpolatedCameraAction OnVisibilityChanged(this InterpolatedCamera target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<InterpolatedCameraAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ItemListAction OnDraw(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnDraw(action, oneShot, deferred);

        public static ItemListAction OnFocusEntered(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static ItemListAction OnFocusExited(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnFocusExited(action, oneShot, deferred);

        public static ItemListAction OnGuiInput(this ItemList target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnGuiInput(action, oneShot, deferred);

        public static ItemListAction OnHide(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnHide(action, oneShot, deferred);

        public static ItemListAction OnItemActivated(this ItemList target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnItemActivated(action, oneShot, deferred);

        public static ItemListAction OnItemRectChanged(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static ItemListAction OnItemRmbSelected(this ItemList target, Action<Vector2, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnItemRmbSelected(action, oneShot, deferred);

        public static ItemListAction OnItemSelected(this ItemList target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnItemSelected(action, oneShot, deferred);

        public static ItemListAction OnMinimumSizeChanged(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static ItemListAction OnModalClosed(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnModalClosed(action, oneShot, deferred);

        public static ItemListAction OnMouseEntered(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static ItemListAction OnMouseExited(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnMouseExited(action, oneShot, deferred);

        public static ItemListAction OnMultiSelected(this ItemList target, Action<int, bool> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnMultiSelected(action, oneShot, deferred);

        public static ItemListAction OnNothingSelected(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnNothingSelected(action, oneShot, deferred);

        public static ItemListAction OnReady(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnReady(action, oneShot, deferred);

        public static ItemListAction OnRenamed(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnRenamed(action, oneShot, deferred);

        public static ItemListAction OnResized(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnResized(action, oneShot, deferred);

        public static ItemListAction OnRmbClicked(this ItemList target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnRmbClicked(action, oneShot, deferred);

        public static ItemListAction OnScriptChanged(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ItemListAction OnSizeFlagsChanged(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static ItemListAction OnTreeEntered(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ItemListAction OnTreeExited(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ItemListAction OnTreeExiting(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ItemListAction OnVisibilityChanged(this ItemList target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ItemListAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static KinematicBodyAction OnGameplayEntered(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBodyAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static KinematicBodyAction OnGameplayExited(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBodyAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static KinematicBodyAction OnInputEvent(this KinematicBody target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBodyAction>(target).OnInputEvent(action, oneShot, deferred);

        public static KinematicBodyAction OnMouseEntered(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBodyAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static KinematicBodyAction OnMouseExited(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBodyAction>(target).OnMouseExited(action, oneShot, deferred);

        public static KinematicBodyAction OnReady(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBodyAction>(target).OnReady(action, oneShot, deferred);

        public static KinematicBodyAction OnRenamed(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBodyAction>(target).OnRenamed(action, oneShot, deferred);

        public static KinematicBodyAction OnScriptChanged(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBodyAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static KinematicBodyAction OnTreeEntered(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBodyAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static KinematicBodyAction OnTreeExited(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBodyAction>(target).OnTreeExited(action, oneShot, deferred);

        public static KinematicBodyAction OnTreeExiting(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBodyAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static KinematicBodyAction OnVisibilityChanged(this KinematicBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBodyAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static KinematicBody2DAction OnDraw(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnDraw(action, oneShot, deferred);

        public static KinematicBody2DAction OnHide(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnHide(action, oneShot, deferred);

        public static KinematicBody2DAction OnInputEvent(this KinematicBody2D target, Action<InputEvent, int, Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnInputEvent(action, oneShot, deferred);

        public static KinematicBody2DAction OnItemRectChanged(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static KinematicBody2DAction OnMouseEntered(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static KinematicBody2DAction OnMouseExited(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnMouseExited(action, oneShot, deferred);

        public static KinematicBody2DAction OnReady(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnReady(action, oneShot, deferred);

        public static KinematicBody2DAction OnRenamed(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static KinematicBody2DAction OnScriptChanged(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static KinematicBody2DAction OnTreeEntered(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static KinematicBody2DAction OnTreeExited(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static KinematicBody2DAction OnTreeExiting(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static KinematicBody2DAction OnVisibilityChanged(this KinematicBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<KinematicBody2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static LabelAction OnDraw(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnDraw(action, oneShot, deferred);

        public static LabelAction OnFocusEntered(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static LabelAction OnFocusExited(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnFocusExited(action, oneShot, deferred);

        public static LabelAction OnGuiInput(this Label target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnGuiInput(action, oneShot, deferred);

        public static LabelAction OnHide(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnHide(action, oneShot, deferred);

        public static LabelAction OnItemRectChanged(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static LabelAction OnMinimumSizeChanged(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static LabelAction OnModalClosed(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnModalClosed(action, oneShot, deferred);

        public static LabelAction OnMouseEntered(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static LabelAction OnMouseExited(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnMouseExited(action, oneShot, deferred);

        public static LabelAction OnReady(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnReady(action, oneShot, deferred);

        public static LabelAction OnRenamed(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnRenamed(action, oneShot, deferred);

        public static LabelAction OnResized(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnResized(action, oneShot, deferred);

        public static LabelAction OnScriptChanged(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static LabelAction OnSizeFlagsChanged(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static LabelAction OnTreeEntered(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static LabelAction OnTreeExited(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnTreeExited(action, oneShot, deferred);

        public static LabelAction OnTreeExiting(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static LabelAction OnVisibilityChanged(this Label target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LabelAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Light2DAction OnDraw(this Light2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Light2DAction>(target).OnDraw(action, oneShot, deferred);

        public static Light2DAction OnHide(this Light2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Light2DAction>(target).OnHide(action, oneShot, deferred);

        public static Light2DAction OnItemRectChanged(this Light2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Light2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static Light2DAction OnReady(this Light2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Light2DAction>(target).OnReady(action, oneShot, deferred);

        public static Light2DAction OnRenamed(this Light2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Light2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Light2DAction OnScriptChanged(this Light2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Light2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Light2DAction OnTreeEntered(this Light2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Light2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Light2DAction OnTreeExited(this Light2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Light2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Light2DAction OnTreeExiting(this Light2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Light2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Light2DAction OnVisibilityChanged(this Light2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Light2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static LightOccluder2DAction OnDraw(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnDraw(action, oneShot, deferred);

        public static LightOccluder2DAction OnHide(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnHide(action, oneShot, deferred);

        public static LightOccluder2DAction OnItemRectChanged(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static LightOccluder2DAction OnReady(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnReady(action, oneShot, deferred);

        public static LightOccluder2DAction OnRenamed(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static LightOccluder2DAction OnScriptChanged(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static LightOccluder2DAction OnTreeEntered(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static LightOccluder2DAction OnTreeExited(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static LightOccluder2DAction OnTreeExiting(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static LightOccluder2DAction OnVisibilityChanged(this LightOccluder2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LightOccluder2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Line2DAction OnDraw(this Line2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Line2DAction>(target).OnDraw(action, oneShot, deferred);

        public static Line2DAction OnHide(this Line2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Line2DAction>(target).OnHide(action, oneShot, deferred);

        public static Line2DAction OnItemRectChanged(this Line2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Line2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static Line2DAction OnReady(this Line2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Line2DAction>(target).OnReady(action, oneShot, deferred);

        public static Line2DAction OnRenamed(this Line2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Line2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Line2DAction OnScriptChanged(this Line2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Line2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Line2DAction OnTreeEntered(this Line2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Line2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Line2DAction OnTreeExited(this Line2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Line2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Line2DAction OnTreeExiting(this Line2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Line2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Line2DAction OnVisibilityChanged(this Line2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Line2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static LineEditAction OnDraw(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnDraw(action, oneShot, deferred);

        public static LineEditAction OnFocusEntered(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static LineEditAction OnFocusExited(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnFocusExited(action, oneShot, deferred);

        public static LineEditAction OnGuiInput(this LineEdit target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnGuiInput(action, oneShot, deferred);

        public static LineEditAction OnHide(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnHide(action, oneShot, deferred);

        public static LineEditAction OnItemRectChanged(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static LineEditAction OnMinimumSizeChanged(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static LineEditAction OnModalClosed(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnModalClosed(action, oneShot, deferred);

        public static LineEditAction OnMouseEntered(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static LineEditAction OnMouseExited(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnMouseExited(action, oneShot, deferred);

        public static LineEditAction OnReady(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnReady(action, oneShot, deferred);

        public static LineEditAction OnRenamed(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnRenamed(action, oneShot, deferred);

        public static LineEditAction OnResized(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnResized(action, oneShot, deferred);

        public static LineEditAction OnScriptChanged(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static LineEditAction OnSizeFlagsChanged(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static LineEditAction OnTextChanged(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnTextChanged(action, oneShot, deferred);

        public static LineEditAction OnTextChangeRejected(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnTextChangeRejected(action, oneShot, deferred);

        public static LineEditAction OnTextEntered(this LineEdit target, Action<string> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnTextEntered(action, oneShot, deferred);

        public static LineEditAction OnTreeEntered(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static LineEditAction OnTreeExited(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnTreeExited(action, oneShot, deferred);

        public static LineEditAction OnTreeExiting(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static LineEditAction OnVisibilityChanged(this LineEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LineEditAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static LinkButtonAction OnButtonDown(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnButtonDown(action, oneShot, deferred);

        public static LinkButtonAction OnButtonUp(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnButtonUp(action, oneShot, deferred);

        public static LinkButtonAction OnDraw(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnDraw(action, oneShot, deferred);

        public static LinkButtonAction OnFocusEntered(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static LinkButtonAction OnFocusExited(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnFocusExited(action, oneShot, deferred);

        public static LinkButtonAction OnGuiInput(this LinkButton target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnGuiInput(action, oneShot, deferred);

        public static LinkButtonAction OnHide(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnHide(action, oneShot, deferred);

        public static LinkButtonAction OnItemRectChanged(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static LinkButtonAction OnMinimumSizeChanged(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static LinkButtonAction OnModalClosed(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnModalClosed(action, oneShot, deferred);

        public static LinkButtonAction OnMouseEntered(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static LinkButtonAction OnMouseExited(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnMouseExited(action, oneShot, deferred);

        public static LinkButtonAction OnPressed(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnPressed(action, oneShot, deferred);

        public static LinkButtonAction OnReady(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnReady(action, oneShot, deferred);

        public static LinkButtonAction OnRenamed(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnRenamed(action, oneShot, deferred);

        public static LinkButtonAction OnResized(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnResized(action, oneShot, deferred);

        public static LinkButtonAction OnScriptChanged(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static LinkButtonAction OnSizeFlagsChanged(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static LinkButtonAction OnToggled(this LinkButton target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnToggled(action, oneShot, deferred);

        public static LinkButtonAction OnTreeEntered(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static LinkButtonAction OnTreeExited(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnTreeExited(action, oneShot, deferred);

        public static LinkButtonAction OnTreeExiting(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static LinkButtonAction OnVisibilityChanged(this LinkButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<LinkButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ListenerAction OnGameplayEntered(this Listener target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ListenerAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static ListenerAction OnGameplayExited(this Listener target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ListenerAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static ListenerAction OnReady(this Listener target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ListenerAction>(target).OnReady(action, oneShot, deferred);

        public static ListenerAction OnRenamed(this Listener target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ListenerAction>(target).OnRenamed(action, oneShot, deferred);

        public static ListenerAction OnScriptChanged(this Listener target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ListenerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ListenerAction OnTreeEntered(this Listener target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ListenerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ListenerAction OnTreeExited(this Listener target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ListenerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ListenerAction OnTreeExiting(this Listener target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ListenerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ListenerAction OnVisibilityChanged(this Listener target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ListenerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Listener2DAction OnDraw(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Listener2DAction>(target).OnDraw(action, oneShot, deferred);

        public static Listener2DAction OnHide(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Listener2DAction>(target).OnHide(action, oneShot, deferred);

        public static Listener2DAction OnItemRectChanged(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Listener2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static Listener2DAction OnReady(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Listener2DAction>(target).OnReady(action, oneShot, deferred);

        public static Listener2DAction OnRenamed(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Listener2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Listener2DAction OnScriptChanged(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Listener2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Listener2DAction OnTreeEntered(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Listener2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Listener2DAction OnTreeExited(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Listener2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Listener2DAction OnTreeExiting(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Listener2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Listener2DAction OnVisibilityChanged(this Listener2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Listener2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static MarginContainerAction OnDraw(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnDraw(action, oneShot, deferred);

        public static MarginContainerAction OnFocusEntered(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static MarginContainerAction OnFocusExited(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static MarginContainerAction OnGuiInput(this MarginContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static MarginContainerAction OnHide(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnHide(action, oneShot, deferred);

        public static MarginContainerAction OnItemRectChanged(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static MarginContainerAction OnMinimumSizeChanged(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static MarginContainerAction OnModalClosed(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static MarginContainerAction OnMouseEntered(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static MarginContainerAction OnMouseExited(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static MarginContainerAction OnReady(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnReady(action, oneShot, deferred);

        public static MarginContainerAction OnRenamed(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnRenamed(action, oneShot, deferred);

        public static MarginContainerAction OnResized(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnResized(action, oneShot, deferred);

        public static MarginContainerAction OnScriptChanged(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static MarginContainerAction OnSizeFlagsChanged(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static MarginContainerAction OnSortChildren(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnSortChildren(action, oneShot, deferred);

        public static MarginContainerAction OnTreeEntered(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static MarginContainerAction OnTreeExited(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static MarginContainerAction OnTreeExiting(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static MarginContainerAction OnVisibilityChanged(this MarginContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MarginContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static MenuButtonAction OnAboutToShow(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnAboutToShow(action, oneShot, deferred);

        public static MenuButtonAction OnButtonDown(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnButtonDown(action, oneShot, deferred);

        public static MenuButtonAction OnButtonUp(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnButtonUp(action, oneShot, deferred);

        public static MenuButtonAction OnDraw(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnDraw(action, oneShot, deferred);

        public static MenuButtonAction OnFocusEntered(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static MenuButtonAction OnFocusExited(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnFocusExited(action, oneShot, deferred);

        public static MenuButtonAction OnGuiInput(this MenuButton target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnGuiInput(action, oneShot, deferred);

        public static MenuButtonAction OnHide(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnHide(action, oneShot, deferred);

        public static MenuButtonAction OnItemRectChanged(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static MenuButtonAction OnMinimumSizeChanged(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static MenuButtonAction OnModalClosed(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnModalClosed(action, oneShot, deferred);

        public static MenuButtonAction OnMouseEntered(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static MenuButtonAction OnMouseExited(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnMouseExited(action, oneShot, deferred);

        public static MenuButtonAction OnPressed(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnPressed(action, oneShot, deferred);

        public static MenuButtonAction OnReady(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnReady(action, oneShot, deferred);

        public static MenuButtonAction OnRenamed(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnRenamed(action, oneShot, deferred);

        public static MenuButtonAction OnResized(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnResized(action, oneShot, deferred);

        public static MenuButtonAction OnScriptChanged(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static MenuButtonAction OnSizeFlagsChanged(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static MenuButtonAction OnToggled(this MenuButton target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnToggled(action, oneShot, deferred);

        public static MenuButtonAction OnTreeEntered(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static MenuButtonAction OnTreeExited(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnTreeExited(action, oneShot, deferred);

        public static MenuButtonAction OnTreeExiting(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static MenuButtonAction OnVisibilityChanged(this MenuButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MenuButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static MeshInstanceAction OnGameplayEntered(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstanceAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static MeshInstanceAction OnGameplayExited(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstanceAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static MeshInstanceAction OnReady(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstanceAction>(target).OnReady(action, oneShot, deferred);

        public static MeshInstanceAction OnRenamed(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstanceAction>(target).OnRenamed(action, oneShot, deferred);

        public static MeshInstanceAction OnScriptChanged(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstanceAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static MeshInstanceAction OnTreeEntered(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstanceAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static MeshInstanceAction OnTreeExited(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstanceAction>(target).OnTreeExited(action, oneShot, deferred);

        public static MeshInstanceAction OnTreeExiting(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstanceAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static MeshInstanceAction OnVisibilityChanged(this MeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstanceAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static MeshInstance2DAction OnDraw(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnDraw(action, oneShot, deferred);

        public static MeshInstance2DAction OnHide(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnHide(action, oneShot, deferred);

        public static MeshInstance2DAction OnItemRectChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static MeshInstance2DAction OnReady(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnReady(action, oneShot, deferred);

        public static MeshInstance2DAction OnRenamed(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static MeshInstance2DAction OnScriptChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static MeshInstance2DAction OnTextureChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnTextureChanged(action, oneShot, deferred);

        public static MeshInstance2DAction OnTreeEntered(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static MeshInstance2DAction OnTreeExited(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static MeshInstance2DAction OnTreeExiting(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static MeshInstance2DAction OnVisibilityChanged(this MeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MeshInstance2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static MultiMeshInstanceAction OnGameplayEntered(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static MultiMeshInstanceAction OnGameplayExited(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static MultiMeshInstanceAction OnReady(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnReady(action, oneShot, deferred);

        public static MultiMeshInstanceAction OnRenamed(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnRenamed(action, oneShot, deferred);

        public static MultiMeshInstanceAction OnScriptChanged(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static MultiMeshInstanceAction OnTreeEntered(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static MultiMeshInstanceAction OnTreeExited(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnTreeExited(action, oneShot, deferred);

        public static MultiMeshInstanceAction OnTreeExiting(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static MultiMeshInstanceAction OnVisibilityChanged(this MultiMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstanceAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static MultiMeshInstance2DAction OnDraw(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnDraw(action, oneShot, deferred);

        public static MultiMeshInstance2DAction OnHide(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnHide(action, oneShot, deferred);

        public static MultiMeshInstance2DAction OnItemRectChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static MultiMeshInstance2DAction OnReady(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnReady(action, oneShot, deferred);

        public static MultiMeshInstance2DAction OnRenamed(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static MultiMeshInstance2DAction OnScriptChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static MultiMeshInstance2DAction OnTextureChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnTextureChanged(action, oneShot, deferred);

        public static MultiMeshInstance2DAction OnTreeEntered(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static MultiMeshInstance2DAction OnTreeExited(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static MultiMeshInstance2DAction OnTreeExiting(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static MultiMeshInstance2DAction OnVisibilityChanged(this MultiMeshInstance2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<MultiMeshInstance2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static NavigationAction OnGameplayEntered(this Navigation target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static NavigationAction OnGameplayExited(this Navigation target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static NavigationAction OnReady(this Navigation target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationAction>(target).OnReady(action, oneShot, deferred);

        public static NavigationAction OnRenamed(this Navigation target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationAction>(target).OnRenamed(action, oneShot, deferred);

        public static NavigationAction OnScriptChanged(this Navigation target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static NavigationAction OnTreeEntered(this Navigation target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static NavigationAction OnTreeExited(this Navigation target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationAction>(target).OnTreeExited(action, oneShot, deferred);

        public static NavigationAction OnTreeExiting(this Navigation target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static NavigationAction OnVisibilityChanged(this Navigation target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Navigation2DAction OnDraw(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Navigation2DAction>(target).OnDraw(action, oneShot, deferred);

        public static Navigation2DAction OnHide(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Navigation2DAction>(target).OnHide(action, oneShot, deferred);

        public static Navigation2DAction OnItemRectChanged(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Navigation2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static Navigation2DAction OnReady(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Navigation2DAction>(target).OnReady(action, oneShot, deferred);

        public static Navigation2DAction OnRenamed(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Navigation2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Navigation2DAction OnScriptChanged(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Navigation2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Navigation2DAction OnTreeEntered(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Navigation2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Navigation2DAction OnTreeExited(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Navigation2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Navigation2DAction OnTreeExiting(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Navigation2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Navigation2DAction OnVisibilityChanged(this Navigation2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Navigation2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static NavigationMeshInstanceAction OnGameplayEntered(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static NavigationMeshInstanceAction OnGameplayExited(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static NavigationMeshInstanceAction OnReady(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnReady(action, oneShot, deferred);

        public static NavigationMeshInstanceAction OnRenamed(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnRenamed(action, oneShot, deferred);

        public static NavigationMeshInstanceAction OnScriptChanged(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static NavigationMeshInstanceAction OnTreeEntered(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static NavigationMeshInstanceAction OnTreeExited(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnTreeExited(action, oneShot, deferred);

        public static NavigationMeshInstanceAction OnTreeExiting(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static NavigationMeshInstanceAction OnVisibilityChanged(this NavigationMeshInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationMeshInstanceAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static NavigationPolygonInstanceAction OnDraw(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnDraw(action, oneShot, deferred);

        public static NavigationPolygonInstanceAction OnHide(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnHide(action, oneShot, deferred);

        public static NavigationPolygonInstanceAction OnItemRectChanged(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static NavigationPolygonInstanceAction OnReady(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnReady(action, oneShot, deferred);

        public static NavigationPolygonInstanceAction OnRenamed(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnRenamed(action, oneShot, deferred);

        public static NavigationPolygonInstanceAction OnScriptChanged(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static NavigationPolygonInstanceAction OnTreeEntered(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static NavigationPolygonInstanceAction OnTreeExited(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnTreeExited(action, oneShot, deferred);

        public static NavigationPolygonInstanceAction OnTreeExiting(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static NavigationPolygonInstanceAction OnVisibilityChanged(this NavigationPolygonInstance target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NavigationPolygonInstanceAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static NinePatchRectAction OnDraw(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnDraw(action, oneShot, deferred);

        public static NinePatchRectAction OnFocusEntered(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static NinePatchRectAction OnFocusExited(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnFocusExited(action, oneShot, deferred);

        public static NinePatchRectAction OnGuiInput(this NinePatchRect target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnGuiInput(action, oneShot, deferred);

        public static NinePatchRectAction OnHide(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnHide(action, oneShot, deferred);

        public static NinePatchRectAction OnItemRectChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static NinePatchRectAction OnMinimumSizeChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static NinePatchRectAction OnModalClosed(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnModalClosed(action, oneShot, deferred);

        public static NinePatchRectAction OnMouseEntered(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static NinePatchRectAction OnMouseExited(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnMouseExited(action, oneShot, deferred);

        public static NinePatchRectAction OnReady(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnReady(action, oneShot, deferred);

        public static NinePatchRectAction OnRenamed(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnRenamed(action, oneShot, deferred);

        public static NinePatchRectAction OnResized(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnResized(action, oneShot, deferred);

        public static NinePatchRectAction OnScriptChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static NinePatchRectAction OnSizeFlagsChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static NinePatchRectAction OnTextureChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnTextureChanged(action, oneShot, deferred);

        public static NinePatchRectAction OnTreeEntered(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static NinePatchRectAction OnTreeExited(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnTreeExited(action, oneShot, deferred);

        public static NinePatchRectAction OnTreeExiting(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static NinePatchRectAction OnVisibilityChanged(this NinePatchRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<NinePatchRectAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Node2DAction OnDraw(this Node2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Node2DAction>(target).OnDraw(action, oneShot, deferred);

        public static Node2DAction OnHide(this Node2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Node2DAction>(target).OnHide(action, oneShot, deferred);

        public static Node2DAction OnItemRectChanged(this Node2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Node2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static Node2DAction OnReady(this Node2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Node2DAction>(target).OnReady(action, oneShot, deferred);

        public static Node2DAction OnRenamed(this Node2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Node2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Node2DAction OnScriptChanged(this Node2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Node2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Node2DAction OnTreeEntered(this Node2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Node2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Node2DAction OnTreeExited(this Node2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Node2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Node2DAction OnTreeExiting(this Node2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Node2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Node2DAction OnVisibilityChanged(this Node2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Node2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static OccluderAction OnGameplayEntered(this Occluder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OccluderAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static OccluderAction OnGameplayExited(this Occluder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OccluderAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static OccluderAction OnReady(this Occluder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OccluderAction>(target).OnReady(action, oneShot, deferred);

        public static OccluderAction OnRenamed(this Occluder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OccluderAction>(target).OnRenamed(action, oneShot, deferred);

        public static OccluderAction OnScriptChanged(this Occluder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OccluderAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static OccluderAction OnTreeEntered(this Occluder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OccluderAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static OccluderAction OnTreeExited(this Occluder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OccluderAction>(target).OnTreeExited(action, oneShot, deferred);

        public static OccluderAction OnTreeExiting(this Occluder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OccluderAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static OccluderAction OnVisibilityChanged(this Occluder target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OccluderAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static OmniLightAction OnGameplayEntered(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OmniLightAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static OmniLightAction OnGameplayExited(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OmniLightAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static OmniLightAction OnReady(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OmniLightAction>(target).OnReady(action, oneShot, deferred);

        public static OmniLightAction OnRenamed(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OmniLightAction>(target).OnRenamed(action, oneShot, deferred);

        public static OmniLightAction OnScriptChanged(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OmniLightAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static OmniLightAction OnTreeEntered(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OmniLightAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static OmniLightAction OnTreeExited(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OmniLightAction>(target).OnTreeExited(action, oneShot, deferred);

        public static OmniLightAction OnTreeExiting(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OmniLightAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static OmniLightAction OnVisibilityChanged(this OmniLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OmniLightAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static OptionButtonAction OnButtonDown(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnButtonDown(action, oneShot, deferred);

        public static OptionButtonAction OnButtonUp(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnButtonUp(action, oneShot, deferred);

        public static OptionButtonAction OnDraw(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnDraw(action, oneShot, deferred);

        public static OptionButtonAction OnFocusEntered(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static OptionButtonAction OnFocusExited(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnFocusExited(action, oneShot, deferred);

        public static OptionButtonAction OnGuiInput(this OptionButton target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnGuiInput(action, oneShot, deferred);

        public static OptionButtonAction OnHide(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnHide(action, oneShot, deferred);

        public static OptionButtonAction OnItemFocused(this OptionButton target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnItemFocused(action, oneShot, deferred);

        public static OptionButtonAction OnItemRectChanged(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static OptionButtonAction OnItemSelected(this OptionButton target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnItemSelected(action, oneShot, deferred);

        public static OptionButtonAction OnMinimumSizeChanged(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static OptionButtonAction OnModalClosed(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnModalClosed(action, oneShot, deferred);

        public static OptionButtonAction OnMouseEntered(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static OptionButtonAction OnMouseExited(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnMouseExited(action, oneShot, deferred);

        public static OptionButtonAction OnPressed(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnPressed(action, oneShot, deferred);

        public static OptionButtonAction OnReady(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnReady(action, oneShot, deferred);

        public static OptionButtonAction OnRenamed(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnRenamed(action, oneShot, deferred);

        public static OptionButtonAction OnResized(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnResized(action, oneShot, deferred);

        public static OptionButtonAction OnScriptChanged(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static OptionButtonAction OnSizeFlagsChanged(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static OptionButtonAction OnToggled(this OptionButton target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnToggled(action, oneShot, deferred);

        public static OptionButtonAction OnTreeEntered(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static OptionButtonAction OnTreeExited(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnTreeExited(action, oneShot, deferred);

        public static OptionButtonAction OnTreeExiting(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static OptionButtonAction OnVisibilityChanged(this OptionButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<OptionButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static PanelAction OnDraw(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnDraw(action, oneShot, deferred);

        public static PanelAction OnFocusEntered(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static PanelAction OnFocusExited(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnFocusExited(action, oneShot, deferred);

        public static PanelAction OnGuiInput(this Panel target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnGuiInput(action, oneShot, deferred);

        public static PanelAction OnHide(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnHide(action, oneShot, deferred);

        public static PanelAction OnItemRectChanged(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static PanelAction OnMinimumSizeChanged(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static PanelAction OnModalClosed(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnModalClosed(action, oneShot, deferred);

        public static PanelAction OnMouseEntered(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static PanelAction OnMouseExited(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnMouseExited(action, oneShot, deferred);

        public static PanelAction OnReady(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnReady(action, oneShot, deferred);

        public static PanelAction OnRenamed(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnRenamed(action, oneShot, deferred);

        public static PanelAction OnResized(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnResized(action, oneShot, deferred);

        public static PanelAction OnScriptChanged(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static PanelAction OnSizeFlagsChanged(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static PanelAction OnTreeEntered(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static PanelAction OnTreeExited(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnTreeExited(action, oneShot, deferred);

        public static PanelAction OnTreeExiting(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static PanelAction OnVisibilityChanged(this Panel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static PanelContainerAction OnDraw(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnDraw(action, oneShot, deferred);

        public static PanelContainerAction OnFocusEntered(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static PanelContainerAction OnFocusExited(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static PanelContainerAction OnGuiInput(this PanelContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static PanelContainerAction OnHide(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnHide(action, oneShot, deferred);

        public static PanelContainerAction OnItemRectChanged(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static PanelContainerAction OnMinimumSizeChanged(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static PanelContainerAction OnModalClosed(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static PanelContainerAction OnMouseEntered(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static PanelContainerAction OnMouseExited(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static PanelContainerAction OnReady(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnReady(action, oneShot, deferred);

        public static PanelContainerAction OnRenamed(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnRenamed(action, oneShot, deferred);

        public static PanelContainerAction OnResized(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnResized(action, oneShot, deferred);

        public static PanelContainerAction OnScriptChanged(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static PanelContainerAction OnSizeFlagsChanged(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static PanelContainerAction OnSortChildren(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnSortChildren(action, oneShot, deferred);

        public static PanelContainerAction OnTreeEntered(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static PanelContainerAction OnTreeExited(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static PanelContainerAction OnTreeExiting(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static PanelContainerAction OnVisibilityChanged(this PanelContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PanelContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ParallaxBackgroundAction OnReady(this ParallaxBackground target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxBackgroundAction>(target).OnReady(action, oneShot, deferred);

        public static ParallaxBackgroundAction OnRenamed(this ParallaxBackground target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxBackgroundAction>(target).OnRenamed(action, oneShot, deferred);

        public static ParallaxBackgroundAction OnScriptChanged(this ParallaxBackground target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxBackgroundAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ParallaxBackgroundAction OnTreeEntered(this ParallaxBackground target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxBackgroundAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ParallaxBackgroundAction OnTreeExited(this ParallaxBackground target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxBackgroundAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ParallaxBackgroundAction OnTreeExiting(this ParallaxBackground target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxBackgroundAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ParallaxLayerAction OnDraw(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnDraw(action, oneShot, deferred);

        public static ParallaxLayerAction OnHide(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnHide(action, oneShot, deferred);

        public static ParallaxLayerAction OnItemRectChanged(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static ParallaxLayerAction OnReady(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnReady(action, oneShot, deferred);

        public static ParallaxLayerAction OnRenamed(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnRenamed(action, oneShot, deferred);

        public static ParallaxLayerAction OnScriptChanged(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ParallaxLayerAction OnTreeEntered(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ParallaxLayerAction OnTreeExited(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ParallaxLayerAction OnTreeExiting(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ParallaxLayerAction OnVisibilityChanged(this ParallaxLayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParallaxLayerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ParticlesAction OnGameplayEntered(this Particles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParticlesAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static ParticlesAction OnGameplayExited(this Particles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParticlesAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static ParticlesAction OnReady(this Particles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParticlesAction>(target).OnReady(action, oneShot, deferred);

        public static ParticlesAction OnRenamed(this Particles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParticlesAction>(target).OnRenamed(action, oneShot, deferred);

        public static ParticlesAction OnScriptChanged(this Particles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParticlesAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ParticlesAction OnTreeEntered(this Particles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParticlesAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ParticlesAction OnTreeExited(this Particles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParticlesAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ParticlesAction OnTreeExiting(this Particles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParticlesAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ParticlesAction OnVisibilityChanged(this Particles target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ParticlesAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Particles2DAction OnDraw(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Particles2DAction>(target).OnDraw(action, oneShot, deferred);

        public static Particles2DAction OnHide(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Particles2DAction>(target).OnHide(action, oneShot, deferred);

        public static Particles2DAction OnItemRectChanged(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Particles2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static Particles2DAction OnReady(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Particles2DAction>(target).OnReady(action, oneShot, deferred);

        public static Particles2DAction OnRenamed(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Particles2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Particles2DAction OnScriptChanged(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Particles2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Particles2DAction OnTreeEntered(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Particles2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Particles2DAction OnTreeExited(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Particles2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Particles2DAction OnTreeExiting(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Particles2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Particles2DAction OnVisibilityChanged(this Particles2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Particles2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static PathAction OnCurveChanged(this Path target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathAction>(target).OnCurveChanged(action, oneShot, deferred);

        public static PathAction OnGameplayEntered(this Path target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static PathAction OnGameplayExited(this Path target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static PathAction OnReady(this Path target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathAction>(target).OnReady(action, oneShot, deferred);

        public static PathAction OnRenamed(this Path target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathAction>(target).OnRenamed(action, oneShot, deferred);

        public static PathAction OnScriptChanged(this Path target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static PathAction OnTreeEntered(this Path target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static PathAction OnTreeExited(this Path target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathAction>(target).OnTreeExited(action, oneShot, deferred);

        public static PathAction OnTreeExiting(this Path target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static PathAction OnVisibilityChanged(this Path target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Path2DAction OnDraw(this Path2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Path2DAction>(target).OnDraw(action, oneShot, deferred);

        public static Path2DAction OnHide(this Path2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Path2DAction>(target).OnHide(action, oneShot, deferred);

        public static Path2DAction OnItemRectChanged(this Path2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Path2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static Path2DAction OnReady(this Path2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Path2DAction>(target).OnReady(action, oneShot, deferred);

        public static Path2DAction OnRenamed(this Path2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Path2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Path2DAction OnScriptChanged(this Path2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Path2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Path2DAction OnTreeEntered(this Path2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Path2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Path2DAction OnTreeExited(this Path2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Path2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Path2DAction OnTreeExiting(this Path2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Path2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Path2DAction OnVisibilityChanged(this Path2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Path2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static PathFollowAction OnGameplayEntered(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollowAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static PathFollowAction OnGameplayExited(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollowAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static PathFollowAction OnReady(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollowAction>(target).OnReady(action, oneShot, deferred);

        public static PathFollowAction OnRenamed(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollowAction>(target).OnRenamed(action, oneShot, deferred);

        public static PathFollowAction OnScriptChanged(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollowAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static PathFollowAction OnTreeEntered(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollowAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static PathFollowAction OnTreeExited(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollowAction>(target).OnTreeExited(action, oneShot, deferred);

        public static PathFollowAction OnTreeExiting(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollowAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static PathFollowAction OnVisibilityChanged(this PathFollow target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollowAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static PathFollow2DAction OnDraw(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollow2DAction>(target).OnDraw(action, oneShot, deferred);

        public static PathFollow2DAction OnHide(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollow2DAction>(target).OnHide(action, oneShot, deferred);

        public static PathFollow2DAction OnItemRectChanged(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollow2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static PathFollow2DAction OnReady(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollow2DAction>(target).OnReady(action, oneShot, deferred);

        public static PathFollow2DAction OnRenamed(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollow2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static PathFollow2DAction OnScriptChanged(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollow2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static PathFollow2DAction OnTreeEntered(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollow2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static PathFollow2DAction OnTreeExited(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollow2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static PathFollow2DAction OnTreeExiting(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollow2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static PathFollow2DAction OnVisibilityChanged(this PathFollow2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PathFollow2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static PhysicalBoneAction OnGameplayEntered(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static PhysicalBoneAction OnGameplayExited(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static PhysicalBoneAction OnInputEvent(this PhysicalBone target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnInputEvent(action, oneShot, deferred);

        public static PhysicalBoneAction OnMouseEntered(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static PhysicalBoneAction OnMouseExited(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnMouseExited(action, oneShot, deferred);

        public static PhysicalBoneAction OnReady(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnReady(action, oneShot, deferred);

        public static PhysicalBoneAction OnRenamed(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnRenamed(action, oneShot, deferred);

        public static PhysicalBoneAction OnScriptChanged(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static PhysicalBoneAction OnTreeEntered(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static PhysicalBoneAction OnTreeExited(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnTreeExited(action, oneShot, deferred);

        public static PhysicalBoneAction OnTreeExiting(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static PhysicalBoneAction OnVisibilityChanged(this PhysicalBone target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PhysicalBoneAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static PinJointAction OnGameplayEntered(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJointAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static PinJointAction OnGameplayExited(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJointAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static PinJointAction OnReady(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJointAction>(target).OnReady(action, oneShot, deferred);

        public static PinJointAction OnRenamed(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJointAction>(target).OnRenamed(action, oneShot, deferred);

        public static PinJointAction OnScriptChanged(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJointAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static PinJointAction OnTreeEntered(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJointAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static PinJointAction OnTreeExited(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJointAction>(target).OnTreeExited(action, oneShot, deferred);

        public static PinJointAction OnTreeExiting(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJointAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static PinJointAction OnVisibilityChanged(this PinJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJointAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static PinJoint2DAction OnDraw(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJoint2DAction>(target).OnDraw(action, oneShot, deferred);

        public static PinJoint2DAction OnHide(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJoint2DAction>(target).OnHide(action, oneShot, deferred);

        public static PinJoint2DAction OnItemRectChanged(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJoint2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static PinJoint2DAction OnReady(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJoint2DAction>(target).OnReady(action, oneShot, deferred);

        public static PinJoint2DAction OnRenamed(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJoint2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static PinJoint2DAction OnScriptChanged(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJoint2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static PinJoint2DAction OnTreeEntered(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJoint2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static PinJoint2DAction OnTreeExited(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJoint2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static PinJoint2DAction OnTreeExiting(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJoint2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static PinJoint2DAction OnVisibilityChanged(this PinJoint2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PinJoint2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Polygon2DAction OnDraw(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Polygon2DAction>(target).OnDraw(action, oneShot, deferred);

        public static Polygon2DAction OnHide(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Polygon2DAction>(target).OnHide(action, oneShot, deferred);

        public static Polygon2DAction OnItemRectChanged(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Polygon2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static Polygon2DAction OnReady(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Polygon2DAction>(target).OnReady(action, oneShot, deferred);

        public static Polygon2DAction OnRenamed(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Polygon2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Polygon2DAction OnScriptChanged(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Polygon2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Polygon2DAction OnTreeEntered(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Polygon2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Polygon2DAction OnTreeExited(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Polygon2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Polygon2DAction OnTreeExiting(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Polygon2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Polygon2DAction OnVisibilityChanged(this Polygon2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Polygon2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static PopupAction OnAboutToShow(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnAboutToShow(action, oneShot, deferred);

        public static PopupAction OnDraw(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnDraw(action, oneShot, deferred);

        public static PopupAction OnFocusEntered(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static PopupAction OnFocusExited(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnFocusExited(action, oneShot, deferred);

        public static PopupAction OnGuiInput(this Popup target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnGuiInput(action, oneShot, deferred);

        public static PopupAction OnHide(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnHide(action, oneShot, deferred);

        public static PopupAction OnItemRectChanged(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static PopupAction OnMinimumSizeChanged(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static PopupAction OnModalClosed(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnModalClosed(action, oneShot, deferred);

        public static PopupAction OnMouseEntered(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static PopupAction OnMouseExited(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnMouseExited(action, oneShot, deferred);

        public static PopupAction OnPopupHide(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnPopupHide(action, oneShot, deferred);

        public static PopupAction OnReady(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnReady(action, oneShot, deferred);

        public static PopupAction OnRenamed(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnRenamed(action, oneShot, deferred);

        public static PopupAction OnResized(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnResized(action, oneShot, deferred);

        public static PopupAction OnScriptChanged(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static PopupAction OnSizeFlagsChanged(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static PopupAction OnTreeEntered(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static PopupAction OnTreeExited(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnTreeExited(action, oneShot, deferred);

        public static PopupAction OnTreeExiting(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static PopupAction OnVisibilityChanged(this Popup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static PopupDialogAction OnAboutToShow(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnAboutToShow(action, oneShot, deferred);

        public static PopupDialogAction OnDraw(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnDraw(action, oneShot, deferred);

        public static PopupDialogAction OnFocusEntered(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static PopupDialogAction OnFocusExited(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnFocusExited(action, oneShot, deferred);

        public static PopupDialogAction OnGuiInput(this PopupDialog target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnGuiInput(action, oneShot, deferred);

        public static PopupDialogAction OnHide(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnHide(action, oneShot, deferred);

        public static PopupDialogAction OnItemRectChanged(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static PopupDialogAction OnMinimumSizeChanged(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static PopupDialogAction OnModalClosed(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnModalClosed(action, oneShot, deferred);

        public static PopupDialogAction OnMouseEntered(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static PopupDialogAction OnMouseExited(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnMouseExited(action, oneShot, deferred);

        public static PopupDialogAction OnPopupHide(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnPopupHide(action, oneShot, deferred);

        public static PopupDialogAction OnReady(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnReady(action, oneShot, deferred);

        public static PopupDialogAction OnRenamed(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnRenamed(action, oneShot, deferred);

        public static PopupDialogAction OnResized(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnResized(action, oneShot, deferred);

        public static PopupDialogAction OnScriptChanged(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static PopupDialogAction OnSizeFlagsChanged(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static PopupDialogAction OnTreeEntered(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static PopupDialogAction OnTreeExited(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnTreeExited(action, oneShot, deferred);

        public static PopupDialogAction OnTreeExiting(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static PopupDialogAction OnVisibilityChanged(this PopupDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupDialogAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static PopupMenuAction OnAboutToShow(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnAboutToShow(action, oneShot, deferred);

        public static PopupMenuAction OnDraw(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnDraw(action, oneShot, deferred);

        public static PopupMenuAction OnFocusEntered(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static PopupMenuAction OnFocusExited(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnFocusExited(action, oneShot, deferred);

        public static PopupMenuAction OnGuiInput(this PopupMenu target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnGuiInput(action, oneShot, deferred);

        public static PopupMenuAction OnHide(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnHide(action, oneShot, deferred);

        public static PopupMenuAction OnIdFocused(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnIdFocused(action, oneShot, deferred);

        public static PopupMenuAction OnIdPressed(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnIdPressed(action, oneShot, deferred);

        public static PopupMenuAction OnIndexPressed(this PopupMenu target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnIndexPressed(action, oneShot, deferred);

        public static PopupMenuAction OnItemRectChanged(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static PopupMenuAction OnMinimumSizeChanged(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static PopupMenuAction OnModalClosed(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnModalClosed(action, oneShot, deferred);

        public static PopupMenuAction OnMouseEntered(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static PopupMenuAction OnMouseExited(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnMouseExited(action, oneShot, deferred);

        public static PopupMenuAction OnPopupHide(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnPopupHide(action, oneShot, deferred);

        public static PopupMenuAction OnReady(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnReady(action, oneShot, deferred);

        public static PopupMenuAction OnRenamed(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnRenamed(action, oneShot, deferred);

        public static PopupMenuAction OnResized(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnResized(action, oneShot, deferred);

        public static PopupMenuAction OnScriptChanged(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static PopupMenuAction OnSizeFlagsChanged(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static PopupMenuAction OnTreeEntered(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static PopupMenuAction OnTreeExited(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnTreeExited(action, oneShot, deferred);

        public static PopupMenuAction OnTreeExiting(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static PopupMenuAction OnVisibilityChanged(this PopupMenu target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupMenuAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static PopupPanelAction OnAboutToShow(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnAboutToShow(action, oneShot, deferred);

        public static PopupPanelAction OnDraw(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnDraw(action, oneShot, deferred);

        public static PopupPanelAction OnFocusEntered(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static PopupPanelAction OnFocusExited(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnFocusExited(action, oneShot, deferred);

        public static PopupPanelAction OnGuiInput(this PopupPanel target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnGuiInput(action, oneShot, deferred);

        public static PopupPanelAction OnHide(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnHide(action, oneShot, deferred);

        public static PopupPanelAction OnItemRectChanged(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static PopupPanelAction OnMinimumSizeChanged(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static PopupPanelAction OnModalClosed(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnModalClosed(action, oneShot, deferred);

        public static PopupPanelAction OnMouseEntered(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static PopupPanelAction OnMouseExited(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnMouseExited(action, oneShot, deferred);

        public static PopupPanelAction OnPopupHide(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnPopupHide(action, oneShot, deferred);

        public static PopupPanelAction OnReady(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnReady(action, oneShot, deferred);

        public static PopupPanelAction OnRenamed(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnRenamed(action, oneShot, deferred);

        public static PopupPanelAction OnResized(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnResized(action, oneShot, deferred);

        public static PopupPanelAction OnScriptChanged(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static PopupPanelAction OnSizeFlagsChanged(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static PopupPanelAction OnTreeEntered(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static PopupPanelAction OnTreeExited(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnTreeExited(action, oneShot, deferred);

        public static PopupPanelAction OnTreeExiting(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static PopupPanelAction OnVisibilityChanged(this PopupPanel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PopupPanelAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static PortalAction OnGameplayEntered(this Portal target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PortalAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static PortalAction OnGameplayExited(this Portal target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PortalAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static PortalAction OnReady(this Portal target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PortalAction>(target).OnReady(action, oneShot, deferred);

        public static PortalAction OnRenamed(this Portal target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PortalAction>(target).OnRenamed(action, oneShot, deferred);

        public static PortalAction OnScriptChanged(this Portal target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PortalAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static PortalAction OnTreeEntered(this Portal target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PortalAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static PortalAction OnTreeExited(this Portal target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PortalAction>(target).OnTreeExited(action, oneShot, deferred);

        public static PortalAction OnTreeExiting(this Portal target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PortalAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static PortalAction OnVisibilityChanged(this Portal target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<PortalAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Position2DAction OnDraw(this Position2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position2DAction>(target).OnDraw(action, oneShot, deferred);

        public static Position2DAction OnHide(this Position2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position2DAction>(target).OnHide(action, oneShot, deferred);

        public static Position2DAction OnItemRectChanged(this Position2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static Position2DAction OnReady(this Position2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position2DAction>(target).OnReady(action, oneShot, deferred);

        public static Position2DAction OnRenamed(this Position2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Position2DAction OnScriptChanged(this Position2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Position2DAction OnTreeEntered(this Position2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Position2DAction OnTreeExited(this Position2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Position2DAction OnTreeExiting(this Position2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Position2DAction OnVisibilityChanged(this Position2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Position3DAction OnGameplayEntered(this Position3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position3DAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static Position3DAction OnGameplayExited(this Position3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position3DAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static Position3DAction OnReady(this Position3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position3DAction>(target).OnReady(action, oneShot, deferred);

        public static Position3DAction OnRenamed(this Position3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position3DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Position3DAction OnScriptChanged(this Position3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position3DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Position3DAction OnTreeEntered(this Position3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position3DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Position3DAction OnTreeExited(this Position3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position3DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Position3DAction OnTreeExiting(this Position3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position3DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Position3DAction OnVisibilityChanged(this Position3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Position3DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ProgressBarAction OnChanged(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnChanged(action, oneShot, deferred);

        public static ProgressBarAction OnDraw(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnDraw(action, oneShot, deferred);

        public static ProgressBarAction OnFocusEntered(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static ProgressBarAction OnFocusExited(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnFocusExited(action, oneShot, deferred);

        public static ProgressBarAction OnGuiInput(this ProgressBar target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnGuiInput(action, oneShot, deferred);

        public static ProgressBarAction OnHide(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnHide(action, oneShot, deferred);

        public static ProgressBarAction OnItemRectChanged(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static ProgressBarAction OnMinimumSizeChanged(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static ProgressBarAction OnModalClosed(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnModalClosed(action, oneShot, deferred);

        public static ProgressBarAction OnMouseEntered(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static ProgressBarAction OnMouseExited(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnMouseExited(action, oneShot, deferred);

        public static ProgressBarAction OnReady(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnReady(action, oneShot, deferred);

        public static ProgressBarAction OnRenamed(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnRenamed(action, oneShot, deferred);

        public static ProgressBarAction OnResized(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnResized(action, oneShot, deferred);

        public static ProgressBarAction OnScriptChanged(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ProgressBarAction OnSizeFlagsChanged(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static ProgressBarAction OnTreeEntered(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ProgressBarAction OnTreeExited(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ProgressBarAction OnTreeExiting(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ProgressBarAction OnValueChanged(this ProgressBar target, Action<float> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnValueChanged(action, oneShot, deferred);

        public static ProgressBarAction OnVisibilityChanged(this ProgressBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProgressBarAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ProximityGroupAction OnBroadcast(this ProximityGroup target, Action<string, Godot.Collections.Array> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProximityGroupAction>(target).OnBroadcast(action, oneShot, deferred);

        public static ProximityGroupAction OnGameplayEntered(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProximityGroupAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static ProximityGroupAction OnGameplayExited(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProximityGroupAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static ProximityGroupAction OnReady(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProximityGroupAction>(target).OnReady(action, oneShot, deferred);

        public static ProximityGroupAction OnRenamed(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProximityGroupAction>(target).OnRenamed(action, oneShot, deferred);

        public static ProximityGroupAction OnScriptChanged(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProximityGroupAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ProximityGroupAction OnTreeEntered(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProximityGroupAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ProximityGroupAction OnTreeExited(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProximityGroupAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ProximityGroupAction OnTreeExiting(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProximityGroupAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ProximityGroupAction OnVisibilityChanged(this ProximityGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ProximityGroupAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static RayCastAction OnGameplayEntered(this RayCast target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCastAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static RayCastAction OnGameplayExited(this RayCast target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCastAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static RayCastAction OnReady(this RayCast target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCastAction>(target).OnReady(action, oneShot, deferred);

        public static RayCastAction OnRenamed(this RayCast target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCastAction>(target).OnRenamed(action, oneShot, deferred);

        public static RayCastAction OnScriptChanged(this RayCast target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCastAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static RayCastAction OnTreeEntered(this RayCast target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCastAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static RayCastAction OnTreeExited(this RayCast target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCastAction>(target).OnTreeExited(action, oneShot, deferred);

        public static RayCastAction OnTreeExiting(this RayCast target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCastAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static RayCastAction OnVisibilityChanged(this RayCast target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCastAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static RayCast2DAction OnDraw(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCast2DAction>(target).OnDraw(action, oneShot, deferred);

        public static RayCast2DAction OnHide(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCast2DAction>(target).OnHide(action, oneShot, deferred);

        public static RayCast2DAction OnItemRectChanged(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCast2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static RayCast2DAction OnReady(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCast2DAction>(target).OnReady(action, oneShot, deferred);

        public static RayCast2DAction OnRenamed(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCast2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static RayCast2DAction OnScriptChanged(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCast2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static RayCast2DAction OnTreeEntered(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCast2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static RayCast2DAction OnTreeExited(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCast2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static RayCast2DAction OnTreeExiting(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCast2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static RayCast2DAction OnVisibilityChanged(this RayCast2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RayCast2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ReferenceRectAction OnDraw(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnDraw(action, oneShot, deferred);

        public static ReferenceRectAction OnFocusEntered(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static ReferenceRectAction OnFocusExited(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnFocusExited(action, oneShot, deferred);

        public static ReferenceRectAction OnGuiInput(this ReferenceRect target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnGuiInput(action, oneShot, deferred);

        public static ReferenceRectAction OnHide(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnHide(action, oneShot, deferred);

        public static ReferenceRectAction OnItemRectChanged(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static ReferenceRectAction OnMinimumSizeChanged(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static ReferenceRectAction OnModalClosed(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnModalClosed(action, oneShot, deferred);

        public static ReferenceRectAction OnMouseEntered(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static ReferenceRectAction OnMouseExited(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnMouseExited(action, oneShot, deferred);

        public static ReferenceRectAction OnReady(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnReady(action, oneShot, deferred);

        public static ReferenceRectAction OnRenamed(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnRenamed(action, oneShot, deferred);

        public static ReferenceRectAction OnResized(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnResized(action, oneShot, deferred);

        public static ReferenceRectAction OnScriptChanged(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ReferenceRectAction OnSizeFlagsChanged(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static ReferenceRectAction OnTreeEntered(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ReferenceRectAction OnTreeExited(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ReferenceRectAction OnTreeExiting(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ReferenceRectAction OnVisibilityChanged(this ReferenceRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReferenceRectAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ReflectionProbeAction OnGameplayEntered(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static ReflectionProbeAction OnGameplayExited(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static ReflectionProbeAction OnReady(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnReady(action, oneShot, deferred);

        public static ReflectionProbeAction OnRenamed(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnRenamed(action, oneShot, deferred);

        public static ReflectionProbeAction OnScriptChanged(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ReflectionProbeAction OnTreeEntered(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ReflectionProbeAction OnTreeExited(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ReflectionProbeAction OnTreeExiting(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ReflectionProbeAction OnVisibilityChanged(this ReflectionProbe target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ReflectionProbeAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static RemoteTransformAction OnGameplayEntered(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransformAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static RemoteTransformAction OnGameplayExited(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransformAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static RemoteTransformAction OnReady(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransformAction>(target).OnReady(action, oneShot, deferred);

        public static RemoteTransformAction OnRenamed(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransformAction>(target).OnRenamed(action, oneShot, deferred);

        public static RemoteTransformAction OnScriptChanged(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransformAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static RemoteTransformAction OnTreeEntered(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransformAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static RemoteTransformAction OnTreeExited(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransformAction>(target).OnTreeExited(action, oneShot, deferred);

        public static RemoteTransformAction OnTreeExiting(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransformAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static RemoteTransformAction OnVisibilityChanged(this RemoteTransform target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransformAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static RemoteTransform2DAction OnDraw(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnDraw(action, oneShot, deferred);

        public static RemoteTransform2DAction OnHide(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnHide(action, oneShot, deferred);

        public static RemoteTransform2DAction OnItemRectChanged(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static RemoteTransform2DAction OnReady(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnReady(action, oneShot, deferred);

        public static RemoteTransform2DAction OnRenamed(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static RemoteTransform2DAction OnScriptChanged(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static RemoteTransform2DAction OnTreeEntered(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static RemoteTransform2DAction OnTreeExited(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static RemoteTransform2DAction OnTreeExiting(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static RemoteTransform2DAction OnVisibilityChanged(this RemoteTransform2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RemoteTransform2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ResourcePreloaderAction OnReady(this ResourcePreloader target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ResourcePreloaderAction>(target).OnReady(action, oneShot, deferred);

        public static ResourcePreloaderAction OnRenamed(this ResourcePreloader target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ResourcePreloaderAction>(target).OnRenamed(action, oneShot, deferred);

        public static ResourcePreloaderAction OnScriptChanged(this ResourcePreloader target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ResourcePreloaderAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ResourcePreloaderAction OnTreeEntered(this ResourcePreloader target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ResourcePreloaderAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ResourcePreloaderAction OnTreeExited(this ResourcePreloader target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ResourcePreloaderAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ResourcePreloaderAction OnTreeExiting(this ResourcePreloader target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ResourcePreloaderAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static RichTextLabelAction OnDraw(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnDraw(action, oneShot, deferred);

        public static RichTextLabelAction OnFocusEntered(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static RichTextLabelAction OnFocusExited(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnFocusExited(action, oneShot, deferred);

        public static RichTextLabelAction OnGuiInput(this RichTextLabel target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnGuiInput(action, oneShot, deferred);

        public static RichTextLabelAction OnHide(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnHide(action, oneShot, deferred);

        public static RichTextLabelAction OnItemRectChanged(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static RichTextLabelAction OnMetaClicked(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnMetaClicked(action, oneShot, deferred);

        public static RichTextLabelAction OnMetaHoverEnded(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnMetaHoverEnded(action, oneShot, deferred);

        public static RichTextLabelAction OnMetaHoverStarted(this RichTextLabel target, Action<object> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnMetaHoverStarted(action, oneShot, deferred);

        public static RichTextLabelAction OnMinimumSizeChanged(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static RichTextLabelAction OnModalClosed(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnModalClosed(action, oneShot, deferred);

        public static RichTextLabelAction OnMouseEntered(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static RichTextLabelAction OnMouseExited(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnMouseExited(action, oneShot, deferred);

        public static RichTextLabelAction OnReady(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnReady(action, oneShot, deferred);

        public static RichTextLabelAction OnRenamed(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnRenamed(action, oneShot, deferred);

        public static RichTextLabelAction OnResized(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnResized(action, oneShot, deferred);

        public static RichTextLabelAction OnScriptChanged(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static RichTextLabelAction OnSizeFlagsChanged(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static RichTextLabelAction OnTreeEntered(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static RichTextLabelAction OnTreeExited(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnTreeExited(action, oneShot, deferred);

        public static RichTextLabelAction OnTreeExiting(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static RichTextLabelAction OnVisibilityChanged(this RichTextLabel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RichTextLabelAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static RigidBodyAction OnBodyEntered(this RigidBody target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnBodyEntered(action, oneShot, deferred);

        public static RigidBodyAction OnBodyExited(this RigidBody target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnBodyExited(action, oneShot, deferred);

        public static RigidBodyAction OnBodyShapeEntered(this RigidBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnBodyShapeEntered(action, oneShot, deferred);

        public static RigidBodyAction OnBodyShapeExited(this RigidBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnBodyShapeExited(action, oneShot, deferred);

        public static RigidBodyAction OnGameplayEntered(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static RigidBodyAction OnGameplayExited(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static RigidBodyAction OnInputEvent(this RigidBody target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnInputEvent(action, oneShot, deferred);

        public static RigidBodyAction OnMouseEntered(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static RigidBodyAction OnMouseExited(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnMouseExited(action, oneShot, deferred);

        public static RigidBodyAction OnReady(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnReady(action, oneShot, deferred);

        public static RigidBodyAction OnRenamed(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnRenamed(action, oneShot, deferred);

        public static RigidBodyAction OnScriptChanged(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static RigidBodyAction OnSleepingStateChanged(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnSleepingStateChanged(action, oneShot, deferred);

        public static RigidBodyAction OnTreeEntered(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static RigidBodyAction OnTreeExited(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnTreeExited(action, oneShot, deferred);

        public static RigidBodyAction OnTreeExiting(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static RigidBodyAction OnVisibilityChanged(this RigidBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBodyAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static RigidBody2DAction OnBodyEntered(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnBodyEntered(action, oneShot, deferred);

        public static RigidBody2DAction OnBodyExited(this RigidBody2D target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnBodyExited(action, oneShot, deferred);

        public static RigidBody2DAction OnBodyShapeEntered(this RigidBody2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnBodyShapeEntered(action, oneShot, deferred);

        public static RigidBody2DAction OnBodyShapeExited(this RigidBody2D target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnBodyShapeExited(action, oneShot, deferred);

        public static RigidBody2DAction OnDraw(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnDraw(action, oneShot, deferred);

        public static RigidBody2DAction OnHide(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnHide(action, oneShot, deferred);

        public static RigidBody2DAction OnInputEvent(this RigidBody2D target, Action<InputEvent, int, Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnInputEvent(action, oneShot, deferred);

        public static RigidBody2DAction OnItemRectChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static RigidBody2DAction OnMouseEntered(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static RigidBody2DAction OnMouseExited(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnMouseExited(action, oneShot, deferred);

        public static RigidBody2DAction OnReady(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnReady(action, oneShot, deferred);

        public static RigidBody2DAction OnRenamed(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static RigidBody2DAction OnScriptChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static RigidBody2DAction OnSleepingStateChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnSleepingStateChanged(action, oneShot, deferred);

        public static RigidBody2DAction OnTreeEntered(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static RigidBody2DAction OnTreeExited(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static RigidBody2DAction OnTreeExiting(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static RigidBody2DAction OnVisibilityChanged(this RigidBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RigidBody2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static RoomAction OnGameplayEntered(this Room target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static RoomAction OnGameplayExited(this Room target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static RoomAction OnReady(this Room target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomAction>(target).OnReady(action, oneShot, deferred);

        public static RoomAction OnRenamed(this Room target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomAction>(target).OnRenamed(action, oneShot, deferred);

        public static RoomAction OnScriptChanged(this Room target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static RoomAction OnTreeEntered(this Room target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static RoomAction OnTreeExited(this Room target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomAction>(target).OnTreeExited(action, oneShot, deferred);

        public static RoomAction OnTreeExiting(this Room target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static RoomAction OnVisibilityChanged(this Room target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static RoomGroupAction OnGameplayEntered(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomGroupAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static RoomGroupAction OnGameplayExited(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomGroupAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static RoomGroupAction OnReady(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomGroupAction>(target).OnReady(action, oneShot, deferred);

        public static RoomGroupAction OnRenamed(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomGroupAction>(target).OnRenamed(action, oneShot, deferred);

        public static RoomGroupAction OnScriptChanged(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomGroupAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static RoomGroupAction OnTreeEntered(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomGroupAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static RoomGroupAction OnTreeExited(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomGroupAction>(target).OnTreeExited(action, oneShot, deferred);

        public static RoomGroupAction OnTreeExiting(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomGroupAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static RoomGroupAction OnVisibilityChanged(this RoomGroup target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomGroupAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static RoomManagerAction OnGameplayEntered(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomManagerAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static RoomManagerAction OnGameplayExited(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomManagerAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static RoomManagerAction OnReady(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomManagerAction>(target).OnReady(action, oneShot, deferred);

        public static RoomManagerAction OnRenamed(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomManagerAction>(target).OnRenamed(action, oneShot, deferred);

        public static RoomManagerAction OnScriptChanged(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomManagerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static RoomManagerAction OnTreeEntered(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomManagerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static RoomManagerAction OnTreeExited(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomManagerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static RoomManagerAction OnTreeExiting(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomManagerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static RoomManagerAction OnVisibilityChanged(this RoomManager target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<RoomManagerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ScrollContainerAction OnDraw(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnDraw(action, oneShot, deferred);

        public static ScrollContainerAction OnFocusEntered(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static ScrollContainerAction OnFocusExited(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static ScrollContainerAction OnGuiInput(this ScrollContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static ScrollContainerAction OnHide(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnHide(action, oneShot, deferred);

        public static ScrollContainerAction OnItemRectChanged(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static ScrollContainerAction OnMinimumSizeChanged(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static ScrollContainerAction OnModalClosed(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static ScrollContainerAction OnMouseEntered(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static ScrollContainerAction OnMouseExited(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static ScrollContainerAction OnReady(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnReady(action, oneShot, deferred);

        public static ScrollContainerAction OnRenamed(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnRenamed(action, oneShot, deferred);

        public static ScrollContainerAction OnResized(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnResized(action, oneShot, deferred);

        public static ScrollContainerAction OnScriptChanged(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ScrollContainerAction OnScrollEnded(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnScrollEnded(action, oneShot, deferred);

        public static ScrollContainerAction OnScrollStarted(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnScrollStarted(action, oneShot, deferred);

        public static ScrollContainerAction OnSizeFlagsChanged(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static ScrollContainerAction OnSortChildren(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnSortChildren(action, oneShot, deferred);

        public static ScrollContainerAction OnTreeEntered(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ScrollContainerAction OnTreeExited(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ScrollContainerAction OnTreeExiting(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ScrollContainerAction OnVisibilityChanged(this ScrollContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ScrollContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static SkeletonAction OnGameplayEntered(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static SkeletonAction OnGameplayExited(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static SkeletonAction OnReady(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonAction>(target).OnReady(action, oneShot, deferred);

        public static SkeletonAction OnRenamed(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonAction>(target).OnRenamed(action, oneShot, deferred);

        public static SkeletonAction OnScriptChanged(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static SkeletonAction OnSkeletonUpdated(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonAction>(target).OnSkeletonUpdated(action, oneShot, deferred);

        public static SkeletonAction OnTreeEntered(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static SkeletonAction OnTreeExited(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonAction>(target).OnTreeExited(action, oneShot, deferred);

        public static SkeletonAction OnTreeExiting(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static SkeletonAction OnVisibilityChanged(this Skeleton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Skeleton2DAction OnBoneSetupChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Skeleton2DAction>(target).OnBoneSetupChanged(action, oneShot, deferred);

        public static Skeleton2DAction OnDraw(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Skeleton2DAction>(target).OnDraw(action, oneShot, deferred);

        public static Skeleton2DAction OnHide(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Skeleton2DAction>(target).OnHide(action, oneShot, deferred);

        public static Skeleton2DAction OnItemRectChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Skeleton2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static Skeleton2DAction OnReady(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Skeleton2DAction>(target).OnReady(action, oneShot, deferred);

        public static Skeleton2DAction OnRenamed(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Skeleton2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Skeleton2DAction OnScriptChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Skeleton2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Skeleton2DAction OnTreeEntered(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Skeleton2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Skeleton2DAction OnTreeExited(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Skeleton2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Skeleton2DAction OnTreeExiting(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Skeleton2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Skeleton2DAction OnVisibilityChanged(this Skeleton2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Skeleton2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static SkeletonIKAction OnReady(this SkeletonIK target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonIKAction>(target).OnReady(action, oneShot, deferred);

        public static SkeletonIKAction OnRenamed(this SkeletonIK target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonIKAction>(target).OnRenamed(action, oneShot, deferred);

        public static SkeletonIKAction OnScriptChanged(this SkeletonIK target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonIKAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static SkeletonIKAction OnTreeEntered(this SkeletonIK target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonIKAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static SkeletonIKAction OnTreeExited(this SkeletonIK target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonIKAction>(target).OnTreeExited(action, oneShot, deferred);

        public static SkeletonIKAction OnTreeExiting(this SkeletonIK target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SkeletonIKAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static SliderJointAction OnGameplayEntered(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SliderJointAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static SliderJointAction OnGameplayExited(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SliderJointAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static SliderJointAction OnReady(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SliderJointAction>(target).OnReady(action, oneShot, deferred);

        public static SliderJointAction OnRenamed(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SliderJointAction>(target).OnRenamed(action, oneShot, deferred);

        public static SliderJointAction OnScriptChanged(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SliderJointAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static SliderJointAction OnTreeEntered(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SliderJointAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static SliderJointAction OnTreeExited(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SliderJointAction>(target).OnTreeExited(action, oneShot, deferred);

        public static SliderJointAction OnTreeExiting(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SliderJointAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static SliderJointAction OnVisibilityChanged(this SliderJoint target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SliderJointAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static SoftBodyAction OnGameplayEntered(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SoftBodyAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static SoftBodyAction OnGameplayExited(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SoftBodyAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static SoftBodyAction OnReady(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SoftBodyAction>(target).OnReady(action, oneShot, deferred);

        public static SoftBodyAction OnRenamed(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SoftBodyAction>(target).OnRenamed(action, oneShot, deferred);

        public static SoftBodyAction OnScriptChanged(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SoftBodyAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static SoftBodyAction OnTreeEntered(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SoftBodyAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static SoftBodyAction OnTreeExited(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SoftBodyAction>(target).OnTreeExited(action, oneShot, deferred);

        public static SoftBodyAction OnTreeExiting(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SoftBodyAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static SoftBodyAction OnVisibilityChanged(this SoftBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SoftBodyAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static SpatialAction OnGameplayEntered(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpatialAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static SpatialAction OnGameplayExited(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpatialAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static SpatialAction OnReady(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpatialAction>(target).OnReady(action, oneShot, deferred);

        public static SpatialAction OnRenamed(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpatialAction>(target).OnRenamed(action, oneShot, deferred);

        public static SpatialAction OnScriptChanged(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpatialAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static SpatialAction OnTreeEntered(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpatialAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static SpatialAction OnTreeExited(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpatialAction>(target).OnTreeExited(action, oneShot, deferred);

        public static SpatialAction OnTreeExiting(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpatialAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static SpatialAction OnVisibilityChanged(this Spatial target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpatialAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static SpinBoxAction OnChanged(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnChanged(action, oneShot, deferred);

        public static SpinBoxAction OnDraw(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnDraw(action, oneShot, deferred);

        public static SpinBoxAction OnFocusEntered(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static SpinBoxAction OnFocusExited(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnFocusExited(action, oneShot, deferred);

        public static SpinBoxAction OnGuiInput(this SpinBox target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnGuiInput(action, oneShot, deferred);

        public static SpinBoxAction OnHide(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnHide(action, oneShot, deferred);

        public static SpinBoxAction OnItemRectChanged(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static SpinBoxAction OnMinimumSizeChanged(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static SpinBoxAction OnModalClosed(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnModalClosed(action, oneShot, deferred);

        public static SpinBoxAction OnMouseEntered(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static SpinBoxAction OnMouseExited(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnMouseExited(action, oneShot, deferred);

        public static SpinBoxAction OnReady(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnReady(action, oneShot, deferred);

        public static SpinBoxAction OnRenamed(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnRenamed(action, oneShot, deferred);

        public static SpinBoxAction OnResized(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnResized(action, oneShot, deferred);

        public static SpinBoxAction OnScriptChanged(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static SpinBoxAction OnSizeFlagsChanged(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static SpinBoxAction OnTreeEntered(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static SpinBoxAction OnTreeExited(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnTreeExited(action, oneShot, deferred);

        public static SpinBoxAction OnTreeExiting(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static SpinBoxAction OnValueChanged(this SpinBox target, Action<float> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnValueChanged(action, oneShot, deferred);

        public static SpinBoxAction OnVisibilityChanged(this SpinBox target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpinBoxAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static SpotLightAction OnGameplayEntered(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpotLightAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static SpotLightAction OnGameplayExited(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpotLightAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static SpotLightAction OnReady(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpotLightAction>(target).OnReady(action, oneShot, deferred);

        public static SpotLightAction OnRenamed(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpotLightAction>(target).OnRenamed(action, oneShot, deferred);

        public static SpotLightAction OnScriptChanged(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpotLightAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static SpotLightAction OnTreeEntered(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpotLightAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static SpotLightAction OnTreeExited(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpotLightAction>(target).OnTreeExited(action, oneShot, deferred);

        public static SpotLightAction OnTreeExiting(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpotLightAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static SpotLightAction OnVisibilityChanged(this SpotLight target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpotLightAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static SpringArmAction OnGameplayEntered(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpringArmAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static SpringArmAction OnGameplayExited(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpringArmAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static SpringArmAction OnReady(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpringArmAction>(target).OnReady(action, oneShot, deferred);

        public static SpringArmAction OnRenamed(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpringArmAction>(target).OnRenamed(action, oneShot, deferred);

        public static SpringArmAction OnScriptChanged(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpringArmAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static SpringArmAction OnTreeEntered(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpringArmAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static SpringArmAction OnTreeExited(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpringArmAction>(target).OnTreeExited(action, oneShot, deferred);

        public static SpringArmAction OnTreeExiting(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpringArmAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static SpringArmAction OnVisibilityChanged(this SpringArm target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpringArmAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static SpriteAction OnDraw(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpriteAction>(target).OnDraw(action, oneShot, deferred);

        public static SpriteAction OnFrameChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpriteAction>(target).OnFrameChanged(action, oneShot, deferred);

        public static SpriteAction OnHide(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpriteAction>(target).OnHide(action, oneShot, deferred);

        public static SpriteAction OnItemRectChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpriteAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static SpriteAction OnReady(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpriteAction>(target).OnReady(action, oneShot, deferred);

        public static SpriteAction OnRenamed(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpriteAction>(target).OnRenamed(action, oneShot, deferred);

        public static SpriteAction OnScriptChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpriteAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static SpriteAction OnTextureChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpriteAction>(target).OnTextureChanged(action, oneShot, deferred);

        public static SpriteAction OnTreeEntered(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpriteAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static SpriteAction OnTreeExited(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpriteAction>(target).OnTreeExited(action, oneShot, deferred);

        public static SpriteAction OnTreeExiting(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpriteAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static SpriteAction OnVisibilityChanged(this Sprite target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<SpriteAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static Sprite3DAction OnFrameChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Sprite3DAction>(target).OnFrameChanged(action, oneShot, deferred);

        public static Sprite3DAction OnGameplayEntered(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Sprite3DAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static Sprite3DAction OnGameplayExited(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Sprite3DAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static Sprite3DAction OnReady(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Sprite3DAction>(target).OnReady(action, oneShot, deferred);

        public static Sprite3DAction OnRenamed(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Sprite3DAction>(target).OnRenamed(action, oneShot, deferred);

        public static Sprite3DAction OnScriptChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Sprite3DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static Sprite3DAction OnTreeEntered(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Sprite3DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static Sprite3DAction OnTreeExited(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Sprite3DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static Sprite3DAction OnTreeExiting(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Sprite3DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static Sprite3DAction OnVisibilityChanged(this Sprite3D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<Sprite3DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static StaticBodyAction OnGameplayEntered(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBodyAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static StaticBodyAction OnGameplayExited(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBodyAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static StaticBodyAction OnInputEvent(this StaticBody target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBodyAction>(target).OnInputEvent(action, oneShot, deferred);

        public static StaticBodyAction OnMouseEntered(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBodyAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static StaticBodyAction OnMouseExited(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBodyAction>(target).OnMouseExited(action, oneShot, deferred);

        public static StaticBodyAction OnReady(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBodyAction>(target).OnReady(action, oneShot, deferred);

        public static StaticBodyAction OnRenamed(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBodyAction>(target).OnRenamed(action, oneShot, deferred);

        public static StaticBodyAction OnScriptChanged(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBodyAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static StaticBodyAction OnTreeEntered(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBodyAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static StaticBodyAction OnTreeExited(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBodyAction>(target).OnTreeExited(action, oneShot, deferred);

        public static StaticBodyAction OnTreeExiting(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBodyAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static StaticBodyAction OnVisibilityChanged(this StaticBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBodyAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static StaticBody2DAction OnDraw(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBody2DAction>(target).OnDraw(action, oneShot, deferred);

        public static StaticBody2DAction OnHide(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBody2DAction>(target).OnHide(action, oneShot, deferred);

        public static StaticBody2DAction OnInputEvent(this StaticBody2D target, Action<InputEvent, int, Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBody2DAction>(target).OnInputEvent(action, oneShot, deferred);

        public static StaticBody2DAction OnItemRectChanged(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBody2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static StaticBody2DAction OnMouseEntered(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBody2DAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static StaticBody2DAction OnMouseExited(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBody2DAction>(target).OnMouseExited(action, oneShot, deferred);

        public static StaticBody2DAction OnReady(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBody2DAction>(target).OnReady(action, oneShot, deferred);

        public static StaticBody2DAction OnRenamed(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBody2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static StaticBody2DAction OnScriptChanged(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBody2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static StaticBody2DAction OnTreeEntered(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBody2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static StaticBody2DAction OnTreeExited(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBody2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static StaticBody2DAction OnTreeExiting(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBody2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static StaticBody2DAction OnVisibilityChanged(this StaticBody2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<StaticBody2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static TabContainerAction OnDraw(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnDraw(action, oneShot, deferred);

        public static TabContainerAction OnFocusEntered(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static TabContainerAction OnFocusExited(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static TabContainerAction OnGuiInput(this TabContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static TabContainerAction OnHide(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnHide(action, oneShot, deferred);

        public static TabContainerAction OnItemRectChanged(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static TabContainerAction OnMinimumSizeChanged(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static TabContainerAction OnModalClosed(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static TabContainerAction OnMouseEntered(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static TabContainerAction OnMouseExited(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static TabContainerAction OnPrePopupPressed(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnPrePopupPressed(action, oneShot, deferred);

        public static TabContainerAction OnReady(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnReady(action, oneShot, deferred);

        public static TabContainerAction OnRenamed(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnRenamed(action, oneShot, deferred);

        public static TabContainerAction OnResized(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnResized(action, oneShot, deferred);

        public static TabContainerAction OnScriptChanged(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static TabContainerAction OnSizeFlagsChanged(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static TabContainerAction OnSortChildren(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnSortChildren(action, oneShot, deferred);

        public static TabContainerAction OnTabChanged(this TabContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnTabChanged(action, oneShot, deferred);

        public static TabContainerAction OnTabSelected(this TabContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnTabSelected(action, oneShot, deferred);

        public static TabContainerAction OnTreeEntered(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static TabContainerAction OnTreeExited(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static TabContainerAction OnTreeExiting(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static TabContainerAction OnVisibilityChanged(this TabContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static TabsAction OnDraw(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnDraw(action, oneShot, deferred);

        public static TabsAction OnFocusEntered(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static TabsAction OnFocusExited(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnFocusExited(action, oneShot, deferred);

        public static TabsAction OnGuiInput(this Tabs target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnGuiInput(action, oneShot, deferred);

        public static TabsAction OnHide(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnHide(action, oneShot, deferred);

        public static TabsAction OnItemRectChanged(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static TabsAction OnMinimumSizeChanged(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static TabsAction OnModalClosed(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnModalClosed(action, oneShot, deferred);

        public static TabsAction OnMouseEntered(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static TabsAction OnMouseExited(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnMouseExited(action, oneShot, deferred);

        public static TabsAction OnReady(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnReady(action, oneShot, deferred);

        public static TabsAction OnRenamed(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnRenamed(action, oneShot, deferred);

        public static TabsAction OnRepositionActiveTabRequest(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnRepositionActiveTabRequest(action, oneShot, deferred);

        public static TabsAction OnResized(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnResized(action, oneShot, deferred);

        public static TabsAction OnRightButtonPressed(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnRightButtonPressed(action, oneShot, deferred);

        public static TabsAction OnScriptChanged(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static TabsAction OnSizeFlagsChanged(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static TabsAction OnTabChanged(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnTabChanged(action, oneShot, deferred);

        public static TabsAction OnTabClicked(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnTabClicked(action, oneShot, deferred);

        public static TabsAction OnTabClose(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnTabClose(action, oneShot, deferred);

        public static TabsAction OnTabHover(this Tabs target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnTabHover(action, oneShot, deferred);

        public static TabsAction OnTreeEntered(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static TabsAction OnTreeExited(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnTreeExited(action, oneShot, deferred);

        public static TabsAction OnTreeExiting(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static TabsAction OnVisibilityChanged(this Tabs target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TabsAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static TextEditAction OnBreakpointToggled(this TextEdit target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnBreakpointToggled(action, oneShot, deferred);

        public static TextEditAction OnCursorChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnCursorChanged(action, oneShot, deferred);

        public static TextEditAction OnDraw(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnDraw(action, oneShot, deferred);

        public static TextEditAction OnFocusEntered(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static TextEditAction OnFocusExited(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnFocusExited(action, oneShot, deferred);

        public static TextEditAction OnGuiInput(this TextEdit target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnGuiInput(action, oneShot, deferred);

        public static TextEditAction OnHide(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnHide(action, oneShot, deferred);

        public static TextEditAction OnInfoClicked(this TextEdit target, Action<string, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnInfoClicked(action, oneShot, deferred);

        public static TextEditAction OnItemRectChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static TextEditAction OnMinimumSizeChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static TextEditAction OnModalClosed(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnModalClosed(action, oneShot, deferred);

        public static TextEditAction OnMouseEntered(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static TextEditAction OnMouseExited(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnMouseExited(action, oneShot, deferred);

        public static TextEditAction OnReady(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnReady(action, oneShot, deferred);

        public static TextEditAction OnRenamed(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnRenamed(action, oneShot, deferred);

        public static TextEditAction OnRequestCompletion(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnRequestCompletion(action, oneShot, deferred);

        public static TextEditAction OnResized(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnResized(action, oneShot, deferred);

        public static TextEditAction OnScriptChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static TextEditAction OnSizeFlagsChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static TextEditAction OnSymbolLookup(this TextEdit target, Action<int, int, string> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnSymbolLookup(action, oneShot, deferred);

        public static TextEditAction OnTextChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnTextChanged(action, oneShot, deferred);

        public static TextEditAction OnTreeEntered(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static TextEditAction OnTreeExited(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnTreeExited(action, oneShot, deferred);

        public static TextEditAction OnTreeExiting(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static TextEditAction OnVisibilityChanged(this TextEdit target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextEditAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static TextureButtonAction OnButtonDown(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnButtonDown(action, oneShot, deferred);

        public static TextureButtonAction OnButtonUp(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnButtonUp(action, oneShot, deferred);

        public static TextureButtonAction OnDraw(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnDraw(action, oneShot, deferred);

        public static TextureButtonAction OnFocusEntered(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static TextureButtonAction OnFocusExited(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnFocusExited(action, oneShot, deferred);

        public static TextureButtonAction OnGuiInput(this TextureButton target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnGuiInput(action, oneShot, deferred);

        public static TextureButtonAction OnHide(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnHide(action, oneShot, deferred);

        public static TextureButtonAction OnItemRectChanged(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static TextureButtonAction OnMinimumSizeChanged(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static TextureButtonAction OnModalClosed(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnModalClosed(action, oneShot, deferred);

        public static TextureButtonAction OnMouseEntered(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static TextureButtonAction OnMouseExited(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnMouseExited(action, oneShot, deferred);

        public static TextureButtonAction OnPressed(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnPressed(action, oneShot, deferred);

        public static TextureButtonAction OnReady(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnReady(action, oneShot, deferred);

        public static TextureButtonAction OnRenamed(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnRenamed(action, oneShot, deferred);

        public static TextureButtonAction OnResized(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnResized(action, oneShot, deferred);

        public static TextureButtonAction OnScriptChanged(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static TextureButtonAction OnSizeFlagsChanged(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static TextureButtonAction OnToggled(this TextureButton target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnToggled(action, oneShot, deferred);

        public static TextureButtonAction OnTreeEntered(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static TextureButtonAction OnTreeExited(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnTreeExited(action, oneShot, deferred);

        public static TextureButtonAction OnTreeExiting(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static TextureButtonAction OnVisibilityChanged(this TextureButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static TextureProgressAction OnChanged(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnChanged(action, oneShot, deferred);

        public static TextureProgressAction OnDraw(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnDraw(action, oneShot, deferred);

        public static TextureProgressAction OnFocusEntered(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static TextureProgressAction OnFocusExited(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnFocusExited(action, oneShot, deferred);

        public static TextureProgressAction OnGuiInput(this TextureProgress target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnGuiInput(action, oneShot, deferred);

        public static TextureProgressAction OnHide(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnHide(action, oneShot, deferred);

        public static TextureProgressAction OnItemRectChanged(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static TextureProgressAction OnMinimumSizeChanged(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static TextureProgressAction OnModalClosed(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnModalClosed(action, oneShot, deferred);

        public static TextureProgressAction OnMouseEntered(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static TextureProgressAction OnMouseExited(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnMouseExited(action, oneShot, deferred);

        public static TextureProgressAction OnReady(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnReady(action, oneShot, deferred);

        public static TextureProgressAction OnRenamed(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnRenamed(action, oneShot, deferred);

        public static TextureProgressAction OnResized(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnResized(action, oneShot, deferred);

        public static TextureProgressAction OnScriptChanged(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static TextureProgressAction OnSizeFlagsChanged(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static TextureProgressAction OnTreeEntered(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static TextureProgressAction OnTreeExited(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnTreeExited(action, oneShot, deferred);

        public static TextureProgressAction OnTreeExiting(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static TextureProgressAction OnValueChanged(this TextureProgress target, Action<float> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnValueChanged(action, oneShot, deferred);

        public static TextureProgressAction OnVisibilityChanged(this TextureProgress target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureProgressAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static TextureRectAction OnDraw(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnDraw(action, oneShot, deferred);

        public static TextureRectAction OnFocusEntered(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static TextureRectAction OnFocusExited(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnFocusExited(action, oneShot, deferred);

        public static TextureRectAction OnGuiInput(this TextureRect target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnGuiInput(action, oneShot, deferred);

        public static TextureRectAction OnHide(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnHide(action, oneShot, deferred);

        public static TextureRectAction OnItemRectChanged(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static TextureRectAction OnMinimumSizeChanged(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static TextureRectAction OnModalClosed(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnModalClosed(action, oneShot, deferred);

        public static TextureRectAction OnMouseEntered(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static TextureRectAction OnMouseExited(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnMouseExited(action, oneShot, deferred);

        public static TextureRectAction OnReady(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnReady(action, oneShot, deferred);

        public static TextureRectAction OnRenamed(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnRenamed(action, oneShot, deferred);

        public static TextureRectAction OnResized(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnResized(action, oneShot, deferred);

        public static TextureRectAction OnScriptChanged(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static TextureRectAction OnSizeFlagsChanged(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static TextureRectAction OnTreeEntered(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static TextureRectAction OnTreeExited(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnTreeExited(action, oneShot, deferred);

        public static TextureRectAction OnTreeExiting(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static TextureRectAction OnVisibilityChanged(this TextureRect target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TextureRectAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static TileMapAction OnDraw(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TileMapAction>(target).OnDraw(action, oneShot, deferred);

        public static TileMapAction OnHide(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TileMapAction>(target).OnHide(action, oneShot, deferred);

        public static TileMapAction OnItemRectChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TileMapAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static TileMapAction OnReady(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TileMapAction>(target).OnReady(action, oneShot, deferred);

        public static TileMapAction OnRenamed(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TileMapAction>(target).OnRenamed(action, oneShot, deferred);

        public static TileMapAction OnScriptChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TileMapAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static TileMapAction OnSettingsChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TileMapAction>(target).OnSettingsChanged(action, oneShot, deferred);

        public static TileMapAction OnTreeEntered(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TileMapAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static TileMapAction OnTreeExited(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TileMapAction>(target).OnTreeExited(action, oneShot, deferred);

        public static TileMapAction OnTreeExiting(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TileMapAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static TileMapAction OnVisibilityChanged(this TileMap target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TileMapAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static TimerAction OnReady(this Timer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TimerAction>(target).OnReady(action, oneShot, deferred);

        public static TimerAction OnRenamed(this Timer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TimerAction>(target).OnRenamed(action, oneShot, deferred);

        public static TimerAction OnScriptChanged(this Timer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TimerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static TimerAction OnTimeout(this Timer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TimerAction>(target).OnTimeout(action, oneShot, deferred);

        public static TimerAction OnTreeEntered(this Timer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TimerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static TimerAction OnTreeExited(this Timer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TimerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static TimerAction OnTreeExiting(this Timer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TimerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ToolButtonAction OnButtonDown(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnButtonDown(action, oneShot, deferred);

        public static ToolButtonAction OnButtonUp(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnButtonUp(action, oneShot, deferred);

        public static ToolButtonAction OnDraw(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnDraw(action, oneShot, deferred);

        public static ToolButtonAction OnFocusEntered(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static ToolButtonAction OnFocusExited(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnFocusExited(action, oneShot, deferred);

        public static ToolButtonAction OnGuiInput(this ToolButton target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnGuiInput(action, oneShot, deferred);

        public static ToolButtonAction OnHide(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnHide(action, oneShot, deferred);

        public static ToolButtonAction OnItemRectChanged(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static ToolButtonAction OnMinimumSizeChanged(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static ToolButtonAction OnModalClosed(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnModalClosed(action, oneShot, deferred);

        public static ToolButtonAction OnMouseEntered(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static ToolButtonAction OnMouseExited(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnMouseExited(action, oneShot, deferred);

        public static ToolButtonAction OnPressed(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnPressed(action, oneShot, deferred);

        public static ToolButtonAction OnReady(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnReady(action, oneShot, deferred);

        public static ToolButtonAction OnRenamed(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnRenamed(action, oneShot, deferred);

        public static ToolButtonAction OnResized(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnResized(action, oneShot, deferred);

        public static ToolButtonAction OnScriptChanged(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ToolButtonAction OnSizeFlagsChanged(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static ToolButtonAction OnToggled(this ToolButton target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnToggled(action, oneShot, deferred);

        public static ToolButtonAction OnTreeEntered(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ToolButtonAction OnTreeExited(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ToolButtonAction OnTreeExiting(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ToolButtonAction OnVisibilityChanged(this ToolButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ToolButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static TouchScreenButtonAction OnDraw(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnDraw(action, oneShot, deferred);

        public static TouchScreenButtonAction OnHide(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnHide(action, oneShot, deferred);

        public static TouchScreenButtonAction OnItemRectChanged(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static TouchScreenButtonAction OnPressed(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnPressed(action, oneShot, deferred);

        public static TouchScreenButtonAction OnReady(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnReady(action, oneShot, deferred);

        public static TouchScreenButtonAction OnReleased(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnReleased(action, oneShot, deferred);

        public static TouchScreenButtonAction OnRenamed(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnRenamed(action, oneShot, deferred);

        public static TouchScreenButtonAction OnScriptChanged(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static TouchScreenButtonAction OnTreeEntered(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static TouchScreenButtonAction OnTreeExited(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnTreeExited(action, oneShot, deferred);

        public static TouchScreenButtonAction OnTreeExiting(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static TouchScreenButtonAction OnVisibilityChanged(this TouchScreenButton target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TouchScreenButtonAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static TreeAction OnButtonPressed(this Tree target, Action<int, int, TreeItem> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnButtonPressed(action, oneShot, deferred);

        public static TreeAction OnCellSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnCellSelected(action, oneShot, deferred);

        public static TreeAction OnColumnTitlePressed(this Tree target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnColumnTitlePressed(action, oneShot, deferred);

        public static TreeAction OnCustomPopupEdited(this Tree target, Action<bool> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnCustomPopupEdited(action, oneShot, deferred);

        public static TreeAction OnDraw(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnDraw(action, oneShot, deferred);

        public static TreeAction OnEmptyRmb(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnEmptyRmb(action, oneShot, deferred);

        public static TreeAction OnEmptyTreeRmbSelected(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnEmptyTreeRmbSelected(action, oneShot, deferred);

        public static TreeAction OnFocusEntered(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static TreeAction OnFocusExited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnFocusExited(action, oneShot, deferred);

        public static TreeAction OnGuiInput(this Tree target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnGuiInput(action, oneShot, deferred);

        public static TreeAction OnHide(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnHide(action, oneShot, deferred);

        public static TreeAction OnItemActivated(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnItemActivated(action, oneShot, deferred);

        public static TreeAction OnItemCollapsed(this Tree target, Action<TreeItem> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnItemCollapsed(action, oneShot, deferred);

        public static TreeAction OnItemCustomButtonPressed(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnItemCustomButtonPressed(action, oneShot, deferred);

        public static TreeAction OnItemDoubleClicked(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnItemDoubleClicked(action, oneShot, deferred);

        public static TreeAction OnItemEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnItemEdited(action, oneShot, deferred);

        public static TreeAction OnItemRectChanged(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static TreeAction OnItemRmbEdited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnItemRmbEdited(action, oneShot, deferred);

        public static TreeAction OnItemRmbSelected(this Tree target, Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnItemRmbSelected(action, oneShot, deferred);

        public static TreeAction OnItemSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnItemSelected(action, oneShot, deferred);

        public static TreeAction OnMinimumSizeChanged(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static TreeAction OnModalClosed(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnModalClosed(action, oneShot, deferred);

        public static TreeAction OnMouseEntered(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static TreeAction OnMouseExited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnMouseExited(action, oneShot, deferred);

        public static TreeAction OnMultiSelected(this Tree target, Action<int, TreeItem, bool> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnMultiSelected(action, oneShot, deferred);

        public static TreeAction OnNothingSelected(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnNothingSelected(action, oneShot, deferred);

        public static TreeAction OnReady(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnReady(action, oneShot, deferred);

        public static TreeAction OnRenamed(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnRenamed(action, oneShot, deferred);

        public static TreeAction OnResized(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnResized(action, oneShot, deferred);

        public static TreeAction OnScriptChanged(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static TreeAction OnSizeFlagsChanged(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static TreeAction OnTreeEntered(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static TreeAction OnTreeExited(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnTreeExited(action, oneShot, deferred);

        public static TreeAction OnTreeExiting(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static TreeAction OnVisibilityChanged(this Tree target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TreeAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static TweenAction OnReady(this Tween target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TweenAction>(target).OnReady(action, oneShot, deferred);

        public static TweenAction OnRenamed(this Tween target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TweenAction>(target).OnRenamed(action, oneShot, deferred);

        public static TweenAction OnScriptChanged(this Tween target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TweenAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static TweenAction OnTreeEntered(this Tween target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TweenAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static TweenAction OnTreeExited(this Tween target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TweenAction>(target).OnTreeExited(action, oneShot, deferred);

        public static TweenAction OnTreeExiting(this Tween target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TweenAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static TweenAction OnTweenAllCompleted(this Tween target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TweenAction>(target).OnTweenAllCompleted(action, oneShot, deferred);

        public static TweenAction OnTweenCompleted(this Tween target, Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TweenAction>(target).OnTweenCompleted(action, oneShot, deferred);

        public static TweenAction OnTweenStarted(this Tween target, Action<Object, NodePath> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TweenAction>(target).OnTweenStarted(action, oneShot, deferred);

        public static TweenAction OnTweenStep(this Tween target, Action<Object, float, NodePath, Object> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<TweenAction>(target).OnTweenStep(action, oneShot, deferred);

        public static VBoxContainerAction OnDraw(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnDraw(action, oneShot, deferred);

        public static VBoxContainerAction OnFocusEntered(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static VBoxContainerAction OnFocusExited(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static VBoxContainerAction OnGuiInput(this VBoxContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static VBoxContainerAction OnHide(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnHide(action, oneShot, deferred);

        public static VBoxContainerAction OnItemRectChanged(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static VBoxContainerAction OnMinimumSizeChanged(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static VBoxContainerAction OnModalClosed(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static VBoxContainerAction OnMouseEntered(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static VBoxContainerAction OnMouseExited(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static VBoxContainerAction OnReady(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnReady(action, oneShot, deferred);

        public static VBoxContainerAction OnRenamed(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnRenamed(action, oneShot, deferred);

        public static VBoxContainerAction OnResized(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnResized(action, oneShot, deferred);

        public static VBoxContainerAction OnScriptChanged(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static VBoxContainerAction OnSizeFlagsChanged(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static VBoxContainerAction OnSortChildren(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnSortChildren(action, oneShot, deferred);

        public static VBoxContainerAction OnTreeEntered(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static VBoxContainerAction OnTreeExited(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static VBoxContainerAction OnTreeExiting(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static VBoxContainerAction OnVisibilityChanged(this VBoxContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VBoxContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static VehicleBodyAction OnBodyEntered(this VehicleBody target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnBodyEntered(action, oneShot, deferred);

        public static VehicleBodyAction OnBodyExited(this VehicleBody target, Action<Node> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnBodyExited(action, oneShot, deferred);

        public static VehicleBodyAction OnBodyShapeEntered(this VehicleBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnBodyShapeEntered(action, oneShot, deferred);

        public static VehicleBodyAction OnBodyShapeExited(this VehicleBody target, Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnBodyShapeExited(action, oneShot, deferred);

        public static VehicleBodyAction OnGameplayEntered(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static VehicleBodyAction OnGameplayExited(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static VehicleBodyAction OnInputEvent(this VehicleBody target, Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnInputEvent(action, oneShot, deferred);

        public static VehicleBodyAction OnMouseEntered(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static VehicleBodyAction OnMouseExited(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnMouseExited(action, oneShot, deferred);

        public static VehicleBodyAction OnReady(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnReady(action, oneShot, deferred);

        public static VehicleBodyAction OnRenamed(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnRenamed(action, oneShot, deferred);

        public static VehicleBodyAction OnScriptChanged(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static VehicleBodyAction OnSleepingStateChanged(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnSleepingStateChanged(action, oneShot, deferred);

        public static VehicleBodyAction OnTreeEntered(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static VehicleBodyAction OnTreeExited(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnTreeExited(action, oneShot, deferred);

        public static VehicleBodyAction OnTreeExiting(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static VehicleBodyAction OnVisibilityChanged(this VehicleBody target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleBodyAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static VehicleWheelAction OnGameplayEntered(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleWheelAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static VehicleWheelAction OnGameplayExited(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleWheelAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static VehicleWheelAction OnReady(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleWheelAction>(target).OnReady(action, oneShot, deferred);

        public static VehicleWheelAction OnRenamed(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleWheelAction>(target).OnRenamed(action, oneShot, deferred);

        public static VehicleWheelAction OnScriptChanged(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleWheelAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static VehicleWheelAction OnTreeEntered(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleWheelAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static VehicleWheelAction OnTreeExited(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleWheelAction>(target).OnTreeExited(action, oneShot, deferred);

        public static VehicleWheelAction OnTreeExiting(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleWheelAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static VehicleWheelAction OnVisibilityChanged(this VehicleWheel target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VehicleWheelAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static VideoPlayerAction OnDraw(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnDraw(action, oneShot, deferred);

        public static VideoPlayerAction OnFinished(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnFinished(action, oneShot, deferred);

        public static VideoPlayerAction OnFocusEntered(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static VideoPlayerAction OnFocusExited(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static VideoPlayerAction OnGuiInput(this VideoPlayer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static VideoPlayerAction OnHide(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnHide(action, oneShot, deferred);

        public static VideoPlayerAction OnItemRectChanged(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static VideoPlayerAction OnMinimumSizeChanged(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static VideoPlayerAction OnModalClosed(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static VideoPlayerAction OnMouseEntered(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static VideoPlayerAction OnMouseExited(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static VideoPlayerAction OnReady(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnReady(action, oneShot, deferred);

        public static VideoPlayerAction OnRenamed(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnRenamed(action, oneShot, deferred);

        public static VideoPlayerAction OnResized(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnResized(action, oneShot, deferred);

        public static VideoPlayerAction OnScriptChanged(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static VideoPlayerAction OnSizeFlagsChanged(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static VideoPlayerAction OnTreeEntered(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static VideoPlayerAction OnTreeExited(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static VideoPlayerAction OnTreeExiting(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static VideoPlayerAction OnVisibilityChanged(this VideoPlayer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VideoPlayerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static ViewportAction OnGuiFocusChanged(this Viewport target, Action<Control> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportAction>(target).OnGuiFocusChanged(action, oneShot, deferred);

        public static ViewportAction OnReady(this Viewport target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportAction>(target).OnReady(action, oneShot, deferred);

        public static ViewportAction OnRenamed(this Viewport target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportAction>(target).OnRenamed(action, oneShot, deferred);

        public static ViewportAction OnScriptChanged(this Viewport target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ViewportAction OnSizeChanged(this Viewport target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportAction>(target).OnSizeChanged(action, oneShot, deferred);

        public static ViewportAction OnTreeEntered(this Viewport target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ViewportAction OnTreeExited(this Viewport target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ViewportAction OnTreeExiting(this Viewport target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ViewportContainerAction OnDraw(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnDraw(action, oneShot, deferred);

        public static ViewportContainerAction OnFocusEntered(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static ViewportContainerAction OnFocusExited(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static ViewportContainerAction OnGuiInput(this ViewportContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static ViewportContainerAction OnHide(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnHide(action, oneShot, deferred);

        public static ViewportContainerAction OnItemRectChanged(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static ViewportContainerAction OnMinimumSizeChanged(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static ViewportContainerAction OnModalClosed(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static ViewportContainerAction OnMouseEntered(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static ViewportContainerAction OnMouseExited(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static ViewportContainerAction OnReady(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnReady(action, oneShot, deferred);

        public static ViewportContainerAction OnRenamed(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnRenamed(action, oneShot, deferred);

        public static ViewportContainerAction OnResized(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnResized(action, oneShot, deferred);

        public static ViewportContainerAction OnScriptChanged(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static ViewportContainerAction OnSizeFlagsChanged(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static ViewportContainerAction OnSortChildren(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnSortChildren(action, oneShot, deferred);

        public static ViewportContainerAction OnTreeEntered(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static ViewportContainerAction OnTreeExited(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static ViewportContainerAction OnTreeExiting(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static ViewportContainerAction OnVisibilityChanged(this ViewportContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<ViewportContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static VisibilityEnablerAction OnCameraEntered(this VisibilityEnabler target, Action<Camera> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnCameraEntered(action, oneShot, deferred);

        public static VisibilityEnablerAction OnCameraExited(this VisibilityEnabler target, Action<Camera> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnCameraExited(action, oneShot, deferred);

        public static VisibilityEnablerAction OnGameplayEntered(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static VisibilityEnablerAction OnGameplayExited(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static VisibilityEnablerAction OnReady(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnReady(action, oneShot, deferred);

        public static VisibilityEnablerAction OnRenamed(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnRenamed(action, oneShot, deferred);

        public static VisibilityEnablerAction OnScreenEntered(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnScreenEntered(action, oneShot, deferred);

        public static VisibilityEnablerAction OnScreenExited(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnScreenExited(action, oneShot, deferred);

        public static VisibilityEnablerAction OnScriptChanged(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static VisibilityEnablerAction OnTreeEntered(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static VisibilityEnablerAction OnTreeExited(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static VisibilityEnablerAction OnTreeExiting(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static VisibilityEnablerAction OnVisibilityChanged(this VisibilityEnabler target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnablerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static VisibilityEnabler2DAction OnDraw(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnDraw(action, oneShot, deferred);

        public static VisibilityEnabler2DAction OnHide(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnHide(action, oneShot, deferred);

        public static VisibilityEnabler2DAction OnItemRectChanged(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static VisibilityEnabler2DAction OnReady(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnReady(action, oneShot, deferred);

        public static VisibilityEnabler2DAction OnRenamed(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static VisibilityEnabler2DAction OnScreenEntered(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnScreenEntered(action, oneShot, deferred);

        public static VisibilityEnabler2DAction OnScreenExited(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnScreenExited(action, oneShot, deferred);

        public static VisibilityEnabler2DAction OnScriptChanged(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static VisibilityEnabler2DAction OnTreeEntered(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static VisibilityEnabler2DAction OnTreeExited(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static VisibilityEnabler2DAction OnTreeExiting(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static VisibilityEnabler2DAction OnViewportEntered(this VisibilityEnabler2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnViewportEntered(action, oneShot, deferred);

        public static VisibilityEnabler2DAction OnViewportExited(this VisibilityEnabler2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnViewportExited(action, oneShot, deferred);

        public static VisibilityEnabler2DAction OnVisibilityChanged(this VisibilityEnabler2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityEnabler2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static VisibilityNotifierAction OnCameraEntered(this VisibilityNotifier target, Action<Camera> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnCameraEntered(action, oneShot, deferred);

        public static VisibilityNotifierAction OnCameraExited(this VisibilityNotifier target, Action<Camera> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnCameraExited(action, oneShot, deferred);

        public static VisibilityNotifierAction OnGameplayEntered(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnGameplayEntered(action, oneShot, deferred);

        public static VisibilityNotifierAction OnGameplayExited(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnGameplayExited(action, oneShot, deferred);

        public static VisibilityNotifierAction OnReady(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnReady(action, oneShot, deferred);

        public static VisibilityNotifierAction OnRenamed(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnRenamed(action, oneShot, deferred);

        public static VisibilityNotifierAction OnScreenEntered(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnScreenEntered(action, oneShot, deferred);

        public static VisibilityNotifierAction OnScreenExited(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnScreenExited(action, oneShot, deferred);

        public static VisibilityNotifierAction OnScriptChanged(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static VisibilityNotifierAction OnTreeEntered(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static VisibilityNotifierAction OnTreeExited(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnTreeExited(action, oneShot, deferred);

        public static VisibilityNotifierAction OnTreeExiting(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static VisibilityNotifierAction OnVisibilityChanged(this VisibilityNotifier target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifierAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static VisibilityNotifier2DAction OnDraw(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnDraw(action, oneShot, deferred);

        public static VisibilityNotifier2DAction OnHide(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnHide(action, oneShot, deferred);

        public static VisibilityNotifier2DAction OnItemRectChanged(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static VisibilityNotifier2DAction OnReady(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnReady(action, oneShot, deferred);

        public static VisibilityNotifier2DAction OnRenamed(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnRenamed(action, oneShot, deferred);

        public static VisibilityNotifier2DAction OnScreenEntered(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnScreenEntered(action, oneShot, deferred);

        public static VisibilityNotifier2DAction OnScreenExited(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnScreenExited(action, oneShot, deferred);

        public static VisibilityNotifier2DAction OnScriptChanged(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static VisibilityNotifier2DAction OnTreeEntered(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static VisibilityNotifier2DAction OnTreeExited(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnTreeExited(action, oneShot, deferred);

        public static VisibilityNotifier2DAction OnTreeExiting(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static VisibilityNotifier2DAction OnViewportEntered(this VisibilityNotifier2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnViewportEntered(action, oneShot, deferred);

        public static VisibilityNotifier2DAction OnViewportExited(this VisibilityNotifier2D target, Action<Viewport> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnViewportExited(action, oneShot, deferred);

        public static VisibilityNotifier2DAction OnVisibilityChanged(this VisibilityNotifier2D target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VisibilityNotifier2DAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static VScrollBarAction OnChanged(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnChanged(action, oneShot, deferred);

        public static VScrollBarAction OnDraw(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnDraw(action, oneShot, deferred);

        public static VScrollBarAction OnFocusEntered(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static VScrollBarAction OnFocusExited(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnFocusExited(action, oneShot, deferred);

        public static VScrollBarAction OnGuiInput(this VScrollBar target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnGuiInput(action, oneShot, deferred);

        public static VScrollBarAction OnHide(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnHide(action, oneShot, deferred);

        public static VScrollBarAction OnItemRectChanged(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static VScrollBarAction OnMinimumSizeChanged(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static VScrollBarAction OnModalClosed(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnModalClosed(action, oneShot, deferred);

        public static VScrollBarAction OnMouseEntered(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static VScrollBarAction OnMouseExited(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnMouseExited(action, oneShot, deferred);

        public static VScrollBarAction OnReady(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnReady(action, oneShot, deferred);

        public static VScrollBarAction OnRenamed(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnRenamed(action, oneShot, deferred);

        public static VScrollBarAction OnResized(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnResized(action, oneShot, deferred);

        public static VScrollBarAction OnScriptChanged(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static VScrollBarAction OnScrolling(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnScrolling(action, oneShot, deferred);

        public static VScrollBarAction OnSizeFlagsChanged(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static VScrollBarAction OnTreeEntered(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static VScrollBarAction OnTreeExited(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnTreeExited(action, oneShot, deferred);

        public static VScrollBarAction OnTreeExiting(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static VScrollBarAction OnValueChanged(this VScrollBar target, Action<float> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnValueChanged(action, oneShot, deferred);

        public static VScrollBarAction OnVisibilityChanged(this VScrollBar target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VScrollBarAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static VSeparatorAction OnDraw(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnDraw(action, oneShot, deferred);

        public static VSeparatorAction OnFocusEntered(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static VSeparatorAction OnFocusExited(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnFocusExited(action, oneShot, deferred);

        public static VSeparatorAction OnGuiInput(this VSeparator target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnGuiInput(action, oneShot, deferred);

        public static VSeparatorAction OnHide(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnHide(action, oneShot, deferred);

        public static VSeparatorAction OnItemRectChanged(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static VSeparatorAction OnMinimumSizeChanged(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static VSeparatorAction OnModalClosed(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnModalClosed(action, oneShot, deferred);

        public static VSeparatorAction OnMouseEntered(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static VSeparatorAction OnMouseExited(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnMouseExited(action, oneShot, deferred);

        public static VSeparatorAction OnReady(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnReady(action, oneShot, deferred);

        public static VSeparatorAction OnRenamed(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnRenamed(action, oneShot, deferred);

        public static VSeparatorAction OnResized(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnResized(action, oneShot, deferred);

        public static VSeparatorAction OnScriptChanged(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static VSeparatorAction OnSizeFlagsChanged(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static VSeparatorAction OnTreeEntered(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static VSeparatorAction OnTreeExited(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnTreeExited(action, oneShot, deferred);

        public static VSeparatorAction OnTreeExiting(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static VSeparatorAction OnVisibilityChanged(this VSeparator target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSeparatorAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static VSliderAction OnChanged(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnChanged(action, oneShot, deferred);

        public static VSliderAction OnDraw(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnDraw(action, oneShot, deferred);

        public static VSliderAction OnFocusEntered(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static VSliderAction OnFocusExited(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnFocusExited(action, oneShot, deferred);

        public static VSliderAction OnGuiInput(this VSlider target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnGuiInput(action, oneShot, deferred);

        public static VSliderAction OnHide(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnHide(action, oneShot, deferred);

        public static VSliderAction OnItemRectChanged(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static VSliderAction OnMinimumSizeChanged(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static VSliderAction OnModalClosed(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnModalClosed(action, oneShot, deferred);

        public static VSliderAction OnMouseEntered(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static VSliderAction OnMouseExited(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnMouseExited(action, oneShot, deferred);

        public static VSliderAction OnReady(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnReady(action, oneShot, deferred);

        public static VSliderAction OnRenamed(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnRenamed(action, oneShot, deferred);

        public static VSliderAction OnResized(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnResized(action, oneShot, deferred);

        public static VSliderAction OnScriptChanged(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static VSliderAction OnSizeFlagsChanged(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static VSliderAction OnTreeEntered(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static VSliderAction OnTreeExited(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnTreeExited(action, oneShot, deferred);

        public static VSliderAction OnTreeExiting(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static VSliderAction OnValueChanged(this VSlider target, Action<float> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnValueChanged(action, oneShot, deferred);

        public static VSliderAction OnVisibilityChanged(this VSlider target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSliderAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static VSplitContainerAction OnDragged(this VSplitContainer target, Action<int> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnDragged(action, oneShot, deferred);

        public static VSplitContainerAction OnDraw(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnDraw(action, oneShot, deferred);

        public static VSplitContainerAction OnFocusEntered(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static VSplitContainerAction OnFocusExited(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnFocusExited(action, oneShot, deferred);

        public static VSplitContainerAction OnGuiInput(this VSplitContainer target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnGuiInput(action, oneShot, deferred);

        public static VSplitContainerAction OnHide(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnHide(action, oneShot, deferred);

        public static VSplitContainerAction OnItemRectChanged(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static VSplitContainerAction OnMinimumSizeChanged(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static VSplitContainerAction OnModalClosed(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnModalClosed(action, oneShot, deferred);

        public static VSplitContainerAction OnMouseEntered(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static VSplitContainerAction OnMouseExited(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnMouseExited(action, oneShot, deferred);

        public static VSplitContainerAction OnReady(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnReady(action, oneShot, deferred);

        public static VSplitContainerAction OnRenamed(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnRenamed(action, oneShot, deferred);

        public static VSplitContainerAction OnResized(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnResized(action, oneShot, deferred);

        public static VSplitContainerAction OnScriptChanged(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static VSplitContainerAction OnSizeFlagsChanged(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static VSplitContainerAction OnSortChildren(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnSortChildren(action, oneShot, deferred);

        public static VSplitContainerAction OnTreeEntered(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static VSplitContainerAction OnTreeExited(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnTreeExited(action, oneShot, deferred);

        public static VSplitContainerAction OnTreeExiting(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static VSplitContainerAction OnVisibilityChanged(this VSplitContainer target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<VSplitContainerAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static WindowDialogAction OnAboutToShow(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnAboutToShow(action, oneShot, deferred);

        public static WindowDialogAction OnDraw(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnDraw(action, oneShot, deferred);

        public static WindowDialogAction OnFocusEntered(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnFocusEntered(action, oneShot, deferred);

        public static WindowDialogAction OnFocusExited(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnFocusExited(action, oneShot, deferred);

        public static WindowDialogAction OnGuiInput(this WindowDialog target, Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnGuiInput(action, oneShot, deferred);

        public static WindowDialogAction OnHide(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnHide(action, oneShot, deferred);

        public static WindowDialogAction OnItemRectChanged(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static WindowDialogAction OnMinimumSizeChanged(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnMinimumSizeChanged(action, oneShot, deferred);

        public static WindowDialogAction OnModalClosed(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnModalClosed(action, oneShot, deferred);

        public static WindowDialogAction OnMouseEntered(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnMouseEntered(action, oneShot, deferred);

        public static WindowDialogAction OnMouseExited(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnMouseExited(action, oneShot, deferred);

        public static WindowDialogAction OnPopupHide(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnPopupHide(action, oneShot, deferred);

        public static WindowDialogAction OnReady(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnReady(action, oneShot, deferred);

        public static WindowDialogAction OnRenamed(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnRenamed(action, oneShot, deferred);

        public static WindowDialogAction OnResized(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnResized(action, oneShot, deferred);

        public static WindowDialogAction OnScriptChanged(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static WindowDialogAction OnSizeFlagsChanged(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnSizeFlagsChanged(action, oneShot, deferred);

        public static WindowDialogAction OnTreeEntered(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static WindowDialogAction OnTreeExited(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnTreeExited(action, oneShot, deferred);

        public static WindowDialogAction OnTreeExiting(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static WindowDialogAction OnVisibilityChanged(this WindowDialog target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WindowDialogAction>(target).OnVisibilityChanged(action, oneShot, deferred);

        public static WorldEnvironmentAction OnReady(this WorldEnvironment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WorldEnvironmentAction>(target).OnReady(action, oneShot, deferred);

        public static WorldEnvironmentAction OnRenamed(this WorldEnvironment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WorldEnvironmentAction>(target).OnRenamed(action, oneShot, deferred);

        public static WorldEnvironmentAction OnScriptChanged(this WorldEnvironment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WorldEnvironmentAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static WorldEnvironmentAction OnTreeEntered(this WorldEnvironment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WorldEnvironmentAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static WorldEnvironmentAction OnTreeExited(this WorldEnvironment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WorldEnvironmentAction>(target).OnTreeExited(action, oneShot, deferred);

        public static WorldEnvironmentAction OnTreeExiting(this WorldEnvironment target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<WorldEnvironmentAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static YSortAction OnDraw(this YSort target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<YSortAction>(target).OnDraw(action, oneShot, deferred);

        public static YSortAction OnHide(this YSort target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<YSortAction>(target).OnHide(action, oneShot, deferred);

        public static YSortAction OnItemRectChanged(this YSort target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<YSortAction>(target).OnItemRectChanged(action, oneShot, deferred);

        public static YSortAction OnReady(this YSort target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<YSortAction>(target).OnReady(action, oneShot, deferred);

        public static YSortAction OnRenamed(this YSort target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<YSortAction>(target).OnRenamed(action, oneShot, deferred);

        public static YSortAction OnScriptChanged(this YSort target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<YSortAction>(target).OnScriptChanged(action, oneShot, deferred);

        public static YSortAction OnTreeEntered(this YSort target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<YSortAction>(target).OnTreeEntered(action, oneShot, deferred);

        public static YSortAction OnTreeExited(this YSort target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<YSortAction>(target).OnTreeExited(action, oneShot, deferred);

        public static YSortAction OnTreeExiting(this YSort target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<YSortAction>(target).OnTreeExiting(action, oneShot, deferred);

        public static YSortAction OnVisibilityChanged(this YSort target, Action action, bool oneShot = false, bool deferred = false) =>
            NodeAction.GetProxy<YSortAction>(target).OnVisibilityChanged(action, oneShot, deferred);
    }
}