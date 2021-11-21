using Godot;
using Tools;
using Veronenger.Game.Tools.Resolution;

namespace Veronenger.Game.Managers {
    /**
     * GameManager es Node para estar en autoload y recibir eventos
     * Crea automaticamente a los otros managers (Manager = siempre cargado)
     * Los Controller son scripts de objetos de le escena que se cargan y se registran en los managers
     *
     * Los Manager actuan de intermediarios entre objetos que no se conocen entre si. Por ejemplo: las death zones,
     * plataformas o stages se añaden en sus managers, que escucha a las señales que estos objetos producen.
     * Por otro lado, el jugador se registra en estos mismos manager para escuchar estas señales, sin llegar a saber
     * donde estan realmente estos objetos (plataformas o areas).
     *
     */
    [Singleton]
    public class GameManager : Node /* needed to receive _EnterTree and OnScreenResized signals */ {
        private ScreenManager ScreenManager;
        [Inject] public StageManager StageManager;

        public static Vector2 FULL_DIV6 = new Vector2(320, 180); // 1920x1080 / 6
        public static Vector2 FULL_DIV4 = new Vector2(480, 270); // 1920x1080 / 4
        public static Vector2 FULL_DIV2 = new Vector2(960, 540); // 1920x1080 / 2
        public static Vector2 FULLHD = new Vector2(1920, 1080);
        public static Vector2 FULLHDx133 = new Vector2(2560, 1440); // 1920x1080 * 1.33

        public bool IsRunningTests = false;

        public override void _EnterTree() {
            if (IsRunningTests) {
                ScreenManager = new ScreenManager(new Vector2(1200, 900), SceneTree.StretchMode.Disabled,
                    SceneTree.StretchAspect.Expand);
                ScreenManager.Start(this, nameof(OnScreenResized));
                ScreenManager.SetAll(false, 1, false);
            } else {
                ScreenManager = new ScreenManager(FULL_DIV4, SceneTree.StretchMode.Viewport,
                    SceneTree.StretchAspect.Keep);
                ScreenManager.Start(this, nameof(OnScreenResized));
                ScreenManager.SetAll(false, 3, false);
            }
        }

        public void OnScreenResized() {
            ScreenManager.UpdateResolution();
        }

        public void ChangeScene(string scene) {
            // _logger.Debug($"Change scene to: {scene}");
            StageManager.ClearTransition();
            Error error = GetTree().ChangeScene(scene);
        }

        public void Quit() {
            LoggerFactory.Dispose();
            GetTree().Quit();
        }
    }
}