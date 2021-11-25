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
            ConfigureScreen();
        }

        private void ConfigureScreen() {
            SceneTree tree = GetTree();
            Viewport rootViewport = GetNode<Viewport>("/root");
            if (IsRunningTests) {
                ScreenManager = new ScreenManager(tree, rootViewport, new Vector2(1200, 900), SceneTree.StretchMode.Disabled,
                    SceneTree.StretchAspect.Expand);
                ScreenManager.Configure(false, 1, false);
            } else {
                ScreenManager = new ScreenManager(tree, rootViewport, FULL_DIV4, SceneTree.StretchMode.Viewport,
                    SceneTree.StretchAspect.Keep);
                ScreenManager.Configure(false, 3, false);
            }
            tree.Connect(GodotConstants.GODOT_SIGNAL_screen_resized, this, nameof(OnScreenResized));
        }

        public void OnScreenResized() {
            ScreenManager.UpdateResolution();
        }

        public void ChangeScene(string scene) {
            // _logger.Debug($"Change scene to: {scene}");
            StageManager.ClearTransition();
            // https://godotengine.org/qa/24773/how-to-load-and-change-scenes
            // https://docs.godotengine.org/en/3.1/getting_started/step_by_step/singletons_autoload.html#custom-scene-switcher
            // https://github.com/kurtsev0103/godot-app-delegate
            Error error = GetTree().ChangeScene(scene);
        }

        public void Quit() {
            LoggerFactory.Dispose();
            GetTree().Quit();
        }
    }
}