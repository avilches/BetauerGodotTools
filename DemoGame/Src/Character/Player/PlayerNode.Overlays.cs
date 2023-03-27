using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.Nodes;
using Godot;

namespace Veronenger.Character.Player; 

public partial class PlayerNode {

	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

	public void ConfigureOverlay() {

		var overlay = DebugOverlayManager.Overlay(CharacterBody2D)
			.Title("Player")
			.SetMaxSize(1000, 1000);

		overlay.OpenBox()
			.Edit("Bullet speed", ItemConfigManager.SlowGun.Speed.ToString("0"), v => ItemConfigManager.SlowGun.Speed = int.Parse(v)).EndMonitor()
			.Edit("Bullet Trail", ItemConfigManager.SlowGun.TrailLength.ToString("0"), v => ItemConfigManager.SlowGun.TrailLength = int.Parse(v)).EndMonitor()
			.Edit("Raycast", ItemConfigManager.SlowGun.RaycastLength.ToString("0"), v => ItemConfigManager.SlowGun.RaycastLength = int.Parse(v)).EndMonitor()
			.CloseBox();

		// AddDebuggingInputAction(overlay);
		// AddOverlayHelpers(overlay);
		// AddOverlayStates(overlay);
		// AddOverlayMotion(overlay);
		// AddOverlayCollisions(overlay);

		// DebugOverlayManager.Overlay(this)
		//     .Title("Player")
		//     .Text("AnimationStack",() => _animationStack.GetPlayingLoop()?.Name + " " + _animationStack.GetPlayingOnce()?.Name).EndMonitor()
		//     .Text("TweenStack", () => _tweenStack.GetPlayingLoop()?.Name + " " + _tweenStack.GetPlayingOnce()?.Name).EndMonitor()
		//     .Add(new HBoxContainer().NodeBuilder()
		//         .Button("DangerTween.PlayLoop", () => DangerTween.PlayLoop()).End()
		//         .Button("DangerTween.Stop", () => DangerTween.Stop()).End()
		//         .TypedNode)
		//     .Add(new HBoxContainer().NodeBuilder()
		//         .Button("PulsateTween.PlayOnce", () => PulsateTween.PlayOnce()).End()
		//         .Button("PulsateTween.Stop", () => PulsateTween.Stop()).End()
		//         .TypedNode)
		//     .Add(new HBoxContainer().NodeBuilder()
		//         .Button("SqueezeTween.PlayOnce(kill)", () => SqueezeTween.PlayOnce(true)).End()
		//         .Button("SqueezeTween.Stop", () => SqueezeTween.Stop()).End()
		//         .TypedNode);
	}

	private void AddDebuggingInputAction(DebugOverlay overlay) {
		overlay.OpenBox()
			.Text("LR", () => Handler.Lateral.Strength.ToString("0.00") ).EndMonitor()
			.Text("UD", () => Handler.Vertical.Strength.ToString("0.00") ).EndMonitor()
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
			if (Handler.Lateral.Strength == 0f) {
				result = $"-{Handler.Left.Strength:0.00}+{Handler.Right.Strength:0.00}={Handler.Lateral.Strength:0.00} {v(Handler.Left)}{v(Handler.Right)} ";
			} else {
				var left = Handler.Left.Strength > 0 ? "-"+Handler.Left.Strength.ToString("0.00") : "     ";
				var right = Handler.Right.Strength > 0 ? "+"+Handler.Right.Strength.ToString("0.00") : "     ";
				result =
					$"{left}{right}={Handler.Lateral.Strength:0.00} {v(Handler.Left)}{v(Handler.Right)} ";
			}
			if (result != prevResult) {
				GD.Print(result);
				prevResult = result;
			}
			var godotAxis = Input.GetAxis("Left", "Right").ToString("0.00");
			if (godotAxis != Handler.Lateral.Strength.ToString("0.00")) {
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
