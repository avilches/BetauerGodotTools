using System;
using Betauer.Bus;
using Betauer.Bus.Signal;
using Godot;
using Betauer.DI;
using Betauer.Signal;
using Veronenger.Character.Player;
using Veronenger.Controller.Character;
using static Veronenger.LayerConstants;

namespace Veronenger.Managers {
    [Service]
    public class CharacterManager {
        private const string GROUP_ENEMY = "enemy";

        public PlayerController PlayerController { get; private set; }
        [Inject] private Game Game { get; set; }

        [Inject] public PlatformManager PlatformManager { get; set;}
        [Inject] public SlopeStairsManager SlopeStairsManager { get; set; }
        [Inject] private Bus Bus { get; set; }

        private readonly AreaOnArea2DEntered.Unicast _playerAttackBus = new("PlayerAttack");
        private readonly AreaOnArea2DEntered.Unicast _playerDamageBus = new("PlayerDamage");

        public void RegisterPlayerController(PlayerController playerController) {
            PlayerController = playerController;
        }

        public void ConfigurePlayerCollisions(PlayerController playerController) {
            playerController.CollisionLayer = 0;
            playerController.CollisionMask = 0;
            
            PlatformManager.ConfigureCharacterCollisionsWithGroundAndPlatforms(playerController);
            PlatformManager.ConfigureCharacterCollisionsWithGroundAndPlatforms(playerController.SlopeRaycast);
            playerController.FloorRaycasts.ForEach(PlatformManager.ConfigureCharacterCollisionsWithGroundAndPlatforms);
            
            SlopeStairsManager.ConfigureCharacterCollisionsWithSlopeStairs(playerController);
            
            playerController.PlayerDetector.CollisionLayer = 0;
            playerController.PlayerDetector.CollisionMask = 0;
            playerController.PlayerDetector.SetCollisionMaskBit(LayerPlayerStageDetector, true);
        }

        // Only one player attack area is allowed.
        // If there are more than one, change from Unicast to Multicast
        public void ConfigurePlayerAttackArea2D(Area2D attackArea2D, Action<Area2D, Area2D> onAttack) {
            attackArea2D.CollisionMask = 0;
            attackArea2D.CollisionLayer = 0;
            attackArea2D.SetCollisionMaskBit(LayerEnemy, true);
            _playerAttackBus.Subscribe(onAttack)
                .WithFilter(attackArea2D); // Filter is redundant in unicast: publisher (the enemy) changes, but the attack area is always the same!
        }

        // Only one player damage area is allowed.
        // If there are more than one, change from Unicast to Multicast
        public void ConfigurePlayerDamageArea2D(Area2D damageArea2D, Action<Area2D, Area2D> onDamage) {
            damageArea2D.CollisionMask = 0;
            damageArea2D.CollisionLayer = 0;
            damageArea2D.SetCollisionMaskBit(LayerEnemy, true);
            _playerDamageBus.Subscribe(onDamage)
                .WithFilter(damageArea2D); // Filter is redundant in unicast: publisher (the enemy) changes, but the attack area is always the same!
        }

        public void ConfigureEnemyCollisions(KinematicBody2D enemy) {
            enemy.AddToGroup(GROUP_ENEMY);
            enemy.CollisionMask = 0;
            enemy.CollisionLayer = 0;                                       
            PlatformManager.ConfigureCharacterCollisionsWithGroundAndPlatforms(enemy);
            SlopeStairsManager.ConfigureCharacterCollisionsWithSlopeStairs(enemy);
        }

        public void ConfigureEnemyCollisions(RayCast2D raycast) {
            raycast.CollisionMask = 0;
            PlatformManager.ConfigureCharacterCollisionsWithGroundAndPlatforms(raycast);
        }

        public void ConfigureEnemyDamageArea2D(Area2D enemyDamageArea2D) {
            if (enemyDamageArea2D.GetParent() is not IEnemy) throw new Exception("Only enemies can use this method");
            enemyDamageArea2D.CollisionMask = 0;
            enemyDamageArea2D.CollisionLayer = 0;
            enemyDamageArea2D.SetCollisionLayerBit(LayerEnemy, true);
            _playerAttackBus.Connect(enemyDamageArea2D);
        }

        public void ConfigureEnemyAttackArea2D(Area2D enemyAttackArea2D) {
            if (enemyAttackArea2D.GetParent() is not IEnemy) throw new Exception("Only enemies can use this method");
            enemyAttackArea2D.CollisionMask = 0;
            enemyAttackArea2D.CollisionLayer = 0;
            enemyAttackArea2D.SetCollisionLayerBit(LayerEnemy, true);
            _playerDamageBus.Connect(enemyAttackArea2D);
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
            deathArea2D.OnAreaEntered((player) => Bus.Publish(PlayerEvent.Death));
        }

        public void ConfigureSceneChange(Area2D sceneChangeArea2D, string scene) {
            sceneChangeArea2D.CollisionLayer = 0;
            sceneChangeArea2D.CollisionMask = 0;
            sceneChangeArea2D.SetCollisionLayerBit(LayerPlayerStageDetector, true);
            sceneChangeArea2D.OnAreaEntered((player) => Game.QueueChangeSceneWithPlayer(scene));
        }
    }
}