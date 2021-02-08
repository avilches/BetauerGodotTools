using System;
using Betauer.Tools.Area;
using Betauer.Tools.Character;
using Betauer.Tools.Platforms;
using Godot;
using static Betauer.Tools.LayerConstants;


namespace Betauer.Tools {
    public class GameManager : Node {
        public static GameManager Instance { get; private set; }

        public GameManager() {
            if (Instance != null) {
                throw new Exception("Only one instance");
            }

            Instance = this;
        }

        public AreaManager AreaManager { get; private set; }
        public void AddManager(PlatformManager platformManager) => PlatformManager = platformManager;
        public PlatformManager PlatformManager { get; private set; }
        public void AddManager(AreaManager areaManager) => AreaManager = areaManager;

        public KinematicBody2D CurrentPlayer { get; private set; }
        public Camera2D Camera2D { get; private set; }

        public override void _Ready() {
        }

        public void RegisterPlayer(CharacterController player) {
            CurrentPlayer = player;
            Camera2D = player.GetNode<Camera2D>("Camera2D");
            if (Camera2D == null) {
                throw new System.Exception("Player must have a child node Camera2D");
            }

            player.SetCollisionLayerBit(PLAYER_LAYER, true);

            PlatformManager.RegisterPlayer(player);
            AreaManager.RegisterPlayer(player);
        }

        public bool IsPlayer(KinematicBody2D player) {
            return CurrentPlayer == player;
        }
    }
}