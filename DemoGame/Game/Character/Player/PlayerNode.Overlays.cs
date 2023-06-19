using Betauer.Animation.Easing;
using Betauer.Application.Monitor;
using Betauer.Core.Nodes;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.Nodes;
using Godot;
using Veronenger.Game.Items;

namespace Veronenger.Game.Character.Player; 

public partial class PlayerNode {

	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
	[Inject] private ItemsManager ItemsManager { get; set; }

	private void ConfigureOverlay() {
		var drawEvent = this.OnDraw(canvas => {
			foreach (var floorRaycast in FloorRaycasts) canvas.DrawRaycast(floorRaycast, Colors.Red);
			canvas.DrawRaycast(RaycastCanJump, Colors.Red);
		});
		drawEvent.Disable();


		var overlay = DebugOverlayManager.Overlay(CharacterBody2D)
			.Title("Player")
			.SetMaxSize(1000, 1000);

		// AddBulletSpeedConfigurator(overlay);
		// AddPlayerJoypadMappings(overlay);
		// AddCameraAndZoomTests(overlay);
		// AddDebuggingInputAction(overlay);
		// AddOverlayHelpers(overlay);
		// AddOverlayStates(overlay);
		// AddOverlayMotion(overlay);
		// AddOverlayCollisions(overlay);
	}

	private void AddPlayerJoypadMappings(DebugOverlay overlay) {
		overlay.OpenBox()
			.Text("PlayerJoys", () => PlayerMapping.ToString()).EndMonitor()
			.CloseBox();
	}

	private void AddBulletSpeedConfigurator(DebugOverlay overlay) {
		overlay.OpenBox()
			.Edit("Bullet speed", ItemsManager.SlowGun.Speed.ToString("0"), v => ItemsManager.SlowGun.Speed = int.Parse(v)).EndMonitor()
			.Edit("Bullet Trail", ItemsManager.SlowGun.TrailLength.ToString("0"), v => ItemsManager.SlowGun.TrailLength = int.Parse(v)).EndMonitor()
			.Edit("Raycast", ItemsManager.SlowGun.RaycastLength.ToString("0"), v => ItemsManager.SlowGun.RaycastLength = int.Parse(v)).EndMonitor()
			.CloseBox();
	}

	private void AddCameraAndZoomTests(DebugOverlay overlay) {
		var spawnPlayer = PlatformWorld.Get().GetNode<Marker2D>("SpawnPlayer");
		overlay.OpenBox()
			.Text("Following", () => _cameraController.IsFollowing).EndMonitor()
			.Text("Transition", () => _cameraController.IsBusy()).EndMonitor();

		overlay.OpenBox()
			.Button("Follow player", () => _cameraController.Follow(CharacterBody2D))
			.Button("Follow player pos", () => _cameraController.Follow(() => CharacterBody2D.GlobalPosition))
			.Button("Stop", () => _cameraController.StopFollowing())
			.Button("Start", () => _cameraController.ContinueFollowing())
			.Button("MoveTo Node", () => _cameraController.MoveTo(spawnPlayer.GlobalPosition, 0.8f))
			.Button("MoveTo Pos", () => _cameraController.MoveTo(() => spawnPlayer.GlobalPosition, 0.8f))
			.Button("MoveTo Node", () => _cameraController.MoveTo(spawnPlayer.GlobalPosition, 0.8f, BezierCurve.Create(0.755f, 0.05f, 0.855f, 0.06f)))
			.Button("MoveTo Pos", () => _cameraController.MoveTo(() => spawnPlayer.GlobalPosition, 0.8f, BezierCurve.Create(0.755f, 0.05f, 0.855f, 0.06f)))
			.Button("Zoom", async () => {
				await _cameraController.Zoom(new Vector2(4f, 4f), 0.8f, null, () => _cameraController.Camera2D.GetLocalMousePosition());
				await _cameraController.Zoom(new Vector2(0.5f, 0.5f), 0.8f, BezierCurve.Create(0.755f, 0.05f, 0.855f, 0.06f),
					() => _cameraController.Camera2D.GetLocalMousePosition());
				await _cameraController.Zoom(new Vector2(2f, 2f), 0.2f, BezierCurve.Create(0.755f, 0.05f, 0.855f, 0.06f),
					() => _cameraController.Camera2D.GetLocalMousePosition());
			});
	}

	private void AddDebuggingInputAction(DebugOverlay overlay) {
		overlay.OpenBox()
			.Text("LR", () => _joypadController.Lateral.Strength.ToString("0.00") ).EndMonitor()
			.Text("UD", () => _joypadController.Vertical.Strength.ToString("0.00") ).EndMonitor()
			.CloseBox();

		overlay.OpenBox()
			.Text("P", () => Jump.IsPressed).EndMonitor()
			.Text("JP", () => Jump.IsJustPressed).EndMonitor()
			.Text("Pressed", () => Jump.PressedTime.ToString("0.00")).EndMonitor()
			.Text("R", () => Jump.IsJustReleased).EndMonitor()
			.Text("Released", () => Jump.ReleasedTime.ToString("0.00")).EndMonitor()
			.CloseBox();

		overlay.OpenBox()
			.Text("P", () => Attack.IsPressed).EndMonitor()
			.Text("JP", () => Attack.IsJustPressed).EndMonitor()
			.Text("Pressed", () => Attack.PressedTime.ToString("0.00")).EndMonitor()
			.Text("R", () => Attack.IsJustReleased).EndMonitor()
			.Text("Released", () => Attack.ReleasedTime.ToString("0.00")).EndMonitor()
			.CloseBox();
		
		var prevResult = "";
		var axisLogger = this.OnProcess(d => {
			var v = string (InputAction a) => {
				var x = a.IsJustPressed? "JP" : a.IsPressed ? "P" : a.IsJustReleased?"R":null;
				return x == null ? "" : a.Name + "[" + x + "]";
			};

			var result = "";
			if (_joypadController.Lateral.Strength == 0f) {
				result = $"-{_joypadController.Left.Strength:0.00}+{_joypadController.Right.Strength:0.00}={_joypadController.Lateral.Strength:0.00} {v(_joypadController.Left)}{v(_joypadController.Right)} ";
			} else {
				var left = _joypadController.Left.Strength > 0 ? "-"+_joypadController.Left.Strength.ToString("0.00") : "     ";
				var right = _joypadController.Right.Strength > 0 ? "+"+_joypadController.Right.Strength.ToString("0.00") : "     ";
				result =
					$"{left}{right}={_joypadController.Lateral.Strength:0.00} {v(_joypadController.Left)}{v(_joypadController.Right)} ";
			}
			if (result != prevResult) {
				GD.Print(result);
				prevResult = result;
			}
			var godotAxis = Input.GetAxis("Left", "Right").ToString("0.00");
			if (godotAxis != _joypadController.Lateral.Strength.ToString("0.00")) {
				GD.Print("wooo");
			}
		});
		axisLogger.Disable();
	}

	public void AddOverlayHelpers(DebugOverlay overlay) {
		_jumpHelperMonitor = overlay.Text("JumpHelper");
		_coyoteMonitor = overlay.Text("Coyote");
	}

	public void AddOverlayStates(DebugOverlay overlay) {
		overlay
			.OpenBox()
				.Text("State", () => _fsm.CurrentState.Key.ToString()).EndMonitor()
			.CloseBox();
	}

	public void AddOverlayMotion(DebugOverlay overlay) {
		overlay
			.OpenBox()
				.Vector("Motion", () => PlatformBody.Motion, PlayerConfig.MaxSpeed).SetChartWidth(100).EndMonitor()
				.Graph("MotionX", () => PlatformBody.MotionX, -PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed).AddSeparator(0)
					.AddSerie("MotionY").Load(() => PlatformBody.MotionY).EndSerie()
				.EndMonitor()
			.CloseBox()
			.GraphSpeed("Speed", PlayerConfig.JumpSpeed * 2).EndMonitor();

	}
	
	public void AddOverlayCollisions(DebugOverlay overlay) {    
		overlay
			.Graph("Floor", () => PlatformBody.IsOnFloor()).Keep(10).SetChartHeight(10)
				.AddSerie("Slope").Load(() => PlatformBody.IsOnSlope()).EndSerie()
			.EndMonitor()
			.Text("Floor", () => PlatformBody.GetFloorCollisionInfo()).EndMonitor()
			.Text("Ceiling", () => PlatformBody.GetCeilingCollisionInfo()).EndMonitor()
			.Text("Wall", () => PlatformBody.GetWallCollisionInfo()).EndMonitor();
	}


}
