using System;
using Betauer.Animation;
using Betauer.Application.Lifecycle;
using Betauer.Core.Nodes.Property;
using Betauer.DI.Attributes;
using Betauer.NodePath;
using Godot;
using Veronenger.Main;
using Veronenger.Managers;

namespace Veronenger.UI; 

public partial class SplashScreenNode : CanvasLayer {
	[NodePath("%SplashScreen")] private Control _base;
	[NodePath("%TextureRect")] private TextureRect _sprite;
	
	[Inject] private GameLoader GameLoader { get; set; }

	private Tween? _tween;

	public void Start() {
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
		GameLoader.OnLoadResourceProgress += OnLoadResourceProgress;
	}

	public void OnLoadResourceProgress(ResourceProgress rp) {
		Console.WriteLine($"{rp.ResourcePercent:P} {rp.Resource}"); 
	}

	public void ResourcesLoaded() {
		GameLoader.OnLoadResourceProgress -= OnLoadResourceProgress;
		_tween?.Kill();
		_tween = null;
	}
}
