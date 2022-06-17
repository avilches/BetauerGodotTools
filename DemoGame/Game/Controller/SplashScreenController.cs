using System;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Managers;
using Godot;
using Veronenger.Game.Managers;
using InputManager = Veronenger.Game.Managers.InputManager;
using SettingsManager = Veronenger.Game.Managers.SettingsManager;

namespace Veronenger.Game.Controller {
    public class SplashScreenController : Control {
        [Inject] private GameManager _gameManager;
        [Inject] private SettingsManager _settingsManager;
        [Inject] private ResourceManager _resourceManager;

        [OnReady("ColorRect")] private ColorRect ColorRect;
        [OnReady("ColorRect/CenterContainer")] private CenterContainer CenterContainer;

        [OnReady("ColorRect/CenterContainer/TextureRect")]
        private TextureRect _sprite;

        private readonly Launcher _launcher = new Launcher();
        private Vector2 _baseResolutionSize;
        private bool _loadFinished = false;
        
        [Inject] private InputManager _inputManager;

        public override async void _Ready() {
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
            GetTree().SetScreenStretch(SceneTree.StretchMode.Mode2d, SceneTree.StretchAspect.Keep,
                _baseResolutionSize, 1);
            CenterContainer.RectSize = _baseResolutionSize;
            ColorRect.RectSize = _baseResolutionSize;
            ColorRect.Color = Colors.Aqua.Darkened(0.9f);
            Visible = true;
            _launcher.WithParent(this)
                .Play(SequenceBuilder
                        .Create(_sprite)
                        .SetInfiniteLoops()
                        .AnimateSteps(null, Property.Modulate)
                        .From(Colors.White)
                        .To(Colors.Red, 0.2f)
                        .EndAnimate()
                    , _sprite);

            await _resourceManager.Load(context => {
                // GD.Print(context.TotalLoadedPercent.ToString("P") + " = " + context.TotalLoadedSize + " / " +
                // context.TotalSize + " resource " + context.ResourceLoadedPercent.ToString("P") + " = " +
                // context.ResourceLoadedSize + " / " + context.ResourceSize + " " + context.ResourcePath);
            }, async () => { await GetTree().AwaitIdleFrame(); });
            _loadFinished = true;
            _launcher.RemoveAll();
        }

        public override void _Input(InputEvent e) {
            if (_loadFinished &&
                (e is InputEventKey { Pressed: true } ||
                 e is InputEventJoypadButton { Pressed: true } ||
                 e is InputEventMouseButton { Pressed: true })) {
                Visible = false;
                GetTree().SetInputAsHandled();
                _loadFinished = false; // Avoid double input executed twice
                _gameManager.OnFinishLoad(this);
            }
        }
    }
}