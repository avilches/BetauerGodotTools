using Betauer;
using Betauer.Animation;
using Betauer.Animation.Tween;
using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller {
    public class SplashScreenController : Control {
        [Inject] private GameManager _gameManager { get; set; }
        [Inject] private ScreenSettingsManager _screenSettingsManager { get; set; }
        [Inject] private SettingsContainer _settingsContainer { get; set; }
        [Inject] private MainResourceLoader _mainResourceLoader { get; set; }

        [OnReady("ColorRect")] private ColorRect ColorRect;
        [OnReady("ColorRect/CenterContainer")] private CenterContainer CenterContainer;

        [OnReady("ColorRect/CenterContainer/TextureRect")]
        private TextureRect _sprite;

        private Vector2 _baseResolutionSize;
        private bool _loadFinished = false;
        

        public override void _Ready() {
            _baseResolutionSize = _screenSettingsManager.WindowedResolution.Size;
            if (_screenSettingsManager.Fullscreen) {
                OS.WindowFullscreen = true;
            } else {
                OS.WindowSize = _screenSettingsManager.WindowedResolution.Size;
                OS.CenterWindow();
            }
            GetTree().SetScreenStretch(SceneTree.StretchMode.Mode2d, SceneTree.StretchAspect.Keep,_baseResolutionSize, 1);
            CenterContainer.RectSize = _baseResolutionSize;
            ColorRect.RectSize = _baseResolutionSize;
            ColorRect.Color = Colors.Aqua.Darkened(0.9f);
            Visible = true;
            _mainResourceLoader.OnProgress += context => {
                // GD.Print("SPLASH " + context.TotalLoadedPercent);
            };
            Sequence
                .Create(_sprite)
                .SetInfiniteLoops()
                .AnimateSteps(Property.Modulate)
                .From(Colors.White)
                .To(Colors.Red, 0.2f)
                .EndAnimate().Play(_sprite);
        }

        public override void _Process(float delta) {
            if (!_gameManager.IsState(GameManager.State.Init)) QueueFree();
        }

    }
}