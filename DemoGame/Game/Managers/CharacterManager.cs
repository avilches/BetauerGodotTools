using System;
using Godot;
using Betauer.DI;
using Betauer.Memory;
using Betauer.Signal;
using Betauer.Signal.Bus;
using Veronenger.Game.Controller.Character;
using static Veronenger.Game.LayerConstants;
using Array = Godot.Collections.Array;

namespace Veronenger.Game.Managers {
    [Service]
    public class CharacterManager : DisposableGodotObject /* needed to receive signals TODO: it shouldn't be! */{
        private const string GROUP_ENEMY = "enemy";

        public PlayerController PlayerController { get; private set; }

        [Inject] public PlatformManager PlatformManager { get; set;}
        [Inject] public SlopeStairsManager SlopeStairsManager { get; set; }
        [Inject] public GameManager GameManager { get; set; }

        private readonly AreaOnArea2DEntered.Unicast _playerAttackBus = new AreaOnArea2DEntered.Unicast("PlayerAttack");

        public void RegisterPlayerController(PlayerController playerController) {
            PlayerController = playerController;
        }

        public void ConfigurePlayerCollisions(PlayerController playerController) {
            playerController.CollisionLayer = 0;
            playerController.CollisionMask = 0;
            PlatformManager.ConfigurePlayerCollisions(playerController);
            SlopeStairsManager.ConfigurePlayerCollisions(playerController);
        }

        public void ConfigurePlayerAttackArea2D(Area2D attackArea2D, Action<Area2D> enterMethod) {
            attackArea2D.CollisionMask = 0;
            attackArea2D.CollisionLayer = 0;
            attackArea2D.SetCollisionMaskBit(LayerEnemy, true);
            _playerAttackBus.OnEventFilter(attackArea2D, enterMethod);
        }

        public void ConfigureEnemyCollisions(KinematicBody2D enemy) {
            enemy.AddToGroup(GROUP_ENEMY);
            enemy.CollisionMask = 0;
            enemy.CollisionLayer = 0;
            PlatformManager.ConfigurePlayerCollisions(enemy);
            SlopeStairsManager.ConfigurePlayerCollisions(enemy);
        }

        public void ConfigureEnemyDamageArea2D(Area2D damageArea2D) {
            damageArea2D.CollisionMask = 0;
            damageArea2D.CollisionLayer = 0;
            damageArea2D.SetCollisionLayerBit(LayerEnemy, true);
            _playerAttackBus.Connect(damageArea2D);
        }

        public bool IsEnemy(KinematicBody2D platform) => platform.IsInGroup(GROUP_ENEMY);

        public bool IsPlayer(KinematicBody2D player) {
            return PlayerController == player;
        }

        public void ConfigurePlayerDeathZone(Area2D deathArea2D) {
            deathArea2D.CollisionLayer = 0;
            deathArea2D.CollisionMask = 0;
            // TODO: this should be a topic, so other places can subscribe like remove all bullets
            deathArea2D.SetCollisionLayerBit(LayerPlayerStageDetector, true);
            deathArea2D.Connect(SignalConstants.Area2D_AreaEnteredSignal, this, nameof(PlayerEnteredDeathZone),
                new Array { deathArea2D });
        }

        public void PlayerEnteredDeathZone(Area2D playerDetector, Area2D deathArea2D) {
            // TODO Send and event instead ?
            GD.Print("a");
            // PlayerController.DeathZone(deathArea2D);
        }

        public void ConfigureSceneChange(Area2D sceneChangeArea2D, string scene) {
            sceneChangeArea2D.Connect(SignalConstants.Area2D_AreaEnteredSignal, this,
                nameof(_on_player_entered_scene_change),
                new Array { sceneChangeArea2D, scene });
            sceneChangeArea2D.CollisionLayer = 0;
            sceneChangeArea2D.CollisionMask = 0;
            sceneChangeArea2D.SetCollisionLayerBit(LayerPlayerStageDetector, true);
        }

        public void _on_player_entered_scene_change(Area2D player, Area2D stageEnteredArea2D, string scene) {
            // TODO: Send an event instead?
            GameManager.QueueChangeSceneWithPlayer(scene);
        }
    }
}