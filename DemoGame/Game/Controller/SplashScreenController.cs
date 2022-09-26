using Betauer;
using Betauer.Animation;
using Betauer.Animation.Tween;
using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.DI;
using Betauer.Input;
using Betauer.Nodes.Property;
using Betauer.OnReady;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller {
    public class SplashScreenController : Control {
        [Inject] private MenuFlowManager MenuFlowManager { get; set; }
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
            SequenceAnimation
                .Create(_sprite)
                .AnimateSteps(Properties.Modulate)
                .From(Colors.White)
                .To(Colors.Red, 0.2f)
                .EndAnimate()
                .SetInfiniteLoops()
                .Play();
        }

        public override void _Process(float delta) {
            if (!MenuFlowManager.IsState(MenuFlowManager.State.Init)) QueueFree();
        }

    }
}