using System.Collections;
using System.Collections.Generic;
using Godot;
using Tools.Bus;
using Tools.Bus.Topics;
using Veronenger.Game.Character;
using static Veronenger.Game.Tools.LayerConstants;
using static Tools.GodotTools;

namespace Veronenger.Game.Managers {
    public class PlatformManager {
        private const string GROUP_PLATFORMS = "platform";
        private const string GROUP_MOVING_PLATFORMS = "moving_platform";
        private const string GROUP_FALLING_PLATFORMS = "falling_platform";

        /**
         * No almacena nada, solo permite que characters se suscriban a cambios en una plataforma
         * y permite cambiar sus mascaras.
         */

        // Configura el layer de la plataforma segun el tipo
        public List<PhysicsBody2D> ConfigurePlatformList(IList nodes, bool falling = false, bool moving = false) {
            var platforms = Filter<PhysicsBody2D>(nodes);
            platforms.ForEach(kb2d => ConfigurePlatform(kb2d, falling, moving));
            return platforms;
        }

        /**
         * Si una plataforma falling tiene un Area2D dentro, se tomará como una zona que vuelve a activar las plataformas falling
         * Es lo mismo que si a esa zona se le añade el script FallingPlatformExit
         */
        public void ConfigurePlatform(PhysicsBody2D platform, bool falling = false, bool moving = false) {
            var message = "PlatformManager.Platform " + (falling ? "falling" : "") + "/" + (moving ? "moving" : "") + ")";
            // platform.AddToGroup("platform");
            platform.CollisionMask = 0;
            platform.CollisionLayer = 0;
            platform.AddToGroup(GROUP_PLATFORMS);
            if (falling) {
                foreach (var child in platform.GetChildren())
                    if (child is CollisionShape2D colShape2d) {
                        colShape2d.OneWayCollision = true;
                    } else if (child is Area2D area2D) {
                        AddArea2DFallingPlatformExit(area2D);
                    }
                platform.AddToGroup(GROUP_FALLING_PLATFORMS);
                platform.SetCollisionLayerBit(FALL_PLATFORM_LAYER, true);
            } else {
                platform.SetCollisionLayerBit(REGULAR_PLATFORM_LAYER, true);
            }

            if (moving) {
                if (platform is KinematicBody2D kb2d) kb2d.Motion__syncToPhysics = true;
                platform.AddToGroup(GROUP_MOVING_PLATFORMS);
            }
        }

        public void ConfigureArea2DAsPlatform(Area2D upHall) {
            upHall.CollisionMask = 0;
            upHall.CollisionLayer = 0;
            upHall.SetCollisionLayerBit(REGULAR_PLATFORM_LAYER, true);
        }

        public void ConfigurePlayerCollisions(CharacterController kb2d) {
            kb2d.SetCollisionMaskBit(REGULAR_PLATFORM_LAYER, true);
            kb2d.SetCollisionMaskBit(FALL_PLATFORM_LAYER, true);
        }

        public bool IsMovingPlatform(KinematicBody2D platform) => platform.IsInGroup(GROUP_MOVING_PLATFORMS);
        public bool IsFallingPlatform(PhysicsBody2D platform) => platform.IsInGroup(GROUP_FALLING_PLATFORMS);


        /**
         * Platform falling & body out
         */
        // Provoca la caida del jugador desde la plataforma quitando la mascara
        public void BodyFallFromPlatform(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(FALL_PLATFORM_LAYER, false);
        public bool IsBodyFallingFromPlatform(KinematicBody2D kb2d) => !kb2d.GetCollisionMaskBit(FALL_PLATFORM_LAYER);
        // Para la caida del jugador

        public void BodyStopFallFromPlatform(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(FALL_PLATFORM_LAYER, true);
        private BodyOnArea2DTopic _platformBodyOutTopic = new BodyOnArea2DTopic("PlatformBodyOut");
        private void AddArea2DFallingPlatformExit(Area2D area2D) => _platformBodyOutTopic.AddArea2D(area2D);
        public void SubscribeFallingPlatformOut(GodotListener<BodyOnArea2D> enterListener) => _platformBodyOutTopic.Subscribe(enterListener);

    }
}