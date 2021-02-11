using System;
using Betauer.Characters.Player;
using Betauer.Tools.Area;
using Betauer.Tools.Platforms;
using Godot;
using static Betauer.Tools.LayerConstants;


namespace Betauer.Tools {
    /**
     * GameManager es Node para estar en autoload.
     * Añade automaticamente a los otros managers (Manager = siempre cargado)
     * Los Controller son scripts de objetos de le escena que se cargan y se registran en los managers
     *
     * Los Manager actuan de intermediarios entre objetos que no se conocen entre si. Por ejemplo: las death zones,
     * plataformas o stages se añaden en sus managers, que escucha a las señales que estos objetos producen.
     * Por otro lado, el jugador se registra en estos mismos manager para escuchar estas señales, sin llegar a saber
     * donde estan realmente estos objetos (plataformas o areas).
     *
     */
    public class GameManager : Node2D {
        public static GameManager Instance { get; private set; }

        public readonly AreaManager AreaManager;
        public readonly PlatformManager PlatformManager;
        public readonly SceneManager SceneManager;

        public GameManager() {
            if (Instance != null) {
                throw new Exception("Only one instance");
            }

            Instance = this;
            AreaManager = new AreaManager();
            PlatformManager = new PlatformManager();
            SceneManager = new SceneManager();
        }

        public override void _EnterTree() {
            AddChild(AreaManager);
            AddChild(PlatformManager);
        }


        /**
         * Variables globales que se guardan. Se actualizan cada vez que el propio PlayerController se registra
         */
        public PlayerController PlayerController { get; private set; }

        public void RegisterPlayerController(PlayerController playerController) {
            Debug.Register("GameManager.PlayerController ", playerController);
            playerController.CollisionLayer = 0;
            playerController.CollisionMask = 0;
            playerController.SetCollisionLayerBit(PLAYER_LAYER, true);

            PlayerController = playerController;
            PlatformManager.RegisterPlayer(playerController);
        }

        public bool IsPlayer(KinematicBody2D player) {
            return PlayerController == player;
        }

        public void PlayerEnteredDeathZone(Area2D deathArea2D) {
            PlayerController.DeathZone(deathArea2D);
        }
    }
}