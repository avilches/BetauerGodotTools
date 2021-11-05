using System;
using Veronenger.Game.Tools.Area;
using Veronenger.Game.Tools.Character;
using Veronenger.Game.Tools.Platforms;
using Veronenger.Game.Tools.Resolution;
using Godot;
using Tools.Events;
using Veronenger.Game.Controller;

namespace Veronenger.Game.Tools {
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
    public class GameManager : Node {
        public static GameManager Instance { get; private set; }

        public readonly AreaManager AreaManager;
        public readonly PlatformManager PlatformManager;
        public readonly SlopeStairsManager SlopeStairsManager;
        public readonly SceneManager SceneManager;
        private ScreenManager ScreenManager;
        public readonly CharacterManager CharacterManager;


        // new Vector2(320, 180),   // 1920x1080 / 6
        public static Vector2 FULL_DIV4 = new Vector2(480, 270);    // 1920x1080 / 4
        public static Vector2 FULL_DIV2 = new Vector2(960, 540);    // 1920x1080 / 2
        public static Vector2 FULLHD = new Vector2(1920, 1080);
        public static Vector2 FULLHDx133 = new Vector2(2560, 1440); // 1920x1080 * 1.33

        public GameManager() {
            if (Instance != null) {
                throw new Exception("Only one instance");
            }
            Instance = this;
            AreaManager = new AreaManager();
            PlatformManager = new PlatformManager();
            SlopeStairsManager = new SlopeStairsManager();
            SceneManager = new SceneManager();
            CharacterManager = new CharacterManager();
        }

        public override void _EnterTree() {
            var runningTests = GetTree().CurrentScene.Filename == "res://Tests/Runner/RunTests.tscn";

            if (runningTests) {
                ScreenManager = new ScreenManager(new Vector2(1200, 900), SceneTree.StretchMode.Disabled, SceneTree.StretchAspect.Expand);
                ScreenManager.Start(this, nameof(OnScreenResized));
                ScreenManager.SetAll(false, 1, false);
            } else {
                ScreenManager = new ScreenManager(FULL_DIV4, SceneTree.StretchMode.Disabled, SceneTree.StretchAspect.Keep);
                ScreenManager.Start(this, nameof(OnScreenResized));
                ScreenManager.SetAll(false, 2, false);
            }
        }

        public void OnScreenResized() {
            ScreenManager.UpdateResolution();
        }

        public void Quit() {
            GetTree().Quit();
        }

        /**
         * Variables globales que se guardan. Se actualizan cada vez que el propio PlayerController se registra
         */
        public PlayerController PlayerController { get; private set; }

        public void RegisterPlayerController(PlayerController playerController) {
            PlayerController = playerController;
            CharacterManager.ConfigurePlayerCollisions(playerController);
        }

        public bool IsPlayer(KinematicBody2D player) {
            return PlayerController == player;
        }

        public void PlayerEnteredDeathZone(Area2D deathArea2D) {
            PlayerController.DeathZone(deathArea2D);
        }
    }
}