using Betauer;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller {
    public class SplashScreenController : DiControl {
        [Inject] GameManager GameManager;

        public override void Ready() {
            GameManager.LoadMainMenu(this);
        }
    }
}