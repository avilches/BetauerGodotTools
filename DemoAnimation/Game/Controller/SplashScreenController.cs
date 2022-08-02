using Godot;

namespace DemoAnimation.Game.Controller {
    public class SplashScreenController : Control {
        public override void _Ready() {
            QueueFree();
        }
    }
}