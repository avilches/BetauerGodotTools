using Betauer;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller {
    public class SplashScreenController : DiControl {
        [Inject] GameManager GameManager;
        [Inject] public ScreenManager ScreenManager;

        public override void Ready() {
            GameManager.PreloadMainMenu();
            ScreenManager.LoadSettingsAndConfigure();
            GameManager.LoadMainMenu(this);
        }
    }
}