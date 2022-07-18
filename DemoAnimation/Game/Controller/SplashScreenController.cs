using Betauer;
using Betauer.Application;
using Betauer.DI;
using DemoAnimation.Game.Managers;
using Godot;
using SettingsManager = DemoAnimation.Game.Managers.SettingsManager;

namespace DemoAnimation.Game.Controller {
    public class SplashScreenController : Control {
        public override void _Ready() {
            QueueFree();
        }
    }
}