using System;
using Betauer.Characters.Player;
using Betauer.Tools.Area;
using Betauer.Tools.Platforms;
using Godot;
using static Betauer.Tools.LayerConstants;


namespace Betauer.Tools {
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