using System;
using System.Threading.Tasks;
using Betauer.Core;
using Betauer.Core.Easing;
using Betauer.Core.Signal;
using Betauer.Nodes;
using Godot;

namespace Betauer.Camera.Control;

public class CameraController {

    private readonly CameraContainer _cameraContainer;
    private Func<Vector2>? _getFollowingPosition;
    private bool _followingPositionPaused;

    private Tween? _positionTween;
    private Tween? _zoomTween;
    private RemoteTransform2D? _remoteTransform2D;

    public Camera2D Camera2D { get; }

    public bool IsFollowing => Camera2D.IsInstanceValid() && (
        (_getFollowingPosition != null && !_followingPositionPaused) ||
        (_remoteTransform2D != null && !_remoteTransform2D.RemotePath.IsEmpty));

    public bool IsBusy() => IsZooming() || IsMoving();
    public bool IsZooming() => _zoomTween?.IsValid() ?? false;
    public bool IsMoving() => _positionTween?.IsValid() ?? false;

    public CameraController(CameraContainer cameraContainer, Camera2D camera2D) {
        _cameraContainer = cameraContainer;
        Camera2D = camera2D;
    }
    
    public void Destroy() {
        _cameraContainer.Remove(Camera2D);
        NodeManager.MainInstance.OnPhysicsProcess -= Refresh;
    }

    public CameraController Follow(Func<Vector2> getFollowingPosition) {
        _positionTween?.Kill();
        if (_remoteTransform2D != null) {
            // Unfollow Node2D
            _remoteTransform2D.RemotePath = null;
        }

        // Follow func
        _getFollowingPosition = getFollowingPosition;
        _followingPositionPaused = false;
        NodeManager.MainInstance.OnPhysicsProcess += Refresh;
        return this;
    }

    public CameraController Follow(Node2D target) {
        _positionTween?.Kill();
        if (_getFollowingPosition != null) {
            // Unfollow func forever
            NodeManager.MainInstance.OnPhysicsProcess -= Refresh;
            _getFollowingPosition = null;
        }
        
        // Follow Node2D
        _remoteTransform2D = Camera2D.Follow(target);
        return this;
    }

    public CameraController StopFollowing() {
        if (_getFollowingPosition != null) {
            // Unfollow func temporarily
            NodeManager.MainInstance.OnPhysicsProcess -= Refresh;
            _followingPositionPaused = true;
        } else if (_remoteTransform2D != null) {
            _remoteTransform2D.RemotePath = null;
        }
        return this;
    }

    public void StopZooming() {
        _zoomTween?.Kill();
    }

    public CameraController ContinueFollowing() {
        if (_getFollowingPosition != null) {
            NodeManager.MainInstance.OnPhysicsProcess += Refresh;
            _followingPositionPaused = false;
        } else if (_remoteTransform2D != null) {
            _remoteTransform2D.RemotePath = Camera2D.GetPath();
        }
        return this;
    }

    public SignalAwaiter Apply(CameraTransition cameraTransition) {
        return cameraTransition switch {
            GetPositionCameraTransition positionTarget => MoveTo(positionTarget.GetPosition, cameraTransition.Time, cameraTransition.Easing),
            PositionCameraTransition nodeTarget => MoveTo(nodeTarget.Position, cameraTransition.Time, cameraTransition.Easing),
            ZoomCameraTransition zoomTarget => Zoom(zoomTarget.Zoom, cameraTransition.Time, cameraTransition.Easing, zoomTarget.GetZoomPoint),
            _ => throw new Exception("!!!!")
        };
    }

    public SignalAwaiter MoveTo(Vector2 target, float time, IInterpolation? easing = null) {
        easing ??= Interpolation.Linear;
        if (easing is not GodotTween godotEasing) return MoveTo(() => target, time, easing);

        StopFollowing();
        _positionTween?.Kill();
        _positionTween = Camera2D.GetTree().CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics);
        _positionTween.TweenProperty(Camera2D, "global_position", target, time)
            .FromCurrent()
            .SetEase(godotEasing.EaseType)
            .SetTrans(godotEasing.TransitionType);
        return _positionTween.AwaitFinished();
    }

    public SignalAwaiter MoveTo(Func<Vector2> target, float time, IInterpolation? easing = null) {
        StopFollowing();
        _positionTween?.Kill();
        _positionTween = Camera2D.GetTree().CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics);
        easing ??= Interpolation.Linear;
        var from = Camera2D.GlobalPosition;
        _positionTween.TweenMethod(Callable.From<float>(t => {
            Camera2D.GlobalPosition = from.Lerp(target(), easing.Get(t));
        }), 0f, 1f, time);
        return _positionTween.AwaitFinished();
    }

    public SignalAwaiter Zoom(Vector2 zoom, float time, IInterpolation? easing = null, Func<Vector2>? getZoomPoint = null) {
        _zoomTween?.Kill();
        _zoomTween = Camera2D.GetTree().CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics);
        easing ??= Interpolation.Linear;
        if (easing is GodotTween godotEasing && getZoomPoint == null) {
            _zoomTween.TweenProperty(Camera2D, "zoom", zoom, time)
                .FromCurrent()
                .SetEase(godotEasing.EaseType)
                .SetTrans(godotEasing.TransitionType);
        } else {
            var from = Camera2D.Zoom;
            if (getZoomPoint != null) {
                StopFollowing();
            }
            _zoomTween.TweenMethod(Callable.From<float>(t => {
                if (getZoomPoint != null) {
                    Camera2D.ZoomTo(from.Lerp(zoom, easing.Get(t)), getZoomPoint);
                } else {
                    Camera2D.Zoom = from.Lerp(zoom, easing.Get(t));
                }
            }), 0f, 1f, time);
        }
        return _zoomTween.AwaitFinished();
    }

    public async Task Await() {
        if (_positionTween != null && _positionTween.IsValid()) await _positionTween.AwaitFinished();
        if (_zoomTween != null && _zoomTween.IsValid()) await _zoomTween.AwaitFinished();
    }

    private void Refresh(double delta) {
        if (!Camera2D.IsInstanceValid()) {
            Destroy();
            return;
        }
        if (_getFollowingPosition != null) {
            Camera2D.GlobalPosition = _getFollowingPosition.Invoke();
        } else {
            NodeManager.MainInstance.OnPhysicsProcess -= Refresh;
        }
    }
}