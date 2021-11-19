using Godot;
using Tools.Bus.Topics;
using Veronenger.Game.Character;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers.Autoload;
using static Veronenger.Game.Tools.LayerConstants;

namespace Veronenger.Game.Managers {
    public class CharacterManager {
        private const string GROUP_ENEMY = "enemy";

        private readonly Area2DOnArea2DTopic _playerAttackTopic = new Area2DOnArea2DTopic("PlayerAttack");

        /**
         * No almacena nada, solo permite que enemigos y armas se suscriban a cambios
         */

        public void ConfigurePlayerCollisions(Player2DPlatformController player2DPlatformController) {
            player2DPlatformController.CollisionLayer = 0;
            player2DPlatformController.CollisionMask = 0;
            GameManager.Instance.PlatformManager.ConfigurePlayerCollisions(player2DPlatformController);
            GameManager.Instance.SlopeStairsManager.ConfigurePlayerCollisions(player2DPlatformController);
        }

        public void ConfigureEnemyCollisions(Character2DPlatformController enemy) {
            enemy.AddToGroup(GROUP_ENEMY);
            enemy.CollisionMask = 0;
            enemy.CollisionLayer = 0;
            GameManager.Instance.PlatformManager.ConfigurePlayerCollisions(enemy);
            GameManager.Instance.SlopeStairsManager.ConfigurePlayerCollisions(enemy);
        }

        public bool IsEnemy(KinematicBody2D platform) => platform.IsInGroup(GROUP_ENEMY);

        public void ConfigurePlayerAreaAttack(Area2D playerWeaponArea2D) {
            playerWeaponArea2D.CollisionMask = 0;
            playerWeaponArea2D.CollisionLayer = 0;
            playerWeaponArea2D.SetCollisionMaskBit(LayerEnemy, true);
            _playerAttackTopic.AddArea2D(playerWeaponArea2D);
        }
    }
}