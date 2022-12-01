using System;
using Betauer.Bus.Signal;
using Godot;
using Betauer.DI;
using Betauer.Core.Signal;
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

        private readonly Area2DInArea2DCollision _playerAttackCollision = new(LayerEnemy);

        public void RegisterPlayerController(PlayerController playerController) {
            PlayerController = playerController;
        }

        public void ConfigurePlayerCollisions(PlayerController playerController) {
            playerController.CollisionLayer = 0;
            playerController.CollisionMask = 0;
            
            PlatformManager.ConfigureCharacterCollisionsWithGroundAndPlatforms(playerController);
            playerController.FloorRaycasts.ForEach(PlatformManager.ConfigureCharacterCollisionsWithGroundAndPlatforms);
            
            SlopeStairsManager.ConfigureCharacterCollisionsWithSlopeStairs(playerController);
            
            playerController.PlayerDetector.CollisionLayer = 0;
            playerController.PlayerDetector.CollisionMask = 0;
            playerController.PlayerDetector.SetCollisionMaskValue(LayerPlayerStageDetector, true);
        }

        public void ConfigurePlayerAttackArea2D(Area2D attackArea2D) {
            attackArea2D.CollisionMask = 0;
            attackArea2D.CollisionLayer = 0;
            _playerAttackCollision.Detect(attackArea2D);
        }

        public void ConfigureEnemyCollisions(CharacterBody2D enemy) {
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

        public void ConfigureEnemyDamageArea2D(Area2D enemyDamageArea2D, Action<Area2D> onAttack) {
            if (enemyDamageArea2D.GetParent() is not IEnemy) throw new Exception("Only enemies can use this method");
            enemyDamageArea2D.CollisionMask = 0;
            enemyDamageArea2D.CollisionLayer = 0;
            _playerAttackCollision.OnArea2DEnteredIn(enemyDamageArea2D, onAttack);
        }

        public bool IsEnemy(CharacterBody2D platform) => platform.IsInGroup(GROUP_ENEMY);

        public bool IsPlayer(CharacterBody2D player) {
            return PlayerController == player;
        }

        public void ConfigurePlayerDeathZone(Area2D deathArea2D) {
            deathArea2D.CollisionLayer = 0;
            deathArea2D.CollisionMask = 0;
            // TODO: this should be a topic, so other places can subscribe like remove all bullets
            deathArea2D.SetCollisionLayerValue(LayerPlayerStageDetector, true);
            deathArea2D.OnAreaEntered((player) => Bus.Publish(PlayerEvent.Death));
        }

        public void ConfigureSceneChange(Area2D sceneChangeArea2D, string scene) {
            sceneChangeArea2D.CollisionLayer = 0;
            sceneChangeArea2D.CollisionMask = 0;
            sceneChangeArea2D.SetCollisionLayerValue(LayerPlayerStageDetector, true);
            sceneChangeArea2D.OnAreaEntered((player) => Game.QueueChangeSceneWithPlayer(scene));
        }
    }
}