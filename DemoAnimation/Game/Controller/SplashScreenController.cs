using Betauer;
using Betauer.Application;
using Betauer.DI;
using DemoAnimation.Game.Managers;
using Godot;
using SettingsManager = DemoAnimation.Game.Managers.SettingsManager;

namespace DemoAnimation.Game.Controller {
    public class SplashScreenController : Control {
        [Inject] private GameManager _gameManager;
        [Inject] private SettingsManager _settingsManager;
        [Inject] private InputManager _inputManager;

        public override async void _Ready() {
            var defaults = new ApplicationConfig.UserSettings();
            var userSettingsFile = new SettingsFile(defaults, _inputManager.ConfigurableActionList);
            _settingsManager.Load(userSettingsFile);
            await this.AwaitIdleFrame();
            _gameManager.OnFinishLoad(this);
        }
    }
}