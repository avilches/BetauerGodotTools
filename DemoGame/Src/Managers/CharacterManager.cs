using System;
using Betauer.Core.Nodes;
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
        [Inject] private Bus Bus { get; set; }

        public void RegisterPlayerController(PlayerController playerController) {
            PlayerController = playerController;
        }

        public void ConfigurePlayerCollisions(PlayerController playerController) {
            playerController.CollisionLayer = 0;
            playerController.CollisionMask = 0;
            
            playerController.DetectLayer(LayerRegularPlatform);
            playerController.DetectLayer(LayerFallPlatform);
            playerController.FloorRaycasts.ForEach(rayCast2D => {
                rayCast2D.DetectLayer(LayerRegularPlatform);
                rayCast2D.DetectLayer(LayerFallPlatform);
            });
            
            playerController.PlayerDetector.CollisionLayer = 0;
            playerController.PlayerDetector.CollisionMask = 0;
            playerController.PlayerDetector.DetectLayer(LayerPlayerStageDetector);
        }

        public void ConfigurePlayerAttackArea2D(Area2D attackArea2D) {
            attackArea2D.CollisionMask = 0;
            attackArea2D.CollisionLayer = 0;
            attackArea2D.AddToLayer(LayerEnemy);
        }

        public void ConfigureEnemyCollisions(CharacterBody2D enemy) {
            enemy.AddToGroup(GROUP_ENEMY);
            enemy.CollisionMask = 0;
            enemy.CollisionLayer = 0;                                       
            enemy.DetectLayer(LayerRegularPlatform);
            enemy.DetectLayer(LayerFallPlatform);
        }

        public void ConfigureEnemyCollisions(RayCast2D rayCast2D) {
            rayCast2D.CollisionMask = 0;
            rayCast2D.CollisionMask = 0;
            rayCast2D.DetectLayer(LayerRegularPlatform);
            rayCast2D.DetectLayer(LayerFallPlatform);
        }

        public void ConfigureEnemyDamageArea2D(Area2D enemyDamageArea2D, Action<Area2D> onAttack) {
            if (enemyDamageArea2D.GetParent() is not IEnemy) throw new Exception("Only enemies can use this method");
            enemyDamageArea2D.CollisionMask = 0;
            enemyDamageArea2D.CollisionLayer = 0;
            enemyDamageArea2D.OnAreaEntered(LayerEnemy, onAttack);
        }

        public bool IsEnemy(CharacterBody2D platform) => platform.IsInGroup(GROUP_ENEMY);

        public bool IsPlayer(CharacterBody2D player) {
            return PlayerController == player;
        }

        public void ConfigurePlayerDeathZone(Area2D deathArea2D) {
            deathArea2D.CollisionLayer = 0;
            deathArea2D.CollisionMask = 0;
            // TODO: this should be a topic, so other places can subscribe like remove all bullets
            deathArea2D.AddToLayer(LayerPlayerStageDetector);
            deathArea2D.OnAreaEntered((player) => Bus.Publish(PlayerEvent.Death));
        }

        public void ConfigureSceneChange(Area2D sceneChangeArea2D, string scene) {
            sceneChangeArea2D.CollisionLayer = 0;
            sceneChangeArea2D.CollisionMask = 0;
            sceneChangeArea2D.AddToLayer(LayerPlayerStageDetector);
            sceneChangeArea2D.OnAreaEntered((player) => Game.QueueChangeSceneWithPlayer(scene));
        }
    }
}