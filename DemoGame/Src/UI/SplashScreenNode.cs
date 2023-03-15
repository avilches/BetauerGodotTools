using Betauer.Animation;
using Betauer.Core.Nodes.Property;
using Betauer.NodePath;
using Godot;

namespace Veronenger.UI; 

public partial class SplashScreenNode : CanvasLayer {
	[NodePath("%SplashScreen")] private Control _base;
	[NodePath("%TextureRect")] private TextureRect _sprite;

	private Tween? _tween;

	public override void _Ready() {
		// TODO Godot 4
		// Vector2 _baseResolutionSize = _screenSettingsManager.WindowedResolution.Size;
		// if (_screenSettingsManager.Fullscreen) {
		// OS.WindowFullscreen = true;
		// } else {
		// OS.WindowSize = _screenSettingsManager.WindowedResolution.Size;
		// OS.CenterWindow();
		// }
		// GetTree().SetScreenStretch(Window.ContentScaleModeEnum.CanvasItems, Window.ContentScaleAspectEnum.Keep,_baseResolutionSize, 1);
		
		_tween = SequenceAnimation
			.Create(_sprite)
			.AnimateSteps(Properties.Modulate)
			.From(Colors.White)
			.To(Colors.Red, 0.2f)
			.EndAnimate()
			.SetInfiniteLoops()
			.Play();
	}

	public void Stop() {
		_tween?.Kill();
		_tween = null;
	}
}
