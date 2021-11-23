using System;
using Godot;
using Tools;
using Tools.Bus.Topics;
using Veronenger.Game.Controller.Character;
using static Veronenger.Game.Tools.LayerConstants;

namespace Veronenger.Game.Managers {

    [Singleton]
    public class CharacterManager {
        private const string GROUP_ENEMY = "enemy";

        public PlayerController PlayerController { get; private set; }

        [Inject] public PlatformManager PlatformManager;
        [Inject] public SlopeStairsManager SlopeStairsManager;

        private readonly Area2DOnArea2DTopic _playerAttackTopic = new Area2DOnArea2DTopic("PlayerAttack");

        public void RegisterPlayerController(PlayerController playerController) {
            PlayerController = playerController;
        }

        public void ConfigurePlayerCollisions(PlayerController playerController) {
            playerController.CollisionLayer = 0;
            playerController.CollisionMask = 0;
            PlatformManager.ConfigurePlayerCollisions(playerController);
            SlopeStairsManager.ConfigurePlayerCollisions(playerController);
        }

        public void ConfigurePlayerAttackArea2D(Area2D attackArea2D) {
            attackArea2D.CollisionMask = 0;
            attackArea2D.CollisionLayer = 0;
            attackArea2D.SetCollisionMaskBit(LayerEnemy, true);
            _playerAttackTopic.Subscribe("Player", attackArea2D, attackArea2D, _OnSlopeStairsEnablerEnter);
        }

        private void _OnSlopeStairsEnablerEnter(Area2DOnArea2D @event) {
            LoggerFactory.GetLogger(GetType()).RemoveDuplicates = false;
            LoggerFactory.GetLogger(GetType()).Debug("Collision from Origin:"+@event.Origin.GetParent().Name+"."+@event.Origin.Name+" / Detected:"+@event.Detected.GetParent().Name+"."+@event.Detected.Name);
        }

        // public void ConfigurePlayerDamageArea2D(Area2D damageArea2D) {
            // damageArea2D.CollisionMask = 0;
            // damageArea2D.CollisionLayer = 0;
            // damageArea2D.SetCollisionLayerBit(LayerEnemy, true);
            // _playerAttackTopic.AddArea2D(attackArea2D);
        // }

        public void ConfigureEnemyCollisions(KinematicBody2D enemy) {
            enemy.AddToGroup(GROUP_ENEMY);
            enemy.CollisionMask = 0;
            enemy.CollisionLayer = 0;
            PlatformManager.ConfigurePlayerCollisions(enemy);
            SlopeStairsManager.ConfigurePlayerCollisions(enemy);
        }

        // public void ConfigureEnemyAttackArea2D(Area2D attackArea2D) {
            // attackArea2D.CollisionMask = 0;
            // attackArea2D.CollisionLayer = 0;
            // attackArea2D.SetCollisionMaskBit(LayerPlayer, true);
            // _playerAttackTopic.AddArea2D(attackArea2D);
        // }

        public void ConfigureEnemyDamageArea2D(Area2D damageArea2D) {
            damageArea2D.CollisionMask = 0;
            damageArea2D.CollisionLayer = 0;
            damageArea2D.SetCollisionLayerBit(LayerEnemy, true);
            _playerAttackTopic.AddArea2D(damageArea2D);
        }

        public bool IsEnemy(KinematicBody2D platform) => platform.IsInGroup(GROUP_ENEMY);

        // public bool IsPlayer(KinematicBody2D player) {
            // return PlayerController == player;
        // }

        public void PlayerEnteredDeathZone(Area2D deathArea2D) {
            PlayerController.DeathZone(deathArea2D);
        }

    }
}