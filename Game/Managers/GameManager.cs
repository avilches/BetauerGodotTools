using System;
using Godot;
using Betauer;
using Veronenger.Game.Managers.Autoload;
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
    public class GameManager : Node /* needed to receive _Ready and OnScreenResized signals */ {
        private static Logger _logger = LoggerFactory.GetLogger(typeof(GameManager));
        private ScreenManager ScreenManager;
        [Inject] public StageManager StageManager;
        [Inject] public InputManager InputManager;

        // 16:9 https://en.wikipedia.org/wiki/16:9_aspect_ratio
        public static Vector2 FULLHD_DIV6 = new Vector2(320, 180); // 1920x1080 / 6
        public static Vector2 FULLHD_DIV4 = new Vector2(480, 270); // 1920x1080 / 4
        public static Vector2 FULLHD_DIV2 = new Vector2(960, 540); // 1920x1080 / 2
        public static Vector2 FULLHD = new Vector2(1920, 1080);
        public static Vector2 FULLHD_2K = new Vector2(2560, 1440); // 1920x1080 * 1.33 aka "2K"
        public static Vector2 FULLHD_4K = new Vector2(3840, 2160); // 1920x1080 * 2 // aka 4K

        public static Vector2 FULLHD_DIV1_875 = new Vector2(1024, 576); // 1920x1080 / 1.875
        public static Vector2 FULLHD_DIV1_5 = new Vector2(1280, 720); // 1920x1080 / 1.5
        public static Vector2 FULLHD_DIV1_2 = new Vector2(1600, 900); // 1920x1080 / 1.2
        public static Vector2 FULLHD_DIV3 = new Vector2(640, 360); // 1920x1080 / 3

        // 16:10 https://en.wikipedia.org/wiki/16:10_aspect_ratio
        public static Vector2 R1610SMALL1 = new Vector2(640, 400);
        public static Vector2 R1610SMALL2 = new Vector2(960, 600);
        public static Vector2 WXGA = new Vector2(1280, 800);
        public static Vector2 WXGAplus = new Vector2(1440, 900);
        public static Vector2 WSXGAplus = new Vector2(1680, 1050);
        public static Vector2 WUXGA = new Vector2(1920, 1200);
        public static Vector2 WQXGA = new Vector2(2560, 1600);
        public static Vector2 WQUXGA = new Vector2(3840, 2400);

        // 21:9 https://en.wikipedia.org/wiki/21:9_aspect_ratio
        // 64:27	2.370
        public static Vector2 UWDIE237_0 = new Vector2(2560, 1080);
        // public static Vector2 UWDIE237_1 = new Vector2(5120, 2160);
        // public static Vector2 UWDIE237_2 = new Vector2(7680, 3240);
        // public static Vector2 UWDIE237_3 = new Vector2(10240, 4320);
        // 43:18	2.38
        public static Vector2 UWDI238_0 = new Vector2(3440, 1440);

        // public static Vector2 UWDI238_1 = new Vector2(5160, 2160);
        // public static Vector2 UWDI238_2 = new Vector2(6880, 2880);
        //	12:5	2.4
        public static Vector2 UWDIE24_0 = new Vector2(1920, 800);
        public static Vector2 UWDIE24_1 = new Vector2(2880, 1200);
        public static Vector2 UWDIE24_2 = new Vector2(3840, 1600);

        public static Vector2 UWDIE24_3 = new Vector2(4320, 1800);
        // public static Vector2 UWDIE24_4 = new Vector2(5760, 2400);
        // public static Vector2 UWDIE24_5 = new Vector2(7680, 3200);
        // public static Vector2 UWDIE24_6 = new Vector2(8640, 3600);

        private SceneTree _sceneTree;
        public override void _Ready() {
            _sceneTree = GetTree();
            ConfigureScreen();
            InputManager.ConfigureMapping();
            this.DisableAllNotifications();

            _logger.Info(OS.GetName() + " application started in "+Bootstrap.Uptime.TotalMilliseconds+" ms");
        }

        private void ConfigureScreen() {
            SceneTree tree = GetTree();
            Viewport rootViewport = GetNode<Viewport>("/root");
            var isRunningTests = GetTree().CurrentScene.Filename == "res://Tests/Runner/RunTests.tscn";
            if (isRunningTests) {
                DisposeSnitchObject.ShowShutdownWarning = false;
                ScreenManager = new ScreenManager(tree, rootViewport, FULLHD_DIV1_2, SceneTree.StretchMode.Disabled,
                    SceneTree.StretchAspect.Expand);
                ScreenManager.Configure(false, 1, false);
            } else {
                ScreenManager = new ScreenManager(tree, rootViewport, FULLHD_DIV4, SceneTree.StretchMode.Viewport,
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

        private bool _quited = false;

        /**
         * Method called from Main Menu -> Quit Game option
         */
        public void Quit() {
            if (_quited) return;
            _quited = true;
            CleanResources(true);
            _sceneTree.Quit();
        }

        /**
         * Detect ALT+F4 or Command+Q
         */
        public override void _Notification(int what) {
            if (what == MainLoop.NotificationWmQuitRequest) {
                CleanResources(false);
            }
        }

        private static void CleanResources(bool userRequested) {
            var timespan = Bootstrap.Uptime;
            var elapsed = $"{(int)timespan.TotalMinutes} min {timespan.Seconds:00} sec";
            if (userRequested) {
                _logger.Info("Application is closed by " + OS.GetName() + " notification. Uptime: " +
                             elapsed);
            } else {
                _logger.Info("User requested exit the application. Uptime: " + elapsed);
            }
            LoggerFactory.Dispose(); // Please, do this the last so previous disposing operation can log
        }
    }
}