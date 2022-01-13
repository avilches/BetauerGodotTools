using Godot;
using Betauer;
using Veronenger.Game.Managers.Autoload;

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
    public class
        GameManager : Node /* needed to receive _Ready TODO: is it really needed? its only used for configureMapping!*/ {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(GameManager));
        [Inject] public StageManager StageManager;
        [Inject] public InputManager InputManager;

        public override void _Ready() {
            InputManager.ConfigureMapping();
            this.DisableAllNotifications();
            Logger.Info(OS.GetName() + " application started in " + Bootstrap.Uptime.TotalMilliseconds + " ms");
        }

        public void StartGame() {
            ChangeScene("res://Worlds/World1.tscn");
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
            GetTree().Quit();
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
                Logger.Info("Application is closed by " + OS.GetName() + " notification. Uptime: " +
                            elapsed);
            } else {
                Logger.Info("User requested exit the application. Uptime: " + elapsed);
            }
            LoggerFactory.Dispose(); // Please, do this the last so previous disposing operation can log
        }
    }
}