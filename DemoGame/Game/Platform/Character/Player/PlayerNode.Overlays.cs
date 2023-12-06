using Betauer.Animation.Easing;
using Betauer.Application.Monitor;
using Betauer.Core.Nodes;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.UI;
using Godot;
using Veronenger.Game.Platform.Items;

namespace Veronenger.Game.Platform.Character.Player; 

public partial class PlayerNode {

	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
	[Inject] private ItemsManager ItemsManager { get; set; }

	public void ConfigureOverlay() {
		// This is only drawn once, because it's the CharacterBody2D which doesn't have anything to draw. Change to the sprite to force drawing more often (or
		// use QueueRedraw() in every frame
		CharacterBody2D.Draw += () => {
			foreach (var floorRaycast in FloorRaycasts) CharacterBody2D.DrawRaycast(floorRaycast, Colors.Red);
			CharacterBody2D.DrawRaycast(RaycastCanJump, Colors.Red);
		};
		var overlay = DebugOverlayManager.Overlay(CharacterBody2D)
			.Title("Player")
			.SetMaxSize(1000, 1000);

		AddBulletSpeedConfigurator(overlay);
		AddPlayerJoypadMappings(overlay);
		AddCameraAndZoomTests(overlay);
		AddDebuggingInputAction(overlay);
		AddOverlayHelpers(overlay);
		AddOverlayStates(overlay);
		AddOverlayMotion(overlay);
		AddOverlayCollisions(overlay);
	}

	private void AddPlayerJoypadMappings(DebugOverlay overlay) {
		overlay
			.Children()
			.Add<HBoxContainer>(box => box.Children()
				.TextField("PlayerJoys", () => PlayerMapping.ToString())
			);
	}

	private void AddBulletSpeedConfigurator(DebugOverlay overlay) {
		overlay
			.Children()
			.Add<HBoxContainer>(box => box.Children()
				.Edit("Bullet speed", ItemsManager.SlowGun.Speed.ToString("0"), v => ItemsManager.SlowGun.Speed = int.Parse(v))
				.Edit("Bullet Trail", ItemsManager.SlowGun.TrailLength.ToString("0"), v => ItemsManager.SlowGun.TrailLength = int.Parse(v))
				.Edit("Raycast", ItemsManager.SlowGun.RaycastLength.ToString("0"), v => ItemsManager.SlowGun.RaycastLength = int.Parse(v))
			);
	}

	private void AddCameraAndZoomTests(DebugOverlay overlay) {
		var spawnPlayer = PlatformWorld.Get().GetNode<Marker2D>("SpawnPlayer");
		overlay
			.Children()
			.Add<HBoxContainer>(box => box.Children()
				.TextField("Following", () => _cameraController.IsFollowing)
				.TextField("Transition", () => _cameraController.IsBusy())
			);

		overlay
			.Children()
			.Add<HBoxContainer>(box => box.Children()
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
				})
			);
	}

	private void AddDebuggingInputAction(DebugOverlay overlay) {
		overlay
			.Children()
			.Add<HBoxContainer>(box => box.Children()
				.TextField("LR", () => _joypadController.Lateral.Strength.ToString("0.00"))
				.TextField("UD", () => _joypadController.Vertical.Strength.ToString("0.00")));

		overlay
			.Children()
			.Add<HBoxContainer>(box => box.Children()
				.TextField("P", () => Jump.IsPressed)
				.TextField("JP", () => Jump.IsJustPressed)
				.TextField("Pressed", () => Jump.PressedTime.ToString("0.00"))
				.TextField("R", () => Jump.IsJustReleased)
				.TextField("Released", () => Jump.ReleasedTime.ToString("0.00")));

		overlay
			.Children()
			.Add<HBoxContainer>(box => box.Children()
				.TextField("P", () => Attack.IsPressed)
				.TextField("JP", () => Attack.IsJustPressed)
				.TextField("Pressed", () => Attack.PressedTime.ToString("0.00"))
				.TextField("R", () => Attack.IsJustReleased)
				.TextField("Released", () => Attack.ReleasedTime.ToString("0.00")));

		var prevResult = "";
		OnProcess += (double d) => {
			var v = string (InputAction a) => {
				var x = a.IsJustPressed ? "JP" : a.IsPressed ? "P" : a.IsJustReleased ? "R" : null;
				return x == null ? "" : a.Name + "[" + x + "]";
			};

			var result = "";
			if (_joypadController.Lateral.Strength == 0f) {
				result =
					$"-{_joypadController.Left.Strength:0.00}+{_joypadController.Right.Strength:0.00}={_joypadController.Lateral.Strength:0.00} {v(_joypadController.Left)}{v(_joypadController.Right)} ";
			} else {
				var left = _joypadController.Left.Strength > 0 ? "-" + _joypadController.Left.Strength.ToString("0.00") : "     ";
				var right = _joypadController.Right.Strength > 0 ? "+" + _joypadController.Right.Strength.ToString("0.00") : "     ";
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
		};
	}

	public void AddOverlayHelpers(DebugOverlay overlay) {
		overlay.Children()
			.TextField("JumpHelper", text => _jumpHelperMonitor = text)
			.TextField("Coyote", text => _coyoteMonitor = text);
	}

	public void AddOverlayStates(DebugOverlay overlay) {
		overlay
			.Children()
			.Add<HBoxContainer>(box => box.Children()
				.TextField("State", () => _fsm.CurrentState.Key.ToString()));
	}

	public void AddOverlayMotion(DebugOverlay overlay) {
		overlay
			.Children()
			.Add<HBoxContainer>(box => box.Children()
				.Vector("Motion", () => PlatformBody.Motion, PlayerConfig.MaxSpeed, motion => motion.SetChartWidth(100))
				.Graph("MotionX", () => PlatformBody.MotionX, -PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed, config: motion => {
					motion.AddSeparator(0)
						.AddSerie("MotionY")
						.Load(() => PlatformBody.MotionY)
						.EndSerie();
				})
			)
			.GraphSpeed("Speed", Speedometer2D.Velocity(CharacterBody2D), PlayerConfig.JumpSpeed * 2);
	}

	public void AddOverlayCollisions(DebugOverlay overlay) {
		overlay
			.Children()
			.Graph("Floor", () => PlatformBody.IsOnFloor(), config: floor => {
				floor.Keep(10)
					.SetChartHeight(10)
					.AddSerie("Slope")
					.Load(() => PlatformBody.IsOnSlope())
					.EndSerie();
			})
			.TextField("Floor", () => PlatformBody.GetFloorCollisionInfo())
			.TextField("Ceiling", () => PlatformBody.GetCeilingCollisionInfo())
			.TextField("Wall", () => PlatformBody.GetWallCollisionInfo());
	}
}
