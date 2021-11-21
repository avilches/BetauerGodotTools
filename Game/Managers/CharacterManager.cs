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

        public void ConfigureEnemyCollisions(KinematicBody2D enemy) {
            enemy.AddToGroup(GROUP_ENEMY);
            enemy.CollisionMask = 0;
            enemy.CollisionLayer = 0;
            PlatformManager.ConfigurePlayerCollisions(enemy);
            SlopeStairsManager.ConfigurePlayerCollisions(enemy);
        }

        public bool IsEnemy(KinematicBody2D platform) => platform.IsInGroup(GROUP_ENEMY);

        public void ConfigurePlayerAreaAttack(Area2D playerWeaponArea2D) {
            playerWeaponArea2D.CollisionMask = 0;
            playerWeaponArea2D.CollisionLayer = 0;
            playerWeaponArea2D.SetCollisionMaskBit(LayerEnemy, true);
            _playerAttackTopic.AddArea2D(playerWeaponArea2D);
        }

        // public bool IsPlayer(KinematicBody2D player) {
            // return PlayerController == player;
        // }

        public void PlayerEnteredDeathZone(Area2D deathArea2D) {
            PlayerController.DeathZone(deathArea2D);
        }

    }
}