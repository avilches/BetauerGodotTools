using Betauer.Animation.Easing;
using Betauer.Application.Persistent;
using Betauer.Camera;
using Betauer.Camera.Control;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.FSM.Sync;
using Betauer.Input;
using Godot;

namespace Veronenger.Game.RTS.World;

public partial class RtsWorld : Node, IInjectable {
    [Inject] private GameObjectRepository GameObjectRepository { get; set; }
    [Inject] public RtsConfig RtsConfig { get; private set; }
    [Inject] public CameraContainer CameraContainer { get; private set; }
	private readonly DragCameraController _dragCameraController = new();

    private CameraController CameraController;
    private CameraGameObject CameraGameObject;
    private Camera2D Camera => CameraController.Camera2D;

    public enum RtsState {
        DoNothing,
        Idle
    }

    public enum RtsTransition {
        Idle,
    }

    private readonly FsmNodeSync<RtsState, RtsTransition> _fsm = new(RtsState.DoNothing, "ScreenState.FSM", true);

    public void PostInject() {
        _fsm.On(RtsTransition.Idle).Set(RtsState.Idle);
        
        _fsm.State(RtsState.DoNothing)
            .Enter(() => {
                _dragCameraController.Enable(false);
            })
            .Build();

        _fsm.State(RtsState.Idle)
            .Enter(() => {
                _dragCameraController.Enable();
            })
            .OnInput(Zooming)
            .Build();
        
        _fsm.Execute();

        AddChild(_fsm);
    }

    private void Zooming(InputEvent @event) {
        if (@event.IsKeyJustPressed(Key.Q) || @event.IsClickPressed(MouseButton.WheelUp)) {
            if (CameraGameObject.ZoomLevel == RtsConfig.ZoomLevels.Count - 1) return;
            var targetZoom = RtsConfig.ZoomLevels[++CameraGameObject.ZoomLevel];
            CameraController.Zoom(new Vector2(targetZoom, targetZoom), RtsConfig.ZoomTime, Easings.Linear, CameraController.Camera2D.GetLocalMousePosition);

            GetViewport().SetInputAsHandled();
        } else if (@event.IsKeyJustPressed(Key.E) || @event.IsClickPressed(MouseButton.WheelDown)) {
            if (CameraGameObject.ZoomLevel == 0) return;
            var targetZoom = RtsConfig.ZoomLevels[--CameraGameObject.ZoomLevel];
            CameraController.Zoom(new Vector2(targetZoom, targetZoom), RtsConfig.ZoomTime, Easings.Linear, CameraController.Camera2D.GetLocalMousePosition);

            GetViewport().SetInputAsHandled();
        }
    }

    public void SetMainCamera(Camera2D camera2D) {
        CameraController = CameraContainer.Camera(camera2D);
        _dragCameraController.Attach(camera2D).WithMouseButton(MouseButton.Left).Enable(false);
    }

    public void StartNewGame() {
        CameraGameObject = GameObjectRepository.Create<CameraGameObject>("ScreenState", "ScreenState");
        Init();
    }

    public void LoadGame(RtsSaveGameConsumer consumer) {
        CameraGameObject = GameObjectRepository.Get<CameraGameObject>("ScreenState");
        Init();
        CameraController.Camera2D.Position = CameraGameObject.Position;
    }

    private void Init() {
        CameraGameObject.Configure(Camera);
        var zoom = RtsConfig.ZoomLevels[CameraGameObject.ZoomLevel];
        CameraController.Camera2D.Zoom = new Vector2(zoom, zoom);
        _fsm.Send(RtsTransition.Idle);
    }
}