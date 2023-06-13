using System;
using Betauer.NodePath;
using Godot;

namespace Veronenger.Game;

public partial class SplitScreen : HBoxContainer {
	private static readonly World2D NoWorld = new(); // A cached World2D to re-use

	[NodePath("%SubViewport1")] public SubViewport SubViewport1 { get; private set; }
	[NodePath("%SubViewport2")] public SubViewport SubViewport2 { get; private set; }
	[NodePath("%Camera2D1")] public Camera2D Camera1 { get; private set; }
	[NodePath("%Camera2D2")] public Camera2D Camera2 { get; private set; }

	private float _splitCameraEffectDuration = 0.2f; // in seconds

	public int VisiblePlayers { get; private set; } = 0;
	public bool BusyPlayerTransition { get; private set; } = false;

	public void SetWorld(Node node) {
		SubViewport1.AddChild(node);
	}

	public event Action<bool> OnDoubleChanged;

	public void SinglePlayer(bool immediate) {
		VisiblePlayers = 1;
		var full = new Vector2I((int)Size.X, (int)Size.Y);
		var zero = new Vector2I(0, (int)Size.Y);
		if (immediate || true) {
			SubViewport2.World2D = NoWorld;
			SubViewport2.GetParent<SubViewportContainer>().Visible = false;
			SubViewport1.Size = full;
			OnDoubleChanged?.Invoke(false);
			BusyPlayerTransition = false;
		} else {
			BusyPlayerTransition = true;
			CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics).TweenProperty(SubViewport1, "size", full, _splitCameraEffectDuration)
				.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
			CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics).TweenProperty(SubViewport2, "size", zero, _splitCameraEffectDuration)
				.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
			CreateTween().TweenCallback(Callable.From(() => {
				SubViewport2.World2D = NoWorld;
				SubViewport2.GetParent<SubViewportContainer>().Visible = false;
				SubViewport1.Size = full;
				OnDoubleChanged?.Invoke(false);
				BusyPlayerTransition = false;
			})).SetDelay(_splitCameraEffectDuration);
		}
	}

	public void EnableSplitScreen(bool immediate) {
		VisiblePlayers = 2;
		var half = new Vector2I((int)Size.X / 2, (int)Size.Y);
		if (immediate || true) {
			SubViewport1.Size = half;
			SubViewport2.Size = half;
			SubViewport2.GetParent<SubViewportContainer>().Visible = true;
			SubViewport2.World2D = SubViewport1.World2D;
			OnDoubleChanged?.Invoke(true);
			BusyPlayerTransition = false;
		} else {
			BusyPlayerTransition = true;
			SubViewport2.World2D = SubViewport1.World2D;
			SubViewport2.Size = new Vector2I(0, (int)Size.Y);
			SubViewport2.GetParent<SubViewportContainer>().Visible = true;
			CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics).TweenProperty(SubViewport1, "size", half, _splitCameraEffectDuration)
				.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
			CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics).TweenProperty(SubViewport2, "size", half, _splitCameraEffectDuration)
				.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
			CreateTween().TweenCallback(Callable.From(() => {
				BusyPlayerTransition = false;
				OnDoubleChanged?.Invoke(true);
			})).SetDelay(_splitCameraEffectDuration);
		}
	}
}
