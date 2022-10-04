using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using Betauer;
using Betauer.Bus.Signal;
using Betauer.DI;
using Betauer.Signal;
using static Veronenger.Game.LayerConstants;

namespace Veronenger.Game.Managers {
    [Service]
    public class PlatformManager {
        private const string GROUP_PLATFORMS = "platform";
        private const string GROUP_MOVING_PLATFORMS = "moving_platform";
        private const string GROUP_FALLING_PLATFORMS = "falling_platform";

        /**
         * No almacena nada, solo permite que characters se suscriban a cambios en una plataforma
         * y permite cambiar sus mascaras.
         */

        // Configura el layer de la plataforma segun el tipo
        public void ConfigurePlatformList(List<PhysicsBody2D> platforms, bool falling = false, bool moving = false) {
            platforms.ForEach(kb2d => ConfigurePlatform(kb2d, falling, moving));
        }

        /**
         * Si una plataforma falling tiene un Area2D dentro, se tomará como una zona que vuelve a activar las plataformas falling
         * Es lo mismo que si a esa zona se le añade el script FallingPlatformExit
         */
        public void ConfigurePlatform(PhysicsBody2D platform, bool falling = false, bool moving = false) {
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
                platform.SetCollisionLayerBit(LayerFallPlatform, true);
            } else {
                platform.SetCollisionLayerBit(LayerRegularPlatform, true);
            }

            if (moving) {
                if (platform is KinematicBody2D kb2d) kb2d.Motion__syncToPhysics = true;
                platform.AddToGroup(GROUP_MOVING_PLATFORMS);
            }
        }

        public void ConfigureArea2DAsPlatform(Area2D upHall) {
            upHall.CollisionMask = 0;
            upHall.CollisionLayer = 0;
            upHall.SetCollisionLayerBit(LayerRegularPlatform, true);
        }

        public void ConfigurePlayerCollisions(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(LayerRegularPlatform, true);
            kb2d.SetCollisionMaskBit(LayerFallPlatform, true);
        }

        public bool IsMovingPlatform(KinematicBody2D platform) => platform.IsInGroup(GROUP_MOVING_PLATFORMS);
        public bool IsFallingPlatform(PhysicsBody2D platform) => platform.IsInGroup(GROUP_FALLING_PLATFORMS);


        /**
         * Platform falling & body out
         */
        // Provoca la caida del jugador desde la plataforma quitando la mascara
        public void BodyFallFromPlatform(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(LayerFallPlatform, false);
        }

        public bool IsBodyFallingFromPlatform(KinematicBody2D kb2d) => !kb2d.GetCollisionMaskBit(LayerFallPlatform);
        // Para la caida del jugador

        public BodyOnArea2DEntered.Unicast PlatformBodyOutTopicBodyOnArea2D = new("PlatformBodyOut");
        
        public void BodyStopFallFromPlatform(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(LayerFallPlatform, true);
        }

        private void AddArea2DFallingPlatformExit(Area2D area2D) => PlatformBodyOutTopicBodyOnArea2D.Connect(area2D);

        public void SubscribeFallingPlatformOut(Node filter, Action<Area2D> action) =>
            PlatformBodyOutTopicBodyOnArea2D.Subscribe((area2D, _) => action(area2D)).WithFilter(filter);
    }
}