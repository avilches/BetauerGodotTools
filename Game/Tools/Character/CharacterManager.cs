using Veronenger.Game.Characters.Player;
using Veronenger.Game.Tools.Events;
using Godot;
using static Veronenger.Game.Tools.LayerConstants;
using static Veronenger.Game.Tools.GodotTools;

namespace Veronenger.Game.Tools.Character {
    public class CharacterManager : Node {
        private const string GROUP_ENEMY = "enemy";

        /**
         * No almacena nada, solo permite que enemigos y armas se suscriban a cambios
         */

        public void RegisterPlayer(PlayerController playerController) {
            Debug.Register("CharacterManager.Player", playerController);
            playerController.CollisionLayer = 0;
            playerController.CollisionMask = 0;
            playerController.SetCollisionLayerBit(PLAYER_LAYER, true);
            GameManager.Instance.PlatformManager.ConfigurePlayerCollisions(playerController);
        }

        public void RegisterEnemy(CharacterController enemy) {
            Debug.Register("CharacterManager.Enemy", enemy);
            enemy.AddToGroup(GROUP_ENEMY);
            enemy.CollisionMask = 0;
            enemy.CollisionLayer = 0;
            enemy.SetCollisionLayerBit(ENEMY_LAYER, true);

            GameManager.Instance.PlatformManager.ConfigurePlayerCollisions(enemy);
        }

        public bool IsEnemy(KinematicBody2D platform) => platform.IsInGroup(GROUP_ENEMY);

        // Thi
        public void RegisterPlayerWeapon(Area2D playerWeaponArea2D) {
            playerWeaponArea2D.CollisionMask = 0;
            playerWeaponArea2D.CollisionLayer = 0;
            ListenArea2DCollisionsWithBodies(playerWeaponArea2D, PlayerWeapon_BodyEntered); //
        }

        public void EnablePlayerWeapon(Area2D playerWeaponArea2D) => playerWeaponArea2D.SetCollisionMaskBit(ENEMY_LAYER, true);
        public void DisablePlayerWeapon(Area2D playerWeaponArea2D) => playerWeaponArea2D.SetCollisionMaskBit(ENEMY_LAYER, false);

        private GodotUnicastTopic<BodyOnArea2D> _playerWeapon_enterTopic = new GodotUnicastTopic<BodyOnArea2D>();
        // private GodotUnicastTopic<BodyOnArea2D> _playerWeapon_exitTopic = new GodotUnicastTopic<BodyOnArea2D>();

        void PlayerWeapon_BodyEntered(Node body, Area2D area2D) => _playerWeapon_enterTopic.Publish(new BodyOnArea2D(body, area2D));
        // void PlayerWeapon_BodyExited(Node body, Area2D area2D) => _playerWeapon_exitTopic.Publish(new BodyOnArea2D(body, area2D));

        // Enemies (or the player!) can subscribe to this event to check if an attack has been done to them
        public void SubscribePlayerWeaponCollision(NodeFromListenerDelegate<BodyOnArea2D> enterListener) {
            // NodeFromListenerDelegate<BodyOnArea2D> exitListener = null) {
            _playerWeapon_enterTopic.Subscribe(enterListener);
            // if (exitListener != null) _playerWeapon_exitTopic.Subscribe(exitListener);
        }

    }
}