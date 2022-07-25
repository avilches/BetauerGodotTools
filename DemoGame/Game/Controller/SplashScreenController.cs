using Betauer;
using Betauer.Animation;
using Betauer.Application;
using Betauer.DI;
using Betauer.OnReady;
using Godot;
using Veronenger.Game.Managers;
using InputManager = Veronenger.Game.Managers.InputManager;

namespace Veronenger.Game.Controller {
    public class SplashScreenController : Control {
        [Inject] private GameManager _gameManager;
        [Inject] private SettingsManager _settingsManager;
        [Inject] private MainResourceLoader _mainResourceLoader;

        [OnReady("ColorRect")] private ColorRect ColorRect;
        [OnReady("ColorRect/CenterContainer")] private CenterContainer CenterContainer;

        [OnReady("ColorRect/CenterContainer/TextureRect")]
        private TextureRect _sprite;

        private readonly Launcher _launcher = new Launcher();
        private Vector2 _baseResolutionSize;
        private bool _loadFinished = false;
        
        [Inject] private InputManager _inputManager;

        public override void _Ready() {
            var defaults = new ApplicationConfig.UserSettings();
            var userSettingsFile = new SettingsFile(defaults, _inputManager.ConfigurableActionList);
            _settingsManager.Load(userSettingsFile);
            
            _baseResolutionSize = _settingsManager.SettingsFile.WindowedResolution.Size;
            if (_settingsManager.SettingsFile.Fullscreen) {
                OS.WindowFullscreen = true;
            } else {
                OS.WindowSize = _settingsManager.SettingsFile.WindowedResolution.Size;
                OS.CenterWindow();
            }
            GetTree().SetScreenStretch(SceneTree.StretchMode.Mode2d, SceneTree.StretchAspect.Keep,_baseResolutionSize, 1);
            CenterContainer.RectSize = _baseResolutionSize;
            ColorRect.RectSize = _baseResolutionSize;
            ColorRect.Color = Colors.Aqua.Darkened(0.9f);
            Visible = true;
            _mainResourceLoader.OnProgress(context => {
                // GD.Print("SPLASH " + context.TotalLoadedPercent);
            });
            _launcher.WithParent(this)
                .Play(SequenceBuilder
                        .Create(_sprite)
                        .SetInfiniteLoops()
                        .AnimateSteps(null, Property.Modulate)
                        .From(Colors.White)
                        .To(Colors.Red, 0.2f)
                        .EndAnimate()
                    , _sprite);
        }

        public override void _Process(float delta) {
            if (!_gameManager.IsState(GameManager.State.Init)) QueueFree();
        }

    }
}