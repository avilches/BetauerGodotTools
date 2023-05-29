using System;
using System.Threading.Tasks;
using Betauer.Animation.Easing;
using Betauer.Core;
using Betauer.Core.Signal;
using Betauer.Nodes;
using Godot;

namespace Betauer.Camera.Follower;

public class CameraControl {

    private readonly CameraContainer _cameraContainer;
    private Func<Vector2>? _getFollowingPosition;
    private Func<bool>? _isFollowingEnabledFunc;
    private bool IsFollowingEnabled => _isFollowingEnabledFunc == null || _isFollowingEnabledFunc(); 

    private readonly CameraTransition _positionTransition;
    private Tween? _zoomTween;
    private RemoteTransform2D? _remoteTransform2D;

    public Camera2D Camera2D { get; }
    public bool IsFollowing { get; set; } = true;
    public IEventHandler Handler { get; }
    public bool IsBusy => _positionTransition.IsBusy;

    public CameraControl(CameraContainer cameraContainer, Camera2D camera2D) {
        _cameraContainer = cameraContainer;
        Camera2D = camera2D;
        Handler = DefaultNodeHandler.Instance.OnPhysicsProcess(Refresh);
        Handler.Disable();
        _positionTransition = new CameraTransition(Camera2D);
    }
    
    public void Destroy() {
        _cameraContainer.Remove(Camera2D);
        Handler.Destroy();
    }

    public CameraControl Follow(Func<Vector2> getFollowingPosition, Func<bool>? isFollowingEnabled = null) {
        _positionTransition.Stop();
        _isFollowingEnabledFunc = isFollowingEnabled;
        _getFollowingPosition = getFollowingPosition;
        if (_remoteTransform2D != null) _remoteTransform2D.RemotePath = null;
        Handler.Enable();
        return this;
    }

    public CameraControl Follow(Node2D target) {
        _positionTransition.Stop();
        _isFollowingEnabledFunc = null;
        _getFollowingPosition = null;
        _remoteTransform2D = Camera2D.Follow(target);
        return this;
    }

    public CameraControl Stop() {
        IsFollowing = false;
        if (_remoteTransform2D != null) _remoteTransform2D.RemotePath = null;
        return this;
    }

    public CameraControl Start() {
        IsFollowing = true;
        if (_remoteTransform2D != null) _remoteTransform2D.RemotePath = Camera2D.GetPath();
        Handler.Enable();
        return this;
    }

    public Task MoveTo(Target target) {
        if (_remoteTransform2D != null) _remoteTransform2D.RemotePath = null;
        _positionTransition.Start(target);
        if (target.ZoomValue.HasValue) Zoom(target);
        Handler.Enable();
        return AwaitTransition();
    }

    public SignalAwaiter Zoom(ZoomTarget target) {
        return target.ZoomValue.HasValue ? Zoom(target.ZoomValue.Value, target.Time, target.EasingZoom) : Camera2D.AwaitProcessFrame();
    }

    public SignalAwaiter Zoom(float zoom, float time, IEasing? easing = null) {
        return Zoom(new Vector2(zoom, zoom), time, easing);
    }

    public SignalAwaiter Zoom(Vector2 zoom, float time, IEasing? easing = null) {
        _zoomTween?.Kill();
        _zoomTween = Camera2D.GetTree().CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics);
        easing ??= Easings.Linear;
        if (easing is GodotEasing godotEasing) {
            _zoomTween.TweenProperty(Camera2D, "zoom", zoom, time).FromCurrent().SetEase(godotEasing.EaseType).SetTrans(godotEasing.TransitionType);
        } else {
            var from = Camera2D.Zoom;
            _zoomTween.TweenMethod(Callable.From<float>(t => Camera2D.Zoom = from.Lerp(zoom, easing.GetY(t))), 0f, 1f, time);
        }
        return _zoomTween.AwaitFinished();
    }

    public Task AwaitTransition() {
        return _positionTransition.AwaitTransition();
    }

    public SignalAwaiter AwaitZoom() {
        return _zoomTween != null && _zoomTween.IsValid() ? _zoomTween.AwaitFinished() : Camera2D.AwaitProcessFrame();
    }

    private void Refresh(double delta) {
        if (!Camera2D.IsInstanceValid()) {
            Destroy();
            return;
        }
        if (_positionTransition.IsBusy) {
            _positionTransition.Refresh(delta);
        } else if (IsFollowing && _getFollowingPosition != null) {
            if (IsFollowingEnabled) Camera2D.GlobalPosition = _getFollowingPosition.Invoke();
        } else {
            Handler.Disable();
        }
    }

    private class CameraTransition {
        private readonly Camera2D _camera2D;
        private TaskCompletionSource _promise;
        private double _elapsedTime = 0;
        private Target _target;

        public bool IsBusy { get; private set; } = false;

        private Action? _onFinish;

        protected internal CameraTransition(Camera2D camera2D) {
            _camera2D = camera2D;
        }

        private Vector2 _startPosition; 

        public void Start(Target target, Action? onFinish = null) {
            if (!IsBusy) {
                _promise = new TaskCompletionSource();
                IsBusy = true;
            }
            _target = target;
            _onFinish = onFinish;
            _elapsedTime = 0;
            _startPosition = _camera2D.GlobalPosition;
        }
            
        public void Refresh(double delta) {
            _elapsedTime += delta;
            if (_elapsedTime > _target.Time) {
                Snap();
                _onFinish?.Invoke();
                Stop();
            } else {
                Lerp((float)(_elapsedTime / _target.Time)); // a 0..1 value meaning the progress of the transition
            }
        }
        public Task AwaitTransition() => IsBusy ? _promise.Task : Task.CompletedTask;

        public void Stop() {
            if (IsBusy) {
                IsBusy = false;
                _promise.SetResult();
            }
        }

        private void Snap() {
            _camera2D.GlobalPosition = _target.GetPosition();
        }

        private void Lerp(double t) {
            var easingPosition = _target.EasingPosition ?? Easings.Linear;
            _camera2D.GlobalPosition = _startPosition.Lerp(_target.GetPosition(), easingPosition.GetY((float)t));
        }
    }
}