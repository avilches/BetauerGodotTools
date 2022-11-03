using Godot;

namespace DemoAnimation.Controller {
    public class SplashScreenController : Control {
        public override void _Ready() {
            QueueFree();
        }
    }
}